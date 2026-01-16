using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolyBirdResort.DTO
{
    public class TheTu
    {
        public string MaTheTu { get; set; }       // CHAR(6) - Khóa chính
        public string MaPhong { get; set; }        // CHAR(4) - Khóa ngoại tham chiếu bảng PHONG
        public string MaCTGD { get; set; }         // CHAR(6) - Khóa ngoại tham chiếu bảng CTGD
        public string TrangThaiThe { get; set; }   // NVARCHAR(20) - Mặc định: N'Đã kích hoạt'
    }
}
