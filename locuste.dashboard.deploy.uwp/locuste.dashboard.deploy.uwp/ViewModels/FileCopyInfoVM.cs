using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using locuste.dashboard.deploy.uwp.Models;

namespace locuste.dashboard.deploy.uwp.ViewModels
{
    public class FileCopyInfoVM : Bindable  {

        private FileCopyInfo _info;

        public FileCopyInfo Info { get => _info; set => SetField(ref _info, value); }

        public FileCopyInfoVM() { }
        public FileCopyInfoVM(FileCopyInfo info)
        {
            Info = info;
        }

        public FileCopyInfoVM(FileCopyInfoArgs info)
        {
            Info = new FileCopyInfo()
            {
                CurrentFile = info.CurrentFile,
                FileCount = info.FileCount,
                FileIndex = info.FileIndex
            };
        }
    }
}
