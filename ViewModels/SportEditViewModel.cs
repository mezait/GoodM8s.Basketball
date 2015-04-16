using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GoodM8s.Basketball.Models;

namespace GoodM8s.Basketball.ViewModels {
    public class SportEditViewModel {
        [Required]
        public int? SeasonId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int? FiXiId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        public int? WeekOffset { get; set; }
        public int? RoundCount { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<SeasonPart> Seasons { get; set; }
    }
}