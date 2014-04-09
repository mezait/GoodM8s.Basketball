using System.Collections.Generic;
using GoodM8s.Basketball.Models;
using Orchard;

namespace GoodM8s.Basketball.Services {
    public interface IGameStatisticService : IDependency {
        void Create(GameStatisticRecord entity);
        void Delete(GameStatisticRecord entity);
        IEnumerable<GameStatisticRecord> GetByGame(int gameId);
    }
}