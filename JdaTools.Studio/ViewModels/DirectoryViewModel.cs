using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using JdaTools.Studio.Models;

namespace JdaTools.Studio.ViewModels
{
    public class DirectoryViewModel : ViewModelBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public ObservableCollection<object> Files { get; set; }

        public DirectoryViewModel(MocaDirectory mocaDirectory)
        {
            Name = mocaDirectory.FileName;
            Description = mocaDirectory.Description;
            Path = mocaDirectory.PathName;
            GetFilesHandler = mocaDirectory.RefreshFiles;
            GetFiles();
        }

        private async void GetFiles()
        {
            if (Files != default)
            {
                //TODO: refresh
                return;
            }
            Files = new ObservableCollection<object>();
            var ifiles = await GetFilesHandler.Invoke();
            foreach (var mocaFile in ifiles)
            {
                if (mocaFile.GetType() == typeof(MocaDirectory))
                {
                    Files.Add(new DirectoryViewModel((MocaDirectory)mocaFile));
                }
                else
                {
                    Files.Add(new FileViewModel((MocaFile)mocaFile));
                }
            }
        }

        private delegate Task<List<IMocaFile>> GetFilesDelegate();
        private GetFilesDelegate GetFilesHandler;
    }
}