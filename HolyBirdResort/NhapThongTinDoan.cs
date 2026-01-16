using System;
using System.Collections.Generic;
using System.Windows.Forms;
using HolyBirdResort.DAO;
using HolyBirdResort.DTO;
using Guna.UI2.WinForms;

namespace HolyBirdResort
{
    public partial class NhapThongTinDoan : Form
    {
        public NhapThongTinDoan()
        {
            InitializeComponent();

            SetupTimePicker(dtpNgayBD);
            SetupTimePicker(dtpNgayKT);
        }

        // Hàm tiện ích để cài đặt định dạng Ngày Giờ
        private void SetupTimePicker(Guna2DateTimePicker dtp)
        {
            dtp.Format = DateTimePickerFormat.Custom;
            dtp.CustomFormat = "dd/MM/yyyy HH:mm"; // Hiển thị: 25/11/2025 14:30
            dtp.ShowUpDown = true; // (Tùy chọn) Hiện nút lên xuống để chỉnh giờ cho nhanh
        }

        private void Exit_Click(object sender, EventArgs e) => Application.Exit();
        private void QuayLai_Click(object sender, EventArgs e) => this.Close();

        // Sự kiện thay đổi số lượng thành viên (Tự động thêm/bớt dòng)
        private void SoLuongThanhVien_ValueChanged(object sender, EventArgs e)
        {
            int soLuong = (int)vcSoLuongThanhVien.Value;
            int soDongHienTai = dgvThanhVien.Rows.Count;
            // Trừ đi dòng NewRow nếu Grid đang để AllowUserToAddRows = true
            if (dgvThanhVien.AllowUserToAddRows) soDongHienTai--;

            if (soDongHienTai < soLuong)
            {
                for (int i = soDongHienTai; i < soLuong; i++) dgvThanhVien.Rows.Add();
            }
            else if (soDongHienTai > soLuong)
            {
                for (int i = soDongHienTai - 1; i >= soLuong; i--) dgvThanhVien.Rows.RemoveAt(i);
            }
        }

        private void XacNhan_Click(object sender, EventArgs e)
        {
            // 1. VALIDATE CƠ BẢN
            int soLuong = (int)vcSoLuongThanhVien.Value;
            if (soLuong <= 0)
            {
                MessageBox.Show("Số lượng thành viên phải > 0", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
            }
            if (dtpNgayKT.Value <= dtpNgayBD.Value)
            {
                MessageBox.Show("Ngày kết thúc phải sau ngày bắt đầu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
            }

            // 2. THU THẬP DỮ LIỆU
            List<KhachHang> dsKhach = new List<KhachHang>();
            int sttTruongDoan = 0;
            int demTruongDoan = 0;
            int rowCount = 0;

            foreach (DataGridViewRow row in dgvThanhVien.Rows)
            {
                if (row.IsNewRow) continue;
                if (rowCount >= soLuong) break;

                string ten = Convert.ToString(row.Cells[0].Value)?.Trim();
                string cccd = Convert.ToString(row.Cells[1].Value)?.Trim();
                string strNgaySinh = Convert.ToString(row.Cells[2].Value)?.Trim();
                string sdt = Convert.ToString(row.Cells[3].Value)?.Trim();

                // Kiểm tra null cho ô CheckBox tránh lỗi
                bool isTruongDoan = false;
                if (row.Cells[4].Value != null && row.Cells[4].Value is bool val)
                {
                    isTruongDoan = val;
                }

                if (string.IsNullOrEmpty(ten) || string.IsNullOrEmpty(cccd))
                {
                    MessageBox.Show($"Dòng {rowCount + 1}: Thiếu Tên hoặc CCCD!", "Thiếu thông tin"); return;
                }

                DateTime ngaySinh;
                if (!DateTime.TryParse(strNgaySinh, out ngaySinh))
                {
                    MessageBox.Show($"Dòng {rowCount + 1}: Ngày sinh sai định dạng (dd/MM/yyyy)!", "Lỗi"); return;
                }

                dsKhach.Add(new KhachHang
                {
                    TenKH = ten,
                    SoCMND = cccd, // Map đúng vào thuộc tính đã sửa trong DTO
                    NgaySinh = ngaySinh,
                    SDT = sdt
                });

                if (isTruongDoan)
                {
                    demTruongDoan++;
                    sttTruongDoan = rowCount + 1;
                }
                rowCount++;
            }

            if (demTruongDoan != 1)
            {
                MessageBox.Show("Vui lòng chọn duy nhất 1 Trưởng đoàn!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. GỌI DAO
            try
            {
                Doan doan = new Doan { SoNguoi = soLuong };
                GiaoDich gd = new GiaoDich { ThoiGianBatDau = dtpNgayBD.Value, ThoiGianKetThuc = dtpNgayKT.Value };

                string resultMaDoan = DoanDAO.ThemDoanMoi(doan, dsKhach, gd, sttTruongDoan);

                if (!string.IsNullOrEmpty(resultMaDoan))
                {
                    MessageBox.Show($"Thêm đoàn thành công!\nMã đoàn: {resultMaDoan}",
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống: " + ex.Message, "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}