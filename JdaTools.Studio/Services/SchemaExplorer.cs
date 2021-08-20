using JdaTools.Connection;
using JdaTools.Studio.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdaTools.Studio.Services
{
    internal class SchemaExplorer
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
            var tables = await _mocaClient.ExecuteQuery<TableDefinition>("list user tables;");
            
            var columns = await _mocaClient.ExecuteQuery<ColumnDefintion>("list tables with column");
            foreach (var table in tables)
            {
                IEnumerable<ColumnDefintion> tableColumns = columns.Where(c => c.TableName.ToUpper() == table.TableName.ToUpper());
                table.Columns = tableColumns;
            }
            Tables = tables;
        }
        public async Task RefreshCommands()
        {
            var commands = await _mocaClient.ExecuteQuery<CommandDefinition>("list active commands;");
                       
            Commands = commands.OrderBy(c=>c.CommandName);
        }

        public async Task RefreshFiles()
        {
            var response = await _mocaClient.ExecuteQuery("find file where pathname = @@LESDIR || '/src/cmdsrc/*' | { if ( @type = 'D' ) find file where pathname = @pathname || '/*.m*' }");
            var dt = response.MocaResults.GetDataTable();
            var files = dt.AsEnumerable().Select(x => x["PATHNAME"].ToString()).ToList();

            Files = files.OrderBy(x=>x);
        }

        public async Task<IEnumerable<MocaFile>> GetDirectory(string path = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                var files = await _mocaClient.ExecuteQuery<MocaFile>("find file where pathname = @@LESDIR || '/src/cmdsrc/*'");
                return files;
            }
            else
            {
                var files = new List<MocaFile>();
                var previousDir = new MocaFile()
                {
                    FileName = "..",
                    PathName = path.Substring(0, path.LastIndexOf('\\')),
                    Type = "D"
                };
                files.Add(previousDir);
                var dirFiles = await _mocaClient.ExecuteQuery<MocaFile>("find file where pathname = @path || '/*'", new {path});
                files.AddRange(dirFiles);
                return files;
            }
        }

    }
}
