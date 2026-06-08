using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace NtbSoft.ERP.Web.Controllers
{
    public class DashboardKhoController : Controller
    {
        private const string DashboardKhoSettingsSessionKey = "DashboardKhoTimingSettings";
        private const string DashboardKhoSettingsCookieName = "DashboardKhoTimingSettings";

        private sealed class DashboardKhoTimingSettings
        {
            public DashboardKhoTimingSettings()
            {
                TvPage1To2 = 60;
                TvPage2To1 = 60;
                Page1Customer = 60;
                Page1Receiving = 60;
                Page2Shipping = 60;
                Page2Ready = 60;
            }

            public int TvPage1To2 { get; set; }
            public int TvPage2To1 { get; set; }
            public int Page1Customer { get; set; }
            public int Page1Receiving { get; set; }
            public int Page2Shipping { get; set; }
            public int Page2Ready { get; set; }
        }

        public ActionResult DashboardKho()
        {
            var settings = GetDashboardKhoSettings();
            var serializer = new JavaScriptSerializer();
            ViewBag.DashboardKhoSettingsJson = serializer.Serialize(new
            {
                tvPage1To2 = settings.TvPage1To2,
                tvPage2To1 = settings.TvPage2To1,
                page1Customer = settings.Page1Customer,
                page1Receiving = settings.Page1Receiving,
                page2Shipping = settings.Page2Shipping,
                page2Ready = settings.Page2Ready
            });
            return View();
        }

        public ActionResult DashboardKhoSetting()
        {
            PopulateSettingsViewBag(GetDashboardKhoSettings());
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DashboardKhoSetting(
            int? TvPage1To2 = null,
            int? TvPage2To1 = null,
            int? Page1Customer = null,
            int? Page1Receiving = null,
            int? Page2Shipping = null,
            int? Page2Ready = null)
        {
            var settings = new DashboardKhoTimingSettings
            {
                TvPage1To2 = ReadSubmittedSeconds("TvPage1To2", TvPage1To2, 60),
                TvPage2To1 = ReadSubmittedSeconds("TvPage2To1", TvPage2To1, 60),
                Page1Customer = ReadSubmittedSeconds("Page1Customer", Page1Customer, 60),
                Page1Receiving = ReadSubmittedSeconds("Page1Receiving", Page1Receiving, 60),
                Page2Shipping = ReadSubmittedSeconds("Page2Shipping", Page2Shipping, 60),
                Page2Ready = ReadSubmittedSeconds("Page2Ready", Page2Ready, 60)
            };

            Session[DashboardKhoSettingsSessionKey] = settings;
            SaveDashboardKhoSettings(settings);
            return RedirectToAction("DashboardKho");
        }

        private DashboardKhoTimingSettings GetDashboardKhoSettings()
        {
            var sessionSettings = Session[DashboardKhoSettingsSessionKey] as DashboardKhoTimingSettings;
            if (sessionSettings != null)
            {
                return sessionSettings;
            }

            var cookie = Request.Cookies[DashboardKhoSettingsCookieName];
            if (cookie != null)
            {
                return new DashboardKhoTimingSettings
                {
                    TvPage1To2 = ReadCookieInt(cookie, "TvPage1To2", 60),
                    TvPage2To1 = ReadCookieInt(cookie, "TvPage2To1", 60),
                    Page1Customer = ReadCookieInt(cookie, "Page1Customer", 60),
                    Page1Receiving = ReadCookieInt(cookie, "Page1Receiving", 60),
                    Page2Shipping = ReadCookieInt(cookie, "Page2Shipping", 60),
                    Page2Ready = ReadCookieInt(cookie, "Page2Ready", 60)
                };
            }

            return new DashboardKhoTimingSettings();
        }

        private void SaveDashboardKhoSettings(DashboardKhoTimingSettings settings)
        {
            var cookie = new HttpCookie(DashboardKhoSettingsCookieName)
            {
                Expires = DateTime.Now.AddYears(1),
                Path = "/"
            };

            cookie.Values["TvPage1To2"] = settings.TvPage1To2.ToString();
            cookie.Values["TvPage2To1"] = settings.TvPage2To1.ToString();
            cookie.Values["Page1Customer"] = settings.Page1Customer.ToString();
            cookie.Values["Page1Receiving"] = settings.Page1Receiving.ToString();
            cookie.Values["Page2Shipping"] = settings.Page2Shipping.ToString();
            cookie.Values["Page2Ready"] = settings.Page2Ready.ToString();

            Response.Cookies.Set(cookie);
        }

        private void PopulateSettingsViewBag(DashboardKhoTimingSettings settings)
        {
            SetTimeViewBag("TvPage1To2", settings.TvPage1To2);
            SetTimeViewBag("TvPage2To1", settings.TvPage2To1);
            SetTimeViewBag("Page1Customer", settings.Page1Customer);
            SetTimeViewBag("Page1Receiving", settings.Page1Receiving);
            SetTimeViewBag("Page2Shipping", settings.Page2Shipping);
            SetTimeViewBag("Page2Ready", settings.Page2Ready);
        }

        private void SetTimeViewBag(string prefix, int totalSeconds)
        {
            var safeSeconds = NormalizeSeconds(totalSeconds, 60);
            ViewData[prefix + "Minutes"] = safeSeconds / 60;
            ViewData[prefix + "Seconds"] = safeSeconds % 60;
        }

        private static int ReadCookieInt(HttpCookie cookie, string key, int fallback)
        {
            if (cookie == null) return fallback;

            int parsed;
            return int.TryParse(cookie.Values[key], out parsed) && parsed > 0
                ? parsed
                : fallback;
        }

        private static int NormalizeSeconds(int value, int fallback)
        {
            if (value < 1) return fallback;
            if (value > 10800) return 10800;
            return value;
        }

        private static int BuildTotalSeconds(int? minutes, int? seconds, int fallback)
        {
            var safeMinutes = Math.Max(0, minutes ?? 0);
            var safeSeconds = Math.Max(0, seconds ?? 0);
            var totalSeconds = (safeMinutes * 60) + safeSeconds;
            return NormalizeSeconds(totalSeconds, fallback);
        }

        private int ReadSubmittedSeconds(string prefix, int? submittedValue, int fallback)
        {
            var minutes = ReadSubmittedNullableInt(prefix + "Minutes");
            var seconds = ReadSubmittedNullableInt(prefix + "Seconds");

            if (minutes.HasValue || seconds.HasValue)
            {
                return BuildTotalSeconds(minutes, seconds, fallback);
            }

            return NormalizeSeconds(submittedValue ?? fallback, fallback);
        }

        private int? ReadSubmittedNullableInt(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            var rawValue = Request.Form[key];

            int parsedValue;
            return int.TryParse(rawValue, out parsedValue) ? parsedValue : (int?)null;
        }
    }
}
