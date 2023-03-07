namespace RepoUtl
{
    partial class WorkTreeUC
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.bnAdd = new System.Windows.Forms.Button();
            this.bnRemove = new System.Windows.Forms.Button();
            this.bnSelectBranch = new System.Windows.Forms.Button();
            this.bnSelectWorkTree = new System.Windows.Forms.Button();
            this.tbWorkTree = new System.Windows.Forms.TextBox();
            this.tbRepo = new System.Windows.Forms.TextBox();
            this.bnExplore = new System.Windows.Forms.Button();
            this.bnExploreRepo = new System.Windows.Forms.Button();
            this.bnSelectRepo = new System.Windows.Forms.Button();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // bnAdd
            // 
            this.bnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnAdd.Location = new System.Drawing.Point(46, 177);
            this.bnAdd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.bnAdd.Name = "bnAdd";
            this.bnAdd.Size = new System.Drawing.Size(155, 36);
            this.bnAdd.TabIndex = 4;
            this.bnAdd.Text = "Add";
            this.bnAdd.UseVisualStyleBackColor = true;
            this.bnAdd.Click += new System.EventHandler(this.bnAdd_Click);
            // 
            // bnRemove
            // 
            this.bnRemove.Location = new System.Drawing.Point(252, 177);
            this.bnRemove.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.bnRemove.Name = "bnRemove";
            this.bnRemove.Size = new System.Drawing.Size(155, 36);
            this.bnRemove.TabIndex = 5;
            this.bnRemove.Text = "Remove...";
            this.bnRemove.UseVisualStyleBackColor = true;
            this.bnRemove.Click += new System.EventHandler(this.bnDelete_Click);
            // 
            // bnSelectBranch
            // 
            this.bnSelectBranch.Location = new System.Drawing.Point(252, 98);
            this.bnSelectBranch.Margin = new System.Windows.Forms.Padding(3, 12, 3, 4);
            this.bnSelectBranch.Name = "bnSelectBranch";
            this.bnSelectBranch.Size = new System.Drawing.Size(155, 36);
            this.bnSelectBranch.TabIndex = 3;
            this.bnSelectBranch.Text = "Branch...";
            this.bnSelectBranch.UseVisualStyleBackColor = true;
            this.bnSelectBranch.Click += new System.EventHandler(this.bnSelectBranch_Click);
            // 
            // bnSelectWorkTree
            // 
            this.bnSelectWorkTree.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnSelectWorkTree.Location = new System.Drawing.Point(46, 98);
            this.bnSelectWorkTree.Margin = new System.Windows.Forms.Padding(3, 12, 3, 4);
            this.bnSelectWorkTree.Name = "bnSelectWorkTree";
            this.bnSelectWorkTree.Size = new System.Drawing.Size(155, 36);
            this.bnSelectWorkTree.TabIndex = 2;
            this.bnSelectWorkTree.Text = "Worktree...";
            this.bnSelectWorkTree.UseVisualStyleBackColor = true;
            this.bnSelectWorkTree.Click += new System.EventHandler(this.bnSelectWorkTree_Click);
            // 
            // tbWorkTree
            // 
            this.tbWorkTree.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbWorkTree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutPanel6.SetColumnSpan(this.tbWorkTree, 3);
            this.tbWorkTree.Location = new System.Drawing.Point(3, 142);
            this.tbWorkTree.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbWorkTree.Name = "tbWorkTree";
            this.tbWorkTree.ReadOnly = true;
            this.tbWorkTree.Size = new System.Drawing.Size(449, 27);
            this.tbWorkTree.TabIndex = 13;
            this.tbWorkTree.TabStop = false;
            this.tbWorkTree.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbWorkTree.TextChanged += new System.EventHandler(this.tbWorkTree_TextChanged);
            // 
            // tbRepo
            // 
            this.tbRepo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbRepo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutPanel6.SetColumnSpan(this.tbRepo, 3);
            this.tbRepo.Location = new System.Drawing.Point(3, 55);
            this.tbRepo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbRepo.Name = "tbRepo";
            this.tbRepo.ReadOnly = true;
            this.tbRepo.Size = new System.Drawing.Size(449, 27);
            this.tbRepo.TabIndex = 9;
            this.tbRepo.TabStop = false;
            this.tbRepo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bnExplore
            // 
            this.bnExplore.Location = new System.Drawing.Point(252, 221);
            this.bnExplore.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.bnExplore.Name = "bnExplore";
            this.bnExplore.Size = new System.Drawing.Size(155, 36);
            this.bnExplore.TabIndex = 6;
            this.bnExplore.Text = "Explore...";
            this.bnExplore.UseVisualStyleBackColor = true;
            this.bnExplore.Click += new System.EventHandler(this.bnExplore_Click);
            // 
            // bnExploreRepo
            // 
            this.bnExploreRepo.Location = new System.Drawing.Point(252, 12);
            this.bnExploreRepo.Margin = new System.Windows.Forms.Padding(3, 12, 3, 4);
            this.bnExploreRepo.Name = "bnExploreRepo";
            this.bnExploreRepo.Size = new System.Drawing.Size(155, 35);
            this.bnExploreRepo.TabIndex = 1;
            this.bnExploreRepo.Text = "Explore...";
            this.bnExploreRepo.UseVisualStyleBackColor = true;
            this.bnExploreRepo.Click += new System.EventHandler(this.bnExploreRepo_Click);
            // 
            // bnSelectRepo
            // 
            this.bnSelectRepo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnSelectRepo.Location = new System.Drawing.Point(46, 12);
            this.bnSelectRepo.Margin = new System.Windows.Forms.Padding(3, 12, 3, 4);
            this.bnSelectRepo.Name = "bnSelectRepo";
            this.bnSelectRepo.Size = new System.Drawing.Size(155, 35);
            this.bnSelectRepo.TabIndex = 0;
            this.bnSelectRepo.Text = "Select...";
            this.bnSelectRepo.UseVisualStyleBackColor = true;
            this.bnSelectRepo.Click += new System.EventHandler(this.bnSelectRepo_Click);
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 3;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tableLayoutPanel6.Controls.Add(this.bnExplore, 2, 5);
            this.tableLayoutPanel6.Controls.Add(this.bnAdd, 0, 4);
            this.tableLayoutPanel6.Controls.Add(this.bnSelectBranch, 2, 2);
            this.tableLayoutPanel6.Controls.Add(this.bnExploreRepo, 2, 0);
            this.tableLayoutPanel6.Controls.Add(this.tbWorkTree, 0, 3);
            this.tableLayoutPanel6.Controls.Add(this.bnSelectWorkTree, 0, 2);
            this.tableLayoutPanel6.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanel6.Controls.Add(this.bnSelectRepo, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.tbRepo, 0, 1);
            this.tableLayoutPanel6.Controls.Add(this.bnRemove, 2, 4);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 6;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(455, 301);
            this.tableLayoutPanel6.TabIndex = 15;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(207, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 51);
            this.label2.TabIndex = 6;
            this.label2.Text = "repo";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // WorkTreeUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel6);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "WorkTreeUC";
            this.Size = new System.Drawing.Size(455, 301);
            this.Load += new System.EventHandler(this.WorkTreeUC_Load);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button bnAdd;
        private System.Windows.Forms.Button bnRemove;
        private System.Windows.Forms.Button bnSelectBranch;
        private System.Windows.Forms.Button bnSelectWorkTree;
        private System.Windows.Forms.TextBox tbWorkTree;
        private System.Windows.Forms.TextBox tbRepo;
        private System.Windows.Forms.Button bnExplore;
        private System.Windows.Forms.Button bnExploreRepo;
        private System.Windows.Forms.Button bnSelectRepo;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.Label label2;
    }
}
