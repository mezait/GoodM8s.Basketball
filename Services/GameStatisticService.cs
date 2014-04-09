using System.Collections.Generic;
using GoodM8s.Basketball.Models;
using Orchard.Data;

namespace GoodM8s.Basketball.Services {
    public class GameStatisticService : IGameStatisticService {
        private readonly IRepository<GameStatisticRecord> _gameStatisticRepository;

        public GameStatisticService(IRepository<GameStatisticRecord> gameStatisticRepository) {
            _gameStatisticRepository = gameStatisticRepository;
        }

        public void Create(GameStatisticRecord entity) {
            _gameStatisticRepository.Create(entity);
        }

        public void Delete(GameStatisticRecord entity) {
            _gameStatisticRepository.Delete(entity);
        }

        public IEnumerable<GameStatisticRecord> GetByGame(int gameId) {
            return _gameStatisticRepository.Fetch(s => s.GamePartRecord.Id == gameId);
        }
    }
}