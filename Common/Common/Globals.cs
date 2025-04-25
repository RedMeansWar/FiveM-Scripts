using System.Collections.Generic;

namespace Common
{
    public class Globals
    {
        public static readonly IReadOnlyList<string> ACCEPT_WORDS = new List<string>
        {
            "yes", "y", "1", "true", "t", "accept", "a", "yep"
        };

        public static readonly IReadOnlyList<string> DENY_WORDS = new List<string>
        {
            "no", "n", "0", "false", "f", "deny", "d", "nope"
        };

        public const int BC_MIN_Y = 680;
        public const int LS_MAX_Y = 1950;
    }
}