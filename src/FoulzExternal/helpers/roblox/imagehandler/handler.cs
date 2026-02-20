using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FoulzExternal.storage;
using FoulzExternal.logging;

// just to make the external look tuff :)

namespace FoulzExternal.helpers.roblox.imagehandler
{
    internal static class handler
    {
        private static readonly HttpClient _http = new HttpClient();
        public static async Task<string> GetAvatarHeadshotUrlAsync(long userId = 0)
        {
            try
            {
                long id = userId != 0 ? userId : Storage.LocalPlayerUserId;
                if (id == 0) return string.Empty;

                string url = $"https://thumbnails.roblox.com/v1/users/avatar-headshot?userIds={id}&size=420x420&format=Png&isCircular=false";

                var res = await _http.GetStringAsync(url).ConfigureAwait(false);
                if (string.IsNullOrEmpty(res)) return string.Empty;

                var m = Regex.Match(res, @"""imageUrl""\s*:\s*""(?<u>https?:\/\/[^""]+)""");
                if (m.Success)
                {
                    string raw = m.Groups["u"].Value;
                    string img = raw.Replace("\\/", "/");
                    return img;
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                LogsWindow.Log("[ImageHandler] Exception: {0}", ex.Message);
                return string.Empty;
            }
        }
    }
}

