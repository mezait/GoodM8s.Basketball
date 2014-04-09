using GoodM8s.Basketball.Models;
using GoodM8s.Basketball.Services;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace GoodM8s.Basketball.Handlers {
    public class SeasonHandler : ContentHandler {
        public SeasonHandler(IRepository<SeasonPartRecord> repository, ISeasonService seasonService) {
            Filters.Add(StorageFilter.For(repository));

            OnCreated<SeasonPart>((context, part) => {
                if (part.IsActive) {
                    seasonService.DeactivateOthers(part.Id);
                }
            });

            OnRemoved<SeasonPart>((context, part) => repository.Delete(part.Record));

            OnUpdated<SeasonPart>((context, part) => {
                if (part.IsActive) {
                    seasonService.DeactivateOthers(part.Id);
                }
            });
        }
    }
}