using System; // http://msdn.microsoft.com/en-us/library/System.aspx
using System.Collections.Generic; // http://msdn.microsoft.com/en-us/library/System.Collections.Generic.aspx
using System.Drawing; // http://msdn.microsoft.com/en-us/library/System.Drawing.aspx
using System.Linq; // http://msdn.microsoft.com/en-us/library/System.Linq.aspx
using System.Runtime.InteropServices; // http://msdn.microsoft.com/en-us/library/System.Runtime.InteropServices.aspx
using System.Text; // http://msdn.microsoft.com/en-us/library/System.Text.aspx
using System.Windows.Forms; // http://msdn.microsoft.com/en-us/library/System.Windows.Forms.aspx
using System.IO;
using System.Xml; // http://msdn.microsoft.com/en-us/library/System.IO.aspx
using System.Configuration;
using System.Management;
using System.ComponentModel; // http://msdn.microsoft.com/en-us/library/System.Configuration.aspx
namespace XmlCompare
{




    public class FileBrowser : Form
    {

        /* menus, actions, file actions */
        #region fileactions
        public static bool HasContext(string ext)
        {
            if (!TreeNodeExpandActions.ContainsKey(ext)) return false;
            if (TreeNodeExpandActions[ext].ToList().Count() > 0) return true;
            return false;
        }
        
        [TypeConverter(typeof(Converse))]
        public static Dictionary<string, List<Delegate>> TreeNodeExpandActions = new Dictionary<string, List<Delegate>>()
        {
            { "_default_", new List<Delegate>(){(Action)delegate() { } }},
            { ".txt", new List<Delegate>(){(Func<string,TreeNode>)delegate(string path) { try { using ( StreamReader sr = new StreamReader(path)) { return new TreeNode(sr.ReadToEnd()); } } catch {} return null; } } },
            { ".xml", new List<Delegate>(){(Func<string,TreeNode>) delegate(string path) { 
                                            XmlDocument doc = new XmlDocument(); 
                                            doc.Load(path); 
                                        if (doc != null) { 
                                            return XmlDocToTreeNode(doc); 
                                        } 
                                        return null; },
                (Func<string,MenuItem>)delegate(string path) { return new MenuItem("dus"); } }} ,
            { "_default", new List<Delegate> { 
                (Func<string,MenuItem>)delegate(string path) { MessageBox.Show("dus"); return null; }
            } },
        };

        // implicit and operator overloading for all
        public class Converse : TypeConverter
        {
        }

        public sealed class MenuFactory
        {
            public static ContextMenu ContextMenuForPath(string path)
            {
                if (!File.Exists(path)) return null;
                string ext = Path.GetExtension(path);
                if (HasContext(ext)) return GetContextMenu(path);
                return null;
//                return new ContextMenu(new MenuItem[] { new MenuItem("dus"), new MenuItem("bar") });
            }

            private static ContextMenu GetContextMenu(string path)
            {
                List<Delegate> menu = getContextForType(path, typeof(MenuItem));
                if (menu.Count() < 1) return null;
                ContextMenu contextMenu = new ContextMenu();
                foreach (Delegate del in menu)
                {
                    contextMenu.MenuItems.Add(del.ToString());
                }
                return contextMenu;
            }

            private static List<Delegate> getContextForType(string path, Type type)
            {
                return TreeNodeExpandActions[Path.GetExtension(path)].Where(p => p.GetType() == typeof(Func<string, MenuItem>)).ToList<Delegate>();
            }
        }
        #endregion fileactions

        private static TreeNode XmlDocToTreeNode(XmlDocument doc)
        {
            return XmlToTreeNode(doc.DocumentElement);
        }
        private static TreeNode XmlToTreeNode(XmlNode xmlNode)
        {
            TreeNode rootNode = new TreeNode(xmlNode.Name);
            foreach (XmlNode node in xmlNode.SelectNodes("*"))
            {
                if (node.HasChildNodes) rootNode.Nodes.Add(XmlToTreeNode(node));
                else rootNode.Nodes.Add(new TreeNode() { Text = node.Name, Tag = "about:blank" });
            }
            rootNode.ExpandAll();
            return rootNode;
        }

        public string CWD;
        public string CurrentWorkingDirectory { get; set; }
        public string RootDirectory { get; set; }

        public TreeView TargetTreeView { get; set; }

        private Dictionary<string, Image> _iconCache = new Dictionary<string, Image>();

        const int maxFileNameDisplayLength = 100;
        const string directoryKey = "_directory_";


        public FileBrowser()
        {



            //            public Dictionary<string,Delegate> ExpandActions { get; set; }

            //if (TargetTreeView == null) { return; } // running unitialized

        }
        public void TreeDecorator(TreeView tv)
        {
            TargetTreeView = tv;

            TargetTreeView.ImageList = new ImageList();
            TargetTreeView.ImageList.Images.Add(RootDirectory, GetSystemIcon(RootDirectory));

            ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDisk");
            ManagementObjectCollection moc = mos.Get();
            foreach (ManagementObject drive in moc)
            {
                string driveName = drive["Name"].ToString() + "\\";
                TargetTreeView.ImageList.Images.Add(driveName, GetSystemIcon(RootDirectory));
                TreeNode treeNode = new TreeNode() { Text = driveName, Tag = driveName, ImageKey = directoryKey, SelectedImageKey = directoryKey };
                treeNode.Nodes.Add("dus");
                TargetTreeView.Nodes.Add(treeNode);
            }



            /* events */
            #region targettreeview events


            TargetTreeView.MouseClick += TargetTreeView_MouseClick;


            TargetTreeView.ItemDrag += TargetTreeView_ItemDrag;
            TargetTreeView.AllowDrop = false;
            //TargetTreeView.DragEnter
            TargetTreeView.BeforeCollapse += delegate(object ds, TreeViewCancelEventArgs de)
            {
                de.Node.Nodes.Clear();
                de.Node.Nodes.Add("dus");//make it expandable ;)
            };

            Dictionary<string, TreeNode> nodeCache = new Dictionary<string, TreeNode>();

            TargetTreeView.BeforeExpand += delegate(object ds, TreeViewCancelEventArgs de)
            {
                if (de.Node.Tag == null) return;
                string path = de.Node.Tag.ToString();
                string ext = Path.GetExtension(path);
                if (nodeCache.ContainsKey(path))
                {
                    de.Node.Nodes.Clear();
                    de.Node.Nodes.Add(nodeCache[path]);
                }
                if (TreeNodeExpandActions.ContainsKey(ext))
                {
                    de.Node.Nodes.Clear();

                    foreach (Delegate del in (List<Delegate>)TreeNodeExpandActions[ext].Where(
                                        p => p.GetType() == typeof(Func<string, TreeNode>)).ToList<Delegate>())
                    {
                        // execute every known delegate for this filetype and add the returning treenodes to the list
                        de.Node.Nodes.Add((TreeNode)((Func<string, TreeNode>)del)(de.Node.Tag.ToString()));
                    }

                }
                else
                {
                    de.Node.Nodes.Clear();
                    NodeDecorator(de.Node);
                }
            };


        }
        private void TargetTreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            try
            {
                DoDragDrop(((TreeNode)e.Item).Tag.ToString(), DragDropEffects.Copy);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debugger.Log(1, "Exception", ex.Message);
            }
        }

        private void TargetTreeView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenu contextMenu;
                string path=((TreeView)sender).SelectedNode.Tag.ToString();
                if ( null == ( contextMenu = MenuFactory.ContextMenuForPath(path))) {
                    contextMenu = new ContextMenu();
                }
                MenuItem openButton = new MenuItem() { Text = "&Open" };
                contextMenu.MenuItems.Add(openButton);
                openButton.Click += (ds, de) => MessageBox.Show("opening " + path);
                //openButton.Click += (EventHandler)((TreeView)sender).SelectedNode.Tag;
                contextMenu.Show((Control)sender, e.Location);
            }
        }

        //private void openButton_Click(object sender, EventArgs e)
        //{
        //    throw new NotImplementedException();
        //}
            #endregion targettreeview events



        public TreeNode NodeDecorator(TreeNode node)
        {
            string pathName = node.Tag as string;
            if (!Directory.Exists(pathName)) { return null; } // prolly will never happen, but still, someone might have moved it 
            DirectoryInfo directoryInfo = new DirectoryInfo(pathName);
            //                node.Nodes.Clear();
            foreach (var dir in directoryInfo.GetDirectories().OrderBy(p => p.FullName))
            {
                if (!TargetTreeView.ImageList.Images.ContainsKey(dir.FullName)) TargetTreeView.ImageList.Images.Add(dir.FullName, GetSystemIcon(dir.FullName));
                TreeNode fileNode = new TreeNode()
                {
                    //support custom directory icons, cache dirs by path:
                    Text = dir.Name.Substring(0, dir.Name.Length > maxFileNameDisplayLength ? maxFileNameDisplayLength : dir.Name.Length),//truncate
                    Tag = dir.FullName,
                    ImageKey = dir.FullName,
                    SelectedImageKey = dir.FullName
                };
                if (Directory.Exists(dir.FullName)) // FIXME: if not readable for user, don't make it expandable!
                    fileNode.Nodes.Add("dus");
                node.Nodes.Add(fileNode);
            }
            foreach (var file in directoryInfo.GetFiles().OrderBy(p => p.FullName))
            {
                string ext = Path.GetExtension(file.FullName);
                if (!TargetTreeView.ImageList.Images.ContainsKey(ext)) TargetTreeView.ImageList.Images.Add(ext, GetSystemIcon(file.FullName));
                TreeNode fileNode = new TreeNode()
                {
                    Text = file.Name.Substring(0, file.Name.Length > maxFileNameDisplayLength ? maxFileNameDisplayLength : file.Name.Length),
                    Tag = file.FullName,
                    ImageKey = ext,
                    SelectedImageKey = ext
                };
                if (NodeIsExpandable(file.FullName))
                    fileNode.Nodes.Add("dus");
                node.Nodes.Add(fileNode);
            }
            return node;
        }





        public Image GetSystemIcon(string path)
        {//FIXME: set and use last mod date of file to invalidate cache.
            if (_iconCache.ContainsKey(path))
            {
                return _iconCache[path];
            }

            try
            {
                SHFILEINFO shFileInfo = new SHFILEINFO();
                IntPtr iconPtr = Win32.SHGetFileInfo(path, 0, ref shFileInfo, (uint)Marshal.SizeOf(shFileInfo), Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON);
                _iconCache.Add(path, Icon.FromHandle(shFileInfo.hIcon).ToBitmap());
            }
            catch (Exception e) { }

            //if we still have no icon here for some reason, return one
            if (!_iconCache.ContainsKey(path))
                _iconCache.Add(path, new System.Drawing.Bitmap(8, 8));
            return _iconCache[path];
        }
        private bool NodeIsExpandable(string path)
        {
            return Directory.Exists(path) || TreeNodeExpandActions.ContainsKey(Path.GetExtension(path));
        }

    }
    /* OpenFileDialog openFileDialog1 = new OpenFileDialog();
openFileDialog1.InitialDirectory = "c:\\temp\\";
openFileDialog1.Filter = "All files (*.*)|*.*";
openFileDialog1.FilterIndex = 2;
openFileDialog1.RestoreDirectory = true ;

*/

    // http://support.microsoft.com/kb/319350 How to use the SHGetFileInfo function to get the icons that are associated with files in Visual C# .NET
    [StructLayout(LayoutKind.Sequential)]
    public struct SHFILEINFO
    {
        public IntPtr hIcon;
        public IntPtr iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    };

    class Win32
    {
        public const uint SHGFI_ICON = 0x100;
        public const uint SHGFI_LARGEICON = 0x0; // 'Large icon
        public const uint SHGFI_SMALLICON = 0x1; // 'Small icon

        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);
    }

}
