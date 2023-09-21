using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReadandModifyFile
{
    public partial class FrmFileModifier : Form
    {
        string[] fileNames = Array.Empty<string>();
        bool exceptionCatched;
        public FrmFileModifier()
        {
            InitializeComponent();
        }

        private void btnSelectMultiple_Click(object sender, EventArgs e)
        {
            try
            {
                selectMultipleFiles();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            btnModifyWrite.Enabled = true;
        }

        private void BtnModifyWrite_Click(object sender, EventArgs e)
        {
            //if (String.IsNullOrEmpty(strFilePath))
            //{
            //    MessageBox.Show("No file loaded");
            //    return;
            //}
            btnModifyWrite.Enabled = false;
            progressBar.Visible = true;
            percentageLabel.Visible = true;
            btnOpenFolder.Enabled = false;
            progressBar.Maximum = 100;
            progressBar.Step = 1;
            progressBar.Value = 0;
            backgroundWorker.RunWorkerAsync();
        }
        public void selectMultipleFiles()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string strSelectedFileNames = "";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileNames = openFileDialog.FileNames;
                foreach (string fileName in fileNames)
                {
                    strSelectedFileNames += fileName + Environment.NewLine;
                }
                textBox.Text = strSelectedFileNames;
            }
            //FolderBrowserDialog fbd = new FolderBrowserDialog();
            //if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //    // shows the path to the selected folder in the folder dialog
            //    //MessageBox.Show(fbd.SelectedPath);
            //textBox.Text = fbd.SelectedPath;
            //DirectoryInfo di = new DirectoryInfo(fbd.SelectedPath);
            //FileInfo[] fileNames = di.GetFiles("*.txt");
            //foreach (string fileName in Directory.EnumerateFiles(fbd.SelectedPath, "*.txt"))
            //{
            //    string contents = File.ReadAllText(fileName);
            //}


            //OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            //if (openFileDialog.ShowDialog() == DialogResult.OK)
            //    try
            //    {
            //        strFilePath = openFileDialog.FileName;
            //        textBox.Text = strFilePath;
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new ApplicationException("Cannot open the file");
            //    }
        }
        public void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ReadNWriteFile(fileNames);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.ToString());
                exceptionCatched = true;
                //if (!string.IsNullOrEmpty(fileNewName))
                //{
                //    File.Delete(fileNewName);
                //}
            }
        }

        // GUI components are updated
        public void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            percentageLabel.Text = e.ProgressPercentage.ToString() + "%";
            if (e.ProgressPercentage == 99)
            {
                exceptionCatched = false;
            }
        }

        // Complete the work after DoWork and ProgressChanged.
        public void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (exceptionCatched)
            {
                textBox.Clear();
            }
            else
            {
                string strFileDirectory = Path.GetDirectoryName(fileNames[0]);
                Process.Start(strFileDirectory);
                btnOpenFolder.Enabled = true;
                textBox.Text = "";
                this.Close();
            }
        }

        public void ReadNWriteFile(string[] fNames)
        {
            int totalFileNumber = fNames.Length;
            int fileNumCounter = 0;
            int position;
            string line = String.Empty;
            string lineModified = String.Empty;
            string fileNewName = String.Empty;

            foreach (var fName in fNames)
            {
                try
                {
                    position = fName.LastIndexOf(".");
                    fileNewName = fName.Insert(position, "_Modified");
                    //line = String.Empty;
                    //lineModified = String.Empty;
                    //long totalLineCount = File.ReadLines(fName).Count();

                    using (StreamReader streamReader = new StreamReader(fName))
                    using (StreamWriter streamWriter = new StreamWriter(fileNewName))
                    {
                        while (!streamReader.EndOfStream)
                        {
                            line = streamReader.ReadLine();
                            lineModified = line.Replace("|0x", "|");
                            streamWriter.WriteLine(lineModified);
                            //lineCounter++;
                            //backgroundWorker.ReportProgress((int)(lineCounter * 100 / totalLineCount));
                        }
                    }
                    fileNumCounter++;
                    backgroundWorker.ReportProgress((fileNumCounter * 100 / totalFileNumber));
                }
                catch (Exception e)
                {
                    throw new ApplicationException("The file cannot be read/written.");
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void FrmFileModifier_Load(object sender, EventArgs e)
        {

        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            textBox.SelectionStart = textBox.Text.Length;
            textBox.ScrollToCaret();
        }
    }
}
