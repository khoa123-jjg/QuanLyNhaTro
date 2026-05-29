
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
IF OBJECT_ID('dbo.PHONG_TRO_TIEN_NGHI','U') IS NOT NULL DROP TABLE dbo.PHONG_TRO_TIEN_NGHI;
IF OBJECT_ID('dbo.TIEN_NGHI',          'U') IS NOT NULL DROP TABLE dbo.TIEN_NGHI;
IF OBJECT_ID('dbo.PHONG_TRO',          'U') IS NOT NULL DROP TABLE dbo.PHONG_TRO;
IF OBJECT_ID('dbo.NHA_TRO',            'U') IS NOT NULL DROP TABLE dbo.NHA_TRO;
IF OBJECT_ID('dbo.DUONG_PHO',          'U') IS NOT NULL DROP TABLE dbo.DUONG_PHO;
IF OBJECT_ID('dbo.XA',                   'U') IS NOT NULL DROP TABLE dbo.XA;
IF OBJECT_ID('dbo.QUANHUYEN',            'U') IS NOT NULL DROP TABLE dbo.QUANHUYEN;
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
    CONSTRAINT CK_NGUOI_DUNG_TrangThai   CHECK  (TrangThai IN ('HOAT_DONG','BI_KHOA'))
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

CREATE TABLE dbo.QUANHUYEN (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    TenQUANHUYEN        NVARCHAR(150) NOT NULL,
    ThanhPho        NVARCHAR(100) NOT NULL CONSTRAINT DF_QUANHUYE_ThanhPho  DEFAULT (N'Đà Nẵng'),
    TrangThai       VARCHAR(30)   NOT NULL CONSTRAINT DF_QUANHUYEN_TrangThai DEFAULT ('HIEN_THI'),
    CONSTRAINT CK_QUANHUYEN_TrangThai  CHECK (TrangThai IN ('HIEN_THI','AN')),
    CONSTRAINT UQ_DVHC_Ten_Loai   UNIQUE (TenQUANHUYEN, ThanhPho)
);
GO
CREATE TABLE dbo.XA (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    QUANHUYENId    INT           NOT NULL,
    TenXAHUYEN        NVARCHAR(150) NOT NULL,
    TrangThai       VARCHAR(30)   NOT NULL CONSTRAINT DF_XA_TrangThai DEFAULT ('HIEN_THI'),
    CONSTRAINT CK_XA_TrangThai  CHECK (TrangThai IN ('HIEN_THI','AN')),
    CONSTRAINT FK_DUONG_PHO_XA           FOREIGN KEY (QUANHUYENId) REFERENCES  dbo.QUANHUYEN(Id),
    CONSTRAINT UQ_XA_Ten_Loai   UNIQUE (TenXAHUYEN)
);
GO
CREATE TABLE dbo.DUONG_PHO (
    Id                  INT IDENTITY(1,1) PRIMARY KEY,
    XAId    INT           NOT NULL,
    TenDuong            NVARCHAR(150) NOT NULL,
    TrangThai           VARCHAR(30)   NOT NULL CONSTRAINT DF_DUONG_PHO_TrangThai DEFAULT ('HIEN_THI'),
    CONSTRAINT FK_DUONG_PHO_XAHUYEN          FOREIGN KEY (XAId) REFERENCES  dbo.XA(Id),
    CONSTRAINT CK_DUONG_PHO_TrangThai      CHECK (TrangThai IN ('HIEN_THI','AN')),
    CONSTRAINT UQ_DUONG_PHO_XAHUYEN_TenDuong  UNIQUE (XAId , TenDuong)
);
GO

/* ============================================================
   3. NHÓM NHÀ TRỌ / LOẠI PHÒNG / PHÒNG CỤ THỂ
   ============================================================ */

CREATE TABLE dbo.NHA_TRO (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    ChuNhaTroId     INT           NOT NULL,
    DuongPhoId          INT             NULL,
    TenNhaTro       NVARCHAR(200) NOT NULL,
    MoTa            NVARCHAR(MAX) NULL,
    SoNha               NVARCHAR(50)    NULL,
    DiaChiChiTiet       NVARCHAR(500)   NOT NULL,
    ViDo                DECIMAL(10,7)   NULL,
    KinhDo              DECIMAL(10,7)   NULL,
    TrangThai       VARCHAR(30)   NOT NULL CONSTRAINT DF_NHA_TRO_TrangThai DEFAULT ('HOAT_DONG'),
    NgayTao         DATETIME2(0)  NOT NULL CONSTRAINT DF_NHA_TRO_NgayTao   DEFAULT (SYSDATETIME()),
    NgayCapNhat     DATETIME2(0)  NULL,
    CONSTRAINT FK_NHA_TRO_CHU_NHA_TRO FOREIGN KEY (ChuNhaTroId) REFERENCES dbo.CHU_NHA_TRO(Id),
    CONSTRAINT FK_NHA_TRO_DUONG_PHO FOREIGN KEY (DuongPhoId)       REFERENCES dbo.DUONG_PHO(Id),
    CONSTRAINT CK_NHA_TRO_TrangThai   CHECK (TrangThai IN ('HOAT_DONG','TAM_AN','NGUNG_HOAT_DONG'))
);
GO


CREATE TABLE dbo.PHONG_TRO (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    NhaTroId        INT              NOT NULL,
    MaPhong         NVARCHAR(50)     NOT NULL,
    TenPhong        NVARCHAR(200)    NOT NULL,
    Tang            INT              NULL,
    DienTich        DECIMAL(8,2)     NOT NULL,
    GiaThueThang    DECIMAL(18,2)    NOT NULL,
    TienCoc         DECIMAL(18,2)    NOT NULL CONSTRAINT DF_PHONG_TRO_TienCoc DEFAULT (0),
    SoNguoiToiDa    INT              NULL,
    MoTa            NVARCHAR(MAX)    NULL,
    TrangThai       VARCHAR(30)      NOT NULL CONSTRAINT DF_PHONG_TRO_TrangThai DEFAULT ('TRONG'),
    GhiChu          NVARCHAR(500)    NULL,
    NgayTao         DATETIME2(0)     NOT NULL CONSTRAINT DF_PHONG_TRO_NgayTao DEFAULT (SYSDATETIME()),
    NgayCapNhat     DATETIME2(0)     NULL,
    CONSTRAINT FK_PHONG_TRO_NHA_TRO FOREIGN KEY (NhaTroId) REFERENCES dbo.NHA_TRO(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_PHONG_TRO_NhaTro_MaPhong UNIQUE (NhaTroId, MaPhong),
    CONSTRAINT CK_PHONG_TRO_Tang CHECK (Tang IS NULL OR Tang >= 0),
    CONSTRAINT CK_PHONG_TRO_DienTich CHECK (DienTich > 0),
    CONSTRAINT CK_PHONG_TRO_GiaThue CHECK (GiaThueThang >= 0),
    CONSTRAINT CK_PHONG_TRO_TienCoc CHECK (TienCoc >= 0),
    CONSTRAINT CK_PHONG_TRO_SoNguoi CHECK (SoNguoiToiDa IS NULL OR SoNguoiToiDa > 0),
    CONSTRAINT CK_PHONG_TRO_TrangThai CHECK (TrangThai IN ('TRONG','DANG_THUE','DANG_SUA','TAM_AN'))
);
GO

CREATE TABLE dbo.TIEN_NGHI (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    TenTienNghi     NVARCHAR(150) NOT NULL,
    TrangThai       VARCHAR(30)   NOT NULL CONSTRAINT DF_TIEN_NGHI_TrangThai DEFAULT ('HIEN_THI'),
    CONSTRAINT UQ_TIEN_NGHI_TenTienNghi UNIQUE (TenTienNghi),
    CONSTRAINT CK_TIEN_NGHI_TrangThai CHECK (TrangThai IN ('HIEN_THI','AN'))
);
GO

CREATE TABLE dbo.PHONG_TRO_TIEN_NGHI (
    PhongTroId      INT NOT NULL,
    TienNghiId      INT NOT NULL,
    CONSTRAINT PK_PHONG_TRO_TIEN_NGHI PRIMARY KEY (PhongTroId, TienNghiId),
    CONSTRAINT FK_PTTN_PHONG_TRO FOREIGN KEY (PhongTroId) REFERENCES dbo.PHONG_TRO(Id) ON DELETE CASCADE,
    CONSTRAINT FK_PTTN_TIEN_NGHI FOREIGN KEY (TienNghiId) REFERENCES dbo.TIEN_NGHI(Id)
);
GO


CREATE TABLE dbo.HINH_ANH (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    PhongTroId     INT           NOT NULL,
    DuongDanAnh     NVARCHAR(500) NOT NULL,
    LaAnhDaiDien    BIT           NOT NULL CONSTRAINT DF_HINH_ANH_LaAnhDaiDien DEFAULT (0),
    ThuTuHienThi    INT           NOT NULL CONSTRAINT DF_HINH_ANH_ThuTuHienThi DEFAULT (1),
    NgayTao         DATETIME2(0)  NOT NULL CONSTRAINT DF_HINH_ANH_NgayTao      DEFAULT (SYSDATETIME()),
    CONSTRAINT FK_HINH_ANH_PHONG_TRO FOREIGN KEY (PhongTroId) REFERENCES dbo.PHONG_TRO(Id) ON DELETE CASCADE,
    CONSTRAINT CK_HINH_ANH_ThuTu      CHECK (ThuTuHienThi > 0)
);
GO

CREATE UNIQUE INDEX UX_HINH_ANH_OneCover ON dbo.HINH_ANH(PhongTroId) WHERE LaAnhDaiDien = 1;
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
    PhongTroId        INT            NOT NULL,
    TieuDe          NVARCHAR(250)  NOT NULL,
    NoiDung         NVARCHAR(MAX)  NOT NULL,
    TrangThaiDuyet  VARCHAR(30)    NOT NULL CONSTRAINT DF_BAI_DANG_TrangThaiDuyet DEFAULT ('NHAP'),
    LyDoTuChoi      NVARCHAR(500)  NULL,
    NguoiDuyetId    INT            NULL,     -- Admin duyệt bài; NULL khi chưa duyệt
    NgayGuiDuyet    DATETIME2(0)   NULL,
    NgayDuyet       DATETIME2(0)   NULL,
    NgayTao         DATETIME2(0)   NOT NULL CONSTRAINT DF_BAI_DANG_NgayTao DEFAULT (SYSDATETIME()),
    NgayCapNhat     DATETIME2(0)   NULL,
    CONSTRAINT FK_BAI_DANG_NHA_TRO     FOREIGN KEY (PhongTroId)     REFERENCES dbo.PHONG_TRO(Id)    ON DELETE CASCADE,
    CONSTRAINT FK_BAI_DANG_NGUOI_DUYET FOREIGN KEY (NguoiDuyetId) REFERENCES dbo.NGUOI_DUNG(Id),
    CONSTRAINT CK_BAI_DANG_TrangThaiDuyet CHECK (TrangThaiDuyet IN ('NHAP','CHO_DUYET','DA_DUYET','TU_CHOI'))
);
GO


/* ============================================================
   5. NHÓM YÊU CẦU THUÊ
   ============================================================ */

CREATE TABLE dbo.DAT_THUE (
    Id                  INT IDENTITY(1,1) PRIMARY KEY,
    NguoiThueId         INT            NOT NULL,
    PhongTroId          INT            NOT NULL  ,
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
    CONSTRAINT FK_DAT_THUE_PHONG_TRO  FOREIGN KEY (PhongTroId)  REFERENCES dbo.PHONG_TRO(Id),
    CONSTRAINT CK_DAT_THUE_TrangThai  CHECK (TrangThai IN ('MOI','CHU_TRO_DONG_Y','CHU_TRO_TU_CHOI'))
);
GO

/* ============================================================
   6. INDEX TỐI ƯU TRA CỨU
   ============================================================ */

CREATE INDEX IX_NGUOI_DUNG_TrangThai    ON dbo.NGUOI_DUNG(TrangThai);
CREATE INDEX IX_CHU_NHA_TRO_NguoiDungId ON dbo.CHU_NHA_TRO(NguoiDungId);
CREATE INDEX IX_NGUOI_THUE_NguoiDungId  ON dbo.NGUOI_THUE(NguoiDungId);

CREATE INDEX IX_DUONG_PHO_DVHC          ON dbo.DUONG_PHO(XAid);

CREATE INDEX IX_NHA_TRO_ChuNhaTroId     ON dbo.NHA_TRO(ChuNhaTroId);
CREATE INDEX IX_NHA_TRO_TrangThai       ON dbo.NHA_TRO(TrangThai);
CREATE INDEX IX_PHONG_TRO_TrangThai     ON dbo.PHONG_TRO(TrangThai);
CREATE INDEX IX_HINH_ANH_PhongTroId    ON dbo.HINH_ANH(PhongTroId);

CREATE INDEX IX_BAI_DANG_TrangThaiDuyet ON dbo.BAI_DANG(TrangThaiDuyet);
CREATE INDEX IX_BAI_DANG_NguoiDuyetId   ON dbo.BAI_DANG(NguoiDuyetId);

CREATE INDEX IX_DAT_THUE_NguoiThueId    ON dbo.DAT_THUE(NguoiThueId);
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

INSERT INTO dbo.TIEN_NGHI (TenTienNghi, TrangThai)
VALUES
(N'Wifi',                     'HIEN_THI'),
(N'Máy lạnh',      'HIEN_THI'),
(N'Chỗ để xe',    'HIEN_THI'),
(N'Máy giặt',  'HIEN_THI'),
(N'WC riêng',    'HIEN_THI'),
(N'Gác lửng',      'HIEN_THI'),
(N'Camera an ninh',    'HIEN_THI'),
(N'Giờ giấc tự do',   'HIEN_THI');
GO

INSERT INTO dbo.QUANHUYEN(TenQUANHUYEN, ThanhPho, TrangThai)
VALUES
(N'Quận Hải Châu', N'Đà Nẵng', 'HIEN_THI'),
(N'Quận Thanh Khê', N'Đà Nẵng', 'HIEN_THI'),
(N'Quận Sơn Trà', N'Đà Nẵng', 'HIEN_THI');
GO



INSERT INTO dbo.XA(TenXAHUYEN, QUANHUYENId, TrangThai)
SELECT N'Phường Thạch Thang', Id, 'HIEN_THI' FROM dbo.QUANHUYEN WHERE TenQUANHUYEN = N'Quận Hải Châu';
INSERT INTO dbo.XA(TenXAHUYEN, QUANHUYENId, TrangThai)
SELECT N'Phường Hòa Cường Bắc', Id, 'HIEN_THI' FROM dbo.QUANHUYEN WHERE TenQUANHUYEN = N'Quận Hải Châu';
INSERT INTO dbo.XA(TenXAHUYEN, QUANHUYENId, TrangThai)
SELECT N'Phường Bình Thuận', Id, 'HIEN_THI' FROM dbo.QUANHUYEN WHERE TenQUANHUYEN = N'Quận Hải Châu';
-- Thuộc Quận Thanh Khê (2 phường)
INSERT INTO dbo.XA(TenXAHUYEN, QUANHUYENId, TrangThai)
SELECT N'Phường Thạc Gián', Id, 'HIEN_THI' FROM dbo.QUANHUYEN WHERE TenQUANHUYEN = N'Quận Thanh Khê';
INSERT INTO dbo.XA(TenXAHUYEN, QUANHUYENId, TrangThai)
SELECT N'Phường Hòa Khê', Id, 'HIEN_THI' FROM dbo.QUANHUYEN WHERE TenQUANHUYEN = N'Quận Thanh Khê';
-- Thuộc Quận Sơn Trà (2 phường)
INSERT INTO dbo.XA(TenXAHUYEN, QUANHUYENId, TrangThai)
SELECT N'Phường An Hải Tây', Id, 'HIEN_THI' FROM dbo.QUANHUYEN WHERE TenQUANHUYEN = N'Quận Sơn Trà';
INSERT INTO dbo.XA(TenXAHUYEN, QUANHUYENId, TrangThai)
SELECT N'Phường Phước Mỹ', Id, 'HIEN_THI' FROM dbo.QUANHUYEN WHERE TenQUANHUYEN = N'Quận Sơn Trà';
GO
INSERT INTO dbo.XA(TenXAHUYEN,QUANHUYENId,TrangThai)
SELECT  N'Phường Khuê Mỹ',Id, 'HIEN_THI' FROM dbo.QUANHUYEN WHERE TenQUANHUYEN = N'Quận Ngũ Hành Sơn';
GO 


INSERT INTO dbo.DUONG_PHO (XAId, TenDuong, TrangThai)
SELECT Id, N'Đường Bạch Đằng', 'HIEN_THI' FROM dbo.XA WHERE TenXAHUYEN = N'Phường Thạch Thang';
INSERT INTO dbo.DUONG_PHO (XAId, TenDuong, TrangThai)
SELECT Id, N'Đường Trần Phú', 'HIEN_THI' FROM dbo.XA WHERE TenXAHUYEN = N'Phường Thạch Thang';
-- Đường thuộc Phường Hòa Cường Bắc (Quận Hải Châu)
INSERT INTO dbo.DUONG_PHO (XAId, TenDuong, TrangThai)
SELECT Id, N'Đường Nguyễn Hữu Thọ', 'HIEN_THI' FROM dbo.XA WHERE TenXAHUYEN = N'Phường Hòa Cường Bắc';
INSERT INTO dbo.DUONG_PHO (XAId, TenDuong, TrangThai)
SELECT Id, N'Đường 2 Tháng 9', 'HIEN_THI' FROM dbo.XA WHERE TenXAHUYEN = N'Phường Hòa Cường Bắc';
-- Đường thuộc Phường Bình Thuận (Quận Hải Châu)
INSERT INTO dbo.DUONG_PHO (XAId, TenDuong, TrangThai)
SELECT Id, N'Đường Trưng Nữ Vương', 'HIEN_THI' FROM dbo.XA WHERE TenXAHUYEN = N'Phường Bình Thuận';
-- Đường thuộc Phường Thạc Gián (Quận Thanh Khê)
INSERT INTO dbo.DUONG_PHO (XAId, TenDuong, TrangThai)
SELECT Id, N'Đường Hàm Nghi', 'HIEN_THI' FROM dbo.XA WHERE TenXAHUYEN = N'Phường Thạc Gián';
INSERT INTO dbo.DUONG_PHO (XAId, TenDuong, TrangThai)
SELECT Id, N'Đường Nguyễn Văn Linh', 'HIEN_THI' FROM dbo.XA WHERE TenXAHUYEN = N'Phường Thạc Gián';
-- Đường thuộc Phường Hòa Khê (Quận Thanh Khê)
INSERT INTO dbo.DUONG_PHO (XAId, TenDuong, TrangThai)
SELECT Id, N'Đường Hà Huy Tập', 'HIEN_THI' FROM dbo.XA WHERE TenXAHUYEN = N'Phường Hòa Khê';
-- Đường thuộc Phường An Hải Tây (Quận Sơn Trà)
INSERT INTO dbo.DUONG_PHO (XAId, TenDuong, TrangThai)
SELECT Id, N'Đường Trần Hưng Đạo', 'HIEN_THI' FROM dbo.XA WHERE TenXAHUYEN = N'Phường An Hải Tây';
-- Đường thuộc Phường Phước Mỹ (Quận Sơn Trà)
INSERT INTO dbo.DUONG_PHO (XAId, TenDuong, TrangThai)
SELECT Id, N'Đường Võ Nguyên Giáp', 'HIEN_THI' FROM dbo.XA WHERE TenXAHUYEN = N'Phường Phước Mỹ';
GO



INSERT INTO dbo.DUONG_PHO (XAId, TenDuong, TrangThai)
SELECT Id, N'Nguyễn Văn Linh', 'HIEN_THI' FROM dbo.XA WHERE TenXAHUYEN = N'Phường Khuê Mỹ';

INSERT INTO dbo.NGUOI_DUNG (HoTen, Email, SoDienThoai, MatKhauHash, TrangThai)
VALUES (N'Admin System', N'admin@phongtrodanang.vn', N'0901234567', N'PLACEHOLDER_PASSWORD_HASH', 'HOAT_DONG');

INSERT INTO dbo.NGUOI_DUNG_VAI_TRO (NguoiDungId, VaiTroId)
SELECT nd.Id, vt.Id
FROM dbo.NGUOI_DUNG nd
JOIN dbo.VAI_TRO vt ON vt.TenVaiTro = 'ADMIN'
WHERE nd.Email = N'admin@phongtrodanang.vn';
GO
