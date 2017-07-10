using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using CarlosAg.ExcelXmlWriter;

namespace MultiRunner
{
    public partial class MultiRunner : Form
    {
        #region Variables

        bool isStopped;
        bool testCompleted;
        string logFolder = "";
        string gatewayLogFile;
        string clientLogFile;
        string currentTest;
        int testIndex;
        Process testProcess;
        enum ServerType { Gateway, Client1, Client2, Client3, Client4 };
        DataTable DataFile;
        AppSettings appSettings = new AppSettings();

        #endregion

        #region Constructor
        public MultiRunner()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Events Section
        #region Form1_Load
        private void Form1_Load(object sender, EventArgs e)
        {
            //Read application settings            
            FillTestListItems();
            CmbTests.Text = appSettings.TestSet;
            logFolder = appSettings.logFolder;
            StripDebug.Checked = appSettings.DebugMode;
            CmbGateway.Text = appSettings.GatewayAddress;
            CmbClient1.Text = appSettings.Client1Address;
            CmbClient2.Text = appSettings.Client2Address;
            CmbClient3.Text = appSettings.Client3Address;
            CmbClient4.Text = appSettings.Client4Address;
            
            UpdateDebugModeLabel();

            if (logFolder == "Not Set")
            {
                ReadLogFolderFromINI();
            }

            //Create table structure to be exported to file
            DataFile = new DataTable("uIEX");
            DataFile.Columns.Add("TestName", typeof(string));
            DataFile.Columns.Add("TestResult", typeof(string));
            DataFile.Columns.Add("GatewayStatus", typeof(string));
            DataFile.Columns.Add("ClientStatus", typeof(string));
            DataFile.Columns.Add("FailureReason", typeof(string));
            DataFile.Columns.Add("TestSet", typeof(string));
            openFileDialog.Filter = "XML files (*.xml)|*.xml";
            saveFileDialog.Filter = "XML files (*.xml)|*.xml";
        }
        #endregion

        #region BtRun_Click
        private void BtRun_Click(object sender, EventArgs e)
        {

            ExcelLogFile xlLog = new ExcelLogFile(DG, logFolder);
            xlLog.SaveLog();

            string DebugMode = "";
            ClearGrid();
            isStopped = false;

            BtRun.Enabled = false;
            CmbTests.Enabled = false;
            BtUp.Enabled = false;
            BtDown.Enabled = false;

            ProcessStartInfo MyProcess = new ProcessStartInfo();

            string Gateway = "";
            string Client = "";

            Gateway = " -g " + CmbGateway.Text;

            if (CmbClient1.Text != "None")
            {
                Client = " -s " + CmbClient1.Text;
                if (CmbClient2.Text != "None")
                {
                    Client += " -s " + CmbClient2.Text;
                    if (CmbClient3.Text != "None")
                    {
                        Client += " -s " + CmbClient3.Text;
                        if (CmbClient4.Text != "None")
                        {
                            Client += " -s " + CmbClient4.Text;
                        }
                    }
                }
            }

            if (StripDebug.Checked)
            {
                DebugMode = " /d";
            }

            bool loop = StripLoop.Checked;
            try
            {
                do
                {
                    xlLog.NewLine();
                    testIndex = 0;
                    for (int i = 0; i < DG.Rows.Count; i++)
                    {
                        if (DG.Rows[i].Cells[0].Value.ToString() == "True")
                        {
                            testIndex = i;
                            currentTest = DG.Rows[testIndex].Cells[1].Value.ToString();
                            MyProcess.FileName = WorkingDirectory(CmbTests.SelectedItem.ToString());
                            MyProcess.Arguments = "-t " + currentTest + Gateway + Client + DebugMode;
                            MyProcess.WindowStyle = ProcessWindowStyle.Hidden;

                            DG.Rows[testIndex].Cells[2].Value = "Running...";

                            testProcess = Process.Start(MyProcess);
                            testProcess.EnableRaisingEvents = true;
                            testProcess.Exited += new EventHandler(Test_Exited);
                            testCompleted = false;

                            do
                            {
                                Application.DoEvents();

                            } while (isStopped == false && testCompleted == false);

                            if (isStopped == false)
                            {
                                CheckResult(xlLog);
                            }
                        }

                        if (isStopped)
                        {
                            break;
                        }
                    }

                    isStopped = false;
                    BtRun.Enabled = true;
                    CmbTests.Enabled = true;
                    BtUp.Enabled = true;
                    BtDown.Enabled = true;
                    loop = StripLoop.Checked;
                } while (loop && !isStopped);
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region CmbTests_DropDown
        private void CmbTests_DropDown(object sender, EventArgs e)
        {
            FillTestListItems();
        }
        #endregion

        #region CmbTests_SelectedIndexChanged
        private void CmbTests_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CmbTests.SelectedItem.ToString().Equals("All"))
            {
                GetAllTests();
                DG.Focus();
                return;
            }

            string processName = WorkingDirectory(CmbTests.SelectedItem.ToString());
            try
            {
                DG.Rows.Clear();
                Assembly asm = Assembly.LoadFrom(processName);
                foreach (Type type in asm.GetTypes())
                {
                    if (type.IsPublic)
                    {
                        string[] row = new string[] { "False", type.FullName, "", "", "", "" };
                        DG.Rows.Add(row);
                    }
                }

                DG.Sort(DG.Columns[1], ListSortDirection.Ascending);
                DG.Rows[0].Selected = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Failed To Load Tests!\nCan't Find Tests Project: " + CmbTests.SelectedItem.ToString(), "ERROR");
            }
            DG.Focus();
        }
        #endregion

        #region CmbGateway_SelectedIndexChanged
        private void CmbGateway_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReadLogFolderFromINI();
        }
        #endregion

        #region ChkAll_CheckedChanged
        private void ChkAll_CheckedChanged(object sender, EventArgs e)
        {
            bool Checked;
            if (ChkAll.Checked)
            {
                Checked = true;
            }
            else
            {
                Checked = false;
            }

            for (int i = 0; i < DG.Rows.Count; i++)
            {
                DG.Rows[i].Cells[0].Value = Checked;
            }

        }
        #endregion

        #region btnUp_Click
        private void btnUp_Click(object sender, EventArgs e)
        {
            DataGridView grid = DG;
            try
            {
                int totalRows = grid.Rows.Count;
                int idx = grid.SelectedCells[0].OwningRow.Index;
                if (idx == 0)
                    return;
                int col = grid.SelectedCells[0].OwningColumn.Index;
                DataGridViewRowCollection rows = grid.Rows;
                DataGridViewRow row = rows[idx];
                rows.Remove(row);
                rows.Insert(idx - 1, row);
                grid.ClearSelection();
                grid.Rows[idx - 1].Cells[col].Selected = true;

            }
            catch { }
        }
        #endregion

        #region btnDown_Click
        private void btnDown_Click(object sender, EventArgs e)
        {
            DataGridView grid = DG;
            try
            {
                int totalRows = grid.Rows.Count;
                int idx = grid.SelectedCells[0].OwningRow.Index;
                if (idx == totalRows - 1)
                    return;
                int col = grid.SelectedCells[0].OwningColumn.Index;
                DataGridViewRowCollection rows = grid.Rows;
                DataGridViewRow row = rows[idx];
                rows.Remove(row);
                rows.Insert(idx + 1, row);
                grid.ClearSelection();
                grid.Rows[idx + 1].Cells[col].Selected = true;
            }
            catch { }
        }
        #endregion

        #region ButStop_Click
        private void ButStop_Click(object sender, EventArgs e)
        {
            try
            {
                isStopped = true;
                testProcess.Kill();
                DG.Rows[testIndex].Cells[2].Value = "Aborted";
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region ButGwLog_Click
        private void ButGwLog_Click(object sender, EventArgs e)
        {
            try
            {
                string LogP = GetLogFile(ServerType.Gateway);
                gatewayLogFile = LogP;
                Process.Start(LogP);
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region ButCl1Log_Click
        private void ButCl1Log_Click(object sender, EventArgs e)
        {
            try
            {
                string LogP = GetLogFile(ServerType.Client1);
                clientLogFile = LogP;
                Process.Start(LogP);
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region ButCl2Log_Click
        private void ButCl2Log_Click(object sender, EventArgs e)
        {
            try
            {
                string LogP = GetLogFile(ServerType.Client2);
                clientLogFile = LogP;
                Process.Start(LogP);
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region ButCl3Log_Click
        private void ButCl3Log_Click(object sender, EventArgs e)
        {
            try
            {
                string LogP = GetLogFile(ServerType.Client3);
                clientLogFile = LogP;
                Process.Start(LogP);
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region ButCl4Log_Click
        private void ButCl4Log_Click(object sender, EventArgs e)
        {
            try
            {
                string LogP = GetLogFile(ServerType.Client4);
                clientLogFile = LogP;
                Process.Start(LogP);
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region ButExecutoinLog_Click
        private void ButExecutoinLog_Click(object sender, EventArgs e)
        {
            try
            {
                string LogP = logFolder + "\\FullLog.xls";
                Process.Start(LogP);
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region Test_Exited
        private void Test_Exited(object sender, EventArgs e)
        {
            testCompleted = true;
        }
        #endregion

        #region MultiRunner_FormClosing
        private void MultiRunner_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                appSettings.TestSet = CmbTests.Text;
                appSettings.logFolder = logFolder;
                appSettings.DebugMode = StripDebug.Checked;
                appSettings.GatewayAddress = CmbGateway.Text;
                appSettings.Client1Address = CmbClient1.Text;
                appSettings.Client2Address = CmbClient2.Text;
                appSettings.Client3Address = CmbClient3.Text;
                appSettings.Client4Address = CmbClient4.Text;
                appSettings.Save();
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region MultiRunner_FormClosed
        private void MultiRunner_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                isStopped = true;
                testProcess.Kill();
            }
            catch (Exception)
            {
            }
        }
        #endregion
        #endregion

        #region Strip Items Section
        #region StripDebug_CheckedChanged
        private void StripDebug_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDebugModeLabel();
        }
        #endregion

        #region StripLogFolder_Click
        private void StripLogFolder_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Your current log folder is set to " + logFolder + "\nTo change this value please set your Environment.ini file.", "Log Folder Information");
        }
        #endregion

        #region StripOpenFile_Click
        private void StripOpenFile_Click(object sender, EventArgs e)
        {
            DialogResult res = openFileDialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                DataFile.ReadXml(openFileDialog.FileName);
                CmbTests.Text = DataFile.Rows[0][5].ToString();
                foreach (DataRow FileRow in DataFile.Rows)
                {
                    for (int i = 0; i < DG.Rows.Count; i++)
                    {
                        if (DG.Rows[i].Cells[1].Value.ToString() == FileRow[0].ToString())
                        {
                            DG.Rows[i].Cells[0].Value = "True";
                            DG.Rows[i].Cells[2].Value = FileRow[1].ToString();
                            DG.Rows[i].Cells[3].Value = FileRow[2].ToString();
                            DG.Rows[i].Cells[4].Value = FileRow[3].ToString();
                            DG.Rows[i].Cells[5].Value = FileRow[4].ToString();
                        }
                    }
                }
                DG.EndEdit();
            }
        }
        #endregion

        #region StripSaveFile_Click
        private void StripSaveFile_Click(object sender, EventArgs e)
        {
            DialogResult res = saveFileDialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                DG.EndEdit();

                DataFile.Clear();
                DataFile.Rows.Add(null, null, null, null, null, CmbTests.Text);
                for (int i = 0; i < DG.Rows.Count; i++)
                {
                    if (DG.Rows[i].Cells[0].Value.ToString() == "True")
                        DataFile.Rows.Add(DG.Rows[i].Cells[1].Value.ToString(), DG.Rows[i].Cells[2].Value.ToString(), DG.Rows[i].Cells[3].Value.ToString(), DG.Rows[i].Cells[4].Value.ToString(), DG.Rows[i].Cells[5].Value.ToString());
                }
                DataFile.WriteXml(saveFileDialog.FileName);
            }
        }
        #endregion

        #region StripTestsSets_Click
        private void StripTestsSets_Click(object sender, EventArgs e)
        {
            TestSetForm tForm = new TestSetForm();
            tForm.Show();
        }
        #endregion
        #endregion

        #region Private Functions Section
        #region FillTestListItems
        private void FillTestListItems()
        {
            CmbTests.Items.Clear();
            CmbTests.Items.Add("All");

            appSettings.Reload();
            string curSets = appSettings.TestSetList;
            string[] testSets = curSets.Split(',');
            foreach (string set in testSets)
            {
                if (set != "")
                {
                    CmbTests.Items.Add(set);
                }
            }
        }
        #endregion

        #region UpdateDebugModeLabel
        private void UpdateDebugModeLabel()
        {
            if (StripDebug.Checked)
            {
                textStatus.Text = "Debug Mode";
            }
            else
            {
                textStatus.Text = "";
            }
        }
        #endregion

        #region ReadLogFolderFromINI
        private void ReadLogFolderFromINI()
        {
            string iexServer = CmbGateway.Text.Split(' ')[1];
            try
            {
                AMS.Profile.Ini iniFile = new AMS.Profile.Ini("C:\\Program Files\\IEX\\Tests\\TestsINI\\IEX" + iexServer + "\\Environment.ini");
                logFolder = iniFile.GetValue("IEX" + iexServer, "LogDirectory").ToString();
            }
            catch
            {
                MessageBox.Show("Failed to Get Log Folder From IEX_" + iexServer + " Environment.ini", "ERROR");
            }
        }
        #endregion

        #region GetLogPath
        private string GetLogFile(ServerType server, bool selected = true)
        {
            string iexServer = "";
            string logFile = null;

            try
            {
                if (server == ServerType.Gateway)
                    iexServer = CmbGateway.Text.Substring(CmbGateway.Text.Length - 1, 1);
                else if (server == ServerType.Client1)
                    iexServer = CmbClient1.Text.Substring(CmbClient1.Text.Length - 1, 1);
                else if (server == ServerType.Client2)
                    iexServer = CmbClient2.Text.Substring(CmbClient2.Text.Length - 1, 1);
                else if (server == ServerType.Client3)
                    iexServer = CmbClient3.Text.Substring(CmbClient3.Text.Length - 1, 1);
                else if (server == ServerType.Client4)
                    iexServer = CmbClient4.Text.Substring(CmbClient4.Text.Length - 1, 1);

                string testName;
                if (selected)
                {
                    testName = DG.SelectedRows[0].Cells[1].Value.ToString();
                }
                else
                {
                    testName = DG.Rows[testIndex].Cells[1].Value.ToString();
                }
                string[] dirs = Directory.GetDirectories(logFolder, testName + "*");

                DateTime newestDirTime = new DateTime(1900, 1, 1);
                string newestDirPath = "";
                foreach (string subDir in dirs)
                {
                    DateTime created = new DirectoryInfo(subDir).LastWriteTime;
                    if (created > newestDirTime)
                    {
                        newestDirPath = subDir;
                        newestDirTime = created;
                    }
                }

                string logPath = "";
                foreach (string subDir in Directory.GetDirectories(newestDirPath))
                {
                    if (subDir.Contains("Server-" + iexServer))
                    {
                        logPath = subDir;
                    }
                }

                string[] files = Directory.GetFiles(logPath, "*.iexlog");
                logFile = files[0];
            }
            catch (Exception)
            {
            }

            return logFile;
        }
        #endregion

        #region ClearGrid
        private void ClearGrid()
        {
            for (int i = 0; i < DG.Rows.Count; i++)
            {
                DataGridViewCellStyle CellStyle = new DataGridViewCellStyle();
                CellStyle.BackColor = Color.White;
                CellStyle.ForeColor = Color.Black;
                DG.Rows[i].Cells[2].Style = CellStyle;
                DG.Rows[i].Cells[2].Value = "";
                DG.Rows[i].Cells[3].Style = CellStyle;
                DG.Rows[i].Cells[3].Value = "";
                DG.Rows[i].Cells[4].Style = CellStyle;
                DG.Rows[i].Cells[4].Value = "";
                DG.Rows[i].Cells[5].Style = CellStyle;
                DG.Rows[i].Cells[5].Value = "";
            }
        }
        #endregion

        #region CheckResult
        private void CheckResult(ExcelLogFile xlLog)
        {
            string[] FileContent;
            string LogP;
            bool DidClient = false;
            bool DidGateway = false;

            string testName = DG.Rows[testIndex].Cells[1].Value.ToString();
            string[] dirs = Directory.GetDirectories(logFolder, testName + "*");

            DateTime newestDirTime = new DateTime(1900, 1, 1);
            string newestDirPath = "";
            foreach (string subDir in dirs)
            {
                DateTime created = new DirectoryInfo(subDir).LastWriteTime;
                if (created > newestDirTime)
                {
                    newestDirPath = subDir;
                    newestDirTime = created;
                }
            }
            LogP = newestDirPath + "\\IEX_Summary.xml";

            try
            {
                FileContent = File.ReadAllLines(LogP);

                if (FileContent[1].Contains("Test Status=" + (char)34 + "Failed" + (char)34))
                {
                    DataGridViewCellStyle CellStyle = new DataGridViewCellStyle();
                    CellStyle.BackColor = Color.Red;
                    CellStyle.ForeColor = Color.White;
                    DG.Rows[testIndex].Cells[2].Style = CellStyle;
                    DG.Rows[testIndex].Cells[2].Value = "FAILED";
                }
                else if (FileContent[1].Contains("Test Status=" + (char)34 + "Passed" + (char)34))
                {
                    DataGridViewCellStyle CellStyle = new DataGridViewCellStyle();
                    CellStyle.BackColor = Color.Green;
                    CellStyle.ForeColor = Color.White;
                    DG.Rows[testIndex].Cells[2].Style = CellStyle;
                    DG.Rows[testIndex].Cells[2].Value = "PASSED";
                }

                for (int i = FileContent.Count() - 1; i > -1; i--)
                {
                    if (DidClient && DidGateway) break;

                    if (FileContent[i].Contains("Status=" + (char)34 + "Failed" + (char)34))
                    {
                        string Reason = FileContent[i].Remove(0, FileContent[i].IndexOf("Reason"));
                        DataGridViewCellStyle CellStyle = new DataGridViewCellStyle();
                        CellStyle.BackColor = Color.Red;
                        CellStyle.ForeColor = Color.White;
                        if (Reason.Contains("Client") && !DidClient)
                        {
                            DG.Rows[testIndex].Cells[4].Style = CellStyle;
                            DG.Rows[testIndex].Cells[4].Value = "FAILED";
                            DidClient = true;
                        }
                        else if (Reason.Contains("Gateway") && !DidGateway)
                        {
                            DG.Rows[testIndex].Cells[3].Style = CellStyle;
                            DG.Rows[testIndex].Cells[3].Value = "FAILED";
                            DidGateway = true;
                        }
                        Reason = Reason.Substring(Reason.IndexOf("=") + 2, (Reason.IndexOf((char)34 + " ") - 3) - Reason.IndexOf("=") + 1);
                        DG.Rows[testIndex].Cells[5].Value = Reason;
                    }
                    else if (FileContent[i].Contains("Status=" + (char)34 + "Passed" + (char)34))
                    {
                        string Reason = FileContent[i].Remove(0, FileContent[i].IndexOf("Reason"));
                        DataGridViewCellStyle CellStyle = new DataGridViewCellStyle();
                        CellStyle.BackColor = Color.Green;
                        CellStyle.ForeColor = Color.White;
                        if (Reason.Contains("Client") && !DidClient)
                        {
                            DG.Rows[testIndex].Cells[4].Style = CellStyle;
                            DG.Rows[testIndex].Cells[4].Value = "PASSED";
                            DidClient = true;
                        }
                        else if (Reason.Contains("Gateway") && !DidGateway)
                        {
                            DG.Rows[testIndex].Cells[3].Style = CellStyle;
                            DG.Rows[testIndex].Cells[3].Value = "PASSED";
                            DidGateway = true;
                        }
                    }
                }

                xlLog.PrintMsg(testIndex, GetLogFile(ServerType.Gateway, false), GetLogFile(ServerType.Client1, false));
            }
            catch
            {
                DG.Rows[testIndex].Cells[2].Value = "FINISHED";
                DG.Rows[testIndex].Cells[5].Value = "Log isn't available";
            }
        }
        #endregion

        #region WorkingDirectory
        private string WorkingDirectory(string testSet)
        {
            string path = "";

            if (testSet == "All")
            {
                path = FindTest(testSet);
                return path;
            }

            if (File.Exists(Application.StartupPath + "\\" + testSet + ".exe"))
            {
                path = Application.StartupPath + "\\" + testSet + ".exe";
            }
            else if (File.Exists(Application.StartupPath + "\\..\\..\\..\\" + testSet + "\\bin\\Debug\\" + testSet + ".exe"))
            {
                path = Application.StartupPath + "\\..\\..\\..\\" + testSet + "\\bin\\Debug\\" + testSet + ".exe";
            }
            else if (File.Exists(Application.StartupPath + "\\..\\..\\..\\IEX_PROJECT_FILES\\IEX_TESTS\\" + testSet + "\\bin\\Debug\\" + testSet + ".exe"))
            {
                path = Application.StartupPath + "\\..\\..\\..\\IEX_PROJECT_FILES\\IEX_TESTS\\" + testSet + "\\bin\\Debug\\" + testSet + ".exe";
            }
            else if (File.Exists(Application.StartupPath + "\\..\\..\\..\\..\\..\\IEX_TESTS\\src\\bin\\" + testSet + ".exe"))
            {
                path = Application.StartupPath + "\\..\\..\\..\\..\\..\\IEX_TESTS\\src\\bin\\" + testSet + ".exe";
            }
            
            return path;
        }
        #endregion

        #region FindTest
        private string FindTest(string testSet)
        {
            string path = "";
            foreach (string item in CmbTests.Items)
            {
                if (item != "All")
                {
                    string processName = WorkingDirectory(item);
                    Assembly asm = Assembly.LoadFrom(processName);
                    foreach (Type type in asm.GetTypes())
                    {
                        if (type.IsPublic)
                        {
                            if (type.FullName == currentTest)
                            {
                                path = processName;
                                break;
                            }
                        }
                    }
                }
            }
            return path;
        }
        #endregion

        #region GetAllTests
        private void GetAllTests()
        {
            DG.Rows.Clear();
            foreach (string testSet in CmbTests.Items)
            {
                if (testSet != "All")
                {
                    string processName = WorkingDirectory(testSet);
                    try
                    {
                        Assembly asm = Assembly.LoadFrom(processName);
                        foreach (Type type in asm.GetTypes())
                        {
                            if (type.IsPublic)
                            {
                                string[] row = new string[] { "False", type.FullName, "", "", "", "" };
                                DG.Rows.Add(row);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Failed To Load Tests!\nCan't Find Tests Project: " + testSet, "ERROR");
                    }
                }
            }
            if (DG.RowCount > 0)
            {
                DG.Sort(DG.Columns[1], ListSortDirection.Ascending);
                DG.Rows[0].Selected = true;
            }
        }
        #endregion
        #endregion

        #region ExcelLogFile Section
        public class ExcelLogFile
        {
            private WorksheetStyle HeadLine;
            private WorksheetStyle Fail;
            private WorksheetStyle Pass;
            private WorksheetStyle Defult;
            private Workbook book;
            private Worksheet fullSatistic;
            private WorksheetRow row;
            private System.Windows.Forms.DataGridView DG;
            private string logFolder;

            public ExcelLogFile(DataGridView dg, string logFolder)
            {
                this.logFolder = logFolder;
                book = new Workbook();
                fullSatistic = book.Worksheets.Add("Full");
                row = fullSatistic.Table.Rows.Add();
                this.DG = dg;

                HeadLine = book.Styles.Add("HeadLine");
                HeadLine.Font.Size = 14;
                HeadLine.Font.Bold = true;
                HeadLine.Alignment.Horizontal = StyleHorizontalAlignment.Center;

                Fail = book.Styles.Add("Fail");
                Fail.Font.Size = 10;
                Fail.Font.Color = "Red";
                Fail.Font.Bold = true;
                Fail.Alignment.Horizontal = StyleHorizontalAlignment.Center;

                Pass = book.Styles.Add("Pass");
                Pass.Font.Size = 10;
                Pass.Font.Color = "Green";
                Pass.Font.Bold = true;
                Pass.Alignment.Horizontal = StyleHorizontalAlignment.Center;

                Defult = book.Styles.Add("Default");
                Defult.Font.Size = 10;
                Defult.Font.Bold = true;
                Defult.Alignment.Horizontal = StyleHorizontalAlignment.Center;

                CerateLogFile();
                NewLine();
            }

            public void NewLine()
            {
                row = fullSatistic.Table.Rows.Add();
            }

            public void SaveLog()
            {
                try
                {
                    book.Save(logFolder + "\\FullLog.xls");
                }
                catch (Exception)
                {
                }
            }

            public void PrintMsg(int testIndex, string GwLogPath, string ClientLogPath)
            {
                WorksheetCell cell;

                string GWStat = DG.Rows[testIndex].Cells[3].Value.ToString();
                string ClientStat = DG.Rows[testIndex].Cells[4].Value.ToString();
                string reson = DG.Rows[testIndex].Cells[5].Value.ToString();

                if (DG.Rows[testIndex].Cells[2].Value.ToString().Contains("FAILED"))
                {
                    cell = row.Cells.Add(reson);
                    cell.StyleID = "Fail";
                }
                else
                {
                    cell = row.Cells.Add("PASSED");
                    cell.StyleID = "Pass";

                }

                cell = row.Cells.Add(GWStat);
                cell.HRef = GwLogPath;
                if (GWStat.Contains("FAILED"))
                {
                    cell.StyleID = "Fail";
                }
                else
                {
                    cell.StyleID = "Pass";
                }

                cell = row.Cells.Add(ClientStat);
                cell.HRef = ClientLogPath;
                if (ClientStat.Contains("FAILED"))
                {
                    cell.StyleID = "Fail";
                }
                else
                {
                    cell.StyleID = "Pass";
                }

                SaveLog();
            }

            public void AddPass(string Msg, string GWlog, string IPClog)
            {
                WorksheetCell cell;

                row.Cells.Add(Msg, DataType.String, "Pass");

                cell = row.Cells.Add("Gateway Log");
                cell.HRef = GWlog;

                cell = row.Cells.Add("Client Log");
                cell.HRef = IPClog;

                SaveLog();
            }

            private void CerateLogFile()
            {
                WorksheetCell cell;

                for (int i = 0; i < DG.Rows.Count; i++)
                {
                    if (DG.Rows[i].Cells[0].Value.ToString() == "True")
                    {
                        row.Table.Columns.Add(new WorksheetColumn(200)); //Result
                        row.Table.Columns.Add(new WorksheetColumn(100)); //GW Status
                        row.Table.Columns.Add(new WorksheetColumn(100)); //IPC Status
                        int TestIndex = i;

                        string CurrentTest = DG.Rows[TestIndex].Cells[1].Value.ToString();

                        row.Cells.Add(CurrentTest, DataType.String, "HeadLine");
                        cell = row.Cells.Add("GW Status");
                        cell.StyleID = "Default";

                        cell = row.Cells.Add("Client Status");
                        cell.StyleID = "Default";
                    }
                }
            }
        }
        #endregion
    }
}
