namespace GoodM8s.Basketball.Models {
    public class GameStatisticRecord {
        public virtual int Id { get; set; }
        public virtual GamePartRecord GamePartRecord { get; set; }
        public virtual PlayerPartRecord PlayerPartRecord { get; set; }
        public virtual int? FieldGoalsMade { get; set; }
        public virtual int? ThreeFieldGoalsMade { get; set; }
        public virtual int? FreeThrowsMade { get; set; }
        public virtual int? PersonalFouls { get; set; }
        public virtual int? TechFouls { get; set; }
    }
}