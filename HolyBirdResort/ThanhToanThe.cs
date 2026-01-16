using Guna.UI2.WinForms;
using System.Windows.Forms;
using HolyBirdResort.DTO;

namespace HolyBirdResort
{
    public partial class ThanhToanThe : Form
    {
        private ThongTinHoaDonTienMat _info;

        // Constructor mặc định (Designer cần cái này)
        public ThanhToanThe()
        {
            InitializeComponent();
        }

        // Constructor nhận thông tin hóa đơn từ form Trả Phòng
        public ThanhToanThe(ThongTinHoaDonTienMat info) : this()
        {
            _info = info;
        }

        private void ThanhToanThe_Load(object sender, System.EventArgs e)
        {
            // Khi mở trong Designer thì _info = null, nên chặn lại cho an toàn
            if (_info == null) return;
            lblSoPhongHD.Text = _info.SoPhong;
            lblTangHD.Text = _info.Tang;
            lblNgayNhanHD.Text = _info.NgayNhan;
            lblNgayTraHD.Text = _info.NgayTra;
            lblGiaThueHD.Text = _info.GiaThue;
            lblPhiBoiThuongHD.Text = _info.PhiBoiThuong;
            lblTongThanhToanHD.Text = _info.TongThanhToan;
            txtCVV.PasswordChar = '*';
        }

        private void btnThanhToan_Click(object sender, System.EventArgs e)
        {
            // === KIỂM TRA ĐẦY ĐỦ THÔNG TIN ===
            if (string.IsNullOrWhiteSpace(txtSoThe.Text) ||
                string.IsNullOrWhiteSpace(txtTenChuThe.Text) ||
                string.IsNullOrWhiteSpace(txtNgayHetHan.Text) ||
                string.IsNullOrWhiteSpace(txtCVV.Text))
            {
                var msgError = new Guna2MessageDialog
                {
                    Parent = this,
                    Style = MessageDialogStyle.Light,
                    Caption = "Thiếu thông tin",
                    Icon = MessageDialogIcon.Warning,
                    Text = "Vui lòng nhập đầy đủ thông tin thẻ trước khi thanh toán.",
                    Buttons = MessageDialogButtons.OK
                };
                msgError.Show();
                return;
            }

            // === THANH TOÁN THÀNH CÔNG (demo) ===
            var msg = new Guna2MessageDialog
            {
                Parent = this,
                Style = MessageDialogStyle.Light,
                Caption = "Thanh toán thẻ",
                Icon = MessageDialogIcon.Information,
                Text = "Thanh toán bằng thẻ thành công.\nCảm ơn quý khách!",
                Buttons = MessageDialogButtons.OK
            };
            msg.Show();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnBack_Click(object sender, System.EventArgs e)
        {
            //this.DialogResult = DialogResult.Cancel; // báo về form trước là người dùng quay lại
            this.Close(); // đóng form ThanhToanThe
        }

        private void guna2ControlBox1_Click(object sender, System.EventArgs e)
        {
            Application.Exit();
        }
    }
}
