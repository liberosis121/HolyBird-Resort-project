using Guna.UI2.WinForms;
using HolyBirdResort.DAO;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HolyBirdResort
{
    public partial class HuyPhong : Form
    {
        private bool _laChiTietCuoiCung;
        private string _maKH, _tenKH, _soPhong;
        public string CurrentMaDoan { get; set; } // Nhận mã đoàn truyền từ UC

        public HuyPhong(string maKH, string tenKH, string soPhong, bool laCuoiCung)
        {
            InitializeComponent();
            this._maKH = maKH;
            this._tenKH = tenKH;
            this._soPhong = soPhong;
            this._laChiTietCuoiCung = laCuoiCung;
        }

        private void HuyPhong_Load(object sender, EventArgs e)
        {
            lblHoTen.Text = _tenKH;
            lblSoPhong.Text = _soPhong;
            HienThiChiTietReal();
        }

        private void HienThiChiTietReal()
        {
            try
            {
                // 1. Gọi DAO lấy dữ liệu thật từ Database
                // Lưu ý: Đảm bảo bạn đã truyền CurrentMaDoan từ UserControl sang
                System.Data.DataRow dr = GiaoDichDAO.GetChiTietDeHuy(_maKH, CurrentMaDoan, _soPhong);

                if (dr != null)
                {
                    // 2. Nạp thông tin khách hàng
                    lblCCCD.Text = dr["SoCMND"].ToString();
                    lblNgaySinh.Text = Convert.ToDateTime(dr["NgaySinh"]).ToString("dd/MM/yyyy");
                    lblSDT.Text = dr["SDT"].ToString();

                    // 3. Nạp thông tin phòng
                    lblSoTang.Text = "Tầng " + dr["MaTang"].ToString();
                    lblHangPhong.Text = dr["LoaiHang"].ToString();
                    lblHinhThuc.Text = dr["TenHinhThuc"].ToString();

                    // 4. Nạp thông tin thời gian & giá tiền
                    DateTime ngayNhan = Convert.ToDateTime(dr["ThoiGianNhanPhong"]);
                    DateTime ngayTra = Convert.ToDateTime(dr["ThoiGianTraPhong"]);
                    lblNgayNhan.Text = ngayNhan.ToString("dd/MM/yyyy HH:mm");
                    lblNgayTra.Text = ngayTra.ToString("dd/MM/yyyy HH:mm");

                    // Hiển thị giá thuê (giả định đơn vị tính là 'k' VNĐ theo giao diện của bạn)
                    decimal gia = Convert.ToDecimal(dr["ThanhTien"]);
                    lblGiaThue.Text = string.Format("{0:N0} k₫", gia);
                }
                else
                {
                    MessageBox.Show("Không tìm thấy thông tin chi tiết cho giao dịch này.", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu chi tiết: " + ex.Message, "Lỗi hệ thống");
            }
        }

        // Lấy tất cả lý do mà khách đã chọn
        private string LayLyDoHuy()
        {
            List<string> reasons = new List<string>();

            if (chkDoiKeHoach.Checked)
                reasons.Add("Đổi kế hoạch / lịch trình");

            if (chkKhongConNhuCau.Checked)
                reasons.Add("Không còn nhu cầu");

            if (chkMuonGopPhong.Checked)
                reasons.Add("Muốn gộp phòng với khách khác");

            if (chkTimResortKhac.Checked)
                reasons.Add("Tìm được resort khác phù hợp hơn");

            if (chkLyDoKhac.Checked && !string.IsNullOrWhiteSpace(txtLyDoKhac.Text))
                reasons.Add(txtLyDoKhac.Text.Trim());

            return string.Join("; ", reasons);
        }

        private void chkLyDoKhac_CheckedChanged(object sender, EventArgs e)
        {
            txtLyDoKhac.Enabled = chkLyDoKhac.Checked;
            if (!chkLyDoKhac.Checked)
                txtLyDoKhac.Text = string.Empty;
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra lý do
            string lyDo = LayLyDoHuy();
            if (string.IsNullOrWhiteSpace(lyDo))
            {
                msgXacNhanHuy.Icon = MessageDialogIcon.Warning;
                msgXacNhanHuy.Buttons = MessageDialogButtons.OK;
                msgXacNhanHuy.Caption = "Lỗi";
                msgXacNhanHuy.Text = "Vui lòng chọn hoặc nhập ít nhất một lý do hủy phòng.";
                msgXacNhanHuy.Show();
                return;
            }

            // 2. Popup xác nhận hủy
            msgXacNhanHuy.Icon = MessageDialogIcon.Question;
            msgXacNhanHuy.Buttons = MessageDialogButtons.YesNo;
            msgXacNhanHuy.Caption = "Xác nhận hủy phòng";
            msgXacNhanHuy.Text =
                $"Bạn có chắc chắn muốn hủy đăng ký phòng {lblSoPhong.Text} cho khách {lblHoTen.Text} không?";

            var confirm = msgXacNhanHuy.Show();
            if (confirm != DialogResult.Yes)
                return;

            // 3. Trường hợp đặc biệt: xóa tài khoản đoàn
            if (_laChiTietCuoiCung)
            {
                msgCanhBaoDoan.Icon = MessageDialogIcon.Warning;
                msgCanhBaoDoan.Buttons = MessageDialogButtons.YesNo;
                msgCanhBaoDoan.Caption = "Cảnh báo quan trọng";
                msgCanhBaoDoan.Text =
                    "Đây là đăng ký cuối cùng của đoàn.\n" +
                    "Nếu tiếp tục, tài khoản đoàn mà resort cấp sẽ bị vô hiệu hóa.\n\n" +
                    "Bạn có muốn tiếp tục thực hiện không?";

                var confirm2 = msgCanhBaoDoan.Show();
                if (confirm2 != DialogResult.Yes)
                    return;

                msgThanhCong.Icon = MessageDialogIcon.Information;
                msgThanhCong.Caption = "Hoàn tất";
                msgThanhCong.Text =
                    "Đã hủy phòng và vô hiệu hóa tài khoản đoàn.\n\n" +
                    "Lý do hủy: " + lyDo;

                msgThanhCong.Show();
            }

            try
            {
                // Gọi hàm xóa bản ghi CTGD trong DAO
                bool isDeleted = GiaoDichDAO.XoaChiTietGiaoDich(_maKH, CurrentMaDoan, _soPhong);

                if (isDeleted)
                {
                    msgThanhCong.Icon = MessageDialogIcon.Information;
                    msgThanhCong.Caption = "Thành công";
                    msgThanhCong.Text = "Đã xóa chi tiết đăng ký phòng thành công.\nHệ thống đã tự động cập nhật lại trạng thái phòng và đoàn.";
                    msgThanhCong.Show();

                    this.DialogResult = DialogResult.OK; // Trả về OK để UserControl load lại Grid
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy dữ liệu để xóa hoặc dữ liệu đã bị thay đổi.", "Lỗi thực thi");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi Database: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnQuayLai_Click(object sender, EventArgs e)
        {
            // Chỉ cần đóng form này lại, ucRoomBooking sẽ tự hiện lại form cha (TtinTK)
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void controlboxExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
