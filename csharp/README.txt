CodePortingAPIUseWithCSharp
===========================

Describes how to use CodePorting API from CSharp code

The sample code enables us to verify the access of CodePorting API with CSharp code.

Here are the steps that should be followed to view the functionality:

1)- First of all you have to download 'ICSharpCode.SharpZipLib' from 
    http://www.icsharpcode.net/opensource/sharpziplib/ and be able to use it into your project.

2)- You should also be able to include a reference to 'System.Web' in your project.

3)- Create an object for Utilities in you app and you should be able to access these methods:
    - Port CSharp source string to java string (public bool PortSingleFile(string, string, ref string, ref string)).
	- Upload zipped CSharp project and add it as new project on CodePorting
	  (public bool UploadProjectZip(string, string, ref string)).
    - Convert a CSharp project to Java (perform porting operation) for a project already present on codeporting.com
	  (public bool ConvertProject(string, string, ref string))
    - Download ported project as a zip from codeporting.com
	  (public bool DownloadportedZipFile(string, string, string, string, ref string)).

4)- You can always refer to "http://codeporting.com/wiki/display/csharp2java/API+Reference" for 
    further details of CodePorting API.