using JdaTools.Connection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdaTools.Test
{
    internal class TestClass
    {
        [MocaColumn("trvseq")]
        public string? TravelSequence { get; set; }
        [MocaColumn("stoloc")]
        public string? StorageLocation { get; set; }
        [MocaColumn("pck_zone_id")]
        public int? PickZoneId { get; set; }
        [MocaColumn("arecod")]
        public string? AreaCode { get; set; }
        [MocaColumn("invsts")]
        public string? Invst { get; set; }
        [MocaColumn("olddte")]
        public DateTime OldDate { get; set; }
        [MocaColumn("newdte")]
        public DateTime NewDate { get; set; }
        [MocaColumn("dtlflg")]
        public bool Flag { get; set; }
        [MocaColumn("uc_totalqty")]
        public int TotalQuantity { get; set; }
        [MocaColumn("uc_availableqty")]
        public int AvailableQuantity { get; set; }
    }
}
