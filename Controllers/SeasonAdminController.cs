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
    public class SeasonAdminController : Controller, IUpdateModel {
        private readonly IOrchardServices _orchardServices;
        private readonly ISeasonService _seasonService;
        private readonly ISiteService _siteService;

        public SeasonAdminController(IOrchardServices orchardServices, ISeasonService seasonService, ISiteService siteService, IShapeFactory shapeFactory) {
            _orchardServices = orchardServices;
            _seasonService = seasonService;
            _siteService = siteService;

            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        private dynamic Shape { get; set; }

        private Localizer T { get; set; }

        public ActionResult Index(PagerParameters pagerParameters) {
            var seasonsProjection = from season in _seasonService.Get()
                                    select Shape.Season
                                        (
                                            Id: season.Id,
                                            IsActive: season.IsActive,
                                            Title: season.Title
                                        );

            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);

            var model = new DynamicIndexViewModel(
                seasonsProjection.Skip(pager.GetStartIndex()).Take(pager.PageSize),
                Shape.Pager(pager).TotalItemCount(_seasonService.Count()));

            return View(model);
        }

        public ActionResult Create() {
            var season = _orchardServices.ContentManager.New<SeasonPart>("Season");
            var model = _orchardServices.ContentManager.BuildEditor(season);

            return View((object) model);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST() {
            var season = _orchardServices.ContentManager.New<SeasonPart>("Season");

            _orchardServices.ContentManager.Create(season);

            var model = _orchardServices.ContentManager.UpdateEditor(season, this);

            if (!ModelState.IsValid) {
                _orchardServices.TransactionManager.Cancel();

                _orchardServices.Notifier.Error(T("Error creating season!"));

                return View((object) model);
            }

            _orchardServices.Notifier.Information(T("Season created successfully."));

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id) {
            var season = _seasonService.Get(id);
            var model = _orchardServices.ContentManager.BuildEditor(season);

            return View((object) model);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPOST(int id) {
            var season = _seasonService.Get(id);
            var model = _orchardServices.ContentManager.UpdateEditor(season, this);

            if (!ModelState.IsValid)
            {
                _orchardServices.TransactionManager.Cancel();

                _orchardServices.Notifier.Error(T("Error updating season!"));

                return View((object)model);
            }

            _orchardServices.Notifier.Information(T("Season updated successfully."));

            return RedirectToAction("Edit", new {id});
        }

        [HttpPost]
        public ActionResult Delete(int id) {
            var season = _seasonService.Get(id);

            if (season == null)
                return HttpNotFound();

            _orchardServices.ContentManager.Remove(season.ContentItem);

            _orchardServices.Notifier.Information(T("Season deleted successfully."));

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