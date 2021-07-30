using JdaTools.Connection;
using JdaTools.Studio.Models;
using System;
using System.Collections.Generic;
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

    }
}
