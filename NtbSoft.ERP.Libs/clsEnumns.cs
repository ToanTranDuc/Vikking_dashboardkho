using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NtbSoft.ERP.Libs
{
    //(*-*) ReadSound: tin hieu doc am thanh. 
    //(*-*) Connected: Trang thai ket noi. 
    //(*-*) Disconnected: Trang thai ngat ket noi
    //(*-*) DisplaySCtrl: Thay doi du lieu Sound Control
    //(*-*) DisplayData: Thay doi du lieu LineName (khi nhan duoc tin hieu co thay doi Data tu man hinh LCD, thi gui du lieu thay doi den cac Client)
    public enum Controls
    {
        Insert, Update, Delete, SendData, SendTime, SendTimeWork, TestConnect, OK, True, SendCommand,ConfigSeri,
        False, Start, Finish, ReadSound, Connected, Disconnected, DisplaySCtrl, DisplayData, ChangeTimeWork, ChangeRate, Refresh,
        RefreshSP_TP,F_KeyBoard
    }

    public enum ELoading
    {
        IsCurrent, IsTotal, IsDetail
    }

    public enum ColumnName { None, Style, KH_Tong, TH_Tong, KH_Ngay, TH_Ngay, SP_Dat, SP_Loi, CreatedEnd, SL_Giao, SL_TH, NDSX_KH }

    [Serializable]
    public enum EColumnName
    {
        None, Style, KH_Tong, TH_Tong, KH_Ngay, TH_Ngay, SP_Dat, SP_Loi, CreatedEnd,
        SL_Giao, SL_TH, NDSX_KH, NDSX_TH, UnitPrice, NoWorker, HeSo, Description
    }
    [Serializable]
    public enum EStatus { None = 0, ChangedData = 1, ChangedDateTime = 2, ChangedTimeWork = 3, ChangedLayout = 5, MainLayout = 6, SettingLCD = 7, CapBTP = 8 }
    public enum ETypeChange { None, AddNew, Edit, Delete, View }

    public enum ECacheManagerName
    {
        CacheData, CacheLineXChange, CacheSCtrlCollection
    }

    public enum DataState { None, AddNew, Edit, Delete, View }

    public class StepDefault
    {
        public const string RA_CHUYEN = "fd1a02eb-1eee-4ce8-af9f-021af05ea5cb";
        public const string QC_KIEM = "31468586-c0d2-4c4a-a025-f7978b0d8ab3";
    }

    public enum Sorts { ASC, DESC }
    public enum Action { CONFIG, DELETE, GETALL, GETPAGING,GETBYID }
}
