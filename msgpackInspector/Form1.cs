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
            public long byteStart;
            public long byteEnd;
            public MessagePackObject mpo;
        }

        Dictionary<string, TreeData> entries = new Dictionary<string, TreeData>();

        byte[] buffer;

        private void openFile(string filename)
        {
            treeView1.Nodes.Clear();

            using (FileStream fs = File.OpenRead(filename))
            {
                buffer = new byte[fs.Length];

                byte[] bufMagic = new byte[3];
                fs.Read(bufMagic, 0, 3);
                if(bufMagic[0] == 'C' && bufMagic[1] == 'M' && bufMagic[2] == 'E')
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

            if (buffer.Length == 0) return;

            if (hexBox1.ByteProvider != null) {
                IDisposable byteProvider = hexBox1.ByteProvider as IDisposable;
                if (byteProvider != null)
                    byteProvider.Dispose();
                hexBox1.ByteProvider = null;
            }
            hexBox1.ByteProvider = new Be.Windows.Forms.DynamicByteProvider(buffer);

            ByteArrayUnpacker u = Unpacker.Create(buffer);

            int i = 0;
            long pos = 0;
            Random r = new Random();
            while (true) {

                if(!u.Read()) {
                    break;
                }
                long newPos = u.Offset;
                var o = u.LastReadData;
                if (o.IsNil) break;

                //Debug.WriteLine(o.UnderlyingType);

                TreeData td = new TreeData();
                td.byteStart = pos;
                td.byteEnd = newPos;
                td.mpo = o;

                hexBox1.setRangeColor(td.byteStart, td.byteEnd, Color.Black, Color.FromArgb(r.Next(150, 256), r.Next(150, 256), r.Next(150, 256)));


                entries[i.ToString()] = td;
                treeView1.Nodes.Add(i.ToString(), i.ToString() + " - " + o.UnderlyingType.ToString().Replace("System.", ""));
                i++;
                pos = u.Offset;
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
            TreeData td = entries[e.Node.Name];
            if (td.mpo.IsNil)
            {
                lblinterp.Text = "Nil";
                return;
            }

            string s = "";
            if(td.mpo.UnderlyingType == typeof(System.Int32))
            {
                Int32 i = td.mpo.AsInt32();
                s = string.Format("Int32 :  0x{0:X2} : {0:d}", i);
            } else
            {
                s = td.mpo.UnderlyingType + " - " + td.mpo.ToString();
            }
            lblinterp.Text = s;

            hexBox1.Select(td.byteStart, td.byteEnd - td.byteStart);
            hexBox1.ScrollByteIntoView(td.byteStart);
            hexBox1.Invalidate();
        }
    }
}
