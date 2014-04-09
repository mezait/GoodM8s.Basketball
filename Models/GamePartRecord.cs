using System;
using System.Collections.Generic;
using Orchard.ContentManagement.Records;

namespace GoodM8s.Basketball.Models {
    public class GamePartRecord : ContentPartRecord {
        public GamePartRecord() {
// ReSharper disable DoNotCallOverridableMethodsInConstructor
            GameStatistics = new List<GameStatisticRecord>();
// ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        public virtual int? SportId { get; set; }
        public virtual DateTime? Date { get; set; }

        public virtual IList<GameStatisticRecord> GameStatistics { get; set; }
    }
}