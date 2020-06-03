using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace locuste.dashboard.deploy.uwp.Utils
{
    public static class JsonParser
    {
    
            public static T ToObject<T>(this string jsonText)
            {
                return JsonConvert.DeserializeObject<T>(jsonText);
            }

            public static string ToJson<T>(this T obj)
            {
                return JsonConvert.SerializeObject(obj);
            }
        
    }
}
