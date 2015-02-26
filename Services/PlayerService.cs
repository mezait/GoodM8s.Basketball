using System.Collections.Generic;
using GoodM8s.Basketball.Models;
using GoodM8s.Core.Services;
using Orchard;

namespace GoodM8s.Basketball.Services {
    public class PlayerService : BaseService<PlayerPart, PlayerPartRecord>, IPlayerService {
        public PlayerService(IOrchardServices orchardServices) : base(orchardServices) {}

        public new IEnumerable<PlayerPart> Get() {
            return GetQuery().OrderBy(player => player.Number).List();
        }
    }
}