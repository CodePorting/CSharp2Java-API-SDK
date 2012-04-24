/*--------------------------------------------------------------------------------------+
|
|   $HeadURL: svn://sialkot.codeporting.com/cs-porter/trunk/MarketPlace/CSharp2Java-API-SDK/csharp/CodePortingCSharp2Java/CodePortingCSharp2Java/Program.cs $
|   $Id: Program.cs muhammad.iqbal $
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
|   NOTE:
|   Run the command line utility from command prompt using following command:
|   CodePortingCSharp2Java codeportingtest testpassword
+--------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodePortingCSharp2Java
{
    class Program
    {
        static void Main(string[] args)
        {
            // Check if command line parameters are available
            bool bFoundArguments = args.Length == 0 ? false : true;
            if (bFoundArguments == false)
            {
                // Inform user about missing arguments.
                System.Console.WriteLine("Provide command line parameters as : <username> <password>");
            }
            else if (args.Length == 1)
            {
                // Inform user about the issue in parameters.
                System.Console.WriteLine("Provide username and password as command line parameters : <username> <password>");
            }
            else if (args.Length == 2)
            {
                try
                {
                    // Create new interface to use the API utilities created.
                    Utilities utils = new Utilities();
                    string error = string.Empty;

                    // Display input information.
                    System.Console.WriteLine("Signing in to CodePorting.com using SignIn web call");
                    System.Console.WriteLine("Username : " + args[0]);
                    System.Console.WriteLine("Password : " + args[1]);
                    System.Console.WriteLine("Please wait ... ");
                    System.Console.WriteLine("");
                    System.Console.WriteLine("");

                    // Initiate sign in request on codeporting.com
                    string token = utils.Signin(args[0], args[1], ref error);

                    // Display header to make output look better.
                    System.Console.WriteLine("*****************************************************************************");
                    System.Console.WriteLine("                       WEB CALL RESULT FOR SIGN IN");
                    System.Console.WriteLine("*****************************************************************************");

                    // Check if token is valid, which is the loose measure to identify success of login call.
                    if (!string.IsNullOrEmpty(token))
                    {
                        // Let user know about the successful signin.
                        System.Console.WriteLine("Sign in is successful, token is : " + token);
                    }
                    else
                    {
                        // If login call fails, present the error information as sent by codeporting.com to user.
                        System.Console.WriteLine("Error while signing in. ERROR: " + error);
                    }
                }
                catch (Exception ex)
                {
                }
            }
            else
            {
                // More then required parameters are sent as input, inform user about it.
                System.Console.WriteLine("Too many parameters provided, please review the command.");
            }

            System.Console.ReadLine();
        }
    }
}
