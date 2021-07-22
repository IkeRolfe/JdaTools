using JdaTools.Connection.Attributes;

namespace JdaTools.Studio.Models
{
    public class ColumnDefintion
    {
        [MocaColumn("table_name")]
        public string TableName { get; set; }
        [MocaColumn("column_name")]
        public string ColumnName { get; set; }
        //[MocaColumn("dscr")]
       // public string Description { get; set; }
        [MocaColumn("data_type")]
        public string DataType { get; set; }
        [MocaColumn("null_flg")]
        public bool Nullable { get; set; }
        [MocaColumn("pk_flg")]
        public bool PrimaryKey { get; set; }
        //[MocaColumn("ident_flg")]
        //public bool Identity { get; set; }
        public override string ToString()
        {
            return $"{(PrimaryKey ? "*" : " ")}{ColumnName}({DataType})";
        }
    }
}