using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;
using System.Diagnostics;
using System.Threading;

namespace ContentGatherer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private int ProcessSubString(string inp, string direction)
        {
            if (direction == "forward")
            {
                for (int i = 0; i < inp.Length; i++)
                {
                    if (inp[i] == '/')
                    {
                        return i + 1;
                    }
                }
            }
            else
            {
                for (int i = inp.Length-1; i > 0; i--)
                {
                    if (inp[i] == '\\')
                    {
                        return i + 1;
                    }

                }
            }
            return inp.Length;
        }

        private int ProcessEndString(string inp)
        {
            for (int i = inp.Length - 1; i > 0; i--)
            {
                if (inp[i] == '.')
                {
                    return i+4;
                }
            }
            return 0;
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private string ColectMainFile(string inp)
        {
            string tmp = "";
            string temp = "";
            foreach(string line in Program.Content.listBox1.Items)
            {
                temp = line.Replace('/', '\\');
                if(inp.Contains(temp) == true)
                {
                    tmp = inp.Substring(temp.Length);
                    break;
                }
            }
            return tmp;
        }

        private void AnalizeAddDUF(string duf, string fename)
        {
            //check is DUF is compressed
            string duf_new = duf;
            using (var checkstream = System.IO.File.OpenRead(duf))
            using (var checkstreamReader = new StreamReader(checkstream, Encoding.UTF8, true, 1024))
            {
                string check;
                if ((check = checkstreamReader.ReadLine()) != "{")
                {
                    string sfolder = duf.Substring(0, ProcessSubString(duf, "backwards") - 1);
                    using (FileStream originalFile = System.IO.File.OpenRead(duf))
                    {
                        string currentFile = duf;
                        string outFile = duf.Remove(duf.Length - 4);
                        using (FileStream decompressedFile = System.IO.File.Create(outFile))
                        {
                            using (GZipStream decompressionStream = new GZipStream(originalFile, CompressionMode.Decompress))
                            {
                                decompressionStream.CopyTo(decompressedFile);
                            } 
                        }
                        duf_new = outFile;
                    }
                    
                }
            }
            

            int count = 0;
            using (var filestream = System.IO.File.OpenRead(duf_new))
            using (var streamReader = new StreamReader(filestream, Encoding.UTF8, true, 1024))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (((line.Contains(".dsf") == true) || (line.Contains(".pmd") == true) || (line.Contains(".obz") == true) || (line.Contains(".obj") == true) || (line.Contains(".png") == true) || (line.Contains(".jpg") == true) || (line.Contains(".jpeg") == true) || (line.Contains(".tga") == true) || (line.Contains(".bmp") == true) || (line.Contains(".hdr") == true) || (line.Contains(".exr") == true)) && (line.Contains("OctaneRender") == false))
                    {
                        bool present = false;
                        string type;
                        line = line.Replace("%20", " ");
                        line = line.Replace("%28", "(");
                        line = line.Replace("%29", ")");
                        line = line.Replace("%27", "\'");

                        if (line.Contains(".dsf") == true)
                        {
                            line = line.Substring(0, ProcessEndString(line));
                            line = line.Substring(ProcessSubString(line, "forward"));
                            type = "DSON";
                        }
                        else if ((line.Contains(".obj") == true) || (line.Contains(".obz")))
                        {
                            line = line.Substring(0, ProcessEndString(line));
                            line = line.Substring(ProcessSubString(line, "forward"));
                            type = "Geometry";
                        }
                        else if ((line.Contains(".png") == true) || (line.Contains(".jpg") == true) || (line.Contains(".jpeg") == true) || (line.Contains(".tga") == true) || (line.Contains(".bmp") == true) || (line.Contains(".hdr") == true))
                        {
                            line = line.Substring(0, ProcessEndString(line));
                            line = line.Substring(ProcessSubString(line, "forward"));
                            type = "Texture";
                        }
                        else
                        {
                            line = line.Substring(0, line.Length - 1);
                            line = line.Substring(ProcessSubString(line, "forward"));
                            type = "Data";
                        }
                        if (line != "")
                        {
                            foreach (DataGridViewRow row in dataGridView1.Rows)
                            {
                                if (Convert.ToString(row.Cells[0].Value) == line)
                                {
                                    present = true;
                                }
                            }

                            if (present == false)
                            {
                                dataGridView1.Rows.Add(line, type, fename);
                                count++;
                                dataGridView1.Update();
                                label1.Text = Convert.ToString(dataGridView1.Rows.Count);
                                label1.Update();
                            }
                        }
                    }
                }
            }
            if(duf_new.Contains(".duf") == false)
            {
                System.IO.File.Delete(duf_new);
            }
            duf = duf.Replace("\\","/");
            dataGridView2.Rows.Add(duf, fename, count);
        }
        private void AnalizeAddCR2(string duf, string fename)
        {
            int links = 0;
            using (var filestream = System.IO.File.OpenRead(duf))
            using (var streamReader = new StreamReader(filestream, Encoding.UTF8, true, 1024))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (((line.Contains(".pmd") == true) || (line.Contains(".obz") == true) || (line.Contains(".png") == true) || (line.Contains(".obj") == true) || (line.Contains(".jpg") == true) || (line.Contains(".jpeg") == true) || (line.Contains(".tga") == true) || (line.Contains(".bmp") == true) || (line.Contains(".hdr") == true) || (line.Contains(".exr") == true)) && (line.Contains("OctaneRender") == false))
                    {
                        bool present = false;
                        string type;
                        line = line.Replace("%20", " ");
                        line = line.Replace("%28", "(");
                        line = line.Replace("%29", ")");
                        line = line.Replace("%27", "\'");
                        line = line.Replace(':', '/');

                        if ((line.Contains(".obj") == true) || (line.Contains(".obz") == true))
                        {
                            line = line.Substring(0, ProcessEndString(line));
                            line = line.Substring(ProcessSubString(line, "forward"));
                            type = "Geometry";
                        }
                        else if (line.Contains(".pmd") == true)
                        {
                            line = line.Substring(0, line.Length - 2);
                            line = line.Substring(ProcessSubString(line, "forward"));
                            type = "Poser morph data";
                        }
                        else
                        {
                            line = line.Substring(0, line.Length - 1);
                            line = line.Substring(ProcessSubString(line, "forward"));
                            type = "Texture";
                        }

                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (Convert.ToString(row.Cells[0].Value) == line)
                            {
                                present = true;
                            }
                        }

                        if (present == false)
                        {
                            links++;
                            dataGridView1.Rows.Add(line, type, fename);
                            dataGridView1.Update();
                            label1.Text = Convert.ToString(dataGridView1.Rows.Count);
                            label1.Update();
                        }

                    }
                }
            }
            duf = duf.Replace('\\', '/');
            dataGridView2.Rows.Add(duf, fename, links);
        }

        private void RemoveItem(int filescount, string fename)
        {
            int targetsize = dataGridView1.Rows.Count - filescount;
            do
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells[2].Value.ToString() == fename)
                    {
                        dataGridView1.Rows.RemoveAt(row.Index);
                        dataGridView1.Update();
                        label1.Text = Convert.ToString(dataGridView1.Rows.Count);
                        label1.Update();
                    }

                }
                
            }while(targetsize < dataGridView1.Rows.Count);
        }
        private void button2_Click(object sender, EventArgs e) //Collect button
        {
            progressBar1.Visible = true;
            progressBar1.Maximum = dataGridView1.Rows.Count;
            progressBar1.Value = 0;
            label4.Text = "Collecting...";
            label4.Visible = true;
            label4.Update();
            if (Program.Content.listBox1.Items.Count < 1) //Check is libraries are added9
            {
                MessageBox.Show("You forgot to add your libraies!\n" +
                    "Click \"Content Folders\",\n" +
                    "add your folders (like \"C:/Poser/Content/\") and press \"Save\".\n" +
                    "Then you can press \"Close\". You can also press \"Autodetect\" if\n" +
                    "your content folders are properly set in DAZ Studio or Poser");
            }
            else
            {
                saveFileDialog2.ShowDialog();
                string outfolder = saveFileDialog2.FileName.Substring(0, ProcessSaveString(saveFileDialog2.FileName, 'p'));
                outfolder += "\\TEMPer\\";
                string savename = saveFileDialog2.FileName.Substring(ProcessSaveString(saveFileDialog2.FileName, 'p')+1);
                //folderBrowserDialog1.ShowDialog();
                //string outfolder = folderBrowserDialog1.SelectedPath + '/';
                outfolder = outfolder.Replace("\\", "/");
                foreach (DataGridViewRow pth in dataGridView1.Rows) //Collecting dependent files
                {
                    string outfile = outfolder + pth.Cells[0].Value.ToString();
                    FileInfo file = new FileInfo(outfile);
                    file.Directory.Create();
                    string infile = Program.Content.listBox1.Items[0] + pth.ToString();
                    if (System.IO.File.Exists(infile) == false)
                    {
                        foreach (string num in Program.Content.listBox1.Items)
                        {

                            infile = num + pth.Cells[0].Value.ToString();
                            if (System.IO.File.Exists(infile) == true)
                            {
                                break;
                            }
                        }
                        if (System.IO.File.Exists(infile) == false)
                        {
                            DialogResult result = MessageBox.Show($"File {infile} was not found! Want to find it by yourself?", "", MessageBoxButtons.YesNo);

                            if (result == DialogResult.Yes)
                            {
                                openFileDialog2.ShowDialog();
                                infile = openFileDialog2.FileName;
                            }
                        }
                    }
                    if (System.IO.File.Exists(infile) == true)
                    {
                        System.IO.File.Copy(infile, outfile, true);
                        progressBar1.Value++;
                        progressBar1.Update();
                        label4.Text = "Collecting..." + infile;
                        label4.Update();
                    }
                }
                foreach (DataGridViewRow pth in dataGridView2.Rows) //Collecting main files
                {
                    string outMainFile = "";
                    bool found = false;
                    for(int i = 0; i < Program.Content.listBox1.Items.Count; i++)
                    {
                        if (pth.Cells[0].Value.ToString().Contains(Program.Content.listBox1.Items[i].ToString()) == true)
                        {
                            outMainFile = pth.Cells[0].Value.ToString().Substring(Program.Content.listBox1.Items[i].ToString().Length);
                            found = true;
                        }
                        if(found == true)
                        {
                            FileInfo file = new FileInfo(outfolder + outMainFile);
                            file.Directory.Create();
                            System.IO.File.Copy(pth.Cells[0].Value.ToString(), outfolder + outMainFile, true);
                            if (pth.Cells[0].Value.ToString().Contains(".duf"))
                            {
                                string InMainFileIcon = pth.Cells[0].Value.ToString().Replace(".duf", ".duf.png");
                                string OutMainFileIcon = outMainFile.Replace(".duf", ".duf.png");
                                System.IO.File.Copy(InMainFileIcon, outfolder + OutMainFileIcon, true);
                            }
                            else
                            {
                                string InMainFileIcon = pth.Cells[0].Value.ToString().Replace(".cr2", ".png");
                                string OutMainFileIcon = outMainFile.Replace(".cr2", ".png");
                                System.IO.File.Copy(InMainFileIcon, outfolder + OutMainFileIcon, true);
                            }
                        }
                        else
                        {
                            MessageBox.Show($"File {outfolder + outMainFile} not found. Fuck you.");
                        }
                    }
                }
                label4.Text = "ZIPping...";
                label4.Update();
                var readme = System.IO.File.OpenWrite(outfolder + "readme.txt");
                using (var writer = new StreamWriter(readme, Encoding.UTF8))
                {
                    writer.WriteLine("This model was collected and packed with ContentGatherer");
                }
                //ZipFile.CreateFromDirectory(folderBrowserDialog1.SelectedPath, folderBrowserDialog1.SelectedPath + ".zip");
                ZipFile.CreateFromDirectory(outfolder, saveFileDialog2.FileName);
                Directory.Delete(outfolder, true);
                MessageBox.Show("Done!", "", MessageBoxButtons.OK);
                label4.Text = "Done!";
                label4.Update();
                progressBar1.Value = 0;
                progressBar1.Visible = false;
                label4.Visible = false;
            }
        }

        private int ProcessSaveString(string fileName, char N)
        {
            if(N == 'p')
            {
                for (int i = fileName.Length - 1; i > 0; i--)
                {
                    if (fileName[i] == '\\')
                    {
                        return i;
                    }
                }
            }
            else if(N == 'n')
            {
                for (int i = fileName.Length - 1; i > 0; i--)
                {
                    if (fileName[i] == '\\')
                    {
                        return i+1;
                    }
                }
            }
            return 0;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.Content.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Program.Content = new Form2();
            if (System.IO.File.Exists("settings.ini"))
            {
                using (var settingsstream = System.IO.File.OpenRead("settings.ini"))
                    using (var streamReader = new StreamReader(settingsstream, Encoding.UTF8, true, 1024))
                    {
                        string line;
                        while ((line = streamReader.ReadLine()) != null)
                        {
                            Program.Content.listBox1.Items.Add(line);
                        }
                    }
            }
            else
            {
                Program.Content.autodetectContent();
                MessageBox.Show("Your content folders were detected! Click \"Content Folders\",\n" +
                                 "if you want to check what folders were added. You can add more\n" +
                                 "and save the list for future. Click \"Where are my folders?\"\n" +
                                 "if you don't see your content folders!");
            }
        }

        private void button4_Click(object sender, EventArgs e) //Add button
        {
            openFileDialog1.ShowDialog();
            string fename = openFileDialog1.FileName.Substring(ProcessSubString(openFileDialog1.FileName, "backward"));
            if (openFileDialog1.FileName != " ")
            {
                dataGridView2.Update();
                if(openFileDialog1.FileName.Contains(".duf"))
                {
                    AnalizeAddDUF(openFileDialog1.FileName, fename);
                }
                else if(openFileDialog1.FileName.Contains(".cr2"))
                {
                    AnalizeAddCR2(openFileDialog1.FileName, fename);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e) //Remove button
        {
            int rowindex = dataGridView2.CurrentCell.RowIndex;
            DataGridViewRow row = dataGridView2.Rows[rowindex];
            string filename = (string)row.Cells[0].Value.ToString();
            dataGridView2.Rows.RemoveAt(rowindex);
            string fename = (string)row.Cells[1].Value.ToString();
            RemoveItem(Convert.ToInt32(row.Cells[2].Value), fename);
        }

        private void button6_Click(object sender, EventArgs e) //clean button
        {
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
        }

        private void button8_Click(object sender, EventArgs e) //Save list button
        {
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != " ")
            {
                var savefile = System.IO.File.OpenWrite(saveFileDialog1.FileName);
                using (var writer = new StreamWriter(savefile, Encoding.UTF8))
                {
                    foreach (DataGridViewRow set in dataGridView2.Rows)
                    {
                        writer.WriteLine(set.Cells[0].Value.ToString());
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e) //Save dependencies button
        {
            saveFileDialog1.ShowDialog();
            var savefile = System.IO.File.OpenWrite(saveFileDialog1.FileName);
            using (var writer = new StreamWriter(savefile, Encoding.UTF8))
            {
                foreach (DataGridViewRow pth in dataGridView1.Rows)
                {
                    writer.WriteLine(pth.Cells[0].Value.ToString());
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            openFileDialog3.ShowDialog();
            using (var settingsstream = System.IO.File.OpenRead(openFileDialog3.FileName))
                using (var streamReader = new StreamReader(settingsstream, Encoding.UTF8, true, 1024))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        string fename = openFileDialog1.FileName.Substring(ProcessSubString(line, "backwards"));
                        dataGridView2.Rows.Add(line, fename);
                        if (line.Contains(".duf"))
                        {
                            AnalizeAddDUF(line, fename);
                        }
                        else if (line.Contains(".cr2"))
                        {
                            AnalizeAddCR2(line, fename);
                        }
                    }
                }
        }


        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            System.Diagnostics.Process.Start("notepad.exe", dataGridView2.CurrentRow.Cells[0].Value.ToString());
            /*
            Program.temp = dataGridView2.CurrentRow.Cells[0].Value.ToString();
            Program.Editor.ShowDialog();
            */
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int rowindex = dataGridView2.CurrentCell.RowIndex;
            DataGridViewRow row = dataGridView2.Rows[rowindex];
            string filename = (string)row.Cells[0].Value.ToString();
            dataGridView2.Rows.RemoveAt(rowindex);
            string fename = (string)row.Cells[1].Value.ToString();
            RemoveItem(Convert.ToInt32(row.Cells[2].Value), fename);
            AnalizeAddDUF(filename, fename);
        }

        private void aboutToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("DAZ and Poser content gatherer by LITTLEFisky, 2023\n" +
                "Alpha release tester: Tom Winkler\n" + 
                "Beta release tester: Kagesan\n\n" +
                $"version: {Program.version}\n" + 
                "No rights reserved, bit I'll bite\n" +
                "your nuts off if you'll claim this\n" +
                "code as yours!", "About", MessageBoxButtons.OK);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Process.Start("https://boosty.to/littlefisky/single-payment/donation/451573/target?share=target_link");
        }
    }
}
