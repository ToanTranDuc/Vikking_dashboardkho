using NtbSoft.ERP.Model.QuanLyDonHang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Repository.R.QuanLyDonHang
{
    public interface IBomNguyenPhuLieuRepository
    {
        DataTable Get_TikKiem(string _mahang);
        DataTable GET_VatTu(string mavt);
        DataTable GET_TenVatTu();
        DataTable GETMH();
        DataTable GET_Mau(string _mahang);
        DataTable GET_InSeam(string _mahang);
        DataTable GET_Size(string _mahang/*, string _inseam*/);
        DataTable GET_KhoSizeVatTu(string _mavt);
        DataTable GET_MauVatTu(string _mavt);
        DataTable GET_KhoSizebyMau(string _mavt,string _mau);
        DataTable GET_MaubyKhoSize(string _mavt, string _khosize);
        string PostBomVT(DataTable tb);
        string DeleteBomVT(string _mahang, string _mavt);
        //string DeleteBomDongVT( DataTable tb, string MaBom);

        DataTable GET_DinhMucNPL(string _mahang);
    }
    public class BomNguyenPhuLieuRepository:IBomNguyenPhuLieuRepository
    {
        BomNguyenPhuLieuModel _model = new BomNguyenPhuLieuModel();

        public DataTable Get_TikKiem(string _mahang)
        {
            return _model.Get_TikKiem(_mahang);
        }
        public DataTable GET_VatTu(string mavt)
        {
            return _model.GET_VatTu(mavt);
        }
        public DataTable GET_TenVatTu()
        {
            return _model.GET_TenVatTu();
        }
        public DataTable GETMH()
        {
            return _model.GETMH();
        }
        public DataTable GET_Mau(string _mahang)
        {
            return _model.GET_Mau(_mahang);
        }
        public DataTable GET_InSeam(string _mahang)
        {
            return _model.GET_InSeam(_mahang);
        }
        public DataTable GET_Size(string _mahang/*, string _inseam*/)
        {
            return _model.GET_Size(_mahang/*, _inseam*/);
        }
        public DataTable GET_KhoSizeVatTu(string _mavt)
        {
            return _model.GET_KhoSizeVatTu(_mavt);
        }
        public DataTable GET_KhoSizebyMau(string _mavt,string _mau)
        {
            return _model.GET_KhoSizebyMau(_mavt, _mau);
        }
        public DataTable GET_MaubyKhoSize(string _mavt, string __khosize)
        {
            return _model.GET_MaubyKhoSize(_mavt, __khosize);
        }
        public DataTable GET_MauVatTu(string _mavt)
        {
            return _model.GET_MauVatTu(_mavt);
        }

        public string PostBomVT(DataTable tb)
        {
            return _model.PostBomVT(tb);
        }

        public string DeleteBomVT(string _mahang, string _mavt)
        {
            return _model.DeleteBomVT(_mahang, _mavt);
        }
        //public string DeleteBomDongVT(DataTable tb, string MaBom)
        //{
        //    return _model.DeleteBomDongVT(tb,MaBom);
        //}
        public DataTable GET_DinhMucNPL(string _mahang)
        {
            return _model.GET_DinhMucNPL(_mahang);
        }

    }
}