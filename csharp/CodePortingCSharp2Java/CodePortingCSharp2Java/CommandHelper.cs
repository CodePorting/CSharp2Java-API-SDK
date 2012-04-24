/*--------------------------------------------------------------------------------------+
|
|   $HeadURL: svn://sialkot.codeporting.com/cs-porter/trunk/MarketPlace/CSharp2Java-API-SDK/csharp/CodePortingCSharp2Java/CodePortingCSharp2Java/CommandHelper.cs $
|   $Id: CommandHelper.cs muhammad.iqbal $
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
using System.Xml;
using System.Collections.Specialized;
using System.Web;
using System.Net;
using System.IO;

namespace CodePortingCSharp2Java
{
    /// <summary>
    /// Helper class
    /// </summary>
    public class CommandHelper
    {
        /// <summary>
        /// Pack data to create command
        /// </summary>
        /// <param name="paramters"></param>
        /// <returns></returns>
        public string PackCommandXML(NameValueCollection parameters, bool post)
        {
            if (post)
                return string.Join("&", Array.ConvertAll(parameters.AllKeys, key => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(parameters[key]))));
            else
                return "?" + string.Join("&", Array.ConvertAll(parameters.AllKeys, key => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(parameters[key]))));

            //return command;
        }

        public string ExecuteAPIMethod(string formattedUri, ref string statusCode)
        {
            string response = string.Empty;
            try
            {
                HttpWebRequest webRequest = GetWebRequest(formattedUri);
                webRequest.Method = "Post";
                HttpWebResponse responseHttp = (HttpWebResponse)webRequest.GetResponse();
                statusCode = responseHttp.StatusCode.ToString();
                using (StreamReader sr = new StreamReader(responseHttp.GetResponseStream()))
                {
                    response = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                statusCode = "400";
            }

            return response;
        }
        private HttpWebRequest GetWebRequest(string formattedUri)
        {
            Uri serviceUri = new Uri(formattedUri, UriKind.Absolute);
            return (HttpWebRequest)System.Net.WebRequest.Create(serviceUri);

        }

        public string ExecuteAPIMethod(string formattedUri, string paramaters, ref string statusCode)
        {
            string response = string.Empty;
            try
            {
                Uri serviceUri = new Uri(formattedUri, UriKind.Absolute);
                HttpWebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(serviceUri);
                //HttpWebRequest webRequest = GetWebRequest(formattedUri);

                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.Method = "POST";
                webRequest.Timeout = 50000;
                webRequest.KeepAlive = true;
                byte[] bytes = Encoding.ASCII.GetBytes(paramaters);
                Stream os = null;
                try
                { // send the Post
                    webRequest.ContentLength = bytes.Length;   //Count bytes to send
                    os = webRequest.GetRequestStream();
                    os.Write(bytes, 0, bytes.Length);         //Send it
                }
                catch (WebException ex)
                {
                    //MessageBox.Show(ex.Message, "HttpPost: Request error",
                    //MessageBoxButtons.OK, MessageBoxIcon.Error);
                    statusCode = "400";
                }
                finally
                {
                    if (os != null)
                    {
                        os.Close();
                    }
                }

                HttpWebResponse responseHttp = (HttpWebResponse)webRequest.GetResponse();
                statusCode = responseHttp.StatusCode.ToString();
                using (StreamReader sr = new StreamReader(responseHttp.GetResponseStream()))
                {
                    response = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                statusCode = "400";
            }

            return response;
        }

        public string ExecuteAPIMethodDownloadFile(string formattedUri, string paramaters, string portedProjectsFolder, ref string statusCode)
        {
            string response = string.Empty;
            try
            {
                Uri serviceUri = new Uri(formattedUri, UriKind.Absolute);
                HttpWebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(serviceUri);
                //HttpWebRequest webRequest = GetWebRequest(formattedUri);

                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.Method = "POST";
                byte[] bytes = Encoding.ASCII.GetBytes(paramaters);
                Stream os = null;
                try
                { // send the Post
                    webRequest.ContentLength = bytes.Length;   //Count bytes to send
                    os = webRequest.GetRequestStream();
                    os.Write(bytes, 0, bytes.Length);         //Send it
                }
                catch (WebException ex)
                {
                    //MessageBox.Show(ex.Message, "HttpPost: Request error",
                    //MessageBoxButtons.OK, MessageBoxIcon.Error);
                    statusCode = "400";
                }
                finally
                {
                    if (os != null)
                    {
                        os.Close();
                    }
                }
                
                HttpWebResponse responseHttp = (HttpWebResponse)webRequest.GetResponse();
                statusCode = responseHttp.StatusCode.ToString();

                byte[] buffer = new byte[4096];

                string fileName = responseHttp.Headers.GetValues(0)[0].Split('=')[1];
                using (Stream responseStream = responseHttp.GetResponseStream())
                {
                    FileStream writeStream = new FileStream(portedProjectsFolder, FileMode.Create);
                    BinaryWriter writeBinay = new BinaryWriter(writeStream);

                        int count = 0;
                        do
                        {
                            count = responseStream.Read(buffer, 0, buffer.Length);
                            writeBinay.Write(buffer);
                        } while (count != 0);

                    writeBinay.Close();
                }


                return response;

            }
            catch (Exception ex)
            {
                statusCode = "400";
            }

            return response;
        }

        public string ExecuteAPIMethod(string formattedUri, string paramaters, ref string statusCode, string fileName)
        {
            string response = string.Empty;
            try
            {
                Uri serviceUri = new Uri(formattedUri, UriKind.Absolute);
                HttpWebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(serviceUri);
                //HttpWebRequest webRequest = GetWebRequest(formattedUri);
                ///webRequest.Proxy = "dfs";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.Method = "POST";
                byte[] bytes = Encoding.ASCII.GetBytes(paramaters);
                Stream os = null;
                try
                { // send the Post
                    webRequest.ContentLength = bytes.Length;   //Count bytes to send
                    os = webRequest.GetRequestStream();
                    os.Write(bytes, 0, bytes.Length);         //Send it

                    byte[] fileC = File.ReadAllBytes(fileName);
                    os.Write(fileC, 0, fileC.Length);
                }
                catch (WebException ex)
                {
                    //MessageBox.Show(ex.Message, "HttpPost: Request error",
                    //MessageBoxButtons.OK, MessageBoxIcon.Error);
                    statusCode = "400";
                }
                finally
                {
                    if (os != null)
                    {
                        os.Close();
                    }
                }

                HttpWebResponse responseHttp = (HttpWebResponse)webRequest.GetResponse();
                statusCode = responseHttp.StatusCode.ToString();
                using (StreamReader sr = new StreamReader(responseHttp.GetResponseStream()))
                {
                    response = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                statusCode = "400";
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="files"></param>
        /// <param name="nvc"></param>
        public void UploadFilesToRemoteUrl(string url, string[] files, NameValueCollection nvc)
        {

            long length = 0;
            string boundary = "----------------------------" +
            DateTime.Now.Ticks.ToString("x");


            HttpWebRequest httpWebRequest2 = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest2.ContentType = "multipart/form-data; boundary=" +
            boundary;
            httpWebRequest2.Method = "POST";
            httpWebRequest2.KeepAlive = true;
            httpWebRequest2.Credentials =
            System.Net.CredentialCache.DefaultCredentials;



            Stream memStream = new System.IO.MemoryStream();

            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" +
            boundary + "\r\n");


            string formdataTemplate = "\r\n--" + boundary +
            "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";
            if (null != nvc)
                foreach (string key in nvc.Keys)
                {
                    string formitem = string.Format(formdataTemplate, key, nvc[key]);
                    byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                    memStream.Write(formitembytes, 0, formitembytes.Length);
                }


            memStream.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n Content-Type: application/octet-stream\r\n\r\n";

            for (int i = 0; i < files.Length; i++)
            {

                //string header = string.Format(headerTemplate, "file" + i, files[i]);
                string header = string.Format(headerTemplate, "uplTheFile", files[i]);

                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);

                memStream.Write(headerbytes, 0, headerbytes.Length);


                FileStream fileStream = new FileStream(files[i], FileMode.Open,
                FileAccess.Read);
                byte[] buffer = new byte[1024];

                int bytesRead = 0;

                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    memStream.Write(buffer, 0, bytesRead);

                }


                memStream.Write(boundarybytes, 0, boundarybytes.Length);


                fileStream.Close();
            }

            httpWebRequest2.ContentLength = memStream.Length;

            Stream requestStream = httpWebRequest2.GetRequestStream();

            memStream.Position = 0;
            byte[] tempBuffer = new byte[memStream.Length];
            memStream.Read(tempBuffer, 0, tempBuffer.Length);
            memStream.Close();
            requestStream.Write(tempBuffer, 0, tempBuffer.Length);
            requestStream.Close();


            WebResponse webResponse2 = httpWebRequest2.GetResponse();

            Stream stream2 = webResponse2.GetResponseStream();
            StreamReader reader2 = new StreamReader(stream2);


            // MessageBox.Show(reader2.ReadToEnd());

            webResponse2.Close();
            httpWebRequest2 = null;
            webResponse2 = null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="responseXML"></param>
        /// <returns></returns>
        public Dictionary<string, string> parseResponse(string responseXML)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(responseXML);
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            XmlNodeList resources = xml.SelectNodes("return");
            foreach (XmlAttribute xmlatt in resources[0].Attributes)
            {
                dictionary.Add(xmlatt.Name, xmlatt.Value);
            }


            resources = xml.SelectNodes("return/TargetCode");

            foreach (XmlNode node in resources)
            {
                dictionary.Add(node.Name, string.Format("{0}", node.InnerText));
            }

            return dictionary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="responseXML"></param>
        /// <returns></returns>
        public Dictionary<string, string> parseResponse(string responseXML, out string success, out string error)
        {
            error = string.Empty;
            success = string.Empty;

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(responseXML);
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            XmlNodeList resources = xml.SelectNodes("return");
            foreach (XmlAttribute xmlatt in resources[0].Attributes)
            {
                dictionary.Add(xmlatt.Name, xmlatt.Value);
            }


            resources = xml.SelectNodes("return/TargetCode");

            foreach (XmlNode node in resources)
            {
                dictionary.Add(node.Name, string.Format("{0}", node.InnerText));
            }

            resources = xml.SelectNodes("return/Token");

            foreach (XmlNode node in resources)
            {
                dictionary.Add(node.Name, string.Format("{0}", node.InnerText));
            }
            return dictionary;
        }


    }
}
