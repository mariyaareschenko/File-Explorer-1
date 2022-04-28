using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace File_Explorer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var root = new TreeNode() { Text = "C:", Tag = "c:\\" };
            treeView.Nodes.Add(root);
            Build(root);
            root.Expand();

        }
        private void Build(TreeNode parent)
        {
            var path = parent.Tag as string;
            parent.Nodes.Clear();
            try
            {
                foreach(var dir in Directory.GetDirectories(path))
                {
                    parent.Nodes.Add(new TreeNode(Path.GetFileName(dir), new[] { new TreeNode("...") }) { Tag = dir });
                }
                foreach(var file in Directory.GetFiles(path))
                {
                    parent.Nodes.Add(new TreeNode(Path.GetFileName(file), 1, 1) { Tag = file });
                }
            }
            catch
            {

            }
        }

        private void AddAllFolders(TreeNode tNode, string folderPath)
        {
            try
            {
                foreach (string folderNode in Directory.GetDirectories(folderPath))
                {
                    TreeNode subFolder = tNode.Nodes.Add(folderNode.Substring(folderNode.LastIndexOf(@"\") + 1));
                    subFolder.Tag = folderNode;
                    subFolder.Nodes.Add("Загрузка...");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void treeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            Build(e.Node);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            treeView.Sort();
            TreeNode treeNode = treeView.Nodes.Add("(Drive C:)");
            AddAllFolders(treeNode, @"C:\");
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string fileExtension = null;
            string dateMod = null;
            string folder = Convert.ToString(treeView.SelectedNode.Tag);
            int itemIndex = 0;
            listView.Items.Clear();
            if (treeView.SelectedNode.Nodes.Count == 1 && treeView.SelectedNode.Nodes[0].Text == "Загрузка...")
            {
                treeView.SelectedNode.Nodes.Clear();
                AddAllFolders(treeView.SelectedNode, Convert.ToString(treeView.SelectedNode.Tag));
            }
            if ((folder != null) && System.IO.Directory.Exists(folder))
            {
                try
                {
                    foreach (string file in System.IO.Directory.GetFiles(folder))
                    {
                        fileExtension = System.IO.Path.GetExtension(file);
                        dateMod = System.IO.File.GetLastWriteTime(file).ToString();
                        listView.Items.Add(file.Substring(file.LastIndexOf(@"\") + 1), file.ToString());
                        listView.Items[itemIndex].SubItems.Add(fileExtension.ToString() + " Файл");
                        listView.Items[itemIndex].SubItems.Add(dateMod.ToString());
                        itemIndex += 1;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка");
                }
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection lvSel = this.listView.SelectedItems;
            string strFileName = null;
            foreach (ListViewItem listItem in lvSel)
            {
                strFileName = treeView.SelectedNode.Tag + @"\" + listItem.Text;
                DataObject dataObject = new DataObject();
                string[] cbBoardFile = new string[1];
                cbBoardFile[0] = strFileName;
                dataObject.SetData(DataFormats.FileDrop, true, cbBoardFile);
                Clipboard.SetDataObject(dataObject);
                MessageBox.Show("Файл скопирован в буфер обмена");
            }
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IDataObject dataObject = Clipboard.GetDataObject();
            if (dataObject.GetDataPresent(DataFormats.FileDrop))
            {
                string[] strClipFile = (string[])dataObject.GetData(DataFormats.FileDrop);
                int i = 0;
                for (i = 0; i <= strClipFile.Length - 1; i++)
                {
                    if (File.Exists(treeView.SelectedNode.Tag + "/" + Path.GetFileName(strClipFile[i])))
                    {
                        File.Move(treeView.SelectedNode.Tag + "/" + Path.GetFileName(strClipFile[i]), treeView.SelectedNode.Tag + "temp");

                    }
                    File.Copy(strClipFile[i], treeView.SelectedNode.Tag + "/" + Path.GetFileName(strClipFile[i]));
                }
                MessageBox.Show("Файл вставлен из буфера обмена");
            }
        }
    }
}
