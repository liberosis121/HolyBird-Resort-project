using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using HolyBirdResort.DTO;

namespace HolyBirdResort.DAO
{
    public class TaiKhoanDAO
    {
        private static DBConnection db = new DBConnection();

        // 1. Lấy danh sách các đoàn CHƯA CÓ tài khoản
        // Trong file DAO/TaiKhoanDAO.cs

        public static List<Doan> GetListDoanChuaCoTaiKhoan()
        {
            List<Doan> list = new List<Doan>();

            // JOIN bảng DOAN và KHACH_HANG để lấy tên người đại diện
            // Điều kiện Join: Cùng Mã Đoàn VÀ Mã Khách Hàng trùng với Người Đại Diện
            string sql = @"
            SELECT D.MaDoan, D.SoNguoi, K.TenKH AS TenTruongDoan
            FROM DOAN D
            LEFT JOIN KHACH_HANG K ON D.MaDoan = K.MaDoan AND D.NguoiDaiDien = K.MaKH
            WHERE D.MaDoan NOT IN (SELECT MaDoan FROM TAI_KHOAN)";

            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Doan
                        {
                            MaDoan = reader["MaDoan"].ToString(),
                            SoNguoi = Convert.ToInt32(reader["SoNguoi"]),
                            // Lấy cột TenTruongDoan từ câu SQL gán vào DTO
                            TenTruongDoan = reader["TenTruongDoan"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        // 2. Tạo tài khoản mới
        public static bool TaoTaiKhoan(TaiKhoan tk)
        {
            string sql = "INSERT INTO TAI_KHOAN (MaDoan, TenDangNhap, MatKhau, TrangThaiKichHoat, SoNguoiDung) VALUES (@Ma, @User, @Pass, N'Đã kích hoạt', @SoNguoi)";

            using (SqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Ma", tk.MaDoan);
                    cmd.Parameters.AddWithValue("@User", tk.TenDangNhap);
                    cmd.Parameters.AddWithValue("@Pass", tk.MatKhau);
                    cmd.Parameters.AddWithValue("@SoNguoi", tk.SoNguoiDung);

                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception)
                {
                    return false; // Thường lỗi do trùng Username (UNIQUE constraint)
                }
            }
        }

        // 3. Kiểm tra tên đăng nhập đã tồn tại chưa
        public static bool CheckUsernameExist(string username)
        {
            string sql = "SELECT COUNT(*) FROM TAI_KHOAN WHERE TenDangNhap = @User";
            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@User", username);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        // KIỂM TRA ĐĂNG NHẬP
        public static TaiKhoan CheckLogin(string username, string password)
        {
            TaiKhoan tk = null;

            // Query kiểm tra user và pass (Lưu ý: Pass trong DB hiện đang là plain text '123456')
            string sql = "SELECT * FROM TAI_KHOAN WHERE TenDangNhap = @User AND MatKhau = @Pass";

            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@User", username);
                cmd.Parameters.AddWithValue("@Pass", password);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        tk = new TaiKhoan
                        {
                            MaDoan = reader["MaDoan"].ToString(),
                            TenDangNhap = reader["TenDangNhap"].ToString(),
                            MatKhau = reader["MatKhau"].ToString(),
                            TrangThaiKichHoat = reader["TrangThaiKichHoat"].ToString(),
                            SoNguoiDung = Convert.ToInt32(reader["SoNguoiDung"])
                        };
                    }
                }
            }
            return tk; // Trả về null nếu không tìm thấy, trả về Object nếu đúng
        }
    }
}