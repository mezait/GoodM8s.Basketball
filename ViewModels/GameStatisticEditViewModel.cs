using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GoodM8s.Basketball.Models;

namespace GoodM8s.Basketball.ViewModels {
    public class GameStatisticEditViewModel {
        [Required]
        public int Id { get; set; }

        [Required]
        public int? PlayerId { get; set; }

        public int? FieldGoalsMade { get; set; }
        public int? ThreeFieldGoalsMade { get; set; }
        public int? FreeThrowsMade { get; set; }
        public int? PersonalFouls { get; set; }
        public IEnumerable<PlayerPart> Players { get; set; }
    }
}