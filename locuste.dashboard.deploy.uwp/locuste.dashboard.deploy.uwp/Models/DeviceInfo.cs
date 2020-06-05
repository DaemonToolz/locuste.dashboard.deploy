﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace locuste.dashboard.deploy.uwp.Models
{
    public class DeviceInfo
    {
        public string Name;
        public string IPAddress;
        public int Port;
        public string Version;
    }

    public class ProjectVersion
    {
        [System.Runtime.Serialization.DataMember(Name = "global_version")]
        [JsonProperty("global_version")]
        public string GlobalVersion;

        [System.Runtime.Serialization.DataMember(Name = "detailed_version")]
        [JsonProperty("detailed_version")]
        public List<AppVersion> DetailedVersions;

    }

    public class AppVersion
    {

        [System.Runtime.Serialization.DataMember(Name = "app_name")]
        [JsonProperty("app_name")]
        public string Name;

        [System.Runtime.Serialization.DataMember(Name = "version")]
        [JsonProperty("version")]
        public string Version;

    }
}