using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
namespace XmlCompare
{
    public partial class Form1 : FileBrowser
    {
        //private Properties.Settings _settings = new Properties.Settings();
        public System.Windows.Forms.Control.ControlCollection controls { get { return splitContainer1.Panel2.Controls; } }
        private string _configDir;
        public string ConfigDir
        {
            get
            {
                if (_configDir == null)
                    _configDir = Properties.Settings.Default.RootDirectory;
                return _configDir;
            }
            set
            {
                Properties.Settings.Default.RootDirectory = value;
            }
        }
        private string _configCWD;
        public string ConfigCWD
        {
            get
            {
                if (_configCWD == null)
                    _configCWD = Properties.Settings.Default.CWD;
                return _configCWD;
            }
            set
            {
                Properties.Settings.Default.CWD  = value;
            }
        }
        public Form1()
        {
            InitializeComponent();
            RootDirectory = @"C:\flagglethort";
            
            TreeDecorator(treeView1);
            treeView1.Nodes[0].Expand();
            this.FormClosing += delegate(object ds, FormClosingEventArgs de) { ConfigDir = RootDirectory; };
            treeView1.Click +=treeView1_Click;
            treeView1.MouseClick+=treeView1_MouseClick;

        }

        private void treeView1_MouseClick(object sender, MouseEventArgs e)
        {
           
           // throw new NotImplementedException();
        }

        private void treeView1_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void splitToolStripMenuItem_Click(object sender, EventArgs e)
        {

            SplitCanvas();
        }

        private void SplitCanvas()
        {
            int controlCount = controls.Count;
            Control lastControl = controls[controlCount - 1];
            SplitContainer splitContainer = new SplitContainer() { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal };
            splitContainer.Panel1.Controls.Add(lastControl);
            splitContainer.Panel2.Controls.Add(new MagicDrop() { Dock = DockStyle.Fill });
            controls.Remove(lastControl);
            controls.Add(splitContainer);
        }

        
        private void unsplitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int controlCount = controls.Count;
            Control lastControl = controls[controlCount - 1];
            if (lastControl is MagicDrop)
            {
                throw new NotImplementedException();
            }
            else if (lastControl is SplitContainer)
            {
                //Control.ControlCollection keepControls = ((SplitContainer)lastControl).Panel1.Controls;
                //Control.ControlCollection lastControls = ((SplitContainer)lastControl).Panel2.Controls;
                //if (lastControls.Count > 0)
                //{
                //    //move bottom to toptab
                //    TabPage tabPage = new TabPage();
                //    tabPage.Controls.Add(lastControls[0]);
                //    ((TabControl)((MagicDrop)controls[0]).Controls[0]).TabPages.Add(tabPage);
                //}
                //if (keepControls.Count > 0) 
                //{
                //    controls.Add(keepControls[0]);
                //}
            }
            else
            {
                TabPage tabPage = new TabPage();
            }

        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode.Tag == null) { return; }
            TabPage tabPage = new TabPage();
            string path = treeView1.SelectedNode.Tag.ToString();
            if (splitContainer1.Panel2.Controls[0] is SplitContainer && ((SplitterPanel)((SplitContainer)splitContainer1.Panel2.Controls[0]).Controls[0]).Controls[0] is MagicDrop )
            {
                ((MagicDrop)((SplitterPanel)((SplitContainer)splitContainer1.Panel2.Controls[0]).Controls[0]).Controls[0]).MagicOpen(path);
            }
            else if (splitContainer1.Panel2.Controls[0] is MagicDrop)
            {
                ((MagicDrop)splitContainer1.Panel2.Controls[0]).MagicOpen(path);
            }
        }

        private void splitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SplitCanvas();
        }

        private void bookmarksToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void xmlCompareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(splitContainer1.Panel2.Controls[0].Controls.Count.ToString());
        }

    }
}

