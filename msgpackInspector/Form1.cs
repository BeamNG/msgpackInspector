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
using System.IO.Compression;

namespace msgpackinspector
{
    public partial class Form1 : Form
    {
        // this struct stores the msg deserialized data in memory
        private struct TreeData
        {
            public int id;
            public long byteStart;
            public long byteEnd;
            public MessagePackObject mpo;
        }

        // map to get from string id to treedata entry for the treeview
        Dictionary<string, TreeData> entries = new Dictionary<string, TreeData>();

        byte[] buffer; // the file buffer - loaded in memory
        TreeData? currentSelection; // currently selected element

        string windowTitle = "MessagePack Inspector v1.1"; // default window title
        private bool ignoreSelect = false; // internal usage: prevent events recursively firing

        public Form1()
        {
            InitializeComponent();
            Text = windowTitle;
        }

        // resets the data visible
        public void reset()
        {
            buffer = new byte[0];
            treeView1.Nodes.Clear();
            entries.Clear();
            hexBox1.Clear();
            currentSelection = null;
            toolStripStatusLabel1.Text = "";
            lblinterp.Text = "";
            Text = windowTitle;
        }

        // reads a messagepack file
        private void openFile(string filename)
        {
            reset();
            try
            {
                using (FileStream fs = File.OpenRead(filename))
                {
                    buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, (int)fs.Length);
                }
            } catch
            {
                MessageBox.Show("Unable to load file: " + filename);
                return;
            }

            try
            {
                Text = windowTitle + " - " + Path.GetFileName(filename);
            }
            catch { }

            if (buffer.Length == 0)
            {
                toolStripStatusLabel1.Text = "File is empty!";
                return;
            }

            try
            {
                hexBox1.ByteProvider = new Be.Windows.Forms.DynamicByteProvider(buffer); // sets the data in the hexedit control

                ByteArrayUnpacker u = Unpacker.Create(buffer);

                int i = 0;
                long pos = 0;
                Random r = new Random();
                while (true)
                {
                    if (!u.Read()) // reads the next item
                    {
                        break;
                    }
                    long newPos = u.Offset;
                    var o = u.LastReadData;
                    if (o.IsNil) break;

                    TreeData td = new TreeData();
                    td.id = i;
                    td.byteStart = pos;
                    td.byteEnd = newPos;
                    td.mpo = o;

                    // create a random color for the hexeditor
                    Color bgc = Color.FromArgb(r.Next(150, 256), r.Next(150, 256), r.Next(150, 256));
                    hexBox1.setRangeColor(td.byteStart, td.byteEnd, Color.Black, bgc);

                    // store it and create the treenode
                    entries[i.ToString()] = td;
                    treeView1.Nodes.Add(i.ToString(), string.Format("{0:d3}", i) + " - " + o.UnderlyingType.ToString().Replace("System.", ""));

                    i++;
                    pos = u.Offset;
                }
                if (treeView1.Nodes.Count > 0)
                {
                    treeView1.SelectedNode = treeView1.Nodes[0];
                }
                toolStripStatusLabel1.Text = "Loaded file " + filename + " with " + i.ToString() + " entries";
            } catch { }
        }

        // select a new element in the treeview - update data everywhere
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeData td = entries[e.Node.Name];
            if (td.mpo.IsNil)
            {
                lblinterp.Text = "Nil";
                return;
            }

            currentSelection = td;

            btnSaveAsFile.Visible = false;
            string s = "Element " + e.Node.Name + " - ";
            if (td.mpo.UnderlyingType == typeof(System.Int32))
            {
                Int32 i = td.mpo.AsInt32();
                s += string.Format("Int32 :  0x{0:X2} : {0:d}", i);
            } else if (td.mpo.UnderlyingType == typeof(byte[])) {
                s += string.Format("Buffer:  0x{0:X4} to 0x{1:X4} [{2:d} bytes]", td.byteStart, td.byteEnd, td.byteEnd - td.byteStart);
                btnSaveAsFile.Visible = true;
            }
            else
            {
                s += td.mpo.UnderlyingType.ToString().Replace("System.", "") + ": " + td.mpo.ToString();
            }
            // TODO: add more UnderlyingType variations

            lblinterp.Text = s;

            updateStatusbar(false);

            if (!ignoreSelect)
            {
                ignoreSelect = true;
                hexBox1.Select(td.byteStart, td.byteEnd - td.byteStart);
                hexBox1.ScrollByteIntoView(td.byteStart);
                ignoreSelect = false;
            }
            //hexBox1.Invalidate();
        }

        // fired upon navigation in the hexedit control
        private void hexBox1_SelectionLengthChanged(object sender, EventArgs e)
        {
            if (ignoreSelect) return;
            updateStatusbar(true);
        }

        // selects new element via the hexedit control
        private void hexBox1_SelectionStartChanged(object sender, EventArgs e)
        {
            if (ignoreSelect) return;
            updateStatusbar(true);
            // find the element that fits to the selection
            long pos = hexBox1.SelectionStart;
            foreach (KeyValuePair<string, TreeData> en in entries)
            {
                if(pos >= en.Value.byteStart && pos < en.Value.byteEnd)
                {
                    ignoreSelect = true;
                    if (treeView1.SelectedNode != null)
                    {
                        treeView1.SelectedNode.ForeColor = Color.Black;
                        treeView1.SelectedNode.BackColor = Color.White;
                    }
                    treeView1.SelectedNode = treeView1.Nodes[en.Key];
                    treeView1.SelectedNode.ForeColor = Color.White;
                    treeView1.SelectedNode.BackColor = SystemColors.Highlight;
                    ignoreSelect = false;
                    return;
                }
            }
        }

        // resets node colors
        private void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                treeView1.SelectedNode.ForeColor = Color.Black;
                treeView1.SelectedNode.BackColor = Color.White;
            }
        }

        // save binary file button click
        private void btnSaveAsFile_Click(object sender, EventArgs e)
        {
            if (!currentSelection.HasValue) return;
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (FileStream fs = File.OpenWrite(saveFileDialog1.FileName))
                {
                    fs.Write(buffer, (int)currentSelection.Value.byteStart, (int)(currentSelection.Value.byteEnd - currentSelection.Value.byteStart));
                }
            }

        }

        // updates the text in the status bar
        private void updateStatusbar(bool showSelection)
        {
            if (!currentSelection.HasValue) return;
            TreeData td = currentSelection.Value;

            string st = "Element " + string.Format("{0:d5}", td.id) + ": " + string.Format("0x{0:X5} > 0x{1:X5}, size: {2:d4} bytes. ", td.byteStart, td.byteEnd, td.byteEnd - td.byteStart);

            if (hexBox1.SelectionLength > 1)
            {
                st += string.Format("Selection: 0x{0:X5} > 0x{1:X5}, size: {2:d4} bytes", hexBox1.SelectionStart, hexBox1.SelectionStart + hexBox1.SelectionLength, hexBox1.SelectionLength);
            }
            else if (hexBox1.SelectionLength <= 1)
            {
                st += string.Format("Selection: 0x{0:X5}", hexBox1.SelectionStart);
            }
            toolStripStatusLabel1.Text = st;
        }

        // File > About Menu
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/BeamNG/msgpackInspector");
        }

        // File > Open Menu
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                openFile(openFileDialog1.FileName);
            }
        }

        // File > close Menu
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reset();
            toolStripStatusLabel1.Text = "Closed file";
        }

        // File > Exit Menu
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

    }
}
