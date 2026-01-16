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
    public partial class FormTrangChuNV : Form
    {
        public FormTrangChuNV()
        {
            InitializeComponent();
        }

        private void FormTrangChuNV_Load(object sender, EventArgs e)
        {

        }

        private void btnTaoTaiKhoan_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var f = new TaoTaiKhoan())
            {
                f.ShowDialog();
            }
            this.Show();
        }

        private void btnQuanLyPhong_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var f = new QuanLyPhong())
            {
                f.ShowDialog();
            }
            this.Show();
        }

        private void btnGhiNhanBoiThuong_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var f = new GhiNhanTraPhong())
            {
                f.ShowDialog();
            }
            this.Show();
        }

        private void btnDangKiDoan_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var f = new NhapThongTinDoan())
            {
                f.ShowDialog();
            }
            this.Show();
        }

        private void pbQuayLai_Click(object sender, EventArgs e)
        {
            //FormNhanVien previous = new FormNhanVien(); ;
            //previous.Show();
            this.Close();
        }

        private void ctrlExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
