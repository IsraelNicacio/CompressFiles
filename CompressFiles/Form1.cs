using System;
using System.IO.Compression;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.IO;


namespace CompressFiles
{
    public partial class Form1 : Form
    {
        DirectoryInfo directorySelected;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(txtPath.Text))
            {
                directorySelected = new DirectoryInfo(txtPath.Text);

                string[] arrayXml = new string[directorySelected.GetFiles().Length];
                int count = 0;

                foreach (FileInfo fi in directorySelected.GetFiles())
                {
                    byte[] fileToBytes = File.ReadAllBytes(fi.FullName);

                    string value = ASCIIEncoding.UTF8.GetString(fileToBytes);

                    arrayXml[count] = value;

                    count++;
                }
            }

            //backgroundWorker.RunWorkerAsync();
        }

        

        private void btnSelectPath_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog frmFolderBrowserDialog = new FolderBrowserDialog())
            {
                if (frmFolderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    //Atualiza caixa de texto
                    txtPath.Text = frmFolderBrowserDialog.SelectedPath;
                }
            }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            FileInfo[] files = directorySelected.GetFiles();

            //Variaveis
            int count = 0;
            int total = files.Length;

            foreach (FileInfo file in files)
            {
                // Get the stream of the source file.
                using (FileStream inFile = file.OpenRead())
                {
                    // Prevent compressing hidden and 
                    // already compressed files.
                    if ((File.GetAttributes(file.FullName) & FileAttributes.Hidden) != FileAttributes.Hidden & file.Extension != ".gz")
                    {
                        // Create the compressed file.
                        using (FileStream outFile = File.Create(file.FullName + ".gz"))
                        {
                            using (GZipStream Compress = new GZipStream(outFile, CompressionMode.Compress))
                            {
                                // Copy the source file into 
                                // the compression stream.
                                //inFile.CopyTo(Compress);

                                Console.WriteLine("Compressed {0} from {1} to {2} bytes.",
                                    file.Name, files.Length.ToString(), outFile.Length.ToString());
                            }
                        }
                    }
                }

                Thread.Sleep(100);

                //Increment count
                count++;

                //Porcentagem
                int porcentagem = ((int)count * 100) / total;

                // Report progress.
                backgroundWorker.ReportProgress(porcentagem);
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Change the value of the ProgressBar to the BackgroundWorker progress.
            progressBar.Value = e.ProgressPercentage;
            progressBar.PerformStep();
            // Set the text.
            this.Text = e.ProgressPercentage.ToString();
        }

        public static byte[] Compress(byte[] raw)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(memory,
                    CompressionMode.Compress, true))
                {
                    gzip.Write(raw, 0, raw.Length);
                }
                return memory.ToArray();
            }
        }
    }
}
