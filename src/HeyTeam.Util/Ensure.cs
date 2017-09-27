using System;

namespace HeyTeam.Util {
    public static class Ensure {
        public static void ArgumentNotNull(object argument) {
            if(argument==null) throw new ArgumentNullException();
        }

        public static void NotNull<T, E>(T t) where E:Exception, new() {
            if(t == null)
                throw new E();
        }
    }
}