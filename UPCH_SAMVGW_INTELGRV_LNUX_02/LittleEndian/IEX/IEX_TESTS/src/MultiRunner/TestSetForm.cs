using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MultiRunner
{
    public partial class TestSetForm : Form
    {
        AppSettings appSettings = new AppSettings();

        public TestSetForm()
        {
            InitializeComponent();
        }

        private void TestSetForm_Load(object sender, EventArgs e)
        {
            AddTestSetRows();
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            string sets = "";
            for (int i = 0; i < testSetTable.Rows.Count; i++)
            {
                if (testSetTable.Rows[i].Cells[0].Value != null)
                {
                    sets += testSetTable.Rows[i].Cells[0].Value.ToString() + ",";
                }
            }
            appSettings.TestSetList = sets;
            appSettings.Save();
            this.Close();
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btReset_Click(object sender, EventArgs e)
        {
            AppSettings temp = new AppSettings();
            temp.Reset();
            appSettings.TestSetList = temp.TestSetList;
            appSettings.Save();
            testSetTable.Rows.Clear();
            AddTestSetRows();
        }

        private void AddTestSetRows()
        {
            string curSets = appSettings.TestSetList;
            string[] testSets = curSets.Split(',');
            foreach (string set in testSets)
            {
                if (set != "")
                {
                    testSetTable.Rows.Add(set);
                }
            }
        }
    }
}
