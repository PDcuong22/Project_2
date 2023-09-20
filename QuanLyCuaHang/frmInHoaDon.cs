using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Microsoft.Reporting.WinForms;

namespace QuanLyCuaHang
{
    public partial class frmInHoaDon : Form
    {
        public frmInHoaDon()
        {
            InitializeComponent();
        }

        public string maHD;
        private void frmInHoaDon_Load(object sender, EventArgs e)
        {
            // MessageBox.Show("In hoa don : " + maHD);
            SqlConnection Con = new SqlConnection();   //Khởi tạo đối tượng
            Con.ConnectionString = @"Data Source=DESKTOP-UAHPQGL;Initial Catalog=QL_cua_hang;Integrated Security=True";
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "hoaDon";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = Con;
            cmd.Parameters.Add(new SqlParameter("@MaHDBan", maHD));
            // Khai bao dataset de chua du lieu
            DataSet ds = new DataSet();
            SqlDataAdapter dap = new SqlDataAdapter(cmd);
            dap.Fill(ds);
            // Thiet lap bao cao
            rpvInHoaDon.ProcessingMode = ProcessingMode.Local;
            rpvInHoaDon.LocalReport.ReportPath = "rptInHoaDon.rdlc";
            if (ds.Tables[0].Rows.Count > 0)
            {
                ReportDataSource rds = new ReportDataSource();
                rds.Name = "dsHoaDon";
                rds.Value = ds.Tables[0];
                //Gắn lên mẫu báo cáo
                rpvInHoaDon.LocalReport.DataSources.Clear();
                rpvInHoaDon.LocalReport.DataSources.Add(rds);
                rpvInHoaDon.RefreshReport();
            }
        }
    }
}
