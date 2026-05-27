/* ============================================================
   PhongTroDaNang_TinhGon_Database_v7_BoLoaiPhong_BoDiaChi.sql
   ------------------------------------------------------------
   ============================================================ */

IF DB_ID(N'PhongTroDaNang') IS NULL
BEGIN
    CREATE DATABASE PhongTroDaNang;
END
GO

USE PhongTroDaNang;
GO

/* Xóa trigger cũ nếu database đã từng chạy các bản trước */
IF OBJECT_ID('dbo.TR_DAT_THUE_Check_Phong', 'TR') IS NOT NULL DROP TRIGGER dbo.TR_DAT_THUE_Check_Phong;
IF OBJECT_ID('dbo.TR_BDLP_Check_NhaTro',    'TR') IS NOT NULL DROP TRIGGER dbo.TR_BDLP_Check_NhaTro;
IF OBJECT_ID('dbo.TR_DIA_CHI_Check_DVHC',   'TR') IS NOT NULL DROP TRIGGER dbo.TR_DIA_CHI_Check_DVHC;
GO

/* Xóa bảng theo thứ tự phụ thuộc khóa ngoại.
   Một số bảng chỉ tồn tại ở bản cũ nhưng vẫn DROP để script chạy lại an toàn. */
IF OBJECT_ID('dbo.DAT_THUE',                'U') IS NOT NULL DROP TABLE dbo.DAT_THUE;
IF OBJECT_ID('dbo.BAI_DANG_LOAI_PHONG',     'U') IS NOT NULL DROP TABLE dbo.BAI_DANG_LOAI_PHONG;
IF OBJECT_ID('dbo.BAI_DANG',                'U') IS NOT NULL DROP TABLE dbo.BAI_DANG;
IF OBJECT_ID('dbo.HINH_ANH',                'U') IS NOT NULL DROP TABLE dbo.HINH_ANH;
IF OBJECT_ID('dbo.PHONG_TRO_TIEN_NGHI',     'U') IS NOT NULL DROP TABLE dbo.PHONG_TRO_TIEN_NGHI;
IF OBJECT_ID('dbo.LOAI_PHONG_TIEN_NGHI',    'U') IS NOT NULL DROP TABLE dbo.LOAI_PHONG_TIEN_NGHI;
IF OBJECT_ID('dbo.TIEN_NGHI',               'U') IS NOT NULL DROP TABLE dbo.TIEN_NGHI;
IF OBJECT_ID('dbo.PHONG_TRO',               'U') IS NOT NULL DROP TABLE dbo.PHONG_TRO;
IF OBJECT_ID('dbo.LOAI_PHONG',              'U') IS NOT NULL DROP TABLE dbo.LOAI_PHONG;
IF OBJECT_ID('dbo.NHA_TRO',                 'U') IS NOT NULL DROP TABLE dbo.NHA_TRO;
IF OBJECT_ID('dbo.DIA_CHI',                 'U') IS NOT NULL DROP TABLE dbo.DIA_CHI;
IF OBJECT_ID('dbo.DUONG_PHO',               'U') IS NOT NULL DROP TABLE dbo.DUONG_PHO;
IF OBJECT_ID('dbo.XA',                      'U') IS NOT NULL DROP TABLE dbo.XA;
IF OBJECT_ID('dbo.QUAN_HUYEN',              'U') IS NOT NULL DROP TABLE dbo.QUAN_HUYEN;
IF OBJECT_ID('dbo.QUANHUYEN',               'U') IS NOT NULL DROP TABLE dbo.QUANHUYEN;
IF OBJECT_ID('dbo.DON_VI_HANH_CHINH',       'U') IS NOT NULL DROP TABLE dbo.DON_VI_HANH_CHINH;
IF OBJECT_ID('dbo.NGUOI_THUE',              'U') IS NOT NULL DROP TABLE dbo.NGUOI_THUE;
IF OBJECT_ID('dbo.CHU_NHA_TRO',             'U') IS NOT NULL DROP TABLE dbo.CHU_NHA_TRO;
IF OBJECT_ID('dbo.NGUOI_DUNG_VAI_TRO',      'U') IS NOT NULL DROP TABLE dbo.NGUOI_DUNG_VAI_TRO;
IF OBJECT_ID('dbo.VAI_TRO',                 'U') IS NOT NULL DROP TABLE dbo.VAI_TRO;
IF OBJECT_ID('dbo.NGUOI_DUNG',              'U') IS NOT NULL DROP TABLE dbo.NGUOI_DUNG;
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
    NgayTao         DATETIME2(0)   NOT NULL CONSTRAINT DF_NGUOI_DUNG_NgayTao DEFAULT (SYSDATETIME()),
    NgayCapNhat     DATETIME2(0)   NULL,
    CONSTRAINT UQ_NGUOI_DUNG_Email UNIQUE (Email),
    CONSTRAINT CK_NGUOI_DUNG_TrangThai CHECK (TrangThai IN ('HOAT_DONG','BI_KHOA','CHO_XAC_THUC'))
);
GO

CREATE UNIQUE INDEX UX_NGUOI_DUNG_SoDienThoai_NotNull
ON dbo.NGUOI_DUNG(SoDienThoai)
WHERE SoDienThoai IS NOT NULL;
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
    CONSTRAINT FK_NDVT_VAI_TRO FOREIGN KEY (VaiTroId) REFERENCES dbo.VAI_TRO(Id) ON DELETE CASCADE
);
GO

CREATE TABLE dbo.CHU_NHA_TRO (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    NguoiDungId     INT          NOT NULL,
    TrangThaiHoSo   VARCHAR(30)  NOT NULL CONSTRAINT DF_CHU_NHA_TRO_TrangThaiHoSo DEFAULT ('HOAT_DONG'),
    NgayTao         DATETIME2(0) NOT NULL CONSTRAINT DF_CHU_NHA_TRO_NgayTao DEFAULT (SYSDATETIME()),
    NgayCapNhat     DATETIME2(0) NULL,
    CONSTRAINT UQ_CHU_NHA_TRO_NguoiDungId UNIQUE (NguoiDungId),
    CONSTRAINT FK_CHU_NHA_TRO_NGUOI_DUNG FOREIGN KEY (NguoiDungId) REFERENCES dbo.NGUOI_DUNG(Id),
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
    CONSTRAINT FK_NGUOI_THUE_NGUOI_DUNG FOREIGN KEY (NguoiDungId) REFERENCES dbo.NGUOI_DUNG(Id)
);
GO

/* ============================================================
   2. NHÓM ĐỊA CHỈ ĐÃ RÚT GỌN
   ============================================================ */

CREATE TABLE dbo.DON_VI_HANH_CHINH (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    TenDonVi        NVARCHAR(150) NOT NULL,
    LoaiDonVi       VARCHAR(30)   NOT NULL,
    TinhThanh       NVARCHAR(100) NOT NULL CONSTRAINT DF_DVHC_TinhThanh DEFAULT (N'Đà Nẵng'),
    TrangThai       VARCHAR(30)   NOT NULL CONSTRAINT DF_DVHC_TrangThai DEFAULT ('HIEN_THI'),
    CONSTRAINT CK_DVHC_LoaiDonVi CHECK (LoaiDonVi IN ('PHUONG','XA','DAC_KHU')),
    CONSTRAINT CK_DVHC_TrangThai CHECK (TrangThai IN ('HIEN_THI','AN')),
    CONSTRAINT UQ_DVHC_Ten_Loai_TinhThanh UNIQUE (TenDonVi, LoaiDonVi, TinhThanh)
);
GO

CREATE TABLE dbo.DUONG_PHO (
    Id                  INT IDENTITY(1,1) PRIMARY KEY,
    DonViHanhChinhId    INT           NOT NULL,
    TenDuong            NVARCHAR(150) NOT NULL,
    TrangThai           VARCHAR(30)   NOT NULL CONSTRAINT DF_DUONG_PHO_TrangThai DEFAULT ('HIEN_THI'),
    CONSTRAINT FK_DUONG_PHO_DVHC FOREIGN KEY (DonViHanhChinhId) REFERENCES dbo.DON_VI_HANH_CHINH(Id),
    CONSTRAINT CK_DUONG_PHO_TrangThai CHECK (TrangThai IN ('HIEN_THI','AN')),
    CONSTRAINT UQ_DUONG_PHO_DVHC_TenDuong UNIQUE (DonViHanhChinhId, TenDuong)
);
GO

/* ============================================================
   3. NHÓM NHÀ TRỌ / PHÒNG TRỌ
   ============================================================ */

CREATE TABLE dbo.NHA_TRO (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    ChuNhaTroId     INT              NOT NULL,
    DuongPhoId      INT              NOT NULL,
    SoNha           NVARCHAR(50)     NULL,
    MoTaDiaChi      NVARCHAR(500)    NULL,
    ViDo            DECIMAL(10,7)    NULL,
    KinhDo          DECIMAL(10,7)    NULL,
    TenNhaTro       NVARCHAR(200)    NOT NULL,
    MoTa            NVARCHAR(MAX)    NULL,
    TrangThai       VARCHAR(30)      NOT NULL CONSTRAINT DF_NHA_TRO_TrangThai DEFAULT ('HOAT_DONG'),
    NgayTao         DATETIME2(0)     NOT NULL CONSTRAINT DF_NHA_TRO_NgayTao DEFAULT (SYSDATETIME()),
    NgayCapNhat     DATETIME2(0)     NULL,
    CONSTRAINT FK_NHA_TRO_CHU_NHA_TRO FOREIGN KEY (ChuNhaTroId) REFERENCES dbo.CHU_NHA_TRO(Id),
    CONSTRAINT FK_NHA_TRO_DUONG_PHO FOREIGN KEY (DuongPhoId) REFERENCES dbo.DUONG_PHO(Id),
    CONSTRAINT CK_NHA_TRO_TrangThai CHECK (TrangThai IN ('HOAT_DONG','TAM_AN','NGUNG_HOAT_DONG')),
    CONSTRAINT CK_NHA_TRO_ViDo CHECK (ViDo IS NULL OR (ViDo BETWEEN -90 AND 90)),
    CONSTRAINT CK_NHA_TRO_KinhDo CHECK (KinhDo IS NULL OR (KinhDo BETWEEN -180 AND 180))
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
    Icon            NVARCHAR(100) NULL,
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
    PhongTroId      INT           NOT NULL,
    DuongDanAnh     NVARCHAR(500) NOT NULL,
    LaAnhDaiDien    BIT           NOT NULL CONSTRAINT DF_HINH_ANH_LaAnhDaiDien DEFAULT (0),
    ThuTuHienThi    INT           NOT NULL CONSTRAINT DF_HINH_ANH_ThuTuHienThi DEFAULT (1),
    NgayTao         DATETIME2(0)  NOT NULL CONSTRAINT DF_HINH_ANH_NgayTao DEFAULT (SYSDATETIME()),
    CONSTRAINT FK_HINH_ANH_PHONG_TRO FOREIGN KEY (PhongTroId) REFERENCES dbo.PHONG_TRO(Id) ON DELETE CASCADE,
    CONSTRAINT CK_HINH_ANH_ThuTu CHECK (ThuTuHienThi > 0)
);
GO

CREATE UNIQUE INDEX UX_HINH_ANH_OneCover
ON dbo.HINH_ANH(PhongTroId)
WHERE LaAnhDaiDien = 1;
GO

/* ============================================================
   4. NHÓM BÀI ĐĂNG / KIỂM DUYỆT
   ============================================================ */

CREATE TABLE dbo.BAI_DANG (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    PhongTroId      INT            NOT NULL,
    TieuDe          NVARCHAR(250)  NOT NULL,
    NoiDung         NVARCHAR(MAX)  NOT NULL,
    TrangThaiDuyet  VARCHAR(30)    NOT NULL CONSTRAINT DF_BAI_DANG_TrangThaiDuyet DEFAULT ('NHAP'),
    LyDoTuChoi      NVARCHAR(500)  NULL,
    NgayGuiDuyet    DATETIME2(0)   NULL,
    NgayDuyet       DATETIME2(0)   NULL,
    NgayTao         DATETIME2(0)   NOT NULL CONSTRAINT DF_BAI_DANG_NgayTao DEFAULT (SYSDATETIME()),
    NgayCapNhat     DATETIME2(0)   NULL,
    CONSTRAINT FK_BAI_DANG_PHONG_TRO FOREIGN KEY (PhongTroId) REFERENCES dbo.PHONG_TRO(Id) ON DELETE CASCADE,
    CONSTRAINT CK_BAI_DANG_TrangThaiDuyet CHECK (TrangThaiDuyet IN ('NHAP','CHO_DUYET','DA_DUYET','TU_CHOI','AN'))
);
GO

/* ============================================================
   5. NHÓM YÊU CẦU THUÊ
   ============================================================ */

CREATE TABLE dbo.DAT_THUE (
    Id                  INT IDENTITY(1,1) PRIMARY KEY,
    NguoiThueId         INT            NOT NULL,
    PhongTroId          INT            NOT NULL,
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
    CONSTRAINT FK_DAT_THUE_PHONG_TRO FOREIGN KEY (PhongTroId) REFERENCES dbo.PHONG_TRO(Id),
    CONSTRAINT CK_DAT_THUE_TrangThai CHECK (TrangThai IN ('MOI','CHU_TRO_DONG_Y','CHU_TRO_TU_CHOI','NGUOI_THUE_HUY'))
);
GO

/* ============================================================
   6. INDEX TỐI ƯU TRA CỨU
   ============================================================ */

CREATE INDEX IX_NGUOI_DUNG_TrangThai ON dbo.NGUOI_DUNG(TrangThai);
CREATE INDEX IX_CHU_NHA_TRO_NguoiDungId ON dbo.CHU_NHA_TRO(NguoiDungId);
CREATE INDEX IX_NGUOI_THUE_NguoiDungId ON dbo.NGUOI_THUE(NguoiDungId);

CREATE INDEX IX_DUONG_PHO_DVHC ON dbo.DUONG_PHO(DonViHanhChinhId);
CREATE INDEX IX_NHA_TRO_DuongPhoId ON dbo.NHA_TRO(DuongPhoId);
CREATE INDEX IX_NHA_TRO_ChuNhaTroId ON dbo.NHA_TRO(ChuNhaTroId);
CREATE INDEX IX_NHA_TRO_TrangThai ON dbo.NHA_TRO(TrangThai);

CREATE INDEX IX_PHONG_TRO_NhaTroId ON dbo.PHONG_TRO(NhaTroId);
CREATE INDEX IX_PHONG_TRO_TrangThai ON dbo.PHONG_TRO(TrangThai);
CREATE INDEX IX_PHONG_TRO_Search ON dbo.PHONG_TRO(TrangThai, GiaThueThang, DienTich);
CREATE INDEX IX_HINH_ANH_PhongTroId ON dbo.HINH_ANH(PhongTroId);

CREATE INDEX IX_BAI_DANG_TrangThaiDuyet ON dbo.BAI_DANG(TrangThaiDuyet);
CREATE INDEX IX_BAI_DANG_PhongTroId ON dbo.BAI_DANG(PhongTroId);

CREATE INDEX IX_DAT_THUE_NguoiThueId ON dbo.DAT_THUE(NguoiThueId);
CREATE INDEX IX_DAT_THUE_PhongTroId ON dbo.DAT_THUE(PhongTroId);
CREATE INDEX IX_DAT_THUE_TrangThai ON dbo.DAT_THUE(TrangThai);
GO

/* ============================================================
   7. SEED DATA CƠ BẢN
   ============================================================ */

INSERT INTO dbo.VAI_TRO (TenVaiTro, MoTa)
VALUES
('ADMIN',      N'Quản trị viên hệ thống'),
('CHU_TRO',    N'Chủ nhà trọ / chủ phòng trọ'),
('NGUOI_THUE', N'Người thuê / người tìm phòng');
GO

INSERT INTO dbo.TIEN_NGHI (TenTienNghi, Icon, TrangThai)
VALUES
(N'Wifi',           'wifi',            'HIEN_THI'),
(N'Máy lạnh',       'air-conditioner', 'HIEN_THI'),
(N'Chỗ để xe',      'parking',         'HIEN_THI'),
(N'Máy giặt',       'washing-machine', 'HIEN_THI'),
(N'WC riêng',       'toilet',          'HIEN_THI'),
(N'Gác lửng',       'loft',            'HIEN_THI'),
(N'Camera an ninh', 'camera',          'HIEN_THI'),
(N'Giờ giấc tự do', 'clock',           'HIEN_THI');
GO

INSERT INTO dbo.DON_VI_HANH_CHINH (TenDonVi, LoaiDonVi, TinhThanh, TrangThai)
VALUES
(N'Phường Hải Châu',     'PHUONG', N'Đà Nẵng', 'HIEN_THI'),
(N'Phường Thanh Khê',    'PHUONG', N'Đà Nẵng', 'HIEN_THI'),
(N'Phường Sơn Trà',      'PHUONG', N'Đà Nẵng', 'HIEN_THI'),
(N'Phường Liên Chiểu',   'PHUONG', N'Đà Nẵng', 'HIEN_THI'),
(N'Phường Ngũ Hành Sơn', 'PHUONG', N'Đà Nẵng', 'HIEN_THI'),
(N'Xã Hòa Vang',         'XA',     N'Đà Nẵng', 'HIEN_THI');
GO

INSERT INTO dbo.DUONG_PHO (DonViHanhChinhId, TenDuong, TrangThai)
SELECT Id, N'Nguyễn Văn Linh', 'HIEN_THI'
FROM dbo.DON_VI_HANH_CHINH
WHERE TenDonVi = N'Phường Hải Châu';

INSERT INTO dbo.DUONG_PHO (DonViHanhChinhId, TenDuong, TrangThai)
SELECT Id, N'Điện Biên Phủ', 'HIEN_THI'
FROM dbo.DON_VI_HANH_CHINH
WHERE TenDonVi = N'Phường Thanh Khê';

INSERT INTO dbo.DUONG_PHO (DonViHanhChinhId, TenDuong, TrangThai)
SELECT Id, N'Tôn Đức Thắng', 'HIEN_THI'
FROM dbo.DON_VI_HANH_CHINH
WHERE TenDonVi = N'Phường Liên Chiểu';

/* Dùng khi nhà trọ chưa có tên đường cụ thể nhưng vẫn cần chọn phường/xã */
INSERT INTO dbo.DUONG_PHO (DonViHanhChinhId, TenDuong, TrangThai)
SELECT Id, N'Khác / Chưa xác định', 'HIEN_THI'
FROM dbo.DON_VI_HANH_CHINH;
GO

INSERT INTO dbo.NGUOI_DUNG (HoTen, Email, SoDienThoai, MatKhauHash, TrangThai)
VALUES (N'Admin System', N'admin@phongtrodanang.vn', N'0901234567', N'PLACEHOLDER_PASSWORD_HASH', 'HOAT_DONG');
GO

INSERT INTO dbo.NGUOI_DUNG_VAI_TRO (NguoiDungId, VaiTroId)
SELECT nd.Id, vt.Id
FROM dbo.NGUOI_DUNG nd
JOIN dbo.VAI_TRO vt ON vt.TenVaiTro = 'ADMIN'
WHERE nd.Email = N'admin@phongtrodanang.vn';
GO

/* ============================================================
   ============================================================ */
