using System.Linq;
using System.Web.Mvc;
using GoodM8s.Basketball.Models;
using GoodM8s.Basketball.Services;
using GoodM8s.Basketball.ViewModels;
using GoodM8s.Core.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;

namespace GoodM8s.Basketball.Controllers {
    [Admin]
    public class GameAdminController : Controller, IUpdateModel {
        private readonly IOrchardServices _orchardServices;
        private readonly IGameService _gameService;
        private readonly IPlayerService _playerService;
        private readonly ISiteService _siteService;

        public GameAdminController(IOrchardServices orchardServices, IGameService gameService, IPlayerService playerService, ISiteService siteService, IShapeFactory shapeFactory) {
            _orchardServices = orchardServices;
            _siteService = siteService;
            _gameService = gameService;
            _playerService = playerService;

            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        private dynamic Shape { get; set; }

        private Localizer T { get; set; }

        public ActionResult Index(PagerParameters pagerParameters) {
            var gamesProjection = from game in _gameService.Get()
                select Shape.Game
                    (
                        Id: game.Id,
                        Date: game.Date,
                        Sport: game.Sport.Name
                    );

            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);

            var model = new DynamicIndexViewModel(
                gamesProjection.Skip(pager.GetStartIndex()).Take(pager.PageSize),
                Shape.Pager(pager).TotalItemCount(_gameService.Count()));

            return View(model);
        }

        public ActionResult Create() {
            var game = _orchardServices.ContentManager.New<GamePart>("Game");
            var model = _orchardServices.ContentManager.BuildEditor(game);

            return View((object) model);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST() {
            var game = _orchardServices.ContentManager.New<GamePart>("Game");

            _orchardServices.ContentManager.Create(game);

            var model = _orchardServices.ContentManager.UpdateEditor(game, this);

            if (!ModelState.IsValid) {
                _orchardServices.TransactionManager.Cancel();

                _orchardServices.Notifier.Error(T("Error creating game!"));

                return View((object) model);
            }

            _orchardServices.Notifier.Information(T("Game created successfully."));

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id) {
            var game = _gameService.Get(id);
            var model = _orchardServices.ContentManager.BuildEditor(game);

            return View((object) model);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPOST(int id) {
            var game = _gameService.Get(id);
            var model = _orchardServices.ContentManager.UpdateEditor(game, this);

            if (!ModelState.IsValid) {
                _orchardServices.TransactionManager.Cancel();

                _orchardServices.Notifier.Error(T("Error updating game!"));

                return View((object) model);
            }

            _orchardServices.Notifier.Information(T("Game updated successfully."));

            return RedirectToAction("Edit", new {id});
        }

        [HttpPost]
        public ActionResult Delete(int id) {
            var game = _gameService.Get(id);

            if (game == null) {
                return HttpNotFound();
            }

            _orchardServices.ContentManager.Remove(game.ContentItem);

            _orchardServices.Notifier.Information(T("Game deleted successfully."));

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Add a statistic to the list
        /// </summary>
        /// <returns>Statistics view</returns>
        public ActionResult AddStatistic() {
            return PartialView("EditorTemplates/GameStatisticEditViewModel", new GameStatisticEditViewModel {
                Players = _playerService.Get()
            });
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}