using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HolyBirdResort
{
    public partial class FormTrangChuKH : Form
    {
        public static void SetDoubleBuffered(Control control)
        {
            typeof(Control).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance)
                ?.SetValue(control, true, null);
        } 

        public FormTrangChuKH()
        {
            InitializeComponent();
            SetDoubleBuffered(BookingButton);
            SetDoubleBuffered(guna2ShadowPanel4);
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            string message = " Vui lòng liên hệ với chúng tôi qua các kênh sau để được hỗ trợ:\n\n" +
                     "      📞 Liên hệ qua tổng đài 1900 6666\n\n" +
                     "      🔔 Liên hệ trực tiếp lễ tân tại quầy!";

            // Tiêu đề của hộp thoại thông báo
            string title = "Trợ giúp khách hàng";

            // Hiển thị hộp thoại thông báo
            MessageBox.Show(
                message,
                title,
                MessageBoxButtons.OK,        // Chỉ hiển thị nút OK
                MessageBoxIcon.Information  // Hiển thị biểu tượng thông tin
            );
        }

        private void FormTrangChuKH_Load(object sender, EventArgs e)
        {

        }

        private void ListButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var f = new FormTimKiemPhong())
            {
                f.ShowDialog();
            }
            this.Show();
        }

        private void AccountButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var f = new FormTtinTK())
            {
                f.ShowDialog();
            }
            this.Show();
        }

        private void btnDatPhong_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var f = new FormTimKiemPhong())
            {
                f.ShowDialog();
            }
            this.Show();
        }

        private void ctrlExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
