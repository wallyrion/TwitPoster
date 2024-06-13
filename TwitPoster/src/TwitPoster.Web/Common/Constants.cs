using System.Collections.Frozen;

namespace TwitPoster.Web.Common;

internal static class WebConstants
{
    public static readonly FrozenSet<string> ValidProfileImageExtensions = new HashSet<string>(
    [
        ".jpg",
        ".jpeg",
        ".bmp"
    ]).ToFrozenSet();

    internal static class Cors
    {
        public const string DefaultPolicy = "DefaultPolicy";
    }
}