using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NtbSoft.ERP.Libs
{
    public class clsConsts
    {

        public const int PortRemote = 10841;
        public const int PortStatus = 10881;
       // public const int PortBroadcast = 9125; //xuong bat dau chuyen 11->22
        public const int PortBroadcast = 9126;  //Xuong bat dau tu chuyen 1->10

        /// <summary>
        /// HH:mm
        /// </summary>
        public const string FormatHour = @"hh\:mm";
        /// <summary>
        /// "HH:mm:ss"
        /// </summary>
        public const string FormatHour24 = "HH:mm:ss";
        /// <summary>
        /// "HH:mm:00"
        /// </summary>
        public const string FormatHHmm = "HH:mm:00";
        /// <summary>
        /// "dd/MM/yyyy"
        /// </summary>
        public const string FormatDMY = "dd/MM/yyyy";
        /// <summary>
        /// "MM/dd/yyyy"
        /// </summary>
        public const string FormatMDY = "MM/dd/yyyy";
        /// <summary>
        /// "d MMM"
        /// </summary>
        public const string FormatDMMM = "dd MMM";

        /// <summary>
        /// "MM/dd/yyyy HH:mm:ss"
        /// </summary>
        public const string FormatMDYHms = "MM/dd/yyyy HH:mm:ss";

        public const string Remote_clsServer = "Remote_clsServer";
        public const string Remote_clsSettingSoundLine = "Remote_clsSettingSoundLine";
        public const string Remote_clsTimeLine = "Remote_clsTimeLine";
        public const string Remote_clsLineCurrent = "Remote_clsLineCurrent";
        public const string Remote_clsApplication = "Remote_clsApplication";
        public const string Remote_clsLineTimeShift = "Remote_clsLineTimeShift";
    }
}
