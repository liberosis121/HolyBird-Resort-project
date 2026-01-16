using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using HolyBirdResort.DTO;

namespace HolyBirdResort.DAO
{
    public class DoanDAO
    {
        private static DBConnection db = new DBConnection();

        // Hàm này nhận vào List<KhachHang>, nên class KhachHang bắt buộc phải là public
        public static string ThemDoanMoi(Doan doan, List<KhachHang> dsKhach, GiaoDich gd, int sttTruongDoan)
        {
            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // 1. INSERT ĐOÀN (Trigger tự sinh MaDoan)
                    string sqlDoan = "INSERT INTO DOAN (SoNguoi, NguoiDaiDien) VALUES (@SoNguoi, NULL); SELECT MAX(MaDoan) FROM DOAN;";
                    SqlCommand cmdDoan = new SqlCommand(sqlDoan, conn, transaction);
                    cmdDoan.Parameters.AddWithValue("@SoNguoi", doan.SoNguoi);

                    object result = cmdDoan.ExecuteScalar();
                    if (result == null) throw new Exception("Lỗi không tạo được mã đoàn.");
                    string maDoanMoi = result.ToString();

                    // 2. INSERT KHÁCH HÀNG
                    string sqlKH = @"INSERT INTO KHACH_HANG (MaDoan, MaKH, SoCMND, TenKH, NgaySinh, SDT) 
                                     VALUES (@MaDoan, @MaKH, @CMND, @Ten, @NgaySinh, @SDT)";

                    int maKH_Count = 1;
                    foreach (var kh in dsKhach)
                    {
                        SqlCommand cmdKH = new SqlCommand(sqlKH, conn, transaction);
                        cmdKH.Parameters.AddWithValue("@MaDoan", maDoanMoi);
                        cmdKH.Parameters.AddWithValue("@MaKH", maKH_Count);
                        cmdKH.Parameters.AddWithValue("@CMND", kh.SoCMND); //
                        cmdKH.Parameters.AddWithValue("@Ten", kh.TenKH);
                        cmdKH.Parameters.AddWithValue("@NgaySinh", kh.NgaySinh);
                        cmdKH.Parameters.AddWithValue("@SDT", kh.SDT);
                        cmdKH.ExecuteNonQuery();

                        if (maKH_Count == sttTruongDoan) doan.NguoiDaiDien = maKH_Count;
                        maKH_Count++;
                    }

                    // 3. UPDATE NGƯỜI ĐẠI DIỆN
                    string sqlUpdate = "UPDATE DOAN SET NguoiDaiDien = @DaiDien WHERE MaDoan = @MaDoan";
                    SqlCommand cmdUp = new SqlCommand(sqlUpdate, conn, transaction);
                    cmdUp.Parameters.AddWithValue("@DaiDien", doan.NguoiDaiDien);
                    cmdUp.Parameters.AddWithValue("@MaDoan", maDoanMoi);
                    cmdUp.ExecuteNonQuery();

                    // 4. INSERT GIAO DỊCH (Đã bỏ phần TAI_KHOAN theo yêu cầu)
                    string sqlGD = "INSERT INTO GIAO_DICH (MaDoan, ThoiGianBatDau, ThoiGianKetThuc) VALUES (@MaDoan, @BD, @KT)";
                    SqlCommand cmdGD = new SqlCommand(sqlGD, conn, transaction);
                    cmdGD.Parameters.AddWithValue("@MaDoan", maDoanMoi);
                    cmdGD.Parameters.AddWithValue("@BD", gd.ThoiGianBatDau);
                    cmdGD.Parameters.AddWithValue("@KT", gd.ThoiGianKetThuc);
                    cmdGD.ExecuteNonQuery();

                    transaction.Commit();
                    return maDoanMoi;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}