using System.Collections.Generic;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace GoodM8s.Basketball.Services {
    public abstract class BaseService<TPart, TRecord> : IBaseService<TPart, TRecord>
        where TPart : ContentPart<TRecord>
        where TRecord : ContentPartRecord {
        private readonly IOrchardServices _orchardServices;

        protected BaseService(IOrchardServices orchardServices) {
            _orchardServices = orchardServices;
        }

        protected IContentQuery<TPart, TRecord> GetQuery() {
            return _orchardServices.ContentManager.Query<TPart, TRecord>();
        }

        public int Count() {
            return GetQuery().Count();
        }

        public IEnumerable<TPart> Get() {
            return GetQuery().List();
        }

        public TPart Get(int id) {
            return _orchardServices.ContentManager.Get<TPart>(id);
        }
    }
}