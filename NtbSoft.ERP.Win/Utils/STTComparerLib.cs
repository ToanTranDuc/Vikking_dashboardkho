using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Win.Utils
{
    public class STTComparerLib : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            var xParts = x.Split('.');
            var yParts = y.Split('.');

            int len = Math.Max(xParts.Length, yParts.Length);

            for (int i = 0; i < len; i++)
            {
                string xi = i < xParts.Length ? xParts[i] : "0";
                string yi = i < yParts.Length ? yParts[i] : "0";

                var xNumber = ExtractNumericPart(xi);
                var yNumber = ExtractNumericPart(yi);

                int cmp = xNumber.CompareTo(yNumber);
                if (cmp != 0) return cmp;

                var xAlpha = ExtractAlphaPart(xi);
                var yAlpha = ExtractAlphaPart(yi);

                cmp = string.Compare(xAlpha, yAlpha, StringComparison.OrdinalIgnoreCase);
                if (cmp != 0) return cmp;
            }

            return 0;
        }

        private int ExtractNumericPart(string part)
        {
            var num = new string(part.TakeWhile(char.IsDigit).ToArray());
            return int.TryParse(num, out int result) ? result : 0;
        }

        private string ExtractAlphaPart(string part)
        {
            return new string(part.SkipWhile(char.IsDigit).ToArray());
        }
    }
}
