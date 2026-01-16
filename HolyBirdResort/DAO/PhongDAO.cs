using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using HolyBirdResort.DTO;
using System.Data;

namespace HolyBirdResort.DAO
{
    public class PhongDAO
    {
        private static DBConnection db = new DBConnection();

        // Lấy chi tiết 1 phòng theo Mã
        public static Phong GetPhongByMa(string maPhong)
        {
            using (var conn = db.GetConnection())
            {
                conn.Open();
                // JOIN để lấy SoNguoiToiDa từ bảng HINH_THUC_PHONG
                string sql = @"
                            SELECT p.MaPhong, p.MaTang, p.LoaiHang, p.TenHinhThuc, p.GiaPhong, p.TrangThai, 
                                   h.SoNguoiToiDa 
                            FROM PHONG p 
                            JOIN HINH_THUC_PHONG h ON p.TenHinhThuc = h.TenHinhThuc
                            WHERE p.MaPhong = @id";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", maPhong);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Phong
                            {
                                MaPhong = reader["MaPhong"].ToString(),
                                MaTang = reader["MaTang"].ToString(),
                                LoaiHang = reader["LoaiHang"].ToString(),
                                TenHinhThuc = reader["TenHinhThuc"].ToString(),
                                GiaPhong = reader.GetDecimal(reader.GetOrdinal("GiaPhong")),
                                TrangThai = reader["TrangThai"].ToString(),
                                // Lấy số người tối đa từ kết quả Join
                                SoNguoiToiDa = Convert.ToInt32(reader["SoNguoiToiDa"])
                            };
                        }
                    }
                }
            }
            return null;
        }

        // Lấy toàn bộ danh sách phòng
        public static List<Phong> GetAllPhong()
        {
            var list = new List<Phong>();
            using (var conn = db.GetConnection())
            {
                conn.Open();
                string sql = "SELECT MaPhong, MaTang, LoaiHang, TenHinhThuc, GiaPhong, TrangThai FROM Phong";
                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Phong
                        {
                            MaPhong = reader["MaPhong"].ToString(),
                            MaTang = reader["MaTang"].ToString(),
                            LoaiHang = reader["LoaiHang"].ToString(),
                            TenHinhThuc = reader["TenHinhThuc"].ToString(),
                            GiaPhong = reader.GetDecimal(reader.GetOrdinal("GiaPhong")),
                            TrangThai = reader["TrangThai"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        //  Thêm phòng mới
        public static bool InsertPhong(Phong p)
        {
            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("sp_ThemPhong", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@MaPhong", p.MaPhong);
                    cmd.Parameters.AddWithValue("@MaTang", p.MaTang);
                    cmd.Parameters.AddWithValue("@LoaiHang", p.LoaiHang);
                    cmd.Parameters.AddWithValue("@TenHinhThuc", p.TenHinhThuc);

                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Cập nhật phòng
        public static string UpdatePhong(Phong p)
        {
            using (SqlConnection conn = db.GetConnection())
            {
                try
                {
                    // TÌNH HUỐNG 15: DEMO TRƯỚC VÀ SAU FIX LỖI
                    SqlCommand cmd = new SqlCommand("sp_CapNhatThongTinPhong_LOI", conn);
                    // SqlCommand cmd = new SqlCommand("sp_CapNhatThongTinPhong", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@MaPhong", p.MaPhong);

                    // Nếu giá trị rỗng hoặc null thì gửi DBNull.Value xuống SQL
                    cmd.Parameters.AddWithValue("@MaTangMoi", string.IsNullOrEmpty(p.MaTang) ? DBNull.Value : (object)p.MaTang);
                    cmd.Parameters.AddWithValue("@HangPhongMoi", string.IsNullOrEmpty(p.LoaiHang) ? DBNull.Value : (object)p.LoaiHang);
                    cmd.Parameters.AddWithValue("@HinhThucMoi", string.IsNullOrEmpty(p.TenHinhThuc) ? DBNull.Value : (object)p.TenHinhThuc);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return "SUCCESS";
                }
                catch (SqlException ex)
                {
                    // Trả về nội dung lỗi THROW từ SQL (ví dụ: "Mã tầng không tồn tại...")
                    return ex.Message;
                }
            }
        }

        //  Lấy danh sách Hạng Phòng (để đổ vào ComboBox)
        public static DataTable GetHangPhong()
        {
            string sql = "SELECT LoaiHang FROM HANG_PHONG";
            using (SqlConnection conn = db.GetConnection())
            {
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        //  Lấy danh sách Hình Thức (để đổ vào ComboBox)
        public static DataTable GetHinhThuc()
        {
            string sql = "SELECT TenHinhThuc FROM HINH_THUC_PHONG";
            using (SqlConnection conn = db.GetConnection())
            {
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        //  Lấy hệ số giá của Hạng Phòng
        public static decimal GetHeSoHangPhong(string loaiHang)
        {
            string sql = "SELECT HeSoGiaHP FROM HANG_PHONG WHERE LoaiHang = @lh";
            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@lh", loaiHang);
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToDecimal(result) : 0;
            }
        }
        // ---------------------------------------------------------
        //  CÁC HÀM DEMO TRANH CHẤP TÌNH HUỐNG 3 (T1 & T2)
        // ---------------------------------------------------------

        // T1: Mô phỏng nhân viên đang tra cứu để ra quyết định (Có Delay 10s)
        // Hàm này trả về chuỗi thông báo từ lệnh PRINT trong SQL
        public static string Demo_TraCuuHeSo_T1(string loaiHang, bool isFix)
        {
            string messageResult = "";
            string spName = isFix ? "sp_TraCuuHeSoGia" : "sp_TraCuuHeSoGia_LOI";

            using (SqlConnection conn = db.GetConnection())
            {
                // Lắng nghe sự kiện InfoMessage để bắt lệnh PRINT từ SQL
                conn.InfoMessage += (object sender, SqlInfoMessageEventArgs e) =>
                {
                    messageResult += e.Message + Environment.NewLine;
                };

                // Phải bật dòng này mới nhận được InfoMessage
                conn.FireInfoMessageEventOnUserErrors = true;

                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(spName, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 60; // Tăng timeout vì SP chạy lâu (10s+)

                    cmd.Parameters.AddWithValue("@LoaiHang", loaiHang);

                    // Dùng ExecuteNonQuery vì SP chỉ PRINT chứ không return Table
                    cmd.ExecuteNonQuery();

                    return string.IsNullOrEmpty(messageResult)
                        ? "Đã thực hiện xong nhưng không có thông báo."
                        : messageResult;
                }
                catch (SqlException ex)
                {
                    return "Lỗi SQL: " + ex.Message;
                }
            }
        }

        // T2: Cập nhật Hạng Phòng (Thay thế hàm UpdateHangPhong cũ)
        public static bool UpdateHangPhong(string loaiHang, decimal heSo)
        {
            // Sử dụng SP cập nhật thay vì câu SQL trần
            using (SqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("sp_CapNhatHeSoGiaHP", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@LoaiHang", loaiHang);
                    cmd.Parameters.AddWithValue("@HeSoMoi", heSo);

                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        // Lấy hệ số giá của Hình Thức
        public static decimal GetHeSoHinhThuc(string tenHT)
        {
            string sql = "SELECT HeSoGiaHT FROM HINH_THUC_PHONG WHERE TenHinhThuc = @ht";
            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ht", tenHT);
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToDecimal(result) : 0;
            }
        }

        // Cập nhật Hình Thức
        public static bool UpdateHinhThuc(string tenHT, decimal heSo)
        {
            string sql = "UPDATE HINH_THUC_PHONG SET HeSoGiaHT = @hs WHERE TenHinhThuc = @ht";
            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ht", tenHT);
                cmd.Parameters.AddWithValue("@hs", heSo);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // TÌNH HUỐNG 11: T1: Thực hiện tra cứu (Chạy SP trả về 2 bảng kết quả)
        public static string Demo_TraCuuPhong_T1(string loaiHang, bool isFix)
        {
            string spName = isFix ? "sp_TraCuuPhongTheoHang" : "sp_TraCuuPhongTheoHang_LOI";
            // Dùng StringBuilder nối chuỗi cho tối ưu
            System.Text.StringBuilder log = new System.Text.StringBuilder();

            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(spName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@LoaiHang", loaiHang);
                cmd.CommandTimeout = 60;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // --- LẦN ĐỌC 1 ---
                    log.AppendLine("=== LẦN ĐỌC 1 (Trước khi thêm phòng) ===");
                    List<string> listPhong1 = new List<string>();
                    int count1 = 0;

                    while (reader.Read())
                    {
                        count1 = Convert.ToInt32(reader["SoLuong"]);
                        listPhong1.Add(reader["MaPhong"].ToString());
                    }

                    // In danh sách cách nhau bởi dấu chấm phẩy
                    log.AppendLine($"-> Tổng: {count1} phòng");
                    log.AppendLine("-> Danh sách: " + string.Join("; ", listPhong1));
                    log.AppendLine(); // Xuống dòng

                    // --- LẦN ĐỌC 2 ---
                    if (reader.NextResult())
                    {
                        log.AppendLine("=== LẦN ĐỌC 2 (Sau 10 giây, đã thêm \"bóng ma\") ===");
                        List<string> listPhong2 = new List<string>();
                        int count2 = 0;

                        while (reader.Read())
                        {
                            count2 = Convert.ToInt32(reader["SoLuong"]);
                            listPhong2.Add(reader["MaPhong"].ToString());
                        }

                        log.AppendLine($"-> Tổng: {count2} phòng");
                        log.AppendLine("-> Danh sách: " + string.Join("; ", listPhong2));
                        log.AppendLine();

                        // KẾT LUẬN
                        if (count1 != count2)
                        {
                            log.AppendLine("❌ KẾT QUẢ: LỆCH DỮ LIỆU!");
                            log.AppendLine("=> Đã xảy ra lỗi PHANTOM READ (Bóng ma).");
                        }
                        else
                        {
                            log.AppendLine("✅ KẾT QUẢ: DỮ LIỆU NHẤT QUÁN.");
                        }
                    }
                }
            }
            return log.ToString();
        }

        public static bool CheckPhongTrong(string maPhong, DateTime tuNgay, DateTime denNgay)
        {
            // Logic: Tìm xem có giao dịch nào chồng lấn thời gian không
            // (Start1 < End2) AND (Start2 < End1)
            string sql = @"
        SELECT COUNT(*) 
        FROM CTGD 
        WHERE MaPhong = @MaPhong 
          AND TrangThai IN (N'Đang ở', N'Đã đặt') -- Chỉ xét các trạng thái chiếm phòng
          AND (@TuNgay < ThoiGianTraPhong AND @DenNgay > ThoiGianNhanPhong)";

            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                cmd.Parameters.AddWithValue("@DenNgay", denNgay);

                int count = (int)cmd.ExecuteScalar();
                return count == 0; // Nếu = 0 là trống, > 0 là bị trùng
            }
        }
    }
}
