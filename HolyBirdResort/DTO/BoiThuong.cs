using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolyBirdResort.DTO
{
    public class BoiThuong
    {
        public string MaBoiThuong { get; set; }
        public string MoTa { get; set; }
        public decimal DonGia { get; set; }
        
        // Property dùng để hiển thị trên ComboBox (Ví dụ: "Vỡ ly (50,000)")
        public string HienThi => $"{MoTa} ({DonGia:N0} VND)";
    }
}
