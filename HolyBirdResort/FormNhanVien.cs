using System;
using System.Windows.Forms;

namespace HolyBirdResort
{
    public partial class FormNhanVien : Form
    {
        public FormNhanVien()
        {
            InitializeComponent();

            // Mẹo nhỏ: Để ô nhập password hiển thị dấu * hoặc chấm tròn
            txtPasscode.UseSystemPasswordChar = true;
        }

        private void FornNhanVien_Load(object sender, EventArgs e)
        {
            // Focus vào ô nhập ngay khi mở form để tiện nhập luôn
            txtPasscode.Focus();
        }

        private void ctrlExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pbQuayLai_Click(object sender, EventArgs e)
        {
            // Quay lại màn hình chọn vai trò (Nếu có) hoặc thoát form này
            this.Close();
        }

        // Sự kiện nhấn phím Enter
        private void txtPasscode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Gọi hàm xử lý click
                btnXacThuc_Click(sender, e);

                // NẾU MUỐN NGHE TIẾNG DING KHI LỖI:
                // Ta chỉ Suppress (chặn tiếng) khi đăng nhập THÀNH CÔNG để chuyển form cho mượt.
                // Còn nếu thất bại (Form chưa đóng/ẩn), ta để nguyên cho nó kêu.

                //if (this.Visible == false) // Nếu form đã ẩn (tức là đăng nhập thành công)
                //{
                //    e.SuppressKeyPress = true;
                //}
                // Ngược lại: Không làm gì cả -> Windows sẽ phát tiếng "Ding" mặc định cho phím Enter
            }
        }

        // Logic nút xác thực
        private void btnXacThuc_Click(object sender, EventArgs e)
        {
            string inputCode = txtPasscode.Text.Trim();

            if (inputCode == "666")
            {
                this.Hide();
                FormTrangChuNV frmTrangChu = new FormTrangChuNV();
                frmTrangChu.ShowDialog();
                this.Close();
            }
            else
            {
                // 1. Phát âm thanh lỗi ngay lập tức (Chắc chắn nghe thấy)
                //System.Media.SystemSounds.Hand.Play();

                // 2. Hiện thông báo
                MessageBox.Show("Mã xác thực không đúng! Vui lòng thử lại.",
                                "Lỗi đăng nhập",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);

                txtPasscode.Clear();
                txtPasscode.Focus();
            }
        }
    }
}