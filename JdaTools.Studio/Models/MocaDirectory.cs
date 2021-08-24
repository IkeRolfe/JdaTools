using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JdaTools.Studio.Models
{
    public class MocaDirectory : IMocaFile
    {
        public string Type { get; set; }
        public string PathName { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }

        public List<IMocaFile> Files
        {
            get
            {
                if (_files == null)
                {
                    RefreshFiles().ConfigureAwait(false);
                }
                return _files; 

            }
        }

        public delegate Task<IEnumerable<IMocaFile>> RefreshFilesDelegate();
        private readonly RefreshFilesDelegate _refreshFilesHandler;
        private List<IMocaFile> _files;

        public MocaDirectory(MocaFile mocaFile, RefreshFilesDelegate refreshFilesDelegate)
        {
            Type = "D";
            PathName = mocaFile.PathName;
            FileName = mocaFile.FileName;
            _refreshFilesHandler = refreshFilesDelegate;
        }

        public async Task<List<IMocaFile>> RefreshFiles()
        {
            if (_refreshFilesHandler == null)
            {
                return null;
            }
            var files = await _refreshFilesHandler.Invoke();
            _files = files.ToList();
            return _files;
        }

        public override string ToString()
        {
            return FileName;
        }
    }

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
}