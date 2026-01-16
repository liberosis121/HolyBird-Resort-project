using HolyBirdResort.DAO;
using HolyBirdResort.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace HolyBirdResort
{

    public partial class FormTtinTK : Form
    {
        public static int SelectedTabIndex = 0;

        public FormTtinTK()
        {
            InitializeComponent();
        }
        private void FormTtinTK_Load(object sender, EventArgs e)
        {
            tabControl.SelectedIndex = SelectedTabIndex;

            if (AuthSession.CurrentUser == null)
            {
                MessageBox.Show("Chưa đăng nhập!", "Lỗi");
                this.Close();
                return;
            }

            // Load dữ liệu
            LoadGuestInformation(AuthSession.CurrentUser.MaDoan);
            LoadDanhSachPhongDaDat();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == tabPage2) 
            {
                LoadDanhSachPhongDaDat();
            }
        }
        private void LoadDanhSachPhongDaDat()
        {
            // 1. Dọn sạch panel cũ
            pnlDanhSachPhong.Controls.Clear();

            // 2. Lấy mã đoàn hiện tại
            string maDoan = "";
            if (AuthSession.CurrentUser != null)
                maDoan = AuthSession.CurrentUser.MaDoan;
            else
                maDoan = "D001"; // Fallback test

            // 3. Lấy danh sách các phòng đoàn này đang đặt
            DataTable dtPhong = GiaoDichDAO.GetDanhSachPhongDaDat(maDoan);

            // 4. Duyệt qua từng phòng và tạo UserControl
            foreach (DataRow row in dtPhong.Rows)
            {
                string maPhong = row["MaPhong"].ToString();
                DateTime ngayNhan = Convert.ToDateTime(row["NgayNhan"]);
                DateTime ngayTra = Convert.ToDateTime(row["NgayTra"]);

                // Tạo UserControl mới
                ucRoomBooking uc = new ucRoomBooking();

                // Set Width cho nó rộng bằng Panel cha để đẹp (trừ đi thanh cuộn)
                //int scrollBarWidth = SystemInformation.VerticalScrollBarWidth;
                //uc.Width = pnlDanhSachPhong.ClientSize.Width - scrollBarWidth - 5;

                // Đổ dữ liệu vào UC
                uc.SetBookingData(maDoan, maPhong, ngayNhan, ngayTra);

                // Thêm vào Panel
                pnlDanhSachPhong.Controls.Add(uc);
            }
        }
        public void LoadGuestInformation(string maDoan)
        {
            // 1. LẤY DANH SÁCH THÀNH VIÊN TỪ DB
            List<KhachHang> danhSachTV = KhachHangDAO.GetListThanhVien(maDoan);

            // 2. LẤY THÔNG TIN CHUNG (Trưởng đoàn, Ngày giờ)
            string tenTruongDoan = KhachHangDAO.GetTenTruongDoan(maDoan);
            GiaoDich chuyenDi = GiaoDichDAO.GetThongTinChuyenDi(maDoan);

            // 3. HIỂN THỊ LÊN GIAO DIỆN
            lblMaDoanValue.Text = maDoan;
            lblSoThanhVienValue.Text = danhSachTV.Count.ToString();
            lblTruongDoanValue.Text = tenTruongDoan;

            // Bổ sung hiển thị Ngày đến - Ngày đi
            if (chuyenDi != null)
            {
                // Giả sử bạn đã thêm 2 Label này trên Design: lblNgayDenValue, lblNgayDiValue
                // Nếu chưa có, hãy thêm vào Form Design nhé!
                lblNgayDenValue.Text = chuyenDi.ThoiGianBatDau.ToString("dd/MM/yyyy HH:mm");
                lblNgayDiValue.Text = chuyenDi.ThoiGianKetThuc.ToString("dd/MM/yyyy HH:mm");
            }

            // 4. ĐỔ DỮ LIỆU VÀO GRID
            dgvMembers.DataSource = null;
            dgvMembers.DataSource = danhSachTV;

            if (danhSachTV == null || !danhSachTV.Any())
            {
                MessageBox.Show("Không có dữ liệu thành viên nào được nạp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (dgvMembers.Columns["MaDoan"] != null)
            {
                dgvMembers.Columns["MaDoan"].Visible = false;
            }

            // --- 2. ĐẶT TÊN CỘT TIẾNG VIỆT (HEADER TEXT) ---
            // Kiểm tra và đổi tên hiển thị
            if (dgvMembers.Columns["MaKH"] != null)
            {
                dgvMembers.Columns["MaKH"].HeaderText = "Mã Khách Hàng";
                dgvMembers.Columns["MaKH"].ReadOnly = true; // Cấm sửa mã
            }
            if (dgvMembers.Columns["SoCMND"] != null) dgvMembers.Columns["SoCMND"].HeaderText = "Số CMND";
            if (dgvMembers.Columns["TenKH"] != null) dgvMembers.Columns["TenKH"].HeaderText = "Họ và Tên";
            if (dgvMembers.Columns["NgaySinh"] != null) dgvMembers.Columns["NgaySinh"].HeaderText = "Ngày Sinh";
            if (dgvMembers.Columns["SDT"] != null) dgvMembers.Columns["SDT"].HeaderText = "Số Điện Thoại";

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

            dgvMembers.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

            // Đặt chiều cao hàng mong muốn (Ví dụ: 38px để tạo khoảng trống)
            dgvMembers.RowTemplate.Height = 50;

            // Tắt tự động điều chỉnh chiều cao cột (nếu chưa làm)
            dgvMembers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Nền mặc định của Grid (Backcolor chung của control)
            dgvMembers.BackgroundColor = System.Drawing.Color.FromArgb(245, 248, 252);
            // Màu đường lưới (GridColor)
            dgvMembers.GridColor = System.Drawing.Color.White;
            // Cho phép người dùng chỉnh sửa 
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

            // Format ngày sinh
            if (dgvMembers.Columns["NgaySinh"] != null)
                dgvMembers.Columns["NgaySinh"].DefaultCellStyle.Format = "dd/MM/yyyy";
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            // Hiển thị hộp thoại xác nhận
            DialogResult result = MessageBox.Show(
                "Bạn có muốn đăng xuất khỏi tài khoản?",
                "Xác nhận đăng xuất",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question
            );

            // Nếu người dùng chọn OK, chuyển về FormDangNhap
            if (result == DialogResult.OK)
            {
                AuthSession.Clear(); // Xóa session
                this.Hide(); // Ẩn FormTtinTK
                using (var formDangNhap = new FormDangNhap())
                {
                    formDangNhap.ShowDialog();
                }
                this.Close(); // Đóng FormTtinTK sau khi đăng xuất
            }
        }

        private void pbQuayLai_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ctrlExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLuuThayDoi_Click(object sender, EventArgs e)
        {
            // Xác nhận
            if (MessageBox.Show("Xác nhận cập nhật thông tin? (Hệ thống sẽ xử lý trong 15s)", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            int countSuccess = 0;
            string currentMaDoan = AuthSession.CurrentUser.MaDoan;

            foreach (DataGridViewRow row in dgvMembers.Rows)
            {
                if (row.IsNewRow) continue;

                try
                {
                    // *** FIX LỖI CỘT TẠI ĐÂY ***
                    // Kiểm tra xem DGV đang dùng tên cột là gì. 
                    // Nếu bạn dùng LoadGuestInformation như bài trước, tên cột là tên thuộc tính DTO.

                    // Lấy giá trị an toàn bằng cách dùng try-catch hoặc kiểm tra cột
                    int maKH = Convert.ToInt32(row.Cells["MaKH"].Value); // Dùng tên Property của DTO

                    // Nếu code trên vẫn lỗi, hãy thử dùng index: row.Cells[0].Value (nhưng không khuyến khích)
                    // Hoặc kiểm tra lại tên cột trong Design. 
                    // Ở đây mình giả định bạn đã map DataPropertyName = "MaKH" => Tên cột trong Code cũng là "MaKH"

                    string tenKH = row.Cells["TenKH"].Value?.ToString().Trim();
                    string cccd = row.Cells["SoCMND"].Value?.ToString().Trim();
                    string sdt = row.Cells["SDT"].Value?.ToString().Trim();

                    DateTime ngaySinh;
                    if (!DateTime.TryParse(row.Cells["NgaySinh"].Value?.ToString(), out ngaySinh))
                    {
                        MessageBox.Show($"Ngày sinh dòng {row.Index + 1} lỗi!", "Lỗi"); return;
                    }

                    // Tạo DTO
                    KhachHang kh = new KhachHang
                    {
                        MaDoan = currentMaDoan,
                        MaKH = maKH,
                        TenKH = tenKH,
                        SoCMND = cccd,
                        NgaySinh = ngaySinh,
                        SDT = sdt
                    };

                    // Gọi DAO (Sẽ chạy SP có delay 15s)
                    if (KhachHangDAO.UpdateThongTinKhachHang(kh))
                    {
                        countSuccess++;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi dòng {row.Index + 1}: {ex.Message}");
                    // Nếu lỗi "Column cannot be found", hãy debug xem row.Cells có những cột tên gì
                    return;
                }
            }

            if (countSuccess > 0)
            {
                MessageBox.Show($"Cập nhật thành công!", "Thông báo");
                LoadGuestInformation(currentMaDoan);
            }
        }
    }
}
