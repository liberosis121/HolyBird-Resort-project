using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolyBirdResort.DTO
{
    public class Tang { public string MaTang { get; set; } public int SoPhong { get; set; } }
    public class HangPhong { public string LoaiHang { get; set; } public decimal HeSoGiaHP { get; set; } }
    public class HinhThucPhong { public string TenHinhThuc { get; set; } public int SoNguoiToiDa { get; set; } public decimal HeSoGiaHT { get; set; } }
}
