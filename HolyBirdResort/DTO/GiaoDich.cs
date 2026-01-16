using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolyBirdResort.DTO
{
    public class GiaoDich
    {
        public string MaDoan { get; set; }
        public DateTime ThoiGianBatDau { get; set; }
        public DateTime ThoiGianKetThuc { get; set; }
        public int SoPhong { get; set; }
        public decimal TongTien { get; set; } 
        public string TrangThai { get; set; } // Chưa thanh toán / Đã thanh toán
    }
}
