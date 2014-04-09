using GoodM8s.Basketball.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace GoodM8s.Basketball.Handlers {
    public class SportHandler : ContentHandler {
        public SportHandler(IContentManager contentManager, IRepository<SportPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
            
            OnRemoved<SportPart>((context, part) => repository.Delete(part.Record));

            OnLoading<SportPart>((context, part) => part.SeasonField.Loader(item => part.SeasonId != null
                                                                                        ? contentManager.Get<SeasonPart>(part.SeasonId.Value)
                                                                                        : null));
        }
    }
}