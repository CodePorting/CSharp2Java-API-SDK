CodePortingAPIUseWithRuby
=========================

Describes how to use CodePorting API from ruby code

The sample code enables us to verify the access of CodePorting API with ruby code. The 
inputs are static and must be changed in the code if some other repository code or code 
porting account is to be used with the sample.

Here are the steps that should be followed to view the functionality:

1)- Download and install ruby distribution and set up development environment from 
"http://www.ruby-lang.org/en/downloads/". RubyInstaller is recommended but you can 
use any way mentioned there.

2)- Download or clone the github repository from "https://github.com/CodePorting/CodePortingAPIUseWithRuby"

3)- Open "CodePorting-C#2Java.rb" from CodePortingAPIUseWithRuby using any text editor 
say NotePad++ and provide fllowing parameters and save the file:
			
		- $project_name = you project name.
			
		- $repo_key = 	source repository name in github, this is the repository where C# 
						code resides and you want it to be ported to Java.
			
		- $target_repo_key = target repository name where you want to place/upload the ported java code.
			
		- $username = Username of you CodePorting account.
			
		- $password = Password of you CodePorting account.
			
		- $active = Always 1, indicates the validity of service hook when deployed on GitHub.
			
		- $userid = Your GitHub account Username.
		
4)- Open command prompt and go to CodePortingAPIUseWithRuby directory.

5)- Start compilation of ruby source there using following command
        ruby CodePorting-C#2Java.rb

6)- If all the parameters provided are valid you will see something like this:
		
		Sending login call
		
		Token is : 8f8112d5-50b5-4c10-83e9-1e1d7c33f848
		
		Now adding new project to code porting
		
		PORTING REPO TO JAVA now ......
		
		PORTING done = True
	
	If so, BINGO! your project has been ported and placed on your codeporting.com account after porting.
	
7)- If you are facing any issue, try going through the process once again, if you still face issues, 
	please ask for help on github wiki.