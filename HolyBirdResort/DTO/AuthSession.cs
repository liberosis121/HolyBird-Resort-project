using HolyBirdResort.DTO;

namespace HolyBirdResort
{
    public static class AuthSession
    {
        // Biến này lưu thông tin tài khoản đang đăng nhập hiện tại
        public static TaiKhoan CurrentUser { get; set; } = null;

        // Hàm xóa session khi đăng xuất
        public static void Clear()
        {
            CurrentUser = null;
        }
    }
}