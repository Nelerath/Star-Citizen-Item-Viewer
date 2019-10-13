namespace Star_Citizen_Item_Viewer.Forms
{
    partial class RadarChartOptions
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
            this.compareValues = new System.Windows.Forms.TreeView();
            this.aggregateValues = new System.Windows.Forms.TreeView();
            this.submit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // compareValues
            // 
            this.compareValues.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.compareValues.CheckBoxes = true;
            this.compareValues.ForeColor = System.Drawing.Color.White;
            this.compareValues.Location = new System.Drawing.Point(11, 32);
            this.compareValues.Name = "compareValues";
            this.compareValues.Size = new System.Drawing.Size(369, 135);
            this.compareValues.TabIndex = 0;
            this.compareValues.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.compareValues_AfterCheck);
            // 
            // aggregateValues
            // 
            this.aggregateValues.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.aggregateValues.CheckBoxes = true;
            this.aggregateValues.ForeColor = System.Drawing.Color.White;
            this.aggregateValues.Location = new System.Drawing.Point(11, 193);
            this.aggregateValues.Name = "aggregateValues";
            this.aggregateValues.Size = new System.Drawing.Size(369, 127);
            this.aggregateValues.TabIndex = 1;
            this.aggregateValues.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.aggregateValues_AfterCheck);
            // 
            // submit
            // 
            this.submit.Location = new System.Drawing.Point(305, 326);
            this.submit.Name = "submit";
            this.submit.Size = new System.Drawing.Size(75, 23);
            this.submit.TabIndex = 2;
            this.submit.Text = "Submit";
            this.submit.UseVisualStyleBackColor = true;
            this.submit.Click += new System.EventHandler(this.submit_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(156, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Components to View";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(12, 170);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(187, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Components to Compare";
            // 
            // RadarChartOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(392, 358);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.submit);
            this.Controls.Add(this.aggregateValues);
            this.Controls.Add(this.compareValues);
            this.Name = "RadarChartOptions";
            this.Text = "RadarChartOptions";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView compareValues;
        private System.Windows.Forms.TreeView aggregateValues;
        private System.Windows.Forms.Button submit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}