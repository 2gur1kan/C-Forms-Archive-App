
namespace Archive
{
    partial class ArchiveMain
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
            this.SearchBar = new System.Windows.Forms.TextBox();
            this.Search = new System.Windows.Forms.Button();
            this.DetailedSearch = new System.Windows.Forms.Button();
            this.New = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Shelf1 = new System.Windows.Forms.Button();
            this.Shelf2 = new System.Windows.Forms.Button();
            this.Shelf3 = new System.Windows.Forms.Button();
            this.Shelf4 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SearchBar
            // 
            this.SearchBar.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.SearchBar.Location = new System.Drawing.Point(22, 50);
            this.SearchBar.Name = "SearchBar";
            this.SearchBar.Size = new System.Drawing.Size(401, 44);
            this.SearchBar.TabIndex = 0;
            // 
            // Search
            // 
            this.Search.BackColor = System.Drawing.Color.AliceBlue;
            this.Search.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Search.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Search.Location = new System.Drawing.Point(429, 50);
            this.Search.Name = "Search";
            this.Search.Size = new System.Drawing.Size(106, 44);
            this.Search.TabIndex = 1;
            this.Search.Text = "Search";
            this.Search.UseVisualStyleBackColor = false;
            this.Search.Click += new System.EventHandler(this.Search_Click);
            // 
            // DetailedSearch
            // 
            this.DetailedSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.DetailedSearch.Location = new System.Drawing.Point(541, 50);
            this.DetailedSearch.Name = "DetailedSearch";
            this.DetailedSearch.Size = new System.Drawing.Size(178, 44);
            this.DetailedSearch.TabIndex = 2;
            this.DetailedSearch.Text = "DetailedSearch";
            this.DetailedSearch.UseVisualStyleBackColor = true;
            this.DetailedSearch.Click += new System.EventHandler(this.DetailedSearch_Click);
            // 
            // New
            // 
            this.New.BackColor = System.Drawing.Color.Gold;
            this.New.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.New.Location = new System.Drawing.Point(725, 50);
            this.New.Name = "New";
            this.New.Size = new System.Drawing.Size(72, 44);
            this.New.TabIndex = 3;
            this.New.Text = "New";
            this.New.UseVisualStyleBackColor = false;
            this.New.Click += new System.EventHandler(this.New_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label1.Location = new System.Drawing.Point(22, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Search Bar";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // Shelf1
            // 
            this.Shelf1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Shelf1.Location = new System.Drawing.Point(25, 174);
            this.Shelf1.Name = "Shelf1";
            this.Shelf1.Size = new System.Drawing.Size(551, 84);
            this.Shelf1.TabIndex = 5;
            this.Shelf1.Text = "Empty";
            this.Shelf1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Shelf1.UseVisualStyleBackColor = true;
            this.Shelf1.Click += new System.EventHandler(this.Shelf1_Click);
            // 
            // Shelf2
            // 
            this.Shelf2.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Shelf2.Location = new System.Drawing.Point(25, 264);
            this.Shelf2.Name = "Shelf2";
            this.Shelf2.Size = new System.Drawing.Size(551, 84);
            this.Shelf2.TabIndex = 6;
            this.Shelf2.Text = "Empty";
            this.Shelf2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Shelf2.UseVisualStyleBackColor = true;
            // 
            // Shelf3
            // 
            this.Shelf3.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Shelf3.Location = new System.Drawing.Point(25, 354);
            this.Shelf3.Name = "Shelf3";
            this.Shelf3.Size = new System.Drawing.Size(551, 84);
            this.Shelf3.TabIndex = 7;
            this.Shelf3.Text = "Empty";
            this.Shelf3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Shelf3.UseVisualStyleBackColor = true;
            // 
            // Shelf4
            // 
            this.Shelf4.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Shelf4.Location = new System.Drawing.Point(25, 444);
            this.Shelf4.Name = "Shelf4";
            this.Shelf4.Size = new System.Drawing.Size(551, 84);
            this.Shelf4.TabIndex = 8;
            this.Shelf4.Text = "Empty";
            this.Shelf4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Shelf4.UseVisualStyleBackColor = true;
            // 
            // ArchiveMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(820, 554);
            this.Controls.Add(this.Shelf4);
            this.Controls.Add(this.Shelf3);
            this.Controls.Add(this.Shelf2);
            this.Controls.Add(this.Shelf1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.New);
            this.Controls.Add(this.DetailedSearch);
            this.Controls.Add(this.Search);
            this.Controls.Add(this.SearchBar);
            this.Name = "ArchiveMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ArchiveMain";
            this.Load += new System.EventHandler(this.ArchiveMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox SearchBar;
        private System.Windows.Forms.Button Search;
        private System.Windows.Forms.Button DetailedSearch;
        private System.Windows.Forms.Button New;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Shelf1;
        private System.Windows.Forms.Button Shelf2;
        private System.Windows.Forms.Button Shelf3;
        private System.Windows.Forms.Button Shelf4;
    }
}