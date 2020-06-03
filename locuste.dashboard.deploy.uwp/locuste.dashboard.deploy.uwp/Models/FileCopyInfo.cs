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
        [System.Runtime.Serialization.DataMember(Name = "current_file")]
        [JsonProperty("current_file")]
        public string CurrentFile;

        [System.Runtime.Serialization.DataMember(Name = "file_count")]
        [JsonProperty("file_count")]
        public int FileCount;

        [System.Runtime.Serialization.DataMember(Name = "file_index")]
        [JsonProperty("file_index")]
        public int FileIndex;
    }

    public class FileCopyInfoArgs : EventArgs
    {

        public string CurrentFile;
        public int FileCount;
        public int FileIndex;

        public FileCopyInfoArgs(FileCopyInfo original)
        {
            CurrentFile = original.CurrentFile;
            FileCount = original.FileCount;
            FileIndex = original.FileIndex;
        }
    }
}
