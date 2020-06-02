using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace locuste.dashboard.deploy.uwp.Models
{
    public class FileCopyInfo
    {

        [JsonProperty("current_file")]
        public string CurrentFile;

        [JsonProperty("file_count")]
        public int FileCount;

        [JsonProperty("file_index")]
        public int FileIndex;
    }
}
