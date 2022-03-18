namespace UniRechner
{
    partial class ActivateForm
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
            this.textBoxKey = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxKey
            // 
            this.textBoxKey.Location = new System.Drawing.Point(12, 12);
            this.textBoxKey.Multiline = true;
            this.textBoxKey.Name = "textBoxKey";
            this.textBoxKey.Size = new System.Drawing.Size(732, 41);
            this.textBoxKey.TabIndex = 0;
            this.textBoxKey.Text = "Paste License Key here";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 59);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(732, 57);
            this.button1.TabIndex = 1;
            this.button1.Text = "Activate";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // ActivateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(756, 124);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBoxKey);
            this.Name = "ActivateForm";
            this.Text = "Activate";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxKey;
        private System.Windows.Forms.Button button1;
    }
}