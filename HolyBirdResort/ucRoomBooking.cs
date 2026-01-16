using HolyBirdResort.DAO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static HolyBirdResort.FormTtinTK;

namespace HolyBirdResort
{
   
    public partial class ucRoomBooking : UserControl
    {
        // Lưu mã đoàn để dùng cho các nút chức năng (Trả phòng, Hủy)
        private string _currentMaDoan;
        private string _currentMaPhong;

        public ucRoomBooking()
        {
            InitializeComponent();
        }

        private void ucRoomBooking_Load(object sender, EventArgs e)
        {
            // Vì dữ liệu được nạp qua hàm SetBookingData nên Load có thể để trống
            // Hoặc gọi lại Style để đảm bảo giao diện đẹp
            ApplyDataGridViewStyles();
        }

        // Trong ucRoomBooking.cs

        // Lưu biến thời gian nhận để so sánh
        private DateTime _ngayNhanPhong;

        public void SetBookingData(string maDoan, string maPhong, DateTime checkIn, DateTime checkOut)
        {
            _currentMaDoan = maDoan;
            _currentMaPhong = maPhong;
            _ngayNhanPhong = checkIn; // Lưu lại ngày nhận từ DB

            lblMaPhongvalue.Text = maPhong;
            lblCheckinValue.Text = checkIn.ToString("dd/MM/yyyy HH:mm");
            lblCheckoutValue.Text = checkOut.ToString("dd/MM/yyyy HH:mm");

            LoadRealGuestList();
            UpdateButtonState(); // Hàm điều khiển bật/tắt nút
        }

        private void UpdateButtonState()
        {
            DataTable dt = GiaoDichDAO.GetKhachHangTrongPhong(_currentMaDoan, _currentMaPhong);
            if (dt == null || dt.Rows.Count == 0) return;

            // Lấy trạng thái từ cột TrangThai (đảm bảo hàm DAO đã SELECT cột này)
            string trangThai = dt.Rows[0]["TrangThai"].ToString();

            // MẶC ĐỊNH ẨN CÁC NÚT ĐỂ TRÁNH CHỒNG CHÉO
            btnNhanPhong.Visible = false;
            btnCheckOut.Visible = false;

            if (trangThai == "Chưa nhận phòng" || trangThai == "Đã đặt")
            {
                btnNhanPhong.Visible = true;
                btnNhanPhong.Text = "NHẬN PHÒNG";
                // Logic kiểm tra giờ nhận
                btnNhanPhong.Enabled = (DateTime.Now >= _ngayNhanPhong);
                btnNhanPhong.FillColor = btnNhanPhong.Enabled ? Color.FromArgb(86, 145, 73) : Color.Gray;
            }
            else if (trangThai == "Yêu cầu trả phòng")
            {
                btnCheckOut.Visible = true;
                btnCheckOut.Text = "CHỜ XÁC NHẬN";
                btnCheckOut.Enabled = false;
                btnCheckOut.FillColor = Color.Orange;
            }
            else if (trangThai == "Đã trả phòng") // Khớp với trạng thái sau khi NV xác nhận
            {
                btnCheckOut.Visible = true;
                btnCheckOut.Text = "THANH TOÁN";
                btnCheckOut.Enabled = true;
                btnCheckOut.FillColor = Color.SeaGreen;
            }
            else if (trangThai == "Chưa trả phòng")
            {
                btnCheckOut.Visible = true;
                btnCheckOut.Text = "TRẢ PHÒNG";
                btnCheckOut.Enabled = true;
                btnCheckOut.FillColor = Color.FromArgb(0, 118, 212);
            }
        }

        // Sự kiện click nút Nhận Phòng
        private void btnNhanPhong_Click(object sender, EventArgs e)
        {
            if (GiaoDichDAO.NhanPhong(_currentMaDoan, _currentMaPhong))
            {
                MessageBox.Show("Nhận phòng thành công!");
                LoadRealGuestList(); // Load lại Grid để cập nhật cột TrangThai trên UI
                UpdateButtonState(); // Chuyển sang chế độ Trả phòng
            }
        }

        private void LoadRealGuestList()
        {
            // Gọi DAO lấy DataTable
            DataTable dtGuests = GiaoDichDAO.GetKhachHangTrongPhong(_currentMaDoan, _currentMaPhong);

            // Gán vào Grid
            dgvMembers.DataSource = dtGuests;

            // --- Cấu hình cột hiển thị ---
            // Mapping tên cột từ SQL (Mã Khách Hàng, Họ và tên) vào DataPropertyName
            if (dgvMembers.Columns.Contains("Mã Khách Hàng"))
                dgvMembers.Columns["Mã Khách Hàng"].DataPropertyName = "Mã Khách Hàng";

            if (dgvMembers.Columns.Contains("Họ và tên"))
                dgvMembers.Columns["Họ và tên"].DataPropertyName = "Họ và tên";

            // Thêm nút Hủy nếu chưa có
            if (!dgvMembers.Columns.Contains("btnHuyPhong"))
            {
                DataGridViewButtonColumn btnColHuy = new DataGridViewButtonColumn();
                btnColHuy.HeaderText = "Hủy";
                btnColHuy.Name = "btnHuyPhong";
                btnColHuy.Text = "❌";
                btnColHuy.UseColumnTextForButtonValue = true;
                btnColHuy.Width = 50;
                dgvMembers.Columns.Add(btnColHuy);
            }

            // Gọi hàm style giao diện cũ của bạn
            ApplyDataGridViewStyles();
        }

        public void ApplyDataGridViewStyles()
        {
            // Đảm bảo DataGridView tồn tại và đã có nguồn dữ liệu
            if (dgvMembers == null || dgvMembers.DataSource == null)
            {
                return;
            }

            // --- 1. THIẾT LẬP CHUNG (ThemeStyle) ---

            // --- 1. KHÓA CHỈ ĐỌC Ở MỨC GRID ---
            dgvMembers.ReadOnly = true;
            dgvMembers.AllowUserToAddRows = false;
            dgvMembers.AllowUserToDeleteRows = false;
            dgvMembers.AllowUserToResizeRows = false;

            // Ngăn việc nhấn vào là hiện con trỏ soạn thảo
            dgvMembers.EditMode = DataGridViewEditMode.EditProgrammatically;
            // Đặt chiều cao hàng mong muốn (Ví dụ: 38px để tạo khoảng trống)
            dgvMembers.RowTemplate.Height = 50;

            // Tắt tự động điều chỉnh chiều cao cột (nếu chưa làm)
            dgvMembers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Nền mặc định của Grid (Backcolor chung của control)
            dgvMembers.BackgroundColor = System.Drawing.Color.FromArgb(245, 248, 252);
            // Màu đường lưới (GridColor)
            dgvMembers.GridColor = System.Drawing.Color.White;
            // Cho phép người dùng chỉnh sửa (theo yêu cầu trước đó)
            dgvMembers.ReadOnly = false;

            // --- 2. THIẾT LẬP ALTERNATING ROWS (MÀU HÀNG CHẴN/LẺ) ---

            // Áp dụng AlternatingRowsStyle (Chỉ áp dụng nếu bạn muốn hàng lẻ/chẵn khác màu)
            // Dựa trên ảnh: AlternatingRowsStyle.BackColor = Transparent
            // Tuy nhiên, để tạo sọc (stripe), ta thường dùng màu nhạt hơn (như màu Background chung)

            // Lấy đối tượng Style cho hàng xen kẽ
            DataGridViewCellStyle alternatingStyle = dgvMembers.AlternatingRowsDefaultCellStyle;
            alternatingStyle.BackColor = System.Drawing.Color.FromArgb(230, 248, 250); // Hoặc màu trắng nếu muốn nền sạch
            alternatingStyle.ForeColor = System.Drawing.Color.Black;
            alternatingStyle.Font = new Font("Microsoft Sans Serif", 10.2f);
            // (Lưu ý: Bạn phải thêm System.Drawing vào using directive nếu chưa có)

            // --- 3. THIẾT LẬP ROWS STYLE (MÀU CÁC HÀNG DỮ LIỆU) ---

            // RowsStyle không được hiển thị chi tiết, nhưng ta dùng giá trị BackColor chung
            DataGridViewCellStyle rowsStyle = dgvMembers.RowsDefaultCellStyle;
            rowsStyle.BackColor = System.Drawing.Color.FromArgb(245, 248, 252); // Nền hàng
            rowsStyle.ForeColor = System.Drawing.Color.Black;
            rowsStyle.SelectionBackColor = System.Drawing.Color.LightBlue; // Màu nền khi chọn
            rowsStyle.SelectionForeColor = System.Drawing.Color.MidnightBlue; // Màu chữ khi chọn
            rowsStyle.Font = new Font("Microsoft Sans Serif", 10.2f);

            // --- 4. THIẾT LẬP HEADER STYLE (TIÊU ĐỀ CỘT) ---

            // Lấy đối tượng Style cho Header
            DataGridViewCellStyle headerStyle = dgvMembers.ColumnHeadersDefaultCellStyle;

            // BackColor (Màu nền Header): 20, 98, 180 (Xanh đậm)
            headerStyle.BackColor = System.Drawing.Color.FromArgb(20, 98, 180);

            // ForeColor (Màu chữ Header): White
            headerStyle.ForeColor = System.Drawing.Color.White;

            // Font Header: Microsoft Sans Serif, 10.2pt, Bold
            headerStyle.Font = new Font("Microsoft Sans Serif", 11.2f, FontStyle.Bold);

            // Độ cao Header
            dgvMembers.ColumnHeadersHeight = 50;
            // Không cho phép người dùng chỉnh sửa độ cao Header (EnableResizing)
            dgvMembers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // --- ÁP DỤNG CÁC THAY ĐỔI ---
            dgvMembers.EnableHeadersVisualStyles = false; // Tắt style mặc định của Windows
            dgvMembers.ClearSelection(); // Xóa lựa chọn ban đầu
            dgvMembers.Refresh(); // Làm mới DataGridView
        }
        private void dgvMembers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvMembers.Columns[e.ColumnIndex].Name == "btnHuyPhong")
            {
                // SỬA TẠI ĐÂY: Dùng "Mã Khách Hàng" thay vì "MaKhachHang"
                string maKH = dgvMembers.Rows[e.RowIndex].Cells["Mã Khách Hàng"].Value.ToString();
                string tenKH = dgvMembers.Rows[e.RowIndex].Cells["Họ và tên"].Value.ToString();
                string soPhong = lblMaPhongvalue.Text;
                int tongSoCTGD = GiaoDichDAO.DemTongSoCTGDActiveCuaDoan(this._currentMaDoan);
                // Nếu tổng số bản ghi trong DB là 1, thì chắc chắn khách này là người cuối cùng
                bool laKhachCuoi = (tongSoCTGD == 1);

                var parentForm = this.FindForm();
                if (parentForm != null) parentForm.Hide();

                using (var dlg = new HuyPhong(maKH, tenKH, soPhong, laKhachCuoi))
                {
                    // Quan trọng: Truyền mã đoàn hiện tại sang form hủy
                    dlg.CurrentMaDoan = this._currentMaDoan;

                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        LoadRealGuestList();

                        if (dgvMembers.Rows.Count == 0)
                        {
                            if (this.Parent != null) this.Parent.Controls.Remove(this);
                            this.Dispose();
                        }
                    }
                }

                if (parentForm != null && !parentForm.IsDisposed) parentForm.Show();
            }
        }

        private void btnCheckOut_Click(object sender, EventArgs e)
        {
            if (btnCheckOut.Text.ToUpper() == "TRẢ PHÒNG")
            {
                if (GiaoDichDAO.YeuCauTraPhong(_currentMaDoan, _currentMaPhong))
                {
                    MessageBox.Show("Đã gửi yêu cầu trả phòng. Vui lòng chờ nhân viên kiểm tra.");
                    UpdateButtonState();
                }
            }
            else if (btnCheckOut.Text == "THANH TOÁN")
            {
                // Mở Form TraPhong
                using (var dlg = new TraPhong(_currentMaDoan, _currentMaPhong))
                {
                    // Nếu người dùng nhấn xác nhận thanh toán ở Form con cuối cùng và trả về OK
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        // THỰC HIỆN LOGIC GIAO DIỆN TẠI ĐÂY (Không cần DB)
                        btnCheckOut.Text = "ĐÃ THANH TOÁN";
                        btnCheckOut.Enabled = false; // Disable nút theo yêu cầu
                        btnCheckOut.FillColor = Color.Gray; // Đổi màu sang xám để nhìn rõ là đã xong

                        // Vô hiệu hóa luôn cả Grid khách hàng nếu cần
                        dgvMembers.Enabled = false;
                    }
                }
            }
        }

    }
}
