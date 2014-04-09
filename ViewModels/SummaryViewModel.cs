using System;

namespace GoodM8s.Basketball.ViewModels {
    public class SummaryViewModel {
        public DateTime GameDate { get; set; }
        public string GameTime { get; set; }
        public string Court { get; set; }
        public string Vs { get; set; }
        public string Position { get; set; }
        public string Points { get; set; }
        public string VsPosition { get; set; }
        public string VsPoints { get; set; }
    }
}