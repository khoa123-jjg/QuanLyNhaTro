
/* ============================================================
   PhongTroDaNang_TinhGon_Database.sql
   ----------------------------------------------------------
   [SỬA 3NF] BAI_DANG
       Thông tin chủ trọ được suy ra qua JOIN 2 bảng:
       BAI_DANG.NhaTroId → NHA_TRO.ChuNhaTroId → CHU_NHA_TRO.NguoiDungId
   [SỬA 3NF] DIA_CHI
     + Trigger TR_DIA_CHI_Check_DVHC: khi DuongPhoId IS NOT NULL,
       DonViHanhChinhId phải khớp DUONG_PHO.DonViHanhChinhId.

   [SỬA RÀNG BUỘC NGHIỆP VỤ] BAI_DANG_LOAI_PHONG
     + Trigger TR_BDLP_Check_NhaTro: LoaiPhongId phải cùng NhaTro
       với bài đăng.

   [SỬA RÀNG BUỘC NGHIỆP VỤ] DAT_THUE
     + Trigger TR_DAT_THUE_Check_Phong: PhongTroId phải thuộc
       đúng LoaiPhongId đã chọn.
   ============================================================
*/

IF DB_ID(N'PhongTroDaNang') IS NULL
BEGIN
    CREATE DATABASE PhongTroDaNang;
END
GO

USE PhongTroDaNang;
GO

/* Xoa trigger truoc khi xoa bang */
IF OBJECT_ID('dbo.TR_DAT_THUE_Check_Phong', 'TR') IS NOT NULL DROP TRIGGER dbo.TR_DAT_THUE_Check_Phong;
IF OBJECT_ID('dbo.TR_BDLP_Check_NhaTro',    'TR') IS NOT NULL DROP TRIGGER dbo.TR_BDLP_Check_NhaTro;
IF OBJECT_ID('dbo.TR_DIA_CHI_Check_DVHC',   'TR') IS NOT NULL DROP TRIGGER dbo.TR_DIA_CHI_Check_DVHC;

IF OBJECT_ID('dbo.DAT_THUE',           'U') IS NOT NULL DROP TABLE dbo.DAT_THUE;
IF OBJECT_ID('dbo.BAI_DANG_LOAI_PHONG','U') IS NOT NULL DROP TABLE dbo.BAI_DANG_LOAI_PHONG;
IF OBJECT_ID('dbo.BAI_DANG',           'U') IS NOT NULL DROP TABLE dbo.BAI_DANG;
IF OBJECT_ID('dbo.HINH_ANH',           'U') IS NOT NULL DROP TABLE dbo.HINH_ANH;
IF OBJECT_ID('dbo.LOAI_PHONG_TIEN_NGHI','U') IS NOT NULL DROP TABLE dbo.LOAI_PHONG_TIEN_NGHI;
IF OBJECT_ID('dbo.TIEN_NGHI',          'U') IS NOT NULL DROP TABLE dbo.TIEN_NGHI;
IF OBJECT_ID('dbo.PHONG_TRO',          'U') IS NOT NULL DROP TABLE dbo.PHONG_TRO;
IF OBJECT_ID('dbo.LOAI_PHONG',         'U') IS NOT NULL DROP TABLE dbo.LOAI_PHONG;
IF OBJECT_ID('dbo.NHA_TRO',            'U') IS NOT NULL DROP TABLE dbo.NHA_TRO;
IF OBJECT_ID('dbo.DIA_CHI',            'U') IS NOT NULL DROP TABLE dbo.DIA_CHI;
IF OBJECT_ID('dbo.DUONG_PHO',          'U') IS NOT NULL DROP TABLE dbo.DUONG_PHO;
IF OBJECT_ID('dbo.DON_VI_HANH_CHINH',  'U') IS NOT NULL DROP TABLE dbo.DON_VI_HANH_CHINH;
IF OBJECT_ID('dbo.NGUOI_THUE',         'U') IS NOT NULL DROP TABLE dbo.NGUOI_THUE;
IF OBJECT_ID('dbo.CHU_NHA_TRO',        'U') IS NOT NULL DROP TABLE dbo.CHU_NHA_TRO;
IF OBJECT_ID('dbo.NGUOI_DUNG_VAI_TRO', 'U') IS NOT NULL DROP TABLE dbo.NGUOI_DUNG_VAI_TRO;
IF OBJECT_ID('dbo.VAI_TRO',            'U') IS NOT NULL DROP TABLE dbo.VAI_TRO;
IF OBJECT_ID('dbo.NGUOI_DUNG',         'U') IS NOT NULL DROP TABLE dbo.NGUOI_DUNG;
GO

/* ============================================================
   1. NHÓM NGƯỜI DÙNG & PHÂN QUYỀN
   ============================================================ */

CREATE TABLE dbo.NGUOI_DUNG (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    HoTen           NVARCHAR(150)  NOT NULL,
    Email           NVARCHAR(255)  NOT NULL,
    SoDienThoai     NVARCHAR(20)   NULL,
    MatKhauHash     NVARCHAR(500)  NOT NULL,
    AnhDaiDien      NVARCHAR(500)  NULL,
    TrangThai       VARCHAR(30)    NOT NULL CONSTRAINT DF_NGUOI_DUNG_TrangThai DEFAULT ('HOAT_DONG'),
    GhiChu          NVARCHAR(500)  NULL,
    NgayTao         DATETIME2(0)   NOT NULL CONSTRAINT DF_NGUOI_DUNG_NgayTao  DEFAULT (SYSDATETIME()),
    NgayCapNhat     DATETIME2(0)   NULL,
    CONSTRAINT UQ_NGUOI_DUNG_Email       UNIQUE (Email),
    CONSTRAINT UQ_NGUOI_DUNG_SoDienThoai UNIQUE (SoDienThoai),
    CONSTRAINT CK_NGUOI_DUNG_TrangThai   CHECK  (TrangThai IN ('HOAT_DONG','BI_KHOA','CHO_XAC_THUC'))
);
GO

CREATE TABLE dbo.VAI_TRO (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    TenVaiTro       VARCHAR(50)    NOT NULL,
    MoTa            NVARCHAR(255)  NULL,
    CONSTRAINT UQ_VAI_TRO_TenVaiTro UNIQUE (TenVaiTro)
);
GO

CREATE TABLE dbo.NGUOI_DUNG_VAI_TRO (
    NguoiDungId     INT          NOT NULL,
    VaiTroId        INT          NOT NULL,
    NgayGan         DATETIME2(0) NOT NULL CONSTRAINT DF_NDVT_NgayGan DEFAULT (SYSDATETIME()),
    CONSTRAINT PK_NGUOI_DUNG_VAI_TRO PRIMARY KEY (NguoiDungId, VaiTroId),
    CONSTRAINT FK_NDVT_NGUOI_DUNG FOREIGN KEY (NguoiDungId) REFERENCES dbo.NGUOI_DUNG(Id) ON DELETE CASCADE,
    CONSTRAINT FK_NDVT_VAI_TRO    FOREIGN KEY (VaiTroId)    REFERENCES dbo.VAI_TRO(Id)     ON DELETE CASCADE
);
GO

CREATE TABLE dbo.CHU_NHA_TRO (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    NguoiDungId     INT          NOT NULL,
    TrangThaiHoSo   VARCHAR(30)  NOT NULL CONSTRAINT DF_CHU_NHA_TRO_TrangThaiHoSo DEFAULT ('HOAT_DONG'),
    NgayTao         DATETIME2(0) NOT NULL CONSTRAINT DF_CHU_NHA_TRO_NgayTao       DEFAULT (SYSDATETIME()),
    NgayCapNhat     DATETIME2(0) NULL,
    CONSTRAINT UQ_CHU_NHA_TRO_NguoiDungId   UNIQUE (NguoiDungId),
    CONSTRAINT FK_CHU_NHA_TRO_NGUOI_DUNG    FOREIGN KEY (NguoiDungId) REFERENCES dbo.NGUOI_DUNG(Id),
    CONSTRAINT CK_CHU_NHA_TRO_TrangThaiHoSo CHECK (TrangThaiHoSo IN ('HOAT_DONG','TAM_KHOA','CHO_DUYET'))
);
GO

CREATE TABLE dbo.NGUOI_THUE (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    NguoiDungId     INT           NOT NULL,
    NgheNghiep      NVARCHAR(150) NULL,
    NhuCauThue      NVARCHAR(255) NULL,
    NgayTao         DATETIME2(0)  NOT NULL CONSTRAINT DF_NGUOI_THUE_NgayTao DEFAULT (SYSDATETIME()),
    NgayCapNhat     DATETIME2(0)  NULL,
    CONSTRAINT UQ_NGUOI_THUE_NguoiDungId UNIQUE (NguoiDungId),
    CONSTRAINT FK_NGUOI_THUE_NGUOI_DUNG  FOREIGN KEY (NguoiDungId) REFERENCES dbo.NGUOI_DUNG(Id)
);
GO

/* ============================================================
   2. NHÓM ĐỊA CHỈ
   ============================================================ */

CREATE TABLE dbo.DON_VI_HANH_CHINH (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    TenDonVi        NVARCHAR(150) NOT NULL,
    LoaiDonVi       VARCHAR(30)   NOT NULL,
    ThanhPho        NVARCHAR(100) NOT NULL CONSTRAINT DF_DVHC_ThanhPho  DEFAULT (N'Đà Nẵng'),
    TrangThai       VARCHAR(30)   NOT NULL CONSTRAINT DF_DVHC_TrangThai DEFAULT ('HIEN_THI'),
    CONSTRAINT CK_DVHC_LoaiDonVi CHECK (LoaiDonVi IN ('PHUONG','XA','DAC_KHU')),
    CONSTRAINT CK_DVHC_TrangThai  CHECK (TrangThai IN ('HIEN_THI','AN')),
    CONSTRAINT UQ_DVHC_Ten_Loai   UNIQUE (TenDonVi, LoaiDonVi, ThanhPho)
);
GO

CREATE TABLE dbo.DUONG_PHO (
    Id                  INT IDENTITY(1,1) PRIMARY KEY,
    DonViHanhChinhId    INT           NOT NULL,
    TenDuong            NVARCHAR(150) NOT NULL,
    TrangThai           VARCHAR(30)   NOT NULL CONSTRAINT DF_DUONG_PHO_TrangThai DEFAULT ('HIEN_THI'),
    CONSTRAINT FK_DUONG_PHO_DVHC           FOREIGN KEY (DonViHanhChinhId) REFERENCES dbo.DON_VI_HANH_CHINH(Id),
    CONSTRAINT CK_DUONG_PHO_TrangThai      CHECK (TrangThai IN ('HIEN_THI','AN')),
    CONSTRAINT UQ_DUONG_PHO_DVHC_TenDuong  UNIQUE (DonViHanhChinhId, TenDuong)
);
GO

/*
   [SỬA 3NF] DIA_CHI
   Vấn đề: Khi DuongPhoId IS NOT NULL, tồn tại phụ thuộc bắc cầu:
     Id → DuongPhoId → DUONG_PHO.DonViHanhChinhId
   Nghĩa là DonViHanhChinhId ở bảng này có thể mâu thuẫn với
   DonViHanhChinhId của đường phố được chọn.
   Giải pháp: Giữ nguyên cấu trúc bảng (vì DuongPhoId là nullable, cần
   lưu trực tiếp DonViHanhChinhId cho trường hợp không có đường phố),
   nhưng bổ sung trigger TR_DIA_CHI_Check_DVHC để enforce tính nhất quán.
*/
CREATE TABLE dbo.DIA_CHI (
    Id                  INT IDENTITY(1,1) PRIMARY KEY,
    DonViHanhChinhId    INT             NOT NULL,
    DuongPhoId          INT             NULL,
    SoNha               NVARCHAR(50)    NULL,
    DiaChiChiTiet       NVARCHAR(500)   NOT NULL,
    ViDo                DECIMAL(10,7)   NULL,
    KinhDo              DECIMAL(10,7)   NULL,
    CONSTRAINT FK_DIA_CHI_DVHC      FOREIGN KEY (DonViHanhChinhId) REFERENCES dbo.DON_VI_HANH_CHINH(Id),
    CONSTRAINT FK_DIA_CHI_DUONG_PHO FOREIGN KEY (DuongPhoId)       REFERENCES dbo.DUONG_PHO(Id),
    CONSTRAINT CK_DIA_CHI_ViDo      CHECK (ViDo  IS NULL OR (ViDo  BETWEEN -90  AND 90)),
    CONSTRAINT CK_DIA_CHI_KinhDo    CHECK (KinhDo IS NULL OR (KinhDo BETWEEN -180 AND 180))
);
GO

/* ============================================================
   TRIGGER 1: DIA_CHI — đảm bảo tính nhất quán DonViHanhChinhId
   Khi DuongPhoId IS NOT NULL, DIA_CHI.DonViHanhChinhId phải bằng
   DUONG_PHO.DonViHanhChinhId của đường đó.
   ============================================================ */
CREATE TRIGGER dbo.TR_DIA_CHI_Check_DVHC
ON dbo.DIA_CHI
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN dbo.DUONG_PHO dp ON dp.Id = i.DuongPhoId
        WHERE i.DuongPhoId IS NOT NULL
          AND i.DonViHanhChinhId <> dp.DonViHanhChinhId
    )
    BEGIN
        RAISERROR (
            N'DIA_CHI: DonViHanhChinhId không khớp với DonViHanhChinhId của DuongPhoId được chọn. Vui lòng dùng cùng phường/xã.',
            16, 1
        );
        ROLLBACK TRANSACTION;
    END
END;
GO

/* ============================================================
   3. NHÓM NHÀ TRỌ / LOẠI PHÒNG / PHÒNG CỤ THỂ
   ============================================================ */

CREATE TABLE dbo.NHA_TRO (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    ChuNhaTroId     INT           NOT NULL,
    DiaChiId        INT           NOT NULL,
    TenNhaTro       NVARCHAR(200) NOT NULL,
    MoTa            NVARCHAR(MAX) NULL,
    TrangThai       VARCHAR(30)   NOT NULL CONSTRAINT DF_NHA_TRO_TrangThai DEFAULT ('HOAT_DONG'),
    NgayTao         DATETIME2(0)  NOT NULL CONSTRAINT DF_NHA_TRO_NgayTao   DEFAULT (SYSDATETIME()),
    NgayCapNhat     DATETIME2(0)  NULL,
    CONSTRAINT FK_NHA_TRO_CHU_NHA_TRO FOREIGN KEY (ChuNhaTroId) REFERENCES dbo.CHU_NHA_TRO(Id),
    CONSTRAINT FK_NHA_TRO_DIA_CHI     FOREIGN KEY (DiaChiId)    REFERENCES dbo.DIA_CHI(Id),
    CONSTRAINT UQ_NHA_TRO_DiaChiId    UNIQUE (DiaChiId),
    CONSTRAINT CK_NHA_TRO_TrangThai   CHECK (TrangThai IN ('HOAT_DONG','TAM_AN','NGUNG_HOAT_DONG'))
);
GO

CREATE TABLE dbo.LOAI_PHONG (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    NhaTroId        INT              NOT NULL,
    TenLoaiPhong    NVARCHAR(200)    NOT NULL,
    DienTich        DECIMAL(8,2)     NOT NULL,
    GiaThueThang    DECIMAL(18,2)    NOT NULL,
    TienCoc         DECIMAL(18,2)    NOT NULL CONSTRAINT DF_LOAI_PHONG_TienCoc DEFAULT (0),
    SoNguoiToiDa    INT              NULL,
    MoTa            NVARCHAR(MAX)    NULL,
    TrangThai       VARCHAR(30)      NOT NULL CONSTRAINT DF_LOAI_PHONG_TrangThai DEFAULT ('HOAT_DONG'),
    NgayTao         DATETIME2(0)     NOT NULL CONSTRAINT DF_LOAI_PHONG_NgayTao   DEFAULT (SYSDATETIME()),
    NgayCapNhat     DATETIME2(0)     NULL,
    CONSTRAINT FK_LOAI_PHONG_NHA_TRO   FOREIGN KEY (NhaTroId) REFERENCES dbo.NHA_TRO(Id) ON DELETE CASCADE,
    CONSTRAINT CK_LOAI_PHONG_DienTich  CHECK (DienTich > 0),
    CONSTRAINT CK_LOAI_PHONG_GiaThue   CHECK (GiaThueThang >= 0),
    CONSTRAINT CK_LOAI_PHONG_TienCoc   CHECK (TienCoc >= 0),
    CONSTRAINT CK_LOAI_PHONG_SoNguoi   CHECK (SoNguoiToiDa IS NULL OR SoNguoiToiDa > 0),
    CONSTRAINT CK_LOAI_PHONG_TrangThai CHECK (TrangThai IN ('HOAT_DONG','TAM_AN','NGUNG_NHAN_YEU_CAU'))
);
GO

CREATE TABLE dbo.PHONG_TRO (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    LoaiPhongId     INT           NOT NULL,
    MaPhong         NVARCHAR(50)  NOT NULL,
    Tang            INT           NULL,
    TrangThai       VARCHAR(30)   NOT NULL CONSTRAINT DF_PHONG_TRO_TrangThai DEFAULT ('TRONG'),
    GhiChu          NVARCHAR(500) NULL,
    NgayTao         DATETIME2(0)  NOT NULL CONSTRAINT DF_PHONG_TRO_NgayTao   DEFAULT (SYSDATETIME()),
    NgayCapNhat     DATETIME2(0)  NULL,
    CONSTRAINT FK_PHONG_TRO_LOAI_PHONG       FOREIGN KEY (LoaiPhongId) REFERENCES dbo.LOAI_PHONG(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_PHONG_TRO_LoaiPhong_MaPhong UNIQUE (LoaiPhongId, MaPhong),
    CONSTRAINT CK_PHONG_TRO_Tang             CHECK (Tang IS NULL OR Tang >= 0),
    CONSTRAINT CK_PHONG_TRO_TrangThai        CHECK (TrangThai IN ('TRONG','DANG_THUE','DANG_SUA','TAM_AN'))
);
GO

CREATE TABLE dbo.TIEN_NGHI (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    TenTienNghi     NVARCHAR(150) NOT NULL,
    Icon            NVARCHAR(100) NULL,
    TrangThai       VARCHAR(30)   NOT NULL CONSTRAINT DF_TIEN_NGHI_TrangThai DEFAULT ('HIEN_THI'),
    CONSTRAINT UQ_TIEN_NGHI_TenTienNghi UNIQUE (TenTienNghi),
    CONSTRAINT CK_TIEN_NGHI_TrangThai   CHECK  (TrangThai IN ('HIEN_THI','AN'))
);
GO

CREATE TABLE dbo.LOAI_PHONG_TIEN_NGHI (
    LoaiPhongId     INT NOT NULL,
    TienNghiId      INT NOT NULL,
    CONSTRAINT PK_LOAI_PHONG_TIEN_NGHI PRIMARY KEY (LoaiPhongId, TienNghiId),
    CONSTRAINT FK_LPTN_LOAI_PHONG FOREIGN KEY (LoaiPhongId) REFERENCES dbo.LOAI_PHONG(Id) ON DELETE CASCADE,
    CONSTRAINT FK_LPTN_TIEN_NGHI  FOREIGN KEY (TienNghiId)  REFERENCES dbo.TIEN_NGHI(Id)
);
GO

CREATE TABLE dbo.HINH_ANH (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    LoaiPhongId     INT           NOT NULL,
    DuongDanAnh     NVARCHAR(500) NOT NULL,
    LaAnhDaiDien    BIT           NOT NULL CONSTRAINT DF_HINH_ANH_LaAnhDaiDien DEFAULT (0),
    ThuTuHienThi    INT           NOT NULL CONSTRAINT DF_HINH_ANH_ThuTuHienThi DEFAULT (1),
    NgayTao         DATETIME2(0)  NOT NULL CONSTRAINT DF_HINH_ANH_NgayTao      DEFAULT (SYSDATETIME()),
    CONSTRAINT FK_HINH_ANH_LOAI_PHONG FOREIGN KEY (LoaiPhongId) REFERENCES dbo.LOAI_PHONG(Id) ON DELETE CASCADE,
    CONSTRAINT CK_HINH_ANH_ThuTu      CHECK (ThuTuHienThi > 0)
);
GO

CREATE UNIQUE INDEX UX_HINH_ANH_OneCover ON dbo.HINH_ANH(LoaiPhongId) WHERE LaAnhDaiDien = 1;
GO

/* ============================================================
   4. NHÓM BÀI ĐĂNG / KIỂM DUYỆT
   ============================================================
   [v4 - ĐẠT 3NF] NguoiDungId đã được xóa khỏi bảng này.
   Để biết ai tạo bài đăng, JOIN qua 2 bảng trung gian:
     BAI_DANG.NhaTroId
       → NHA_TRO.ChuNhaTroId
       → CHU_NHA_TRO.NguoiDungId
   ============================================================ */

CREATE TABLE dbo.BAI_DANG (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    NhaTroId        INT            NOT NULL,
    TieuDe          NVARCHAR(250)  NOT NULL,
    NoiDung         NVARCHAR(MAX)  NOT NULL,
    TrangThaiDuyet  VARCHAR(30)    NOT NULL CONSTRAINT DF_BAI_DANG_TrangThaiDuyet DEFAULT ('NHAP'),
    LyDoTuChoi      NVARCHAR(500)  NULL,
    NguoiDuyetId    INT            NULL,     -- Admin duyệt bài; NULL khi chưa duyệt
    NgayGuiDuyet    DATETIME2(0)   NULL,
    NgayDuyet       DATETIME2(0)   NULL,
    NgayTao         DATETIME2(0)   NOT NULL CONSTRAINT DF_BAI_DANG_NgayTao DEFAULT (SYSDATETIME()),
    NgayCapNhat     DATETIME2(0)   NULL,
    CONSTRAINT FK_BAI_DANG_NHA_TRO     FOREIGN KEY (NhaTroId)     REFERENCES dbo.NHA_TRO(Id)    ON DELETE CASCADE,
    CONSTRAINT FK_BAI_DANG_NGUOI_DUYET FOREIGN KEY (NguoiDuyetId) REFERENCES dbo.NGUOI_DUNG(Id),
    CONSTRAINT CK_BAI_DANG_TrangThaiDuyet CHECK (TrangThaiDuyet IN ('NHAP','CHO_DUYET','DA_DUYET','TU_CHOI','AN'))
);
GO

CREATE TABLE dbo.BAI_DANG_LOAI_PHONG (
    BaiDangId       INT NOT NULL,
    LoaiPhongId     INT NOT NULL,
    CONSTRAINT PK_BAI_DANG_LOAI_PHONG PRIMARY KEY (BaiDangId, LoaiPhongId),
    CONSTRAINT FK_BDLP_BAI_DANG   FOREIGN KEY (BaiDangId)   REFERENCES dbo.BAI_DANG(Id)   ON DELETE CASCADE,
    CONSTRAINT FK_BDLP_LOAI_PHONG FOREIGN KEY (LoaiPhongId) REFERENCES dbo.LOAI_PHONG(Id)
);
GO

/* ============================================================
   TRIGGER 3: BAI_DANG_LOAI_PHONG — ràng buộc nghiệp vụ
   LoaiPhongId phải thuộc cùng NhaTroId với BAI_DANG.
   Không thể biểu diễn bằng FK đơn thuần vì liên quan đến 2 bảng cha.
   ============================================================ */
CREATE TRIGGER dbo.TR_BDLP_Check_NhaTro
ON dbo.BAI_DANG_LOAI_PHONG
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN dbo.BAI_DANG  bd ON bd.Id = i.BaiDangId
        JOIN dbo.LOAI_PHONG lp ON lp.Id = i.LoaiPhongId
        WHERE lp.NhaTroId <> bd.NhaTroId
    )
    BEGIN
        RAISERROR (
            N'BAI_DANG_LOAI_PHONG: LoaiPhongId không thuộc cùng NhaTroId với bài đăng.',
            16, 1
        );
        ROLLBACK TRANSACTION;
    END
END;
GO

/* ============================================================
   5. NHÓM YÊU CẦU THUÊ
   ============================================================ */

CREATE TABLE dbo.DAT_THUE (
    Id                  INT IDENTITY(1,1) PRIMARY KEY,
    NguoiThueId         INT            NOT NULL,
    LoaiPhongId         INT            NOT NULL,
    PhongTroId          INT            NULL,    -- chủ trọ xác nhận sau
    /*
       Snapshot pattern (hợp lệ, không vi phạm 3NF):
       HoTenLienHe và SoDienThoaiLienHe được lưu tại thời điểm đặt
       để bảo toàn thông tin liên hệ kể cả khi người dùng thay đổi
       thông tin cá nhân sau này.
    */
    HoTenLienHe         NVARCHAR(150)  NOT NULL,
    SoDienThoaiLienHe   NVARCHAR(20)   NOT NULL,
    LoiNhan             NVARCHAR(1000) NULL,
    NgayMuonXemPhong    DATETIME2(0)   NULL,
    TrangThai           VARCHAR(30)    NOT NULL CONSTRAINT DF_DAT_THUE_TrangThai DEFAULT ('MOI'),
    LyDoTuChoi          NVARCHAR(500)  NULL,
    GhiChuChuTro        NVARCHAR(500)  NULL,
    NgayXuLy            DATETIME2(0)   NULL,
    NgayTao             DATETIME2(0)   NOT NULL CONSTRAINT DF_DAT_THUE_NgayTao DEFAULT (SYSDATETIME()),
    NgayCapNhat         DATETIME2(0)   NULL,
    CONSTRAINT FK_DAT_THUE_NGUOI_THUE FOREIGN KEY (NguoiThueId) REFERENCES dbo.NGUOI_THUE(Id),
    CONSTRAINT FK_DAT_THUE_LOAI_PHONG FOREIGN KEY (LoaiPhongId) REFERENCES dbo.LOAI_PHONG(Id),
    CONSTRAINT FK_DAT_THUE_PHONG_TRO  FOREIGN KEY (PhongTroId)  REFERENCES dbo.PHONG_TRO(Id),
    CONSTRAINT CK_DAT_THUE_TrangThai  CHECK (TrangThai IN ('MOI','CHU_TRO_DONG_Y','CHU_TRO_TU_CHOI','NGUOI_THUE_HUY'))
);
GO

/* ============================================================
   TRIGGER 4: DAT_THUE — ràng buộc nghiệp vụ
   PhongTroId (khi không NULL) phải thuộc đúng LoaiPhongId đã chọn.
   ============================================================ */
CREATE TRIGGER dbo.TR_DAT_THUE_Check_Phong
ON dbo.DAT_THUE
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN dbo.PHONG_TRO pt ON pt.Id = i.PhongTroId
        WHERE i.PhongTroId IS NOT NULL
          AND pt.LoaiPhongId <> i.LoaiPhongId
    )
    BEGIN
        RAISERROR (
            N'DAT_THUE: PhongTroId không thuộc LoaiPhongId đã chọn.',
            16, 1
        );
        ROLLBACK TRANSACTION;
    END
END;
GO

/* ============================================================
   6. INDEX TỐI ƯU TRA CỨU
   ============================================================ */

CREATE INDEX IX_NGUOI_DUNG_TrangThai    ON dbo.NGUOI_DUNG(TrangThai);
CREATE INDEX IX_CHU_NHA_TRO_NguoiDungId ON dbo.CHU_NHA_TRO(NguoiDungId);
CREATE INDEX IX_NGUOI_THUE_NguoiDungId  ON dbo.NGUOI_THUE(NguoiDungId);

CREATE INDEX IX_DUONG_PHO_DVHC          ON dbo.DUONG_PHO(DonViHanhChinhId);
CREATE INDEX IX_DIA_CHI_DVHC_DUONG      ON dbo.DIA_CHI(DonViHanhChinhId, DuongPhoId);

CREATE INDEX IX_NHA_TRO_ChuNhaTroId     ON dbo.NHA_TRO(ChuNhaTroId);
CREATE INDEX IX_NHA_TRO_TrangThai       ON dbo.NHA_TRO(TrangThai);
CREATE INDEX IX_LOAI_PHONG_NhaTroId     ON dbo.LOAI_PHONG(NhaTroId);
CREATE INDEX IX_LOAI_PHONG_Search       ON dbo.LOAI_PHONG(TrangThai, GiaThueThang, DienTich);
CREATE INDEX IX_PHONG_TRO_LoaiPhongId   ON dbo.PHONG_TRO(LoaiPhongId);
CREATE INDEX IX_PHONG_TRO_TrangThai     ON dbo.PHONG_TRO(TrangThai);
CREATE INDEX IX_HINH_ANH_LoaiPhongId    ON dbo.HINH_ANH(LoaiPhongId);

CREATE INDEX IX_BAI_DANG_TrangThaiDuyet ON dbo.BAI_DANG(TrangThaiDuyet);
CREATE INDEX IX_BAI_DANG_NhaTroId       ON dbo.BAI_DANG(NhaTroId);
CREATE INDEX IX_BAI_DANG_NguoiDuyetId   ON dbo.BAI_DANG(NguoiDuyetId);

CREATE INDEX IX_DAT_THUE_NguoiThueId    ON dbo.DAT_THUE(NguoiThueId);
CREATE INDEX IX_DAT_THUE_LoaiPhongId    ON dbo.DAT_THUE(LoaiPhongId);
CREATE INDEX IX_DAT_THUE_PhongTroId     ON dbo.DAT_THUE(PhongTroId);
CREATE INDEX IX_DAT_THUE_TrangThai      ON dbo.DAT_THUE(TrangThai);
GO

/* ============================================================
   7. SEED DATA CƠ BẢN
   ============================================================ */

INSERT INTO dbo.VAI_TRO (TenVaiTro, MoTa)
VALUES
('ADMIN',       N'Quản trị viên hệ thống'),
('CHU_TRO',     N'Chủ nhà trọ / chủ phòng trọ'),
('NGUOI_THUE',  N'Người thuê / người tìm phòng');
GO

INSERT INTO dbo.TIEN_NGHI (TenTienNghi, Icon, TrangThai)
VALUES
(N'Wifi',            'wifi',            'HIEN_THI'),
(N'Máy lạnh',        'air-conditioner', 'HIEN_THI'),
(N'Chỗ để xe',       'parking',         'HIEN_THI'),
(N'Máy giặt',        'washing-machine', 'HIEN_THI'),
(N'WC riêng',        'toilet',          'HIEN_THI'),
(N'Gác lửng',        'loft',            'HIEN_THI'),
(N'Camera an ninh',  'camera',          'HIEN_THI'),
(N'Giờ giấc tự do',  'clock',           'HIEN_THI');
GO

INSERT INTO dbo.DON_VI_HANH_CHINH (TenDonVi, LoaiDonVi, ThanhPho, TrangThai)
VALUES
(N'Phường Hải Châu',     'PHUONG', N'Đà Nẵng', 'HIEN_THI'),
(N'Phường Thanh Khê',    'PHUONG', N'Đà Nẵng', 'HIEN_THI'),
(N'Phường Sơn Trà',      'PHUONG', N'Đà Nẵng', 'HIEN_THI'),
(N'Phường Liên Chiểu',   'PHUONG', N'Đà Nẵng', 'HIEN_THI'),
(N'Phường Ngũ Hành Sơn', 'PHUONG', N'Đà Nẵng', 'HIEN_THI'),
(N'Xã Hòa Vang',         'XA',     N'Đà Nẵng', 'HIEN_THI');
GO

INSERT INTO dbo.DUONG_PHO (DonViHanhChinhId, TenDuong, TrangThai)
SELECT Id, N'Nguyễn Văn Linh', 'HIEN_THI' FROM dbo.DON_VI_HANH_CHINH WHERE TenDonVi = N'Phường Hải Châu';

INSERT INTO dbo.DUONG_PHO (DonViHanhChinhId, TenDuong, TrangThai)
SELECT Id, N'Điện Biên Phủ',   'HIEN_THI' FROM dbo.DON_VI_HANH_CHINH WHERE TenDonVi = N'Phường Thanh Khê';

INSERT INTO dbo.DUONG_PHO (DonViHanhChinhId, TenDuong, TrangThai)
SELECT Id, N'Tôn Đức Thắng',   'HIEN_THI' FROM dbo.DON_VI_HANH_CHINH WHERE TenDonVi = N'Phường Liên Chiểu';
GO

INSERT INTO dbo.NGUOI_DUNG (HoTen, Email, SoDienThoai, MatKhauHash, TrangThai)
VALUES (N'Admin System', N'admin@phongtrodanang.vn', N'0901234567', N'PLACEHOLDER_PASSWORD_HASH', 'HOAT_DONG');

INSERT INTO dbo.NGUOI_DUNG_VAI_TRO (NguoiDungId, VaiTroId)
SELECT nd.Id, vt.Id
FROM dbo.NGUOI_DUNG nd
JOIN dbo.VAI_TRO vt ON vt.TenVaiTro = 'ADMIN'
WHERE nd.Email = N'admin@phongtrodanang.vn';
GO

/* ============================================================
   GHI CHÚ THIẾT KẾ (v4 — đạt 3NF):

   CÁC VI PHẠM ĐÃ XỬ LÝ:
   1. [3NF - BAI_DANG] Xóa NguoiDungId vì suy ra được qua JOIN:
        BAI_DANG.NhaTroId → NHA_TRO.ChuNhaTroId → CHU_NHA_TRO.NguoiDungId
      Khi cần lấy thông tin chủ trọ tạo bài, dùng query:
        SELECT nd.*
        FROM BAI_DANG bd
        JOIN NHA_TRO nt  ON nt.Id = bd.NhaTroId
        JOIN CHU_NHA_TRO c ON c.Id = nt.ChuNhaTroId
        JOIN NGUOI_DUNG nd ON nd.Id = c.NguoiDungId
        WHERE bd.Id = @BaiDangId

   2. [3NF - DIA_CHI] Phụ thuộc bắc cầu khi DuongPhoId IS NOT NULL.
      → Đã bổ sung trigger TR_DIA_CHI_Check_DVHC để enforce nhất quán.

   3. [Ràng buộc nghiệp vụ - BAI_DANG_LOAI_PHONG]
      LoaiPhongId phải cùng NhaTro với bài đăng.
      → Bổ sung trigger TR_BDLP_Check_NhaTro.

   4. [Ràng buộc nghiệp vụ - DAT_THUE]
      PhongTroId phải thuộc LoaiPhongId đã chọn.
      → Bổ sung trigger TR_DAT_THUE_Check_Phong.

   CHẤP NHẬN (có giải thích):
   5. DAT_THUE: HoTenLienHe, SoDienThoaiLienHe lưu trùng với NGUOI_DUNG.
      → Snapshot pattern hợp lệ: ghi nhận thông tin tại thời điểm đặt.

   6. DON_VI_HANH_CHINH: ThanhPho luôn = 'Đà Nẵng' trong dữ liệu hiện tại.
      → Giữ lại để hỗ trợ mở rộng thành phố khác sau này.

   KHÔNG LƯU (tính được):
   7. TongSoPhong, SoPhongTrong → tính từ PHONG_TRO.
   ============================================================ */
