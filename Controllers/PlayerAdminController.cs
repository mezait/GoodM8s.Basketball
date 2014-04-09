using System.Linq;
using System.Web.Mvc;
using GoodM8s.Basketball.Models;
using GoodM8s.Basketball.Services;
using GoodM8s.Basketball.ViewModels;
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
    public class PlayerAdminController : Controller, IUpdateModel {
        private readonly IOrchardServices _orchardServices;
        private readonly IPlayerService _playerService;
        private readonly ISiteService _siteService;

        public PlayerAdminController(IOrchardServices orchardServices, IPlayerService playerService, ISiteService siteService, IShapeFactory shapeFactory) {
            _orchardServices = orchardServices;
            _playerService = playerService;
            _siteService = siteService;

            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        private dynamic Shape { get; set; }

        private Localizer T { get; set; }

        public ActionResult Index(PagerParameters pagerParameters) {
            var playersProjection = from player in _playerService.Get()
                                    select Shape.Player
                                        (
                                            Id: player.Id,
                                            FirstName: player.FirstName,
                                            LastName: player.LastName,
                                            Number: player.Number
                                        );

            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);

            var model = new DynamicIndexViewModel(
                playersProjection.Skip(pager.GetStartIndex()).Take(pager.PageSize),
                Shape.Pager(pager).TotalItemCount(_playerService.Count()));

            return View(model);
        }

        public ActionResult Create() {
            var player = _orchardServices.ContentManager.New<PlayerPart>("Player");
            var model = _orchardServices.ContentManager.BuildEditor(player);

            return View((object) model);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST() {
            var player = _orchardServices.ContentManager.New<PlayerPart>("Player");

            _orchardServices.ContentManager.Create(player);

            var model = _orchardServices.ContentManager.UpdateEditor(player, this);

            if (!ModelState.IsValid) {
                _orchardServices.TransactionManager.Cancel();

                _orchardServices.Notifier.Error(T("Error creating player!"));

                return View((object) model);
            }

            _orchardServices.Notifier.Information(T("Player created successfully."));

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id) {
            var player = _playerService.Get(id);
            var model = _orchardServices.ContentManager.BuildEditor(player);

            return View((object) model);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPOST(int id) {
            var player = _playerService.Get(id);
            var model = _orchardServices.ContentManager.UpdateEditor(player, this);

            if (!ModelState.IsValid) {
                _orchardServices.TransactionManager.Cancel();

                _orchardServices.Notifier.Error(T("Error updating player!"));

                return View((object) model);
            }

            _orchardServices.Notifier.Information(T("Player updated successfully."));

            return RedirectToAction("Edit", new {id});
        }

        [HttpPost]
        public ActionResult Delete(int id) {
            var player = _playerService.Get(id);

            if (player == null) {
                return HttpNotFound();
            }

            _orchardServices.ContentManager.Remove(player.ContentItem);

            _orchardServices.Notifier.Information(T("Player deleted successfully."));

            return RedirectToAction("Index");
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}