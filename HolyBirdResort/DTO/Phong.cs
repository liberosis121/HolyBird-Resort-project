using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolyBirdResort.DTO
{
    public class Phong
    {
        public string MaPhong { get; set; }      // CHAR(4)
        public string MaTang { get; set; }       // CHAR(2)
        public string LoaiHang { get; set; }     // NVARCHAR(30)
        public string TenHinhThuc { get; set; }  // NVARCHAR(50)
        public decimal GiaPhong { get; set; }    // DECIMAL(12,2)
        public string TrangThai { get; set; }    // NVARCHAR(20) - Mặc định: N'Đang trống'

        public int SoNguoiToiDa { get; set; }
    }
}
