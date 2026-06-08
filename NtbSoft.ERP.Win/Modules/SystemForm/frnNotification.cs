using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.XtraGrid.Views.Card.ViewInfo;
using Newtonsoft.Json;
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

namespace NtbSoft.ERP.Win.Modules.SystemForm
{
    public partial class frnNotification : DevExpress.XtraEditors.XtraForm
    {
        string URL = string.Empty;
        System.Configuration.AppSettingsReader settingsReader =
                                     new System.Configuration.AppSettingsReader();


        private HttpClientExtension _clientExtension;
        public frnNotification()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            LoadNotify(0);
            SetUpCardView();
        }

        private void SetUpCardView()
        {


            
            cardViewNotifyChuaDoc.CardCaptionFormat = $"EMS Messager";
            cardViewNotifyLichSu.CardCaptionFormat = $"EMS Messager";


            cardViewNotifyChuaDoc.OptionsView.ShowQuickCustomizeButton = false;
            cardViewNotifyLichSu.OptionsView.ShowQuickCustomizeButton = false;


            cardViewNotifyChuaDoc.OptionsBehavior.FieldAutoHeight = true;
            cardViewNotifyLichSu.OptionsBehavior.FieldAutoHeight = true;

            //cardViewNotifyChuaDoc.CustomDrawCardFieldValue += CardViewNotifyChuaDoc_CustomDrawCardFieldValue; 

            cardViewNotifyLichSu.OptionsView.ShowCardExpandButton = false;
            cardViewNotifyChuaDoc.OptionsView.ShowCardExpandButton = false;

            cardViewNotifyLichSu.OptionsView.ShowEmptyFields = true;
            cardViewNotifyChuaDoc.OptionsView.ShowEmptyFields = true;

            cardViewNotifyLichSu.MaximumCardColumns = 1;
            cardViewNotifyChuaDoc.MaximumCardColumns = 1;

           

            
            RepositoryItemMemoEdit memoEdit1 = new RepositoryItemMemoEdit();
            memoEdit1.WordWrap = true;
            memoEdit1.ScrollBars = System.Windows.Forms.ScrollBars.None;


            foreach (GridColumn col in cardViewNotifyChuaDoc.Columns)
            {
                col.ColumnEdit = memoEdit1;
                col.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                col.AppearanceCell.Options.UseTextOptions = true;
            }


            RepositoryItemMemoEdit memoEdit2 = new RepositoryItemMemoEdit();
            memoEdit2.WordWrap = true;
            memoEdit2.ScrollBars = System.Windows.Forms.ScrollBars.None;


            //cardViewNotifyLichSu.OptionsView.ShowCardCaption = false;
            //cardViewNotifyChuaDoc.OptionsView.ShowCardCaption = false;
            cardViewNotifyLichSu.OptionsView.ShowFieldCaptions = false;
            cardViewNotifyChuaDoc.OptionsView.ShowFieldCaptions = false;
            foreach (GridColumn col in cardViewNotifyLichSu.Columns)
            {

                col.ColumnEdit = memoEdit2;
                col.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                col.AppearanceCell.Options.UseTextOptions = true;
            }

            cardViewNotifyChuaDoc.CardWidth = 550;
            cardViewNotifyLichSu.CardWidth = 550;
        }

        private void CardViewNotifyChuaDoc_CustomDrawCardFieldValue(object sender,
         DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            CardView view = sender as CardView;
            if (view == null) return;
            try
            {
                object cellValue = view.GetRowCellValue(e.RowHandle, e.Column);
                
                if (e.Column.FieldName == "TrangThai")
                {
                    string status = cellValue?.ToString();
                    switch (status)
                    {
                        case "Đã xem":
                            e.Appearance.ForeColor = Color.Green;
                            e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                            e.Appearance.BackColor = Color.AliceBlue;
                            break;
                        case "Chưa xem":
                            e.Appearance.ForeColor = Color.Red;
                            e.Appearance.BackColor = Color.MistyRose;
                            break;
                    }
                }
                else if (e.Column.FieldName == "NotificationTime")
                {
                    e.Appearance.ForeColor = Color.Red;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
                else if (e.Column.FieldName == "Title")
                {

                    e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, 13f, FontStyle.Bold);
                    e.Appearance.ForeColor = Color.FromArgb(30, 80, 160);
                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;

                    
                }
                else if (e.Column.FieldName == "Detail")
                {

                    e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, 10f, FontStyle.Bold);
                    e.Appearance.ForeColor = Color.FromArgb(50, 50, 50);
                    if (e.RowHandle == view.FocusedRowHandle)
                    {
                        e.Appearance.BackColor = Color.FromArgb(255, 220, 185);
                        e.Appearance.BorderColor = Color.FromArgb(230, 150, 80);
                    }
                }
                else
                {

                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
            }
            catch (Exception ex)
            {

            }

        }

        private void LoadNotify(int status)
        {
            try
            {



                DataTable tbl = new DataTable();
                string url = $"{URL}NotifyManger/Get?action=GetNotification&para1={GlobleData.UserName}&para2={status}&para3=NONE&para4=NONE&para5=NONE&para6=0";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json != "[]")
                {
                    tbl = JsonConvert.DeserializeObject<DataTable>(json);


                }
                if (tabPaneNotify.SelectedPage == tabChuaDoc)
                {
                    gridControl1.DataSource = tbl;

                    cardViewNotifyChuaDoc.BeginSort();
                    cardViewNotifyChuaDoc.ClearSorting();
                    cardViewNotifyChuaDoc.Columns["NotificationTime"].SortOrder =
                        DevExpress.Data.ColumnSortOrder.Descending;
                    cardViewNotifyChuaDoc.EndSort();
                }
                else if (tabPaneNotify.SelectedPage == tabLichSu)
                {
                    gridControl2.DataSource = tbl;
                    cardViewNotifyLichSu.BeginSort();
                    cardViewNotifyLichSu.ClearSorting();
                    cardViewNotifyLichSu.Columns["NotificationTime"].SortOrder =
                        DevExpress.Data.ColumnSortOrder.Descending;
                    cardViewNotifyLichSu.EndSort();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void LoadNotify(int status, DateTime FromDate, DateTime ToDate)
        {
            try
            {

                string para3 = FromDate == DateTime.MinValue ? "NONE" : FromDate.ToString("yyyy-MM-dd");
                string para4 = ToDate == DateTime.MinValue ? "NONE" : ToDate.ToString("yyyy-MM-dd");

                DataTable tbl = new DataTable();
                string url = $"{URL}NotifyManger/Get?action=GetNotification&para1={GlobleData.UserName}&para2={status}&para3={para3}&para4={para4}&para5={txtSeach.EditValue}&para6=0";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json != "[]")
                {
                    tbl = JsonConvert.DeserializeObject<DataTable>(json);


                }
                if (tabPaneNotify.SelectedPage == tabChuaDoc)
                {
                    gridControl1.DataSource = tbl;

                    cardViewNotifyChuaDoc.BeginSort();
                    cardViewNotifyChuaDoc.ClearSorting();
                    cardViewNotifyChuaDoc.Columns["NotificationTime"].SortOrder =
                        DevExpress.Data.ColumnSortOrder.Descending;
                    cardViewNotifyChuaDoc.EndSort();
                }
                else if (tabPaneNotify.SelectedPage == tabLichSu)
                {
                    gridControl2.DataSource = tbl;
                    cardViewNotifyLichSu.BeginSort();
                    cardViewNotifyLichSu.ClearSorting();
                    cardViewNotifyLichSu.Columns["NotificationTime"].SortOrder =
                        DevExpress.Data.ColumnSortOrder.Descending;
                    cardViewNotifyLichSu.EndSort();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void tabPaneNotify_SelectedPageChanged(object sender, DevExpress.XtraBars.Navigation.SelectedPageChangedEventArgs e)
        {
            if (tabPaneNotify.SelectedPage == tabChuaDoc)
            {
                LoadNotify(0);
            }
            else if (tabPaneNotify.SelectedPage == tabLichSu)
            {
                LoadNotify(1);
            }
        }
        /*Lọc*/
        private void barButtonItem18_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DateTime FromDate = clsForrmatUtils.ConvertDate(fromDate.EditValue);
            DateTime ToDate = clsForrmatUtils.ConvertDate(toDate.EditValue);

            if (tabPaneNotify.SelectedPage == tabChuaDoc)
            {
                LoadNotify(0, FromDate, ToDate);
            }
            else if (tabPaneNotify.SelectedPage == tabLichSu)
            {
                LoadNotify(1, FromDate, ToDate);
            }

        }

        private void cardViewNotifyChuaDoc_CustomDrawCardCaption(object sender,
         DevExpress.XtraGrid.Views.Card.CardCaptionCustomDrawEventArgs e)
        {
            try
            {
                Color headerBg = Color.FromArgb(207, 226, 255);
                Color headerBorder = Color.FromArgb(100, 160, 230);

                using (SolidBrush bgBrush = new SolidBrush(headerBg))
                {
                    e.Cache.Graphics.FillRectangle(bgBrush, e.Bounds);
                }


                using (Pen borderPen = new Pen(headerBorder, 1))
                {
                    e.Cache.Graphics.DrawLine(borderPen,
                        e.Bounds.Left, e.Bounds.Bottom - 1,
                        e.Bounds.Right, e.Bounds.Bottom - 1);
                }


                string logoPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "Resources", "logo.ico");

                int imgSize = e.Bounds.Height + 12;
                Rectangle imgRect = new Rectangle(
                    e.Bounds.Left + 4,
                    e.Bounds.Top + 3,
                    imgSize,
                    imgSize);

                if (File.Exists(logoPath))
                {
                    using (Image logo = Image.FromFile(logoPath))
                    {
                        e.Cache.Graphics.DrawImage(logo, imgRect);
                    }
                }


                string captionText = e.CardCaption;
                Rectangle textRect = new Rectangle(
                    imgRect.Right + 6,
                    e.Bounds.Top,
                    e.Bounds.Width - imgRect.Width - 16,
                    e.Bounds.Height);

                using (SolidBrush textBrush = new SolidBrush(Color.FromArgb(30, 80, 160)))
                using (Font captionFont = new Font(e.Appearance.Font.FontFamily, 9.5f, FontStyle.Bold))
                {
                    StringFormat sf = new StringFormat
                    {
                        Alignment = StringAlignment.Near,
                        LineAlignment = StringAlignment.Center,
                        Trimming = StringTrimming.EllipsisCharacter
                    };
                    e.Cache.Graphics.DrawString(captionText, captionFont, textBrush, textRect, sf);
                }


                e.Handled = true;
            }
            catch (Exception ex)
            {

            }

        }

        private void btnWaychAll_Click(object sender, EventArgs e)
        {
            try
            {
                string url = URL + $"NotifyManger/Update?action=SetNotifyWatched&para1={GlobleData.UserName}&para2=ALL&para3=0";

                string msResult = Task.Run(async () =>
                {
                    return await _clientExtension.PostAsync(url, null);
                }).Result;
                if(msResult?.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);

                    LoadNotify(0);

                    frmMain mainForm = Application.OpenForms.OfType<frmMain>().FirstOrDefault();
                    if (mainForm != null)
                    {
                        if (mainForm.InvokeRequired)
                        {
                            mainForm.Invoke(new Action(async () => await Task.Run(() => mainForm.SetNotifyBadge())));
                        }
                        else
                        {
                            mainForm.SetNotifyBadge(); // async void — gọi thẳng không cần await
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {

            }
        }

        private void btnWatch_Click(object sender, EventArgs e)
        {
            try
            {
                string NotifyID = "";
                if (tabPaneNotify.SelectedPage == tabChuaDoc)
                {
                    DataRow rowFocused = cardViewNotifyChuaDoc.GetFocusedDataRow();
                    if (rowFocused != null)
                    {
                        NotifyID = rowFocused["NotificationID"]?.ToString();
                    }
                }
                
                if (string.IsNullOrEmpty(NotifyID))
                {
                    return;
                }
                string url = URL + $"NotifyManger/Update?action=SetNotifyWatched&para1={GlobleData.UserName}&para2={NotifyID}&para3=0";

                string msResult = Task.Run(async () =>
                {
                    return await _clientExtension.PostAsync(url, null);
                }).Result;

                if(msResult?.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);

                    LoadNotify(0);
                    frmMain mainForm = Application.OpenForms.OfType<frmMain>().FirstOrDefault();
                    if (mainForm != null)
                    {
                        if (mainForm.InvokeRequired)
                        {
                            mainForm.Invoke(new Action(async () => await Task.Run(() => mainForm.SetNotifyBadge())));
                        }
                        else
                        {
                            mainForm.SetNotifyBadge(); // async void — gọi thẳng không cần await
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}