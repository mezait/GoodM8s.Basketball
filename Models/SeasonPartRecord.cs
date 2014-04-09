using Orchard.ContentManagement.Records;

namespace GoodM8s.Basketball.Models {
    public class SeasonPartRecord : ContentPartRecord {
        public virtual bool IsActive { get; set; }
        public virtual string Title { get; set; }
    }
}