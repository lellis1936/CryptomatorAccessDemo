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
using CryptomatorTools.Helpers;

#pragma warning disable IDE1006,IDE0017

namespace CryptomatorDecryptor
{
    public partial class Decryptor : Form
    {
        public string Vault { get; set; }
        public string Password { get; set; }
        public CryptomatorHelper cryptomatorHelper { get; set; }
        public bool VaultIsOpen { get; set; }
        public string OutputFolder { get; set; }

        TreeNode clickedNode;
        MenuItem nodeMenuItem = new MenuItem("Decrypt");
        ContextMenu mnu = new ContextMenu();

        class NodeTag
        {
            public bool IsDirectory { get; set; }
            public string FilePath { get; set; }
        }

        public Decryptor()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            mnu.MenuItems.Add(nodeMenuItem);
            nodeMenuItem.Click += new EventHandler(nodeMenuItem_Click);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool bHandled = false;
            switch (keyData)
            {
                case Keys.F5:
                    if (VaultIsOpen)
                        RefreshTree();
                    bHandled = true;
                    break;
            }
            return bHandled;
        }

        private void treeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
                PopulateNodeCollection(e.Node.Nodes, ((NodeTag)e.Node.Tag).FilePath);
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (((NodeTag)treeView.SelectedNode.Tag).IsDirectory)
                btnDecrypt.Enabled = false;
            else
                btnDecrypt.Enabled = true;
        }

        private void OpenVault()
        {
            string folder = "";
            bool isValid = false;

            folderBrowserDialog1.Description = "Choose a Cryptomator vault folder.";
            folderBrowserDialog1.ShowNewFolderButton = false;
            while (!isValid)
            {
                DialogResult result = folderBrowserDialog1.ShowDialog(); // Show the dialog.
                if (result == DialogResult.OK) // Test result.
                {
                    folder = folderBrowserDialog1.SelectedPath;
                }

                if (result == DialogResult.Cancel)
                    return;

                isValid = File.Exists(Path.Combine(folder, "masterkey.cryptomator"));
                if (!isValid)
                {
                    folderBrowserDialog1.Reset();
                    MessageBox.Show("Selected folder is not a Cryptomator folder, please select a different folder or click Cancel.");
                }
                else
                {
                    Vault = folder;

                    DialogResult passwordResult = new Password(this).ShowDialog();
                    if (passwordResult == DialogResult.OK)
                        UnlockVault(Vault, Password);
                }
            }

        }

        private void CloseVault()
        {
            cryptomatorHelper = null;
            VaultIsOpen = false;
            treeView.Nodes.Clear();
            btnDecrypt.Enabled = false;
            refreshTreeF5ToolStripMenuItem.Enabled = false;
        }

        private void Decryptor_Load(object sender, EventArgs e)
        {
            treeView.HideSelection = false;
            refreshTreeF5ToolStripMenuItem.Enabled = false;
            btnDecrypt.Enabled = false;
            lblOutputFolder.Text = "";
            toolStripStatusLabel1.Text = "";

            treeView.Sorted = true;
        }

        private void UnlockVault(string vault, string password)
        {
            try
            {
                treeView.Nodes.Clear();
                cryptomatorHelper = CryptomatorHelper.Create(Password, Vault);
                VaultIsOpen = true;
                treeView.Nodes.Add(Vault);
                treeView.Nodes[0].Tag = new NodeTag { IsDirectory = true, FilePath = "" };
                PopulateNodeCollection(treeView.Nodes[0].Nodes, "");
                treeView.Focus();
                treeView.Nodes[0].Expand();
                refreshTreeF5ToolStripMenuItem.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Vault open failure");
            }

        }

        private void PopulateNodeCollection(TreeNodeCollection nodeCollection, string dirPath)
        {

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                nodeCollection.Clear();

                List<FolderInfo> dirs = cryptomatorHelper.GetFolders(dirPath);
                List<string> files = cryptomatorHelper.GetFiles(dirPath);
                foreach (FolderInfo dir in dirs)
                {
                    bool hasChildren = dir.HasChildren;
                    TreeNode node = new TreeNode(Path.GetFileName(dir.VirtualPath));
                    if (hasChildren)
                        node.Nodes.Add("...");  //placeholder

                    node.Tag = new NodeTag { FilePath = dir.VirtualPath, IsDirectory = true };
                    nodeCollection.Add(node);

                }
                foreach (string file in files)
                {
                    TreeNode node = new TreeNode(Path.GetFileName(file));
                    node.Tag = new NodeTag { FilePath = file, IsDirectory = false };
                    nodeCollection.Add(node);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }


        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            try
            {
                DecryptFileAtNode(treeView.SelectedNode);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Decryption failure");
            }
        }

        private void DecryptFileAtNode(TreeNode node)
        {
            if (OutputFolder == null)
            {
                MessageBox.Show("You must choose an output destination (Options menu) first", "No destination set", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                NodeTag tag = (NodeTag)node.Tag;
                string filename = Path.GetFileName(tag.FilePath);
                string outputPath = Path.Combine(OutputFolder, filename);

                if (File.Exists(outputPath))
                {
                    var result = MessageBox.Show("File already exists!  Do you want to overwrite this file?", "File Exists", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
                    if (result == DialogResult.No)
                        return;
                }

                toolStripStatusLabel1.Text = "Decrypting: " + filename + ". Please wait...";
                statusStrip1.Update();

                cryptomatorHelper.DecryptFile(tag.FilePath, outputPath);

                toolStripStatusLabel1.Text = "\"" + filename + "\"" + " decrypted successfully.";
                statusStrip1.Update();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message, ex);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                treeView.Focus();

            }
        }

        private void RefreshTree()
        {
            treeView.Nodes.Clear();
            PopulateNodeCollection(treeView.Nodes, "");
            btnDecrypt.Enabled = false;
        }

        private void setTargetFolderForAllOperationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChooseOutputFolder();
        }

        private DialogResult ChooseOutputFolder()
        {
            folderBrowserDialog2.Description = "Choose a destination for decryption.  This destination will be used for ALL decryption in the session (until changed via the Option menu).";

            folderBrowserDialog2.ShowNewFolderButton = true;

            DialogResult result = folderBrowserDialog2.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                OutputFolder = folderBrowserDialog2.SelectedPath;
                lblOutputFolder.Text = "=> " + OutputFolder;
            }

            return result;
        }

        private void refreshTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshTree();
        }

        private void openVaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenVault();
            if (VaultIsOpen)
            {
                openVaultToolStripMenuItem.Enabled = false;
                closeVaultToolStripMenuItem.Enabled = true;
            }
        }

        private void closeVaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseVault();
            openVaultToolStripMenuItem.Enabled = true;
            closeVaultToolStripMenuItem.Enabled = false;
        }

        private void aExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void refreshTreeF5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshTree();

        }

        void nodeMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
                DecryptFileAtNode(clickedNode);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Decryption failure");
            }
        }

        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                clickedNode = e.Node;
                treeView.SelectedNode = clickedNode;
                if (((NodeTag)e.Node.Tag).IsDirectory)
                    return;
                mnu.Show(treeView, e.Location);
            }
        }

        private void licenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "copyright 2019, L. Ellis\n\n" +
                "This program and related source code are licensed under GPLv3\n\n",
                "License"
                );
        }

    }
}
