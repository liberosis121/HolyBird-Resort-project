using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolyBirdResort.DTO
{
    public class Doan
    {
        public string MaDoan { get; set; }    // CHAR(4)
        public int SoNguoi { get; set; }      // INT
        public int NguoiDaiDien { get; set; } // INT 

        // Đây là thuộc tính lấy từ bảng KHACH_HANG thông qua lệnh JOIN
        public string TenTruongDoan { get; set; }
    }
}
