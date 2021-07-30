using JdaTools.Connection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdaTools.Studio.Models
{
    public class CommandDefinition
    {
        [MocaColumn("CMPLVL")]
        public string Level { get; set; }
        [MocaColumn("CMPLVLSEQ")]
        public int? Sequence { get; set; }
        [MocaColumn("COMMAND")]
        public string CommandName { get; set; }
        [MocaColumn("TYPE")]
        public string Type { get; set; }
        [MocaColumn("SYNTAX")]
        public string Syntax { get; set; }

        public override string ToString()
        {
            return CommandName;
        }
    }
}
