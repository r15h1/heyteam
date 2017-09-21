using System;

namespace HeyTeam.Lib.Util {
    public static class Extensions {
        public static bool IsValidUrl(this string url) {
            return Uri.IsWellFormedUriString(url, UriKind.Absolute);
        }
    }
}