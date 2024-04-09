
namespace Archive
{
    partial class DetailedSearchForm
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
            this.Panel = new System.Windows.Forms.Panel();
            this.SearchButton = new System.Windows.Forms.Button();
            this.rateLabel = new System.Windows.Forms.Label();
            this.Rate = new System.Windows.Forms.ComboBox();
            this.SearchBar = new System.Windows.Forms.TextBox();
            this.TagsLabel = new System.Windows.Forms.Label();
            this.SelectedTags = new System.Windows.Forms.ListBox();
            this.Tags = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // Panel
            // 
            this.Panel.Dock = System.Windows.Forms.DockStyle.Right;
            this.Panel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Panel.Location = new System.Drawing.Point(316, 0);
            this.Panel.Name = "Panel";
            this.Panel.Size = new System.Drawing.Size(843, 654);
            this.Panel.TabIndex = 1;
            // 
            // SearchButton
            // 
            this.SearchButton.AutoSize = true;
            this.SearchButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.SearchButton.Location = new System.Drawing.Point(220, 0);
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.Size = new System.Drawing.Size(90, 35);
            this.SearchButton.TabIndex = 2;
            this.SearchButton.Text = "Search";
            this.SearchButton.UseVisualStyleBackColor = true;
            this.SearchButton.Click += new System.EventHandler(this.SearchButton_Click);
            // 
            // rateLabel
            // 
            this.rateLabel.AutoSize = true;
            this.rateLabel.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.rateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.rateLabel.Location = new System.Drawing.Point(7, 94);
            this.rateLabel.Name = "rateLabel";
            this.rateLabel.Size = new System.Drawing.Size(69, 25);
            this.rateLabel.TabIndex = 11;
            this.rateLabel.Text = "Rate :";
            // 
            // Rate
            // 
            this.Rate.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Rate.FormattingEnabled = true;
            this.Rate.Location = new System.Drawing.Point(82, 91);
            this.Rate.Name = "Rate";
            this.Rate.Size = new System.Drawing.Size(63, 33);
            this.Rate.TabIndex = 10;
            // 
            // SearchBar
            // 
            this.SearchBar.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.SearchBar.Location = new System.Drawing.Point(5, 41);
            this.SearchBar.Name = "SearchBar";
            this.SearchBar.Size = new System.Drawing.Size(305, 44);
            this.SearchBar.TabIndex = 12;
            // 
            // TagsLabel
            // 
            this.TagsLabel.AutoSize = true;
            this.TagsLabel.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.TagsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.TagsLabel.Location = new System.Drawing.Point(86, 127);
            this.TagsLabel.Name = "TagsLabel";
            this.TagsLabel.Size = new System.Drawing.Size(150, 25);
            this.TagsLabel.TabIndex = 16;
            this.TagsLabel.Text = ": Select Tags :";
            // 
            // SelectedTags
            // 
            this.SelectedTags.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.SelectedTags.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.SelectedTags.FormattingEnabled = true;
            this.SelectedTags.ItemHeight = 25;
            this.SelectedTags.Location = new System.Drawing.Point(172, 157);
            this.SelectedTags.Name = "SelectedTags";
            this.SelectedTags.Size = new System.Drawing.Size(138, 179);
            this.SelectedTags.TabIndex = 15;
            this.SelectedTags.SelectedIndexChanged += new System.EventHandler(this.SelectedTags_SelectedIndexChanged_1);
            // 
            // Tags
            // 
            this.Tags.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.Tags.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Tags.FormattingEnabled = true;
            this.Tags.ItemHeight = 25;
            this.Tags.Location = new System.Drawing.Point(5, 156);
            this.Tags.Name = "Tags";
            this.Tags.Size = new System.Drawing.Size(140, 179);
            this.Tags.TabIndex = 14;
            this.Tags.SelectedIndexChanged += new System.EventHandler(this.Tags_SelectedIndexChanged);
            // 
            // DetailedSearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(1159, 654);
            this.Controls.Add(this.TagsLabel);
            this.Controls.Add(this.SelectedTags);
            this.Controls.Add(this.Tags);
            this.Controls.Add(this.SearchBar);
            this.Controls.Add(this.rateLabel);
            this.Controls.Add(this.Rate);
            this.Controls.Add(this.SearchButton);
            this.Controls.Add(this.Panel);
            this.Name = "DetailedSearchForm";
            this.Text = "DetailedSearchForm";
            this.Load += new System.EventHandler(this.DetailedSearchForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel Panel;
        private System.Windows.Forms.Button SearchButton;
        private System.Windows.Forms.Label rateLabel;
        private System.Windows.Forms.ComboBox Rate;
        private System.Windows.Forms.TextBox SearchBar;
        private System.Windows.Forms.Label TagsLabel;
        private System.Windows.Forms.ListBox SelectedTags;
        private System.Windows.Forms.ListBox Tags;
    }
}