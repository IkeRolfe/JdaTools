using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;
using System.Linq;

namespace JdaTools.Connection
{
	[XmlRoot(ElementName = "column")]
    public class Column
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "length")]
        public string Length { get; set; }
        [XmlAttribute(AttributeName = "nullable")]
        public bool Nullable { get; set; }
    }

    [XmlRoot(ElementName = "metadata")]
    public class Metadata
    {
        [XmlElement(ElementName = "column")]
        public List<Column> Column { get; set; }
    }

    [XmlRoot(ElementName = "field")]
    public class Field
    {
        private string _value;

        [XmlAttribute(AttributeName = "null")]
        public bool Null { get; set; }
        [XmlTextAttribute()]
        public string Value
        {
            get => _value;
            set => _value = value;
        }

    }

    [XmlRoot(ElementName = "row")]
    public class Row
    {
        [XmlElement(ElementName = "field")]
        public List<Field> Field { get; set; }
    }

    [XmlRoot(ElementName = "data")]
    public class Data
    {
        [XmlElement(ElementName = "row")]
        public List<Row> Row { get; set; }
    }

    [XmlRoot(ElementName = "moca-results")]
    public class MocaResult
    {
        [XmlElement(ElementName = "metadata")]
        public Metadata Metadata { get; set; }
        [XmlElement(ElementName = "data")]
        public Data Data { get; set; }

        public DataTable GetDataTable()
        {
            var columnDefs = Metadata.Column;
            var dataTable = new DataTable("data");
            var types = columnDefs.Select(GetColumnType).ToArray();

            for (var index = 0; index < columnDefs.Count; index++)
            {
                var name = columnDefs[index].Name;
                var type = types[index];
                dataTable.Columns.Add(name, type);
            }

            foreach (var row in Data.Row)
            {
                var fields = new object[row.Field.Count];
                for (var index = 0; index < row.Field.Count; index++)
                {
                    var field = row.Field[index].Value;
                    var type = types[index];

                    if (field == null) continue;

                    //sql bool comes as bit
                    if (type == typeof(bool))
                    {
                        field = field == "1" ? true.ToString() : false.ToString();
                    }

                    fields[index] = Convert.ChangeType(field,type);
                }

                dataTable.Rows.Add(fields);

            }

            return dataTable;
        } 
        private Type GetColumnType(Column columnDef)
        {
            var nullable = columnDef.Nullable;
            var columnType = columnDef.Type.ToUpper() switch
            {
                "S" => typeof(string),
                //"I" => nullable ? typeof(int) : typeof(int?),
                "I" => typeof(int),
                //"D" => typeof(DateTime),
                "O" => typeof(bool),
                _ => typeof(string)
            };
            return columnType;
        }
    }
}