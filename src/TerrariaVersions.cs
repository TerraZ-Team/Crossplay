using System.Collections.Generic;

namespace Crossplay
{
    public static class TerrariaVersions
    {
        public const int LAST_VERSION_NUM = 279;
        public const int VERSION_STRING_LEN = 11;
        public static string LastVersion => SupportedVersions[LAST_VERSION_NUM];

        public static readonly IReadOnlyDictionary<int, string> SupportedVersions = new Dictionary<int, string>()
        {
            { 269, "v1.4.4" },
            { 270, "v1.4.4.1" },
            { 271, "v1.4.4.2" },
            { 272, "v1.4.4.3" },
            { 273, "v1.4.4.4" },
            { 274, "v1.4.4.5" },
            { 275, "v1.4.4.6" },
            { 276, "v1.4.4.7" },
            { 277, "v1.4.4.8" },
            { 278, "v1.4.4.8.1" },
            { 279, "v1.4.4.9" },
        };

        public static readonly IReadOnlyDictionary<int, int> MaxItems = new Dictionary<int, int>()
        {
            { 269, 5453 },
            { 270, 5453 },
            { 271, 5453 },
            { 272, 5453 },
            { 273, 5453 },
            { 274, 5456 },
            { 275, 5456 },
            { 276, 5456 },
            { 277, 5456 },
            { 278, 5456 },
            { 279, 5456 },
        };
    }
}
