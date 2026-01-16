using System;
using System.Collections.Generic;
using System.Windows.Forms;
using HolyBirdResort.DAO;
using HolyBirdResort.DTO;

namespace HolyBirdResort
{
    public partial class TaoTaiKhoan : Form
    {
        // Biến lưu thông tin đoàn đang được chọn để tạo TK
        private string _maDoanDangChon = "";
        private int _soNguoiDangChon = 0;

        public TaoTaiKhoan()
        {
            InitializeComponent();
            this.Load += DanhSachDoan_Load;
        }

        private void Exit_Click(object sender, EventArgs e) => Application.Exit();
        private void QuayLai_Click(object sender, EventArgs e) => this.Close();

        // 1. Load danh sách đoàn chưa có tài khoản
        private void DanhSachDoan_Load(object sender, EventArgs e)
        {
            LoadDataGrid();
            FormTaoTaiKhoan.Visible = false; // Ẩn panel con
        }

        private void LoadDataGrid()
        {
            dgvDoan.Rows.Clear();
            List<Doan> list = TaiKhoanDAO.GetListDoanChuaCoTaiKhoan();

            foreach (var item in list)
            {
                int index = dgvDoan.Rows.Add(
                    item.MaDoan,
                    item.SoNguoi,
                    item.TenTruongDoan,
                    "+" // Nút tạo
                );

                // Lưu object vào Tag để dùng lại
                dgvDoan.Rows[index].Tag = item;
            }
        }

        // 2. Xử lý khi bấm nút (+) trên lưới
        private void dgvDoan_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Kiểm tra có phải cột nút bấm không (Giả sử tên cột là "colTaoTaiKhoan" hoặc header "+")
            if (dgvDoan.Columns[e.ColumnIndex].Name == "colTaoTaiKhoan" || dgvDoan.Columns[e.ColumnIndex].HeaderText == "+")
            {
                // Lấy dữ liệu từ Tag
                if (dgvDoan.Rows[e.RowIndex].Tag is Doan data)
                {
                    _maDoanDangChon = data.MaDoan;
                    _soNguoiDangChon = data.SoNguoi;

                    // Gợi ý tên đăng nhập = Mã đoàn luôn cho tiện
                    txtTenTaiKhoan.Text = data.MaDoan;
                    txtMatKhau.Text = "123"; // Pass mặc định

                    FormTaoTaiKhoan.Visible = true;
                    FormTaoTaiKhoan.BringToFront();
                }
            }
        }

        // 3. Nút LƯU trên Form con
        private void LuuTaiKhoan_Click(object sender, EventArgs e)
        {
            // Validate
            string user = txtTenTaiKhoan.Text.Trim();
            string pass = txtMatKhau.Text.Trim();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Tên đăng nhập và Mật khẩu!", "Cảnh báo"); return;
            }

            if (TaiKhoanDAO.CheckUsernameExist(user))
            {
                MessageBox.Show("Tên đăng nhập này đã tồn tại! Vui lòng chọn tên khác.", "Trùng lặp"); return;
            }

            // Tạo DTO
            TaiKhoan tk = new TaiKhoan
            {
                MaDoan = _maDoanDangChon,
                TenDangNhap = user,
                MatKhau = pass,
                SoNguoiDung = _soNguoiDangChon // Số người dùng = Số khách trong đoàn (Trigger DB cũng tự sync cái này)
            };

            // Gọi DAO
            if (TaiKhoanDAO.TaoTaiKhoan(tk))
            {
                MessageBox.Show($"Đã tạo tài khoản cho đoàn {_maDoanDangChon} thành công!", "Thông báo");
                FormTaoTaiKhoan.Visible = false;
                LoadDataGrid(); // Load lại lưới để đoàn vừa tạo biến mất (vì giờ nó đã có TK rồi)
            }
            else
            {
                MessageBox.Show("Lỗi Database: Không thể tạo tài khoản.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ThoatTaoTaiKhoan_Click(object sender, EventArgs e) => FormTaoTaiKhoan.Visible = false;
        private void TenTaiKhoan_TextChanged(object sender, EventArgs e) { }
        private void MatKhau_TextChanged(object sender, EventArgs e) { }
    }
}