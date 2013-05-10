using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;

namespace XmlCompare
{

    public partial class MagicDrop : UserControl,IComponent
    {
        public MagicDrop()
        {
            InitializeComponent();
        }

        private void MagicDrop_DragLeave(object sender, EventArgs e)
        {

        }

        private void MagicDrop_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void MagicDrop_DragDrop(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
            string path = "";
            if (e.Data.GetDataPresent(typeof(System.String)))
                path = (string)e.Data.GetData(typeof(System.String));

            if (!String.IsNullOrEmpty(path))
            {
                MagicOpen(path);
            }
        }

        private void MagicDrop_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(System.String))) 
                e.Effect = DragDropEffects.Copy;

        }

        private void TabControl_DragDrop(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;
        }

        public void MagicOpen(string path)
        {
            TabPage tabPage = new TabPage(path) { AllowDrop = false } ;
            WebBrowser webBrowser = new WebBrowser() { Dock = DockStyle.Fill };
            webBrowser.Navigate(new Uri(path));
            tabPage.Controls.Add(webBrowser);
            tabControl1.TabPages.Add(tabPage);
            //tabControl1.TabPages[tabControl1.TabPages.Count - 1].Focus();
            tabControl1.SelectedTab = tabPage;
        }

        private void tabControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                for (int x = 0; x < ((TabControl)sender).TabCount; x++ )
                {
                    if (tabControl1.GetTabRect(x).Contains(e.Location))
                    {
                        tabControl1.TabPages.RemoveAt(x);

                    }
                }
            }
        }

        internal static void openTab()
        {
            throw new NotImplementedException();
        }

        internal static void openTab(MagicDrop drop)
        {

          
        }
    }
}
