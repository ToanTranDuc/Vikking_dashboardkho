using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.ApplicationServices;
using DevExpress.XtraEditors;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Utils
{
    class SingleInstanceApp : WindowsFormsApplicationBase
    {
        public SingleInstanceApp()
        {
        }
        public SingleInstanceApp(Form f)
        {
            // Set IsSingleInstance property to true to make the application 
            base.IsSingleInstance = true;
            // Set MainForm of the application.
            this.MainForm = f;
        }
    }
}
