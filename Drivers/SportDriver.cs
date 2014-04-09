using GoodM8s.Basketball.Models;
using GoodM8s.Basketball.Services;
using GoodM8s.Basketball.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;

namespace GoodM8s.Basketball.Drivers {
    public class SportDriver : ContentPartDriver<SportPart> {
        private readonly ISeasonService _seasonService;
        private readonly ISportService _sportService;

        public SportDriver(ISeasonService seasonService, ISportService sportService) {
            _seasonService = seasonService;
            _sportService = sportService;
        }

        protected override string Prefix {
            get { return "Sport"; }
        }

        private SportEditViewModel BuildEditorViewModel(SportPart part) {
            return new SportEditViewModel {
                FiXiId = part.FiXiId,
                Name = part.Name,
                SeasonId = part.SeasonId,
                Seasons = _seasonService.Get(),
                StartDate = part.StartDate,
                WeekOffset = part.WeekOffset
            };
        }

        protected override DriverResult Editor(SportPart part, dynamic shapeHelper) {
            return ContentShape("Parts_Sport_Edit", () => shapeHelper.EditorTemplate(TemplateName: "Parts/Sport", Model: BuildEditorViewModel(part), Prefix: Prefix));
        }

        protected override DriverResult Editor(SportPart part, IUpdateModel updater, dynamic shapeHelper) {
            var model = new SportEditViewModel();

            updater.TryUpdateModel(model, Prefix, null, null);

            if (part.ContentItem.Id != 0) {
                _sportService.UpdateForContentItem(part.ContentItem, model);
            }

            return Editor(part, shapeHelper);
        }
    }
}