using Guna.UI2.WinForms;
using HolyBirdResort.DAO;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HolyBirdResort
{
    public partial class XacNhanDat : Form
    {
        public XacNhanDat()
        {
            InitializeComponent();
        }

        // ==== Các thuộc tính nhận dữ liệu từ Form TTDatPhong ====
        public string MaPhong { get; set; }
        public int MaKH { get; set; }
        public string MaDoan { get; set; }

        public string TenKhach { get; set; }
        public string CCCD { get; set; }
        public DateTime NgaySinh { get; set; }
        public string SDT { get; set; }

        public string SoPhong { get; set; }
        public string Tang { get; set; }
        public string HangPhong { get; set; }
        public string HinhThuc { get; set; }
        public string GiaUocTinh { get; set; }

        public DateTime NgayNhan { get; set; }
        public DateTime NgayTra { get; set; }

        private void lbDP_Click(object sender, EventArgs e)
        {

        }

        private void pnlMain_Paint(object sender, PaintEventArgs e)
        {

        }

        private void XacNhanDat_Load(object sender, EventArgs e)
        {
            btnXacNhan.Enabled = false;   // khóa nút khi mở form
            btnXacNhan.FillColor = Color.LightGray;  // đổi màu nhìn như bị disabled
            // Thông tin khách
            lblTenKhach.Text = TenKhach;
            lblCCCD.Text = CCCD;
            lblNgaySinh.Text = NgaySinh.ToString("dd/MM/yyyy");
            lblSDT.Text = SDT;

            // Thông tin phòng
            lblSoPhong.Text = SoPhong;
            lblTang.Text = Tang;
            lblHangPhong.Text = HangPhong;
            lblHinhThuc.Text = HinhThuc;

            // Thời gian & giá
            lblNgayNhan.Text = NgayNhan.ToString("dd/MM/yyyy");
            lblNgayTra.Text = NgayTra.ToString("dd/MM/yyyy");
            lblGiaUocTinh.Text = GiaUocTinh;
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            // Popup xác nhận đặt phòng
            msgXacNhanDat.Buttons = MessageDialogButtons.YesNo;
            msgXacNhanDat.Icon = MessageDialogIcon.Question;
            msgXacNhanDat.Caption = "Xác nhận đặt phòng";

            msgXacNhanDat.Text =
                $"Bạn có chắc chắn muốn đặt phòng {lblSoPhong.Text} " +
                $"từ ngày {lblNgayNhan.Text} đến {lblNgayTra.Text} không?";

            var result = msgXacNhanDat.Show();
            if (result != DialogResult.Yes)
                return;

            // Popup thông báo thành công
            msgThanhCong.Buttons = MessageDialogButtons.OK;
            msgThanhCong.Icon = MessageDialogIcon.Information;
            msgThanhCong.Caption = "Đặt phòng thành công";
            msgThanhCong.Text =
                "Bạn đã đặt phòng thành công!\n" +
                "Cảm ơn bạn đã chọn HolyBird Resort.";

            msgThanhCong.Show();  // Hiện popup thành công

            try
            {
                bool result1 = GiaoDichDAO.InsertDatPhong(this.MaPhong, this.MaKH, this.MaDoan, this.NgayNhan, this.NgayTra);

                if (result1)
                {
                    msgThanhCong.Show();
                    // Quay về FormTrangChuKH
                    this.Hide();
                    using (var formTrangChuKH = new FormTrangChuKH())
                    {
                        formTrangChuKH.ShowDialog();
                    }
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu dữ liệu: " + ex.Message, "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void chkDongY_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDongY.Checked)
            {
                btnXacNhan.Enabled = true;
                btnXacNhan.FillColor = Color.FromArgb(86, 145, 73); // xanh lá
            }
            else
            {
                btnXacNhan.Enabled = false;
                btnXacNhan.FillColor = Color.LightGray;
            }
        }
        private void guna2ControlBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnQuayLai_Click_1(object sender, EventArgs e)
        {
            //this.DialogResult = DialogResult.Cancel; // báo về form trước là người dùng quay lại
            this.Close(); // đóng form 
        }
    }
}
