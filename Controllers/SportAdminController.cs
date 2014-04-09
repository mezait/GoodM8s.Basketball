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
    public class SportAdminController : Controller, IUpdateModel {
        private readonly IOrchardServices _orchardServices;
        private readonly ISiteService _siteService;
        private readonly ISportService _sportService;

        public SportAdminController(IOrchardServices orchardServices, ISportService sportService, ISiteService siteService, IShapeFactory shapeFactory) {
            _orchardServices = orchardServices;
            _siteService = siteService;
            _sportService = sportService;

            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        private dynamic Shape { get; set; }

        private Localizer T { get; set; }

        public ActionResult Index(PagerParameters pagerParameters) {
            var sportsProjection = from sport in _sportService.Get()
                                   select Shape.Sport
                                       (
                                           Id: sport.Id,
                                           Name: sport.Name,
                                           Season: sport.Season.Title
                                       );

            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);

            var model = new DynamicIndexViewModel(
                sportsProjection.Skip(pager.GetStartIndex()).Take(pager.PageSize),
                Shape.Pager(pager).TotalItemCount(_sportService.Count()));

            return View(model);
        }

        public ActionResult Create() {
            var sport = _orchardServices.ContentManager.New<SportPart>("Sport");
            var model = _orchardServices.ContentManager.BuildEditor(sport);

            return View((object) model);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST() {
            var sport = _orchardServices.ContentManager.New<SportPart>("Sport");

            _orchardServices.ContentManager.Create(sport);

            var model = _orchardServices.ContentManager.UpdateEditor(sport, this);

            if (!ModelState.IsValid) {
                _orchardServices.TransactionManager.Cancel();

                _orchardServices.Notifier.Error(T("Error creating sport!"));

                return View((object) model);
            }

            _orchardServices.Notifier.Information(T("Sport created successfully."));

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id) {
            var sport = _sportService.Get(id);
            var model = _orchardServices.ContentManager.BuildEditor(sport);

            return View((object) model);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPOST(int id) {
            var sport = _sportService.Get(id);
            var model = _orchardServices.ContentManager.UpdateEditor(sport, this);

            if (!ModelState.IsValid) {
                _orchardServices.TransactionManager.Cancel();

                _orchardServices.Notifier.Error(T("Error updating sport!"));

                return View((object) model);
            }

            _orchardServices.Notifier.Information(T("Sport updated successfully."));

            return RedirectToAction("Edit", new {id});
        }

        [HttpPost]
        public ActionResult Delete(int id) {
            var sport = _sportService.Get(id);

            if (sport == null) {
                return HttpNotFound();
            }

            _orchardServices.ContentManager.Remove(sport.ContentItem);

            _orchardServices.Notifier.Information(T("Sport deleted successfully."));

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