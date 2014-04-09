using System;
using GoodM8s.Basketball.Services;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.UI.Navigation;

namespace GoodM8s.Basketball {
    public class MainMenu : IMenuProvider {
        private readonly ISeasonService _seasonService;
        private readonly ISportService _sportService;

        public MainMenu(ISeasonService seasonService, ISportService sportService) {
            _seasonService = seasonService;
            _sportService = sportService;

            T = NullLocalizer.Instance;
        }

        private Localizer T { get; set; }

        public void GetMenu(IContent menu, NavigationBuilder builder) {
            builder.Add(T("Basketball"), "2", GetActiveSeason);
        }

        private void GetActiveSeason(NavigationBuilder builder) {
            var season = _seasonService.GetActive();

            if (season == null) {
                return;
            }

            builder.Add(T("Summary"), "2.1", item =>
                                             item.Action("Summary", "Home", new {area = "GoodM8s.Basketball"}));

            var index = 2;

            foreach (var sport in _sportService.GetBySeason(season.Id)) {
                var sportId = sport.Id;

                builder.Add(T(sport.Name + " - Fixtures"), String.Format("2.{0}", index), item =>
                                                                                          item.Action("Fixtures", "Home", new {area = "GoodM8s.Basketball", sportId}));

                index++;

                builder.Add(T(sport.Name + " - Ladder"), String.Format("2.{0}", index), item =>
                                                                                        item.Action("Ladder", "Home", new {area = "GoodM8s.Basketball", sportId}));

                index++;

                builder.Add(T(sport.Name + " - Statistics"), String.Format("2.{0}", index), item =>
                                                                                            item.Action("Statistics", "Home", new {area = "GoodM8s.Basketball", sportId}));

                index++;
            }
        }
    }
}