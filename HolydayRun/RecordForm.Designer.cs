namespace HolydayRun
{
    partial class RecordForm
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
            this.btnBackForm = new System.Windows.Forms.Button();
            this.btnFixEye = new System.Windows.Forms.Button();
            this.btnRec = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnBackForm
            // 
            this.btnBackForm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBackForm.Location = new System.Drawing.Point(161, 192);
            this.btnBackForm.Name = "btnBackForm";
            this.btnBackForm.Size = new System.Drawing.Size(47, 23);
            this.btnBackForm.TabIndex = 0;
            this.btnBackForm.Text = "Back";
            this.btnBackForm.UseVisualStyleBackColor = true;
            this.btnBackForm.Click += new System.EventHandler(this.btnBackForm_Click);
            // 
            // btnFixEye
            // 
            this.btnFixEye.Location = new System.Drawing.Point(12, 12);
            this.btnFixEye.Name = "btnFixEye";
            this.btnFixEye.Size = new System.Drawing.Size(75, 23);
            this.btnFixEye.TabIndex = 1;
            this.btnFixEye.Text = "Fix Eye";
            this.btnFixEye.UseVisualStyleBackColor = true;
            this.btnFixEye.Click += new System.EventHandler(this.btnFixEye_Click);
            // 
            // btnRec
            // 
            this.btnRec.Location = new System.Drawing.Point(12, 41);
            this.btnRec.Name = "btnRec";
            this.btnRec.Size = new System.Drawing.Size(75, 23);
            this.btnRec.TabIndex = 2;
            this.btnRec.Text = "Record";
            this.btnRec.UseVisualStyleBackColor = true;
            this.btnRec.Click += new System.EventHandler(this.btnRec_Click);
            // 
            // RecordForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(210, 218);
            this.Controls.Add(this.btnRec);
            this.Controls.Add(this.btnFixEye);
            this.Controls.Add(this.btnBackForm);
            this.Name = "RecordForm";
            this.Text = "RecordForm";
            this.Load += new System.EventHandler(this.RecordForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnBackForm;
        private System.Windows.Forms.Button btnFixEye;
        private System.Windows.Forms.Button btnRec;
    }
}