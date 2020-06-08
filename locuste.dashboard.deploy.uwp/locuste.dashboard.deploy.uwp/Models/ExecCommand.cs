using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace locuste.dashboard.deploy.uwp.Models
{
    public enum CommandType  {
        Start,
        Stop
    }

    public class ExecCommand
    {

        [System.Runtime.Serialization.DataMember(Name = "application")]
        [JsonProperty("application")]
        public string Application;

        [System.Runtime.Serialization.DataMember(Name = "version")]
        [JsonProperty("version")]
        public string Version;

        [System.Runtime.Serialization.DataMember(Name = "command")]
        [JsonProperty("command")]
        public CommandType Command;
    }
}
