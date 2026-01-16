using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolyBirdResort.DTO
{
    public class TaiKhoan
    {
        public string MaDoan { get; set; }         // CHAR(4)
        public string TenDangNhap { get; set; }    // NVARCHAR(50)
        public string MatKhau { get; set; }        // NVARCHAR(55)
        public string TrangThaiKichHoat { get; set; }
        public int SoNguoiDung { get; set; }
    }
}
