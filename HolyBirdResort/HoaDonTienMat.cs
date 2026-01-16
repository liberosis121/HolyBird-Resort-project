using Guna.UI2.WinForms;
using HolyBirdResort.DAO;
using HolyBirdResort.DTO;
using System.Windows.Forms;

namespace HolyBirdResort
{
    public partial class HoaDonTienMat : Form
    {
        private ThongTinHoaDonTienMat _info;


        private string _maDoan;
        private string _maPhong;

        public HoaDonTienMat(ThongTinHoaDonTienMat info, string maDoan, string maPhong) : this()
        {
            _info = info;
            _maDoan = maDoan; // Gán giá trị được truyền sang
            _maPhong = maPhong;
        }

        // Constructor mặc định (Designer cần cái này)
        public HoaDonTienMat()
        {
            InitializeComponent();
        }

        // Constructor nhận thông tin hóa đơn từ form Trả Phòng
        public HoaDonTienMat(ThongTinHoaDonTienMat info) : this()
        {
            _info = info;
        }

        private void HoaDonTienMat_Load(object sender, System.EventArgs e)
        {
            if (_info == null) return;

            // Đảm bảo tên các Label này đúng với tên bạn đặt trong Designer của Form HoaDonTienMat
            lblSoPhongHD.Text = _info.SoPhong;
            lblTangHD.Text = _info.Tang;
            lblNgayNhanHD.Text = _info.NgayNhan;
            lblNgayTraHD.Text = _info.NgayTra;
            lblGiaThueHD.Text = _info.GiaThue;
            lblPhiBoiThuongHD.Text = _info.PhiBoiThuong;
            lblTongThanhToanHD.Text = _info.TongThanhToan;
        }

        private void btnDong_Click(object sender, System.EventArgs e)
        {
            Application.Exit();
        }

        private void btnBack_Click(object sender, System.EventArgs e)
        {
            //this.DialogResult = DialogResult.Cancel; // báo về form trước là người dùng quay lại
            this.Close(); // đóng form 
        }

        private void controlboxExit_Click(object sender, System.EventArgs e)
        {
            Application.Exit();
        }

        //private void btnHome_Click(object sender, System.EventArgs e)
        //{
        //    FormTrangChuKH nxtform = new FormTrangChuKH();
        //    nxtform.Show();
        //    this.Close();
        //}

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
    }
}
