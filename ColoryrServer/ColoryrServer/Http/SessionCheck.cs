using System;
using System.Collections.Generic;

namespace ColoryrServer.Http
{
    class SessionCheck
    {
        /// <summary>
        /// 会话的缓存数据
        /// </summary>
        private static Dictionary<string, Dictionary<string, dynamic>> SessionCache = new Dictionary<string, Dictionary<string, dynamic>>();

        internal static dynamic GetCache(string Cookie, string key)
        {
            SessionCache.TryGetValue(Cookie, out var temp);
            if (temp == null)
                return null;
            temp.TryGetValue(key, out var temp1);
            return temp1;
        }
        internal static bool HaveKey(string Cookie, string Key)
        {
            if (!HaveCookie(Cookie))
                return false;
            return SessionCache[Cookie].ContainsKey(Key);
        }
        internal static string GetCookie(string Cookie)
        {
            if (!string.IsNullOrWhiteSpace(Cookie) && SessionCache.ContainsKey(Cookie))
                return Cookie;
            else
            {
                var stringCookie = Guid.NewGuid().ToString();
                while (SessionCache.ContainsKey(stringCookie))
                {
                    stringCookie = Guid.NewGuid().ToString();
                }
                return stringCookie;
            }
        }
        internal static void SetCache(string Cookie, string key, dynamic value)
        {
            if (!SessionCache.ContainsKey(Cookie))
                SessionCache.Add(Cookie, new Dictionary<string, dynamic>
                {
                    { key, value }
                });
            if (!SessionCache[Cookie].ContainsKey(key))
                SessionCache[Cookie].Add(key, value);
            SessionCache[Cookie][key] = value;
        }
        internal static void SessionClose(string Cookie)
        {
            if (!SessionCache.ContainsKey(Cookie))
                return;
            SessionCache.Remove(Cookie);
        }

        internal static bool HaveCookie(string Cookie)
        {
            return SessionCache.ContainsKey(Cookie);
        }
    }
}
