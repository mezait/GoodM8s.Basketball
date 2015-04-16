using System;
using Orchard.ContentManagement.Records;

namespace GoodM8s.Basketball.Models {
    public class SportPartRecord : ContentPartRecord {
        public virtual int? SeasonId { get; set; }
        public virtual string Name { get; set; }
        public virtual int? FiXiId { get; set; }
        public virtual DateTime? StartDate { get; set; }
        public virtual int? WeekOffset { get; set; }
        public virtual int? RoundCount { get; set; }
        public virtual bool IsActive { get; set; }
    }
}