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
using QuanLyCuaHang.Class;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;

namespace QuanLyCuaHang
{
    public partial class frmHoaDonBan : Form
    {
        DataTable tblCTHDB; //Bảng chi tiết hoá đơn bán

        public frmHoaDonBan()
        {
            InitializeComponent();
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void frmHoaDonBan_Load(object sender, EventArgs e)
        {
            btnThemHD.Enabled = true;
            btnBoQua.Enabled = false;
            btnLuuHD.Enabled = false;
            btnXoa.Enabled = false;
            btnInHD.Enabled = false;
            txtMaHD.ReadOnly = true;
            cboMaNV.Enabled = false;
            cboMaKhachHang.Enabled = false;
            dtpNgayBan.Enabled = false;
            txtMaHang.ReadOnly = true;
            txtTenNV.ReadOnly = true;
            txtTenKhachHang.ReadOnly = true;
            txtDiaChi.ReadOnly = true;
            mtbDienThoai.ReadOnly = true;
            txtTenHang.ReadOnly = true;
         //   txtSoLuong.ReadOnly = true;
        //    txtGiamGia.ReadOnly = true;
            txtDonGia.ReadOnly = true;
            txtThanhTien.ReadOnly = true;
            txtTongTien.ReadOnly = true;
            txtGiamGia.Text = "0";
            txtTongTien.Text = "0";
            Functions.FillCombo("SELECT MaKhach, TenKhach FROM TblKhach", cboMaKhachHang, "MaKhach", "MaKhach");
            cboMaKhachHang.SelectedIndex = -1;
            Functions.FillCombo("SELECT MaNhanVien, TenNhanVien FROM TblNhanVien", cboMaNV, "MaNhanVien", "MaNhanVien");
            cboMaNV.SelectedIndex = -1;
        //    Functions.FillCombo("SELECT MaHang, TenHang FROM TblHang", cboMaHang, "MaHang", "MaHang");
       //     cboMaHang.SelectedIndex = -1;
            //Hiển thị thông tin của một hóa đơn được gọi từ form tìm kiếm
            if (txtMaHD.Text != "")
            {
                LoadInfoHoaDon();
                btnXoa.Enabled = true;
                btnInHD.Enabled = true;
            }
            LoadDataGridView();
        }

        private void LoadDataGridView()
        {
            string sql;
            sql = "SELECT a.MaHang, b.TenHang, a.SoLuong, b.DonGiaBan, a.GiamGia,a.ThanhTien FROM TblChiTietHDBan AS a, TblHang AS b WHERE a.MaHDBan = N'" + txtMaHD.Text + "' AND a.MaHang=b.MaHang";
            tblCTHDB = Functions.GetDataToTable(sql);
            dgvHDBanHang.DataSource = tblCTHDB;
            dgvHDBanHang.Columns[0].HeaderText = "Mã hàng";
            dgvHDBanHang.Columns[1].HeaderText = "Tên hàng";
            dgvHDBanHang.Columns[2].HeaderText = "Số lượng";
            dgvHDBanHang.Columns[3].HeaderText = "Đơn giá";
            dgvHDBanHang.Columns[4].HeaderText = "Giảm giá %";
            dgvHDBanHang.Columns[5].HeaderText = "Thành tiền";
            dgvHDBanHang.Columns[0].Width = 80;
            dgvHDBanHang.Columns[1].Width = 130;
            dgvHDBanHang.Columns[2].Width = 80;
            dgvHDBanHang.Columns[3].Width = 90;
            dgvHDBanHang.Columns[4].Width = 90;
            dgvHDBanHang.Columns[5].Width = 90;
            dgvHDBanHang.AllowUserToAddRows = false;
            dgvHDBanHang.EditMode = DataGridViewEditMode.EditProgrammatically;
        }

        private void LoadInfoHoaDon()
        {
            string str;
            str = "SELECT NgayBan FROM TblHDBan WHERE MaHDBan = N'" + txtMaHD.Text + "'";
            dtpNgayBan.Value = DateTime.Parse(Functions.GetFieldValues(str));
            str = "SELECT MaNhanVien FROM TblHDBan WHERE MaHDBan = N'" + txtMaHD.Text + "'";
            cboMaNV.Text = Functions.GetFieldValues(str);
            str = "SELECT MaKhach FROM TblHDBan WHERE MaHDBan = N'" + txtMaHD.Text + "'";
            cboMaKhachHang.Text = Functions.GetFieldValues(str);
            str = "SELECT TongTien FROM TblHDBan WHERE MaHDBan = N'" + txtMaHD.Text + "'";
            txtTongTien.Text = Functions.GetFieldValues(str);
            lblBangChu.Text = "Bằng chữ: " + Functions.ChuyenSoSangChuoi(double.Parse(txtTongTien.Text));
        }

        private FilterInfoCollection fic2;
        private VideoCaptureDevice vcd2;
        private ComboBox cboFilter2 = new ComboBox();

        private void btnThemHD_Click(object sender, EventArgs e)
        {
            k = 1;
            btnXoa.Enabled = false;
            btnLuuHD.Enabled = true;
            btnInHD.Enabled = false;
            btnThemHD.Enabled = false;
            btnBoQua.Enabled = true;
            cboMaKhachHang.Enabled = true;
            cboMaNV.Enabled = true;
            dtpNgayBan.Enabled = true;
            ResetValues();
            txtMaHD.Text = Functions.CreateKey("HDB");
            LoadDataGridView();
            txtMaHang.Enabled = false;

            fic2 = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in fic2)
            {
                cboFilter2.Items.Add(device.Name);
            }
            cboFilter2.SelectedIndex = 0;

            vcd2 = new VideoCaptureDevice(fic2[cboFilter2.SelectedIndex].MonikerString);
            vcd2.NewFrame += VideoCaptureDevice_NewFrame;
            vcd2.Start();

        }

        private void VideoCaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
                BarcodeReader reader = new BarcodeReader();
                var result = reader.Decode(bitmap);
                if (result != null)
                {
                    txtMaHang.Invoke(new MethodInvoker(delegate ()
                    {
                        string sql;
                        txtMaHang.Text = result.ToString();
                        sql = "SELECT MaHang FROM tblHang WHERE MaHang=N'" + txtMaHang.Text.Trim() + "'";
                        if (!Functions.CheckKey(sql))
                        {
                            MessageBox.Show("Mã hàng này chưa có, bạn phải nhập mã khác", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                       //     ResetValuesHang();
                         //   txtMaHang.Focus();
                         //   return;
                        } 
                        txtSoLuong.Focus();
                    }));
                }
            }
            catch (Exception Err)
            {
                MessageBox.Show(Err.Message, Err.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResetValues()
        {
            txtMaHD.Text = "";
            dtpNgayBan.Value = DateTime.Now;
            cboMaNV.Text = "";
            cboMaKhachHang.Text = "";
            txtTongTien.Text = "0";
            lblBangChu.Text = "Bằng chữ: ";
            txtMaHang.Text = "";
            txtSoLuong.Text = "";
            txtGiamGia.Text = "0";
            txtThanhTien.Text = "0";
        }

        private void btnLuuHD_Click(object sender, EventArgs e)
        {
            string sql;
            double sl, SLcon, tong, Tongmoi;
            sql = "SELECT MaHDBan FROM TblHDBan WHERE MaHDBan=N'" + txtMaHD.Text + "'";
            if (!Functions.CheckKey(sql))
            {
                if (cboMaNV.Text.Length == 0)
                {
                    MessageBox.Show("Bạn phải nhập nhân viên", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    cboMaNV.Focus();
                    return;
                }
                if (cboMaKhachHang.Text.Length == 0)
                 {
                     MessageBox.Show("Bạn phải nhập khách hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                     cboMaKhachHang.Focus();
                     return;
                 }
                sql = "INSERT INTO TblHDBan(MaHDBan, NgayBan, MaNhanVien, MaKhach, TongTien) VALUES (N'" + txtMaHD.Text.Trim() + "','" +
                        DateTime.Now + "',N'" + cboMaNV.SelectedValue + "',N'" +
                        cboMaKhachHang.SelectedValue + "'," + txtTongTien.Text + ")";
                Functions.RunSQL(sql);
            }
            // Lưu thông tin của các mặt hàng
            if (txtMaHang.Text.Trim().Length == 0)
            {
                MessageBox.Show("Bạn phải nhập mã hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtMaHang.Focus();
                return;
            }
            if ((txtSoLuong.Text.Trim().Length == 0) || (txtSoLuong.Text == "0"))
            {
                MessageBox.Show("Bạn phải nhập số lượng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtSoLuong.Text = "";
                txtSoLuong.Focus();
                return;
            }
            if (txtGiamGia.Text.Trim().Length == 0)
            {
                MessageBox.Show("Bạn phải nhập giảm giá", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtGiamGia.Focus();
                return;
            }
            sql = "SELECT MaHang FROM TblChiTietHDBan WHERE MaHang=N'" + txtMaHang.Text.Trim() + "' AND MaHDBan = N'" + txtMaHD.Text.Trim() + "'";
            if (Functions.CheckKey(sql))
            {
                MessageBox.Show("Mã hàng này đã có, bạn phải nhập mã khác", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ResetValuesHang();
                txtMaHang.Focus();
                return;
            }
            // Kiểm tra xem số lượng hàng trong kho còn đủ để cung cấp không?
            sl = Convert.ToDouble(Functions.GetFieldValues("SELECT SoLuong FROM TblHang WHERE MaHang = N'" + txtMaHang.Text + "'"));
            if (Convert.ToDouble(txtSoLuong.Text) > sl)
            {
                MessageBox.Show("Số lượng mặt hàng này chỉ còn " + sl, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtSoLuong.Text = "";
                txtSoLuong.Focus();
                return;
            }
            sql = "INSERT INTO TblChiTietHDBan(MaHDBan,MaHang,SoLuong,DonGia, GiamGia,ThanhTien) VALUES(N'" + txtMaHD.Text.Trim() + "',N'" + txtMaHang.Text.Trim() + "'," + txtSoLuong.Text + "," + txtDonGia.Text + "," + txtGiamGia.Text + "," + txtThanhTien.Text + ")";
            Functions.RunSQL(sql);
            LoadDataGridView();
            // Cập nhật lại số lượng của mặt hàng vào bảng tblHang
            SLcon = sl - Convert.ToDouble(txtSoLuong.Text);
            sql = "UPDATE TblHang SET SoLuong =" + SLcon + " WHERE MaHang= N'" + txtMaHang.Text.Trim() + "'";
            Functions.RunSQL(sql);
            // Cập nhật lại tổng tiền cho hóa đơn bán
            tong = Convert.ToDouble(Functions.GetFieldValues("SELECT TongTien FROM TblHDBan WHERE MaHDBan = N'" + txtMaHD.Text + "'"));
            Tongmoi = tong + Convert.ToDouble(txtThanhTien.Text);
            sql = "UPDATE TblHDBan SET TongTien =" + Tongmoi + " WHERE MaHDBan = N'" + txtMaHD.Text + "'";
            Functions.RunSQL(sql);
            txtTongTien.Text = Tongmoi.ToString();
            lblBangChu.Text = "Bằng chữ: " + Functions.ChuyenSoSangChuoi(double.Parse(Tongmoi.ToString()));
            ResetValuesHang();
            btnXoa.Enabled = true;
            btnThemHD.Enabled = true;
            btnInHD.Enabled = true;
            btnBoQua.Enabled = true;
        }

        private void ResetValuesHang()
        {
            txtMaHang.Text = "";
            txtSoLuong.Text = "";
            txtGiamGia.Text = "0";
            txtThanhTien.Text = "0";
        }


        private void cboMaKhachHang_SelectedIndexChanged(object sender, EventArgs e)
        {
            string str;
            if (cboMaKhachHang.Text == "")
            {
                txtTenKhachHang.Text = "";
                txtDiaChi.Text = "";
                mtbDienThoai.Text = "";
            }
            //Khi chọn Mã khách hàng thì các thông tin của khách hàng sẽ hiện ra
            str = "Select TenKhach from tblKhach where MaKhach = N'" + cboMaKhachHang.SelectedValue + "'";
            txtTenKhachHang.Text = Functions.GetFieldValues(str);
            str = "Select DiaChi from tblKhach where MaKhach = N'" + cboMaKhachHang.SelectedValue + "'";
            txtDiaChi.Text = Functions.GetFieldValues(str);
            str = "Select DienThoai from tblKhach where MaKhach= N'" + cboMaKhachHang.SelectedValue + "'";
            mtbDienThoai.Text = Functions.GetFieldValues(str);
        }

        private void txtSoLuong_TextChanged(object sender, EventArgs e)
        {
            //Khi thay đổi số lượng thì thực hiện tính lại thành tiền
            double tt, sl = 0, dg = 0, gg = 0;
            if (txtSoLuong.Text == "")
                sl = 0;
            else
            {
                try
                {
                    sl = Convert.ToDouble(txtSoLuong.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bạn nhập sai dữ liệu Số lượng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            
            if (txtGiamGia.Text == "")
                gg = 0;
            else
            {
                try
                {
                    gg = Convert.ToDouble(txtGiamGia.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bạn nhập sai dữ liệu Giảm giá", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            
            if (txtDonGia.Text == "")
                dg = 0;
            else
                dg = Convert.ToDouble(txtDonGia.Text);
            tt = sl * dg - sl * dg * gg / 100;
            txtThanhTien.Text = tt.ToString();
        }

        private void txtGiamGia_TextChanged(object sender, EventArgs e)
        {
            //Khi thay đổi giảm giá thì tính lại thành tiền
            double tt, sl=0, dg = 0, gg =0;
            if (txtSoLuong.Text == "")
                sl = 0;
            else
            {
                try
                {
                    sl = Convert.ToDouble(txtSoLuong.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bạn nhập sai dữ liệu Số lượng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            if (txtGiamGia.Text == "")
                gg = 0;
            else
            {
                try
                {
                    gg = Convert.ToDouble(txtGiamGia.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bạn nhập sai dữ liệu Giảm giá", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            
            if (txtDonGia.Text == "")
                dg = 0;
            else
            {
                try
                {
                    dg = Convert.ToDouble(txtDonGia.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bạn nhập sai dữ liệu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            tt = sl * dg - sl * dg * gg / 100;
            txtThanhTien.Text = tt.ToString();
        }

        private void cboMaHD_DropDown(object sender, EventArgs e)
        {
            Functions.FillCombo("SELECT MaHDBan FROM tblHDBan", cboMaHD, "MaHDBan", "MaHDBan");
            cboMaHD.SelectedIndex = -1;
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            if (cboMaHD.Text == "")
            {
                MessageBox.Show("Bạn phải chọn một mã hóa đơn để tìm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cboMaHD.Focus();
                return;
            }
            txtMaHD.Text = cboMaHD.Text;
            LoadInfoHoaDon();
            LoadDataGridView();
            btnXoa.Enabled = true;
            btnLuuHD.Enabled = true;
            btnInHD.Enabled = true;
            btnBoQua.Enabled = true;
            cboMaHD.SelectedIndex = -1;
        }

        private void dtpNgayBan_ValueChanged(object sender, EventArgs e)
        {

        }

        private void cboMaNV_SelectedIndexChanged(object sender, EventArgs e)
        {
            string str;
            if (cboMaNV.Text == "")
                txtTenNV.Text = "";
            // Khi chọn Mã nhân viên thì tên nhân viên tự động hiện ra
            str = "Select TenNhanVien from TblNhanVien where MaNhanVien =N'" + cboMaNV.SelectedValue + "'";
            txtTenNV.Text = Functions.GetFieldValues(str);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            double sl, slcon, slxoa;
            if (MessageBox.Show("Bạn có chắc chắn muốn xóa không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string sql = "SELECT MaHang,SoLuong FROM TblChiTietHDBan WHERE MaHDBan = N'" + txtMaHD.Text + "'";
                DataTable tblHang = Functions.GetDataToTable(sql);
                for (int hang = 0; hang <= tblHang.Rows.Count - 1; hang++)
                {
                    // Cập nhật lại số lượng cho các mặt hàng
                    sl = Convert.ToDouble(Functions.GetFieldValues("SELECT SoLuong FROM TblHang WHERE MaHang = N'" + tblHang.Rows[hang][0].ToString() + "'"));
                    slxoa = Convert.ToDouble(tblHang.Rows[hang][1].ToString());
                    slcon = sl + slxoa;
                    sql = "UPDATE TblHang SET SoLuong =" + slcon + " WHERE MaHang= N'" + tblHang.Rows[hang][0].ToString() + "'";
                    Functions.RunSQL(sql);
                }

                //Xóa chi tiết hóa đơn
                sql = "DELETE TblChiTietHDBan WHERE MaHDBan=N'" + txtMaHD.Text + "'";
                Functions.RunSqlDel(sql);

                //Xóa hóa đơn
                sql = "DELETE TblHDBan WHERE MaHDBan=N'" + txtMaHD.Text + "'";
                Functions.RunSqlDel(sql);
                ResetValues();
                LoadDataGridView();
                btnXoa.Enabled = false;
                btnInHD.Enabled = false;
            }
        }

        private void dgvHDBanHang_DoubleClick(object sender, EventArgs e)
        {
            string MaHangxoa, sql;
            Double ThanhTienxoa, SoLuongxoa, sl, slcon, tong, tongmoi;
            if (tblCTHDB.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if ((MessageBox.Show("Bạn có chắc chắn muốn xóa không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))
            {
                //Xóa hàng và cập nhật lại số lượng hàng 
                MaHangxoa = dgvHDBanHang.CurrentRow.Cells["MaHang"].Value.ToString();
                SoLuongxoa = Convert.ToDouble(dgvHDBanHang.CurrentRow.Cells["SoLuong"].Value.ToString());
                ThanhTienxoa = Convert.ToDouble(dgvHDBanHang.CurrentRow.Cells["ThanhTien"].Value.ToString());
                sql = "DELETE TblChiTietHDBan WHERE MaHDBan=N'" + txtMaHD.Text + "' AND MaHang = N'" + MaHangxoa + "'";
                Functions.RunSQL(sql);
                // Cập nhật lại số lượng cho các mặt hàng
                sl = Convert.ToDouble(Functions.GetFieldValues("SELECT SoLuong FROM tblHang WHERE MaHang = N'" + MaHangxoa + "'"));
                slcon = sl + SoLuongxoa;
                sql = "UPDATE TblHang SET SoLuong =" + slcon + " WHERE MaHang= N'" + MaHangxoa + "'";
                Functions.RunSQL(sql);
                // Cập nhật lại tổng tiền cho hóa đơn bán
                tong = Convert.ToDouble(Functions.GetFieldValues("SELECT TongTien FROM tblHDBan WHERE MaHDBan = N'" + txtMaHD.Text + "'"));
                tongmoi = tong - ThanhTienxoa;
                sql = "UPDATE TblHDBan SET TongTien =" + tongmoi + " WHERE MaHDBan = N'" + txtMaHD.Text + "'";
                Functions.RunSQL(sql);
                txtTongTien.Text = tongmoi.ToString();
                lblBangChu.Text = "Bằng chữ: " + Functions.ChuyenSoSangChuoi(double.Parse(tongmoi.ToString()));
                LoadDataGridView();
            }
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            if (k==1)
                vcd2.Stop();
            this.Close();
        }

        private void btnInHD_Click(object sender, EventArgs e)
        {
            frmInHoaDon frm = new frmInHoaDon();
            frm.maHD = txtMaHD.Text.Trim();
            frm.Show();
        }

        private void txtMaHang_TextChanged(object sender, EventArgs e)
        {
            string str;
            if (txtMaHang.Text == "")
            {
                txtTenHang.Text = "";
                txtDonGia.Text = "";
            }
            // Khi chọn mã hàng thì các thông tin về hàng hiện ra
            str = "SELECT TenHang FROM TblHang WHERE MaHang =N'" + txtMaHang.Text.Trim() + "'";
            txtTenHang.Text = Functions.GetFieldValues(str);
            str = "SELECT DonGiaBan FROM TblHang WHERE MaHang =N'" + txtMaHang.Text.Trim() + "'";
            txtDonGia.Text = Functions.GetFieldValues(str);
        }

        int k = 0;
        private void frmHoaDonBan_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (k==1)
                vcd2.Stop();
        }

        private void btnBoQua_Click(object sender, EventArgs e)
        {
            ResetValues();
            btnThemHD.Enabled = true;
            btnLuuHD.Enabled = false;
            btnBoQua.Enabled = false;
            btnXoa.Enabled = false;
            btnInHD.Enabled = false;
            txtMaHD.ReadOnly = true;
            cboMaNV.Enabled = false;
            cboMaKhachHang.Enabled = false;
            dtpNgayBan.Enabled = false;
            txtTenNV.ReadOnly = true;
            txtTenKhachHang.ReadOnly = true;
            txtDiaChi.ReadOnly = true;
            mtbDienThoai.ReadOnly = true;
            txtTenHang.ReadOnly = true;
            txtDonGia.ReadOnly = true;
            txtThanhTien.ReadOnly = true;
            txtTongTien.ReadOnly = true;
            txtGiamGia.Text = "0";
            txtTongTien.Text = "0";
        }
    }
}
