using DevExpress.XtraGrid.Views.Grid;
using NtbSoft.ERP.Libs;
using NtbSoft.ERP.Win.Service.SYSTEM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.KeHoach
{
    public partial class frmChonCayVaiCap : Form
    {
        GetDataService _service = new GetDataService();
        System.Configuration.AppSettingsReader settingsReader =
                                           new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private string createUrlPhieuYeuCau(string action, string maphieu, string malenhSX)
        {
            return string.Format(@"{0}PhieuYeuCau/GetData?action={1}&&maphieu={2}&&malenhsx={3}", URL, action, maphieu, malenhSX);
        }
        private string createUrlPhieuCap(string action, string maphieu, string malenhSX)
        {
            return string.Format(@"{0}PhieuCap/GetData?action={1}&&maphieu={2}&&malenhsx={3}", URL, action, maphieu, malenhSX);
        }
        private DataTable GetDataTable(string urlStr)
        {
            return Task.Run(
                async () => { return await _service.GetDataTable(urlStr); }
                ).Result;
        }
        private string PostDataTable(string urlStr, DataTable dataPost)
        {
            return Task.Run(
                async () => { return await _service.PostData(urlStr, dataPost); }
                ).Result;
        }
        private DataTable GETDataTableDB(string urlStr, DataTable dataPost)
        {
            return Task.Run(
                async () => { return await _service.GETBYPOST(urlStr, dataPost); }
                ).Result;
        }
        public DataTable result;
        private string _maPhieuYeuCau = string.Empty;
        private string _maVTMau = string.Empty;
        private DataTable dbCayVai;
        public frmChonCayVaiCap(string maPhieuYeuCau, string maVTMau)
        {
            InitializeComponent();
            URL = settingsReader.GetValue("URL", typeof(string)).ToString();
            _maPhieuYeuCau = maPhieuYeuCau;
            _maVTMau = maVTMau;
            loadForm();
        }
        private void loadForm()
        {
            LoadDV();
            LoadCayVai();
            setTextInfo();
        }
        private void LoadDV()
        {
            try
            {
                DataTable donvi = new DataTable();
                donvi.Columns.Add("STT", typeof(int));
                donvi.Columns.Add("MaDV", typeof(string));
                donvi.Columns.Add("TenDV", typeof(string));
                donvi.Columns.Add("KiHieu", typeof(string));
                donvi.Columns.Add("DisplayMember", typeof(string));

                string urlGetQC = URL + "LenghtUnitConvert/get";
                DataTable heQuyChieu = GetDataTable(urlGetQC);
                List<LengthUnit> lstLenght = new List<LengthUnit>();
                foreach (DataRow row in heQuyChieu.Rows)
                    lstLenght.Add(new LengthUnit(row["maDV"].ToString(), row["tenDV"].ToString(), row["kiHieu"].ToString(), Convert.ToDouble(row["heQuyChieu_m"].ToString())));
                LengthUnitConverter.SetStatusConnectSQL(true, lstLenght);

                List<LengthUnit> lstLenghtUnit = LengthUnitConverter.lstLengthUnit;
                foreach (LengthUnit lengthUnit in lstLenghtUnit)
                {
                    DataRow newRow = donvi.NewRow();
                    newRow["STT"] = 0;
                    newRow["MaDV"] = lengthUnit.maDV;
                    newRow["TenDV"] = lengthUnit.tenDV;
                    newRow["KiHieu"] = lengthUnit.kiHieu;
                    newRow["DisplayMember"] = string.Format(@"{0}({1})", lengthUnit.tenDV, lengthUnit.kiHieu);
                    donvi.Rows.Add(newRow);
                }
                slud_DonVi.Properties.DataSource = donvi;
                slud_DonVi.Properties.DisplayMember = "DisplayMember";
                slud_DonVi.Properties.ValueMember = "MaDV";

                if (donvi.Rows.Count > 0)
                {
                    slud_DonVi.EditValue = lstLenghtUnit[0].maDV;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Cài đặt hệ quy chiếu thất bại!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCayVai()
        {
            try
            {
                string urlStr = URL + "PhieuCap/GetDataDB";
                DataTable res = new DataTable();
                res.Columns.Add("action");
                res.Columns.Add("maphieu");
                res.Columns.Add("malenhsx");
                res.Rows.Add("GET_CayVai", _maVTMau, _maPhieuYeuCau);
                DataTable dbResult = GETDataTableDB(urlStr, res);
                gridControl1.DataSource = dbResult;

                if (!(dbResult != null && dbResult.Rows.Count > 0))
                    return;
                DataRow row0 = dbResult.Rows[0];
                txtYC_per_KH.Text = string.Format(@"{0}/{1}", row0["SL_YeuCau"].ToString(), row0["KeHoachCap"].ToString());
                txtDC_per_YC.Text = string.Format(@"{0}/{1}", row0["DaCap"].ToString(), row0["SL_YeuCau"].ToString());
                txtConLaiPhaiCap.Text = (Convert.ToDouble(row0["SL_YeuCau"].ToString()) - Convert.ToDouble(row0["DaCap"].ToString())).ToString();

                dbCayVai = new DataTable();
                dbCayVai.Columns.Add("Barcode", typeof(string));
                dbCayVai.Columns.Add("ChieuDai", typeof(float));

                foreach (DataRow row in dbResult.Rows)
                {
                    DataRow newR = dbCayVai.NewRow();
                    newR["Barcode"] = row["Barcode"].ToString();
                    newR["ChieuDai"] = (float)Convert.ToDouble(row["SoLuong"].ToString());
                    dbCayVai.Rows.Add(newR);
                }
            }
            catch (Exception ex)
            {
                return;
            }

        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            result = new DataTable();
            this.Close();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {

            DataTable db = gridControl1.DataSource as DataTable;
            result = db.Clone();
            for (int i = 0; i < db.Rows.Count; i++)
            {
                if (Convert.ToBoolean(db.Rows[i]["Chon"]))
                    result.ImportRow(db.Rows[i]);
            }
            if (result.Rows.Count == 0)
            {
                MessageBox.Show("Chưa chọn cây vải!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                this.Close();
            }
            this.Close();
        }

        private void searchControl1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
        }
        private DataTable GoiY_dataTable(DataTable lstDataT)
        {
            DataTable result = new DataTable();

            return result;
        }
        private DataTable createDBGoiYPhuongAnCap()
        {
            DataTable result = new DataTable();
            result.Columns.Add("STT", typeof(int));
            result.Columns.Add("PhuongAn", typeof(string));
            result.Columns.Add("SoCayVai", typeof(int));
            result.Columns.Add("TongChieuDai", typeof(float));
            result.Columns.Add("HaoPhi", typeof(string));
            return result;
        }
        private DataTable CreateDBPhuongAnCap(DataTable cayVaiCap, float SoLuong)
        {
            DataTable result = createDBGoiYPhuongAnCap();
            // Danh sách để lưu các cây vải ghép lại
            float totalLength = 0;
            List<string> currentGroupCayVai = new List<string>();
            int stt = 1;  // Số thứ tự phương án
            // Duyệt qua từng dòng cây vải
            foreach (DataRow row in cayVaiCap.Rows)
            {
                string maCayVai = row["Barcode"].ToString();
                float length = Convert.ToSingle(row["ChieuDai"]);
                // Nếu tổng chiều dài cộng với cây vải hiện tại vượt quá soluong, kết thúc nhóm hiện tại
                if (totalLength + length > SoLuong)
                {
                    currentGroupCayVai.Add(maCayVai);
                    totalLength += length;
                    // Nếu nhóm có cây vải, thêm phương án vào DataTable
                    if (currentGroupCayVai.Count > 0)
                    {
                        string phuongAn = string.Join("@", currentGroupCayVai); // Danh sách mã cây vải
                        result.Rows.Add(stt, phuongAn, currentGroupCayVai.Count, totalLength, ((totalLength - SoLuong)).ToString());
                        // Tăng số thứ tự
                        stt++;
                    }
                    // Bắt đầu nhóm mới
                    currentGroupCayVai.Clear();
                    currentGroupCayVai.Add(maCayVai);
                    totalLength = length;
                }
                else
                {
                    // Thêm cây vải vào nhóm hiện tại
                    currentGroupCayVai.Add(maCayVai);
                    totalLength += length;
                }
            }
            // Thêm nhóm cuối cùng vào kết quả nếu có
            if (currentGroupCayVai.Count > 0)
            {
                string phuongAn = string.Join("@", currentGroupCayVai);
                result.Rows.Add(stt, phuongAn, currentGroupCayVai.Count, totalLength, (totalLength - SoLuong).ToString());
            }
            return result;
        }
        private void btnUserGoiY_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dbSource = gridControl1.DataSource as DataTable;
                bool isTrue = false;
                for (int i = 0; i < dbSource.Rows.Count; i++)
                {
                    if ((bool)dbSource.Rows[i]["Chon"])
                    {
                        isTrue = true;
                        break;
                    }
                }
                if (isTrue)
                {
                    DialogResult dr = MessageBox.Show("Đang có dữ liệu được chọn, sử dụng gợi ý sẽ bỏ những dữ liệu trước đó, xác nhận?", "Xác nhận",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (dr == DialogResult.OK)
                    {
                        UseGoiY();
                    }
                }
                else
                {
                    UseGoiY();
                }
                setTextInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thông báo ngoại lệ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }
        private void UseGoiY()
        {
            try
            {
                DataTable dbSource = gridControl1.DataSource as DataTable;
                DataTable dbPhuongAn = CreateDBPhuongAnCap(dbCayVai, (float)Convert.ToDouble(txtConLaiPhaiCap.Text));
                float min = (float)Convert.ToDouble(dbPhuongAn.Rows[0]["HaoPhi"].ToString());
                if (dbPhuongAn.Rows.Count == 1 && min < 0)
                {
                    DialogResult dr2 = MessageBox.Show("Số lượng trong kho không đủ để tạo gợi ý, xuất tất cả vật tư hiện có cho lần cấp này?", "Xác nhận",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (dr2 == DialogResult.OK)
                    {
                        for (int i = 0; i < dbSource.Rows.Count; i++)
                        {
                            dbSource.Rows[i]["Chon"] = false;
                            dbSource.Rows[i]["SoLuongCap"] = 0;
                        }
                        for (int i = 0; i < dbSource.Rows.Count; i++)
                        {
                            dbSource.Rows[i]["Chon"] = true;
                            dbSource.Rows[i]["SoLuongCap"] = dbSource.Rows[i]["SoLuong"];
                        }
                        gridControl1.DataSource = dbSource;
                        gridControl1.RefreshDataSource();
                        return;
                    }
                }
                DataRow rowResult = dbPhuongAn.Rows[0];
                for (int i = 0; i < dbPhuongAn.Rows.Count; i++)
                {
                    float hpIndex = (float)Convert.ToDouble(dbPhuongAn.Rows[i]["HaoPhi"].ToString());
                    if (hpIndex < min && hpIndex >= 0)
                    {
                        rowResult = dbPhuongAn.Rows[i];
                        min = hpIndex;
                    }
                }

                List<string> lstGoiY = rowResult["PhuongAn"].ToString().Split('@').ToList();
                for (int i = 0; i < dbSource.Rows.Count; i++)
                {
                    if (lstGoiY.Contains(dbSource.Rows[i]["Barcode"].ToString()))
                    {
                        dbSource.Rows[i]["Chon"] = true;
                        dbSource.Rows[i]["SoLuongCap"] = dbSource.Rows[i]["SoLuong"];
                    }
                }
                gridControl1.DataSource = dbSource;
                gridControl1.RefreshDataSource();
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thông báo ngoại lệ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        private void gridView1_DataSourceChanged(object sender, EventArgs e)
        {
            setTextInfo();
        }


        private void gridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "Chon")
                {
                    GridView view = sender as GridView;
                    bool val = Convert.ToBoolean(e.Value);
                    view.SetRowCellValue(e.RowHandle, "Chon", val);
                    if (val)
                        view.SetRowCellValue(e.RowHandle, "SoLuongCap", view.GetRowCellValue(e.RowHandle, "SoLuong"));
                    else
                        view.SetRowCellValue(e.RowHandle, "SoLuongCap", 0);
                }
                setTextInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thông báo ngoại lệ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }
        private void setTextInfo()
        {
            if (string.IsNullOrEmpty(txtConLaiPhaiCap.Text))
                return;
            DataTable dbSource = gridControl1.DataSource as DataTable;
            if (dbSource == null || dbSource.Rows.Count == 0)
            {
                txtLbEmtyDanhGia.Text = string.Format(@"<color=red>Dữ liệu vật tư trống!</color>");
                return;
            }

            float tong = 0;
            int dem = 0;
            foreach (DataRow row in dbSource.Rows)
            {
                if ((bool)row["Chon"])
                {
                    dem++;
                    tong += (float)Convert.ToDouble(row["SoLuongCap"].ToString());
                }
            }
            float haophi = tong - (float)Convert.ToDouble(txtConLaiPhaiCap.Text);
            if (haophi > 0)
                txtLbEmtyDanhGia.Text = string.Format(@"Sử dụng <color=blue>{0}</color> kiện để <color=green>cấp {1}{2} vật tư</color>  và phát sinh <color=red>{3}{2} vật tư hao phí</color>!", dem, tong, slud_DonVi.Text,
                   haophi);
            else if (haophi == 0)
                txtLbEmtyDanhGia.Text = string.Format(@"Sử dụng <color=blue>{0}</color> kiện để <color=green>cấp {1}{2} vật tư</color>  và không phát sinh vật tư hao phí!", dem, tong, slud_DonVi.Text,
              haophi);
            else
                txtLbEmtyDanhGia.Text = string.Format(@"Sử dụng <color=blue>{0}</color> kiện để <color=green>cấp {1}{2} vật tư</color>  và thiếu <color=red>{3}{2} vật tư</color>!", dem, tong, slud_DonVi.Text, haophi * (-1));
            float phantram = tong / (float)Convert.ToDouble(txtConLaiPhaiCap.Text);
            phantram = phantram < 1 ? phantram > 0 ? phantram : 0 : 1;
            textEdit1.Text = (phantram * 100).ToString("0.##");
        }

        private void gridView1_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            e.Appearance.BackColor = System.Drawing.Color.LightBlue;

            e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Bold);

            e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            e.Appearance.ForeColor = System.Drawing.Color.ForestGreen;

            e.Handled = false;
        }

        private void gridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            int rowHandle = gridView1.FocusedRowHandle;
            if (rowHandle < 0)
                return;

            if (gridView1.FocusedColumn.FieldName == "SoLuongCap")
            {
                string inputValue = e.Value?.ToString();

                // Kiểm tra nếu giá trị nhập vào không phải là null hoặc rỗng
                if (string.IsNullOrWhiteSpace(inputValue))
                {
                    e.Valid = false;  // Không hợp lệ
                    e.ErrorText = "Số lượng cấp không thể để trống.";  // Thông báo lỗi
                    return;
                }

                if (float.TryParse(inputValue, out float floatValue))
                {
                    // Kiểm tra nếu giá trị là số thực và lớn hơn 0
                    if (floatValue <= 0)
                    {
                        e.Valid = false;
                        e.ErrorText = "Số lượng cấp phải lớn hơn 0.";
                        return;
                    }
                    float tonkho = (float)Convert.ToDouble(gridView1.GetRowCellValue(rowHandle, "SoLuong").ToString());
                    if (floatValue > tonkho)
                    {
                        e.Valid = false;
                        e.ErrorText = string.Format(@"Số lượng cấp không thể vượt quá số tồn kho({0})", tonkho);
                        return;
                    }
                }
                else
                {
                    // Nếu không phải số hợp lệ (cả int và float)
                    e.Valid = false;
                    e.ErrorText = "Số lượng cấp phải là một số hợp lệ.";
                    return;
                }
            }
        }
    }
}
