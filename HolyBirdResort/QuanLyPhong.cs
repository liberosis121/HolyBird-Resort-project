using HolyBirdResort.DAO;
using HolyBirdResort.DTO;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HolyBirdResort
{
    public partial class QuanLyPhong : Form
    {
        public QuanLyPhong()
        {
            InitializeComponent();
            this.Load += DanhSachPhong_Load;
        }

        private void ResetGiaoDien()
        {
            // Ẩn tất cả các Panel/Form con
            FormThemPhong.Visible = false;
            FormSuaPhong.Visible = false;
            FormHangPhong.Visible = false;
            FormHinhThucPhong.Visible = false;
        }

        private void guna2ControlBox1_Click(object sender, EventArgs e) => Application.Exit();

        private void DanhSachPhong_Load(object sender, EventArgs e)
        {
            LoadDGV();
            LoadComboBoxData();
        }

        private void QuayLai_Click(object sender, EventArgs e) => this.Close();

        #region QUẢN LÝ PHÒNG (ROOMS)
        private void LoadDGV()
        {
            dgvPhong.Rows.Clear();
            var dsPhong = PhongDAO.GetAllPhong();
            foreach (var p in dsPhong)
            {
                dgvPhong.Rows.Add(p.MaTang, p.MaPhong, p.LoaiHang, p.TenHinhThuc, p.GiaPhong, p.TrangThai, "+");
            }
        }

        private void LoadComboBoxData()
        {
            DataTable dtHang = PhongDAO.GetHangPhong();
            DataTable dtHT = PhongDAO.GetHinhThuc();

            // Đổ dữ liệu vào combobox quản lý phòng
            cboThemPhong_HangPhong.DataSource = dtHang;
            cboThemPhong_HangPhong.DisplayMember = "LoaiHang";
            cboThemPhong_HangPhong.ValueMember = "LoaiHang";

            cboThemPhong_HinhThuc.DataSource = dtHT;
            cboThemPhong_HinhThuc.DisplayMember = "TenHinhThuc";
            cboThemPhong_HinhThuc.ValueMember = "TenHinhThuc";

            cboCaNhatPhong_SuaHangPhong.DataSource = dtHang.Copy();
            cboCaNhatPhong_SuaHangPhong.DisplayMember = "LoaiHang";
            cboCaNhatPhong_SuaHangPhong.ValueMember = "LoaiHang";

            cboCapNhatPhong_SuaHinhThuc.DataSource = dtHT.Copy();
            cboCapNhatPhong_SuaHinhThuc.DisplayMember = "TenHinhThuc";
            cboCapNhatPhong_SuaHinhThuc.ValueMember = "TenHinhThuc";
        }

        private void dgvPhong_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string colName = dgvPhong.Columns[e.ColumnIndex].Name;

            // Xử lý nút SỬA
            if (colName == "Sua" || dgvPhong.Columns[e.ColumnIndex].HeaderText == "+")
            {
                ResetGiaoDien();

                // 1. Đổ dữ liệu cũ vào
                txtSuaSoPhong.Text = dgvPhong.Rows[e.RowIndex].Cells["MaPhong"].Value.ToString();
                txtSuaSoTang.Text = dgvPhong.Rows[e.RowIndex].Cells["MaTang"].Value.ToString();
                cboCaNhatPhong_SuaHangPhong.Text = dgvPhong.Rows[e.RowIndex].Cells["HangPhong"].Value.ToString();
                cboCapNhatPhong_SuaHinhThuc.Text = dgvPhong.Rows[e.RowIndex].Cells["HinhThucPhong"].Value.ToString();

                // 2. KHÓA KHÔNG CHO SỬA MÃ PHÒNG VÀ TẦNG (Thêm đoạn này)
                txtSuaSoPhong.Enabled = false;
                txtSuaSoTang.Enabled = false;

                FormSuaPhong.Visible = true;
                FormSuaPhong.BringToFront();
            }
        }

        private void CapNhatPhong_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSuaSoPhong.Text)) return;

            Phong p = new Phong
            {
                MaPhong = txtSuaSoPhong.Text.Trim(),
                MaTang = txtSuaSoTang.Text.Trim(),
                LoaiHang = cboCaNhatPhong_SuaHangPhong.Text,
                TenHinhThuc = cboCapNhatPhong_SuaHinhThuc.Text
            };

            this.Cursor = Cursors.WaitCursor;
            string res = PhongDAO.UpdatePhong(p); // Gọi hàm Update có xử lý tranh chấp
            this.Cursor = Cursors.Default;

            if (res == "SUCCESS") { MessageBox.Show("Cập nhật thành công!"); LoadDGV(); FormSuaPhong.Visible = false; }
            else MessageBox.Show("Lỗi: " + res);
        }

        private void ThemPhong_Them_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtThemSoPhong.Text)) { MessageBox.Show("Nhập số phòng!"); return; }

            Phong p = new Phong
            {
                MaPhong = txtThemSoPhong.Text.Trim(),
                MaTang = txtThemSoTang.Text.Trim(),
                LoaiHang = cboThemPhong_HangPhong.Text,
                TenHinhThuc = cboThemPhong_HinhThuc.Text
            };

            if (PhongDAO.InsertPhong(p)) { MessageBox.Show("Thêm thành công!"); LoadDGV(); FormThemPhong.Visible = false; }
            else MessageBox.Show("Lỗi: Mã phòng trùng hoặc tầng không hợp lệ.");
        }

        private void TimKiemPhong_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtTimKiemPhong.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(keyword)) { dgvPhong.ClearSelection(); return; }
            foreach (DataGridViewRow row in dgvPhong.Rows)
            {
                if (row.Cells["MaPhong"].Value.ToString().ToLower().Contains(keyword)) // Sửa SoPhong thành MaPhong cho khớp DB
                {
                    dgvPhong.ClearSelection();
                    row.Selected = true;
                    dgvPhong.FirstDisplayedScrollingRowIndex = row.Index;
                    return; // Tìm thấy thì dừng
                }
            }
        }

        // Các nút bật tắt Form con
        private void ThemPhong_Click(object sender, EventArgs e) {
            ResetGiaoDien(); // Ẩn các cái cũ đi
            FormThemPhong.Visible = true;
            FormThemPhong.BringToFront();
        }
        private void ThoatThemPhong_Click(object sender, EventArgs e) => FormThemPhong.Visible = false;
        private void ThoatCapNhatPhong_Click(object sender, EventArgs e) => FormSuaPhong.Visible = false;
        // Các sự kiện TextChanged rỗng
        private void ThemSoPhong_TextChanged(object sender, EventArgs e) { }
        private void ThemSoTang_TextChanged(object sender, EventArgs e) { }
        private void ThemPhong_HangPhong_SelectedIndexChanged(object sender, EventArgs e) { }
        private void ThemPhong_HinhThuc_SelectedIndexChanged(object sender, EventArgs e) { }
        private void SuaSoPhong_TextChanged(object sender, EventArgs e) { }
        private void SuaSoTang_TextChanged(object sender, EventArgs e) { }
        private void cboCaNhatPhong_SuaHangPhong_SelectedIndexChanged(object sender, EventArgs e) { }
        private void cboCapNhatPhong_SuaHinhThuc_SelectedIndexChanged(object sender, EventArgs e) { }
        private void Luu_Click(object sender, EventArgs e) { } // Nút này có vẻ thừa hoặc dùng chung, tôi để trống
        #endregion

        #region QUẢN LÝ HẠNG PHÒNG (CATEGORY)

        private void HangPhong_Click(object sender, EventArgs e)
        {
            FormHangPhong.Visible = true;
            FormHangPhong.BringToFront();
            LoadCboHangPhongCon();
        }

        private void LoadCboHangPhongCon()
        {
            // Load lại danh sách hạng phòng vào combobox trong form Hạng Phòng
            DataTable dt = PhongDAO.GetHangPhong();
            cboHangPhong_SuaHangPhong.DataSource = dt;
            cboHangPhong_SuaHangPhong.DisplayMember = "LoaiHang";
            cboHangPhong_SuaHangPhong.ValueMember = "LoaiHang";
        }

        private void HangPhong_SuaHangPhong_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Khi chọn hạng phòng, tự động hiện đơn giá (Hệ số giá)
            if (cboHangPhong_SuaHangPhong.SelectedValue != null)
            {
                string tenHang = cboHangPhong_SuaHangPhong.SelectedValue.ToString();
                decimal heSo = PhongDAO.GetHeSoHangPhong(tenHang);
                txtHangPhong_SuaDonGia.Text = heSo.ToString();
            }
        }

        private void LuuHangPhong_Click(object sender, EventArgs e) // Nút Sửa Hạng Phòng
        {
            if (string.IsNullOrEmpty(txtHangPhong_SuaDonGia.Text)) return;

            string tenHang = cboHangPhong_SuaHangPhong.Text;
            if (decimal.TryParse(txtHangPhong_SuaDonGia.Text, out decimal heSo))
            {
                if (PhongDAO.UpdateHangPhong(tenHang, heSo))
                {
                    MessageBox.Show("Cập nhật giá hạng phòng thành công!");
                    LoadComboBoxData(); // Refresh lại dữ liệu ở form cha
                    LoadDGV();
                }
                else MessageBox.Show("Lỗi cập nhật.");
            }
            else MessageBox.Show("Hệ số giá phải là số.");
        }

        private void ThoatHangPhong_Click(object sender, EventArgs e) => FormHangPhong.Visible = false;
        private void HangPhong_SuaDonGia_TextChanged(object sender, EventArgs e) { }
        private void HangPhong_ThemHangPhong_TextChanged(object sender, EventArgs e) { }
        private void HangPhong_ThemDonGia_TextChanged(object sender, EventArgs e) { }

        #endregion

        #region QUẢN LÝ HÌNH THỨC PHÒNG (MODE)

        private void HinhThucPhong_Click(object sender, EventArgs e)
        {
            FormHinhThucPhong.Visible = true;
            FormHinhThucPhong.BringToFront();
            LoadCboHinhThucCon();
        }

        private void LoadCboHinhThucCon()
        {
            DataTable dt = PhongDAO.GetHinhThuc();
            cboHinhThucPhong_SuaHinhThuc.DataSource = dt;
            cboHinhThucPhong_SuaHinhThuc.DisplayMember = "TenHinhThuc";
            cboHinhThucPhong_SuaHinhThuc.ValueMember = "TenHinhThuc";
        }

        private void HinhThucPhong_SuaHinhThuc_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Khi chọn hình thức, lấy hệ số giá
            if (cboHinhThucPhong_SuaHinhThuc.SelectedValue != null)
            {
                string tenHT = cboHinhThucPhong_SuaHinhThuc.SelectedValue.ToString();
                decimal heSo = PhongDAO.GetHeSoHinhThuc(tenHT);
                txtHinhThucPhong_SuaDonGia.Text = heSo.ToString();
            }
        }

        private void LuuHinhThucPhong_Click(object sender, EventArgs e) // Nút Sửa Hình Thức
        {
            if (string.IsNullOrEmpty(txtHinhThucPhong_SuaDonGia.Text)) return;

            string tenHT = cboHinhThucPhong_SuaHinhThuc.Text;
            if (decimal.TryParse(txtHinhThucPhong_SuaDonGia.Text, out decimal heSo))
            {
                if (PhongDAO.UpdateHinhThuc(tenHT, heSo))
                {
                    MessageBox.Show("Cập nhật giá hình thức thành công!");
                    LoadDGV();
                }
                else MessageBox.Show("Lỗi cập nhật.");
            }
            else MessageBox.Show("Hệ số giá phải là số.");
        }

        private void ThoatHinhThucPhong_Click(object sender, EventArgs e) => FormHinhThucPhong.Visible = false;
        private void txtHinhThucPhong_SuaDonGia_TextChanged(object sender, EventArgs e) { }
        private void txtHinhThucPhong_ThemHinhThuc_TextChanged(object sender, EventArgs e) { }
        private void txtHinhThucPhong_ThemDonGia_TextChanged(object sender, EventArgs e) { }
        #endregion

        private async void btnKiemTraKeHoach_Click(object sender, EventArgs e)
        {
            // 1. Lấy dữ liệu từ giao diện
            if (cboHangPhong_SuaHangPhong.SelectedValue == null) return;
            string loaiHang = cboHangPhong_SuaHangPhong.SelectedValue.ToString();

            // false -> chạy bản lỗi, true -> chạy bản fix
            bool cheDoFix = true;

            // 2. Thông báo bắt đầu
            MessageBox.Show("NV1: Bắt đầu tra cứu hệ số...\n\n" +
                            "Hãy đợi 10s.\n");

            // 3. Gọi xuống Database (Chạy bất đồng bộ để không đơ Form)
            // Code sẽ dừng ở dòng 'await' này 10 giây, nhưng Form vẫn bấm được nút khác
            string ketQua = await Task.Run(() => PhongDAO.Demo_TraCuuHeSo_T1(loaiHang, cheDoFix));

            // 4. Sau 10 giây, hiện kết quả
            MessageBox.Show("KẾT QUẢ TỪ SERVER TRẢ VỀ CHO NV1:\n\n" + ketQua);
        }
    }
}