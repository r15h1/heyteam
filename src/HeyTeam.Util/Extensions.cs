using System;

namespace HeyTeam.Util {
    public static class Extensions {
        public static bool IsValidUrl(this string url) {
            return Uri.IsWellFormedUriString(url, UriKind.Absolute);
        }

        public static bool IsEmpty(this Guid guid) {
            return guid == Guid.Empty;
        }
    }
}