using GoodM8s.Basketball.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;

namespace GoodM8s.Basketball.Drivers {
    public class SeasonDriver : ContentPartDriver<SeasonPart> {
        protected override string Prefix {
            get { return "Season"; }
        }

        protected override DriverResult Editor(SeasonPart part, dynamic shapeHelper) {
            return ContentShape("Parts_Season_Edit", () => shapeHelper.EditorTemplate(TemplateName: "Parts/Season", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(SeasonPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);

            return Editor(part, shapeHelper);
        }
    }
}