using System.Linq;
using GoodM8s.Basketball.Models;
using GoodM8s.Core.Services;
using Orchard;

namespace GoodM8s.Basketball.Services {
    public class SeasonService : BaseService<SeasonPart, SeasonPartRecord>, ISeasonService {
        public SeasonService(IOrchardServices orchardServices) : base(orchardServices) {}

        public void DeactivateOthers(int id) {
            foreach (var season in GetQuery().Where(s => s.Id != id).List()) {
                season.IsActive = false;
            }
        }

        public SeasonPart GetActive() {
            return GetQuery().Where(s => s.IsActive).Slice(0, 1).FirstOrDefault();
        }
    }
}