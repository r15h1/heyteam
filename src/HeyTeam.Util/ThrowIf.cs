using System;

namespace HeyTeam.Util {
    public static class ThrowIf {
        public static void ArgumentIsNull(object argument) {
            if(argument==null) throw new ArgumentNullException();
        }

		public static void Null(object subject, Exception ex) {
			if (subject == null)
				throw ex ?? new Exception("Subject cannot be null");
		}
	}
}