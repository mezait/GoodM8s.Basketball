using System.Linq;
using GoodM8s.Basketball.Models;
using GoodM8s.Basketball.Services;
using GoodM8s.Basketball.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;

namespace GoodM8s.Basketball.Drivers {
    public class GameDriver : ContentPartDriver<GamePart> {
        private readonly IGameService _gameService;
        private readonly IPlayerService _playerService;
        private readonly ISportService _sportService;

        public GameDriver(IGameService gameService, IPlayerService playerService, ISportService sportService) {
            _gameService = gameService;
            _playerService = playerService;
            _sportService = sportService;
        }

        protected override string Prefix {
            get { return "Game"; }
        }

        private GameEditViewModel BuildEditorViewModel(GamePart part) {
            var players = _playerService.Get();

            var statistics = part.GameStatistics.Select(gameStatistic => new GameStatisticEditViewModel {
                FieldGoalsMade = gameStatistic.FieldGoalsMade,
                FreeThrowsMade = gameStatistic.FreeThrowsMade,
                Id = gameStatistic.Id,
                PersonalFouls = gameStatistic.PersonalFouls,
                PlayerId = gameStatistic.PlayerPartRecord.Id,
                Players = players,
                TechFouls = gameStatistic.TechFouls,
                ThreeFieldGoalsMade = gameStatistic.ThreeFieldGoalsMade
            }).ToList();

            return new GameEditViewModel {
                Date = part.Date,
                SportId = part.SportId,
                Sports = _sportService.Get(),
                GameStatistics = statistics
            };
        }

        protected override DriverResult Editor(GamePart part, dynamic shapeHelper) {
            return ContentShape("Parts_Game_Edit", () => shapeHelper.EditorTemplate(TemplateName: "Parts/Game", Model: BuildEditorViewModel(part), Prefix: Prefix));
        }

        protected override DriverResult Editor(GamePart part, IUpdateModel updater, dynamic shapeHelper) {
            var model = new GameEditViewModel();

            updater.TryUpdateModel(model, Prefix, null, null);

            if (part.ContentItem.Id != 0) {
                _gameService.UpdateForContentItem(part.ContentItem, model);
            }

            return Editor(part, shapeHelper);
        }
    }
}