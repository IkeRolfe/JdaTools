using JdaTools.Connection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdaTools.Studio.Models
{
    public class TableDefinition
    {
        private IEnumerable<ColumnDefintion> _columns;

        [MocaColumn("table_name")]
        public string TableName { get; set; }
        public IEnumerable<ColumnDefintion> Columns { 
            get => _columns ?? new[] { new ColumnDefintion { TableName = "loading..." } }; 
            set => _columns = value; 
        }
        public override string ToString()
        {
            return TableName;
        }
    }
}
