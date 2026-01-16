using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using HolyBirdResort.DTO;

namespace HolyBirdResort.DAO
{
    public class TraPhongDAO
    {
        private static DBConnection db = new DBConnection();

        // 1. Lấy danh sách Chi tiết giao dịch ĐANG Ở (Chưa trả phòng)
        public static List<CTGD> GetListChoTraPhong()
        {
            List<CTGD> list = new List<CTGD>();
            // JOIN bảng KHACH_HANG để lấy TenKH hiển thị
            string sql = @"
                SELECT C.MaCTGD, C.MaPhong, C.MaDoan, C.MaKH, K.TenKH, C.ThoiGianNhanPhong, C.ThoiGianTraPhong, C.TrangThai
                FROM CTGD C
                JOIN KHACH_HANG K ON C.MaKH = K.MaKH AND C.MaDoan = K.MaDoan
                WHERE C.TrangThai = N'Yêu cầu trả phòng'
                ORDER BY C.MaPhong ASC, C.MaDoan ASC";

            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new CTGD
                        {
                            MaCTGD = reader["MaCTGD"].ToString(),
                            MaPhong = reader["MaPhong"].ToString(),
                            MaDoan = reader["MaDoan"].ToString(),
                            MaKH = Convert.ToInt32(reader["MaKH"]),
                            TenKH = reader["TenKH"].ToString(), // Lấy từ bảng KH
                            ThoiGianNhanPhong = Convert.ToDateTime(reader["ThoiGianNhanPhong"]),
                            ThoiGianTraPhong = Convert.ToDateTime(reader["ThoiGianTraPhong"]),
                            TrangThai = reader["TrangThai"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        // 2. Lấy danh mục Bồi thường
        public static List<BoiThuong> GetListBoiThuong()
        {
            List<BoiThuong> list = new List<BoiThuong>();
            string sql = "SELECT MaBoiThuong, MoTa, DonGia FROM BOI_THUONG";

            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();
                using (SqlDataReader reader = new SqlCommand(sql, conn).ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new BoiThuong
                        {
                            MaBoiThuong = reader["MaBoiThuong"].ToString(),
                            MoTa = reader["MoTa"].ToString(),
                            DonGia = Convert.ToDecimal(reader["DonGia"])
                        });
                    }
                }
            }
            return list;
        }

        // 3. Thực hiện Trả phòng
        public static bool XacNhanTraPhongCaPhong(string maPhong, string maDoan)
        {
            // Update tất cả các CTGD thuộc phòng đó + đoàn đó đang yêu cầu trả
            string sql = @"
        UPDATE CTGD 
        SET TrangThai = N'Đã trả phòng', 
            ThoiGianTraPhong = GETDATE() 
        WHERE MaPhong = @phong 
          AND MaDoan = @doan
          AND TrangThai = N'Yêu cầu trả phòng'"; // Chỉ xử lý khách đang yêu cầu

            using (SqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@phong", maPhong);
                    cmd.Parameters.AddWithValue("@doan", maDoan);
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return false;
                }
            }
        }

        // 4. Thêm chi tiết bồi thường (Sử dụng DTO CTBoiThuong)
        // Trong TraPhongDAO.cs

        // Sửa hàm này nhận thêm tham số boolean isFix
        public static bool ThemCTBoiThuong(CTBoiThuong ctbt)
        {
            using (SqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();

                    // THAY ĐỔI ĐỂ TEST TRƯỚC VÀ SAU KHI GIẢI QUYẾT TRANH CHẤP
                    //string spName = "sp_ThemCTBoiThuong_LOI";
                    string spName = "sp_ThemCTBoiThuong";

                    SqlCommand cmd = new SqlCommand(spName, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 60; // Tăng timeout vì có delay 10s

                    cmd.Parameters.AddWithValue("@MaCTGD", ctbt.MaCTGD);
                    cmd.Parameters.AddWithValue("@MaBoiThuong", ctbt.MaBoiThuong);
                    cmd.Parameters.AddWithValue("@SoLuongThem", ctbt.SoLuong);

                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (SqlException ex)
                {
                    // Trong trường hợp Serializable có thể xảy ra Deadlock nếu tranh chấp gắt
                    // Mã lỗi 1205 là Deadlock
                    if (ex.Number == 1205)
                    {
                        throw new Exception("Giao dịch bị chặn do xung đột (Deadlock). Vui lòng thử lại!");
                    }
                    throw new Exception(ex.Message);
                }
            }
        }

        // Lấy danh sách chi tiết bồi thường của 1 CTGD
        public static string GetDanhSachBoiThuong(string maCTGD)
        {
            string result = "";
            string sql = @"
                        SELECT B.MoTa, C.SoLuong
                        FROM CT_BOI_THUONG C
                        JOIN BOI_THUONG B ON C.MaBoiThuong = B.MaBoiThuong
                        WHERE C.MaCTGD = @ma";

            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ma", maCTGD);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows) return "Chưa có bồi thường nào.";

                    int stt = 1;
                    while (reader.Read())
                    {
                        string ten = reader["MoTa"].ToString();
                        int sl = Convert.ToInt32(reader["SoLuong"]);

                        result += $"{stt}. {ten} | SL: {sl} \n";
                        stt++;
                    }
                }
            }
            return result;
        }
    }
}