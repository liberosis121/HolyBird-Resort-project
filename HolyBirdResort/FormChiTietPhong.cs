using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using HolyBirdResort.DAO;
using HolyBirdResort.DTO;

namespace HolyBirdResort
{
    public partial class FormChiTietPhong : Form
    {
        public string MaPhongCanXem { get; set; }
        private List<Image> imageList = new List<Image>();
        private int currentImageIndex = 0;

        public FormChiTietPhong()
        {
            InitializeComponent();
        }

        private void FormChiTietPhong_Load(object sender, EventArgs e)
        {
            LoadImages();
            SetupDatePickerLogic();

            // Kiểm tra xem biến có nhận được dữ liệu không
            if (!string.IsNullOrEmpty(MaPhongCanXem))
            {
                HienThiThongTinPhong(MaPhongCanXem);
                KiemTraTinhTrangPhong();
            }
            else
            {
                // Nếu hiện thông báo này -> Lỗi do bên UC chưa truyền sang
                MessageBox.Show("Lỗi: Không nhận được Mã phòng cần xem!");
            }
        }

        // ---  KIỂM TRA TRẠNG THÁI DỰA TRÊN NGÀY CHỌN ---
        private void KiemTraTinhTrangPhong()
        {
            // 1. Kiểm tra logic ngày cơ bản
            if (pickerTGNhanPhong.Value >= pickerTGTraPhong.Value)
            {
                SetButtonState(false, "NGÀY KHÔNG HỢP LỆ");
                return;
            }

            // 2. Gọi DAO kiểm tra trong Database xem khoảng thời gian này có trống ko
            bool phongTrong = PhongDAO.CheckPhongTrong(MaPhongCanXem, pickerTGNhanPhong.Value, pickerTGTraPhong.Value);

            if (phongTrong)
            {
                SetButtonState(true, "ĐẶT PHÒNG NGAY");
            }
            else
            {
                SetButtonState(false, "ĐÃ CÓ LỊCH ĐẶT");
            }
        }

        // Hàm phụ trợ để bật/tắt nút cho gọn code
        private void SetButtonState(bool enable, string text)
        {
            btnDatPhong.Enabled = enable;
            btnDatPhong.Text = text;
            btnDatPhong.FillColor = enable ? Color.FromArgb(0, 118, 212) : Color.Gray;
        }

        private void SetupDatePickerLogic()
        {
            // --- FIX LỖI CRASH (FINAL) ---
            // DateTimePicker không nhận năm 0001 (DateTime.MinValue).
            // Giới hạn thấp nhất của nó là 1/1/1753.
            DateTime safeMinDate = new DateTime(1753, 1, 1);

            // Reset MinDate về mốc an toàn 1753
            pickerTGNhanPhong.MinDate = safeMinDate;
            pickerTGTraPhong.MinDate = safeMinDate;

            // Định dạng hiển thị
            pickerTGNhanPhong.Format = DateTimePickerFormat.Custom;
            pickerTGNhanPhong.CustomFormat = "dd/MM/yyyy HH:mm";
            pickerTGTraPhong.Format = DateTimePickerFormat.Custom;
            pickerTGTraPhong.CustomFormat = "dd/MM/yyyy HH:mm";

            // Gán giá trị mặc định là hiện tại
            DateTime now = DateTime.Now;

            // Lưu ý: Đôi khi giây/mili giây gây lệch, ta lấy Value hiện tại gán vào
            pickerTGNhanPhong.Value = now;
            pickerTGTraPhong.Value = now.AddDays(1);

            // Sau khi gán Value xong mới set MinDate thực tế để chặn quá khứ
            pickerTGNhanPhong.MinDate = now;
            pickerTGTraPhong.MinDate = now.AddHours(1); // Trả phòng ít nhất sau 1 tiếng

            // Gắn sự kiện (Cần kiểm tra để không gắn nhiều lần nếu load lại form)
            pickerTGNhanPhong.ValueChanged -= PickerTGNhanPhong_ValueChanged; // Gỡ trước cho chắc
            pickerTGNhanPhong.ValueChanged += PickerTGNhanPhong_ValueChanged;

            pickerTGTraPhong.ValueChanged -= PickerTGTraPhong_ValueChanged;
            pickerTGTraPhong.ValueChanged += PickerTGTraPhong_ValueChanged;
        }

        // --- CẬP NHẬT SỰ KIỆN KHI THAY ĐỔI NGÀY ---
        private void PickerTGNhanPhong_ValueChanged(object sender, EventArgs e)
        {
            DateTime ngayNhan = pickerTGNhanPhong.Value;
            DateTime ngayTraToiThieu = ngayNhan.AddDays(1);
            pickerTGTraPhong.MinDate = new DateTime(1753, 1, 1); // Reset tạm

            if (pickerTGTraPhong.Value <= ngayNhan)
            {
                pickerTGTraPhong.Value = ngayTraToiThieu;
            }
            pickerTGTraPhong.MinDate = ngayNhan.AddHours(1);

            // --- GỌI HÀM KIỂM TRA LẠI DB ---
            KiemTraTinhTrangPhong();
        }

        private void PickerTGTraPhong_ValueChanged(object sender, EventArgs e)
        {
            // Kiểm tra chặt chẽ: Ngày trả không được nhỏ hơn hoặc bằng ngày nhận
            if (pickerTGTraPhong.Value <= pickerTGNhanPhong.Value)
            {
                MessageBox.Show("Thời gian trả phòng phải sau thời gian nhận phòng!", "Lỗi chọn giờ");
                // Tự động sửa lại cho đúng
                pickerTGTraPhong.Value = pickerTGNhanPhong.Value.AddDays(1);
            }
            KiemTraTinhTrangPhong();
        }

        private void HienThiThongTinPhong(string maPhong)
        {
            Phong p = PhongDAO.GetPhongByMa(maPhong);

            if (p != null)
            {
                Control[] c;
                c = this.Controls.Find("lblMaPhong", true); if (c.Length > 0) c[0].Text = p.MaPhong;
                c = this.Controls.Find("lblTang", true); if (c.Length > 0) c[0].Text = p.MaTang;
                c = this.Controls.Find("lblLoaiPhong", true); if (c.Length > 0) c[0].Text = p.LoaiHang;
                c = this.Controls.Find("lblHinhThuc", true); if (c.Length > 0) c[0].Text = p.TenHinhThuc;
                c = this.Controls.Find("lblGia", true); if (c.Length > 0) c[0].Text = $"{p.GiaPhong:N0} kVNĐ";
                c = this.Controls.Find("lblTrangThai", true); if (c.Length > 0) c[0].Text = p.TrangThai;

                // --- LOGIC LẤY SỐ NGƯỜI TỪ DB ---
                c = this.Controls.Find("lblSoNguoi", true);
                if (c.Length > 0)
                {
                    // Lấy trực tiếp từ thuộc tính SoNguoiToiDa (đã Join bảng)
                    c[0].Text = p.SoNguoiToiDa.ToString() + " người";
                }

                // Xử lý trạng thái phòng
                if (p.TrangThai == "Đang có khách")
                {
                    btnDatPhong.Enabled = false;
                    btnDatPhong.Text = "ĐÃ CÓ KHÁCH";
                    btnDatPhong.FillColor = Color.Gray;
                    pickerTGNhanPhong.Enabled = false;
                    pickerTGTraPhong.Enabled = false;
                }
                else
                {
                    btnDatPhong.Enabled = true;
                    btnDatPhong.Text = "ĐẶT PHÒNG";
                    btnDatPhong.FillColor = Color.FromArgb(0, 118, 212); // Màu xanh chủ đạo
                    pickerTGNhanPhong.Enabled = true;
                    pickerTGTraPhong.Enabled = true;
                }
            }
        }

        // --- CÁC HÀM XỬ LÝ ẢNH GIỮ NGUYÊN ---
        private void LoadImages()
        {
            // Thêm ảnh mẫu (hoặc load từ DB nếu có)
            imageList.Add(Properties.Resources.P7_1);
            imageList.Add(Properties.Resources.P7_2);
            if (imageList.Count > 0) pbDisplayImage.Image = imageList[0];
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (imageList.Count == 0) return;
            currentImageIndex--;
            if (currentImageIndex < 0) currentImageIndex = imageList.Count - 1;
            pbDisplayImage.Image = imageList[currentImageIndex];
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (imageList.Count == 0) return;
            currentImageIndex++;
            if (currentImageIndex >= imageList.Count) currentImageIndex = 0;
            pbDisplayImage.Image = imageList[currentImageIndex];
        }

        private void pbQuayLai_Click(object sender, EventArgs e) => this.Close();
        private void ctrlExit_Click(object sender, EventArgs e) => Application.Exit();

        private void btnDatPhong_Click(object sender, EventArgs e)
        {
            // Validate cơ bản
            if (pickerTGNhanPhong.Value >= pickerTGTraPhong.Value)
            {
                MessageBox.Show("Ngày trả phải lớn hơn ngày nhận!"); return;
            }

            // Check lại lần cuối cho chắc ăn trước khi chuyển form
            if (btnDatPhong.Text == "ĐÃ CÓ LỊCH ĐẶT" || !btnDatPhong.Enabled)
            {
                MessageBox.Show("Phòng không trống trong thời gian này!");
                return;
            }

            this.Hide();
            using (var f = new TTDatPhong())
            {
                // 1. TRUYỀN MÃ PHÒNG VÀ NGÀY GIỜ ĐÃ CHỌN SANG
                f.MaPhongCanDat = this.MaPhongCanXem;
                f.NgayNhanDuKien = pickerTGNhanPhong.Value;
                f.NgayTraDuKien = pickerTGTraPhong.Value;

                f.ShowDialog();
            }
            this.Show();
        }
    }
}