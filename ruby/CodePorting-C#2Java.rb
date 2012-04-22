require 'openssl'
require 'digest/sha1'
require 'openssl/cipher'
require 'digest/md5'
require 'digest'
require 'base64'
require 'digest/hmac'
require 'hex_string'
require 'net/http'
require 'net/https'
require 'uri'
require 'open-uri'
require 'rexml/document'

#####################################################################################################
#  Modules to aid sending of multipart form data to CodePorting.
#####################################################################################################
module FileUploadIO
  def self.new(file_path, content_type)
    raise ArgumentError, "File content type required" unless content_type
    file_io = File.open(file_path, "rb")
    file_io.instance_eval(<<-EOS, __FILE__, __LINE__)
      def content_type
        "#{content_type}"
      end

      def file_name
        "#{File.basename(file_path)}"
      end

      def file_size
        "#{File.size(file_path)}".to_i
      end
    EOS
    file_io
  end
end

module Parts
  class StreamParam
    def initialize(stream, size)
      @stream, @size = stream, size
    end
    
    def size
      @size
    end

    def read(offset, how_much)
      @stream.read(how_much)
    end
  end

  class StringParam
    def initialize (str)
      @str = str
    end

    def size
      @str.length
    end

    def read (offset, how_much)
      @str[offset, how_much]
    end
  end
end

module Stream
  class MultiPart
    def initialize(parts)
      @parts = parts
      @part_no = 0
      @part_offset = 0
    end
  
    def size
      total = 0
      @parts.each do |part|
        total += part.size
      end
      total
    end
  
    def read (how_much)
      return nil if @part_no >= @parts.size
  
      how_much_current_part = @parts[@part_no].size - @part_offset
  
      how_much_current_part = if how_much_current_part > how_much
        how_much
      else
        how_much_current_part
      end
  
      how_much_next_part = how_much - how_much_current_part
      current_part = @parts[@part_no].read(@part_offset, how_much_current_part )
  
      if how_much_next_part > 0
        @part_no += 1
        @part_offset = 0
        next_part = read( how_much_next_part  )
        current_part + if next_part
          next_part
        else
          ''
        end
      else
        @part_offset += how_much_current_part
        current_part
      end
    end
  end
end

module MultiPart
  class Post
    def initialize(post_params, request_headers={})
      @parts, @streams = [], []
      construct_post_params(post_params)
      @request_headers = request_headers
    end

    def construct_post_params(post_params)
      post_params.each_pair do |key, val|
        if(val.respond_to?(:content_type)) #construct file part
          @parts << Parts::StringParam.new( "--" + multi_part_boundary + "\r\n" + \
            "Content-Disposition: form-data; name=\"#{key}\"; filename=\"#{val.file_name}\"\r\n" + \
            "Content-Type: #{val.content_type}\r\n\r\n"
          )
          @streams << val
          @parts << Parts::StreamParam.new(val, val.file_size)
        else #construct string part param
          @parts << Parts::StringParam.new("--#{multi_part_boundary}\r\n" + "Content-Disposition: form-data; name=\"#{key}\"\r\n" + "\r\n" + "#{val}\r\n")
        end
      end
      @parts << Parts::StringParam.new( "\r\n--" + multi_part_boundary + "--\r\n" )
    end

    def multi_part_boundary
      @boundary ||= '----RubyMultiPart' + rand(1000000).to_s + 'ZZZZZ'
    end

    def submit(to_url, query_string=nil)
      post_stream = Stream::MultiPart.new(@parts)
      url = URI.parse( to_url )
      post_url_with_query_string = "#{url.path}"
      post_url_with_query_string = "#{post_url_with_query_string}?#{query_string}" unless(query_string.nil?)
      req = Net::HTTP::Post.new(post_url_with_query_string, @request_headers)
      req.content_length = post_stream.size
      req.content_type = 'multipart/form-data; boundary=' + multi_part_boundary
      req.body_stream = post_stream
      http_handle = Net::HTTP.new(url.host, url.port)
      http_handle.use_ssl = true if(url.scheme == "https")
	  http_handle.verify_mode = OpenSSL::SSL::VERIFY_NONE if(url.scheme == "https")
      res = http_handle.start {|http| http.request(req)}

      #close all the open hooks to the file on file-system
      @streams.each do |local_stream|
        local_stream.close();
      end
      res
    end
  end
end
#####################################################################################################
  
  $md5Key = OpenSSL::Digest::MD5.hexdigest('Code123!@#$$$$Porting')

  #####################################################################################################
  #  List of vaiables reuired for the ruby code to work.
  #  set values to suit your requirements, here are the details
  # - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  # - $project_name is the project name that you are going to use.
  # - $repo_key is the github C# source repository name, that you want to be ported to java.
  # - $target_repo_key is the hgithub java source repository, where you want to place the ported code.
  # - $username is the CodePorting username using which porting is to be performed.
  # - $password is the CodePorting password.
  # - $active should be 1 always.
  # - $userid is the GituHub user name of the account you are going to use.
  # - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  # - Here is the list of original parameters as received from the hithub-serivce-hook
  #   string   :project_name, :repo_key, :target_repo_key, :username, :password, :userid
  #   boolean  :active
  #####################################################################################################
  String $project_name = "TEST-PROJECT"
  $repo_key = "CSharpSourceProject"
  $target_repo_key = "TestJava"
  $username = "bali2u"
  $password = "pakistan"
  $active = 1
  $userid = "CodePorting"

  #####################################################################################################
  #  Function to get the redirection url if it is encountered during a web call
  #####################################################################################################
  def redirect_url (response)
	if response.nil?
		return
	end
	if response == ''
		return
	end
    if response['location'].nil?
      response.body.match(/<a href=\"([^>]+)\">/i)[1]
    else
      response['location']
    end
  end

  # Path of the source repository downloadable compressed content.
  path = "https://github.com/#{$userid}/#{$repo_key}/zipball/master"
  #path = "https://github.com/CodePorting/Test/zipball/master"
  
  # File where the downloaded repo content is to be placed/saved.
  download_file = open("#{$repo_key}.zip", "wb")

  #####################################################################################################
  #  Block of code to ddownload source repository from GitHub.
  #####################################################################################################
  begin
	resp = ''
	begin
		url = URI.parse(path)
		request = Net::HTTP.new(url.host, url.port)
		request.use_ssl = true
		request.verify_mode = OpenSSL::SSL::VERIFY_NONE
		resp = request.request_get(url.path)
		
		if resp.kind_of?(Net::HTTPRedirection)
			path = redirect_url(resp)
		end
	end while resp.kind_of?(Net::HTTPRedirection)

	download_file.write(resp.body)
  end
  download_file.close
  
  #$data = $password.unpack('H*').first.gsub(/(..)/,'\1').rstrip

  #####################################################################################################
  #  Function to check validity of input data
  #####################################################################################################
  def check_configuration_options
    if ($project_name == "")
	  raise 'Project name must be set' 
	end
	if ($repo_key == "")
      raise 'Repository is required'
    end
	if ($target_repo_key == "")
      raise 'Target repository is required'
	end
	if ($username == "")
      raise 'Codeporting username must be provided'
	end
	if ($password == "")
      raise 'Codeporting password must be provided'
	end
  end
  
  #####################################################################################################
  #  Function to start process when something is pushed to github repository, 
  #  start point for whole process
  #####################################################################################################
  def receive_push
    check_configuration_options
	perform_login_on_codeporting
  end

  #####################################################################################################
  #  Port repository code to java on code porting server
  #####################################################################################################
  def port_code
	puts "PORTING REPO TO JAVA now ......"
	uri = URI.parse("https://apps.codeporting.com")
	http_porting = Net::HTTP.new(uri.host, uri.port)
	http_porting.use_ssl = true
	http_porting.verify_mode = OpenSSL::SSL::VERIFY_NONE
	path_porting = '/csharp2java/v0/portproject'
	#data_porting = 'token=8f8112d5-50b5-4c10-83e9-1e1d7c33f848&ProjectName=CS'
	data_porting = "token=#{$token}&ProjectName=#{$repo_key}"
	headers_porting = {
		'Content-Type' => 'application/x-www-form-urlencoded'
	}
	resp, data = http_porting.post(path_porting, data_porting, headers_porting)

	doc = REXML::Document.new(data)
	retValue = ""
	doc.each_element('//return') { |item| 
		retValue = item.attributes['success']
		if (retValue == 'True')
			puts "PORTING done = #{retValue}"
		end
	}
	
  end

  #####################################################################################################
  #  Upload fetched repository data to code porting
  #####################################################################################################
  def upload_code_to_codeporting
    puts "Now adding new project to code porting"

	to_url = "https://apps.codeporting.com/csharp2java/v0/newproject"
	boundary = "------------------------------8ceeb9a4afbe0e1"
	#$token = "8f8112d5-50b5-4c10-83e9-1e1d7c33f848"
	params = {
		"token" => "#{$token}",
		"Test" => FileUploadIO.new("#{$repo_key}.zip", "application/zip")
	}   
	multipart_post = MultiPart::Post.new(params) # MultiPart::Post.new(params, some_extra_headers)
	res, dat = multipart_post.submit("https://apps.codeporting.com/csharp2java/v0/newproject") # multipart_post.submit(to_url, query_string)
	#puts res

	doc = REXML::Document.new(dat)
	retVal = ""
	doc.each_element('//return') { |item| 
		retVal = item.attributes['success']
	}

	if (retVal == "True")
		port_code
	end
  end
  
  #####################################################################################################
  #  perform login on code porting server.
  #####################################################################################################
  def perform_login_on_codeporting
	
	puts "Sending login call"
	#WORKING
	uri = URI.parse("https://apps.codeporting.com")
	http = Net::HTTP.new(uri.host, uri.port)
	http.use_ssl = true
	http.verify_mode = OpenSSL::SSL::VERIFY_NONE
	path = '/csharp2java/v0/UserSignin'
	data = "LoginName=#{$username}&Password=#{$password}"
	headers = {
		'Content-Type' => 'application/x-www-form-urlencoded'
	}
	resp, data = http.post(path, data, headers)

	doc = REXML::Document.new(data)

	retValue = ""
	doc.each_element('//return') { |item| 
		retValue = item.attributes['success']
	}

	$token = ''
	if (retValue == "True")
		doc.each_element('//Token') { |item| 
		$token = item.text
		puts $token
		upload_code_to_codeporting
	}
	end
  end
  
  receive_push
