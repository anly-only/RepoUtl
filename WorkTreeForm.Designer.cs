namespace RepoUtl
{
    partial class WorkTreeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorkTreeForm));
            this.uc = new RepoUtl.WorkTreeUC();
            this.SuspendLayout();
            // 
            // uc
            // 
            this.uc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uc.Location = new System.Drawing.Point(0, 0);
            this.uc.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.uc.Name = "uc";
            this.uc.Size = new System.Drawing.Size(461, 308);
            this.uc.TabIndex = 0;
            // 
            // WorkTreeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(461, 308);
            this.Controls.Add(this.uc);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WorkTreeForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Worktrees";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WorkTreeForm_FormClosing);
            this.Load += new System.EventHandler(this.WorkTreeForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        internal WorkTreeUC uc;
    }
}