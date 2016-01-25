using System.Collections.Generic;
using System.Linq;
using GoodM8s.Basketball.Models;
using GoodM8s.Basketball.ViewModels;
using GoodM8s.Core.Services;
using Orchard;
using Orchard.ContentManagement;

namespace GoodM8s.Basketball.Services {
    public class GameService : BaseService<GamePart, GamePartRecord>, IGameService {
        private readonly IGameStatisticService _gameStatisticService;
        private readonly IPlayerService _playerService;

        public GameService(IOrchardServices orchardServices, IGameStatisticService gameStatisticService, IPlayerService playerService)
            : base(orchardServices) {
            _gameStatisticService = gameStatisticService;
            _playerService = playerService;
        }

        public new IEnumerable<GamePart> Get() {
            return GetQuery().OrderBy(game => game.Date).List();
        }

        public IEnumerable<GamePart> GetBySport(int sportId) {
            return GetQuery().Where(s => s.SportId == sportId).List();
        }

        public void UpdateForContentItem(IContent item, GameEditViewModel model) {
            var gamePart = item.As<GamePart>();

            gamePart.Date = model.Date;
            gamePart.SportId = model.SportId;
            gamePart.Notes = model.Notes;

            var oldStatistics = _gameStatisticService.GetByGame(gamePart.Id).ToList();

            // Make sure this is never null
            if (model.GameStatistics == null) {
                model.GameStatistics = new List<GameStatisticEditViewModel>();
            }

            foreach (var statistic in oldStatistics) {
                var statisticModel = model.GameStatistics.SingleOrDefault(m => m.Id == statistic.Id);

                if (statisticModel != null) {
                    // Update existing statistics
                    statistic.FieldGoalsMade = statisticModel.FieldGoalsMade;
                    statistic.FreeThrowsMade = statisticModel.FreeThrowsMade;
                    statistic.PersonalFouls = statisticModel.PersonalFouls;
                    statistic.TechFouls = statisticModel.TechFouls;
                    statistic.PlayerPartRecord = statisticModel.PlayerId.HasValue
                        ? _playerService.Get(statisticModel.PlayerId.Value).Record
                        : null;
                    statistic.ThreeFieldGoalsMade = statisticModel.ThreeFieldGoalsMade;
                }
                else {
                    // Delete the statistics that no longer exist
                    _gameStatisticService.Delete(statistic);
                }
            }

            // Add the new statistics
            foreach (var statistic in from statistic in model.GameStatistics
                let oldStatistic = oldStatistics.SingleOrDefault(m => m.Id == statistic.Id)
                where oldStatistic == null
                select statistic) {
                _gameStatisticService.Create(
                    new GameStatisticRecord {
                        FieldGoalsMade = statistic.FieldGoalsMade,
                        FreeThrowsMade = statistic.FreeThrowsMade,
                        GamePartRecord = gamePart.Record,
                        Id = statistic.Id,
                        PersonalFouls = statistic.PersonalFouls,
                        TechFouls = statistic.TechFouls,
                        PlayerPartRecord = statistic.PlayerId.HasValue
                            ? _playerService.Get(statistic.PlayerId.Value).Record
                            : null,
                        ThreeFieldGoalsMade = statistic.ThreeFieldGoalsMade
                    });
            }
        }
    }
}