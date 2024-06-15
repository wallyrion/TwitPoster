using System.Collections;
using System.Collections.Frozen;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace TwitPoster.Web.Common;

internal static class WebConstants
{
    public static readonly FrozenSet<string> ValidProfileImageExtensions = new HashSet<string>(
    [
        ".jpg",
        ".jpeg",
        ".png",
        ".bmp"
    ]).ToFrozenSet(new CaseInsensitiveValueComparer());

    internal static class Cors
    {
        public const string DefaultPolicy = "DefaultPolicy";
    }
}