using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using NtbSoft.ERP.Entity.Kho;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using DevExpress.XtraPrinting.Drawing;

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmPrintReviewTheBai : DevExpress.XtraReports.UI.XtraReport
    {
        DataTable _dt;
        List<ExcelPicture> _listPC;
        List<TheBaiVatTuNhapKhoEntity> _lstEntity;
        private double height;
        private double widthCol1;
        private double widthCol2;
        private double widthCol3;
        private double widthCol4;
        private int imageIndex = 0;

        public frmPrintReviewTheBai(DataTable dt, List<ExcelPicture> listPC)
        {
            InitializeComponent();
            SetHeightWidth();
            _dt = dt;
            _listPC = listPC;
            _lstEntity = new List<TheBaiVatTuNhapKhoEntity>();
            try
            {
                int rowIndex = 1;
                TheBaiVatTuNhapKhoEntity _thebai = new TheBaiVatTuNhapKhoEntity();
                foreach (DataRow dataRow in _dt.Rows)
                {

                    int chiSoDong = XacDinhDong(rowIndex);
                    for (int i = 0; i < _dt.Columns.Count; i++)
                    {
                        DataColumn column = _dt.Columns[i];
                        XRTableCell cell = new XRTableCell();
                        int chiSoCot = XacDinhCot(column.ColumnName);
                        string a = dataRow[column.ColumnName].ToString();
                        lbBarcode.Text = a;
                        if (rowIndex == 2)
                        {
                            _thebai.MaVT = dataRow[0].ToString();
                            _thebai.NgayNhap = dataRow[3].ToString();
                        }
                        if (rowIndex == 3)
                        {
                            _thebai.TenVT = dataRow[0].ToString();

                        }
                        if (rowIndex == 4)
                        {

                            _thebai.Kien = dataRow[0].ToString();
                            _thebai.Lot = dataRow[1].ToString();
                            _thebai.Ghichu = dataRow[2].ToString();
                        }
                        if (rowIndex == 5)
                        {

                            _thebai.SoLuong = dataRow[0].ToString();

                            _thebai.DVTinh = dataRow[2].ToString();
                        }
                        if (rowIndex == 6)
                        {

                            _thebai.Barcode = dataRow[0].ToString();

                        }



                    }
                    //table.Rows.Add(row);
                    rowIndex++;


                }
                _lstEntity.Add(_thebai);
                lbMaVT.Text = _thebai.MaVT;
                lbTenVT.Text = _thebai.TenVT;
                lbNgayNhap.Text = _thebai.NgayNhap;
                lbKien.Text = _thebai.Kien;
                lbLot.Text = _thebai.Lot;
                lbGhiChu.Text = _thebai.Ghichu;
                lbSoLuong.Text = _thebai.SoLuong;
                lbDVTinh.Text = _thebai.DVTinh;
                lbBarcode.Text = _thebai.Barcode;
                Image image = ConvertByteArrayToImage(_listPC[imageIndex].Image);

                pictureBoxQR.ImageSource = new DevExpress.XtraPrinting.Drawing.ImageSource(image);
                pictureBoxQR.Sizing = DevExpress.XtraPrinting.ImageSizeMode.StretchImage;


            }
            catch (Exception ex)
            {
            }
        }
        private float GetTextWidth(string text, int fontSize)
        {
            Font font = new System.Drawing.Font("Times New Roman", fontSize, FontStyle.Bold);
            float result = 0;
            using (Bitmap bitmap = new Bitmap(1, 1))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                SizeF size = graphics.MeasureString(text, font);
                Console.WriteLine($"Width: {size.Width}px, Height: {size.Height}px");
                result = size.Width;
            }
            return result;
        }
        private float GetTextHeight(string text, int fontSize)
        {
            Font font = new System.Drawing.Font("Times New Roman", fontSize, FontStyle.Bold);
            float result = 0;
            using (Bitmap bitmap = new Bitmap(1, 1))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                SizeF size = graphics.MeasureString(text, font);
                Console.WriteLine($"Width: {size.Width}px, Height: {size.Height}px");
                result = size.Height;
            }
            return result;
        }
        private void SetHeightWidth()
        {
            height = 807.0 / (10 * 6);
            widthCol1 = 0.2 * (100.0 / 2);
            widthCol2 = 0.22 * (100.0 / 2);
            widthCol3 = 0.225 * (100.0 / 2);
            widthCol4 = 0.355 * (100.0 / 2);
        }
        public static Image ConvertByteArrayToImage(Image excelImage)
        {
            byte[] imageBytes = ConvertImageToByteArray(excelImage);
            if (imageBytes == null || imageBytes.Length == 0)
            {
                throw new ArgumentException("Mảng byte không hợp lệ.", nameof(imageBytes));
            }

            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                return Image.FromStream(ms);
            }
        }
        public static byte[] ConvertImageToByteArray(Image image)
        {
            // Đọc hình ảnh từ file

            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Chuyển đổi hình ảnh thành byte[]
                image.Save(memoryStream, image.RawFormat);
                return memoryStream.ToArray();
            }

        }
        private int XacDinhDong(int RowIndex)
        {
            return RowIndex % 6 == 0 ? 6 : RowIndex % 6;
        }

        private int XacDinhCot(string colunmName)
        {
            int chiSoCot = Convert.ToInt32(colunmName.Split('_')[1].ToString());
            if (chiSoCot > 12)
            {
                chiSoCot -= 12;
            }
            else if (chiSoCot > 8)
            {
                chiSoCot -= 8;
            }
            else if (chiSoCot > 4)
            {
                chiSoCot -= 4;
            }
            return chiSoCot;
        }



    }
}
