using System.Collections.Generic;
using GoodM8s.Basketball.Models;

namespace GoodM8s.Basketball.ViewModels {
    public class PlayerMaxViewModel {
        public IList<PlayerPartRecord> Players { get; set; }
        public int Value { get; set; }
    }
}