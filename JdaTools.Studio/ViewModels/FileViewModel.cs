using JdaTools.Studio.Models;

namespace JdaTools.Studio.ViewModels
{
    public class FileViewModel : ViewModelBase
    {
        public FileViewModel(MocaFile mocaFile)
        {
            Name = mocaFile.FileName;
            Description = mocaFile.Description;
            Path = mocaFile.PathName;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
    }
}