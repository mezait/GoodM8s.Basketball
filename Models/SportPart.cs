using System;
using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;

namespace GoodM8s.Basketball.Models {
    public class SportPart : ContentPart<SportPartRecord> {
        private readonly LazyField<SeasonPart> _seasonPart = new LazyField<SeasonPart>();

        public LazyField<SeasonPart> SeasonField {
            get { return _seasonPart; }
        }

        [Required]
        public int? SeasonId {
            get { return Record.SeasonId; }
            set { Record.SeasonId = value; }
        }

        [Required]
        public string Name {
            get { return Record.Name; }
            set { Record.Name = value; }
        }

        [Required]
        public int? FiXiId {
            get { return Record.FiXiId; }
            set { Record.FiXiId = value; }
        }

        [Required]
        [DataType(DataType.Date)]
        public DateTime? StartDate {
            get { return Record.StartDate; }
            set { Record.StartDate = value; }
        }

        public int? WeekOffset {
            get { return Record.WeekOffset; }
            set { Record.WeekOffset = value; }
        }

        public int? RoundCount {
            get { return Record.RoundCount; }
            set { Record.RoundCount = value; }
        }

        public bool IsActive {
            get { return Record.IsActive; }
            set { Record.IsActive = value; }
        }

        public SeasonPart Season {
            get { return _seasonPart.Value; }
            set { _seasonPart.Value = value; }
        }
    }
}