using System.Diagnostics;

namespace Lyricify.Helpers.General
{
    public static class GeneralHelper
    {
        public static void ProcessStartUrl(string url)
        {
            var pInfo = new ProcessStartInfo($"cmd", $"/c start {url.Replace("&", "^&")}")
            {
                CreateNoWindow = true
            };
            Process.Start(pInfo);
        }

        public static void ProcessOpenFile(string path)
        {
            var pInfo = new ProcessStartInfo($"cmd", $"/c start \"\" \"{path}\"")
            {
                CreateNoWindow = true
            };
            Process.Start(pInfo);
        }
    }
}
