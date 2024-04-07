
namespace Archive
{
    partial class Data
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
            this.Title = new System.Windows.Forms.Label();
            this.Info = new System.Windows.Forms.Label();
            this.Edit = new System.Windows.Forms.Button();
            this.Link = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // Title
            // 
            this.Title.AutoSize = true;
            this.Title.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.Title.Dock = System.Windows.Forms.DockStyle.Top;
            this.Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Title.Location = new System.Drawing.Point(0, 0);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(77, 37);
            this.Title.TabIndex = 0;
            this.Title.Text = "Title";
            // 
            // Info
            // 
            this.Info.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Info.Dock = System.Windows.Forms.DockStyle.Left;
            this.Info.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Info.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.Info.Location = new System.Drawing.Point(0, 37);
            this.Info.Name = "Info";
            this.Info.Size = new System.Drawing.Size(464, 469);
            this.Info.TabIndex = 0;
            this.Info.Text = "Info";
            // 
            // Edit
            // 
            this.Edit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Edit.AutoSize = true;
            this.Edit.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Edit.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.Edit.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Edit.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Edit.Location = new System.Drawing.Point(838, 0);
            this.Edit.Name = "Edit";
            this.Edit.Size = new System.Drawing.Size(63, 34);
            this.Edit.TabIndex = 1;
            this.Edit.Text = "Edit";
            this.Edit.UseVisualStyleBackColor = false;
            this.Edit.Click += new System.EventHandler(this.Edit_Click);
            // 
            // Link
            // 
            this.Link.BackColor = System.Drawing.SystemColors.InfoText;
            this.Link.Dock = System.Windows.Forms.DockStyle.Top;
            this.Link.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Link.Location = new System.Drawing.Point(464, 37);
            this.Link.Name = "Link";
            this.Link.Size = new System.Drawing.Size(436, 30);
            this.Link.TabIndex = 2;
            this.Link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Link_LinkClicked);
            // 
            // Data
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(900, 506);
            this.Controls.Add(this.Link);
            this.Controls.Add(this.Edit);
            this.Controls.Add(this.Info);
            this.Controls.Add(this.Title);
            this.Name = "Data";
            this.Text = "Data";
            this.Load += new System.EventHandler(this.Data_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Title;
        private System.Windows.Forms.Label Info;
        private System.Windows.Forms.Button Edit;
        private System.Windows.Forms.LinkLabel Link;
    }
}