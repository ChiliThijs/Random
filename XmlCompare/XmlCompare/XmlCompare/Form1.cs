using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XmlCompare
{
    public partial class Form1 : FileBrowser
    {

        public Form1()
        {
            InitializeComponent();
            RootDirectory = @"C:\";
            TreeDecorator(treeView1);
            treeView1.Nodes[0].Expand();

        }

    }
}
