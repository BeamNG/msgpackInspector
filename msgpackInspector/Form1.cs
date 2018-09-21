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
        public Form1()
        {
            InitializeComponent();
        }

        private struct TreeData
        {
            public int id;
            public long byteStart;
            public long byteEnd;
            public MessagePackObject mpo;
        }

        Dictionary<string, TreeData> entries = new Dictionary<string, TreeData>();

        byte[] buffer;
        TreeData? currentSelection;
        private bool ignoreSelect = false;

        public void reset()
        {
            buffer = new byte[0];
            treeView1.Nodes.Clear();
            entries.Clear();
            hexBox1.Clear();
            currentSelection = null;
            toolStripStatusLabel1.Text = "";
            lblinterp.Text = "";
        }

        private void openFile(string filename)
        {
            reset();
            try
            {
                using (FileStream fs = File.OpenRead(filename))
                {
                    buffer = new byte[fs.Length];

                    byte[] bufMagic = new byte[3];
                    fs.Read(bufMagic, 0, 3);
                    if (bufMagic[0] == 'C' && bufMagic[1] == 'M' && bufMagic[2] == 'E')
                    {
                        /*
                         // TODO: FIXME
                        var output = new MemoryStream();
                        using (DeflateStream s = new DeflateStream(fs, CompressionMode.Decompress))
                        {
                            s.CopyTo(output);
                        }
                        output.Read(buffer, 0, (int)output.Length);
                        */
                    }
                    else
                    {
                        // uncompressed, raw, read from start
                        fs.Seek(0, SeekOrigin.Begin);
                        fs.Read(buffer, 0, (int)fs.Length);
                    }
                }
            } catch
            {
                MessageBox.Show("Unable to load file: " + filename);
                return;
            }

            if (buffer.Length == 0)
            {
                toolStripStatusLabel1.Text = "File is empty!";
                return;
            }
//             if (hexBox1.ByteProvider != null) {
//                 IDisposable byteProvider = hexBox1.ByteProvider as IDisposable;
//                 if (byteProvider != null)
//                     byteProvider.Dispose();
//                 hexBox1.ByteProvider = null;
//             }
            hexBox1.ByteProvider = new Be.Windows.Forms.DynamicByteProvider(buffer);

            try
            {
                ByteArrayUnpacker u = Unpacker.Create(buffer);

                int i = 0;
                long pos = 0;
                Random r = new Random();
                while (true)
                {

                    if (!u.Read())
                    {
                        break;
                    }
                    long newPos = u.Offset;
                    var o = u.LastReadData;
                    if (o.IsNil) break;

                    //Debug.WriteLine(o.UnderlyingType);

                    TreeData td = new TreeData();
                    td.id = i;
                    td.byteStart = pos;
                    td.byteEnd = newPos;
                    td.mpo = o;

                    Color bgc = Color.FromArgb(r.Next(150, 256), r.Next(150, 256), r.Next(150, 256));
                    hexBox1.setRangeColor(td.byteStart, td.byteEnd, Color.Black, bgc);


                    entries[i.ToString()] = td;
                    treeView1.Nodes.Add(i.ToString(), string.Format("{0:d3}", i) + " - " + o.UnderlyingType.ToString().Replace("System.", ""));

                    //treeView1.Nodes[treeView1.Nodes.Count - 1].BackColor = bgc;

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
            //Debug.WriteLine(e.Node.Name);
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

        private void hexBox1_SelectionLengthChanged(object sender, EventArgs e)
        {
            if (ignoreSelect) return;
            updateStatusbar(true);
        }

        private void hexBox1_SelectionStartChanged(object sender, EventArgs e)
        {
            if (ignoreSelect) return;
            updateStatusbar(true);
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

        private void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                treeView1.SelectedNode.ForeColor = Color.Black;
                treeView1.SelectedNode.BackColor = Color.White;
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reset();
            toolStripStatusLabel1.Text = "Closed file";
        }

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

        private void updateStatusbar(bool showSelection)
        {
            if (!currentSelection.HasValue) return;
            TreeData td = currentSelection.Value;

            string st = "Element " + string.Format("{0:d5}", td.id) + ": " + string.Format("0x{0:X5} > 0x{1:X5}, size: {2:d4} bytes. ", td.byteStart, td.byteEnd, td.byteEnd - td.byteStart);

            if (hexBox1.SelectionLength > 1)
            {
                st += string.Format("Selection: 0x{0:X5} > 0x{1:X5}, size: {2:d4} bytes", hexBox1.SelectionStart, hexBox1.SelectionStart + hexBox1.SelectionLength, hexBox1.SelectionLength);
            }
            else if (hexBox1.SelectionLength == 0)
            {
                st += string.Format("Selection: 0x{0:X5}", hexBox1.SelectionStart);
            }
            toolStripStatusLabel1.Text = st;
        }
    }
}
