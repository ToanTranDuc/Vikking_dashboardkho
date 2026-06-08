using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using NtbSoft.ERP.Entity.ThuVien;

namespace NtbSoft.ERP.Win.Utils
{
    public class Helper
    {
        //string _userName;
        //string _passWord;

        //public Helper(RememberMe me)
        //{
        //    this._userName = me.UserID;
        //    this._passWord = me.PassWord;
        //}
        string path = Path.Combine(Application.StartupPath, @"Account\");

        public RememberMe GetRememberMe()
        {
            RememberMe rMe = new RememberMe();
            if (File.Exists(path + "account.txt"))
            {
                using (StreamReader sr = File.OpenText(path))
                {
                    string s = "";
                    while (sr.ReadLine() != null)
                    {
                        s += sr.ReadLine() + "||";
                    }
                    string[] arr = s.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
                    if (arr.Length > 1)
                    {
                        rMe.UserID = arr[0];
                        rMe.PassWord = arr[1].Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries)[0];
                        string isCheck = arr[1].Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries)[1];
                        if (isCheck == Boolean.FalseString)
                        {
                            rMe.IsRememberMe = false;
                        }
                        else
                        {
                            rMe.IsRememberMe = true;
                        }

                    }
                    return rMe;
                }
            }
            return rMe;

        }
        public void Forgot()
        {
            if (File.Exists(path + "account.txt"))
            {
                string[] txtList = Directory.GetFiles(path, "account.txt");
                foreach (string f in txtList)
                {
                    File.Delete(f);
                }
            }
        }
        public void SaveMe(RememberMe rMe)
        {

            if (!File.Exists(path + "account.txt"))
            {
                using (StreamWriter sw = File.CreateText(path + "account.txt"))
                {
                    sw.WriteLine(rMe.UserID);
                    sw.WriteLine(rMe.PassWord);
                    sw.WriteLine(rMe.IsRememberMe.ToString());
                }
            }
        }

        public List<QuocKy> lstQuocKy = new List<QuocKy> {
            new QuocKy(Properties.Resources.vn, "VN"), new QuocKy(Properties.Resources.ad, "AD"), new QuocKy(Properties.Resources.ae, "AE"), new QuocKy(Properties.Resources.af, "AF"), new QuocKy(Properties.Resources.ag, "AG")
            , new QuocKy(Properties.Resources.ai, "AI"), new QuocKy(Properties.Resources.al, "AL"),new QuocKy(Properties.Resources.am, "AM"),new QuocKy(Properties.Resources.ao, "AO"), new QuocKy(Properties.Resources.aq, "AQ")
            , new QuocKy(Properties.Resources.ar, "AR"), new QuocKy(Properties.Resources._as, "AS"),new QuocKy(Properties.Resources.at, "AT"),new QuocKy(Properties.Resources.au, "AU"), new QuocKy(Properties.Resources.aw, "AW")
            , new QuocKy(Properties.Resources.ax, "AX"), new QuocKy(Properties.Resources.az, "AZ"),new QuocKy(Properties.Resources.ba, "BA"),new QuocKy(Properties.Resources.bb, "BB"), new QuocKy(Properties.Resources.bd, "BD")
            , new QuocKy(Properties.Resources.be, "BE"), new QuocKy(Properties.Resources.bf, "BF"),new QuocKy(Properties.Resources.bg, "BG"),new QuocKy(Properties.Resources.bh, "BH"), new QuocKy(Properties.Resources.bi, "BI")
            , new QuocKy(Properties.Resources.bj, "BJ"), new QuocKy(Properties.Resources.bl, "BL"),new QuocKy(Properties.Resources.bm, "BM"),new QuocKy(Properties.Resources.bn, "BN"), new QuocKy(Properties.Resources.bo, "BO")
            , new QuocKy(Properties.Resources.bq, "BQ"), new QuocKy(Properties.Resources.br, "BR"),new QuocKy(Properties.Resources.bs, "BS"),new QuocKy(Properties.Resources.bt, "BT"), new QuocKy(Properties.Resources.bv, "BV")
            , new QuocKy(Properties.Resources.bw, "BW"), new QuocKy(Properties.Resources.by, "BY"),new QuocKy(Properties.Resources.bz, "BZ"), new QuocKy(Properties.Resources.ca, "CA"),new QuocKy(Properties.Resources.cc, "CC")
            , new QuocKy(Properties.Resources.cd, "CD"), new QuocKy(Properties.Resources.cf, "CF"), new QuocKy(Properties.Resources.cg, "CG"), new QuocKy(Properties.Resources.ch, "CH"),new QuocKy(Properties.Resources.ci, "CI")
            , new QuocKy(Properties.Resources.ck, "CK") , new QuocKy(Properties.Resources.cl, "CL"), new QuocKy(Properties.Resources.cm, "CM"), new QuocKy(Properties.Resources.cn, "CN"),new QuocKy(Properties.Resources.co, "CO")
            , new QuocKy(Properties.Resources.cr, "CR"), new QuocKy(Properties.Resources.cu, "CU"), new QuocKy(Properties.Resources.cv, "CV"), new QuocKy(Properties.Resources.cw, "CW"),new QuocKy(Properties.Resources.cx, "CX")
            , new QuocKy(Properties.Resources.cy, "CY"), new QuocKy(Properties.Resources.cz, "CZ"), new QuocKy(Properties.Resources.de, "DE"), new QuocKy(Properties.Resources.dj, "DJ"),new QuocKy(Properties.Resources.dk, "DK")
            , new QuocKy(Properties.Resources.dm, "DM"), new QuocKy(Properties.Resources._do, "DO"), new QuocKy(Properties.Resources.dz, "DZ"), new QuocKy(Properties.Resources.ec, "EC"),new QuocKy(Properties.Resources.ee, "EE")
            , new QuocKy(Properties.Resources.eg, "EG"), new QuocKy(Properties.Resources.eh, "EH"), new QuocKy(Properties.Resources.er, "ER"), new QuocKy(Properties.Resources.es, "ES"),new QuocKy(Properties.Resources.et, "ET")
            , new QuocKy(Properties.Resources.eu, "EU"), new QuocKy(Properties.Resources.fi, "FI"), new QuocKy(Properties.Resources.fj, "FJ"), new QuocKy(Properties.Resources.fk, "FK"),new QuocKy(Properties.Resources.fm, "FM")
            , new QuocKy(Properties.Resources.fo, "FO"), new QuocKy(Properties.Resources.fr, "FR"), new QuocKy(Properties.Resources.ga, "GA"), new QuocKy(Properties.Resources.gb, "GB"),new QuocKy(Properties.Resources.gb_eng, "GB_ENG")
            , new QuocKy(Properties.Resources.gb_nir, "GB_NIR"), new QuocKy(Properties.Resources.gb_sct, "GB_SCT"), new QuocKy(Properties.Resources.gb_wls, "GB_WLS"), new QuocKy(Properties.Resources.gd, "GD"),new QuocKy(Properties.Resources.ge, "GE")
            , new QuocKy(Properties.Resources.gf, "GF"), new QuocKy(Properties.Resources.gg, "GG"), new QuocKy(Properties.Resources.gh, "GH"), new QuocKy(Properties.Resources.gi, "GI"),new QuocKy(Properties.Resources.gl, "GL")
            , new QuocKy(Properties.Resources.gm, "GM"), new QuocKy(Properties.Resources.gn, "GN"), new QuocKy(Properties.Resources.gp, "GP"), new QuocKy(Properties.Resources.gq, "GQ"),new QuocKy(Properties.Resources.gr, "GR")
            , new QuocKy(Properties.Resources.gs, "GS"), new QuocKy(Properties.Resources.gt, "GT"), new QuocKy(Properties.Resources.gu, "GU"), new QuocKy(Properties.Resources.gw, "GW"),new QuocKy(Properties.Resources.gy, "GY")
            , new QuocKy(Properties.Resources.hk, "HK"), new QuocKy(Properties.Resources.hm, "HM"), new QuocKy(Properties.Resources.hn, "HN"), new QuocKy(Properties.Resources.hr, "HR"),new QuocKy(Properties.Resources.ht, "HT")
            , new QuocKy(Properties.Resources.hu, "HU"), new QuocKy(Properties.Resources.id, "ID"), new QuocKy(Properties.Resources.ie, "IE"), new QuocKy(Properties.Resources.il, "IL"),new QuocKy(Properties.Resources.im, "IM")
            , new QuocKy(Properties.Resources._in, "IN"), new QuocKy(Properties.Resources.io, "IO"), new QuocKy(Properties.Resources.iq, "IQ"), new QuocKy(Properties.Resources.ir, "IR"),new QuocKy(Properties.Resources._is, "IS")
            , new QuocKy(Properties.Resources.it, "IT"), new QuocKy(Properties.Resources.je, "JE"), new QuocKy(Properties.Resources.jm, "JM"), new QuocKy(Properties.Resources.jo, "JO"),new QuocKy(Properties.Resources.jp, "JP")
            , new QuocKy(Properties.Resources.ke, "KE"), new QuocKy(Properties.Resources.kg, "KG"), new QuocKy(Properties.Resources.kh, "KH"), new QuocKy(Properties.Resources.ki, "KI"),new QuocKy(Properties.Resources.km, "KM")
            , new QuocKy(Properties.Resources.kn, "KN"), new QuocKy(Properties.Resources.kp, "KP"), new QuocKy(Properties.Resources.kr, "KR"), new QuocKy(Properties.Resources.kw, "KW"),new QuocKy(Properties.Resources.ky, "KY")
            , new QuocKy(Properties.Resources.kz, "KZ"), new QuocKy(Properties.Resources.la, "LA"), new QuocKy(Properties.Resources.lb, "LB"), new QuocKy(Properties.Resources.lc, "LC"),new QuocKy(Properties.Resources.li, "LI")
            , new QuocKy(Properties.Resources.lk, "LK"), new QuocKy(Properties.Resources.lr, "LR"), new QuocKy(Properties.Resources.ls, "LS"), new QuocKy(Properties.Resources.lt, "LT"),new QuocKy(Properties.Resources.lu, "LU")
            , new QuocKy(Properties.Resources.lv, "LV"), new QuocKy(Properties.Resources.ly, "LY"), new QuocKy(Properties.Resources.ma, "MA"), new QuocKy(Properties.Resources.mc, "MC"),new QuocKy(Properties.Resources.md, "MD")
            , new QuocKy(Properties.Resources.me, "ME"), new QuocKy(Properties.Resources.mf, "MF"), new QuocKy(Properties.Resources.mg, "MG"), new QuocKy(Properties.Resources.mh, "MH"),new QuocKy(Properties.Resources.mk, "MK")
            , new QuocKy(Properties.Resources.ml, "ML"), new QuocKy(Properties.Resources.mm, "MM"), new QuocKy(Properties.Resources.mn, "MN"), new QuocKy(Properties.Resources.mo, "MO"),new QuocKy(Properties.Resources.mp, "MP")
            , new QuocKy(Properties.Resources.mq, "MQ"), new QuocKy(Properties.Resources.mr, "MR"), new QuocKy(Properties.Resources.ms, "MS"), new QuocKy(Properties.Resources.mt, "MT"),new QuocKy(Properties.Resources.mu, "MU")
            , new QuocKy(Properties.Resources.mv, "MV"), new QuocKy(Properties.Resources.mw, "MW"), new QuocKy(Properties.Resources.mx, "MX"), new QuocKy(Properties.Resources.my, "MY"),new QuocKy(Properties.Resources.mz, "MZ")
            , new QuocKy(Properties.Resources.na, "NA"), new QuocKy(Properties.Resources.nc, "NC"), new QuocKy(Properties.Resources.ne, "NE"), new QuocKy(Properties.Resources.nf, "NF"),new QuocKy(Properties.Resources.ng, "NG")
            , new QuocKy(Properties.Resources.ni, "NI"), new QuocKy(Properties.Resources.nl, "NL"), new QuocKy(Properties.Resources.no, "NO"), new QuocKy(Properties.Resources.np, "NP"),new QuocKy(Properties.Resources.nr, "NR")
            , new QuocKy(Properties.Resources.nu, "NU"), new QuocKy(Properties.Resources.nz, "NZ"), new QuocKy(Properties.Resources.om, "OM"), new QuocKy(Properties.Resources.pa, "PA"),new QuocKy(Properties.Resources.pe, "PE")
            , new QuocKy(Properties.Resources.pf, "PF"), new QuocKy(Properties.Resources.pg, "PG"), new QuocKy(Properties.Resources.ph, "PH"), new QuocKy(Properties.Resources.pk, "PK"),new QuocKy(Properties.Resources.pl, "PL")
            , new QuocKy(Properties.Resources.pm, "PM"), new QuocKy(Properties.Resources.pn, "PN"), new QuocKy(Properties.Resources.pr, "PR"), new QuocKy(Properties.Resources.ps, "PS"),new QuocKy(Properties.Resources.pt, "PT")
            , new QuocKy(Properties.Resources.pw, "PW"), new QuocKy(Properties.Resources.py, "PY"), new QuocKy(Properties.Resources.qa, "QA"), new QuocKy(Properties.Resources.re, "RE"),new QuocKy(Properties.Resources.ro, "RO")
            , new QuocKy(Properties.Resources.rs, "RS"), new QuocKy(Properties.Resources.ru, "RU"), new QuocKy(Properties.Resources.rw, "RW"), new QuocKy(Properties.Resources.sa, "SA"),new QuocKy(Properties.Resources.sb, "SB")
            , new QuocKy(Properties.Resources.sc, "SC"), new QuocKy(Properties.Resources.sd, "SD"), new QuocKy(Properties.Resources.se, "SE"), new QuocKy(Properties.Resources.sg, "SG"),new QuocKy(Properties.Resources.sh, "SH")
            , new QuocKy(Properties.Resources.si, "SI"), new QuocKy(Properties.Resources.sj, "SJ"), new QuocKy(Properties.Resources.sk, "SK"), new QuocKy(Properties.Resources.sl, "SL"),new QuocKy(Properties.Resources.sm, "SM")
            , new QuocKy(Properties.Resources.sn, "SN"), new QuocKy(Properties.Resources.so, "SO"), new QuocKy(Properties.Resources.sr, "SR"), new QuocKy(Properties.Resources.ss, "SS"),new QuocKy(Properties.Resources.st, "ST")
            , new QuocKy(Properties.Resources.sv, "SV"), new QuocKy(Properties.Resources.sx, "SX"), new QuocKy(Properties.Resources.sy, "SY"), new QuocKy(Properties.Resources.sz, "SZ"),new QuocKy(Properties.Resources.tc, "TC")
            , new QuocKy(Properties.Resources.td, "TD"), new QuocKy(Properties.Resources.tf, "TF"), new QuocKy(Properties.Resources.tg, "TG"), new QuocKy(Properties.Resources.th, "TH"),new QuocKy(Properties.Resources.tj, "TJ")
            , new QuocKy(Properties.Resources.tk, "TK"), new QuocKy(Properties.Resources.tl, "TL"), new QuocKy(Properties.Resources.tm, "TM"), new QuocKy(Properties.Resources.tn, "TN"),new QuocKy(Properties.Resources.to, "TO")
            , new QuocKy(Properties.Resources.tr, "TR"), new QuocKy(Properties.Resources.tt, "TT"), new QuocKy(Properties.Resources.tv, "TV"), new QuocKy(Properties.Resources.tw, "TW"),new QuocKy(Properties.Resources.tz, "TZ")
            , new QuocKy(Properties.Resources.ua, "UA"), new QuocKy(Properties.Resources.ug, "UG"), new QuocKy(Properties.Resources.um, "UM"), new QuocKy(Properties.Resources.us, "US"),new QuocKy(Properties.Resources.uy, "UY")
            , new QuocKy(Properties.Resources.uz, "UZ"), new QuocKy(Properties.Resources.va, "VA"), new QuocKy(Properties.Resources.vc, "VC"), new QuocKy(Properties.Resources.ve, "VE"),new QuocKy(Properties.Resources.vg, "VG")
            , new QuocKy(Properties.Resources.vi, "VI"), new QuocKy(Properties.Resources.vu, "VU"), new QuocKy(Properties.Resources.wf, "WF"),new QuocKy(Properties.Resources.ws, "WS"), new QuocKy(Properties.Resources.xk, "XK")
            , new QuocKy(Properties.Resources.ye, "YE"), new QuocKy(Properties.Resources.yt, "YT"), new QuocKy(Properties.Resources.za, "ZA"), new QuocKy(Properties.Resources.zm, "ZM"), new QuocKy(Properties.Resources.zw, "ZW")
};

    }
    public class RememberMe
    {
        private string _userID;
        private string _passWord;
        private bool _isRememberMe;

        public RememberMe(string userId = "", string password = "", bool isRememberMe = false)
        {
            this._userID = userId;
            this._passWord = password;
            this._isRememberMe = isRememberMe;
        }
        public string UserID
        {
            set { _userID = value; }
            get { return _userID; }
        }
        public string PassWord
        {
            set { _passWord = value; }
            get { return _passWord; }
        }
        public bool IsRememberMe
        {
            set { _isRememberMe = value; }
            get { return _isRememberMe; }
        }
    }
}
