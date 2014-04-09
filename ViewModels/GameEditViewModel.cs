using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GoodM8s.Basketball.Models;

namespace GoodM8s.Basketball.ViewModels {
    public class GameEditViewModel {
        [Required]
        public int? SportId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? Date { get; set; }

        public IEnumerable<SportPart> Sports { get; set; }
        public IEnumerable<GameStatisticEditViewModel> GameStatistics { get; set; }
    }
}