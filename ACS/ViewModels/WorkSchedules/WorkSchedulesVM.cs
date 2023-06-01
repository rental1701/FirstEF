using ACS.Model.WorkSchedules;
using ACS.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACS.ViewModels.WorkSchedules
{
    public class WorkSchedulesVM : ViewModel
    {
        private readonly DirectoryInfo _DirectoryInfo;
        public WorkSchedulesVM(string Path)
        {
            _DirectoryInfo = new DirectoryInfo(Path);
           
        }

        public СheckpointVM Checkpoint { get; set; }
        private Uri _UriPath;

        public Uri UriPath
        {
            get => _UriPath;
            set => Set(ref _UriPath, value);
        }


        private FileVM? _SelectedFile;

        public FileVM? SelectedFile
        {
            get { return _SelectedFile; }
            set
            {
                if (Set(ref _SelectedFile, value) && SelectedFile is FileVM file)
                {
                    UriPath = new(file.Path);
                    Checkpoint.SelectedPdfFile = new(file.Path);
                };
            }
        }

        private bool _IsSelected;

        public bool IsSelected
        {
            get { return _IsSelected; }
            set => Set(ref _IsSelected, value);
        }

        public IEnumerable<WorkSchedulesVM> SubDirectories
        {
            get
            {
                return _DirectoryInfo.EnumerateDirectories().Select(dir => new WorkSchedulesVM(dir.FullName) { Checkpoint = this.Checkpoint });
            }
        }

        public IEnumerable<FileVM> Files
        {
            get
            {
                return _DirectoryInfo.EnumerateFiles().Select(f => new FileVM(f.FullName, this));
            }
        }

        public IEnumerable<object> DirectoryItems
        {
            get
            {
                return SubDirectories.Cast<object>().Concat(Files);
            }
        }

        public string Name => _DirectoryInfo.Name;

        public string Path => _DirectoryInfo.FullName;
    }
}
