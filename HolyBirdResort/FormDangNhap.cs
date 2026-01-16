using System;
using System.Windows.Forms;
using HolyBirdResort.DAO; 
using HolyBirdResort.DTO; 

namespace HolyBirdResort
{
    public partial class FormDangNhap : Form
    {
        public FormDangNhap()
        {
            InitializeComponent();
            //Cho textbox mật khẩu hiện dấu *
            txtPassword.UseSystemPasswordChar = true;
        }

        private void Login_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // 1. Kiểm tra rỗng
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Tên đăng nhập và Mật khẩu!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 2. Gọi DAO kiểm tra trong Database
                TaiKhoan tk = TaiKhoanDAO.CheckLogin(username, password);

                if (tk != null)
                {
                    // 3. Kiểm tra trạng thái kích hoạt
                    if (tk.TrangThaiKichHoat != "Đã kích hoạt")
                    {
                        MessageBox.Show("Tài khoản này đã bị khóa hoặc chưa kích hoạt!", "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 4. Đăng nhập thành công -> Lưu vào Session
                    AuthSession.CurrentUser = tk;

                    MessageBox.Show($"Xin chào đoàn {tk.MaDoan}!", "Đăng nhập thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // 5. Mở Form Trang Chủ Khách Hàng (Giả sử tên form là FormTrangChuKH)
                    this.Hide();

                    FormTrangChuKH frmHome = new FormTrangChuKH();
                    frmHome.ShowDialog();

                    this.Close();
                }
                else
                {
                    // Đăng nhập thất bại
                    MessageBox.Show("Tên đăng nhập hoặc Mật khẩu không đúng!", "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối Database: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pbQuayLai_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var formTongQuan = new FormTongQuan())
            {
                formTongQuan.ShowDialog();
            }
            this.Close();
        }

        private void ctrlExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}