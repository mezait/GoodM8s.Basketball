using Orchard.ContentManagement.Records;

namespace GoodM8s.Basketball.Models {
    public class PlayerPartRecord : ContentPartRecord {
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual int Number { get; set; }
    }
}