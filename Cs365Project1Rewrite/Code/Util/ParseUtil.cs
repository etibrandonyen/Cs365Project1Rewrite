using System;

namespace Cs365Project1Rewrite.Code.Util
{
    public static class ParseUtil
    {
        public static int? ParseOrNull(this string s) => int.TryParse(s, out int result) ? result : null;

        #nullable enable
        public static int? ParseOrNull(this string s, System.Globalization.NumberStyles style, IFormatProvider? provider)
            => int.TryParse(s, style, provider, out var result) ? result : null;
    }
}
