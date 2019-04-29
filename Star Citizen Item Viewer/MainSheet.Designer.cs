namespace Star_Citizen_Item_Viewer
{
    partial class MainSheet
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainSheet));
            this.componentSelect = new System.Windows.Forms.TreeView();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.OverpoweredCheckbox = new System.Windows.Forms.CheckBox();
            this.OverclockedCheckbox = new System.Windows.Forms.CheckBox();
            this.DisplayGrid = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.HotCheckbox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // componentSelect
            // 
            this.componentSelect.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.componentSelect.CheckBoxes = true;
            this.componentSelect.ForeColor = System.Drawing.Color.White;
            this.componentSelect.Location = new System.Drawing.Point(12, 41);
            this.componentSelect.Name = "componentSelect";
            this.componentSelect.Size = new System.Drawing.Size(321, 351);
            this.componentSelect.TabIndex = 0;
            this.componentSelect.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.weaponsSelect_AfterCheck);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(229, 484);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(104, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Download Data";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(118, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 29);
            this.label1.TabIndex = 3;
            this.label1.Text = "Weapons";
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(757, 510);
            this.dataGridView1.TabIndex = 4;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(0, 0);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(1104, 25);
            this.richTextBox1.TabIndex = 5;
            this.richTextBox1.Text = "";
            // 
            // listBox1
            // 
            this.listBox1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox1.ForeColor = System.Drawing.Color.White;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 16;
            this.listBox1.Items.AddRange(new object[] {
            "Weapons",
            "Guns",
            "Armor"});
            this.listBox1.Location = new System.Drawing.Point(229, 398);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(104, 84);
            this.listBox1.TabIndex = 7;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.Transparent;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.HotCheckbox);
            this.splitContainer1.Panel1.Controls.Add(this.OverpoweredCheckbox);
            this.splitContainer1.Panel1.Controls.Add(this.OverclockedCheckbox);
            this.splitContainer1.Panel1.Controls.Add(this.DisplayGrid);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.componentSelect);
            this.splitContainer1.Panel1.Controls.Add(this.listBox1);
            this.splitContainer1.Panel1.Controls.Add(this.button1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.progressBar1);
            this.splitContainer1.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer1.Size = new System.Drawing.Size(1104, 510);
            this.splitContainer1.SplitterDistance = 343;
            this.splitContainer1.TabIndex = 8;
            // 
            // OverpoweredCheckbox
            // 
            this.OverpoweredCheckbox.AutoSize = true;
            this.OverpoweredCheckbox.Location = new System.Drawing.Point(93, 490);
            this.OverpoweredCheckbox.Name = "OverpoweredCheckbox";
            this.OverpoweredCheckbox.Size = new System.Drawing.Size(78, 17);
            this.OverpoweredCheckbox.TabIndex = 10;
            this.OverpoweredCheckbox.Text = "Overpower";
            this.OverpoweredCheckbox.UseVisualStyleBackColor = true;
            this.OverpoweredCheckbox.CheckedChanged += new System.EventHandler(this.OverpoweredCheckbox_CheckedChanged);
            // 
            // OverclockedCheckbox
            // 
            this.OverclockedCheckbox.AutoSize = true;
            this.OverclockedCheckbox.Location = new System.Drawing.Point(12, 490);
            this.OverclockedCheckbox.Name = "OverclockedCheckbox";
            this.OverclockedCheckbox.Size = new System.Drawing.Size(75, 17);
            this.OverclockedCheckbox.TabIndex = 9;
            this.OverclockedCheckbox.Text = "Overclock";
            this.OverclockedCheckbox.UseVisualStyleBackColor = true;
            this.OverclockedCheckbox.CheckedChanged += new System.EventHandler(this.OverclockedCheckbox_CheckedChanged);
            // 
            // DisplayGrid
            // 
            this.DisplayGrid.Location = new System.Drawing.Point(12, 398);
            this.DisplayGrid.Name = "DisplayGrid";
            this.DisplayGrid.Size = new System.Drawing.Size(211, 89);
            this.DisplayGrid.TabIndex = 8;
            this.DisplayGrid.Text = "Display Grid";
            this.DisplayGrid.UseVisualStyleBackColor = true;
            this.DisplayGrid.Click += new System.EventHandler(this.DisplayGrid_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar1.Location = new System.Drawing.Point(0, 0);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(757, 510);
            this.progressBar1.TabIndex = 8;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BackColor = System.Drawing.Color.Transparent;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.richTextBox1);
            this.splitContainer2.Size = new System.Drawing.Size(1104, 539);
            this.splitContainer2.SplitterDistance = 510;
            this.splitContainer2.TabIndex = 9;
            // 
            // HotCheckbox
            // 
            this.HotCheckbox.AutoSize = true;
            this.HotCheckbox.Location = new System.Drawing.Point(177, 490);
            this.HotCheckbox.Name = "HotCheckbox";
            this.HotCheckbox.Size = new System.Drawing.Size(43, 17);
            this.HotCheckbox.TabIndex = 11;
            this.HotCheckbox.Text = "Hot";
            this.HotCheckbox.UseVisualStyleBackColor = true;
            this.HotCheckbox.CheckedChanged += new System.EventHandler(this.HotCheckbox_CheckedChanged);
            // 
            // MainSheet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1104, 539);
            this.Controls.Add(this.splitContainer2);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainSheet";
            this.Text = "Angels of the Warp Component Viewer";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView componentSelect;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button DisplayGrid;
        private System.Windows.Forms.CheckBox OverpoweredCheckbox;
        private System.Windows.Forms.CheckBox OverclockedCheckbox;
        private System.Windows.Forms.CheckBox HotCheckbox;
    }
}

