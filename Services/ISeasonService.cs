using GoodM8s.Basketball.Models;
using GoodM8s.Core.Services;

namespace GoodM8s.Basketball.Services {
    public interface ISeasonService : IBaseService<SeasonPart, SeasonPartRecord> {
        void DeactivateOthers(int id);
        SeasonPart GetActive();
    }
}