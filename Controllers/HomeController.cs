using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using GoodM8s.Basketball.Models;
using GoodM8s.Basketball.Services;
using GoodM8s.Basketball.ViewModels;
using Orchard.Themes;
using sneakyresults;

namespace GoodM8s.Basketball.Controllers {
    [Themed]
    public class HomeController : Controller {
        private readonly IGameService _gameService;
        private readonly ISeasonService _seasonService;
        private readonly ISportService _sportService;

        public HomeController(IGameService gameService, ISeasonService seasonService, ISportService sportService) {
            _gameService = gameService;
            _seasonService = seasonService;
            _sportService = sportService;
        }

        #region Private Methods

        /// <summary>
        /// Count how many weeks between two dates
        /// </summary>
        /// <param name="start">Start date</param>
        /// <param name="end">End date</param>
        /// <returns>Number of weeks</returns>
        private static int CountWeeks(DateTime start, DateTime end) {
            return (int) (end - start).TotalDays/7;
        }

        /// <summary>
        /// How many days from now until this day of the week
        /// </summary>
        /// <param name="now">Current date</param>
        /// <param name="dayOfWeek">Day of the week</param>
        /// <returns>Number of days</returns>
        private static int DaysToAdd(DayOfWeek now, DayOfWeek dayOfWeek) {
            return (7 - (int) now + (int) dayOfWeek)%7;
        }

        /// <summary>
        /// Get the next date for a day of the week
        /// </summary>
        /// <param name="dayOfWeek">Day of the week</param>
        /// <returns>Date for the day of the week</returns>
        private static DateTime NextDayOfWeek(DayOfWeek dayOfWeek) {
            var now = DateTime.Now;

            return (now.AddDays(DaysToAdd(now.DayOfWeek, dayOfWeek)));
        }

        /// <summary>
        /// Count the maximum number of rounds
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="offset">Offset for missed weeks</param>
        /// <returns>Maximum number of rounds</returns>
        private static int MaximumRoundNumber(DateTime startDate, int offset) {
            return CountWeeks(startDate, NextDayOfWeek(startDate.DayOfWeek)) + 1 - offset;
        }

        /// <summary>
        /// Add an ordinal to a number
        /// </summary>
        /// <param name="number">Number</param>
        /// <returns>Number and ordinal</returns>
        private static string Ordinal(int number) {
            if (number <= 0) {
                return number.ToString(CultureInfo.InvariantCulture);
            }

            switch (number%100) {
                case 11:
                case 12:
                case 13:
                    return number + "th";
            }

            switch (number%10) {
                case 1:
                    return number + "st";
                case 2:
                    return number + "nd";
                case 3:
                    return number + "rd";
                default:
                    return number + "th";
            }
        }

        #endregion

        public ActionResult Fixtures(int sportId, int? round) {
            var sport = _sportService.Get(sportId);

            if (sport == null) {
                return HttpNotFound();
            }

            var maxRoundNumber = MaximumRoundNumber(
                sport.StartDate.GetValueOrDefault(),
                sport.WeekOffset.GetValueOrDefault());

            var roundNumber = round.HasValue
                                  ? round.Value
                                  : maxRoundNumber;

            ViewBag.Id = sport.Id;
            ViewBag.Name = sport.Name;
            ViewBag.MaxRoundNumber = maxRoundNumber;
            ViewBag.RoundNumber = roundNumber;

            return View(fixtures.GetYmcaFixtures(sport.FiXiId.GetValueOrDefault(), roundNumber).Tables[0]);
        }

        public ActionResult Ladder(int sportId, int? round) {
            var sport = _sportService.Get(sportId);

            if (sport == null) {
                return HttpNotFound();
            }

            var maxRoundNumber = MaximumRoundNumber(
                sport.StartDate.GetValueOrDefault(),
                sport.WeekOffset.GetValueOrDefault());

            var roundNumber = round.HasValue
                                  ? round.Value
                                  : maxRoundNumber;

            ViewBag.Id = sport.Id;
            ViewBag.Name = sport.Name;
            ViewBag.MaxRoundNumber = maxRoundNumber;
            ViewBag.RoundNumber = roundNumber;

            return View(fixtures.GetYmcaFixtures(sport.FiXiId.GetValueOrDefault(), roundNumber).Tables[1]);
        }

        public ActionResult Statistics(int sportId, int? gameId) {
            var sport = _sportService.Get(sportId);

            if (sport == null) {
                return HttpNotFound();
            }

            var gameCount = 0;
            var games = _gameService.GetBySport(sport.Id);
            var players = new Dictionary<PlayerPartRecord, SportTotalsViewModel>();

            ViewBag.GameIds = games.Select(g => g.Id).ToList();

            if (gameId.HasValue) {
                var game = games.SingleOrDefault(g => g.Id == gameId);

                if (game != null) {
                    games = new List<GamePart> {game};
                }
            }

            if (games != null) {
                foreach (var game in games) {
                    gameCount++;

                    foreach (var statistic in game.GameStatistics) {
                        var player = statistic.PlayerPartRecord;

                        if (!players.ContainsKey(player)) {
                            players.Add(player, new SportTotalsViewModel());
                        }

                        var fieldGoalsMade = statistic.FieldGoalsMade.GetValueOrDefault();
                        var threeFieldGoalsMade = statistic.ThreeFieldGoalsMade.GetValueOrDefault();
                        var freeThrowsMade = statistic.FreeThrowsMade.GetValueOrDefault();

                        players[player].GamesPlayed++;
                        players[player].FieldGoalsMade += fieldGoalsMade;
                        players[player].ThreeFieldGoalsMade += threeFieldGoalsMade;
                        players[player].FreeThrowsMade += freeThrowsMade;
                        players[player].PersonalFouls += statistic.PersonalFouls.GetValueOrDefault();

                        if (fieldGoalsMade + threeFieldGoalsMade + freeThrowsMade == 0) {
                            players[player].Donuts++;
                        }
                    }
                }
            }
            
            ViewBag.Id = sportId;
            ViewBag.Name = sport.Name;
            ViewBag.GameId = gameId ?? 0;
            ViewBag.Games = gameCount;

            return View(players);
        }

        public ActionResult Summary() {
            var season = _seasonService.GetActive();
            var summaries = new List<SummaryViewModel>();

            foreach (var sport in _sportService.GetBySeason(season.Id)) {
                var maxRoundNumber = MaximumRoundNumber(
                    sport.StartDate.GetValueOrDefault(),
                    sport.WeekOffset.GetValueOrDefault());

                var dataSet = fixtures.GetYmcaFixtures(sport.FiXiId.GetValueOrDefault(), maxRoundNumber);

                // Fixtures
                var fixtureQuery = from row in dataSet.Tables[0].AsEnumerable()
                                   where row.Field<string>("hometeamname") == "GOODM8S" ||
                                         row.Field<string>("awayteamname") == "GOODM8S"
                                   select row;

                var fixtureRow = fixtureQuery.FirstOrDefault();

                // Ladder
                var ladderQuery = from row in dataSet.Tables[1].AsEnumerable()
                                  where row.Field<string>("teamname") == "GOODM8S"
                                  select row;

                var ladderRow = ladderQuery.FirstOrDefault();

                // Ladder vs
                var vsQuery = from row in dataSet.Tables[1].AsEnumerable()
                              where row.Field<string>("teamname") == (fixtureRow["hometeamname"].ToString() != "GOODM8S"
                                                                          ? fixtureRow["hometeamname"].ToString()
                                                                          : fixtureRow["awayteamname"].ToString())
                              select row;

                var vsRow = vsQuery.FirstOrDefault();

                summaries.Add(new SummaryViewModel {
                    Court = fixtureRow["court"].ToString(),
                    GameDate = Convert.ToDateTime(fixtureRow["gamedate"].ToString()),
                    GameTime = fixtureRow["gametime"].ToString(),
                    Points = ladderRow["points"].ToString(),
                    Position = Ordinal((int) ladderRow["position"]),
                    Vs = vsRow["teamname"].ToString(),
                    VsPoints = vsRow["points"].ToString(),
                    VsPosition = Ordinal((int) vsRow["position"])
                });
            }

            return View(summaries);
        }
    }
}