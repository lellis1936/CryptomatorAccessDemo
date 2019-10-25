namespace CryptomatorDecryptor
{
    partial class Decryptor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openVaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeVaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshTreeF5ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setTargetFolderForAllOperationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.licenseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderBrowserDialog2 = new System.Windows.Forms.FolderBrowserDialog();
            this.treeView = new System.Windows.Forms.TreeView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnDecrypt = new System.Windows.Forms.Button();
            this.lblOutputFolder = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 503);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 17, 0);
            this.statusStrip1.Size = new System.Drawing.Size(615, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.toolStripStatusLabel1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripStatusLabel1.ForeColor = System.Drawing.SystemColors.Highlight;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(127, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel2";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.licenseToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(615, 25);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openVaultToolStripMenuItem,
            this.closeVaultToolStripMenuItem,
            this.refreshTreeF5ToolStripMenuItem,
            this.aExitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 21);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openVaultToolStripMenuItem
            // 
            this.openVaultToolStripMenuItem.Name = "openVaultToolStripMenuItem";
            this.openVaultToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.openVaultToolStripMenuItem.Text = "Open Vault";
            this.openVaultToolStripMenuItem.Click += new System.EventHandler(this.openVaultToolStripMenuItem_Click);
            // 
            // closeVaultToolStripMenuItem
            // 
            this.closeVaultToolStripMenuItem.Name = "closeVaultToolStripMenuItem";
            this.closeVaultToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.closeVaultToolStripMenuItem.Text = "Close Vault";
            this.closeVaultToolStripMenuItem.Click += new System.EventHandler(this.closeVaultToolStripMenuItem_Click);
            // 
            // refreshTreeF5ToolStripMenuItem
            // 
            this.refreshTreeF5ToolStripMenuItem.Name = "refreshTreeF5ToolStripMenuItem";
            this.refreshTreeF5ToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.refreshTreeF5ToolStripMenuItem.Text = "Refresh Tree (F5)";
            this.refreshTreeF5ToolStripMenuItem.Click += new System.EventHandler(this.refreshTreeF5ToolStripMenuItem_Click);
            // 
            // aExitToolStripMenuItem
            // 
            this.aExitToolStripMenuItem.Name = "aExitToolStripMenuItem";
            this.aExitToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.aExitToolStripMenuItem.Text = "Exit";
            this.aExitToolStripMenuItem.Click += new System.EventHandler(this.aExitToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setTargetFolderForAllOperationsToolStripMenuItem});
            this.optionsToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(66, 21);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // setTargetFolderForAllOperationsToolStripMenuItem
            // 
            this.setTargetFolderForAllOperationsToolStripMenuItem.Name = "setTargetFolderForAllOperationsToolStripMenuItem";
            this.setTargetFolderForAllOperationsToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.setTargetFolderForAllOperationsToolStripMenuItem.Text = "Target folder for decyption";
            this.setTargetFolderForAllOperationsToolStripMenuItem.Click += new System.EventHandler(this.setTargetFolderForAllOperationsToolStripMenuItem_Click);
            // 
            // licenseToolStripMenuItem
            // 
            this.licenseToolStripMenuItem.Name = "licenseToolStripMenuItem";
            this.licenseToolStripMenuItem.Size = new System.Drawing.Size(58, 21);
            this.licenseToolStripMenuItem.Text = "License";
            this.licenseToolStripMenuItem.Click += new System.EventHandler(this.licenseToolStripMenuItem_Click);
            // 
            // treeView
            // 
            this.treeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeView.Location = new System.Drawing.Point(20, 20);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(575, 388);
            this.treeView.TabIndex = 11;
            this.treeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView_BeforeExpand);
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            this.treeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_NodeMouseClick);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.treeView);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 25);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(20);
            this.panel2.Size = new System.Drawing.Size(615, 478);
            this.panel2.TabIndex = 13;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnDecrypt);
            this.panel3.Controls.Add(this.lblOutputFolder);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(20, 408);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(8);
            this.panel3.Size = new System.Drawing.Size(575, 50);
            this.panel3.TabIndex = 18;
            // 
            // btnDecrypt
            // 
            this.btnDecrypt.Location = new System.Drawing.Point(12, 18);
            this.btnDecrypt.Margin = new System.Windows.Forms.Padding(4);
            this.btnDecrypt.Name = "btnDecrypt";
            this.btnDecrypt.Size = new System.Drawing.Size(88, 26);
            this.btnDecrypt.TabIndex = 10;
            this.btnDecrypt.Text = "Decrypt";
            this.btnDecrypt.UseVisualStyleBackColor = true;
            this.btnDecrypt.Click += new System.EventHandler(this.btnDecrypt_Click);
            // 
            // lblOutputFolder
            // 
            this.lblOutputFolder.AutoSize = true;
            this.lblOutputFolder.Location = new System.Drawing.Point(117, 24);
            this.lblOutputFolder.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblOutputFolder.Name = "lblOutputFolder";
            this.lblOutputFolder.Size = new System.Drawing.Size(91, 15);
            this.lblOutputFolder.TabIndex = 9;
            this.lblOutputFolder.Text = "lblOutputFolder";
            // 
            // panel4
            // 
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(0, 489);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(615, 14);
            this.panel4.TabIndex = 15;
            // 
            // Decryptor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(615, 525);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Decryptor";
            this.Text = "Cryptomator Decryptor";
            this.Load += new System.EventHandler(this.Decryptor_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setTargetFolderForAllOperationsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openVaultToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeVaultToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aExitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshTreeF5ToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog2;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnDecrypt;
        private System.Windows.Forms.Label lblOutputFolder;
        private System.Windows.Forms.ToolStripMenuItem licenseToolStripMenuItem;
    }
}