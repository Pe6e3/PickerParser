namespace PickerParser
{
    partial class Parser
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
            this.button1 = new System.Windows.Forms.Button();
            this.output = new System.Windows.Forms.Label();
            this.allGamesField = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // output
            // 
            this.output.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.output.Location = new System.Drawing.Point(413, 37);
            this.output.Name = "output";
            this.output.Size = new System.Drawing.Size(452, 625);
            this.output.TabIndex = 0;
            // 
            // allGamesField
            // 
            this.allGamesField.Location = new System.Drawing.Point(113, 35);
            this.allGamesField.Multiline = true;
            this.allGamesField.Name = "allGamesField";
            this.allGamesField.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.allGamesField.Size = new System.Drawing.Size(264, 616);
            this.allGamesField.TabIndex = 2;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(1074, 85);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(887, 132);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(452, 528);
            this.label1.TabIndex = 0;
            // 
            // Parser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1539, 674);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.allGamesField);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.output);
            this.Name = "Parser";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label output;
        private System.Windows.Forms.TextBox allGamesField;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
    }
}

