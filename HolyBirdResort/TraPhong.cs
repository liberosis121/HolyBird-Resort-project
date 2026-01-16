using Guna.UI2.WinForms;
using HolyBirdResort.DAO;
using HolyBirdResort.DTO;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace HolyBirdResort
{
    public partial class TraPhong : Form
    {

        private string _maDoan, _maPhong;

        public TraPhong(string maDoan, string maPhong) : this()
        {
            _maDoan = maDoan;
            _maPhong = maPhong;
        }

        public TraPhong()
        {
            InitializeComponent();

        }

        private void TraPhong_Load(object sender, EventArgs e)
        {
            // 1. Nạp danh sách khách hàng
            DataTable dtKhach = GiaoDichDAO.GetKhachHangTrongPhong(_maDoan, _maPhong);
            dgvTTKhach.AutoGenerateColumns = false;

            // Gán theo Index cột để tránh lỗi đặt tên sai trong Designer (Index bắt đầu từ 0)
            if (dgvTTKhach.Columns.Count >= 4)
            {
                dgvTTKhach.Columns[0].DataPropertyName = "Họ và tên"; // Khớp với câu SELECT SQL
                dgvTTKhach.Columns[1].DataPropertyName = "CCCD";
                dgvTTKhach.Columns[2].DataPropertyName = "SĐT";
                dgvTTKhach.Columns[3].DataPropertyName = "Ngày Sinh";
            }
            dgvTTKhach.DataSource = dtKhach;

            // 2. Nạp tổng hợp bồi thường
            DataTable dtBoiThuong = GiaoDichDAO.GetTongHopBoiThuong(_maDoan, _maPhong);
            dgvBoiThuong.AutoGenerateColumns = false;
            if (dgvBoiThuong.Columns.Count >= 4)
            {
                dgvBoiThuong.Columns[0].DataPropertyName = "Danh mục bồi thường";
                dgvBoiThuong.Columns[1].DataPropertyName = "Số lượng";
                dgvBoiThuong.Columns[2].DataPropertyName = "Đơn giá";
                dgvBoiThuong.Columns[3].DataPropertyName = "Thành tiền";
            }
            dgvBoiThuong.DataSource = dtBoiThuong;

            // ... Load thông tin phòng và tiền bạc như cũ ...
            LoadThongTinPhongReal();
            LoadThanhTienReal();

            btnXacNhan.Enabled = false;
            btnXacNhan.FillColor = Color.LightGray;   // màu xám để nhìn giống disabled

            // Click vào panel / label => chọn phương thức tương ứng
            pnlTienMat.Click += (s, ev) => ChonPhuongThuc(pnlTienMat, radTienMat);
            lblTienMat.Click += (s, ev) => ChonPhuongThuc(pnlTienMat, radTienMat);
            radTienMat.CheckedChanged += (s, ev) =>
            {
                if (radTienMat.Checked)
                    ChonPhuongThuc(pnlTienMat, radTienMat);
            };

            pnlThe.Click += (s, ev) => ChonPhuongThuc(pnlThe, radThe);
            lblThe.Click += (s, ev) => ChonPhuongThuc(pnlThe, radThe);
            radThe.CheckedChanged += (s, ev) =>
            {
                if (radThe.Checked)
                    ChonPhuongThuc(pnlThe, radThe);
            };

            pnlVi.Click += (s, ev) => ChonPhuongThuc(pnlVi, radVi);
            lblVi.Click += (s, ev) => ChonPhuongThuc(pnlVi, radVi);
            radVi.CheckedChanged += (s, ev) =>
            {
                if (radVi.Checked)
                    ChonPhuongThuc(pnlVi, radVi);
            };
        }

        private void LoadThongTinPhongReal()
        {
            // Sử dụng _maDoan và _maPhong đã được truyền vào Constructor
            DataRow dr = GiaoDichDAO.GetChiTietDeHuy("", _maDoan, _maPhong);

            if (dr != null)
            {
                lblSoPhong.Text = _maPhong;
                lblSoTang.Text = dr["MaTang"].ToString();
                lblHangPhong.Text = dr["LoaiHang"].ToString();
                lblHinhThuc.Text = dr["TenHinhThuc"].ToString();

                DateTime ngayNhan = Convert.ToDateTime(dr["ThoiGianNhanPhong"]);
                DateTime ngayTra = Convert.ToDateTime(dr["ThoiGianTraPhong"]);

                lblNgayNhan.Text = ngayNhan.ToString("dd/MM/yyyy HH:mm");
                lblNgayTra.Text = ngayTra.ToString("dd/MM/yyyy HH:mm");


                // Load giá thuê theo phòng (Nếu cần)
                lblDonGia.Text = dr["GiaPhong"].ToString();
            }
        }

        private void LoadThanhTienReal()
        {
            // Lấy tổng tiền thuê của tất cả khách trong phòng từ DB
            decimal tongTienThue = GiaoDichDAO.GetTongTienThuePhong(_maDoan, _maPhong);

            // Tính tổng tiền bồi thường từ Grid
            decimal tongBoiThuong = 0;
            if (dgvBoiThuong.DataSource is DataTable dt)
            {
                foreach (DataRow row in dt.Rows)
                {
                    tongBoiThuong += Convert.ToDecimal(row["Thành tiền"]);
                }
            }

            // Hiển thị lên Label
            lblGiaThue.Text = tongTienThue.ToString("#,##0") + " k₫";
            lblPhiBoiThuong.Text = tongBoiThuong.ToString("#,##0") + " k₫";
            lblTongTT.Text = (tongTienThue + tongBoiThuong).ToString("#,##0") + " k₫";
        }

        // Đổi màu panel về trạng thái bình thường
        private void ResetPanel(Guna.UI2.WinForms.Guna2Panel pnl)
        {
            pnl.BorderColor = Color.Silver;
            pnl.FillColor = Color.White;
        }

        // Chọn 1 phương thức: đảm bảo chỉ 1 radio = true
        private void ChonPhuongThuc(Guna.UI2.WinForms.Guna2Panel pnlSelected,
                                     Guna.UI2.WinForms.Guna2RadioButton radSelected)
        {
            // 1. Uncheck hết radio, rồi check lại cái được chọn
            radTienMat.Checked = (radSelected == radTienMat);
            radThe.Checked = (radSelected == radThe);
            radVi.Checked = (radSelected == radVi);

            // 2. Reset màu tất cả panel
            ResetPanel(pnlTienMat);
            ResetPanel(pnlThe);
            ResetPanel(pnlVi);

            // 3. Làm nổi panel đang chọn
            pnlSelected.BorderColor = Color.FromArgb(25, 118, 210);
            pnlSelected.FillColor = Color.FromArgb(235, 245, 255);
        }

        private string LayPhuongThuc()
        {
            if (radTienMat.Checked) return "Tiền mặt tại quầy";
            if (radThe.Checked) return "Thẻ tín dụng / ghi nợ";
            if (radVi.Checked) return "Ví điện tử / QR";
            return null;
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            string pt = LayPhuongThuc();

            if (pt == null)
            {
                msgXacNhanTT.Caption = "Thiếu thông tin";
                msgXacNhanTT.Text = "Vui lòng chọn phương thức thanh toán.";
                msgXacNhanTT.Icon = MessageDialogIcon.Warning;
                msgXacNhanTT.Buttons = MessageDialogButtons.OK;
                msgXacNhanTT.Show();
                return;
            }

            msgXacNhanTT.Caption = "Xác nhận thanh toán";
            msgXacNhanTT.Text = $"Bạn chọn phương thức: {pt}\nBạn có muốn tiếp tục?";
            msgXacNhanTT.Icon = MessageDialogIcon.Question;
            msgXacNhanTT.Buttons = MessageDialogButtons.YesNo;

            if (msgXacNhanTT.Show() == DialogResult.Yes)
            {
                // Gom dữ liệu chung cho cả 3 form
                // Trong btnXacNhan_Click của TraPhong.cs

                ThongTinHoaDonTienMat info = new ThongTinHoaDonTienMat
                {
                    SoPhong = lblSoPhong.Text,
                    Tang = lblSoTang.Text,
                    NgayNhan = lblNgayNhan.Text,
                    NgayTra = lblNgayTra.Text,
                    GiaThue = lblGiaThue.Text,
                    PhiBoiThuong = lblPhiBoiThuong.Text,
                    TongThanhToan = lblTongTT.Text
                };


                bool thanhToanXong = false;
                // Điều hướng theo phương thức
                if (pt == "Tiền mặt tại quầy")
                {
                    using (var f = new HoaDonTienMat(info))
                    {
                        if (f.ShowDialog() == DialogResult.OK) thanhToanXong = true;
                    }
                }
                else if (pt.StartsWith("Thẻ"))
                {
                    using (var f = new ThanhToanThe(info))
                    {
                        if (f.ShowDialog() == DialogResult.OK) thanhToanXong = true;
                    }
                }
                else // Ví điện tử
                {
                    using (var f = new ThanhToanVi(info))
                    {
                        if (f.ShowDialog() == DialogResult.OK) thanhToanXong = true;
                    }
                }

                if (thanhToanXong)
                {
                    this.DialogResult = DialogResult.OK; // Trả về tiếp cho ucRoomBooking
                    this.Close();
                }

            }
        }

        private void chkDongYTraPhong_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDongYTraPhong.Checked)
            {
                btnXacNhan.Enabled = true;
                btnXacNhan.FillColor = Color.FromArgb(86, 145, 73); // xanh lá đẹp
            }
            else
            {
                btnXacNhan.Enabled = false;
                btnXacNhan.FillColor = Color.LightGray;
            }
        }

        private ThongTinHoaDonTienMat LayThongTinHoaDon()
        {
            return new ThongTinHoaDonTienMat
            {
                SoPhong = lblSoPhong.Text,
                Tang = lblSoTang.Text,
                NgayNhan = lblNgayNhan.Text,
                NgayTra = lblNgayTra.Text,
                GiaThue = lblGiaThue.Text,
                PhiBoiThuong = lblPhiBoiThuong.Text,
                TongThanhToan = lblTongTT.Text
            };
        }

        private void btnQuayLai_Click(object sender, EventArgs e)
        {
            //this.DialogResult = DialogResult.Cancel; // báo về form trước là người dùng quay lại
            this.Close(); // đóng form 
        }

        private void controlboxExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
