using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.Json;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace QLBQA.Extension
{
    public static class SessionExtensions
    {
        //public static void Set<T>(this ISession session, string key, T value)
        //{
        //    session.SetString(key, JsonSerializer.Serialize(value));
        //}

        //public static T? Get<T>(this ISession session, string key)
        //{
        //    var value = session.GetString(key);
        //    return value == null ? default : JsonSerializer.Deserialize<T>(value);
        //}

        //public static void Set<T>(this ISession session, string key, T value)
        //{
        //    var jsonString = System.Text.Json.JsonSerializer.Serialize(value);
        //    var utf8Bytes = Encoding.UTF8.GetBytes(jsonString);
        //    session.Set(key, utf8Bytes);
        //}
        //public static void Set<T>(this HttpSessionStateBase session, string key, T value)
        //{
        //    var jsonString = JsonConvert.SerializeObject(value);
        //    var utf8Bytes = Encoding.UTF8.GetBytes(jsonString);
        //    session[key] = utf8Bytes;
        //}
        public static void Set(this HttpSessionStateBase session, string key, string value)
        {
            session[key] = value;
        }

        public static T Get<T>(this HttpSessionStateBase session, string key)
        {
            var utf8Bytes = session[key] as byte[];
            if (utf8Bytes == null)
                return default(T);

            var jsonString = Encoding.UTF8.GetString(utf8Bytes);
            return JsonConvert.DeserializeObject<T>(jsonString);
        }







    }
}