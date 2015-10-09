namespace GoodM8s.Basketball.ViewModels {
    public class SportTotalsViewModel {
        public int GamesPlayed { get; set; }
        public int FieldGoalsMade { get; set; }
        public int ThreeFieldGoalsMade { get; set; }
        public int FreeThrowsMade { get; set; }
        public int PersonalFouls { get; set; }
        public int TechFouls { get; set; }
        public int Donuts { get; set; }
        public int FoulOuts { get; set; }

        public int Points {
            get { return FreeThrowsMade + (FieldGoalsMade*2) + (ThreeFieldGoalsMade*3); }
        }

        public double PointsPerGame {
            get { return (double) Points/GamesPlayed; }
        }

        public double FoulsPerGame {
            get { return (double) PersonalFouls/GamesPlayed; }
        }

        public decimal DonutPercentage {
            get { return (decimal) Donuts/GamesPlayed; }
        }

        public decimal FoulOutsPercentage {
            get { return (decimal) FoulOuts/GamesPlayed; }
        }
    }
}