using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;
using AutoUpdaterDotNET;
using System.Configuration;
using System.Net;

namespace NtbSoft.ERP.Win.Modules.ResourceForm
{
    public partial class frmFirstRun : DevExpress.XtraEditors.XtraForm
    {
        bool _updateClick = false;
        public frmFirstRun()
        {
            InitializeComponent();
            CheckUpdate();
            AutoUpdater.CheckForUpdateEvent += new AutoUpdater.CheckForUpdateEventHandler(AutoUpdaterOnCheckForUpdateEvent);
        }

        private void btnCheckUpdate_Click(object sender, EventArgs e)
        {
            _updateClick = true;
            CheckUpdate();
        }

        private void CheckUpdate()
        {
            System.Configuration.AppSettingsReader settingsReader =
                                               new AppSettingsReader();
            string _url_update = (string)settingsReader.GetValue("URL_UP", typeof(String));
            string _file_update = (string)settingsReader.GetValue("File_UP", typeof(String));

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);

            string version = fvi.FileVersion;
            AutoUpdater.DownloadPath = _file_update;
            //System.Timers.Timer timer = new System.Timers.Timer
            //{
            //    Interval = 15 * 60 * 1000,
            //    SynchronizingObject = this
            //};
            //timer.Elapsed += delegate
            //{
            //    AutoUpdater.Start(_url_update + "Version.XML");
            //};
            //timer.Start();
            AutoUpdater.Start(_url_update + "Version.XML");
            // AutoUpdater
        }

        //Tai
        private void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            //if (!_updateClick) return;
            if (args.IsUpdateAvailable || _updateClick)
            //if (true)
            {
                try
                {
                    if (args.DownloadURL == null || (args.DownloadURL != null && args.DownloadURL == ""))
                    {
                        throw new Exception("Path download file đã xảy ra lỗi. Vui lòng kiểm tra lại.");
                    }
                    DialogResult dialogResult;
                    dialogResult =
                            XtraMessageBox.Show(
                                _updateClick ?
                                $@"Bạn ơi, phần mềm của bạn có phiên bản mới. Bạn có muốn cập nhật phần mềm không?" :
                                $@"Bạn ơi, phần mềm của bạn có phiên bản mới {args.CurrentVersion}. Phiên bản bạn đang sử dụng hiện tại  {args.InstalledVersion}. Bạn có muốn cập nhật phần mềm không?", @"Cập nhật phần mềm",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Information);
                    _updateClick = false;

                    if (dialogResult.Equals(DialogResult.Yes) || dialogResult.Equals(DialogResult.OK))
                    {
                        string currentDir = Environment.CurrentDirectory;

                        string fullDirectory = System.IO.Directory.GetParent(currentDir).FullName;

                        if (fullDirectory == null || (fullDirectory != null && fullDirectory == ""))
                        {
                            throw new Exception("Path lưu file đã xảy ra lỗi. Vui lòng kiểm tra lại");
                        }
                        AutoUpdater.InstallationPath = fullDirectory;
                        if (AutoUpdater.DownloadUpdate(args))
                        {
                            Application.Exit();
                        }

                    }
                }
                catch (WebException exception)
                {
                    MessageBox.Show(exception.Message, exception.GetType().ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, exception.GetType().ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }

        //private void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        //{
        //    //if (!_updateClick) return;
        //    if (args.IsUpdateAvailable || _updateClick)
        //    {

        //        DialogResult dialogResult;
        //        dialogResult =
        //                XtraMessageBox.Show(
        //                    _updateClick?
        //                    $@"Bạn ơi, phần mềm của bạn có phiên bản mới. Bạn có muốn cập nhật phần mềm không?":
        //                    $@"Bạn ơi, phần mềm của bạn có phiên bản mới {args.CurrentVersion}. Phiên bản bạn đang sử dụng hiện tại  {args.InstalledVersion}. Bạn có muốn cập nhật phần mềm không?", @"Cập nhật phần mềm",
        //                    MessageBoxButtons.YesNo,
        //                    MessageBoxIcon.Information);
        //        _updateClick = false;

        //        if (dialogResult.Equals(DialogResult.Yes) || dialogResult.Equals(DialogResult.OK))
        //        {
        //            try
        //            {

        //                string fileName = string.Empty, path = string.Empty;
        //                string currentDir = Environment.CurrentDirectory;
        //                DirectoryInfo directory = new DirectoryInfo(currentDir);

        //                string fullDirectory = System.IO.Directory.GetParent(currentDir).FullName;

        //                SaveFileDialog dlg = new SaveFileDialog();
        //                dlg.InitialDirectory = fullDirectory;
        //                dlg.FileName = System.IO.Path.GetFileName(args.DownloadURL);

        //                fileName = dlg.FileName;

        //                var currentDirectory = new DirectoryInfo(fileName);
        //                //if (currentDirectory.Parent != null)
        //                //{
        //                    AutoUpdater.InstallationPath = fullDirectory;
        //                //}
        //                if (AutoUpdater.DownloadUpdate(args))
        //                {
        //                    Application.Exit();
        //                }

        //            }
        //            catch (WebException exception)
        //            {
        //                MessageBox.Show(exception.Message, exception.GetType().ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            }
        //            //catch (Exception exception)
        //            //{
        //            //    MessageBox.Show(exception.Message, exception.GetType().ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            //}
        //        }
        //    }
        //}
    }
}