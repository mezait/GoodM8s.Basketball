using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;

namespace GoodM8s.Basketball.Models {
    public class SeasonPart : ContentPart<SeasonPartRecord> {
        public bool IsActive {
            get { return Record.IsActive; }
            set { Record.IsActive = value; }
        }

        [Required]
        public string Title {
            get { return Record.Title; }
            set { Record.Title = value; }
        }
    }
}