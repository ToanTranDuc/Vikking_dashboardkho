using DevExpress.XtraEditors;
using DevExpress.XtraLayout.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmTimKiem : DevExpress.XtraEditors.XtraForm
    {
        private string _madhdt = string.Empty, _poiddt = string.Empty, _mapkldt = string.Empty, _tendvsxdt = string.Empty, 
        _malenhdt = string.Empty, _tenlenhdt = string.Empty, _mapkl = string.Empty;
        private int _sttthung = 0, _slkhdt = 0, _slthdt = 0, _slcldt = 0, _kiemtra = 0;
        private bool _IsDongThung = false;

        public int SoLuong { get; private set; }
        public frmTimKiem()
        {
            InitializeComponent();
        }

      
        private void btnXacNhan_Click_1(object sender, EventArgs e)
        {
           
            DialogResult = DialogResult.OK;
            this.Close();
        }
        private void btnHuy_Click_1(object sender, EventArgs e)
        {
            this.Close(); 
        }

    }
}
