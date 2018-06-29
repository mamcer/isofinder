namespace IsoFinder.IsoScanner.Views
{
    partial class Analize
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.chkListIso = new System.Windows.Forms.CheckedListBox();
            this.lblIsoFiles = new System.Windows.Forms.Label();
            this.lblError = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // chkListIso
            // 
            this.chkListIso.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkListIso.FormattingEnabled = true;
            this.chkListIso.Location = new System.Drawing.Point(17, 13);
            this.chkListIso.Name = "chkListIso";
            this.chkListIso.Size = new System.Drawing.Size(768, 565);
            this.chkListIso.TabIndex = 0;
            // 
            // lblIsoFiles
            // 
            this.lblIsoFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblIsoFiles.AutoSize = true;
            this.lblIsoFiles.Location = new System.Drawing.Point(680, 581);
            this.lblIsoFiles.Name = "lblIsoFiles";
            this.lblIsoFiles.Size = new System.Drawing.Size(107, 13);
            this.lblIsoFiles.TabIndex = 1;
            this.lblIsoFiles.Text = "XXXX ISO files found";
            this.lblIsoFiles.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblIsoFiles.Visible = false;
            // 
            // lblError
            // 
            this.lblError.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblError.AutoSize = true;
            this.lblError.ForeColor = System.Drawing.Color.DarkRed;
            this.lblError.Location = new System.Drawing.Point(14, 581);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(74, 13);
            this.lblError.TabIndex = 4;
            this.lblError.Text = "Error message";
            this.lblError.Visible = false;
            // 
            // Analize
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.lblIsoFiles);
            this.Controls.Add(this.chkListIso);
            this.Name = "Analize";
            this.Size = new System.Drawing.Size(800, 600);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox chkListIso;
        private System.Windows.Forms.Label lblIsoFiles;
        private System.Windows.Forms.Label lblError;
    }
}
