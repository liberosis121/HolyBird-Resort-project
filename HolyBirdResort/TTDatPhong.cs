using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using HolyBirdResort.DAO;
using HolyBirdResort.DTO;

namespace HolyBirdResort
{
    public partial class TTDatPhong : Form
    {
        // --- NHẬN DỮ LIỆU TỪ FORM TRƯỚC ---
        public string MaPhongCanDat { get; set; }
        public DateTime NgayNhanDuKien { get; set; }
        public DateTime NgayTraDuKien { get; set; }

        private bool dangLoadCombo = false;
        private List<KhachHang> dsKhach;
        private Phong _phongHienTai;

        public TTDatPhong()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 1. Load thông tin Phòng từ DB
            LoadThongTinPhong();

            // 2. Load danh sách Khách Hàng thật từ DB
            LoadKhachHangLenCombo();

            // 3. Gán ngày giờ đã chọn từ form trước
            dtpNgayNhan.Value = NgayNhanDuKien;
            dtpNgayTra.Value = NgayTraDuKien;

            TinhGiaUocTinh();
        }

        private void LoadThongTinPhong()
        {
            if (string.IsNullOrEmpty(MaPhongCanDat)) return;

            _phongHienTai = PhongDAO.GetPhongByMa(MaPhongCanDat);
            if (_phongHienTai != null)
            {
                txtSoPhong.Text = _phongHienTai.MaPhong;
                txtTang.Text = _phongHienTai.MaTang;
                txtHangPhong.Text = _phongHienTai.LoaiHang;
                txtHinhThuc.Text = _phongHienTai.TenHinhThuc;
                txtGiaMotDem.Text = $"{_phongHienTai.GiaPhong:N0} kđ / giờ";
            }
        }

        private void LoadKhachHangLenCombo()
        {
            dangLoadCombo = true;

            // Lấy Mã Đoàn từ phiên đăng nhập 
            string maDoanHienTai = "";
            if (AuthSession.CurrentUser != null)
            {
                maDoanHienTai = AuthSession.CurrentUser.MaDoan;
            }
            else
            {
                // Fallback nếu chạy test chưa đăng nhập (tránh crash)
                MessageBox.Show("Chưa đăng nhập! Đang chạy chế độ test (Mặc định lấy D001).");
                maDoanHienTai = "D001";
            }

            // Gọi hàm DAO mới đã sửa ở Bước 1
            dsKhach = KhachHangDAO.GetKhachHangByMaDoan(maDoanHienTai);

            cboKhachHang.DataSource = dsKhach;
            cboKhachHang.DisplayMember = "TenKH";
            cboKhachHang.ValueMember = "MaKH";
            cboKhachHang.SelectedIndex = -1;
            dangLoadCombo = false;
        }

        private void cboKhachHang_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dangLoadCombo || cboKhachHang.SelectedIndex < 0) return;

            KhachHang kh = (KhachHang)cboKhachHang.SelectedItem;
            txtCMND.Text = kh.SoCMND;
            txtTenKH.Text = kh.TenKH;
            dtpNgaySinh.Value = kh.NgaySinh;
            txtSDT.Text = kh.SDT;

            // --- KHÓA GIAO DIỆN (Chỉ xem, không sửa) ---
            txtCMND.ReadOnly = true;
            txtTenKH.ReadOnly = true;
            txtSDT.ReadOnly = true;
    
            // Với DateTimePicker, Enabled = false là cách chặn tốt nhất
            dtpNgaySinh.Enabled = false;
        }

        private void TinhGiaUocTinh()
        {
            // Kiểm tra dữ liệu phòng
            if (_phongHienTai == null) return;

            DateTime nhan = dtpNgayNhan.Value;
            DateTime tra = dtpNgayTra.Value;

            // Validate: Nếu ngày trả nhỏ hơn hoặc bằng ngày nhận -> Không tính
            if (tra <= nhan)
            {
                txtGiaUocTinh.Text = "0 k₫";
                return;
            }

            // --- LOGIC MỚI: TÍNH THEO GIỜ (Khớp với Trigger SQL) ---

            // 1. Tính tổng số giờ chênh lệch
            TimeSpan duration = tra - nhan;

            // 2. Làm tròn lên (Ví dụ: 1 tiếng 1 phút -> tính 2 tiếng). 
            // Dùng Math.Ceiling giống logic CEILING() trong SQL
            double soGio = Math.Ceiling(duration.TotalHours);

            // Tối thiểu là 1 giờ
            if (soGio < 1) soGio = 1;

            // 3. Tính tiền: Số giờ * Giá phòng
            // Lưu ý: Trigger SQL có đoạn chia cho số người (/ GC.SoNguoi). 
            // Nhưng đây là "Giá ước tính" cho khách đang đặt, nên ta tạm tính là họ trả 100% tiền phòng.
            decimal thanhTien = (decimal)soGio * _phongHienTai.GiaPhong;

            // 4. Hiển thị (Giả định GiaPhong trong DB lưu 50 nghĩa là 50k)
            txtGiaUocTinh.Text = $"{thanhTien:N0} k₫";
        }

        private void dtpNgayNhan_ValueChanged(object sender, EventArgs e) => TinhGiaUocTinh();
        private void dtpNgayTra_ValueChanged(object sender, EventArgs e) => TinhGiaUocTinh();

        private void btnDatPhong_Click(object sender, EventArgs e)
        {
            // --- VALIDATE LOGIC NGHIỆP VỤ ---

            // 1. Chưa chọn khách
            if (cboKhachHang.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn khách hàng!", "Thiếu thông tin"); return;
            }

            // 2. Ngày trả phải sau ngày nhận
            if (dtpNgayTra.Value <= dtpNgayNhan.Value)
            {
                MessageBox.Show("Thời gian trả phòng phải sau thời gian nhận!", "Lỗi thời gian"); return;
            }

            // 3. KIỂM TRA TRÙNG LỊCH (QUAN TRỌNG NHẤT)
            // Gọi DAO để check DB xem khoảng thời gian này phòng có trống không
            bool isPhongTrong = PhongDAO.CheckPhongTrong(MaPhongCanDat, dtpNgayNhan.Value, dtpNgayTra.Value);

            if (!isPhongTrong)
            {
                // TODO: Logic check chung đoàn (Nâng cao)
                // Nếu muốn cho phép chung đoàn đặt trùng -> Cần check xem cái booking đang chiếm chỗ đó có cùng MaDoan với khách này không.
                // Nhưng logic đơn giản nhất là: Phòng đã có người ở thì không cho đặt mới, trừ khi người kia trả phòng xong.

                MessageBox.Show($"Phòng {MaPhongCanDat} đã có người đặt trong khoảng thời gian này!\nVui lòng chọn thời gian khác.",
                                "Hết phòng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // --- TRUYỀN DỮ LIỆU SANG FORM XÁC NHẬN ---
            KhachHang kh = (KhachHang)cboKhachHang.SelectedItem;

            var f = new XacNhanDat();
            f.MaPhong = MaPhongCanDat;
            f.MaKH = kh.MaKH; // Truyền ID để lưu DB
            f.MaDoan = kh.MaDoan; // Giả sử Khách có mã đoàn

            // Truyền thông tin hiển thị
            f.TenKhach = kh.TenKH;
            f.CCCD = kh.SoCMND;
            f.NgaySinh = kh.NgaySinh;
            f.SDT = kh.SDT;
            f.Tang = txtTang.Text;
            f.SoPhong = txtSoPhong.Text;
            f.HangPhong = txtHangPhong.Text;
            f.HinhThuc = txtHinhThuc.Text;
            f.NgayNhan = dtpNgayNhan.Value;
            f.NgayTra = dtpNgayTra.Value;
            f.GiaUocTinh = txtGiaUocTinh.Text;

            this.Hide();
            using (var form = new XacNhanDat())
            {
                // Lưu ý: Đoạn code cũ của bạn new XacNhanDat() 2 lần. Hãy sửa lại:
                f.ShowDialog();
            }
            this.Show();
        }

        private void btnQuaylai_Click(object sender, EventArgs e) => this.Close();
        private void controlboxExit_Click(object sender, EventArgs e) => Application.Exit();
    }
}