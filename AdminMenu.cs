using Orchard.Localization;
using Orchard.UI.Navigation;

namespace GoodM8s.Basketball {
    public class AdminMenu : INavigationProvider {
        public AdminMenu() {
            T = NullLocalizer.Instance;
        }

        private Localizer T { get; set; }

        public string MenuName {
            get { return "admin"; }
        }

        public void GetNavigation(NavigationBuilder builder) {
            builder
                .AddImageSet("goodm8s")
                .Add(menu => menu
                    .Caption(T("GoodM8s Basketball"))
                    .Position("20")
                    .LinkToFirstChild(false)
                    .Add(submenu => submenu
                        .Caption(T("Seasons"))
                        .Position("20.1")
                        .Action("Index", "SeasonAdmin", new {area = "GoodM8s.Basketball"}))
                    .Add(submenu => submenu
                        .Caption(T("Sports"))
                        .Position("20.2")
                        .Action("Index", "SportAdmin", new {area = "GoodM8s.Basketball"}))
                    .Add(submenu => submenu
                        .Caption(T("Players"))
                        .Position("20.3")
                        .Action("Index", "PlayerAdmin", new {area = "GoodM8s.Basketball"}))
                    .Add(submenu => submenu
                        .Caption(T("Games"))
                        .Position("20.4")
                        .Action("Index", "GameAdmin", new {area = "GoodM8s.Basketball"}))
                );
        }
    }
}