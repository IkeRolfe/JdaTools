using JdaTools.Connection;
using JdaTools.Studio.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Caliburn.Micro;
using JdaTools.Studio.AvalonEdit;

namespace JdaTools.Studio.Services
{
    public class SchemaExplorer
    {
        private MocaClient _mocaClient;
        private IEnumerable<TableDefinition> _tables;

        public SchemaExplorer(MocaClient mocaClient)
        {
            _mocaClient = mocaClient;
        }
        private IEnumerable<CommandDefinition> _commands;
        private IEnumerable<string> _files;

        public IEnumerable<CommandDefinition> Commands
        {
            get
            {
                //TODO Disabled till we have good IsConnected check for moca client
                /*if (_tables == null)
                {
                    RefreshTables().RunSynchronously();
                }*/
                return _commands;
            }
            private set => _commands = value;
        }

        public IEnumerable<TableDefinition> Tables
        {
            get
            {
                //TODO Disabled till we have good IsConnected check for moca client
                /*if (_tables == null)
                {
                    RefreshTables().RunSynchronously();
                }*/
                return _tables;
            }
            private set => _tables = value;
        }

        public IEnumerable<string> Files
        {
            get
            {
                //TODO Disabled till we have good IsConnected check for moca client
                /*if (_tables == null)
                {
                    RefreshTables().RunSynchronously();
                }*/
                return _files;
            }
            private set => _files = value;
        }

        public async Task RefreshTables()
        {
            var tables = await _mocaClient.ExecuteQueryAsync<TableDefinition>("list user tables;");
            
            var columns = await _mocaClient.ExecuteQueryAsync<ColumnDefintion>("list tables with column");
            foreach (var table in tables)
            {
                IEnumerable<ColumnDefintion> tableColumns = columns.Where(c => c.TableName.ToUpper() == table.TableName.ToUpper());
                table.Columns = tableColumns;
            }
            Tables = tables;
        }
        public async Task RefreshCommands()
        {
            var commands = await _mocaClient.ExecuteQueryAsync<CommandDefinition>("list active commands;");
            if (commands != null)
            {
                foreach (var highlightingDef in IoC.GetAllInstances(typeof(MocaHighlightingDefinition)))
                {
                    (highlightingDef as MocaHighlightingDefinition)?.SetCommands(
                        commands.Select(c => c.CommandName));
                }
                Commands = commands.OrderBy(c=>c.CommandName);
            }
        }

        public async Task RefreshFiles()
        {
            var response = await _mocaClient.ExecuteQueryAsync("find file where pathname = @@LESDIR || '/src/cmdsrc/*' | { if ( @type = 'D' ) find file where pathname = @pathname || '/*.m*' }");
            var dt = response.MocaResults.GetDataTable();
            var files = dt.AsEnumerable().Select(x => x["PATHNAME"].ToString()).ToList();

            Files = files.OrderBy(x=>x);
        }

        public async Task<IEnumerable<IMocaFile>> GetDirectory(string path = null)
        {
            List<IMocaFile> returnFiles = new List<IMocaFile>();
            List<MocaFile> filesRaw;
            
            if (string.IsNullOrEmpty(path))
            {
                filesRaw = (await _mocaClient.ExecuteQueryAsync<MocaFile>("find file where pathname = @@LESDIR || '/src/cmdsrc/*'")).ToList();
            }
            else
            {
                filesRaw = new List<MocaFile>();
                var previousDir = new MocaFile()
                {
                    FileName = "..",
                    PathName = path.Substring(0, path.LastIndexOf('\\')),
                    Type = "D"
                };
                filesRaw.Add(previousDir);
                var dirFiles = await _mocaClient.ExecuteQueryAsync<MocaFile>("find file where pathname = @path || '/*'", new {path});
                filesRaw.AddRange(dirFiles);
            }

            foreach (var mocaFile in filesRaw)
            {
                var componentLevelDeserializer = new XmlSerializer(typeof(ComponentLevel));
                if (mocaFile.Type == "D")
                {
                    //System files should associated definition file with file description in XML format
                    var definitionFile = filesRaw.FirstOrDefault(f => f.FileName.Equals(mocaFile.FileName + ".mlvl", StringComparison.InvariantCultureIgnoreCase));
                    ComponentLevel componentLevel = null;
                    if (definitionFile != null)
                    {
                        string definitionXml;
                        definitionXml = await GetFileContent(definitionFile);
                        using var stream = new StringReader(definitionXml);
                        componentLevel = (ComponentLevel) componentLevelDeserializer.Deserialize(stream);
                    }
                    //TODO:Attach definition
                    returnFiles.Add(new MocaDirectory(mocaFile, () => GetDirectory(mocaFile.PathName))
                    {
                        Description = componentLevel?.Description
                    });
                }
                else
                {
                    //Skip directory definitions
                    if (mocaFile.FileName.EndsWith(".mlvl", StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }
                    returnFiles.Add(mocaFile);
                }
            }

            return returnFiles;
        }

        public async Task<string> GetFileContent(MocaFile file)
        {
            var response = await _mocaClient.ExecuteQueryAsync("download file where filename = @filePath", new { filePath = file.PathName });
            var content = response.MocaResults.GetDataTable().Rows[0]["DATA"].ToString();
            var text = Encoding.UTF8.GetString(Convert.FromBase64String(content));
            return text;
        }
    }
}
