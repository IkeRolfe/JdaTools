using System;

namespace JdaTools.Connection.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MocaColumnAttribute : Attribute
    {
        public string ColumnName { get; set; }
        public MocaColumnAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }
}