using HolyBirdResort.DAO;
using HolyBirdResort.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HolyBirdResort
{
    public partial class FormTimKiemPhong : Form
    {
        // Biến lưu trữ toàn bộ danh sách phòng (Cache) để lọc không cần gọi lại DB
        private List<Phong> _allRooms = new List<Phong>();

        public FormTimKiemPhong()
        {
            InitializeComponent();
        }

        // Sự kiện Load Form
        // Thêm từ khóa async
        private async void FormTimKiemPhong_Load(object sender, EventArgs e)
        {
            LoadComboBoxData(); // Cái này nhẹ, để chạy thường cũng được

            // Hiện thông báo "Đang tải..." nếu cần
            if (lblKetQua != null) lblKetQua.Text = "Đang tải dữ liệu...";

            // --- CHẠY DB Ở LUỒNG PHỤ ---
            // Form vẫn mượt, xoay vòng tròn chờ đợi
            _allRooms = await Task.Run(() => PhongDAO.GetAllPhong());

            // Sau khi lấy xong data thì mới Lọc và Hiện
            FilterRooms(null, null);
        }

        // 1. Load dữ liệu vào các ComboBox
        private void LoadComboBoxData()
        {
            // --- Hạng Phòng ---
            DataTable dtHang = PhongDAO.GetHangPhong();
            // Thêm dòng "Tất cả"
            DataRow drHang = dtHang.NewRow();
            drHang["LoaiHang"] = "Tất cả";
            dtHang.Rows.InsertAt(drHang, 0);

            cboHangPhong.DataSource = dtHang;
            cboHangPhong.DisplayMember = "LoaiHang";
            cboHangPhong.ValueMember = "LoaiHang";
            cboHangPhong.SelectedIndex = 0;

            // --- Hình Thức ---
            DataTable dtHinhThuc = PhongDAO.GetHinhThuc();
            DataRow drHT = dtHinhThuc.NewRow();
            drHT["TenHinhThuc"] = "Tất cả";
            dtHinhThuc.Rows.InsertAt(drHT, 0);

            cboHinhThuc.DataSource = dtHinhThuc;
            cboHinhThuc.DisplayMember = "TenHinhThuc";
            cboHinhThuc.ValueMember = "TenHinhThuc";
            cboHinhThuc.SelectedIndex = 0;

            // --- Trạng Thái (Nếu bạn đã thêm ComboBox này) ---
            if (cboTrangThai != null) // Kiểm tra null phòng trường hợp chưa tạo
            {
                cboTrangThai.Items.Clear();
                cboTrangThai.Items.Add("Tất cả trạng thái");
                cboTrangThai.Items.Add("Đang trống");
                cboTrangThai.SelectedIndex = 0;
            }

            // Đăng ký sự kiện (Nếu chưa làm trong Designer)
            // Khi thay đổi giá trị combo hoặc text -> Gọi hàm Lọc
            cboHangPhong.SelectedIndexChanged += FilterRooms;
            cboHinhThuc.SelectedIndexChanged += FilterRooms;
            if (cboTrangThai != null) cboTrangThai.SelectedIndexChanged += FilterRooms;
            txtTimKiem.TextChanged += FilterRooms;
        }

        // 3. Hàm Lọc Trung Tâm (Core Logic)
        private void FilterRooms(object sender, EventArgs e)
        {
            // Bắt đầu từ danh sách gốc
            var result = _allRooms.AsEnumerable();

            // a. Lọc theo Từ khóa (Mã phòng)
            string keyword = txtTimKiem.Text.ToLower().Trim();
            if (!string.IsNullOrEmpty(keyword))
            {
                result = result.Where(p => p.MaPhong.ToLower().Contains(keyword));
            }

            // b. Lọc theo Hạng Phòng
            if (cboHangPhong.SelectedIndex > 0 && cboHangPhong.SelectedValue != null)
            {
                string selectedHang = cboHangPhong.SelectedValue.ToString();
                result = result.Where(p => p.LoaiHang == selectedHang);
            }

            // c. Lọc theo Hình Thức
            if (cboHinhThuc.SelectedIndex > 0 && cboHinhThuc.SelectedValue != null)
            {
                string selectedHT = cboHinhThuc.SelectedValue.ToString();
                result = result.Where(p => p.TenHinhThuc == selectedHT);
            }

            // d. Lọc theo Trạng Thái
            if (cboTrangThai != null && cboTrangThai.SelectedIndex > 0)
            {
                string status = cboTrangThai.SelectedItem.ToString(); // "Trống" hoặc "Đã đặt"
                // So sánh chuỗi (Lưu ý trong DB bạn lưu là "Trống" hay "Trong" để so sánh cho đúng)
                result = result.Where(p => p.TrangThai.Equals(status, StringComparison.OrdinalIgnoreCase));
            }

            // 4. Hiển thị kết quả
            RenderRoomList(result.ToList());
        }

        // 4. Render danh sách ra giao diện (Tạo UserControl động)
        private void RenderRoomList(List<Phong> roomList)
        {
            // BƯỚC 1: Dọn dẹp nhà cửa
            // Vì mỗi lần lọc số lượng khác nhau, ta phải xóa sạch danh sách cũ đi
            flpDanhSachPhong.Controls.Clear();

            // Cập nhật Label số lượng kết quả
            if (lblKetQua != null) lblKetQua.Text = $"Tìm thấy {roomList.Count} phòng";

            // Tối ưu: Tạm ngưng vẽ giao diện để nạp cho nhanh, đỡ bị nháy hình
            flpDanhSachPhong.SuspendLayout();

            // BƯỚC 2: Vòng lặp thần thánh
            // Duyệt qua từng phòng trong danh sách tìm được từ Database
            foreach (var room in roomList)
            {
                // a. Tự động tạo ra 1 cái UserControl mới toanh (trong bộ nhớ)
                ucRoomDescription uc = new ucRoomDescription();

                // b. Đổ dữ liệu của phòng đó vào UC này
                // (Hàm SetRoomData này bạn đã viết bên ucRoomDescription.cs ở bước trước)
                uc.SetRoomData(room);

                // c. Quan trọng: Đặt kích thước margin để các UC không dính sát vào nhau
                uc.Margin = new Padding(10);

                // d. GẮN VÀO FORM: Câu lệnh này sẽ đưa UC từ bộ nhớ hiển thị lên màn hình
                // FlowLayoutPanel sẽ tự động xếp nó vào vị trí tiếp theo
                flpDanhSachPhong.Controls.Add(uc);
            }

            // Cho phép vẽ lại giao diện sau khi đã nạp xong hết
            flpDanhSachPhong.ResumeLayout();
        }

        private void pbQuayLai_Click(object sender, EventArgs e) => this.Close();
        private void ctrlExit_Click(object sender, EventArgs e) => Application.Exit();

        private async void btnDemoErr11_Click(object sender, EventArgs e)
        {
            // Lấy hạng phòng đang chọn trên ComboBox
            if (cboHangPhong.SelectedValue == null || cboHangPhong.SelectedIndex == 0)
            {
                MessageBox.Show("Vui lòng chọn 1 hạng phòng cụ thể (VD: VIP) để test!");
                return;
            }
            string loaiHang = cboHangPhong.SelectedValue.ToString();

            // false -> chạy bản không fix lỗi, true -> chạy bản đã fix lỗi
            bool isFix = true; 

            MessageBox.Show($"NV1: Bắt đầu tìm kiếm phòng hạng {loaiHang}...\n(Hệ thống sẽ treo 10s, hãy nhanh tay bấm nút T2)", "T1 Start");

            try
            {
                // Gọi hàm bất đồng bộ để không đơ UI
                string ketQua = await Task.Run(() => PhongDAO.Demo_TraCuuPhong_T1(loaiHang, isFix));

                // Hiện kết quả so sánh
                MessageBox.Show(ketQua, "Kết quả Demo Phantom Read");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }
    }
}