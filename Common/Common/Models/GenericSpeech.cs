using System;

namespace Common.Models
{
    public struct GenericSpeech
    {
        // pastebin.com/1GZS5dCL
        public static string HI { get { return PickRandomFrom("GENERIC_HI", "GENERIC_HOWS_IT_GOING"); } }
        public static string BYE { get { return "GENERIC_BYE"; } }

        public static string YES { get { return "GENERIC_YES"; } }
        public static string NO { get { return "GENERIC_NO"; } }

        public static string SUPRISED { get { return PickRandomFrom("GENERIC_INSULT_MED", "GENERIC_SHOCKED_MED"); } }
        public static string VERY_SUPRISED { get { return PickRandomFrom("GENERIC_INSULT_HIGH", "GENERIC_FUCK_YOU", "GENERIC_SHOCKED_HIGH"); } }

        public static string WHATEVER { get { return "GENERIC_WHATEVER"; } }

        public static string CHEER { get { return "GENERIC_CHEER"; } }

        public static string THANKS { get { return "GENERIC_THANKS"; } }

        public static string SORRY { get { return "APOLOGY_NO_TROUBLE"; } }

        public static string RESPONSE { get { return "CHAT_RESP"; } }

        #region Helper Functions
        internal static string PickRandomFrom(params string[] options) => options[new Random().Next(options.Length)];
        #endregion
    }
}