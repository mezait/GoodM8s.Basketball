namespace GoodM8s.Basketball.ViewModels {
    public class SportMaxViewModel {
        public int GamesPlayed { get; set; }
        public int FieldGoalsMade { get; set; }
        public int ThreeFieldGoalsMade { get; set; }
        public int FreeThrowsMade { get; set; }
        public int PersonalFouls { get; set; }
        public int Donuts { get; set; }
        public decimal DonutPercentage { get; set; }
        public double FoulsPerGame { get; set; }
        public int FoulOuts { get; set; }
        public decimal FoulOutsPercentage { get; set; }
        public int Points { get; set; }
        public double PointsPerGame { get; set; }
    }
}