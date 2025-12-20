using System;

namespace MusicClicker.Helpers
{
    /// <summary>
    /// Utility class for formatting large numbers with abbreviated suffixes.
    /// </summary>
    public static class NumberFormatter
    {
        /// <summary>
        /// Formats large numbers with abbreviated suffixes for readability.
        /// E.g., 1,500,000 becomes "1.50M"
        /// Supports up to Centillion (10^303)
        /// </summary>
        public static string FormatLargeNumber(double num)
        {
            if (double.IsInfinity(num) || double.IsNaN(num))
                return "∞";

            if (num >= 1e303)
                return $"{num / 1e303:F2}Ct";  // Centillion
            if (num >= 1e300)
                return $"{num / 1e300:F2}Uc";  // Uncentillion
            if (num >= 1e270)
                return $"{num / 1e270:F2}No";  // Nonillion
            if (num >= 1e240)
                return $"{num / 1e240:F2}Og";  // Octogintillion
            if (num >= 1e210)
                return $"{num / 1e210:F2}Sg";  // Septuagintillion
            if (num >= 1e180)
                return $"{num / 1e180:F2}Vg";  // Vigintillion
            if (num >= 1e150)
                return $"{num / 1e150:F2}Qg";  // Quinquagintillion
            if (num >= 1e120)
                return $"{num / 1e120:F2}Tt";  // Trigintillion
            if (num >= 1e90)
                return $"{num / 1e90:F2}Dg";   // Decillion
            if (num >= 1e60)
                return $"{num / 1e60:F2}Vl";   // Vigintillion
            if (num >= 1e54)
                return $"{num / 1e54:F2}Nl";   // Novendecillion
            if (num >= 1e51)
                return $"{num / 1e51:F2}Sd";   // Sexdecillion
            if (num >= 1e48)
                return $"{num / 1e48:F2}Qd";   // Quindecillion
            if (num >= 1e45)
                return $"{num / 1e45:F2}Qt";   // Quattuordecillion
            if (num >= 1e42)
                return $"{num / 1e42:F2}Td";   // Tredecillion
            if (num >= 1e39)
                return $"{num / 1e39:F2}Dd";   // Duodecillion
            if (num >= 1e36)
                return $"{num / 1e36:F2}Ud";   // Undecillion
            if (num >= 1e33)
                return $"{num / 1e33:F2}Dc";   // Decillion
            if (num >= 1e30)
                return $"{num / 1e30:F2}No";   // Nonillion
            if (num >= 1e27)
                return $"{num / 1e27:F2}Oc";   // Octillion
            if (num >= 1e24)
                return $"{num / 1e24:F2}Sp";   // Septillion
            if (num >= 1e21)
                return $"{num / 1e21:F2}Sx";   // Sextillion
            if (num >= 1e18)
                return $"{num / 1e18:F2}Qn";   // Quintillion
            if (num >= 1e15)
                return $"{num / 1e15:F2}Qd";   // Quadrillion
            if (num >= 1e12)
                return $"{num / 1e12:F2}T";    // Trillion
            if (num >= 1e9)
                return $"{num / 1e9:F2}B";     // Billion
            if (num >= 1e6)
                return $"{num / 1e6:F2}M";     // Million
            if (num >= 1e3)
                return $"{num / 1e3:F2}K";     // Thousand
            return $"{Math.Round(num, 1)}";     // Less than 1000, show actual number
        }
    }
}
