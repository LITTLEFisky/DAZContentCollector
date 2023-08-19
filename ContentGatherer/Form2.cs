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

namespace ContentGatherer
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Program.Content.Close();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            listBox1.Items.Add(folderBrowserDialog1.SelectedPath.Replace('\\', '/') + '/');
            listBox1.Update();
        }

        private void DetectStudio4(string path)
        {
            var autodetect = File.OpenRead(path);
            using (var detect = new StreamReader(autodetect, Encoding.UTF8))
            {
                string line;
                while ((line = detect.ReadLine()) != null)
                {
                    if (line.Contains("Directory PATH="))
                    {
                        int i;
                        for (i = line.Length - 1; i >= 0; i--)
                        {
                            if (line[i] == 'A')
                                break;
                        }
                        int newlen = i - 2;
                        line = line.Substring(0, newlen);
                        line = line.Substring(20);
                        line = line + '/';
                        if (listBox1.Items.Contains(line) == false)
                        {
                            listBox1.Items.Add(line);
                        }
                    }
                }
            }
        }
        private void DetectPoser(string path)
        {
            var autodetect = File.OpenRead(path);
            using (var detect = new StreamReader(autodetect, Encoding.UTF8))
            {
                string line;
                while ((line = detect.ReadLine()) != null)
                {
                    if (line.Contains("<ContentFolder "))
                    {
                        int i;
                        for (i = line.Length - 1; i >= 0; i--)
                        {
                            if (line[i] == '\\')
                                break;
                        }
                        int newlen = i - 7;
                        line = line.Substring(0, newlen);
                        line = line.Substring(24);
                        line = line.Replace("\\", "/");
                        if (listBox1.Items.Contains(line) == false)
                        {
                            listBox1.Items.Add(line);
                        }
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(File.Exists("settings.ini"))
            {
                File.Delete("settings.ini");
            }
            var settingsfile = File.OpenWrite("settings.ini");
            using (var writer = new StreamWriter(settingsfile, Encoding.UTF8))
            {
                foreach (var set in listBox1.Items)
                {
                    writer.WriteLine(set);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(listBox1.Items.Count != 0)
            {
                int selected = listBox1.SelectedIndex;
                listBox1.Items.RemoveAt(selected);
                if (listBox1.Items.Count == selected)
                {
                    listBox1.SelectedIndex = selected - 1;
                }
                else
                {
                    listBox1.SelectedIndex = selected;
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string path = Environment.ExpandEnvironmentVariables("%APPDATA%\\DAZ 3D\\Studio4\\ContentDirectoryManager.dsx");
            if (File.Exists(path))
            {
                DetectStudio4(path);
            }
            path = Environment.ExpandEnvironmentVariables("%APPDATA%\\DAZ 3D\\Studio4 Public Build\\ContentDirectoryManager.dsx");
            if (File.Exists(path))
            {
                DetectStudio4(path);
            }
            path = Environment.ExpandEnvironmentVariables("%APPDATA%\\Poser\\11\\LibraryPrefs.xml");
            if (File.Exists(path))
            {
                DetectPoser(path);
            }
            path = Environment.ExpandEnvironmentVariables("%APPDATA%\\Poser\\12\\LibraryPrefs.xml");
            if (File.Exists(path))
            {
                DetectPoser(path);
            }
            path = Environment.ExpandEnvironmentVariables("%APPDATA%\\Poser\\13\\LibraryPrefs.xml");
            if (File.Exists(path))
            {
                DetectPoser(path);
            }
            path = Environment.ExpandEnvironmentVariables("%APPDATA%\\Poser Pro\\10\\LibraryPrefs.xml");
            if (File.Exists(path))
            {
                DetectPoser(path);
            }
            path = Environment.ExpandEnvironmentVariables("%APPDATA%\\Poser Pro\\10\\LibraryPrefs.xml");
            if (File.Exists(path))
            {
                DetectPoser(path);
            }
        }
    }
}
