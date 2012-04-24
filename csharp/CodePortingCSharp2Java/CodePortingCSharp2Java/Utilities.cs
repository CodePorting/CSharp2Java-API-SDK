/*--------------------------------------------------------------------------------------+
|
|   $HeadURL: svn://sialkot.codeporting.com/cs-porter/trunk/MarketPlace/CSharp2Java-API-SDK/csharp/CodePortingCSharp2Java/CodePortingCSharp2Java/Utilities.cs $
|   $Id: Utilities.cs muhammad.iqbal $
|   $Revision: 1 $
|   $Date: (Fri, 24 Apr 2012) $
|   $Author: muhammad.iqbal $
|
|   $Copyright: (c) 2001-2012 Aspose Pty Ltd. All rights reserved. $
|   
|   This program is confidential, proprietary and unpublished property of Aspose Pty
|   Ltd. It may NOT be copied in part or in whole on any medium, either electronic or
|   printed, without the express written consent of Aspose Pty Ltd.This program is 
|   developed and maintained by CodePorting venture [http://codeporting.com ] a division of Aspose Pty Ltd.
|
+--------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using ICSharpCode.SharpZipLib.Core;

namespace CodePortingCSharp2Java
{
    /// <summary>
    /// Utilities class to perform actions on CodePorting API.
    /// </summary>
    public class Utilities
    {
        /// <summary>
        /// CodePorting API base URL.
        /// </summary>
        public const string baseURL = "https://apps.codeporting.com";

        /// <summary>
        /// CodePorting API port code snippet API path.
        /// </summary>
        public const string snippetPortCommand = "/csharp2java/v0/portsnippet";

        /// <summary>
        /// CodePorting API create new project API path.
        /// </summary>
        public const string uploadZipCommand = "/csharp2java/v0/newproject";

        /// <summary>
        /// CodePorting API port project API path.
        /// </summary>
        public const string convertProjectCommand = "/csharp2java/v0/portproject";

        /// <summary>
        /// CodePorting API download ported file API path.
        /// </summary>
        public const string downloadPortedProjectCommand = "/csharp2java/v0/downloadportedfile";

        /// <summary>
        /// CodePorting API sign in API path.
        /// </summary>
        public const string signInCommand = "/csharp2java/v0/UserSignin";


        /// <summary>
        /// Port CSharp source string to java string.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="sourceCode"></param>
        /// <param name="error"></param>
        /// <param name="convertedCode"></param>
        /// <returns></returns>
        public bool PortSingleFile(string token, string sourceCode, ref string error, ref string convertedCode)
        {
            try
            {
                error = "";
                bool result = true;
                NameValueCollection parameters = new NameValueCollection();
                parameters.Add("token", SigninToken.Token);
                parameters.Add("SourceCode", sourceCode);
                CommandHelper helper = new CommandHelper();
                string param = helper.PackCommandXML(parameters, true);
                string statusCode = string.Empty;
                string response = helper.ExecuteAPIMethod(baseURL + snippetPortCommand, param, ref statusCode);
                if (string.IsNullOrEmpty(response))
                {
                    error = "Unable to connect to CodePorting.";
                    return false;
                }
                Dictionary<string, string> dataDictionary = new Dictionary<string, string>();
                dataDictionary = helper.parseResponse(response);

                //if (statusCode != "200")
                //{
                //    error = dataDictionary["error"];
                //    result = false;
                //}
                if (dataDictionary["success"].ToLower() == "false")
                {
                    error = dataDictionary["error"];
                    result = false;
                }

                convertedCode = dataDictionary["TargetCode"];
                return result;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return false;
        }

        /// <summary>
        /// Upload zipped CSharp project and add it as new project on CodePorting.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="zipFilePath"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool UploadProjectZip(string token, string zipFilePath, ref string error)
        {
           
            try
            {
                NameValueCollection parameters = new NameValueCollection();
                parameters.Add("token", token);
                CommandHelper helper = new CommandHelper();
                string param = helper.PackCommandXML(parameters, true);
                string statusCode = string.Empty;
                string response = string.Empty;
                string[] fileNames = new string[] { zipFilePath };

                helper.UploadFilesToRemoteUrl(baseURL + uploadZipCommand, fileNames, parameters);
               
            }
            catch (Exception ex)
            {
                error = "Error while uploading project";
            }
            return true;
        }

        /// <summary>
        /// Convert a CSharp project to Java (perform porting operation) for a project already present on codeporting.com.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="projectName"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool ConvertProject(string token, string projectName, ref string  error)
        {
            try
            {
                NameValueCollection parameters = new NameValueCollection();
                parameters.Add("token", token);
                parameters.Add("ProjectName", projectName);
                CommandHelper helper = new CommandHelper();
                string param = helper.PackCommandXML(parameters, true);
                string statusCode = string.Empty;
                string response = helper.ExecuteAPIMethod(baseURL + convertProjectCommand, param, ref statusCode);
                if (string.IsNullOrEmpty(response))
                {
                    error = "Error while conversion.";
                    return false;
                }
                Dictionary<string, string> dataDictionary = new Dictionary<string, string>();
                string get_error = string.Empty;
                string get_success = string.Empty;

                dataDictionary = helper.parseResponse(response, out get_success, out get_error);

                if (get_success.ToLower() == "false")
                {
                    error = get_error;
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                error = "Error while conversion.";
                return false;
            }

            return false;
        }

        /// <summary>
        /// Download ported project as a zip from codeporting.com.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="projectName"></param>
        /// <param name="fileName"></param>
        /// <param name="portedProjectsFolder"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool DownloadportedZipFile(string token,string projectName, string fileName, string portedProjectsFolder, ref string error)
        {
            //comboBoxListPortedFiles.Items.Clear();
            try
            {
                NameValueCollection parameters = new NameValueCollection();
                parameters.Add("token", SigninToken.Token);
                parameters.Add("ProjectName", projectName);
                parameters.Add("FileName", fileName);
                CommandHelper helper = new CommandHelper();
                string param = helper.PackCommandXML(parameters, true);
                string statusCode = string.Empty;
                string response = helper.ExecuteAPIMethodDownloadFile(baseURL + downloadPortedProjectCommand, param, portedProjectsFolder, ref statusCode);
                

                return true;
            }
            catch (Exception ex)
            {
                error = "Error while download ported project.";
                return false;
            }

            return true;
        }

        #region file compression methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceFolder"></param>
        /// <param name="targetZipFilePath"></param>
        /// <returns></returns>
        public bool CompressFolder(string sourceFolder, string targetZipFilePath)
        {
            if (!Directory.Exists(sourceFolder))
                return false;
            try
            {
                Stream stream = new FileStream(targetZipFilePath, FileMode.Create);
                ZipOutputStream zipStream = new ZipOutputStream(stream);

                int folderOffset = sourceFolder.Length + (sourceFolder.EndsWith("\\") ? 0 : 1);
                CompressFolder(sourceFolder, zipStream, folderOffset);
                zipStream.IsStreamOwner = true;	// Makes the Close also Close the underlying stream
                zipStream.Close();
                return true;
            }
            catch (Exception ex)
            {

                //logg error
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="zipStream"></param>
        /// <param name="folderOffset"></param>
        private void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset)
        {
            string[] files = Directory.GetFiles(path);

            foreach (string filename in files)
            {
                string fileExtension = Path.GetExtension(filename).ToLower();
                if (!(fileExtension == ".cs" || fileExtension == ".sln" || fileExtension == ".csproj"))
                    continue;
                
                FileInfo fi = new FileInfo(filename);

                string entryName = filename.Substring(folderOffset); // Makes the name in zip based on the folder
                entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
                ZipEntry newEntry = new ZipEntry(entryName);
                newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity

                // Specifying the AESKeySize triggers AES encryption. Allowable values are 0 (off), 128 or 256.
                //   newEntry.AESKeySize = 256;

                // To permit the zip to be unpacked by built-in extractor in WinXP and Server2003, WinZip 8, Java, and other older code,
                // you need to do one of the following: Specify UseZip64.Off, or set the Size.
                // If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility, you do not need either,
                // but the zip will be in Zip64 format which not all utilities can understand.
                //   zipStream.UseZip64 = UseZip64.Off;
                newEntry.Size = fi.Length;

                zipStream.PutNextEntry(newEntry);

                // Zip the file in buffered chunks
                // the "using" will close the stream even if an exception occurs
                byte[] buffer = new byte[4096];
                using (FileStream streamReader = File.OpenRead(filename))
                {
                    StreamUtils.Copy(streamReader, zipStream, buffer);
                }
                zipStream.CloseEntry();
            }
            string[] folders = Directory.GetDirectories(path);
            foreach (string folder in folders)
            {
                CompressFolder(folder, zipStream, folderOffset);
            }
        }

        public string Signin(string userName, string password, ref string error)
        {
            //comboBoxListPortedFiles.Items.Clear();
            try
            {
                NameValueCollection parameters = new NameValueCollection();
                //parameters.Add("token", "5EF931D5-24D9-4640-BB21-112641312AB6");
                parameters.Add("LoginName", userName);
                parameters.Add("Password", password);
                string get_success = string.Empty;
                CommandHelper helper = new CommandHelper();
                string param = helper.PackCommandXML(parameters, true);
                string statusCode = string.Empty;
                string response = helper.ExecuteAPIMethod(baseURL + signInCommand, param, ref statusCode);

                if (string.IsNullOrEmpty(response))
                    return string.Empty;
                Dictionary<string, string> dataDictionary = new Dictionary<string, string>();
                string get_error = string.Empty;
                
                
                dataDictionary = helper.parseResponse(response, out get_success, out get_error);

                 return dataDictionary["Token"];

                
            }
            catch (Exception ex)
            {
                error = "Error while download ported project.";
                return string.Empty;
            }

            return string.Empty;
        }

        /// <summary>
        /// Unzip files
        /// </summary>
        /// <param name="zipFilePath"></param>
        /// <param name="pathToExtract"></param>
        public Boolean ExtractZipFile(string zipFilePath, string pathToExtract)
        {
            try
            {
                if (!Directory.Exists(pathToExtract))
                {
                    Directory.CreateDirectory(pathToExtract);
                }
                FileStream fr = File.OpenRead(zipFilePath);
                ZipInputStream ins = new ZipInputStream(fr);
                //ZipFile zf = new ZipFile(zipFile);
                ZipEntry ze = ins.GetNextEntry();
                while (ze != null)
                {
                    if (ze.IsDirectory)
                    {
                        Directory.CreateDirectory(pathToExtract + "\\" + ze.Name);
                    }
                    else if (ze.IsFile)
                    {
                        if (!Directory.Exists(pathToExtract + Path.GetDirectoryName(ze.Name)))
                        {
                            Directory.CreateDirectory(Path.Combine(pathToExtract, Path.GetDirectoryName(ze.Name)));
                        }

                        FileStream fs = File.Create(pathToExtract + "\\" + ze.Name);

                        byte[] writeData = new byte[ze.Size];
                        int iteration = 0;
                        while (true)
                        {
                            int size = 2048;
                            size = ins.Read(writeData, (int)Math.Min(ze.Size, (iteration * 2048)), (int)Math.Min(ze.Size - (int)Math.Min(ze.Size, (iteration * 2048)), 2048));
                            if (size > 0)
                            {
                                fs.Write(writeData, (int)Math.Min(ze.Size, (iteration * 2048)), size);
                            }
                            else
                            {
                                break;
                            }
                            iteration++;
                        }
                        fs.Close();
                    }
                    ze = ins.GetNextEntry();
                }
                ins.Close();
                fr.Close();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}