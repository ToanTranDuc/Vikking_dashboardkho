using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Win.Properties;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmPhapDanhCty : DevExpress.XtraEditors.XtraForm
    {

        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;

        List<PhapDanhCtyEntity> lstPhapDanhCty;
        List<int> lstRowUpdate = new List<int>();

        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;

        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        bool indicatorIcon = true; bool IsVal = false;
        int FocusedIndex = 0;
        public static string TenImage;
        KeyDownControlHandler keyDownControlHandler;

        ActionControl actionControlAdd;
        ActionControl actionControlEdit;
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;
        public frmPhapDanhCty()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            lstPhapDanhCty = new List<PhapDanhCtyEntity>();
        }

        protected override void OnLoad(EventArgs e)
        {

            CheckPerminsion();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            LoadPhapDanhCty();
        }

        private List<ActionControl> InitActionKeyDown()
        {
            actionControlAdd = new ActionControl(ThemDong, _allowAdd, ActionType.Add, this.Them.Enabled);
            actionControlEdit = new ActionControl(SuaDong, _allowEdit, ActionType.Edit, this.Sua.Enabled);
            actionControlDelete = new ActionControl(XoaDong, _allowDelete, ActionType.Delete, this.Xoa.Enabled);
            actionControlSave = new ActionControl(LuuDong, (_allowAdd || _allowEdit), ActionType.Save, this.Luu.Enabled);
            actionControlRefresh = new ActionControl(NapLaiDong, true, ActionType.Refresh, this.Naplai.Enabled);

            lstActionControls = new List<ActionControl> {
                actionControlAdd, actionControlEdit,
                actionControlDelete, actionControlSave,actionControlRefresh };
            return lstActionControls;
        }
       
        private void LoadPhapDanhCty()
        {
            try
            {
                _status = ResourceURL.EventStatus.View;
                GridViewUpdateStatus(_status);
                string url = string.Format("{0}?", URL + "PhapDanhCty/Get");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    lstPhapDanhCty = JsonConvert.DeserializeObject<List<PhapDanhCtyEntity>>(json);
                }
                grcPhapDanhCty.DataSource = lstPhapDanhCty;
                if (lstPhapDanhCty != null && lstPhapDanhCty.Count != 0)
                    InsertImage(lstPhapDanhCty[0].ImageLoGo);

            }
            catch
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }


        private void CheckPerminsion()
        {
            string url = string.Format("{0}/GetPer?userID={1}", URL + ResourceURL.UrlUserModule, GlobleData.UserName);
            List<SystemUserModuleEntity> List = Task.Run(async () => { return await _serviceUserModule.UserModuleGet(url); }).Result;
            SystemUserModuleEntity obj = (from m in List
                                          where m.FormShow == this.Name
                                          select m).FirstOrDefault();
            if (obj == null) return;
            _allowAdd = obj.AllowAdd;
            _allowEdit = obj.AllowEdit;
            _allowDelete = obj.AllowDelete;
            if (!_allowAdd)
            {
                Them.Enabled = false;
            }
            if (!_allowEdit)
            {
                Sua.Enabled = false;
            }
            if (_allowAdd || _allowEdit)
            {
                Luu.Enabled = true;
            }
            else
            {
                Luu.Enabled = false;
            }

            if (!_allowDelete)
                Xoa.Enabled = false;

        }

        private void GridViewUpdateStatus(ResourceURL.EventStatus status)
        {
            switch (status)
            {
                case ResourceURL.EventStatus.View:
                    bgrvPhapDanhCty.OptionsBehavior.Editable = false;

                    if (_allowAdd)
                    {
                        Them.Enabled = true;
                        actionControlAdd.Enabled = true;
                    }

                    if (_allowEdit)
                    {
                        Sua.Enabled = true;
                        actionControlEdit.Enabled = true;
                    }

                    if (_allowDelete)
                    {
                        Xoa.Enabled = true;
                        actionControlDelete.Enabled = true;
                    }

                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = false;
                        actionControlSave.Enabled = false;
                    }

                    break;
                case ResourceURL.EventStatus.Edit:
                    bgrvPhapDanhCty.OptionsBehavior.Editable = true;
                    if (_allowAdd)
                    {
                        Them.Enabled = false;
                        actionControlAdd.Enabled = false;
                    }
                    if (_allowEdit)
                    {
                        Sua.Enabled = false;
                        actionControlEdit.Enabled = false;
                    }

                    if (_allowDelete)
                    {
                        Xoa.Enabled = false;
                        actionControlDelete.Enabled = false;
                    }

                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = true;
                        actionControlSave.Enabled = true;
                    }
                    break;
                case ResourceURL.EventStatus.Add:
                    bgrvPhapDanhCty.OptionsBehavior.Editable = true;
                    if (_allowAdd)
                    {
                        Them.Enabled = false;
                        actionControlAdd.Enabled = false;
                    }

                    if (_allowEdit)
                    {
                        Sua.Enabled = false;
                        actionControlEdit.Enabled = false;
                    }
                    if (_allowDelete)
                    {
                        Xoa.Enabled = false;
                        actionControlDelete.Enabled = false;
                    }
                    Sua.Enabled = false;
                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = true;
                        actionControlSave.Enabled = true;
                    }

                    break;
            }
        }

        private void ThemDong()
        {

            PhapDanhCtyEntity obj = new PhapDanhCtyEntity();
            lstPhapDanhCty.Add(obj);
            _rowAdd = bgrvPhapDanhCty.RowCount - 1;
            _status = ResourceURL.EventStatus.Add;
            GridViewUpdateStatus(_status);
            bgrvPhapDanhCty.FocusedRowHandle = _rowAdd;
            grcPhapDanhCty.RefreshDataSource();

        }

        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ThemDong();
        }

        private void SuaDong()
        {
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);

        }

        private void Sua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SuaDong();
        }

        private async void XoaDong()
        {
            try
            {
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    PhapDanhCtyEntity row = bgrvPhapDanhCty.GetRow(bgrvPhapDanhCty.FocusedRowHandle) as PhapDanhCtyEntity;
                    string url = string.Format("{0}?Parameter={1}", URL + "PhapDanhCty/Delete", row.ID);
                    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    if (result.ToLower() == "true")
                        LoadPhapDanhCty();
                    else XtraMessageBox.Show(result);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Xóa đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XoaDong();
        }

        private void LuuDong()
        {
            try
            {
                this.ActiveControl = this.button1;

                if ((_status != ResourceURL.EventStatus.View) && bgrvPhapDanhCty.FocusedRowHandle >= 0)
                {
                    FocusedIndex = bgrvPhapDanhCty.FocusedRowHandle;
                }
                List<PhapDanhCtyEntity> _lstUpdate = new List<PhapDanhCtyEntity>();

                PhapDanhCtyEntity row = bgrvPhapDanhCty.GetRow(bgrvPhapDanhCty.FocusedRowHandle) as PhapDanhCtyEntity;
                if (row == null)
                {
                    XtraMessageBox.Show(Resources.UpdateDataBefore, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    return;
                }

                lstRowUpdate = lstRowUpdate.Distinct().OrderBy(x => x).ToList();
                for (int i = 0; i < lstRowUpdate.Count; i++)
                {
                    PhapDanhCtyEntity item = bgrvPhapDanhCty.GetRow(lstRowUpdate[i]) as PhapDanhCtyEntity;

                    if (item != null && !string.IsNullOrEmpty(item.MaCty) && !string.IsNullOrEmpty(item.TenCty))
                    {

                        _lstUpdate.Add(item);

                    }
                    else
                    {
                        XtraMessageBox.Show(Resources.UpdateDataBefore, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                        return;
                    }
                }
                string msResult = "";
                if (_lstUpdate == null || (_lstUpdate.Count == 0))
                {
                    return;
                }


                string url = string.Format("{0}?", URL + "PhapDanhCty/Post");
                msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _lstUpdate); }).Result;

                //
                if (msResult.ToLower() == "true")
                {
                    LoadPhapDanhCty();
                    clsWaitForm.ShowSuccessForm(this, 2000);
                }
                else XtraMessageBox.Show(msResult);

                lstRowUpdate.Clear();

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lưu đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LuuDong();
        }

        private void bgrvPhapDanhCty_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            lstRowUpdate.Add(e.RowHandle);
        }

        private void NapLaiDong()
        {
            LoadPhapDanhCty();

            
        }

        private void bgrvPhapDanhCty_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            focused(sender);
            
            // Lấy DataRow từ FocusedRowHandle
           var focusedRow = bgrvPhapDanhCty.GetFocusedRow();
            if (focusedRow == null) return;
            string image = bgrvPhapDanhCty.GetFocusedRowCellValue("ImageLoGo") == null ? "" : bgrvPhapDanhCty.GetFocusedRowCellValue("ImageLoGo").ToString();
            InsertImage(image);
        }

        private void bgrvPhapDanhCty_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            focused(sender);
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLaiDong();
        }



        private void focused(object sender)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (_status.Equals(ResourceURL.EventStatus.Add))
            {
                if (_rowAdd != -1 && view.FocusedRowHandle == _rowAdd)
                {
                    view.FocusedColumn.OptionsColumn.AllowEdit = true;
                }

                else
                    view.FocusedColumn.OptionsColumn.AllowEdit = false;
            }
            //else
            //{
            //    if (_status.Equals(ResourceURL.EventStatus.Edit))
            //    {
            //        if (view.FocusedColumn == colMaCty)
            //            view.FocusedColumn.OptionsColumn.AllowEdit = false;
            //        else
            //            view.FocusedColumn.OptionsColumn.AllowEdit = true;
            //    }
            //}
        }

        private void gridView_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            try
            {
                GridView view = (GridView)sender;
                if (e.Info.IsRowIndicator && e.RowHandle >= 0)
                {
                    string sText = (e.RowHandle + 1).ToString();
                    Graphics gr = e.Info.Graphics;
                    gr.PageUnit = GraphicsUnit.Pixel;
                    GridView gridView = ((GridView)sender);
                    SizeF size = gr.MeasureString(sText, e.Info.Appearance.Font);
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width + 15;
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize;
                    }

                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Info.DisplayText = sText;
                }
                if (!indicatorIcon)
                    e.Info.ImageIndex = -1;

                if (e.RowHandle == GridControl.InvalidRowHandle)
                {
                    Graphics gr = e.Info.Graphics;
                    gr.PageUnit = GraphicsUnit.Pixel;
                    GridView gridView = ((GridView)sender);
                    SizeF size = gr.MeasureString("STT", e.Info.Appearance.Font);
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width + 15;
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize;
                    }

                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Info.DisplayText = "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void bgrvPhapDanhCty_InvalidValueException(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            ColumnView view = sender as ColumnView;
            if (view == null) return;
            e.ExceptionMode = ExceptionMode.DisplayError;
            e.WindowCaption = "Cảnh Báo";
            view.HideEditor();
        }

        private void bgrvPhapDanhCty_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
        {
            if (this.bgrvPhapDanhCty.FocusedColumn != null)
            {
                e.Valid = true;
                if (this.bgrvPhapDanhCty.FocusedColumn == colMaCty)
                {
                    List<PhapDanhCtyEntity> lst = this.grcPhapDanhCty.DataSource as List<PhapDanhCtyEntity>;
                    bool IsMaCty = lst.Any(x => x.MaCty == e.Value.ToString());
                    if (string.IsNullOrEmpty(e.Value.ToString()) || string.IsNullOrWhiteSpace(e.Value.ToString()))
                    {
                        e.ErrorText = $"Mã Công ty không được bỏ trống";
                        e.Valid = false;
                    }
                    else if (IsMaCty)
                    {

                        e.ErrorText = $"Mã Công ty {e.Value.ToString()} đã bị trùng";
                        e.Valid = false;
                    }

                }
                else if (this.bgrvPhapDanhCty.FocusedColumn == colTenCty)
                {
                    if (string.IsNullOrEmpty(e.Value.ToString()) || string.IsNullOrWhiteSpace(e.Value.ToString()))
                    {
                        e.ErrorText = $" Tên Công ty không được bỏ trống";
                        e.Valid = false;
                    }
                }


            }
        }

        private void bgrvPhapDanhCty_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle >= 0 && e.RowHandle == bgrvPhapDanhCty.FocusedRowHandle)
            {
                e.Appearance.BackColor = ColorTranslator.FromHtml("#E6FBFF");
                e.HighPriority = true;
            }
        }

        private void pictureEdit1_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Chọn hình ảnh",
                InitialDirectory = @"C:\", // Thư mục mặc định khi mở
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp", // Lọc tệp hình ảnh
                Multiselect = false // Chỉ cho phép chọn 1 tệp
            };


            if (ofd.ShowDialog() == DialogResult.OK)
            {
                ImageMerge(ofd.FileName);
            }
        }
        private string currentImageFileName;
        private void InsertImage(string Image2)
        {
            try
            {
                if (Image2 == null) return;
                string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", Image2.ToString());
                ImageMerge(imagePath);
            }
            catch (Exception ex)
            {
            }
        }
        private void ImageMerge(string imagePath)
        {
            Image img = Image.FromFile(imagePath);
            img = new Bitmap(img, new Size(300, 250));
            pictureEdit1.Image = img;
            currentImageFileName = Path.GetFileName(imagePath);
        }
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            Image currentImage = pictureEdit1.Image;
            if (currentImage != null)
            {
                // Chuyển hình ảnh thành mảng byte
                using (MemoryStream ms = new MemoryStream())
                {
                    try
                    {
                        currentImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        byte[] imageBytes = ms.ToArray();

                        string base64String = Convert.ToBase64String(imageBytes);
                        var data = new
                        {
                            Image = base64String,
                            TenHinh = currentImageFileName,

                        };
                        byte[] bytes = Convert.FromBase64String(base64String);

                        string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
                        string imageName = currentImageFileName.ToString();
                        // Tạo tên hình ảnh mới
                        var mPath = string.Format(@"{0}\{1}", folderPath, imageName);

                        if (File.Exists(mPath))
                        {
                            File.Delete(mPath);
                        }

                        Image image = Image.FromStream(ms);
                        image.Save(mPath);
                        string url = $"{URL}PhapDanhCty/PostImageInFolder";
                        string json = Task.Run(async () => { return await _clientExtension.PostAsync(url, data); }).Result;
                        
                        LoadPhapDanhCty();
                        clsWaitForm.ShowSuccessForm(this, 1000);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            else
            {
            }
        }


        private void bgrvPhapDanhCty_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {

        }

        private void bgrvPhapDanhCty_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            if (e.Column.FieldName == "ImageLoGo")
            {
                try
                {

                    // Lấy tên hình ảnh từ giá trị ô
                    string imageFileName = e.CellValue as string;
                    if (!string.IsNullOrEmpty(imageFileName))
                    {
                        string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", imageFileName);
                        if (File.Exists(imagePath))
                        {
                            // Vẽ hình ảnh vào ô
                            Image img = Image.FromFile(imagePath);
                            e.Graphics.DrawImage(img, e.Bounds.Left, e.Bounds.Top, 70, 50);  // Vẽ logo nhỏ ở góc trái của ô

                            // Vẽ tên cột (hoặc văn bản bạn muốn hiển thị)
                            //string columnTitle = e.Column.Caption;  // Lấy tên cột
                            e.Appearance.ForeColor = Color.Transparent;

                            e.Appearance.Font = new Font("Arial", 1, FontStyle.Bold); // Font chữ
                            e.DisplayText = "";
                            // Vẽ tên cột bên cạnh logo
                            //e.Graphics.DrawString(columnTitle, e.Appearance.Font, Brushes.Black, e.Bounds.Left + 25, e.Bounds.Top + 5);
                        }
                    }
                }
                catch
                {
                    e.Appearance.ForeColor = Color.Transparent;
                    e.Appearance.Font = new Font("Arial", 1, FontStyle.Bold); // Font chữ
                    e.DisplayText = "";
                }
            }
        }

        private void gridView_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }
        private void bgrvPhapDanhCty_CustomDrawBandHeader(object sender, DevExpress.XtraGrid.Views.BandedGrid.BandHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Band == null) return;

                // Draw border
                Rectangle rect = e.Bounds;
                ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);


                Brush brush = e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 204, 102), Color.FromArgb(255, 204, 102), e.Band.AppearanceHeader.GradientMode);
                rect.Inflate(-1, -1);
                e.Graphics.FillRectangle(brush, rect);


                Font font = new Font(e.Appearance.Font, FontStyle.Bold);
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                SolidBrush textColorBrush = new SolidBrush(Color.FromArgb(0, 17, 102));
                e.Graphics.DrawString(e.Info.Caption, font, textColorBrush, e.Info.CaptionRect, stringFormat);


                foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
                {
                    DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
                }

                e.Handled = true;
            }
            catch (Exception ex)
            {

            }
        }



    }
}