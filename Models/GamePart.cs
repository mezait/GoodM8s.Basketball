using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;

namespace GoodM8s.Basketball.Models {
    public class GamePart : ContentPart<GamePartRecord> {
        private readonly LazyField<SportPart> _sportPart = new LazyField<SportPart>();

        public LazyField<SportPart> SportField {
            get { return _sportPart; }
        }

        [Required]
        public int? SportId {
            get { return Record.SportId; }
            set { Record.SportId = value; }
        }

        [Required]
        [DataType(DataType.Date)]
        public DateTime? Date {
            get { return Record.Date; }
            set { Record.Date = value; }
        }

        public SportPart Sport {
            get { return _sportPart.Value; }
            set { _sportPart.Value = value; }
        }

        public IList<GameStatisticRecord> GameStatistics {
            get { return Record.GameStatistics; }
        }

        public string Notes
        {
            get { return Record.Notes; }
            set { Record.Notes = value; }
        }
    }
}