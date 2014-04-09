using GoodM8s.Basketball.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;

namespace GoodM8s.Basketball.Drivers {
    public class PlayerDriver : ContentPartDriver<PlayerPart> {
        protected override string Prefix {
            get { return "Player"; }
        }

        protected override DriverResult Editor(PlayerPart part, dynamic shapeHelper) {
            return ContentShape("Parts_Player_Edit", () => shapeHelper.EditorTemplate(TemplateName: "Parts/Player", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(PlayerPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);

            return Editor(part, shapeHelper);
        }
    }
}