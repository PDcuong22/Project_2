namespace QuanLyCuaHang
{
    partial class frmInHoaDon
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
            this.rpvInHoaDon = new Microsoft.Reporting.WinForms.ReportViewer();
            this.SuspendLayout();
            // 
            // rpvInHoaDon
            // 
            this.rpvInHoaDon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rpvInHoaDon.LocalReport.ReportEmbeddedResource = "QuanLyCuaHang.rptInHoaDon.rdlc";
            this.rpvInHoaDon.Location = new System.Drawing.Point(0, 0);
            this.rpvInHoaDon.Name = "rpvInHoaDon";
            this.rpvInHoaDon.ServerReport.BearerToken = null;
            this.rpvInHoaDon.Size = new System.Drawing.Size(800, 450);
            this.rpvInHoaDon.TabIndex = 0;
            // 
            // frmInHoaDon
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.rpvInHoaDon);
            this.Name = "frmInHoaDon";
            this.Text = "Hóa Đơn In";
            this.Load += new System.EventHandler(this.frmInHoaDon_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer rpvInHoaDon;
    }
}