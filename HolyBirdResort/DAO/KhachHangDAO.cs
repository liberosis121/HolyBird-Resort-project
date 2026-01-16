using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using HolyBirdResort.DTO;

namespace HolyBirdResort.DAO
{
    public class KhachHangDAO
    {
        private static DBConnection db = new DBConnection();

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

        public static List<KhachHang> GetKhachHangByMaDoan(string maDoan)
        {
            List<KhachHang> list = new List<KhachHang>();

            // QUAN TRỌNG: Phải có cột MaDoan trong câu SELECT
            string sql = "SELECT MaKH, MaDoan, TenKH, SoCMND, NgaySinh, SDT FROM KHACH_HANG WHERE MaDoan = @MaDoan";

            using (var conn = db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaDoan", maDoan);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new KhachHang
                        {
                            MaKH = Convert.ToInt32(reader["MaKH"]),
                            // LẤY DỮ LIỆU MÃ ĐOÀN ĐỂ TRUYỀN ĐI KHÔNG BỊ NULL
                            MaDoan = reader["MaDoan"].ToString(),
                            TenKH = reader["TenKH"].ToString(),
                            SoCMND = reader["SoCMND"].ToString(),
                            NgaySinh = reader["NgaySinh"] != DBNull.Value ? Convert.ToDateTime(reader["NgaySinh"]) : DateTime.Now,
                            SDT = reader["SDT"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        // 1. UPDATE (GỌI SP TỔNG QUÁT - CÓ DELAY)
        public static bool UpdateThongTinKhachHang(KhachHang kh)
        {
            using (SqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    // Gọi SP mới tạo
                    SqlCommand cmd = new SqlCommand("sp_CapNhatThongTinKhachHang", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Truyền đủ tham số
                    cmd.Parameters.AddWithValue("@MaKH", kh.MaKH);
                    cmd.Parameters.AddWithValue("@MaDoan", kh.MaDoan);
                    cmd.Parameters.AddWithValue("@TenKH", kh.TenKH);
                    cmd.Parameters.AddWithValue("@SoCMND", kh.SoCMND);
                    cmd.Parameters.AddWithValue("@NgaySinh", kh.NgaySinh);
                    cmd.Parameters.AddWithValue("@SDT", kh.SDT);

                    // Tăng timeout vì SP có delay 15s
                    cmd.CommandTimeout = 30;

                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi Update: " + ex.Message);
                }
            }
        }

        // 2. GET LIST (CHỌN 1 TRONG 2 SP ĐỂ DEMO)
        public static List<KhachHang> GetListThanhVien(string maDoan)
        {
            List<KhachHang> list = new List<KhachHang>();

            // --- CẤU HÌNH DEMO TẠI ĐÂY ---
            string spName = "sp_XemChiTietKhachHang";      // Bản chuẩn (Sẽ bị treo chờ Update xong)
            // string spName = "sp_XemChiTietKhachHang_LOI"; // Bản lỗi (Sẽ thấy dữ liệu mới ngay lập tức dù chưa xong)

            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(spName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaDoan", maDoan);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new KhachHang
                        {
                            MaDoan = reader["MaDoan"].ToString(), // Lấy thêm MaDoan cho đủ bộ
                            MaKH = Convert.ToInt32(reader["MaKH"]),
                            SoCMND = reader["SoCMND"].ToString(),
                            TenKH = reader["TenKH"].ToString(),
                            NgaySinh = Convert.ToDateTime(reader["NgaySinh"]),
                            SDT = reader["SDT"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        // Lấy tên người đại diện
        public static string GetTenTruongDoan(string maDoan)
        {
            string sql = @"
                SELECT K.TenKH 
                FROM DOAN D JOIN KHACH_HANG K ON D.MaDoan = K.MaDoan AND D.NguoiDaiDien = K.MaKH
                WHERE D.MaDoan = @Ma";

            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Ma", maDoan);
                object result = cmd.ExecuteScalar();
                return result != null ? result.ToString() : "Không xác định";
            }
        }
    }
}