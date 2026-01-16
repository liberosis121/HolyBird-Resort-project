USE master
GO

IF DB_ID('DB_HolyBird') IS NOT NULL
    DROP DATABASE DB_HolyBird;
GO

CREATE DATABASE DB_HolyBird;
GO

USE DB_HolyBird;
GO

-- =============================================
-- 1. TẠO BẢNG (STRUCTURE)
-- =============================================

CREATE TABLE TANG (
    MaTang CHAR(2) CONSTRAINT PK_TANG PRIMARY KEY,        
    SoPhong INT NOT NULL CHECK (SoPhong >= 0)
);
GO

CREATE TABLE HANG_PHONG (
    LoaiHang NVARCHAR(30) CONSTRAINT PK_HP PRIMARY KEY, CHECK (LoaiHang IN (N'Thường', N'Trung bình', N'Sang', N'Rất sang', N'VIP')), 
    HeSoGiaHP DECIMAL(8,4) NOT NULL CHECK (HeSoGiaHP > 0)
);
GO

CREATE TABLE HINH_THUC_PHONG (
    TenHinhThuc NVARCHAR(50) CONSTRAINT PK_HTPHONG PRIMARY KEY, CHECK (TenHinhThuc IN (N'1 giường đôi', N'1 giường đơn', N'2 giường đôi', N'2 giường đơn', N'VIP')),  
    SoNguoiToiDa INT NOT NULL CHECK (SoNguoiToiDa >= 1),
    HeSoGiaHT DECIMAL(8,4) NOT NULL CHECK (HeSoGiaHT > 0)
);
GO

CREATE TABLE PHONG (
    MaPhong CHAR(4) CONSTRAINT PK_PHONG PRIMARY KEY,          
    MaTang CHAR(2) NOT NULL,
    LoaiHang NVARCHAR(30) NOT NULL,
    TenHinhThuc NVARCHAR(50) NOT NULL,
    GiaPhong DECIMAL(12,2) DEFAULT 50,
    TrangThai NVARCHAR(20) DEFAULT N'Đang trống',
    CONSTRAINT FK_Phong_Tang FOREIGN KEY (MaTang) REFERENCES TANG(MaTang),
    CONSTRAINT FK_Phong_LoaiHang FOREIGN KEY (LoaiHang) REFERENCES HANG_PHONG(LoaiHang),
    CONSTRAINT FK_Phong_HinhThuc FOREIGN KEY (TenHinhThuc) REFERENCES HINH_THUC_PHONG(TenHinhThuc)
);
GO

CREATE TABLE DOAN (
    MaDoan CHAR(4) NOT NULL PRIMARY KEY,        
    SoNguoi INT NOT NULL CHECK (SoNguoi >= 1),
    NguoiDaiDien INT NULL                     
);
GO

CREATE TABLE KHACH_HANG (
    MaDoan CHAR(4) NOT NULL,
    MaKH INT NOT NULL,
    SoCMND NVARCHAR(20) NULL,
    TenKH NVARCHAR(55) NULL,
    NgaySinh DATE NULL CHECK (NgaySinh <= CAST(GETDATE() AS DATE)),
    SDT NVARCHAR(13) NULL,
    CONSTRAINT PK_KHACH_HANG PRIMARY KEY (MaKH, MaDoan),
    CONSTRAINT FK_KHACH_HANG_DOAN FOREIGN KEY (MaDoan) REFERENCES DOAN(MaDoan)
	ON DELETE CASCADE
    ON UPDATE CASCADE,
    CONSTRAINT Ma_MaKH CHECK (MaKH >= 1)
);
GO


CREATE TABLE TAI_KHOAN (
    MaDoan CHAR(4) CONSTRAINT PK_TK PRIMARY KEY,
    TenDangNhap NVARCHAR(50) NOT NULL UNIQUE,
    MatKhau NVARCHAR(55) NOT NULL,
    TrangThaiKichHoat NVARCHAR(20) DEFAULT N'Đã kích hoạt',
    SoNguoiDung INT DEFAULT 1,
    CONSTRAINT FK_TAIKHOAN_DOAN FOREIGN KEY (MaDoan) REFERENCES DOAN(MaDoan)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
);
GO

CREATE TABLE GIAO_DICH (
    MaDoan CHAR(4) NOT NULL PRIMARY KEY,
    ThoiGianBatDau DATETIME NOT NULL,
    ThoiGianKetThuc DATETIME NOT NULL,
    SoPhong INT DEFAULT 1,
    TongTien DECIMAL(18,2) DEFAULT 50,
    CONSTRAINT FK_GIAODICH_DOAN FOREIGN KEY (MaDoan) REFERENCES DOAN(MaDoan)
	ON DELETE CASCADE
    ON UPDATE CASCADE,
    CONSTRAINT CHK_GIAODICH_TG CHECK (ThoiGianKetThuc > ThoiGianBatDau)
);
GO

CREATE TABLE CTGD (
	MaDoan CHAR(4) NOT NULL,
	MaKH INT NOT NULL,
    MaCTGD CHAR(6) NOT NULL PRIMARY KEY,    
    MaPhong CHAR(4) NOT NULL,
    ThoiGianNhanPhong DATETIME NOT NULL,
    ThoiGianTraPhong DATETIME NOT NULL,
	ThoiGianThucHien DATETIME NOT NULL,
    ThanhTien DECIMAL(18,2) DEFAULT 50,
	TrangThai NVARCHAR(50) CHECK (TrangThai IN (N'Chưa nhận phòng', N'Chưa trả phòng', N'Yêu cầu trả phòng', N'Đã trả phòng', N'Đã hủy')),
    CONSTRAINT FK_CTGD_PHONG FOREIGN KEY (MaPhong) REFERENCES PHONG(MaPhong),
    CONSTRAINT FK_CTGD_GD FOREIGN KEY (MaDoan) REFERENCES GIAO_DICH(MaDoan),
    CONSTRAINT FK_CTGD_KH FOREIGN KEY (MaKH, MaDoan) REFERENCES KHACH_HANG(MaKH, MaDoan)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
    CONSTRAINT CHK_CTGD_TG CHECK (ThoiGianNhanPhong < ThoiGianTraPhong AND ThoiGianThucHien <= ThoiGianNhanPhong),
	CONSTRAINT UQ_CTGD_MaDoan_MaKH UNIQUE (MaDoan, MaKH)
);
GO

CREATE TABLE THETU (
    MaTheTu CHAR(6) NOT NULL PRIMARY KEY,    
    MaPhong CHAR(4) NOT NULL,
    MaCTGD CHAR(6) NOT NULL UNIQUE,
    TrangThaiThe NVARCHAR(20) DEFAULT N'Đã kích hoạt',
    CONSTRAINT FK_THETU_PHONG FOREIGN KEY (MaPhong) REFERENCES PHONG(MaPhong),
    CONSTRAINT FK_THETU_CTGD FOREIGN KEY (MaCTGD) REFERENCES CTGD(MaCTGD)
	ON DELETE CASCADE
    ON UPDATE CASCADE
);
GO

CREATE TABLE BOI_THUONG (
    MaBoiThuong CHAR(4) CONSTRAINT PK_BT PRIMARY KEY,  -- BT01, BT02, ...
    MoTa NVARCHAR(100) NOT NULL,
    DonGia DECIMAL(18,2) NOT NULL CHECK (DonGia > 0)
);
GO

CREATE TABLE CT_BOI_THUONG (
    MaBoiThuong CHAR(4) NOT NULL,
    MaCTGD CHAR(6) NOT NULL,
	SoLuong INT DEFAULT 1
    CONSTRAINT PK_CTBOITHUONG PRIMARY KEY (MaBoiThuong, MaCTGD),
    CONSTRAINT FK_CTBT_BOITHUONG FOREIGN KEY (MaBoiThuong) REFERENCES BOI_THUONG(MaBoiThuong),
    CONSTRAINT FK_CTBT_CTGD FOREIGN KEY (MaCTGD) REFERENCES CTGD(MaCTGD)
	ON DELETE CASCADE
    ON UPDATE CASCADE
);
GO

ALTER TABLE DOAN
ADD CONSTRAINT FK_DOAN_KH
    FOREIGN KEY (NguoiDaiDien, MaDoan)
    REFERENCES KHACH_HANG(MaKH, MaDoan);
GO

-- =============================================
-- 2. DANH SÁCH CÁC TRIGGER
-- =============================================

--Trigger tự động sinh mã đoàn
CREATE OR ALTER TRIGGER TRG_AUTO_MADOAN
ON DOAN
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @MaxNum INT;
    SELECT @MaxNum = ISNULL(MAX(CAST(SUBSTRING(MaDoan, 2, 3) AS INT)), 0) FROM DOAN;

    INSERT INTO DOAN (MaDoan, SoNguoi, NguoiDaiDien)
    SELECT 'D' + RIGHT('000' + CAST(@MaxNum + ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS VARCHAR(3)), 3), I.SoNguoi, I.NguoiDaiDien
    FROM INSERTED I;
END;
GO


--Trigger makh hợp lệ
CREATE TRIGGER TRG_KHACHHANG_MaKH_Valid
ON KHACH_HANG
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra xem MaKH có nằm trong khoảng [1, SoNguoi] của đoàn hay không
    IF EXISTS (
        SELECT 1
        FROM INSERTED I
        JOIN DOAN D ON I.MaDoan = D.MaDoan
        WHERE I.MaKH < 1 OR I.MaKH > D.SoNguoi
    )
    BEGIN
        RAISERROR(N'Mã KH phải nằm trong khoảng [1, Số người của đoàn].', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO	

-- Đồng bộ số người trong đoàn vài số người dùng tài khoản khi thêm/xóa/sửa CTGD.
CREATE OR ALTER TRIGGER TRG_CTGD_CapNhatSoNguoi
ON CTGD
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

	-- Xóa đoàn không còn người
    DELETE D
    FROM DOAN D
    WHERE D.MaDoan NOT IN (SELECT MaDoan FROM CTGD);

    -- Cập nhật số người trong đoàn
    UPDATE D
    SET D.SoNguoi = (
        SELECT COUNT(*) 
        FROM CTGD C 
        WHERE C.MaDoan = D.MaDoan
    )
    FROM DOAN D
    WHERE D.MaDoan IN (
        SELECT MaDoan FROM INSERTED
        UNION
        SELECT MaDoan FROM DELETED
    );

	-- Cập nhật số người dùng trên tài khoản
    UPDATE TK
    SET TK.SoNguoiDung = D.SoNguoi
    FROM TAI_KHOAN TK
    JOIN DOAN D ON D.MaDoan = TK.MaDoan;
END;
GO

/*DROP TRIGGER TRG_CTGD_XoaKhachHangKhongConTonTai
-- Xóa khách hàng không còn trong CTGD 
CREATE OR ALTER TRIGGER TRG_CTGD_XoaKhachHangKhongConTonTai
ON CTGD
AFTER DELETE, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

	-- CẬP NHẬT NGƯỜI ĐẠI DIỆN
    UPDATE D
    SET NguoiDaiDien = (
        SELECT TOP 1 C.MaKH
        FROM CTGD C
        WHERE C.MaDoan = D.MaDoan
        ORDER BY C.MaKH
    )
    FROM DOAN D
    WHERE D.NguoiDaiDien IN (SELECT MaKH FROM DELETED)
      AND D.MaDoan IN (SELECT MaDoan FROM DELETED);

    IF NOT EXISTS (SELECT 1 FROM INSERTED)
    BEGIN
        DELETE KH
        FROM KHACH_HANG KH
        JOIN DELETED D ON KH.MaKH = D.MaKH AND KH.MaDoan = D.MaDoan;

    END;
END;
GO */

/*
-- Chỉ được thêm, xóa, sửa ctgd trước thời gian nhận phòng
CREATE OR ALTER TRIGGER TRG_CTGD_KiemTraThoiGianTruocNhanPhong
ON CTGD
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Now DATETIME = GETDATE();

    -- Nếu INSERT hoặc UPDATE: kiểm tra ThoiGianNhanPhong mới
    IF EXISTS (
        SELECT 1
        FROM inserted I
        WHERE @Now >= I.ThoiGianNhanPhong
    )
    BEGIN
        RAISERROR(N'Không được thêm hoặc sửa chi tiết giao dịch sau thời gian nhận phòng.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END;

    -- Nếu DELETE: kiểm tra ThoiGianNhanPhong cũ
    IF EXISTS (
        SELECT 1
        FROM deleted D
        WHERE @Now >= D.ThoiGianNhanPhong
    )
    BEGIN
        RAISERROR(N'Không được xóa chi tiết giao dịch sau thời gian nhận phòng.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END;
END;
GO */

-- Kiểm tra mã phòng và mã tầng đồng bộ
CREATE OR ALTER TRIGGER TRG_PHONG_KiemTraMaPhongHopLe
ON PHONG
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra: Nếu tồn tại dòng nào mà 2 ký tự đầu của Mã Phòng KHÁC Mã Tầng
    IF EXISTS (
        SELECT 1 
        FROM INSERTED 
        WHERE LEFT(MaPhong, 2) <> MaTang
    )
    BEGIN
        -- Báo lỗi và hủy thao tác
        RAISERROR(N'Lỗi logic: 2 ký tự đầu của Mã phòng phải trùng khớp với Mã tầng (Ví dụ: Phòng 0101 phải thuộc tầng 01).', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

--Trigger tính giá phòng: giá cơ bản 50k/h
CREATE TRIGGER TRG_PHONG_TinhGia
ON PHONG
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @GiaCoBan DECIMAL(12,2) = 50; -- giá cơ bản (nghìn VNĐ)

    UPDATE P
    SET P.GiaPhong = @GiaCoBan * HP.HeSoGiaHP * HT.HeSoGiaHT
    FROM PHONG P
    JOIN INSERTED I ON P.MaPhong = I.MaPhong
    JOIN HANG_PHONG HP ON I.LoaiHang = HP.LoaiHang
    JOIN HINH_THUC_PHONG HT ON I.TenHinhThuc = HT.TenHinhThuc;
END;
GO

-- Tính lại giá phòng khi cập nhật hệ số
CREATE OR ALTER TRIGGER TRG_HANGPHONG_UpdateGia
ON HANG_PHONG
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @GiaCoBan DECIMAL(12,2) = 50;

    -- Nếu có sự thay đổi về Hệ số giá
    IF UPDATE(HeSoGiaHP)
    BEGIN
        -- Cập nhật lại giá cho TẤT CẢ các phòng thuộc hạng phòng vừa sửa
        UPDATE P
        SET P.GiaPhong = @GiaCoBan * I.HeSoGiaHP * HT.HeSoGiaHT
        FROM PHONG P
        JOIN INSERTED I ON P.LoaiHang = I.LoaiHang -- Lấy hệ số mới từ bảng Inserted
        JOIN HINH_THUC_PHONG HT ON P.TenHinhThuc = HT.TenHinhThuc;
    END
END;
GO

-- Tính lại giá phòng khi cập nhật hệ số
CREATE OR ALTER TRIGGER TRG_HINHTHUCPHONG_UpdateGia
ON HINH_THUC_PHONG
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @GiaCoBan DECIMAL(12,2) = 50;

    -- Nếu có sự thay đổi về Hệ số giá hình thức
    IF UPDATE(HeSoGiaHT)
    BEGIN
        -- Cập nhật lại giá cho TẤT CẢ các phòng thuộc hình thức vừa sửa
        UPDATE P
        SET P.GiaPhong = @GiaCoBan * HP.HeSoGiaHP * I.HeSoGiaHT
        FROM PHONG P
        JOIN INSERTED I ON P.TenHinhThuc = I.TenHinhThuc -- Lấy hệ số mới
        JOIN HANG_PHONG HP ON P.LoaiHang = HP.LoaiHang;
    END
END;
GO

--Tính tiền bồi thường cho một chi tiết giao dịch
CREATE OR ALTER FUNCTION FUNC_TinhTongBoiThuong (@MaCTGD CHAR(6)) 
RETURNS DECIMAL(18, 2) -- Trả về tổng tiền
AS
BEGIN
    DECLARE @TongTien DECIMAL(18, 2);

    SELECT @TongTien = ISNULL(SUM(BT.DonGia * CTBT.SoLuong), 0)
    FROM CT_BOI_THUONG CTBT
    JOIN BOI_THUONG BT ON CTBT.MaBoiThuong = BT.MaBoiThuong
    WHERE CTBT.MaCTGD = @MaCTGD;

    RETURN @TongTien;
END;
GO 

--Tính giá thành tiền ở ctgd: giờ * giá + tổng bồi thường
CREATE OR ALTER TRIGGER TRG_CTGD_TinhThanhTien
ON CTGD
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH AffectedGroups AS (
        SELECT DISTINCT MaDoan, MaPhong
        FROM INSERTED
        UNION
        SELECT DISTINCT MaDoan, MaPhong
        FROM DELETED
    ),
    GroupCounts AS (
        SELECT C.MaDoan, C.MaPhong, COUNT(DISTINCT C.MaKH) AS SoNguoi
        FROM CTGD C
        JOIN AffectedGroups AG ON C.MaDoan = AG.MaDoan
        GROUP BY C.MaDoan, C.MaPhong
    )
    UPDATE C
    SET C.ThanhTien = ROUND((P.GiaPhong * CEILING(DATEDIFF(HOUR, C.ThoiGianNhanPhong, C.ThoiGianTraPhong))) / GC.SoNguoi
                      + dbo.FUNC_TinhTongBoiThuong(C.MaCTGD), 2)
    FROM CTGD C
    JOIN PHONG P ON C.MaPhong = P.MaPhong
    JOIN GroupCounts GC ON C.MaDoan = GC.MaDoan AND C.MaPhong = GC.MaPhong;
END;
GO

-- Tính tiền bồi thường cho mỗi ctgd sau khi update ct bồi thường
CREATE OR ALTER TRIGGER TRIG_CTBT_Update_CTGD
ON CT_BOI_THUONG
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE C
    SET C.ThanhTien = ROUND((P.GiaPhong * CEILING(DATEDIFF(HOUR, C.ThoiGianNhanPhong, C.ThoiGianTraPhong))) / GC.SoNguoi 
	                  + dbo.FUNC_TinhTongBoiThuong(C.MaCTGD), 2)
    FROM CTGD C
    JOIN PHONG P ON C.MaPhong = P.MaPhong
    JOIN (SELECT MaCTGD
          FROM INSERTED
          UNION
          SELECT MaCTGD
          FROM DELETED
    ) AS A ON A.MaCTGD = C.MaCTGD
    JOIN (SELECT C2.MaDoan, C2.MaPhong, COUNT(DISTINCT C2.MaKH) AS SoNguoi
          FROM CTGD C2
          GROUP BY C2.MaDoan, C2.MaPhong
    ) AS GC ON GC.MaDoan = C.MaDoan AND GC.MaPhong = C.MaPhong;
END;
GO


--Tính tổng tiền ở giao dịch = tổng các thành tiền
CREATE OR ALTER TRIGGER TRG_CTGD_TinhTongTien
ON CTGD
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH AffectedDoan AS (
        SELECT MaDoan FROM INSERTED
        UNION
        SELECT MaDoan FROM DELETED
    )

    UPDATE GD
    SET GD.TongTien = ISNULL((
        SELECT SUM(C.ThanhTien)
        FROM CTGD C
        WHERE C.MaDoan = GD.MaDoan
    ), 0)
    FROM GIAO_DICH GD
    JOIN AffectedDoan A ON GD.MaDoan = A.MaDoan;

END;
GO

--Tính số phòng trong mỗi giao dịch
CREATE OR ALTER TRIGGER TRG_CTGD_CapNhatSoPhong
ON CTGD
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH AffectedGD AS (
        SELECT MaDoan FROM INSERTED
        UNION
        SELECT MaDoan FROM DELETED
    )

    UPDATE G
    SET G.SoPhong = (
        SELECT COUNT(DISTINCT C.MaPhong) 
        FROM CTGD C 
        WHERE C.MaDoan = G.MaDoan
    )
    FROM GIAO_DICH G
    JOIN AffectedGD A ON G.MaDoan = A.MaDoan;
END;
GO

/*-- Kiểm tra thời gian giao dịch hợp lệ: khi thêm giao dịch phải trong tương lai
CREATE OR ALTER TRIGGER TRG_GIAODICH_KiemTraThoiGianHopLe
ON GIAO_DICH
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1 
        FROM INSERTED I
        WHERE I.ThoiGianBatDau < GETDATE()
    )
    BEGIN
        RAISERROR(N'Ngày giao dịch phải nằm trong tương lai hoặc hôm nay.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END;
END;
GO */

-- Tự động sinh mã CTGD và kiểm tra phòng và kiểm tra phòng đăng bận

CREATE OR ALTER TRIGGER TRG_CTGD_TuSinhMa_KiemTraPhong
ON CTGD
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaxNum INT;
    SELECT @MaxNum = ISNULL(MAX(CAST(SUBSTRING(MaCTGD, 3, 4) AS INT)), 0) FROM CTGD;

    -- 1. Kiểm tra chồng lấn thời gian với ĐOÀN KHÁC
    IF EXISTS (
        SELECT 1 FROM INSERTED I 
        JOIN CTGD C ON I.MaPhong = C.MaPhong
        WHERE C.MaDoan <> I.MaDoan -- Khác đoàn mới check
          AND C.TrangThai NOT IN (N'Đã hủy', N'Đã trả phòng')
          AND (I.ThoiGianNhanPhong < C.ThoiGianTraPhong AND I.ThoiGianTraPhong > C.ThoiGianNhanPhong)
    )
    BEGIN
        RAISERROR(N'Phòng đang bận (trùng lịch với đoàn khác).', 16, 1);
        RETURN;
    END;

    -- 2. Kiểm tra Phòng đang có khách (Nhưng phải loại trừ trường hợp CÙNG ĐOÀN ở ghép)
    IF EXISTS (
        SELECT 1 
        FROM INSERTED I 
        JOIN PHONG P ON I.MaPhong = P.MaPhong
        WHERE P.TrangThai = N'Đang có khách'
        -- Chỉ báo lỗi nếu phòng đó đang chứa khách của ĐOÀN KHÁC
        AND EXISTS (
            SELECT 1 FROM CTGD Existing 
            WHERE Existing.MaPhong = I.MaPhong 
            AND Existing.TrangThai IN (N'Chưa trả phòng', N'Yêu cầu trả phòng')
            AND Existing.MaDoan <> I.MaDoan -- Nếu cùng đoàn thì cho qua
        )
    )
    BEGIN
        RAISERROR(N'Phòng hiện đang có khách (không thể đặt).', 16, 1);
        RETURN;
    END

    -- 3. Kiểm tra sức chứa (Phòng đã đầy chưa?)
    IF EXISTS (
        SELECT 1 FROM INSERTED I 
        JOIN PHONG P ON I.MaPhong = P.MaPhong 
        JOIN HINH_THUC_PHONG H ON P.TenHinhThuc = H.TenHinhThuc
        WHERE (
            SELECT COUNT(C.MaKH) 
            FROM CTGD C 
            WHERE C.MaPhong = I.MaPhong 
            AND C.MaDoan = I.MaDoan -- Đếm số người cùng đoàn trong phòng này
        ) >= H.SoNguoiToiDa
    )
    BEGIN
        RAISERROR(N'Phòng đã đầy (vượt quá sức chứa).', 16, 1);
        RETURN;
    END;

    -- Insert
    INSERT INTO CTGD (MaCTGD, MaDoan, MaKH, MaPhong, ThoiGianNhanPhong, ThoiGianTraPhong, ThoiGianThucHien, TrangThai)
    SELECT ISNULL(I.MaCTGD, 'CT' + RIGHT('0000' + CAST(@MaxNum + ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS VARCHAR(4)), 4)),
        I.MaDoan, I.MaKH, I.MaPhong, I.ThoiGianNhanPhong, I.ThoiGianTraPhong, I.ThoiGianThucHien, I.TrangThai         
    FROM INSERTED I;
END;
GO

--Tự sinh mã thẻ từ
CREATE OR ALTER TRIGGER TRG_THETU_TuSinhMa
ON THETU
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaxNum INT;
    SELECT @MaxNum = ISNULL(MAX(CAST(SUBSTRING(MaTheTu, 3, 4) AS INT)), 0)
    FROM THETU;

    INSERT INTO THETU (MaTheTu, MaPhong, MaCTGD)
    SELECT 'TT' + RIGHT('0000' + CAST(@MaxNum + ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS VARCHAR(4)), 4) AS MaTheTu, I.MaPhong, I.MaCTGD
    FROM INSERTED I
	WHERE NOT EXISTS (
        SELECT 1
        FROM THETU T
        WHERE T.MaCTGD = I.MaCTGD   
    );
END;
GO


--Tự động sinh mã bồi thường
CREATE OR ALTER TRIGGER TRG_BOITHUONG_TuSinhMa
ON BOI_THUONG
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;

	DECLARE @MaxNum INT
	SELECT @MaxNum = ISNULL(MAX(CAST(SUBSTRING(MaBoiThuong, 3, 2) AS INT)), 0)
	FROM BOI_THUONG

    INSERT INTO BOI_THUONG (MaBoiThuong, MoTa, DonGia)
    SELECT ISNULL(I.MaBoiThuong,'BT' + RIGHT('00' + CAST(@MaxNum + ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS VARCHAR(2)), 2)) AS MaBoiThuong,
        I.MoTa, I.DonGia
    FROM INSERTED I;
END;
GO


--Trigger khi khách hàng cùng 1 đoàn đăng kí ở chung 1 phòng thì thời gian
--thực hiện có thể khác nhau nhưng thời gian nhận trả phòng phải giống nhau
CREATE OR ALTER TRIGGER TRG_CTGD_KiemTra_ThoiGianOChung
ON CTGD
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM CTGD C1
        JOIN CTGD C2 
            ON C1.MaDoan = C2.MaDoan
            AND C1.MaPhong = C2.MaPhong
            AND C1.MaCTGD <> C2.MaCTGD  
        JOIN INSERTED I ON I.MaCTGD = C1.MaCTGD
        WHERE 
            -- So sánh chênh lệch theo phút. Nếu chênh lệch > 0 thì mới báo lỗi.
            ABS(DATEDIFF(SECOND, C1.ThoiGianNhanPhong, C2.ThoiGianNhanPhong)) > 59
            OR 
            ABS(DATEDIFF(SECOND, C1.ThoiGianTraPhong, C2.ThoiGianTraPhong)) > 59
    )
    BEGIN
        RAISERROR(N'Lỗi: Khách ở chung phòng phải có thời gian nhận/trả khớp đến từng phút.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

-- Trigger tự động cập nhật trạng thái phòng
CREATE OR ALTER TRIGGER TRG_CTGD_CapNhatTrangThaiPhong
ON CTGD
AFTER INSERT, UPDATE, DELETE 
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @CurrentTime DATETIME = GETDATE();

    -- Lấy danh sách các phòng bị ảnh hưởng
    ;WITH AffectedRooms AS (
        SELECT DISTINCT MaPhong FROM INSERTED 
        UNION 
        SELECT DISTINCT MaPhong FROM DELETED
    )
    UPDATE P
    SET P.TrangThai = 
        CASE 
            -- 1. ĐANG CÓ KHÁCH:
            -- Điều kiện: Có đơn (Chưa trả/Yêu cầu trả) VÀ Thời gian hiện tại >= Thời gian nhận
            WHEN EXISTS (
                SELECT 1 FROM CTGD C 
                WHERE C.MaPhong = P.MaPhong 
                  AND C.TrangThai IN (N'Chưa trả phòng', N'Yêu cầu trả phòng')
                  AND @CurrentTime >= C.ThoiGianNhanPhong -- Quan trọng: Đã đến giờ nhận phòng
            ) THEN N'Đang có khách'
            
            -- 2. ĐÃ ĐẶT (Sắp đến):
            -- Chỉ hiện trạng thái này nếu khách sắp đến TRONG HÔM NAY mà chưa nhận phòng
            -- (Để lễ tân biết đường chuẩn bị, còn đặt ngày mai/kia thì vẫn coi là Trống để bán tiếp)
            WHEN EXISTS (
                SELECT 1 FROM CTGD C 
                WHERE C.MaPhong = P.MaPhong 
                  AND C.TrangThai = N'Chưa nhận phòng'
                  AND CAST(@CurrentTime AS DATE) = CAST(C.ThoiGianNhanPhong AS DATE) -- Chỉ xét trong ngày hôm nay
            ) THEN N'Đã đặt trước'
            
            -- 3. CÒN LẠI: Trống (Bao gồm cả việc có khách đặt cho tuần sau, thì giờ vẫn là trống)
            ELSE N'Đang trống'
        END
    FROM PHONG P 
    JOIN AffectedRooms AR ON P.MaPhong = AR.MaPhong;
END;
GO

-- Trigger cập nhật số lượng phòng trên mỗi tầng
CREATE OR ALTER TRIGGER TRG_PHONG_CapNhatSoPhongCuaTang
ON PHONG
AFTER INSERT, DELETE, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Tìm danh sách các Mã Tầng bị ảnh hưởng
    -- (Lấy từ cả bảng INSERTED và DELETED đề phòng trường hợp sửa đổi tầng của phòng)
    ;WITH CacTangBiAnhHuong AS (
        SELECT MaTang FROM INSERTED WHERE MaTang IS NOT NULL
        UNION
        SELECT MaTang FROM DELETED WHERE MaTang IS NOT NULL
    )
    
    -- 2. Cập nhật lại cột SoPhong trong bảng TANG
    UPDATE T
    SET T.SoPhong = (
        SELECT COUNT(*) 
        FROM PHONG P 
        WHERE P.MaTang = T.MaTang
    )
    FROM TANG T
    JOIN CacTangBiAnhHuong A ON T.MaTang = A.MaTang;
END;
GO

--Trigger cập nhật trạng thái thẻ từ
CREATE OR ALTER TRIGGER TRG_CTGD_QuanLyTheTu
ON CTGD
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

	DELETE T
	FROM THETU T
	JOIN DELETED D ON D.MaCTGD = T.MaCTGD

    -- Tạo thẻ mới khi có CTGD mới hoặc đổi phòng
    INSERT INTO THETU (MaPhong, MaCTGD)
    SELECT DISTINCT I.MaPhong, I.MaCTGD
    FROM INSERTED I;

    -- Cập nhật hiệu lực thẻ
    UPDATE T
    SET T.TrangThaiThe =
        CASE 
            WHEN GETDATE() BETWEEN C.ThoiGianNhanPhong AND C.ThoiGianTraPhong 
                THEN N'Đã kích hoạt'
			WHEN GETDATE() < C.ThoiGianNhanPhong
			    THEN N'Chưa kích hoạt'
            ELSE N'Hết hiệu lực'
        END
    FROM THETU T
    JOIN CTGD C ON C.MaCTGD = T.MaCTGD
    WHERE C.MaCTGD IN (
        SELECT MaCTGD FROM INSERTED
        UNION
        SELECT MaCTGD FROM DELETED
    );
END;
GO

---	Khoảng thời gian nhận phòng - trả phòng của mỗi chi tiết giao dịch 
-- phải nằm trong khoảng thời gian bắt đầu - kết thúc của giao dịch tương ứng.
CREATE OR ALTER TRIGGER TRG_CTGD_THOIGIANNHANTRA
ON CTGD
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM INSERTED I
		JOIN GIAO_DICH GD ON GD.MaDoan = I.MaDoan 
        WHERE 
            (I.ThoiGianNhanPhong < GD.ThoiGianBatDau 
             OR I.ThoiGianTraPhong > GD.ThoiGianKetThuc)
    )
    BEGIN
        RAISERROR(N'Chi tiết giao dịch có thời gian nhận và trả phòng không phù hợp.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

END;
GO

-- =============================================
-- 3. INSERT DỮ LIỆU CỐ ĐỊNH
-- =============================================

INSERT INTO TANG (MaTang, SoPhong) VALUES 
('01', 15),
('02', 16),
('03', 18),
('04', 20), 
('05', 15),
('06', 17),
('07', 16),
('08', 19),
('09', 20),
('10', 18),
('11', 15),
('12', 16),
('13', 15);
GO

INSERT INTO HANG_PHONG (LoaiHang, HeSoGiaHP)
VALUES 
(N'Thường', 1.0),
(N'Trung bình', 1.2),
(N'Sang', 1.5),
(N'Rất sang', 1.8),
(N'VIP', 2.5);
GO

INSERT INTO HINH_THUC_PHONG (TenHinhThuc, SoNguoiToiDa, HeSoGiaHT)
VALUES
(N'1 giường đơn', 1, 1.0),
(N'1 giường đôi', 2, 1.2),
(N'2 giường đơn', 2, 1.4),
(N'2 giường đôi', 4, 1.6);
GO

INSERT INTO BOI_THUONG (MoTa, DonGia)
VALUES
(N'Vỡ bình hoa', 2500),
(N'Làm hư TV', 1500),
(N'Bể ly', 50),
(N'Mất khăn tắm', 80),
(N'Hư điều hoà', 2500),
(N'Bể gương', 1200);
GO

DECLARE @MaTang CHAR(2);
DECLARE @SoPhongToiDa INT;
DECLARE @i INT;
DECLARE @MaPhong CHAR(4);
DECLARE @LoaiHang NVARCHAR(30);
DECLARE @TenHinhThuc NVARCHAR(50);

-- Khai báo con trỏ để duyệt qua từng tầng đã tạo ở bảng TANG
DECLARE cur_Tang CURSOR FOR 
SELECT MaTang, SoPhong FROM TANG;

OPEN cur_Tang;
FETCH NEXT FROM cur_Tang INTO @MaTang, @SoPhongToiDa;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @i = 1;
    WHILE @i <= @SoPhongToiDa
    BEGIN
        -- Tạo Mã Phòng: Ghép Mã Tầng + Số thứ tự (ví dụ: 01 + 01 = 0101)
        SET @MaPhong = @MaTang + RIGHT('00' + CAST(@i AS VARCHAR(2)), 2);

        -- LOGIC RANDOM HẠNG PHÒNG THEO TẦNG CHO THỰC TẾ
        IF CAST(@MaTang AS INT) <= 4 -- Tầng 1-4: Bình dân
        BEGIN
            IF @i % 3 = 0 SET @LoaiHang = N'Sang';
            ELSE IF @i % 2 = 0 SET @LoaiHang = N'Trung bình';
            ELSE SET @LoaiHang = N'Thường';
        END
        ELSE IF CAST(@MaTang AS INT) <= 9 -- Tầng 5-9: Cao cấp
        BEGIN
            IF @i % 4 = 0 SET @LoaiHang = N'VIP';
            ELSE IF @i % 2 = 0 SET @LoaiHang = N'Rất sang';
            ELSE SET @LoaiHang = N'Sang';
        END
        ELSE -- Tầng 10-13: VIP
        BEGIN
            IF @i % 2 = 0 SET @LoaiHang = N'VIP';
            ELSE SET @LoaiHang = N'Rất sang';
        END

        -- LOGIC RANDOM HÌNH THỨC PHÒNG
            -- Random các loại giường khác
            IF @i % 4 = 0 SET @TenHinhThuc = N'2 giường đôi';
            ELSE IF @i % 3 = 0 SET @TenHinhThuc = N'2 giường đơn';
            ELSE IF @i % 2 = 0 SET @TenHinhThuc = N'1 giường đôi';
            ELSE SET @TenHinhThuc = N'1 giường đơn';

        -- Thực hiện Insert
        INSERT INTO PHONG (MaPhong, MaTang, LoaiHang, TenHinhThuc, GiaPhong, TrangThai)
        VALUES (@MaPhong, @MaTang, @LoaiHang, @TenHinhThuc, 0, N'Đang trống'); 
        -- Giá phòng = 0 để Trigger tự tính lại sau

        SET @i = @i + 1;
    END

    FETCH NEXT FROM cur_Tang INTO @MaTang, @SoPhongToiDa;
END;

CLOSE cur_Tang;
DEALLOCATE cur_Tang;
GO

-- Kích hoạt Trigger để tính lại giá tiền cho tất cả các phòng vừa thêm
UPDATE PHONG SET GiaPhong = 0; 
GO


-- 2. BIẾN CẤU HÌNH VÒNG LẶP
DECLARE @i INT = 1;             -- Biến chạy số đoàn (1 -> 20)
DECLARE @SoDoan INT = 20;       -- Tổng số đoàn muốn tạo
DECLARE @MaDoanCurrent CHAR(4); -- Mã đoàn hiện tại vừa sinh
DECLARE @SoNguoi INT;    -- Số người trong đoàn (3-7)
DECLARE @SoPhongCanDat INT;     -- Số phòng đoàn đặt (3-4)
DECLARE @TrangThaiGD NVARCHAR(50); -- Trạng thái mong muốn
DECLARE @NgayBatDau DATETIME;
DECLARE @NgayKetThuc DATETIME;
DECLARE @NgayNhan DATETIME;
DECLARE @NgayTra DATETIME;
DECLARE @NgayThucHien DATETIME;

-- Biến hỗ trợ chia phòng
DECLARE @PhongBatDau INT = 0; -- Con trỏ để lấy phòng không trùng
DECLARE @j INT; -- Biến chạy khách hàng
DECLARE @k INT; -- Biến chạy phòng
DECLARE @MaPhongChon CHAR(4);
DECLARE @KhachHangHienTai INT;

-- BẮT ĐẦU VÒNG LẶP TẠO 20 ĐOÀN
DECLARE @SucChuaPhong INT; -- Biến mới để check sức chứa

WHILE @i <= @SoDoan
BEGIN
    SET @SoNguoi = 3 + CAST(RAND() * 5 AS INT); 
    SET @SoPhongCanDat = 3 + CAST(RAND() * 2 AS INT);

    -- Cấu hình thời gian (Giữ nguyên logic của bạn)
    IF @i = 1 BEGIN SET @TrangThaiGD = N'Đã trả phòng'; SET @NgayBatDau = DATEADD(DAY, -10, GETDATE()); SET @NgayKetThuc = DATEADD(DAY, -5, GETDATE()); SET @NgayNhan = @NgayBatDau; SET @NgayTra = @NgayKetThuc; SET @NgayThucHien = @NgayBatDau; END
    ELSE IF @i = 2 BEGIN SET @TrangThaiGD = N'Đã hủy'; SET @NgayBatDau = DATEADD(DAY, 2, GETDATE()); SET @NgayKetThuc = DATEADD(DAY, 5, GETDATE()); SET @NgayNhan = @NgayBatDau; SET @NgayTra = @NgayKetThuc; SET @NgayThucHien = GETDATE(); END
    ELSE IF @i >= 3 AND @i <= 15 BEGIN SET @TrangThaiGD = N'Yêu cầu trả phòng'; SET @NgayBatDau = DATEADD(DAY, -3, GETDATE()); SET @NgayKetThuc = DATEADD(DAY, 2, GETDATE()); SET @NgayNhan = @NgayBatDau; SET @NgayTra = DATEADD(DAY, 1, GETDATE()); SET @NgayThucHien = @NgayBatDau; END
    ELSE IF @i >= 16 AND @i <= 18 BEGIN SET @TrangThaiGD = N'Chưa trả phòng'; SET @NgayBatDau = DATEADD(DAY, -1, GETDATE()); SET @NgayKetThuc = DATEADD(DAY, 5, GETDATE()); SET @NgayNhan = @NgayBatDau; SET @NgayTra = DATEADD(DAY, 4, GETDATE()); SET @NgayThucHien = @NgayBatDau; END
    ELSE BEGIN SET @TrangThaiGD = N'Chưa nhận phòng'; SET @NgayBatDau = DATEADD(DAY, 10 + @i, GETDATE()); SET @NgayKetThuc = DATEADD(DAY, 15 + @i, GETDATE()); SET @NgayNhan = @NgayBatDau; SET @NgayTra = DATEADD(DAY, 4, @NgayNhan); SET @NgayThucHien = GETDATE(); END

    INSERT INTO DOAN (SoNguoi, NguoiDaiDien) VALUES (@SoNguoi, NULL);
    SELECT @MaDoanCurrent = MAX(MaDoan) FROM DOAN;

    -- Insert Khách hàng
    SET @j = 1;
    WHILE @j <= @SoNguoi BEGIN
        INSERT INTO KHACH_HANG (MaKH, MaDoan, SoCMND, TenKH, NgaySinh, SDT)
        VALUES (@j, @MaDoanCurrent, '0' + CAST(100000000 + @i*100 + @j AS VARCHAR(12)), N'Khách ' + CHAR(64+@j) + N' Đoàn ' + CAST(@i AS NVARCHAR(5)), DATEADD(YEAR, -20 - @j, GETDATE()), '090' + CAST(1000000 + @i*100 + @j AS VARCHAR(10)));
        SET @j = @j + 1;
    END
    UPDATE DOAN SET NguoiDaiDien = 1 WHERE MaDoan = @MaDoanCurrent;
    INSERT INTO TAI_KHOAN (MaDoan, TenDangNhap, MatKhau) VALUES (@MaDoanCurrent, @MaDoanCurrent , '123');
    INSERT INTO GIAO_DICH (MaDoan, ThoiGianBatDau, ThoiGianKetThuc) VALUES (@MaDoanCurrent, @NgayBatDau, @NgayKetThuc);

    -- PHÂN BỔ PHÒNG (Đã sửa logic check sức chứa)
    DECLARE cur_Phong CURSOR FOR SELECT MaPhong FROM PHONG ORDER BY MaPhong OFFSET @PhongBatDau ROWS FETCH NEXT @SoPhongCanDat ROWS ONLY;
    OPEN cur_Phong;
    
    SET @KhachHangHienTai = 1;
    FETCH NEXT FROM cur_Phong INTO @MaPhongChon;
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- Lấy sức chứa tối đa của phòng này
        SELECT @SucChuaPhong = H.SoNguoiToiDa 
        FROM PHONG P JOIN HINH_THUC_PHONG H ON P.TenHinhThuc = H.TenHinhThuc 
        WHERE P.MaPhong = @MaPhongChon;

        -- Luôn nhét ít nhất 1 người
        IF @KhachHangHienTai <= @SoNguoi
        BEGIN
            INSERT INTO CTGD (MaCTGD, MaDoan, MaKH, MaPhong, ThoiGianNhanPhong, ThoiGianTraPhong, ThoiGianThucHien, TrangThai)
            VALUES (NULL, @MaDoanCurrent, @KhachHangHienTai, @MaPhongChon, @NgayNhan, @NgayTra, @NgayThucHien, @TrangThaiGD);
            SET @KhachHangHienTai = @KhachHangHienTai + 1;
        END
        
        -- Nhét tiếp người thứ 2, 3... nếu còn chỗ (Check < @SucChuaPhong)
        WHILE @KhachHangHienTai <= @SoNguoi 
              AND (SELECT COUNT(*) FROM CTGD WHERE MaDoan = @MaDoanCurrent AND MaPhong = @MaPhongChon) < @SucChuaPhong
        BEGIN
             INSERT INTO CTGD (MaCTGD, MaDoan, MaKH, MaPhong, ThoiGianNhanPhong, ThoiGianTraPhong, ThoiGianThucHien, TrangThai)
            VALUES (NULL, @MaDoanCurrent, @KhachHangHienTai, @MaPhongChon, @NgayNhan, @NgayTra, @NgayThucHien, @TrangThaiGD);
            SET @KhachHangHienTai = @KhachHangHienTai + 1;
        END
        FETCH NEXT FROM cur_Phong INTO @MaPhongChon;
    END
    CLOSE cur_Phong; DEALLOCATE cur_Phong;

    SET @PhongBatDau = @PhongBatDau + @SoPhongCanDat;

    -- BỒI THƯỜNG
    IF @TrangThaiGD IN (N'Đã trả phòng', N'Yêu cầu trả phòng') AND (@i % 2 = 0)
    BEGIN
        DECLARE @MaCTGD_Random CHAR(6);
        SELECT TOP 1 @MaCTGD_Random = MaCTGD FROM CTGD WHERE MaDoan = @MaDoanCurrent;
        INSERT INTO CT_BOI_THUONG (MaBoiThuong, MaCTGD, SoLuong) VALUES ('BT0' + CAST((1 + CAST(RAND()*5 AS INT)) AS VARCHAR(1)), @MaCTGD_Random, 1);
    END

    SET @i = @i + 1;
END;
GO
-- Update tiền
UPDATE CTGD SET ThanhTien = ThanhTien;
GO

-- =============================================
-- 4. DANH SÁCH CÁC STORED PROCEDURE CHO CÁC TÍNH NĂNG
-- =============================================

-- =============================================
-- Tình huống 15: Hai nhân viên cùng tra cứu một phòng 
-- và sau đó cả hai cùng muốn cập nhật thông tin phòng đó (T1 VÀ T2 NHƯ NHAU)

	-- Tình huống 15: CHƯA FIX
CREATE OR ALTER PROC sp_CapNhatThongTinPhong_LOI
    @MaPhong CHAR(4),
    @MaTangMoi CHAR(2) = NULL,      -- Giữ tham số để không lỗi code C#, nhưng sẽ không dùng update
    @HangPhongMoi NVARCHAR(30) = NULL,
    @HinhThucMoi NVARCHAR(50) = NULL
AS
BEGIN
    SET TRANSACTION ISOLATION LEVEL REPEATABLE READ;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Xin khóa S kiểm tra phòng tồn tại
        IF NOT EXISTS (SELECT 1 FROM PHONG WHERE MaPhong = @MaPhong)
        BEGIN
            ;THROW 50001, N'Mã phòng không tồn tại.', 1;
        END

        -- 2. Validate dữ liệu (Chỉ cần kiểm tra cái nào ĐƯỢC PHÉP sửa)
        -- (Bỏ qua kiểm tra Tầng vì không cho sửa Tầng nữa)

        -- Kiểm tra Hạng Phòng
        IF @HangPhongMoi IS NOT NULL AND NOT EXISTS (SELECT 1 FROM HANG_PHONG WHERE LoaiHang = @HangPhongMoi)
        BEGIN
            ;THROW 50003, N'Hạng phòng không tồn tại trong danh mục.', 1;
        END

        -- Kiểm tra Hình Thức Phòng
        IF @HinhThucMoi IS NOT NULL AND NOT EXISTS (SELECT 1 FROM HINH_THUC_PHONG WHERE TenHinhThuc = @HinhThucMoi)
        BEGIN
            ;THROW 50004, N'Hình thức phòng không tồn tại trong danh mục.', 1;
        END

        -- Giả lập xử lý lâu (10 giây)
        WAITFOR DELAY '00:00:10'; 

        -- 3. Cập nhật: CHỈ UPDATE LOẠI HANG VÀ HÌNH THỨC (Bỏ dòng MaTang)
        UPDATE PHONG
        SET 
            LoaiHang = ISNULL(@HangPhongMoi, LoaiHang),
            TenHinhThuc = ISNULL(@HinhThucMoi, TenHinhThuc)
            -- MaTang không được liệt kê ở đây => Không bao giờ bị đổi
        WHERE MaPhong = @MaPhong;

        COMMIT TRANSACTION;
        PRINT N'Cập nhật thành công!';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrMsg, 16, 1);
    END CATCH
END;
GO

	-- Tình huống 15: ĐÃ FIX LỖI
CREATE OR ALTER PROC sp_CapNhatThongTinPhong
    @MaPhong CHAR(4),
    @MaTangMoi CHAR(2) = NULL,      -- Giữ tham số để không lỗi code C#, nhưng sẽ không dùng update
    @HangPhongMoi NVARCHAR(30) = NULL,
    @HinhThucMoi NVARCHAR(50) = NULL
AS
BEGIN
    SET TRANSACTION ISOLATION LEVEL REPEATABLE READ;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Xin khóa U và kiểm tra phòng tồn tại
        IF NOT EXISTS (SELECT 1 FROM PHONG WITH (UPDLOCK) WHERE MaPhong = @MaPhong)
        BEGIN
            ;THROW 50001, N'Mã phòng không tồn tại.', 1;
        END

        -- 2. Validate dữ liệu (Chỉ cần kiểm tra cái nào ĐƯỢC PHÉP sửa)
        -- (Bỏ qua kiểm tra Tầng vì không cho sửa Tầng nữa)

        -- Kiểm tra Hạng Phòng
        IF @HangPhongMoi IS NOT NULL AND NOT EXISTS (SELECT 1 FROM HANG_PHONG WHERE LoaiHang = @HangPhongMoi)
        BEGIN
            ;THROW 50003, N'Hạng phòng không tồn tại trong danh mục.', 1;
        END

        -- Kiểm tra Hình Thức Phòng
        IF @HinhThucMoi IS NOT NULL AND NOT EXISTS (SELECT 1 FROM HINH_THUC_PHONG WHERE TenHinhThuc = @HinhThucMoi)
        BEGIN
            ;THROW 50004, N'Hình thức phòng không tồn tại trong danh mục.', 1;
        END

        -- Giả lập xử lý lâu (10 giây)
        WAITFOR DELAY '00:00:10'; 

        -- 3. Cập nhật: CHỈ UPDATE LOẠI HANG VÀ HÌNH THỨC (Bỏ dòng MaTang)
        UPDATE PHONG
        SET 
            LoaiHang = ISNULL(@HangPhongMoi, LoaiHang),
            TenHinhThuc = ISNULL(@HinhThucMoi, TenHinhThuc)
            -- MaTang không được liệt kê ở đây => Không bao giờ bị đổi
        WHERE MaPhong = @MaPhong;

        COMMIT TRANSACTION;
        PRINT N'Cập nhật thành công!';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrMsg, 16, 1);
    END CATCH
END;
GO
-- =============================================

-- Tình huống 12: KH thực hiện thanh toán cho CTGD. NV ghi nhận bồi thường cho CTGD đó.

	-- Tình huống 12: T1 - CHƯA FIX LỖI
CREATE OR ALTER PROC sp_ThanhToan_Phantom_LOI
    @MaDoan CHAR(4)
AS
BEGIN
    SET NOCOUNT ON;

	SET TRANSACTION ISOLATION LEVEL READ COMMITTED;  

    BEGIN TRY
        BEGIN TRAN;

        -- 1. Kiểm tra giao dịch tồn tại
        IF NOT EXISTS (SELECT 1 FROM GIAO_DICH WHERE MaDoan = @MaDoan)
        BEGIN
            RAISERROR (N'Đoàn %s chưa có giao dịch thanh toán.', 16, 1, @MaDoan);
            ROLLBACK TRAN;
            RETURN;
        END;

        PRINT N'Lần đọc 1 - trước khi nhân viên ghi nhận bồi thường:';

        SELECT 
            C.MaCTGD,
            C.MaPhong,
            CTBT.MaBoiThuong,
            C.ThanhTien,
            GD.TongTien
        FROM CTGD C
        JOIN GIAO_DICH GD      ON GD.MaDoan  = C.MaDoan
        LEFT JOIN CT_BOI_THUONG CTBT ON CTBT.MaCTGD = C.MaCTGD
        WHERE C.MaDoan = @MaDoan;

        -- Giữ transaction mở cho T2 chen thêm bồi thường
        WAITFOR DELAY '00:00:05';

        PRINT N'Lần đọc 2 - sau khi nhân viên ghi nhận bồi thường:';

        SELECT 
            C.MaCTGD,
            C.MaPhong,
            CTBT.MaBoiThuong,
            C.ThanhTien,
            GD.TongTien
        FROM CTGD C
        JOIN GIAO_DICH GD      ON GD.MaDoan  = C.MaDoan
        LEFT JOIN CT_BOI_THUONG CTBT ON CTBT.MaCTGD = C.MaCTGD
        WHERE C.MaDoan = @MaDoan;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH;
END;
GO 

	-- Tình huống 12: T2
CREATE OR ALTER PROC sp_GhiNhanBoiThuong_Phantom
    @MaCTGD CHAR(6),
    @MaBoiThuong CHAR(4),
    @SoLuong INT = 1 -- Tham số mới thêm vào
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        
        -- 1. Kiểm tra tồn tại
        IF NOT EXISTS (SELECT 1 FROM CTGD WHERE MaCTGD = @MaCTGD) 
        BEGIN 
            RAISERROR (N'Không tìm thấy chi tiết giao dịch %s.', 16, 1, @MaCTGD); 
            ROLLBACK TRAN; 
            RETURN; 
        END;

        IF NOT EXISTS (SELECT 1 FROM BOI_THUONG WHERE MaBoiThuong = @MaBoiThuong) 
        BEGIN 
            RAISERROR (N'Không tìm thấy loại bồi thường %s.', 16, 1, @MaBoiThuong); 
            ROLLBACK TRAN; 
            RETURN; 
        END;

        -- 2. Kiểm tra trùng lặp
        IF EXISTS (SELECT 1 FROM CT_BOI_THUONG WHERE MaCTGD = @MaCTGD AND MaBoiThuong = @MaBoiThuong) 
        BEGIN 
            RAISERROR (N'Đã tồn tại bồi thường này cho giao dịch. Vui lòng kiểm tra lại.', 16, 1); 
            ROLLBACK TRAN; 
            RETURN; 
        END;

        -- 3. Insert có tham số Số Lượng
        INSERT INTO CT_BOI_THUONG (MaBoiThuong, MaCTGD, SoLuong) 
        VALUES (@MaBoiThuong, @MaCTGD, @SoLuong);
        
        PRINT N'Ghi nhận bồi thường thành công';
        COMMIT TRAN;
    END TRY
    BEGIN CATCH 
        IF @@TRANCOUNT > 0 ROLLBACK TRAN; 
        THROW; 
    END CATCH;
END;
GO
	-- Tình huống 12: T1 - FIX LỖI
CREATE OR ALTER PROC sp_ThanhToan_Phantom
    @MaDoan CHAR(4)
AS
BEGIN
    SET NOCOUNT ON;

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;  

    BEGIN TRY
        BEGIN TRAN;

        -- 1. Kiểm tra giao dịch tồn tại
        IF NOT EXISTS (SELECT 1 FROM GIAO_DICH WHERE MaDoan = @MaDoan)
        BEGIN
            RAISERROR (N'Đoàn %s chưa có giao dịch thanh toán.', 16, 1, @MaDoan);
            ROLLBACK TRAN;
            RETURN;
        END;

        PRINT N'Lần đọc 1 - trước khi nhân viên ghi nhận bồi thường:';

        SELECT 
            C.MaCTGD,
            C.MaPhong,
            CTBT.MaBoiThuong,
            C.ThanhTien,
            GD.TongTien
        FROM CTGD C
        JOIN GIAO_DICH GD      ON GD.MaDoan  = C.MaDoan
        LEFT JOIN CT_BOI_THUONG CTBT ON CTBT.MaCTGD = C.MaCTGD
        WHERE C.MaDoan = @MaDoan;

        -- Giữ transaction mở cho T2 chen thêm bồi thường
        WAITFOR DELAY '00:00:05';

        PRINT N'Lần đọc 2 - sau khi nhân viên ghi nhận bồi thường:';

        SELECT 
            C.MaCTGD,
            C.MaPhong,
            CTBT.MaBoiThuong,
            C.ThanhTien,
            GD.TongTien
        FROM CTGD C
        JOIN GIAO_DICH GD      ON GD.MaDoan  = C.MaDoan
        LEFT JOIN CT_BOI_THUONG CTBT ON CTBT.MaCTGD = C.MaCTGD
        WHERE C.MaDoan = @MaDoan;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH;
END;
GO

-- =============================================

-- Tình huống 10:  KH1 và KH2 cùng đoàn, KH1 chỉnh sửa sđt, KH2 tra cứu thông tin đoàn
-- 1. SP UPDATE TỔNG QUÁT (Dùng cho T1 - Gây tranh chấp)
CREATE OR ALTER PROCEDURE sp_CapNhatThongTinKhachHang
    @MaKH INT,
    @MaDoan CHAR(4),
    @TenKH NVARCHAR(55),
    @SoCMND NVARCHAR(20),
    @NgaySinh DATE,
    @SDT NVARCHAR(13)
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

    BEGIN TRAN;
    BEGIN TRY
        -- 1. Cập nhật bình thường (Cho phép trùng tạm thời để Demo)
        UPDATE KHACH_HANG
        SET TenKH = @TenKH,
            SoCMND = @SoCMND,
            NgaySinh = @NgaySinh,
            SDT = @SDT
        WHERE MaKH = @MaKH AND MaDoan = @MaDoan;

        -- 2. Delay 15 giây (Lúc này dữ liệu đã vào DB nhưng chưa Commit)
        -- T2 đọc lúc này sẽ thấy SĐT đã bị đổi (và bị trùng)
        WAITFOR DELAY '00:00:15'; 

        -- 3. KIỂM TRA LOGIC: Nếu phát hiện trùng SĐT thì Rollback
        -- (Đếm xem trong bảng có bao nhiêu người có cùng SĐT vừa nhập)
        IF (SELECT COUNT(*) FROM KHACH_HANG WHERE SDT = @SDT) > 1
        BEGIN
            -- Nếu > 1 người có cùng SĐT -> Lỗi -> Rollback
            ROLLBACK TRAN;
            RAISERROR(N'Lỗi Update: Violation of UNIQUE KEY constraint (Giả lập). SĐT bị trùng!', 16, 1);
            RETURN;
        END

        -- Nếu không trùng thì Commit
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH
END;
GO


-- Store Procedure T2: sp_XemChiTietKhachHang (Khách hàng 2 - Trưởng đoàn). Chưa fix lỗi
CREATE OR ALTER PROCEDURE sp_XemChiTietKhachHang_LOI
    @MaDoan CHAR(4)
AS
BEGIN
    SET NOCOUNT ON;

    -- Đặt mức độ cô lập: READ UNCOMMITTED (Đọc bẩn) 
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; 

    BEGIN TRAN;
        -- Đọc thông tin chi tiết giao dịch có mã chi tiết giao dịch tương ứng 
        -- Không cần xin khóa đọc 
        SELECT *
        FROM KHACH_HANG
        WHERE MaDoan = @MaDoan;
        -- Output: Trưởng đoàn thấy được có 2 thành viên trong đoàn có cùng số điện thoại 
    COMMIT;
END
GO

-- Tình huống 10: T2 - ĐÃ FIX LỖI
CREATE OR ALTER PROCEDURE sp_XemChiTietKhachHang
    @MaDoan CHAR(4)
AS
BEGIN
    SET NOCOUNT ON;

    -- THAY ĐỔI: Chuyển từ READ UNCOMMITTED sang READ COMMITTED để tránh Dirty Read
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED; 

    BEGIN TRAN;
        -- Bây giờ, lệnh SELECT này sẽ bị chặn (block) cho đến khi T1 kết thúc.
        -- Nếu T1 Rollback, T2 sẽ chỉ thấy dữ liệu cũ (dữ liệu đúng).
        SELECT *
        FROM KHACH_HANG
        WHERE MaDoan = @MaDoan;
        
    COMMIT;
END
GO

-- =============================================
-- T1: KHÁCH HÀNG TRA CỨU (BẢN LỖI)
-- =============================================
CREATE OR ALTER PROC sp_TraCuuPhongTheoHang_LOI
    @LoaiHang NVARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;
    -- Mức cô lập READ COMMITTED: Cho phép Phantom Read
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED; 

    BEGIN TRY
        BEGIN TRAN; 

        -- LẦN ĐỌC 1: Đếm số lượng và lấy danh sách
        DECLARE @Count1 INT;
        SELECT @Count1 = COUNT(*) FROM PHONG WHERE LoaiHang = @LoaiHang;
        
        -- Trả về Result Set 1: Kết quả lần đầu
        SELECT 'LAN_1' as LanDoc, @Count1 as SoLuong, MaPhong, GiaPhong 
        FROM PHONG WHERE LoaiHang = @LoaiHang;

        -- TREO 10 GIÂY (Để T2 kịp chèn phòng mới)
        WAITFOR DELAY '00:00:10'; 

        -- LẦN ĐỌC 2: Đọc lại
        DECLARE @Count2 INT;
        SELECT @Count2 = COUNT(*) FROM PHONG WHERE LoaiHang = @LoaiHang;

        -- Trả về Result Set 2: Kết quả lần hai (Sẽ thấy dư ra 1 phòng nếu lỗi)
        SELECT 'LAN_2' as LanDoc, @Count2 as SoLuong, MaPhong, GiaPhong 
        FROM PHONG WHERE LoaiHang = @LoaiHang;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH
END;
GO


-- =============================================
-- Tình huống 11
-- T1: KHÁCH HÀNG TRA CỨU (BẢN FIX)
-- =============================================
CREATE OR ALTER PROC sp_TraCuuPhongTheoHang
    @LoaiHang NVARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;
    -- SERIALIZABLE: Giữ khóa phạm vi (Range Lock), T2 không thể Insert vào vùng này
    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE; 

    BEGIN TRY
        BEGIN TRAN; 

        -- LẦN ĐỌC 1
        DECLARE @Count1 INT;
        SELECT @Count1 = COUNT(*) FROM PHONG WHERE LoaiHang = @LoaiHang;
        
        SELECT 'LAN_1' as LanDoc, @Count1 as SoLuong, MaPhong, GiaPhong 
        FROM PHONG WHERE LoaiHang = @LoaiHang;

        -- TREO 10 GIÂY
        WAITFOR DELAY '00:00:10'; 

        -- LẦN ĐỌC 2
        DECLARE @Count2 INT;
        SELECT @Count2 = COUNT(*) FROM PHONG WHERE LoaiHang = @LoaiHang;

        SELECT 'LAN_2' as LanDoc, @Count2 as SoLuong, MaPhong, GiaPhong 
        FROM PHONG WHERE LoaiHang = @LoaiHang;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH
END;
GO

-- Tình huống 1
-- =============================================================
-- BẢN LỖI: Gây ra Lost Update
-- Mức cô lập: READ COMMITTED (Mặc định)
-- =============================================================
CREATE OR ALTER PROC sp_ThemCTBoiThuong_LOI
    @MaCTGD VARCHAR(10),
    @MaBoiThuong VARCHAR(10),
    @SoLuongThem INT
AS
BEGIN
    SET NOCOUNT ON;
    -- Mức cô lập thấp, cho phép đọc phantome/không giữ khóa
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED; 

    BEGIN TRY
        BEGIN TRAN;

        -- 1. KIỂM TRA TỒN TẠI (Cả T1 và T2 đều thấy FALSE)
        DECLARE @DaTonTai BIT = 0;
        DECLARE @SoLuongCu INT = 0;

        IF EXISTS (SELECT 1 FROM CT_BOI_THUONG WHERE MaCTGD = @MaCTGD AND MaBoiThuong = @MaBoiThuong)
        BEGIN
            SET @DaTonTai = 1;
            SELECT @SoLuongCu = SoLuong FROM CT_BOI_THUONG WHERE MaCTGD = @MaCTGD AND MaBoiThuong = @MaBoiThuong;
        END

        -- Giả lập độ trễ để T2 chạy tới đây khi T1 chưa kịp Insert
        WAITFOR DELAY '00:00:10'; 

        -- 2. XỬ LÝ
        IF @DaTonTai = 1
        BEGIN
            -- Nếu đã thấy có rồi thì cộng dồn bình thường (nhưng vẫn bị Lost Update nếu chạy nhánh này)
            UPDATE CT_BOI_THUONG
            SET SoLuong = @SoLuongCu + @SoLuongThem 
            WHERE MaCTGD = @MaCTGD AND MaBoiThuong = @MaBoiThuong;
        END
        ELSE
        BEGIN
            -- Cả T1 và T2 cùng chạy vào đây
            BEGIN TRY
                INSERT INTO CT_BOI_THUONG (MaCTGD, MaBoiThuong, SoLuong)
                SELECT @MaCTGD, @MaBoiThuong, @SoLuongThem
                FROM BOI_THUONG WHERE MaBoiThuong = @MaBoiThuong;
            END TRY
            BEGIN CATCH
                -- NẾU T2 BỊ LỖI TRÙNG KHÓA (Do T1 đã Insert trước)
                -- Ta xử lý để không bị văng lỗi ra App, nhưng logic vẫn sai (Lost Update)
                IF ERROR_NUMBER() = 2627 -- Violation of PRIMARY KEY
                BEGIN
                    -- T2 quyết định: "Thôi lỡ rồi, tao ghi đè số của tao vào luôn"
                    -- Thay vì cộng dồn (SoLuong + @SoLuongThem), ta GÁN LUÔN bằng @SoLuongThem
                    -- => Mất dữ liệu T1 vừa thêm.
                    UPDATE CT_BOI_THUONG
                    SET SoLuong = @SoLuongThem 
                    WHERE MaCTGD = @MaCTGD AND MaBoiThuong = @MaBoiThuong;
                END
                ELSE
                BEGIN
                    THROW; -- Lỗi khác thì ném ra
                END
            END CATCH
        END

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH
END;
GO

-- =============================================================
-- BẢN FIX: Ngăn chặn Lost Update
-- Giải pháp: SERIALIZABLE + UPDLOCK (Hoặc chỉ cần UPDLOCK ở mức Read Committed)
-- =============================================================
CREATE OR ALTER PROC sp_ThemCTBoiThuong
    @MaCTGD VARCHAR(10),
    @MaBoiThuong VARCHAR(10),
    @SoLuongThem INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- SERIALIZABLE: Đảm bảo giao dịch tuần tự chặt chẽ nhất
    -- Hoặc giữ Read Committed nhưng dùng UPDLOCK cũng được (ở đây dùng Serializable cho chắc)
    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

    BEGIN TRY
        BEGIN TRAN;

        -- 1. KIỂM TRA & ĐỌC (Kèm khóa UPDLOCK để chặn thằng khác đọc tranh)
        -- WITH (UPDLOCK): Tuyên bố "Tôi đọc để chuẩn bị sửa, ai muốn sửa thì xếp hàng chờ tôi xong đã"
        DECLARE @SoLuongCu INT = 0;
        DECLARE @DaTonTai BIT = 0;

        IF EXISTS (SELECT 1 FROM CT_BOI_THUONG WITH (UPDLOCK) WHERE MaCTGD = @MaCTGD AND MaBoiThuong = @MaBoiThuong)
        BEGIN
            SET @DaTonTai = 1;
            
            SELECT @SoLuongCu = SoLuong 
            FROM CT_BOI_THUONG 
            WHERE MaCTGD = @MaCTGD AND MaBoiThuong = @MaBoiThuong;
        END

        -- Giả lập độ trễ (Dù có delay, T2 vẫn phải chờ T1 nhả khóa mới được chạy tiếp)
        WAITFOR DELAY '00:00:10'; 

        -- 2. TÍNH TOÁN & CẬP NHẬT
        IF @DaTonTai = 1
        BEGIN
            -- Cách viết an toàn nhất: Update trực tiếp trên cột (Atomic Update)
            -- UPDATE CT_BOI_THUONG SET SoLuong = SoLuong + @SoLuongThem ...
            -- Nhưng để demo Serializable chặn đọc, ta vẫn dùng biến @SoLuongCu
            UPDATE CT_BOI_THUONG
            SET SoLuong = @SoLuongCu + @SoLuongThem 
            WHERE MaCTGD = @MaCTGD AND MaBoiThuong = @MaBoiThuong;
        END
        ELSE
        BEGIN
            INSERT INTO CT_BOI_THUONG (MaCTGD, MaBoiThuong, SoLuong)
            SELECT @MaCTGD, @MaBoiThuong, @SoLuongThem
            FROM BOI_THUONG WHERE MaBoiThuong = @MaBoiThuong;
        END

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH
END;
GO

-- Tình huống 3
-- T1: Nhân viên tra cứu Hệ số giá Hạng Phòng
CREATE OR ALTER PROC sp_TraCuuHeSoGia_LOI
    @LoaiHang NVARCHAR(30)
AS
BEGIN
    -- Mức cô lập READ COMMITTED (chưa fix lỗi Unrepeatable Read)
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED; 

    BEGIN TRY
        BEGIN TRAN;

		-- Kiểm tra LoaiHang có tồn tại?
		IF NOT EXISTS ( SELECT 1 
						FROM HANG_PHONG
						WHERE LoaiHang = @LoaiHang )
		BEGIN
			THROW 50001, N'Không tồn tại loại hạng trong hệ thống', 1;
		END

        DECLARE @HeSoGiaHP DECIMAL(8,4);

		-- Mốc quyết định: Giả sử dưới 1.7 thì cần tăng
        DECLARE @NguongGiam DECIMAL(8,4) = 1.7; 

        -- Kiểm tra Hệ số Giá Hạng Phòng hiện tại
        SELECT @HeSoGiaHP = HeSoGiaHP
        FROM HANG_PHONG 
        WHERE LoaiHang = @LoaiHang;

		
        -- Chờ T2 cập nhật và COMMIT
        WAITFOR DELAY '00:00:10'; 
        
		-- Đưa ra quyết định cho kế hoạch
        IF @HeSoGiaHP < @NguongGiam
        BEGIN
            PRINT N'Hệ số hiện tại (' + FORMAT(@HeSoGiaHP, 'N4') + N') đang thấp so với thị trường. RA QUYẾT ĐỊNH: Cần tăng thêm.';
        END
        ELSE
        BEGIN
            PRINT N'Hệ số đã đạt. RA QUYẾT ĐỊNH: Giữ nguyên.';
        END

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        PRINT N'Lỗi: ' + ERROR_MESSAGE();
		IF @@TRANCOUNT > 0
		BEGIN
			ROLLBACK TRANSACTION;
		END
    END CATCH;
END;
GO

-- T1: Nhân viên tra cứu Hệ số giá Hạng Phòng
CREATE OR ALTER PROC sp_TraCuuHeSoGia
    @LoaiHang NVARCHAR(30)
AS
BEGIN
    -- Mức cô lập REPEATABLE READ fix lỗi unrepeatable read
    SET TRANSACTION ISOLATION LEVEL REPEATABLE READ; 

    BEGIN TRY
        BEGIN TRAN;

		-- Kiểm tra LoaiHang có tồn tại?
		IF NOT EXISTS ( SELECT 1 
						FROM HANG_PHONG
						WHERE LoaiHang = @LoaiHang )
		BEGIN
			THROW 50001, N'Không tồn tại loại hạng trong hệ thống', 1;
		END

        DECLARE @HeSoGiaHP DECIMAL(8,4);

		-- Mốc quyết định: Giả sử dưới 1.7 thì cần tăng
        DECLARE @NguongGiam DECIMAL(8,4) = 1.7; 

        -- Kiểm tra Hệ số Giá Hạng Phòng hiện tại
        SELECT @HeSoGiaHP = HeSoGiaHP
        FROM HANG_PHONG 
        WHERE LoaiHang = @LoaiHang;

		
        -- Chờ T2 cập nhật và COMMIT
        WAITFOR DELAY '00:00:10'; 
        
		-- Đưa ra quyết định cho kế hoạch
        IF @HeSoGiaHP < @NguongGiam
        BEGIN
            PRINT N'Hệ số hiện tại (' + FORMAT(@HeSoGiaHP, 'N4') + N') đang thấp so với thị trường. RA QUYẾT ĐỊNH: Cần tăng thêm.';
        END
        ELSE
        BEGIN
            PRINT N'Hệ số đã đạt. RA QUYẾT ĐỊNH: Giữ nguyên.';
        END

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        PRINT N'Lỗi: ' + ERROR_MESSAGE();
		IF @@TRANCOUNT > 0
		BEGIN
			ROLLBACK TRANSACTION;
		END
    END CATCH;
END;
GO

-- T2: Nhân viên cập nhật Hệ số giá Hạng Phòng 
CREATE OR ALTER PROC sp_CapNhatHeSoGiaHP
    @LoaiHang NVARCHAR(30),
    @HeSoMoi DECIMAL(8,4)
AS
BEGIN
    SET NOCOUNT ON;

    -- Mức cô lập mặc định: READ COMMITTED
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED; 

    BEGIN TRY
        BEGIN TRAN;

		-- Kiểm tra LoaiHang có tồn tại?
		IF NOT EXISTS ( SELECT 1 
						FROM HANG_PHONG
						WHERE LoaiHang = @LoaiHang )
		BEGIN
			THROW 50001, N'Không tồn tại loại hạng trong hệ thống', 1;
		END

        -- Cập nhật HeSoGiaHP trực tiếp
        UPDATE HANG_PHONG
        SET HeSoGiaHP = @HeSoMoi
        WHERE LoaiHang = @LoaiHang;
        
        -- Trigger TRG_PHONG_TinhGia sẽ tự động kích hoạt và cập nhật GiaPhong.
        
        -- T2 COMMIT dữ liệu
        COMMIT TRAN;
		PRINT N'Cập nhật hệ số giá thành công';
    END TRY
    BEGIN CATCH
        PRINT N'Lỗi: ' + ERROR_MESSAGE();
		IF @@TRANCOUNT > 0
		BEGIN
			ROLLBACK TRANSACTION;
		END
    END CATCH;
END;
GO

--TinhHuong2:
	--T1: Nhân viên thống kê số lượng phòng vip
CREATE OR ALTER PROCEDURE sp_ThongKePhong_NhanVien
AS
BEGIN
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ COMMITTED; --Cải thiện bằng cách nâng lên REPEATABLE READ
	BEGIN TRY
	BEGIN TRAN

	DECLARE @SoPhongVip INT
	SELECT @SoPhongVip = COUNT(*)
	FROM PHONG
	WHERE LoaiHang = 'VIP'

	WAITFOR DELAY '00:00:10';
	
	IF @SoPhongVip <= 5
          PRINT  N'Số lượng  phòng VIP quá ít cần thêm phòng vip';
	ELSE 
          PRINT N'Số lượng phòng VIP đã đủ không cần thêm phòng';

	COMMIT
	END TRY
	BEGIN CATCH
             PRINT N'Lỗi: ' + ERROR_MESSAGE();
             IF @@TRANCOUNT > 0
                   ROLLBACK TRAN;
	END CATCH
END;
GO

CREATE OR ALTER PROCEDURE sp_ThongKePhong_FIX_NhanVien
AS
BEGIN
    SET NOCOUNT ON;

    -- Nâng chuẩn mức cô lập lên cao nhất SERIALIZABLE
    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

    BEGIN TRY
        BEGIN TRAN;

        DECLARE @SoPhongVip INT;
        -- Khi SELECT COUNT(*):
				-- Shared Lock (S) được đặt trên các dòng VIP
				-- Giao tác T2 sẽ bị BLOCK nếu cố UPDATE LoaiHang
        SELECT @SoPhongVip = COUNT(*)
        FROM PHONG
        WHERE LoaiHang = 'VIP';

        WAITFOR DELAY '00:00:10';

        IF @SoPhongVip <= 5
            PRINT N'Số lượng phòng VIP quá ít, cần thêm phòng VIP';
        ELSE
            PRINT N'Số lượng phòng VIP đã đủ, không cần thêm phòng';

        COMMIT;
    END TRY
    BEGIN CATCH
        PRINT N'Lỗi: ' + ERROR_MESSAGE();
        IF @@TRANCOUNT > 0
            ROLLBACK TRAN;
    END CATCH
END;
GO

	--T2:Nhân viên cập nhật phòng
CREATE OR ALTER PROCEDURE sp_CapNhatPhong_NhanVien
    @MaPhong CHAR(4),
    @HangPhongMoi NVARCHAR(30)
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRAN
	--Kiểm tra phòng có tồn tại không
	IF NOT EXISTS (SELECT 1 FROM PHONG 
		WHERE @MaPhong = MaPhong)
	BEGIN
       RAISERROR (N'Không tồn tại phòng cần tìm!!', 16, 1);
       ROLLBACK TRAN;
       RETURN;
	END;

	UPDATE PHONG 
	SET LoaiHang = @HangPhongMoi
	WHERE MaPhong = @MaPhong;
	IF @@ERROR <> 0
	BEGIN
		RAISERROR (N'Cập nhật thông tin phòng không thành công', 16, 1);
		ROLLBACK TRAN;
		RETURN;
	END;
	COMMIT
END;
GO

-- Tình huống 4
-- Giả định: Đoàn nào có từ 2 trẻ em trở lên (sinh sau năm 2013, tức dưới 12 tuổi) 
-- sẽ được tặng một Set Gấu Bông HolyBird làm quà lưu niệm.
-- Tình huống: Nhân viên đọc thông tin của đoàn để quyết định việc tặng quà.
-- Khách hàng sửa lại năm sinh.
-- Lỗi Unrepeatable Read

-- T1: Nhân viên đếm số lượng khách hàng dưới 12 tuổi trong đoàn để ra quyết định tặng quà.
CREATE OR ALTER PROC sp_LeTan_TangQuaTreEm_LOI
    @MaDoan CHAR(4)
AS
BEGIN
    -- Mức cô lập mặc định gây lỗi Unrepeatable Read
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
	BEGIN TRY
		BEGIN TRANSACTION;
		-- Kiểm tra mã đoàn có tồn tại không
		IF NOT EXISTS (SELECT 1 FROM DOAN WHERE MaDoan = @MaDoan)
		BEGIN
		    ;THROW 50001, N'Mã đoàn không tồn tại trong hệ thống.', 1;
		END

		DECLARE @SoTreEm INT;
		-- Đếm số khách sinh sau năm 2013 (Dưới 12 tuổi)
		SELECT @SoTreEm = COUNT(*) 
		FROM KHACH_HANG 
		WHERE MaDoan = @MaDoan AND NgaySinh > '2013-01-01';
		-- Giả sử cần thời gian xử lý nghiệp vụ (đợi)
		-- Trong lúc này T2 sẽ sửa tuổi
		WAITFOR DELAY '00:00:12'; 

		IF @SoTreEm >= 2
		    PRINT N'Đủ điều kiện nhận Set Gấu Bông';
		ELSE
		BEGIN
			PRINT N'Không đủ điều kiện nhận quà';
		END

		COMMIT TRANSACTION;
	END TRY
    BEGIN CATCH
        PRINT N'Lỗi: ' + ERROR_MESSAGE();
		IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END
    END CATCH
END;
GO

-- Cải thiện T1: set isolation level REPEATABLE READ
CREATE OR ALTER PROC sp_LeTan_TangQuaTreEm
    @MaDoan CHAR(4)
AS
BEGIN
    SET TRANSACTION ISOLATION LEVEL REPEATABLE READ;
	BEGIN TRY
	BEGIN TRANSACTION;
		-- Kiểm tra mã đoàn có tồn tại không
		IF NOT EXISTS (SELECT 1 FROM DOAN WHERE MaDoan = @MaDoan)
		BEGIN
		    ;THROW 50001, N'Mã đoàn không tồn tại trong hệ thống.', 1;
		END

		DECLARE @SoTreEm INT;
		-- Đếm số khách sinh sau năm 2013 (Dưới 12 tuổi)
		SELECT @SoTreEm = COUNT(*) 
		FROM KHACH_HANG 
		WHERE MaDoan = @MaDoan AND NgaySinh > '2013-01-01';
		-- Giả sử cần thời gian xử lý nghiệp vụ (đợi)
		-- Trong lúc này T2 sẽ sửa tuổi
		WAITFOR DELAY '00:00:12'; 

		IF @SoTreEm >= 2
		    PRINT N'Đủ điều kiện nhận Set Gấu Bông';
		ELSE
		BEGIN
		    PRINT N'Không đủ điều kiện nhận quà';
		END

		COMMIT TRANSACTION;
	END TRY
    BEGIN CATCH
        PRINT N'Lỗi: ' + ERROR_MESSAGE();
		IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END
    END CATCH
END;
GO

-- T2: 1 khách hàng sửa ngày sinh lại sao cho tuổi > 12 
CREATE OR ALTER PROC sp_KH_SuaNgaySinh
    @MaDoan CHAR(4),
    @MaKH INT, -- Mã khách hàng bị nhập sai ngày sinh
    @NgaySinhDung DATE
AS
BEGIN
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
    BEGIN TRY
    BEGIN TRANSACTION;
		-- Kiểm tra ngày sinh hợp lệ (Không được ở tương lai)
        IF @NgaySinhDung >= CAST(GETDATE() AS DATE)
        BEGIN
            ;THROW 50002, N'Ngày sinh không hợp lệ (không được lớn hơn ngày hiện tại).', 1;
        END

        -- Kiểm tra Mã Đoàn có tồn tại không
        IF NOT EXISTS (SELECT 1 FROM DOAN WHERE MaDoan = @MaDoan)
        BEGIN
            ;THROW 50003, N'Mã đoàn không tồn tại trong hệ thống.', 1;
        END

        -- Kiểm tra Mã Khách Hàng có tồn tại không
        IF NOT EXISTS (SELECT 1 FROM KHACH_HANG WHERE MaKH = @MaKH AND MaDoan = @MaDoan)
        BEGIN
            ;THROW 50004, N'Mã khách hàng không tồn tại.', 1;
        END

        PRINT N'Khách đang sửa lại ngày sinh...';
        
        UPDATE KHACH_HANG
        SET NgaySinh = @NgaySinhDung
        WHERE MaKH = @MaKH AND MaDoan = @MaDoan;

        COMMIT TRANSACTION;
        PRINT N'Cập nhật thành công!';
    END TRY
    BEGIN CATCH
        PRINT N'Lỗi: ' + ERROR_MESSAGE();
		IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END
    END CATCH
END;
GO

-- TÌNH HUỐNG 5
CREATE OR ALTER PROC sp_TraCuuTrangThaiPhong_LOI
    @MaPhong CHAR(4)
AS
BEGIN
    SET NOCOUNT ON;

	SET TRANSACTION ISOLATION LEVEL READ COMMITTED;  

    BEGIN TRY
        BEGIN TRAN;

        -- Kiểm tra phòng có tồn tại không
        IF NOT EXISTS (SELECT 1 FROM PHONG WHERE MaPhong = @MaPhong)
        BEGIN
            RAISERROR (N'Không tìm thấy phòng %s.', 16, 1, @MaPhong);
            ROLLBACK TRAN;
            RETURN;
        END;

        PRINT N'Lần đọc 1:';
        SELECT MaPhong, TrangThai
        FROM PHONG
        WHERE MaPhong = @MaPhong;

        -- Giữ transaction mở 5s cho T2 kịp trả phòng
        WAITFOR DELAY '00:00:05';

        PRINT N'Lần đọc 2:';
        SELECT MaPhong, TrangThai
        FROM PHONG
        WHERE MaPhong = @MaPhong;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROC sp_TraCuuTrangThaiPhong
    @MaPhong CHAR(4)
AS
BEGIN
    SET NOCOUNT ON;

	SET TRANSACTION ISOLATION LEVEL REPEATABLE READ;

    BEGIN TRY
        BEGIN TRAN;

        -- Kiểm tra phòng có tồn tại không
        IF NOT EXISTS (SELECT 1 FROM PHONG WHERE MaPhong = @MaPhong)
        BEGIN
            RAISERROR (N'Không tìm thấy phòng %s.', 16, 1, @MaPhong);
            ROLLBACK TRAN;
            RETURN;
        END;

        PRINT N'Lần đọc 1:';
        SELECT MaPhong, TrangThai
        FROM PHONG
        WHERE MaPhong = @MaPhong;

        -- Giữ transaction mở 5s cho T2 kịp trả phòng
        WAITFOR DELAY '00:00:05';

        PRINT N'Lần đọc 2:';
        SELECT MaPhong, TrangThai
        FROM PHONG
        WHERE MaPhong = @MaPhong;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROC sp_TraPhong
    @MaDoan  CHAR(4),
    @MaPhong CHAR(4)
AS
BEGIN
    SET NOCOUNT ON;

    SET TRANSACTION ISOLATION LEVEL READ COMMITTED; 

    BEGIN TRY
        BEGIN TRAN;

        DECLARE @Now DATETIME = GETDATE();

        -- 0. Kiểm tra đoàn có tồn tại không
        IF NOT EXISTS (SELECT 1 FROM DOAN WHERE MaDoan = @MaDoan)
        BEGIN
            RAISERROR (N'Không tìm thấy đoàn %s.', 16, 1, @MaDoan);
            ROLLBACK TRAN;
            RETURN;
        END;

        -- 1. Kiểm tra có CTGD cho đoàn + phòng này không
        IF NOT EXISTS (
            SELECT 1
            FROM CTGD
            WHERE MaDoan  = @MaDoan
              AND MaPhong = @MaPhong
        )
        BEGIN
            RAISERROR (N'Không tìm thấy chi tiết giao dịch cho phòng %s trong đoàn %s.', 
                       16, 1, @MaPhong, @MaDoan);
            ROLLBACK TRAN;
            RETURN;
        END;

        -- 2. Kiểm tra trạng thái hiện tại: phải là 'Chưa trả phòng'
        IF EXISTS (
            SELECT 1
            FROM CTGD
            WHERE MaDoan  = @MaDoan
              AND MaPhong = @MaPhong
              AND TrangThai <> N'Chưa trả phòng'
        )
        BEGIN
            RAISERROR (N'Trả phòng không thành công!!!', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END;

        -- 3. Thực hiện trả phòng
        UPDATE C
        SET ThoiGianTraPhong = @Now,
            TrangThai        = N'Đã trả phòng'
        FROM CTGD C
        WHERE C.MaDoan  = @MaDoan
          AND C.MaPhong = @MaPhong;

		PRINT N'Trả phòng thành công';
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH;
END;
GO

--TÌNH HUỐNG 6
	--T1: Khách hàng A đặt phòng
CREATE OR ALTER PROCEDURE sp_DatPhong_KhachHangA
			@MaDoan CHAR(4), @MaKH INT, @MaPhong CHAR(4), 
			@ThoiGianNhanPhong DATETIME, @ThoiGianTraPhong DATETIME, 
			@ThoiGianThucHien DATETIME
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRAN
		--Kiểm tra thông tin khách hàng
		IF NOT EXISTS (
			SELECT 1
			FROM KHACH_HANG kh
			WHERE kh.MaDoan = @MaDoan AND kh.MaKH = @MaKH
		)
		BEGIN
		   RAISERROR (N'Mã đoàn  hoặc mã khách hàng không hợp lệ!!', 16, 1);
		   ROLLBACK TRAN;
		   RETURN;
		END;
		--Kiểm tra mã phòng
		IF NOT EXISTS (
			SELECT 1
			FROM PHONG WHERE MaPhong = @MaPhong
		)
		BEGIN
		   RAISERROR (N'Mã phòng không hợp lệ!! %s', 16, 1, @MaPhong);
		   ROLLBACK TRAN;
		   RETURN;
		END;

		INSERT INTO CTGD (MaDoan, MaKH, MaPhong, ThoiGianNhanPhong, ThoiGianTraPhong, ThoiGianThucHien) --Ở ngoài thêm trước 1 giao dịch có đoàn mới hoặc
		VALUES (@MaDoan, @MaKH, @MaPhong, @ThoiGianNhanPhong, @ThoiGianTraPhong, @ThoiGianThucHien);					--xóa hết dữ liệu rồi thêm mới ban đầu
    
		PRINT N'Khách A: Đã Insert đặt phòng thành công';
		WAITFOR DELAY '00:00:10';
		UPDATE CTGD 
		SET MaDoan = 'D999' --Mã đoàn không tồn tại
		WHERE MaPhong = @MaPhong AND MaDoan = @MaDoan;
    
		IF @@ERROR <> 0
		BEGIN
			RAISERROR (N'Đặt phòng bị hủy, có thông tin không hợp lệ!!', 16, 1);
			ROLLBACK TRANSACTION
			RETURN
		END
	COMMIT
END;
GO

	--T2: Khách hàng B tra cứu thông tin phòng
CREATE OR ALTER PROCEDURE sp_TraCuuPhong_KhachHangB
    @MaPhong CHAR(4)
AS
BEGIN
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; --Khắc phục lỗi này bằng cách đứa lên thành READ COMMITTED
	BEGIN TRAN
	IF NOT EXISTS (
       SELECT 1
       FROM PHONG WHERE MaPhong = @MaPhong
	)
	BEGIN
       RAISERROR (N'Mã phòng không hợp lệ!! %s', 16, 1, @MaPhong);
       ROLLBACK TRAN;
       RETURN;
	END;
	
	PRINT N'Thông tin phòng ' + @MaPhong + ': ';
	SELECT * 
	FROM PHONG
	Where MaPhong = @MaPhong;
	
	COMMIT
END;
GO

CREATE OR ALTER PROCEDURE sp_TraCuuPhong_FIX_KhachHangB
    @MaPhong CHAR(4)
AS
BEGIN
    SET NOCOUNT ON;

	-- Nâng chuẩn mức cô lập lên read commited giúp không cho phép đọc dữ liệu khi chưa COMMIT
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

    BEGIN TRAN;

    IF NOT EXISTS (
        SELECT 1
        FROM PHONG 
        WHERE MaPhong = @MaPhong
    )
    BEGIN
        RAISERROR (N'Mã phòng không hợp lệ!! %s', 16, 1, @MaPhong);
        ROLLBACK TRAN;
        RETURN;
    END;

    -- Nếu T1 đang thao tác và chưa COMMIT:
				--select này sẽ chờ và không đọc dữ liệu bẩn
    PRINT N'Thông tin phòng ' + @MaPhong + ': ';

    SELECT * 
    FROM PHONG
    WHERE MaPhong = @MaPhong;

    COMMIT;
END;
GO

-- TÌNH HUỐNG 7
-- T1: Nhân viên cập nhật thông tin phòng và ROLLBACK do lỗi hệ thống
CREATE OR ALTER PROC sp_CapNhatPhong
    @MaPhong CHAR(4),
    @HinhThucMoi NVARCHAR(50)
AS
BEGIN
    -- Mức cô lập mặc định: READ COMMITTED
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED; 

    BEGIN TRY
        BEGIN TRAN;

        -- 1. KIỂM TRA DỮ LIỆU ĐẦU VÀO
        IF NOT EXISTS (SELECT 1 FROM PHONG WHERE MaPhong = @MaPhong)
        BEGIN
            THROW 50001, N'Mã phòng không tồn tại trong hệ thống.', 1;
            RETURN;
        END

        IF NOT EXISTS (SELECT 1 FROM HINH_THUC_PHONG WHERE TenHinhThuc = @HinhThucMoi)
        BEGIN
            THROW 50002, N'Hình thức phòng không hợp lệ.', 1;
            RETURN;
        END

        -- 2. Cập nhật TenHinhThuc (Dữ liệu bẩn bắt đầu tồn tại)
        UPDATE PHONG
        SET TenHinhThuc = @HinhThucMoi
        WHERE MaPhong = @MaPhong;
        
        PRINT N'T1: Đã cập nhật TenHinhThuc (Dữ liệu bẩn). Đang chờ 5 giây.';

        -- Giữ transaction mở cho T2 kịp đọc dữ liệu bẩn
        WAITFOR DELAY '00:00:05';

        -- 3. GÂY LỖI BẮT BUỘC để kích hoạt ROLLBACK
        -- Giả sử: Nhân viên cố gắng đổi MaPhong thành một mã đã tồn tại ('0102')
        -- Lệnh này sẽ vi phạm ràng buộc khóa chính PK_PHONG
        UPDATE PHONG 
        SET MaPhong = '0102' 
        WHERE MaPhong = @MaPhong; 

        -- Nếu không có lỗi, T1 sẽ COMMIT (không mong muốn trong kịch bản này)
        COMMIT TRAN; 
    END TRY
    BEGIN CATCH
        PRINT N'Lỗi: ' + ERROR_MESSAGE(); 
        -- Bắt lỗi (vi phạm PK) và ROLLBACK toàn bộ giao tác
        IF @@TRANCOUNT > 0 
        BEGIN
             ROLLBACK TRAN; 
             PRINT N'T1: Lỗi hệ thống xảy ra (Vi phạm khóa chính). Đã ROLLBACK toàn bộ giao tác.';
        END
    END CATCH;
END;
GO

-- T2: Khách hàng tra cứu phòng (dùng READ UNCOMMITTED để mô phỏng lỗi)
CREATE OR ALTER PROC sp_TraCuuPhong_LOI
    @MaPhong CHAR(4)
AS
BEGIN
    -- Mức cô lập READ UNCOMMITTED (chưa fix lỗi)
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; 

    BEGIN TRY
        BEGIN TRAN;

		-- KIỂM TRA DỮ LIỆU ĐẦU VÀO
        IF NOT EXISTS (SELECT 1 FROM PHONG WHERE MaPhong = @MaPhong)
        BEGIN
            THROW 50001, N'Mã phòng không tồn tại trong hệ thống.', 1;
            RETURN;
        END

        DECLARE @HinhThucDocDuoc NVARCHAR(50);
        DECLARE @HinhThucSauRollback NVARCHAR(50);

        -- Đọc thông tin phòng (đọc cả dữ liệu chưa Commit - Dữ liệu Bẩn)
        SELECT @HinhThucDocDuoc = TenHinhThuc
        FROM PHONG
        WHERE MaPhong = @MaPhong;
           
        -- THÔNG BÁO GIẢ ĐỊNH QUYẾT ĐỊNH CỦA KHÁCH HÀNG
        PRINT N'T2: Khách hàng đã đọc được: Phòng ' + @MaPhong + N' là loại "' + @HinhThucDocDuoc + N'".';
        PRINT N'T2: Giả định: Khách hàng RA QUYẾT ĐỊNH ĐẶT PHÒNG dựa trên thông tin này.';
/*
        -- Chờ T1 ROLLBACK
        WAITFOR DELAY '00:00:10'; 

        -- Đọc lại sau khi T1 Rollback để thấy dữ liệu thực tế
        SELECT @HinhThucSauRollback = TenHinhThuc
        FROM PHONG 
        WHERE MaPhong = @MaPhong;

        PRINT N'-------------------------------------------------------';
        PRINT N'T2: Lần đọc 2 - Dữ liệu thực tế sau khi T1 ROLLBACK:';
        -- CẢNH BÁO LỖI
        IF @HinhThucDocDuoc <> @HinhThucSauRollback
        BEGIN
             PRINT N'*** LỖI DIRTY READ: Dữ liệu bẩn đã bị đọc. ***';
             PRINT N'Khách hàng đã ra quyết định đặt phòng loại "' + @HinhThucDocDuoc + N'" nhưng thực tế phòng là "' + @HinhThucSauRollback + N'".';
        END
*/
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        PRINT N'Lỗi: ' + ERROR_MESSAGE();
    END CATCH;
END;
GO

-- T2: Khách hàng tra cứu phòng (FIX)
CREATE OR ALTER PROC sp_TraCuuPhong
    @MaPhong CHAR(4)
AS
BEGIN
    -- Mức cô lập READ COMMITTED
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED; 

    BEGIN TRY
        BEGIN TRAN;

		-- KIỂM TRA DỮ LIỆU ĐẦU VÀO
        IF NOT EXISTS (SELECT 1 FROM PHONG WHERE MaPhong = @MaPhong)
        BEGIN
            THROW 50001, N'Mã phòng không tồn tại trong hệ thống.', 1;
            RETURN;
        END

        DECLARE @HinhThucDocDuoc NVARCHAR(50);
        DECLARE @HinhThucSauRollback NVARCHAR(50);

        -- Đọc thông tin phòng (đọc cả dữ liệu chưa Commit - Dữ liệu Bẩn)
        SELECT @HinhThucDocDuoc = TenHinhThuc
        FROM PHONG
        WHERE MaPhong = @MaPhong;
           
        -- THÔNG BÁO GIẢ ĐỊNH QUYẾT ĐỊNH CỦA KHÁCH HÀNG
        PRINT N'T2: Khách hàng đã đọc được: Phòng ' + @MaPhong + N' là loại "' + @HinhThucDocDuoc + N'".';
        PRINT N'T2: Giả định: Khách hàng RA QUYẾT ĐỊNH ĐẶT PHÒNG dựa trên thông tin này.';
/*
        -- Chờ T1 ROLLBACK
        WAITFOR DELAY '00:00:10'; 

        -- Đọc lại sau khi T1 Rollback để thấy dữ liệu thực tế
        SELECT @HinhThucSauRollback = TenHinhThuc
        FROM PHONG 
        WHERE MaPhong = @MaPhong;

        PRINT N'-------------------------------------------------------';
        PRINT N'T2: Lần đọc 2 - Dữ liệu thực tế sau khi T1 ROLLBACK:';
        -- CẢNH BÁO LỖI
        IF @HinhThucDocDuoc <> @HinhThucSauRollback
        BEGIN
             PRINT N'*** LỖI DIRTY READ: Dữ liệu bẩn đã bị đọc. ***';
             PRINT N'Khách hàng đã ra quyết định đặt phòng loại "' + @HinhThucDocDuoc + N'" nhưng thực tế phòng là "' + @HinhThucSauRollback + N'".';
        END
*/
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        PRINT N'Lỗi: ' + ERROR_MESSAGE();
    END CATCH;
END;
GO

-- TÌNH HUỐNG 8
-- Tình huống 8: Khách hàng 1 của đoàn A hủy phòng, Khách hàng 2 của đoàn B đặt phòng đó, 
-- Khách hàng 1 rollback 
-- Lỗi Dirty Read

-- T1: Khách hàng 1 của đoàn A hủy phòng
-- Trường hợp hiện tại chỉ có 1 mình khách hàng 1 ở phòng 0102
CREATE OR ALTER PROC sp_KhachHang_HuyPhong
    @MaPhong CHAR(4)
AS
BEGIN
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
	BEGIN TRY
		BEGIN TRANSACTION;
		-- Kiểm tra phòng có tồn tại không
		IF NOT EXISTS (SELECT 1 FROM PHONG WHERE MaPhong = @MaPhong)
		BEGIN
			;THROW 50001, N'Mã phòng không tồn tại trong hệ thống.', 1;
		END
		-- Tạm chuyển thành 'Đang trống' (Chưa commit)
		PRINT N'Update trạng thái phòng ' + @MaPhong + N' thành Đang trống...';
		UPDATE PHONG SET TrangThai = N'Đang trống' WHERE MaPhong = @MaPhong;

		-- Giả lập chờ xác nhận (10 giây) - Trong lúc này T2 sẽ đọc dữ liệu
		PRINT N'Đang chờ xác nhận...';
		WAITFOR DELAY '00:00:10';

		-- Trường hợp hủy phòng sau thời gian nhận phòng nên trigger check và nhận thấy vi phạm
		-- Không cho hủy phòng (xóa CTGD)
		-- Rollback (Trạng thái quay về 'Đã đặt trước')
		DELETE FROM CTGD WHERE MaPhong = @MaPhong AND TrangThai = N'Chưa trả phòng';
		COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
		PRINT N'Lỗi: ' + ERROR_MESSAGE();
        IF @@TRANCOUNT > 0 
        BEGIN
            ROLLBACK TRANSACTION;
        END    
	END CATCH
END;
GO

-- T2: Khách hàng 2 của đoàn B đặt phòng đó
CREATE OR ALTER PROC sp_KhachHang_TraCuuVaDatPhong_LOI
    @MaDoan CHAR (4),
    @MaKH INT,
    @MaPhong CHAR(4),
    @TGNhanPhong DATETIME,
    @TGTraPhong DATETIME
AS
BEGIN
    -- Bắt buộc dùng Read Uncommitted để dính lỗi Dirty Read
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    BEGIN TRY
        BEGIN TRANSACTION;
        -- Kiểm tra ngày tháng: Ngày trả phải sau ngày nhận
        IF @TGNhanPhong >= @TGTraPhong
        BEGIN
            ;THROW 50001, N'Ngày trả phòng phải sau ngày nhận phòng.', 1;
        END

        -- Kiểm tra ngày nhận phòng: Không được đặt trong quá khứ
        IF @TGNhanPhong < GETDATE()
        BEGIN
            ;THROW 50002, N'Ngày nhận phòng không hợp lệ (đã trôi qua).', 1;
        END

        -- Kiểm tra Mã Phòng có tồn tại không
        IF NOT EXISTS (SELECT 1 FROM PHONG WHERE MaPhong = @MaPhong)
        BEGIN
            ;THROW 50003, N'Mã phòng không tồn tại.', 1;
        END

        -- Kiểm tra Mã Đoàn
        IF NOT EXISTS (SELECT 1 FROM DOAN WHERE MaDoan = @MaDoan)
        BEGIN
            ;THROW 50004, N'Mã đoàn không tồn tại.', 1;
        END

        -- Kiểm tra Mã Khách Hàng và xem Khách có thuộc Đoàn không
        IF NOT EXISTS (SELECT 1 FROM KHACH_HANG WHERE MaKH = @MaKH AND MaDoan = @MaDoan)
        BEGIN
            ;THROW 50005, N'Khách hàng không tồn tại hoặc không thuộc đoàn này.', 1;
        END

        DECLARE @TrangThaiLucDau NVARCHAR(20);
        DECLARE @TrangThaiLucSau NVARCHAR(20);

        -- Tra cứu lần đầu (Dính Dirty Read: Đọc thấy 'Đang trống' do T1 đang giữ nhưng chưa Commit)
        SELECT @TrangThaiLucDau = TrangThai FROM PHONG WHERE MaPhong = @MaPhong;

        IF @TrangThaiLucDau = N'Đang trống'
        BEGIN
            PRINT N'Phòng ' + @MaPhong + N' hiện đang trống';
            
            -- Khách hàng check lại thông tin trước khi xác nhận đặt
            -- 15s > 10s của T1 => Đảm bảo T1 đã Rollback rồi
            WAITFOR DELAY '00:00:15';

            -- Khách bấm nút "Xác nhận đặt" -> Hệ thống kiểm tra kỹ lại lần cuối
            -- Lúc này T1 đã Rollback xong, trạng thái thực tế đã quay về 'Đã đặt trước' (hoặc Bận)
            SELECT @TrangThaiLucSau = TrangThai FROM PHONG WHERE MaPhong = @MaPhong;

            IF @TrangThaiLucSau = N'Đang trống'
            BEGIN
                -- Nếu vẫn trống thật thì mới Insert
                INSERT INTO CTGD (MaDoan, MaKH, MaPhong, ThoiGianNhanPhong, ThoiGianTraPhong, ThoiGianThucHien, ThanhTien, TrangThai)
                VALUES
                (@MaDoan, @MaKH, @MaPhong, @TGNhanPhong, @TGTraPhong, GETDATE(), 0, N'Chưa nhận phòng');
                
                PRINT N'Đặt phòng thành công!';
            END
            ELSE
            BEGIN
                PRINT N'ĐẶT PHÒNG THẤT BẠI! Phòng này đã có người đặt.';
            END
        END
        ELSE
        BEGIN
            PRINT N'Phòng này đã có người đặt.';
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        PRINT N'Lỗi: ' + ERROR_MESSAGE();
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    END CATCH
END;
GO

-- Cải thiện T2 set isolation level READ COMMITTED
CREATE OR ALTER PROC sp_KhachHang_TraCuuVaDatPhong
    @MaDoan CHAR (4),
    @MaKH INT,
    @MaPhong CHAR(4),
    @TGNhanPhong DATETIME,
    @TGTraPhong DATETIME
AS
BEGIN
    -- Không dính lỗi Dirty Read nữa
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

    BEGIN TRY
        BEGIN TRANSACTION;
        -- Kiểm tra ngày tháng: Ngày trả phải sau ngày nhận
        IF @TGNhanPhong >= @TGTraPhong
        BEGIN
            ;THROW 50001, N'Ngày trả phòng phải sau ngày nhận phòng.', 1;
        END

        -- Kiểm tra ngày nhận phòng: Không được đặt trong quá khứ
        IF @TGNhanPhong < GETDATE()
        BEGIN
            ;THROW 50002, N'Ngày nhận phòng không hợp lệ (đã trôi qua).', 1;
        END

        -- Kiểm tra Mã Phòng có tồn tại không
        IF NOT EXISTS (SELECT 1 FROM PHONG WHERE MaPhong = @MaPhong)
        BEGIN
            ;THROW 50003, N'Mã phòng không tồn tại.', 1;
        END

        -- Kiểm tra Mã Đoàn
        IF NOT EXISTS (SELECT 1 FROM DOAN WHERE MaDoan = @MaDoan)
        BEGIN
            ;THROW 50004, N'Mã đoàn không tồn tại.', 1;
        END

        -- Kiểm tra Mã Khách Hàng và xem Khách có thuộc Đoàn không
        IF NOT EXISTS (SELECT 1 FROM KHACH_HANG WHERE MaKH = @MaKH AND MaDoan = @MaDoan)
        BEGIN
            ;THROW 50005, N'Khách hàng không tồn tại hoặc không thuộc đoàn này.', 1;
        END

        DECLARE @TrangThaiLucDau NVARCHAR(20);
        DECLARE @TrangThaiLucSau NVARCHAR(20);

        -- 
        SELECT @TrangThaiLucDau = TrangThai FROM PHONG WHERE MaPhong = @MaPhong;

        IF @TrangThaiLucDau = N'Đang trống'
        BEGIN
            PRINT N'Phòng ' + @MaPhong + N' hiện đang trống';
            
            -- Khách hàng check lại thông tin trước khi xác nhận đặt
            -- 15s > 10s của T1 => Đảm bảo T1 đã Rollback rồi
            WAITFOR DELAY '00:00:15';

            -- Khách bấm nút "Xác nhận đặt" -> Hệ thống kiểm tra kỹ lại lần cuối
            -- Lúc này T1 đã Rollback xong, trạng thái thực tế đã quay về 'Đã đặt trước' (hoặc Bận)
            SELECT @TrangThaiLucSau = TrangThai FROM PHONG WHERE MaPhong = @MaPhong;

            IF @TrangThaiLucSau = N'Đang trống'
            BEGIN
                -- Nếu vẫn trống thật thì mới Insert
                INSERT INTO CTGD (MaDoan, MaKH, MaPhong, ThoiGianNhanPhong, ThoiGianTraPhong, ThoiGianThucHien, ThanhTien, TrangThai)
                VALUES
                (@MaDoan, @MaKH, @MaPhong, @TGNhanPhong, @TGTraPhong, GETDATE(), 0, N'Chưa nhận phòng');
                
                PRINT N'Đặt phòng thành công!';
            END
            ELSE
            BEGIN
                PRINT N'ĐẶT PHÒNG THẤT BẠI! Phòng này đã có người đặt.';
            END
        END
        ELSE
        BEGIN
            PRINT N'Phòng này đã có người đặt.';
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        PRINT N'Lỗi: ' + ERROR_MESSAGE();
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    END CATCH
END;
GO

-- TÌNH HUỐNG 9:
CREATE OR ALTER PROC sp_GhiNhanBoiThuong_Sai
    @MaCTGD      CHAR(6),
    @MaBoiThuong CHAR(4)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRAN;

        -- 1. Kiểm tra CTGD tồn tại
        IF NOT EXISTS (SELECT 1 FROM CTGD WHERE MaCTGD = @MaCTGD)
        BEGIN
            RAISERROR (N'Không tìm thấy chi tiết giao dịch %s.', 16, 1, @MaCTGD);
            ROLLBACK TRAN;
            RETURN;
        END;

        -- 2. Kiểm tra loại bồi thường tồn tại
        IF NOT EXISTS (SELECT 1 FROM BOI_THUONG WHERE MaBoiThuong = @MaBoiThuong)
        BEGIN
            RAISERROR (N'Không tìm thấy loại bồi thường %s.', 16, 1, @MaBoiThuong);
            ROLLBACK TRAN;
            RETURN;
        END;

        -- 3. Không cho trùng cặp (MaCTGD, MaBoiThuong)
        IF EXISTS (
            SELECT 1 
            FROM CT_BOI_THUONG 
            WHERE MaCTGD = @MaCTGD AND MaBoiThuong = @MaBoiThuong
        )
        BEGIN
            RAISERROR (N'Đã tồn tại bồi thường này cho CTGD %s. Vui lòng UPDATE SoLuong.', 
                       16, 1, @MaCTGD);
            ROLLBACK TRAN;
            RETURN;
        END;

        -- 4. INSERT mới, SoLuong mặc định = 1
        INSERT INTO CT_BOI_THUONG (MaBoiThuong, MaCTGD)
        VALUES (@MaBoiThuong, @MaCTGD);

        -- Giữ transaction mở cho T2 đọc dữ liệu bẩn
        WAITFOR DELAY '00:00:05';

        -- Nhân viên phát hiện nhập sai → rollback
		-- 5. Nhân viên soi lại tình trạng CTGD
        DECLARE @TrangThaiCTGD NVARCHAR(50);
        SELECT @TrangThaiCTGD = TrangThai
        FROM CTGD
        WHERE MaCTGD = @MaCTGD;

        -- Giả sử quy định: chỉ được ghi bồi thường sau khi khách đã trả phòng
        -- => Nếu trạng thái vẫn khác 'Đã trả phòng' thì coi là nhập sai, phải rollback
        IF (@TrangThaiCTGD <> N'Đã trả phòng')
        BEGIN
            RAISERROR (N'CTGD %s chưa trả phòng, không được ghi bồi thường lúc này.', 16, 1, @MaCTGD);
            ROLLBACK TRAN;
            RETURN;
        END;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROC sp_XemHoaDon_Dirty_LOI
    @MaDoan CHAR(4)
AS
BEGIN
    SET NOCOUNT ON;

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; 

    BEGIN TRY
        BEGIN TRAN;

        -- 1. Kiểm tra đoàn & giao dịch tồn tại
        IF NOT EXISTS (SELECT 1 FROM DOAN WHERE MaDoan = @MaDoan)
        BEGIN
            RAISERROR (N'Không tìm thấy đoàn %s.', 16, 1, @MaDoan);
            ROLLBACK TRAN;
            RETURN;
        END;

        IF NOT EXISTS (SELECT 1 FROM GIAO_DICH WHERE MaDoan = @MaDoan)
        BEGIN
            RAISERROR (N'Đoàn %s chưa có giao dịch thanh toán.', 16, 1, @MaDoan);
            ROLLBACK TRAN;
            RETURN;
        END;

        -- 2. Xem chi tiết từng CTGD + tổng tiền (có thể dính dữ liệu bẩn)
        SELECT 
            C.MaCTGD,
            C.MaPhong,
            C.ThanhTien,
            GD.TongTien
        FROM CTGD C
        JOIN GIAO_DICH GD ON GD.MaDoan = C.MaDoan
        WHERE C.MaDoan = @MaDoan;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROC sp_XemHoaDon_Dirty
    @MaDoan CHAR(4)
AS
BEGIN
    SET NOCOUNT ON;

	SET TRANSACTION ISOLATION LEVEL READ COMMITTED; 

    BEGIN TRY
        BEGIN TRAN;

        -- 1. Kiểm tra đoàn & giao dịch tồn tại
        IF NOT EXISTS (SELECT 1 FROM DOAN WHERE MaDoan = @MaDoan)
        BEGIN
            RAISERROR (N'Không tìm thấy đoàn %s.', 16, 1, @MaDoan);
            ROLLBACK TRAN;
            RETURN;
        END;

        IF NOT EXISTS (SELECT 1 FROM GIAO_DICH WHERE MaDoan = @MaDoan)
        BEGIN
            RAISERROR (N'Đoàn %s chưa có giao dịch thanh toán.', 16, 1, @MaDoan);
            ROLLBACK TRAN;
            RETURN;
        END;

        -- 2. Xem chi tiết từng CTGD + tổng tiền (có thể dính dữ liệu bẩn)
        SELECT 
            C.MaCTGD,
            C.MaPhong,
            C.ThanhTien,
            GD.TongTien
        FROM CTGD C
        JOIN GIAO_DICH GD ON GD.MaDoan = C.MaDoan
        WHERE C.MaDoan = @MaDoan;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH;
END;
GO

-- TÌNH HUỐNG 13
-- Store Procedure T1: sp_TraCuuPhong (Khách hàng 1)
CREATE OR ALTER PROCEDURE sp_TraCuuPhong13_LOI
AS
BEGIN
    SET NOCOUNT ON;

    -- Đặt mức độ cô lập: REPEATABLE READ 
    SET TRANSACTION ISOLATION LEVEL REPEATABLE READ;

    BEGIN TRAN;

    -- B1: Đếm số phòng VIP đang trống lần thứ 1 
    -- Xin khoá đọc R trên các dòng có LoaiHang = ‘VIP’ và TrangThai = ‘Đang trống’ 
    SELECT COUNT(MaPhong) AS SoLuongLan1 
    FROM PHONG 
    WHERE LoaiHang = N'VIP' AND TrangThai = N'Đang trống'; 

    -- Tạm dừng để Giao tác 2 kịp thực hiện trả phòng (UPDATE) 
    WAITFOR DELAY '00:00:20';

    -- B2: Đếm lại số phòng VIP đang trống để hiển thị trên pop-up 
    -- Xin khóa đọc trên các dòng có LoaiHang = ‘VIP’ và TrangThai = ‘Đang trống’ 
    SELECT COUNT(MaPhong) AS SoLuongLan2 
    FROM PHONG 
    WHERE LoaiHang = N'VIP' AND TrangThai = N'Đang trống';
    -- Kết quả lần 2 sẽ tăng thêm 1 (3 thay vì 2) do Phantom Read 

    COMMIT;
END
GO

CREATE OR ALTER PROCEDURE sp_TraCuuPhong13
AS
BEGIN
    SET NOCOUNT ON;

    -- GIẢI PHÁP: Sử dụng SERIALIZABLE để đặt khóa dải (Range Lock)
    -- Ngăn chặn các giao tác khác chèn hoặc cập nhật dữ liệu rơi vào phạm vi truy vấn này
    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

    BEGIN TRAN;

    -- B1: Đếm số phòng VIP đang trống lần thứ 1 
    SELECT COUNT(MaPhong) AS SoLuongLan1 
    FROM PHONG 
    WHERE LoaiHang = N'VIP' AND TrangThai = N'Đang trống'; 

    -- Giao tác 2 (T2) sẽ bị chặn (Blocked) tại đây nếu nó cố tình cập nhật 
    -- một phòng thành 'Đang trống' thuộc loại 'VIP'
    WAITFOR DELAY '00:00:20';

    -- B2: Đếm lại số phòng VIP đang trống
    SELECT COUNT(MaPhong) AS SoLuongLan2 
    FROM PHONG 
    WHERE LoaiHang = N'VIP' AND TrangThai = N'Đang trống';
    -- Kết quả Lan1 và Lan2 chắc chắn sẽ bằng nhau.

    COMMIT;
END
GO

-- Store Procedure T2: sp_TraPhong (Khách hàng 2)
CREATE OR ALTER PROCEDURE sp_TraPhong13
    @MaPhong CHAR(5)
AS
BEGIN
    SET NOCOUNT ON;

    -- Đặt mức độ cô lập: READ COMMITTED 
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED; 

    BEGIN TRAN;

    -- Khách hàng 2 tiến hành trả phòng VIP 
    -- Xin khóa ghi X trên dòng có MaPhong = @MaPhong 
    UPDATE PHONG 
    SET TrangThai = N'Đang trống' 
    WHERE MaPhong = @MaPhong;
    -- Output: Cập nhật lại trạng thái phòng có mã phòng là @MaPhong thành trạng thái ‘đang trống’ 

    COMMIT;
END
GO

-- TÌNH HUỐNG 14
-- Store Procedure T1: sp_CapNhatTraPhong (Nhân viên)
CREATE OR ALTER PROCEDURE sp_CapNhatTraPhong_LOI
    @MaCTGD CHAR(6),
    @ThoiGianTraPhongMoi DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    -- Đặt mức độ cô lập: READ COMMITTED 
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

    BEGIN TRAN;

    -- Cập nhật thời gian trả phòng của chi tiết giao dịch (@MaCTGD) 
    -- Xin khóa ghi X trên dòng có mã chi tiết giao dịch = @MaCTGD ở bảng CTGD 
    UPDATE CTGD
    SET ThoiGianTraPhong = @ThoiGianTraPhongMoi
    WHERE MaCTGD = @MaCTGD;

    -- Tạm dừng để T2 kịp chiếm khóa X(CTBT) 
    WAITFOR DELAY '00:00:20'; 
    
    -- Trigger (giả định) cập nhật lại thành tiền sẽ chạy tại đây 
    -- Lệnh Trigger (Giả định) này cần: Xin khóa đọc R trên các dòng có mã chi tiết giao dịch = @MaCTGD trên bảng CT_BOI_THUONG để tính lại thành tiền 
    -- => T1 GIỮ X(CTGD) VÀ CHỜ R(CT_BOI_THUONG)

    COMMIT;
    -- Output: chờ vô hạn 
END
GO

CREATE OR ALTER PROCEDURE sp_CapNhatTraPhong
    @MaCTGD CHAR(6),
    @ThoiGianTraPhongMoi DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

    BEGIN TRAN;
    BEGIN TRY
        -- Xin khóa UPDLOCK trên dòng CTGD ngay lập tức để thống nhất thứ tự
        SELECT 1 FROM CTGD WITH (UPDLOCK) WHERE MaCTGD = @MaCTGD;

        UPDATE CTGD
        SET ThoiGianTraPhong = @ThoiGianTraPhongMoi
        WHERE MaCTGD = @MaCTGD;

        -- Giả lập xử lý Trigger
        WAITFOR DELAY '00:00:10'; 

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END
GO

-- Store Procedure T2: sp_ThemChiTietBoiThuong (Nhân viên)
CREATE OR ALTER PROCEDURE sp_ThemChiTietBoiThuong_LOI
    @MaCTGD CHAR(6),
    @MaBoiThuong CHAR(6)
AS
BEGIN
    SET NOCOUNT ON;

    -- Đặt mức độ cô lập: READ COMMITTED 
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

    BEGIN TRAN;

    -- Thêm một chi tiết bồi thường (@MaBT) cho chi tiết giao dịch (@MaCTGD) 
    -- Xin khóa ghi X trên dòng chi tiết bồi thường mới được thêm vào ở bảng CT_BOI_THUONG 
    INSERT INTO CT_BOI_THUONG (MaBoiThuong, MaCTGD)
    VALUES (@MaBoiThuong, @MaCTGD);

    -- Tạm dừng để T1 kịp chiếm khóa X(CTGD) 
    WAITFOR DELAY '00:00:20'; 
    
    -- Trigger (giả định) cập nhật lại thành tiền sẽ chạy tại đây 
    -- Lệnh Trigger (Giả định) này cần: Xin khóa ghi X trên dòng có mã chi tiết giao dịch = @MaCTGD ở bảng CTGD 
    -- => T2 GIỮ X(CT_BOI_THUONG) VÀ CHỜ X(CTGD)

    COMMIT;
    -- Output: Chờ vô hạn 
END
GO


CREATE OR ALTER PROCEDURE sp_ThemChiTietBoiThuong
    @MaCTGD CHAR(6),
    @MaBoiThuong CHAR(6)
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

    BEGIN TRAN;
    BEGIN TRY
        -- Quan trọng: Phải xin khóa trên CTGD TRƯỚC KHI chèn vào CT_BOI_THUONG
        -- Việc này đảm bảo T2 phải đợi T1 xong hoàn toàn rồi mới bắt đầu làm việc
        SELECT 1 FROM CTGD WITH (UPDLOCK) WHERE MaCTGD = @MaCTGD;

        INSERT INTO CT_BOI_THUONG (MaBoiThuong, MaCTGD)
        VALUES (@MaBoiThuong, @MaCTGD);

        WAITFOR DELAY '00:00:10'; 

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END;
GO