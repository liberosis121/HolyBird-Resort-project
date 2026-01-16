using System;
using System.Drawing;
using System.Windows.Forms;
using HolyBirdResort.DTO; // Nhớ using DTO

namespace HolyBirdResort
{
    public partial class ucRoomDescription : UserControl
    {
        // Biến lưu trữ thông tin phòng hiện tại
        public Phong CurrentPhong { get; set; }

        public ucRoomDescription()
        {
            InitializeComponent();
        }

        // --- HÀM MỚI: NHẬN DỮ LIỆU TỪ FORM ---
        public void SetRoomData(Phong p)
        {
            this.CurrentPhong = p;

            // GÁN TRỰC TIẾP (Thay label1, label2 bằng tên thật trên Design của bạn)
            // Ví dụ:
            this.lblTenPhong.Text = $"{p.LoaiHang} - {p.MaPhong}";
            this.lblGiaPhong.Text = $"{p.GiaPhong:N0} kVND/đêm";
            this.lblThongTinKhac.Text = $"{p.TenHinhThuc} | {p.TrangThai}";

            // Logic đổi màu nút
            if (p.TrangThai == "Đã đặt")
            {
                btnBooking.Enabled = false;
                btnBooking.Text = "ĐÃ ĐẶT";
                btnBooking.FillColor = Color.Gray;
            }
            else
            {
                btnBooking.Enabled = true;
                btnBooking.Text = "ĐẶT PHÒNG NGAY";
                btnBooking.FillColor = Color.FromArgb(0, 118, 212);
            }
        }

        // --- SỰ KIỆN CLICK (Đã có sẵn, cập nhật logic truyền mã phòng) ---
        private void btnDetails_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra xem dữ liệu phòng hiện tại có tồn tại không
            if (this.CurrentPhong == null)
            {
                MessageBox.Show("Lỗi: Không tìm thấy dữ liệu phòng tại ô này!");
                return;
            }

            // 2. Ẩn form cha (Form tìm kiếm)
            var parentForm = this.FindForm();
            if (parentForm != null) parentForm.Hide();

            // 3. Mở form chi tiết và TRUYỀN MÃ
            using (var f = new FormChiTietPhong())
            {
                // --- QUAN TRỌNG: PHẢI GÁN VÀO ĐÚNG BIẾN 'MaPhongCanXem' ---
                // (Không dùng f.Tag nữa vì bên kia bạn đang dùng biến MaPhongCanXem)
                f.MaPhongCanXem = this.CurrentPhong.MaPhong;

                // Debug: Hiện thông báo để chắc chắn mã đã được lấy
                // MessageBox.Show("Đang xem phòng: " + f.MaPhongCanXem); 

                f.ShowDialog();
            }

            // 4. Hiện lại form cha sau khi tắt form chi tiết
            if (parentForm != null && !parentForm.IsDisposed) parentForm.Show();
        }

        private void btnBooking_Click(object sender, EventArgs e)
        {
            // Kiểm tra dữ liệu
            if (CurrentPhong == null) return;

            // Ẩn form cha (Form tìm kiếm)
            var parentForm = this.FindForm();
            if (parentForm != null) parentForm.Hide();

            // Mở trực tiếp Form TTDatPhong
            using (var f = new TTDatPhong())
            {
                // 1. Truyền Mã Phòng
                f.MaPhongCanDat = this.CurrentPhong.MaPhong;

                // 2. Truyền Ngày Mặc Định (Vì ở đây chưa chọn ngày)
                // Ta gán mặc định là Hôm nay nhận, Ngày mai trả
                f.NgayNhanDuKien = DateTime.Now;
                f.NgayTraDuKien = DateTime.Now.AddDays(1);

                f.ShowDialog();
            }

            // Hiện lại form cha sau khi đặt xong (hoặc hủy)
            if (parentForm != null && !parentForm.IsDisposed) parentForm.Show();
        }
    }
}