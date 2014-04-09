using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;

namespace GoodM8s.Basketball.Models {
    public class PlayerPart : ContentPart<PlayerPartRecord> {
        [Required]
        public string FirstName {
            get { return Record.FirstName; }
            set { Record.FirstName = value; }
        }

        [Required]
        public string LastName {
            get { return Record.LastName; }
            set { Record.LastName = value; }
        }

        [Required]
        public int Number {
            get { return Record.Number; }
            set { Record.Number = value; }
        }
    }
}