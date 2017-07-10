using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Net.Sockets;

namespace ResultSummaryGenerator
{
    static class XMLUtil
    {
        public static XmlDocument getXmlDocument(this XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }
    }

    class Program
    {
        private static XmlWriter writer;
        private static int fileCount = 0;

        static void Main(string[] args)
        {
            try
            {
                string[] commandLineArgs = Environment.GetCommandLineArgs();
                string directoryPath = commandLineArgs[1]; /* The path where all XMLs are present */
                string new_file_name = "\\IEX_Summary.xml"; /* Output file name */
                string new_file = directoryPath + new_file_name; /* Output file path */
                string LogFile= "";
                string newFile_snD = "";

                XDocument doc = null; /* An X Document which will refer to each XML in the path */

                /* Creating a WRITER object to access the new file */
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Encoding = Encoding.UTF8;
                settings.Indent = true;
                writer = XmlWriter.Create(new_file, settings);

                writer.WriteStartDocument();
                writer.WriteStartElement("SUMMARY"); /* HEAD Node */

                /* Fetch all the files in the given path/folder */
                if (String.IsNullOrEmpty(directoryPath))
                {
                    //writeErrorToFile("Invalid path as argument");
                }
                DirectoryInfo d = new DirectoryInfo(directoryPath);
                if (!d.Exists)
                {
                    //writeErrorToFile("Incorrect path. Directory does not exist!");
                }

                FileInfo[] files = d.GetFiles("*.xml*", SearchOption.AllDirectories);

                /* Run the loop for each XML file in the given path */
                foreach (FileInfo file in files)
                {
                    fileCount++;
                    try
                    {
                    if ("\\" + file.Name != new_file_name || fileCount > 1) /* The new file (output file) should not be counted but all other IEX_Summary.xml files inside should be counted */
                    {
                        doc = XDocument.Load(file.FullName);

                        /* Get the complete element with tag name "Test" in the XML file */
                        XmlNodeList nodeList = XMLUtil.getXmlDocument(doc).GetElementsByTagName("Test");

                        if (nodeList.Count > 0)
                        {
                            writer.WriteStartElement("Test");
                            foreach (XmlNode xn in nodeList)
                            {
                                /* Search for attribute "LogFile" and put its value to "Name" attribute in new(output) XML */
                                if (xn.Attributes["Status"] != null)
                                {
                                    /* Search for attribute "Status" and put its value to "Status" attribute in new(output) XML */
                                    writer.WriteAttributeString("Status", xn.Attributes["Status"].Value);
                                }

                                /* If the status is Failed, then fetch the failure reason too */
                                /* Search for attribute "Reason" and put its value to "Reason" attribute in new(output) XML */
                                if (xn.Attributes["Reason"] != null)
                                {
                                    writer.WriteAttributeString("Reason", xn.Attributes["Reason"].Value);
                                }

                                /* Search for attribute "LogFile" and put its value to "Name" attribute in new(output) XML */
                                if (xn.Attributes["LogFile"] != null)
                                {
                                    writer.WriteAttributeString("LogFile", xn.Attributes["LogFile"].Value);
                                }
                            }

                            writer.WriteEndElement();
                        }
                    }
					                    }
                    catch
                    { }
                }
                FileInfo[] files_Snd = d.GetFiles("*.res*", SearchOption.AllDirectories);
                if (fileCount == 1||files_Snd.Count()>0)
                {
                    try
                    {
                        
                        /* Run the loop for each XML file in the given path */
                        foreach (FileInfo myfile in files_Snd)
                        {
                            newFile_snD = directoryPath + "\\" + myfile.Name;
                            //fileCount++;
                            //To get the XML summary generator from res file for  SnD
                            try
                            {
                                //Add Test tag in the summary XML
                                writer.WriteStartElement("Test");
                                //Add status from Res file and add the attributes to the status tag
                                string Status = GetValueFromResFile("Status", newFile_snD);
                                writer.WriteAttributeString("Status", Status);
                                //Add Reason tag and values for summary XML from comments of SnD Res file along with Healcheck status which is in Fieldvalue7
                                string Reason = GetValueFromResFile("Comments", newFile_snD);
                                string healthCheckValue = GetValueFromResFile("FieldValue7", newFile_snD);
                                string expectedIterations = GetValueFromResFile("Iterations", newFile_snD);
                                string successfulIterations = GetValueFromResFile("SuccessfulIterations", newFile_snD);
                                writer.WriteAttributeString("Reason", Reason + " HelathCheck:" + healthCheckValue + "Expected Iterations: " + expectedIterations + "SuccessfulIterations: " + successfulIterations);
                                //Add Testname from Resfile
                                LogFile = GetValueFromResFile("TESTName", newFile_snD);
                                writer.WriteAttributeString("LogFile", LogFile);
                                writer.WriteEndElement();
                                WritetoResFile(directoryPath, newFile_snD,myfile.Name);
                                //Copy the Resfile to the ManualReportToHPQC_UploadResultFile results folder to upload the results
                                File.Copy(newFile_snD, "C:\\ManualReportToHPQC_UploadResultFile\\results\\" + myfile.Name);
                                //If the Snd Robustness Script fails we are Killing the Process and creating the Coer dump
                               // try
                               // {
                               //     if (Status.ToUpper().Contains("FAIL"))
                                //    {
                                 //       System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
                                  //      try
                                  //      {
                                    //        string IEXNumber = GetValueFromResFile("IEXNumber", newFile_snD);
                                      //      string telnetip = GetValueFromTelnetIni("unixServer", "C:\\Program Files\\IEX\\Tests\\TestsINI\\IEX" + IEXNumber + "\\Telnet.ini");
                                        //    string coreFilePath = GetValueFromTelnetIni("PcatSTBPath", "C:\\Program Files\\IEX\\Tests\\TestsINI\\IEX" + IEXNumber + "\\Environment.ini");
                                          //  coreFilePath = coreFilePath.Replace('"', ' ').Trim();
                                            //coreFilePath = coreFilePath.Substring(0, coreFilePath.Length - 1);
                                            //int i = telnetip.LastIndexOf('.');
                                            //string systemIP = telnetip.Substring(0, i + 1) + "50";
                                            //clientSocket.Connect(telnetip, 23);
                                            ////Killing the APP Process
                                            //string request = "pkill -6 APP_Process";
                                            //Byte[] bytesSent = Encoding.ASCII.GetBytes(request + "\n");
                                            //NetworkStream stream = clientSocket.GetStream();
                                            //stream.Write(bytesSent, 0, bytesSent.Length);
                                            //System.Threading.Thread.Sleep(10000);

                                            ////Creating the Tar file
                                            //string request1 = "tar -zcvf core.tar.gz " + coreFilePath;
                                            //Byte[] bytesSent1 = Encoding.ASCII.GetBytes(request1 + "\n");
                                            //NetworkStream stream1 = clientSocket.GetStream();
                                            //stream1.Write(bytesSent1, 0, bytesSent1.Length);
                                            //System.Threading.Thread.Sleep(300000);

                                            ////Copying the Core file to the resulsts path
                                            //string request2 = directoryPath + "\\core.tar core.tar.gz";
                                            //Byte[] bytesSent2 = Encoding.ASCII.GetBytes(request2 + "\n");
                                            //NetworkStream stream2 = clientSocket.GetStream();
                                            //stream2.Write(bytesSent2, 0, bytesSent2.Length);
                                            //System.Threading.Thread.Sleep(10000);

                                            //Remove the Core.tar file which we created
                                            //string request3 = "rm -rf core.tar.gz";
                                            //Byte[] bytesSent3 = Encoding.ASCII.GetBytes(request3 + "\n");
                                            //NetworkStream stream3 = clientSocket.GetStream();
                                            //stream3.Write(bytesSent3, 0, bytesSent3.Length);
                                            //System.Threading.Thread.Sleep(20000);
											
											//Remove the Core.* file which we created
                                            //string request4 = "rm -rf "+coreFilePath+"core.*";
                                            //Byte[] bytesSent4 = Encoding.ASCII.GetBytes(request4 + "\n");
                                            //NetworkStream stream4 = clientSocket.GetStream();
                                            //stream4.Write(bytesSent4, 0, bytesSent4.Length);
                                            //System.Threading.Thread.Sleep(10000);
                                            //clientSocket.Close();
                                        //}
                                        //catch
                                        //{
                                          //  clientSocket.Close();
                                        //}

                                   // }
                                //}
                                //catch
                                //{
                                    //Do Nothing
                                //}

                                //Calling the batch file to update the results to HPQC
                                //System.Diagnostics.Process.Start("C:\\ManualReportToHPQC_UploadResultFile\\runHPQCUpdateSnD.bat");
                            }
                            catch
                            {
                                //Do nothing
                            }
                        }

                    
                    }
                    catch
                    {
                        writeErrorToFile("No XML or res files inside the directory");
                    }
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();
                /* Move all the valid XML files to the main(root) folder */
                //foreach (FileInfo file in files)
                //{
                //    if ("\\" + file.Name != new_file_name) /* The new file (output file) should not be counted */
                //    {
                //        doc = XDocument.Load(file.FullName);

                /* Get the complete element with tag name "Test" in the XML file */
                //        XmlNodeList nodeList = XMLUtil.getXmlDocument(doc).GetElementsByTagName("Test");

                //        if (nodeList.Count > 0) /* Making sure that only valid XML fiels are copied */
                //        {
                //            string destFile = System.IO.Path.Combine(directoryPath, file.Name);
                //            System.IO.File.Copy(file.FullName, destFile, true);
                //        }
                //    }

                //If Test XML files are not generated in the folder, create the Test XML by copying the same contents of IEX_SUMMARY.XML and copy to the same loaction as in IEX_SUMMARY.XML
                if (files_Snd.Count() > 0)
                {
                    try
                    {

                        File.Copy(new_file, directoryPath + "\\IEX_Summary_" + LogFile + ".xml");
                     
                    }
                    catch
                    {
                        //Do nothing
                    }
                }
            }
            catch (DirectoryNotFoundException e)
            {
                //Do nothing
            }
            catch (Exception e)
            {
                // Do nothing!
                //writeErrorToFile(e.Message);
                //writeErrorToFile(e.StackTrace);
                //writer.WriteEndElement();
                //writer.WriteEndDocument();
                //writer.Flush();
                //writer.Close();
            }
        }

        static void writeErrorToFile(string message)
        {
            try
            {
                writer.WriteStartElement("Error");
                writer.WriteAttributeString("Reason", message);
                writer.WriteEndElement();
            }
            catch (Exception e)
            {
                //Do Nothing
            }
        }
        //This function is to read and get the values from Resfile
        static string GetValueFromResFile(string key, string fileName)
        {
            string myStatus = "";
            using (StreamReader reader = new StreamReader(fileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Split('=')[0].Trim() == key)
                    {
                        myStatus = line.Split('=')[1].Trim();
                        break;
                    }
                }
            }
            return myStatus;
        }
        //This function is to read and get the values from Resfile
        static string GetValueFromTelnetIni(string key, string fileName)
        {
            string myStatus = "";
            using (StreamReader reader = new StreamReader(fileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Split('=')[0].Trim() == key)
                    {
                        myStatus = line.Split('=')[1].Trim();
                        break;
                    }
                }
            }
            return myStatus;
        }
        static void WritetoResFile(string directoryPath,string fileName,string resFileName)
        {
            string[] directorypatharray = directoryPath.Split('\\');
            string jobid = directorypatharray[directorypatharray.Length - 3];
            string tempLineValue;
            string tempFile = directoryPath + "\\"+"temp.res";
            using (FileStream inputStream = File.OpenRead(fileName))
            {
                using (StreamReader inputReader = new StreamReader(fileName))
                {
                    using (StreamWriter outputWriter = File.AppendText(tempFile))
                    {
                        while (null != (tempLineValue = inputReader.ReadLine()))
                        {

                            if (tempLineValue.StartsWith("Comments"))
                            {
                                string comments = tempLineValue + " Jobid:" + jobid;
                                outputWriter.WriteLine(comments);
                            }
                            else
                            {
                                outputWriter.WriteLine(tempLineValue);
                            }
                        }
                    }
                }
            }
            File.Delete(fileName);
            File.Move(tempFile, directoryPath+"\\"+ resFileName);

        }
    }
}

