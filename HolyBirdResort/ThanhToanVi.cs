using Guna.UI2.WinForms;
using System;
using System.Windows.Forms;
using HolyBirdResort.DTO;
using HolyBirdResort.DAO;

namespace HolyBirdResort
{
    public partial class ThanhToanVi : Form
    {
        private ThongTinHoaDonTienMat _info;
        public ThanhToanVi()
        {
            InitializeComponent();
        }

        // Constructor nhận thông tin hóa đơn từ form Trả Phòng
        public ThanhToanVi(ThongTinHoaDonTienMat info) : this()
        {
            _info = info;
        }

        private void ThanhToanVi_Load(object sender, System.EventArgs e)
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
        }



        private void btnBack_Click(object sender, EventArgs e)
        {
            //this.DialogResult = DialogResult.Cancel; // báo về form trước là người dùng quay lại
            this.Close(); // đóng form
        }

        // Trong các Form: HoaDonTienMat, ThanhToanThe, ThanhToanVi
        private void btnThanhToan_Click(object sender, System.EventArgs e)
        {
            // 1. Gọi hàm cập nhật Database
            // Giả sử bạn đã lưu _maDoan, _maPhong vào biến toàn cục của Form này khi mở nó lên
            bool success = GiaoDichDAO.CapNhatThanhToanXong(this._maDoan, this._maPhong);

            if (success)
            {
                var msg = new Guna2MessageDialog
                {
                    Parent = this,
                    Style = MessageDialogStyle.Light,
                    Caption = "Thanh toán thành công",
                    Icon = MessageDialogIcon.Information,
                    Text = "Thanh toán thành công.\nCảm ơn quý khách!"
                };
                msg.Show();

                // 2. Trả về DialogResult.OK để Form cha biết là thanh toán xong
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra trong quá trình cập nhật thanh toán.");
            }
        }

        private void guna2ControlBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
