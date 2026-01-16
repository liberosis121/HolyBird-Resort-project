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

        private void guna2ControlBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
