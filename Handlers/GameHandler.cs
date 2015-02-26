using GoodM8s.Basketball.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace GoodM8s.Basketball.Handlers {
    public class GameHandler : ContentHandler {
        public GameHandler(IContentManager contentManager, IRepository<GamePartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));

            OnRemoved<GamePart>((context, part) => repository.Delete(part.Record));

            OnLoading<GamePart>((context, part) => part.SportField.Loader(item => part.SportId != null
                ? contentManager.Get<SportPart>(part.SportId.Value)
                : null));
        }
    }
}