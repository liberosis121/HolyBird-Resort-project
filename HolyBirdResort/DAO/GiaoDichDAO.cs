using HolyBirdResort.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace HolyBirdResort.DAO
{
    public class GiaoDichDAO
    {
        private static DBConnection db = new DBConnection();

        public static GiaoDich GetThongTinChuyenDi(string maDoan)
        {
            string sql = "SELECT * FROM GIAO_DICH WHERE MaDoan = @Ma";
            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Ma", maDoan);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new GiaoDich
                        {
                            MaDoan = maDoan,
                            ThoiGianBatDau = Convert.ToDateTime(reader["ThoiGianBatDau"]),
                            ThoiGianKetThuc = Convert.ToDateTime(reader["ThoiGianKetThuc"])
                        };
                    }
                }
            }
            return null;
        }

        public static bool InsertDatPhong(string maPhong, int maKH, string maDoan, DateTime nhan, DateTime tra)
        {
            // 1. Bỏ cột MaCTGD ra khỏi câu Insert (Để Database tự sinh)
            // 2. Chỉ Insert các cột cần thiết
            string sql = @"
                        INSERT INTO CTGD (MaPhong, MaDoan, MaKH, ThoiGianNhanPhong, ThoiGianTraPhong, ThoiGianThucHien, TrangThai)
                        VALUES (@MaPhong, @MaDoan, @MaKH, @Nhan, @Tra, GETDATE(), N'Chưa nhận phòng')";

            using (SqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);

                    // Truyền tham số
                    cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                    cmd.Parameters.AddWithValue("@MaKH", maKH);

                    // Xử lý MaDoan null (Tránh lỗi parameter not supplied)
                    if (string.IsNullOrEmpty(maDoan))
                        cmd.Parameters.AddWithValue("@MaDoan", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@MaDoan", maDoan);

                    // Mỗi lần gọi hàm là một lần Add mới, không lo lỗi "Already declared"
                    cmd.Parameters.AddWithValue("@Nhan", nhan);
                    cmd.Parameters.AddWithValue("@Tra", tra);

                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (SqlException ex)
                {
                    // Ném lỗi chi tiết ra để dễ debug
                    throw new Exception("Lỗi SQL: " + ex.Message);
                }
            }
        }

        public static DataTable GetDanhSachPhongDaDat(string maDoan)
        {
            string sql = @"
                SELECT 
                    MaPhong, 
                    MIN(ThoiGianNhanPhong) as NgayNhan, 
                    MAX(ThoiGianTraPhong) as NgayTra
                FROM CTGD
                WHERE MaDoan = @MaDoan 
                  AND TrangThai IN (N'Chưa nhận phòng', N'Chưa trả phòng', N'Yêu cầu trả phòng', N'Đã trả phòng')
                GROUP BY MaPhong";

            using (SqlConnection conn = db.GetConnection())
            {
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.SelectCommand.Parameters.AddWithValue("@MaDoan", maDoan);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // 2. Lấy danh sách KHÁCH HÀNG trong một phòng cụ thể của đoàn
        public static DataTable GetKhachHangTrongPhong(string maDoan, string maPhong)
        {
            // Bổ sung đầy đủ các cột thông tin khách hàng cần thiết cho Grid
            string sql = @"
        SELECT 
            K.MaKH as [Mã Khách Hàng],
            K.TenKH as [Họ và tên],
            K.SoCMND as [CCCD], 
            K.SDT as [SĐT], 
            K.NgaySinh as [Ngày Sinh],
            C.TrangThai
        FROM CTGD C
        INNER JOIN KHACH_HANG K ON C.MaKH = K.MaKH AND C.MaDoan = K.MaDoan
        WHERE C.MaDoan = @MaDoan 
          AND C.MaPhong = @MaPhong
          -- Cho phép lấy cả khách đã xác nhận trả để thực hiện bước thanh toán
          AND C.TrangThai NOT IN (N'Đã hủy')";

            using (SqlConnection conn = db.GetConnection())
            {
                try
                {
                    SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                    da.SelectCommand.Parameters.AddWithValue("@MaDoan", maDoan);
                    da.SelectCommand.Parameters.AddWithValue("@MaPhong", maPhong);

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi lấy danh sách khách trong phòng: " + ex.Message);
                }
            }
        }

        public static DataRow GetChiTietDeHuy(string maKH, string maDoan, string maPhong)
        {
            // Sử dụng LEFT JOIN để nếu không có MaKH thì vẫn lấy được thông tin PHONG
            // Dùng điều kiện (MaKH = @MaKH OR @MaKH = '') để xử lý trường hợp truyền rỗng
            string sql = @"
        SELECT TOP 1
            K.SoCMND, 
            K.NgaySinh, 
            K.SDT, 
            P.MaTang, 
            P.LoaiHang, 
            P.TenHinhThuc, 
            C.ThoiGianNhanPhong, 
            C.ThoiGianTraPhong, 
            C.ThanhTien,
            P.GiaPhong
        FROM CTGD C
        INNER JOIN PHONG P ON C.MaPhong = P.MaPhong
        LEFT JOIN KHACH_HANG K ON C.MaKH = K.MaKH AND C.MaDoan = K.MaDoan
        WHERE C.MaDoan = @MaDoan 
          AND C.MaPhong = @MaPhong
          AND (@MaKH = '' OR C.MaKH = @MaKH)";

            using (SqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@MaKH", maKH);
                    cmd.Parameters.AddWithValue("@MaDoan", maDoan);
                    cmd.Parameters.AddWithValue("@MaPhong", maPhong);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    return dt.Rows.Count > 0 ? dt.Rows[0] : null;
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi truy vấn chi tiết: " + ex.Message);
                }
            }
        }

        public static bool XoaChiTietGiaoDich(string maKH, string maDoan, string maPhong)
        {
            // Sử dụng lệnh DELETE để xóa hẳn bản ghi khỏi bảng CTGD
            string sql = @"DELETE FROM CTGD 
                   WHERE MaKH = @MaKH AND MaDoan = @MaDoan AND MaPhong = @MaPhong";

            using (SqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@MaKH", maKH);
                    cmd.Parameters.AddWithValue("@MaDoan", maDoan);
                    cmd.Parameters.AddWithValue("@MaPhong", maPhong);

                    int result = cmd.ExecuteNonQuery();
                    return result > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi khi xóa chi tiết giao dịch: " + ex.Message);
                }
            }
        }

        public static int DemTongSoCTGDActiveCuaDoan(string maDoan)
        {
            // Đếm tất cả các bản ghi chưa bị hủy/trả của đoàn trên mọi phòng
            string sql = "SELECT COUNT(*) FROM CTGD WHERE MaDoan = @MaDoan AND TrangThai NOT IN (N'Đã hủy', N'Đã trả phòng')";
            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaDoan", maDoan);
                return (int)cmd.ExecuteScalar();
            }
        }

        // Trong GiaoDichDAO.cs

        // 1. Cập nhật tất cả khách trong phòng sang trạng thái 'Yêu cầu trả phòng'
        public static bool YeuCauTraPhong(string maDoan, string maPhong)
        {
            string sql = @"UPDATE CTGD SET TrangThai = N'Yêu cầu trả phòng' 
                   WHERE MaDoan = @MaDoan AND MaPhong = @MaPhong 
                   AND TrangThai IN (N'Chưa trả phòng', N'Đã đặt')";
            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaDoan", maDoan);
                cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // 2. Lấy danh sách bồi thường tổng hợp của tất cả khách trong phòng
        public static DataTable GetTongHopBoiThuong(string maDoan, string maPhong)
        {
            // Câu lệnh SQL truy vấn bồi thường của tất cả khách hàng thuộc cùng 1 phòng trong 1 đoàn
            string sql = @"SELECT LBT.MoTa as [Danh mục bồi thường], 
                          CTBT.SoLuong as [Số lượng], 
                          LBT.DonGia as [Đơn giá],
                          SUM(LBT.DonGia) as [Thành tiền]
                   FROM CT_BOI_THUONG CTBT
                   JOIN BOI_THUONG LBT ON CTBT.MaBoiThuong = LBT.MaBoiThuong
                   JOIN CTGD C ON CTBT.MaCTGD = C.MaCTGD
                   WHERE C.MaDoan = @MaDoan 
                     AND C.MaPhong = @MaPhong
                     AND C.TrangThai NOT IN (N'Đã hủy') -- Chỉ lấy bồi thường của các giao dịch hợp lệ
                   GROUP BY LBT.MoTa, CTBT.SoLuong, LBT.DonGia";

            // Sử dụng đối tượng kết nối db đã có sẵn trong DAO của bạn
            using (SqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);

                    // Truyền tham số để chống SQL Injection
                    cmd.Parameters.AddWithValue("@MaDoan", maDoan);
                    cmd.Parameters.AddWithValue("@MaPhong", maPhong);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    return dt;
                }
                catch (Exception ex)
                {
                    // Ném lỗi để phía UI (UserControl/Form) có thể hiển thị MessageBox
                    throw new Exception("Lỗi khi lấy dữ liệu bồi thường tổng hợp: " + ex.Message);
                }
            }
        }

        public static bool NhanPhong(string maDoan, string maPhong)
        {
            // Chuyển từ 'Chưa nhận phòng' (hoặc 'Đã đặt') sang 'Chưa trả phòng'
            string sql = @"UPDATE CTGD SET TrangThai = N'Chưa trả phòng' 
                   WHERE MaDoan = @MaDoan AND MaPhong = @MaPhong 
                   AND TrangThai IN (N'Chưa nhận phòng', N'Đã đặt')";

            using (SqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@MaDoan", maDoan);
                    cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch { return false; }
            }
        }

        public static decimal GetTongTienThuePhong(string maDoan, string maPhong)
        {
            // Tính tổng tiền của tất cả CTGD thuộc phòng này (chưa bị hủy)
            string sql = @"SELECT SUM(ThanhTien) 
                   FROM CTGD 
                   WHERE MaDoan = @MaDoan AND MaPhong = @MaPhong 
                   AND TrangThai NOT IN (N'Đã hủy')";
            using (SqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@MaDoan", maDoan);
                    cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                    object res = cmd.ExecuteScalar();
                    return res != DBNull.Value ? Convert.ToDecimal(res) : 0;
                }
                catch { return 0; }
            }
        }

        public static bool CapNhatThanhToanXong(string maDoan, string maPhong)
        {
            // Chuyển toàn bộ khách trong phòng đó sang trạng thái 'Đã thanh toán'
            string sql = @"UPDATE CTGD SET TrangThai = N'Đã thanh toán' 
                   WHERE MaDoan = @MaDoan AND MaPhong = @MaPhong 
                   AND TrangThai NOT IN (N'Đã hủy')";
            using (SqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@MaDoan", maDoan);
                    cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch { return false; }
            }
        }
    }
}