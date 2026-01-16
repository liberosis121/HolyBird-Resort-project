using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using HolyBirdResort.DAO;
using HolyBirdResort.DTO;

namespace HolyBirdResort
{
    public partial class GhiNhanTraPhong : Form
    {
        // Biến toàn cục lưu Mã CTGD đang chọn (để truyền sang Form con)
        private string _maCTGD_DangChon = "";

        public GhiNhanTraPhong()
        {
            InitializeComponent();
            this.Load += DanhSachTraPhong_Load;
        }

        private void Exit_Click(object sender, EventArgs e) => Application.Exit();
        private void QuayLai_Click(object sender, EventArgs e) => this.Close();

        private void DanhSachTraPhong_Load(object sender, EventArgs e)
        {
            LoadDataGrid();
            LoadComboBoxBoiThuong();
            FormBoiThuong.Visible = false;
        }

        // ==========================================
        // 1. LOAD DỮ LIỆU (KỸ THUẬT LƯU OBJECT VÀO TAG)
        // ==========================================
        // File: GhiNhanTraPhong.cs

        private void LoadDataGrid()
        {
            dgvTraPhong.Rows.Clear();
            List<CTGD> list = TraPhongDAO.GetListChoTraPhong();

            string previousMaPhong = "";
            bool toggleColor = false; // Biến để đổi màu nền xen kẽ giữa các phòng

            foreach (var item in list)
            {
                // Kiểm tra xem dòng này có phải là dòng đầu tiên của Phòng mới không
                bool isNewRoom = (item.MaPhong != previousMaPhong);

                if (isNewRoom)
                {
                    toggleColor = !toggleColor; // Đổi màu khi sang phòng mới
                }

                // Quyết định có hiện nút "$" hay không?
                // Chỉ hiện nút "$" ở dòng đầu tiên của mỗi phòng
                string textNutXacNhan = isNewRoom ? "✅" : "";

                int index = dgvTraPhong.Rows.Add(
                    item.MaPhong,
                    item.MaDoan,
                    item.MaKH,
                    item.TenKH,
                    "$",            // Nút Bồi thường: Dòng nào cũng có
                    "+",
                    textNutXacNhan, // Nút Xác nhận: Chỉ dòng đầu có chữ
                    item.TrangThai
                );

                // Lưu Object vào Tag
                dgvTraPhong.Rows[index].Tag = item;

                // 1. Tô màu nền theo nhóm phòng để dễ nhìn
                dgvTraPhong.Rows[index].DefaultCellStyle.BackColor = toggleColor ? Color.White : Color.LightCyan;

                // 2. Nếu không phải dòng đầu, ta làm cho nút bấm trông như "biến mất" (dù ô đó vẫn click được nếu k chặn)
                if (!isNewRoom)
                {
                    // Tùy chọn: Có thể set style ô này thành readonly hoặc đổi màu cho tiệp màu nền
                    // Ở đây mình để text rỗng ("") là người dùng hiểu không bấm được rồi.
                }

                // Cập nhật mã phòng hiện tại
                previousMaPhong = item.MaPhong;
            }
        }

        private void LoadComboBoxBoiThuong()
        {
            cboBoiThuong.DataSource = TraPhongDAO.GetListBoiThuong();
            cboBoiThuong.DisplayMember = "HienThi";
            cboBoiThuong.ValueMember = "MaBoiThuong";
        }

        // ==========================================
        // 2. XỬ LÝ CLICK TRÊN LƯỚI
        // ==========================================
        private void XacNhanBoiThuong_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvTraPhong.Rows[e.RowIndex];
            if (!(row.Tag is CTGD data)) return;

            string colName = dgvTraPhong.Columns[e.ColumnIndex].Name;

            // ---  NÚT BỒI THƯỜNG (+) 
            if (colName == "BoiThuong" || dgvTraPhong.Columns[e.ColumnIndex].HeaderText == "+")
            {
                if (data.TrangThai != "Yêu cầu trả phòng")
                {
                    MessageBox.Show("Chỉ ghi nhận bồi thường khi khách đang yêu cầu trả phòng!", "Cảnh báo");
                    return;
                }

                _maCTGD_DangChon = data.MaCTGD;
                cboBoiThuong.SelectedIndex = -1;
                numSoLuong.Value = 1;
                FormBoiThuong.Visible = true;
                FormBoiThuong.BringToFront();
            }

            // ---  NÚT XEM CHI TIẾT () ---
            if (colName == "CTBT" || dgvTraPhong.Columns[e.ColumnIndex].HeaderText == "$")
            {
                string noiDung = TraPhongDAO.GetDanhSachBoiThuong(data.MaCTGD);
                MessageBox.Show($"Danh sách bồi thường của {data.TenKH}:\n\n" + noiDung,
                                "Chi tiết bồi thường", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // --- NÚT XÁC NHẬN ()
            if (colName == "XacNhan" || dgvTraPhong.Columns[e.ColumnIndex].HeaderText.Contains("Xác nhận"))
            {
                // Chỉ bấm được nếu ô đó có chữ (là dòng đầu tiên của phòng)
                string buttonText = row.Cells[e.ColumnIndex].Value.ToString();
                if (string.IsNullOrEmpty(buttonText)) return; // Bấm vào ô trống của người thứ 2,3 thì bỏ qua

                // Xác nhận
                if (MessageBox.Show($"Bạn muốn xác nhận trả phòng cho TOÀN BỘ PHÒNG {data.MaPhong}?",
                    "Xác nhận trả phòng", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // Gọi hàm xử lý theo Phòng + Đoàn
                    if (TraPhongDAO.XacNhanTraPhongCaPhong(data.MaPhong, data.MaDoan))
                    {
                        MessageBox.Show($"Đã trả phòng {data.MaPhong} thành công!\nCác khách hàng có thể thanh toán bên hóa đơn.");
                        LoadDataGrid(); // Load lại để ẩn nhóm phòng này đi
                    }
                    else
                    {
                        MessageBox.Show("Lỗi: Không tìm thấy yêu cầu trả phòng hợp lệ cho phòng này.", "Lỗi");
                    }
                }
            }
        }

        // ==========================================
        // 3. NÚT LƯU BỒI THƯỜNG
        // ==========================================
        private void GhiNhanBoiThuong_Click(object sender, EventArgs e)
        {
            // Validate dữ liệu
            if (cboBoiThuong.SelectedValue == null)
            {
                MessageBox.Show("Chưa chọn loại bồi thường!", "Cảnh báo"); return;
            }
            if (numSoLuong.Value <= 0)
            {
                MessageBox.Show("Số lượng phải > 0!", "Cảnh báo"); return;
            }
            if (string.IsNullOrEmpty(_maCTGD_DangChon))
            {
                MessageBox.Show("Lỗi: Mất mã giao dịch. Vui lòng chọn lại khách hàng.", "Lỗi"); return;
            }

            CTBoiThuong ctbt = new CTBoiThuong
            {
                MaCTGD = _maCTGD_DangChon,
                MaBoiThuong = cboBoiThuong.SelectedValue.ToString(),
                SoLuong = (int)numSoLuong.Value
            };

            try
            {
                // Gọi DAO (DAO sẽ gọi SP Demo Phantom)
                if (TraPhongDAO.ThemCTBoiThuong(ctbt))
                {
                    MessageBox.Show("Ghi nhận bồi thường thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    FormBoiThuong.Visible = false;
                }
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi từ Database (ví dụ: Đã tồn tại bồi thường này...)
                MessageBox.Show("Lỗi Database: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ThoatBoiThuong_Click(object sender, EventArgs e) => FormBoiThuong.Visible = false;
        private void label1_Click(object sender, EventArgs e) { }
    }
}