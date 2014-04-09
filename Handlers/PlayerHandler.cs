using GoodM8s.Basketball.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Users.Models;

namespace GoodM8s.Basketball.Handlers {
    public class PlayerHandler : ContentHandler {
        public PlayerHandler(IRepository<PlayerPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
            Filters.Add(new ActivatingFilter<UserPart>("Player"));

            OnRemoved<PlayerPart>((context, part) => repository.Delete(part.Record));
        }
    }
}