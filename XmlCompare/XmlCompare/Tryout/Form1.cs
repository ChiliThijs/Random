using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Tryout
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            XmlDocument xdoc1 = new XmlDocument();
            XmlDocument xdoc2 = new XmlDocument();
            XmlDocument xdoc3 = new XmlDocument();
            xdoc1.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\"?><foo><bar/><baz id=\"dus\"><foo/><bar/><baz/></baz></foo>");
            xdoc2.LoadXml("<?xml version=\"1.0\" encoding=\"utf-16\"?><foo><bar/><baz id=\"dus\"><foo/><bar/><baz/></baz></foo>");
            xdoc3.LoadXml("<?xml version=\"1.0\" encoding=\"iso-8859-1\"?><foo><bar/><baz name=\"bart\" id=\"dus\"><foo/><bar/><baz/></baz></foo>");

            xdoc1.diff(xdoc2);

        }
    }

    public static partial class Utils
    {
        public static void diff(this XmlDocument xdoc1, XmlDocument xdoc2)
        {
            if (xdoc1.InnerXml == xdoc2.InnerXml)
            {
                var wtf = xdoc1;
                return;
            }
            
            //FIXME:compare preamble and other non real stuff (comments, text, ...)

           
            foreach (XmlNode node in xdoc1.SelectNodes("*"))
            {
                int nodecount = xdoc1.SelectNodes(node.Name).Count;
                int testcount = xdoc2.SelectNodes(node.Name).Count;
                if (nodecount != testcount)
                {
                    var dbg = node;
                }


            }

        }

      

    }

}
