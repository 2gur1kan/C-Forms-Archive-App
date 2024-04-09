
namespace Archive
{
    partial class SaveForm
    {
        /// <summary>
        ///Gerekli tasarımcı değişkeni.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///Kullanılan tüm kaynakları temizleyin.
        /// </summary>
        ///<param name="disposing">yönetilen kaynaklar dispose edilmeliyse doğru; aksi halde yanlış.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer üretilen kod

        /// <summary>
        /// Tasarımcı desteği için gerekli metot - bu metodun 
        ///içeriğini kod düzenleyici ile değiştirmeyin.
        /// </summary>
        private void InitializeComponent()
        {
            this.Save = new System.Windows.Forms.Button();
            this.Picture = new System.Windows.Forms.PictureBox();
            this.Title = new System.Windows.Forms.TextBox();
            this.Info = new System.Windows.Forms.TextBox();
            this.Link = new System.Windows.Forms.TextBox();
            this.Tags = new System.Windows.Forms.ListBox();
            this.SelectedTags = new System.Windows.Forms.ListBox();
            this.Rate = new System.Windows.Forms.ComboBox();
            this.rateLabel = new System.Windows.Forms.Label();
            this.TagsLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Picture)).BeginInit();
            this.SuspendLayout();
            // 
            // Save
            // 
            this.Save.AutoSize = true;
            this.Save.BackColor = System.Drawing.Color.Goldenrod;
            this.Save.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Save.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Save.Location = new System.Drawing.Point(0, 467);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(888, 45);
            this.Save.TabIndex = 0;
            this.Save.Text = "Save";
            this.Save.UseVisualStyleBackColor = false;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // Picture
            // 
            this.Picture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Picture.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.Picture.Location = new System.Drawing.Point(532, 50);
            this.Picture.Name = "Picture";
            this.Picture.Size = new System.Drawing.Size(344, 249);
            this.Picture.TabIndex = 3;
            this.Picture.TabStop = false;
            this.Picture.Click += new System.EventHandler(this.Picture_Click);
            // 
            // Title
            // 
            this.Title.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Title.Location = new System.Drawing.Point(0, 0);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(888, 44);
            this.Title.TabIndex = 0;
            this.Title.TextChanged += new System.EventHandler(this.Title_TextChanged);
            // 
            // Info
            // 
            this.Info.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Info.Location = new System.Drawing.Point(12, 118);
            this.Info.Multiline = true;
            this.Info.Name = "Info";
            this.Info.Size = new System.Drawing.Size(514, 343);
            this.Info.TabIndex = 4;
            this.Info.TextChanged += new System.EventHandler(this.Info_TextChanged);
            // 
            // Link
            // 
            this.Link.BackColor = System.Drawing.Color.PaleTurquoise;
            this.Link.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Link.Location = new System.Drawing.Point(12, 50);
            this.Link.Name = "Link";
            this.Link.Size = new System.Drawing.Size(514, 31);
            this.Link.TabIndex = 5;
            this.Link.Text = "Link";
            // 
            // Tags
            // 
            this.Tags.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.Tags.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Tags.FormattingEnabled = true;
            this.Tags.ItemHeight = 25;
            this.Tags.Location = new System.Drawing.Point(533, 331);
            this.Tags.Name = "Tags";
            this.Tags.Size = new System.Drawing.Size(172, 129);
            this.Tags.TabIndex = 6;
            this.Tags.SelectedIndexChanged += new System.EventHandler(this.Tags_SelectedIndexChanged);
            // 
            // SelectedTags
            // 
            this.SelectedTags.BackColor = System.Drawing.Color.Yellow;
            this.SelectedTags.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.SelectedTags.FormattingEnabled = true;
            this.SelectedTags.ItemHeight = 25;
            this.SelectedTags.Location = new System.Drawing.Point(711, 332);
            this.SelectedTags.Name = "SelectedTags";
            this.SelectedTags.Size = new System.Drawing.Size(165, 129);
            this.SelectedTags.TabIndex = 7;
            this.SelectedTags.SelectedIndexChanged += new System.EventHandler(this.SelectedTags_SelectedIndexChanged);
            // 
            // Rate
            // 
            this.Rate.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Rate.FormattingEnabled = true;
            this.Rate.Location = new System.Drawing.Point(462, 84);
            this.Rate.Name = "Rate";
            this.Rate.Size = new System.Drawing.Size(63, 33);
            this.Rate.TabIndex = 8;
            // 
            // rateLabel
            // 
            this.rateLabel.AutoSize = true;
            this.rateLabel.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.rateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.rateLabel.Location = new System.Drawing.Point(387, 87);
            this.rateLabel.Name = "rateLabel";
            this.rateLabel.Size = new System.Drawing.Size(69, 25);
            this.rateLabel.TabIndex = 9;
            this.rateLabel.Text = "Rate :";
            // 
            // TagsLabel
            // 
            this.TagsLabel.AutoSize = true;
            this.TagsLabel.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.TagsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.TagsLabel.Location = new System.Drawing.Point(634, 302);
            this.TagsLabel.Name = "TagsLabel";
            this.TagsLabel.Size = new System.Drawing.Size(150, 25);
            this.TagsLabel.TabIndex = 10;
            this.TagsLabel.Text = ": Select Tags :";
            this.TagsLabel.Click += new System.EventHandler(this.TagsLabel_Click);
            // 
            // SaveForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(888, 512);
            this.Controls.Add(this.TagsLabel);
            this.Controls.Add(this.rateLabel);
            this.Controls.Add(this.Rate);
            this.Controls.Add(this.SelectedTags);
            this.Controls.Add(this.Tags);
            this.Controls.Add(this.Link);
            this.Controls.Add(this.Info);
            this.Controls.Add(this.Title);
            this.Controls.Add(this.Picture);
            this.Controls.Add(this.Save);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Name = "SaveForm";
            this.Text = "SavePanel";
            this.Load += new System.EventHandler(this.SaveForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Picture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.PictureBox Picture;
        private System.Windows.Forms.TextBox Title;
        private System.Windows.Forms.TextBox Info;
        private System.Windows.Forms.TextBox Link;
        private System.Windows.Forms.ListBox Tags;
        private System.Windows.Forms.ListBox SelectedTags;
        private System.Windows.Forms.ComboBox Rate;
        private System.Windows.Forms.Label rateLabel;
        private System.Windows.Forms.Label TagsLabel;
    }
}

