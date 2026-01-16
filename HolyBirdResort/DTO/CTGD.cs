using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolyBirdResort.DTO
{ 
    public class CTGD
    {
        public string MaCTGD { get; set; }           // CHAR(6)
        public string MaDoan { get; set; }           // CHAR(4)
        public int MaKH { get; set; }                // INT
        public string MaPhong { get; set; }          // CHAR(4)
        public DateTime ThoiGianNhanPhong { get; set; }
        public DateTime ThoiGianTraPhong { get; set; }
        public DateTime ThoiGianThucHien { get; set; }
        public decimal ThanhTien { get; set; }       // DECIMAL(18,2)
        public string TrangThai { get; set; }        // NVARCHAR(50) 

        // --- BỔ SUNG THUỘC TÍNH HIỂN THỊ (Không map vào DB, chỉ dùng để hiện lên Grid) ---
        public string TenKH { get; set; }
    }
}
