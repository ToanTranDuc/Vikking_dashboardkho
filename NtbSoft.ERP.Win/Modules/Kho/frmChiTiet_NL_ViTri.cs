using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ViTriKho
{
    public partial class frmChiTiet_NL_ViTri : DevExpress.XtraEditors.XtraForm
    {
        DataTable _tblChiTietViTri = new DataTable();

        public frmChiTiet_NL_ViTri(DataTable tbl, DataRow drVT)
        {
            InitializeComponent();
            _tblChiTietViTri = tbl;
            //CreateFormat();
            BindingData(tbl, drVT);
            
        }

        private void BindingData(DataTable tbl, DataRow dr)
        {
            double sumCBM = tbl.AsEnumerable().Where(x => (bool)x["Status_PX"] == false).Sum(x => (double)x["CBM"]);
            dgrCTNL.DataSource = tbl;
            lbTenVT.Text = dr["TenVT"].ToString();
            lbSucChua.Text = dr["CBM"].ToString();
            lbDaChua.Text = sumCBM.ToString();
            lbConLai.Text = (double.Parse(dr["CBM"].ToString()) - sumCBM).ToString();

        }

        //private void CreateFormat()
        //{
        //    GridFormatRule gridFormatRule1 = new GridFormatRule();
        //    FormatConditionRuleDataBar formatConditionRuleDataBar1 = new FormatConditionRuleDataBar();

        //    gridFormatRule1.Column = this.gridColumn3;
        //    gridFormatRule1.ColumnApplyTo = this.gridColumn4;
        //    gridFormatRule1.Name = "Format0";
        //    formatConditionRuleDataBar1.Appearance.BackColor = System.Drawing.Color.Aqua;
        //    formatConditionRuleDataBar1.Appearance.BackColor2 = System.Drawing.Color.White;
        //    formatConditionRuleDataBar1.Appearance.BorderColor = System.Drawing.Color.Black;
        //    formatConditionRuleDataBar1.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        //    formatConditionRuleDataBar1.Appearance.Options.UseBackColor = true;
        //    formatConditionRuleDataBar1.Appearance.Options.UseBorderColor = true;
        //    formatConditionRuleDataBar1.Maximum = new decimal(new int[] {
        //    120,
        //    0,
        //    0,
        //    0});
        //    formatConditionRuleDataBar1.MaximumType = DevExpress.XtraEditors.FormatConditionValueType.Number;
        //    formatConditionRuleDataBar1.Minimum = new decimal(new int[] {
        //    1,
        //    0,
        //    0,
        //    0});
        //    formatConditionRuleDataBar1.MinimumType = DevExpress.XtraEditors.FormatConditionValueType.Number;
        //    formatConditionRuleDataBar1.PredefinedName = null;
        //    gridFormatRule1.Rule = formatConditionRuleDataBar1;
        //    this.gridViewCTNL.FormatRules.Add(gridFormatRule1);
        //}
    }
}
