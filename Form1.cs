using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using System.IO;
using MsgPack;
using System.Diagnostics;

namespace msgpackinspector
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Dictionary<string, MessagePackObject> entries = new Dictionary<string, MessagePackObject>();

        private void openFile(string filename)
        {
            treeView1.Nodes.Clear();
            using (FileStream fs = File.OpenRead(filename))
            {
                Unpacker u = Unpacker.Create(fs);
                int i = 0;
                while (u.Read()) {
                    var o = u.LastReadData;
                    if (o.IsNil) break;

                    Debug.WriteLine(o.UnderlyingType);
                    entries[i.ToString()] = o;
                    treeView1.Nodes.Add(i.ToString(), i.ToString() + " - " + o.UnderlyingType.ToString().Replace("System.", ""));
                    i++;
                    
                }
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                openFile(openFileDialog1.FileName);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Debug.WriteLine(e.Node.Name);
            MessagePackObject o = entries[e.Node.Name];
            if (o.IsNil)
            {
                lblinterp.Text = "Nil";
                return;
            }

            lblinterp.Text = o.UnderlyingType + "\n" + o.ToString();
        }
    }
}
