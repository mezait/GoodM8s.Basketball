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
        private const int MaxRounds = 26;
        private readonly IGameService _gameService;
        private readonly ISeasonService _seasonService;
        private readonly ISportService _sportService;
        private readonly IPlayerService _playerService;

        public HomeController(IGameService gameService, ISeasonService seasonService, ISportService sportService, IPlayerService playerService) {
            _gameService = gameService;
            _seasonService = seasonService;
            _sportService = sportService;
            _playerService = playerService;
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

            if (number%100 == 11 || number%100 == 12 || number%100 == 13) {
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

        /// <summary>
        /// Set the maximum value for a statistic
        /// </summary>
        /// <param name="maxPlayers">Dictionary</param>
        /// <param name="key">Statistic</param>
        /// <param name="value">Value</param>
        /// <param name="player">Player</param>
        private static void SetPlayerMax(IDictionary<string, PlayerMaxViewModel> maxPlayers, string key, int value, PlayerPartRecord player) {
            if (value <= 0) {
                return;
            }

            if (maxPlayers[key] == null || value > maxPlayers[key].Value) {
                maxPlayers[key] = new PlayerMaxViewModel {
                    Players = new List<PlayerPartRecord> {
                        player
                    },
                    Value = value
                };
            }
            else if (value == maxPlayers[key].Value) {
                if (maxPlayers[key].Players.All(p => p.Id != player.Id)) {
                    maxPlayers[key].Players.Add(player);
                }
            }
        }

        /// <summary>
        /// Figure out who we are playing this round
        /// </summary>
        /// <param name="fixId">Fixi Id</param>
        /// <param name="round">Round number</param>
        /// <returns>Team name of the team we are playing</returns>
        private static string Vs(int fixId, int round) {
            DataSet dataSet;

            try {
                dataSet = fixtures.GetYmcaFixtures(fixId, round);
            }
            catch (Exception) {
                throw new Exception("Regex failed.");
            }

            var m8Query = from row in dataSet.Tables[0].AsEnumerable()
                where row.Field<string>("hometeamname") == "GOODM8S" ||
                      row.Field<string>("awayteamname") == "GOODM8S"
                select row;

            var m8Row = m8Query.FirstOrDefault();

            if (m8Row != null) {
                return m8Row["hometeamname"].ToString() == "GOODM8S"
                    ? m8Row["awayteamname"].ToString()
                    : m8Row["hometeamname"].ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Determine the vs score
        /// </summary>
        /// <param name="dataSet"><see cref="DataSet" /></param>
        /// <param name="teamName">Team name</param>
        /// <returns><see cref="CompareScoreVs"/> object</returns>
        private static CompareScoreVs VsScore(DataSet dataSet, string teamName) {
            var vsQuery = from row in dataSet.Tables[0].AsEnumerable()
                where row.Field<string>("hometeamname") == teamName ||
                      row.Field<string>("awayteamname") == teamName
                select row;

            var vsRow = vsQuery.FirstOrDefault();

            if (vsRow == null) {
                return null;
            }

            if (vsRow["hometeamname"].ToString() == teamName) {
                return new CompareScoreVs {
                    Bye = vsRow["court"].ToString() == "None" || vsRow["awayteamname"].ToString() == "BYE",
                    Name = vsRow["awayteamname"].ToString(),
                    Score = new CompareScore {
                        Against = (int) vsRow["awayteamscore"],
                        For = (int) vsRow["hometeamscore"]
                    }
                };
            }

            return new CompareScoreVs {
                Bye = vsRow["court"].ToString() == "None" || vsRow["hometeamname"].ToString() == "BYE",
                Name = vsRow["hometeamname"].ToString(),
                Score = new CompareScore {
                    Against = (int) vsRow["hometeamscore"],
                    For = (int) vsRow["awayteamscore"]
                }
            };
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

            var roundNumber = round ?? maxRoundNumber;

            ViewBag.Id = sport.Id;
            ViewBag.Name = sport.Name;
            ViewBag.RoundCount = sport.RoundCount;
            ViewBag.MaxRoundNumber = maxRoundNumber;
            ViewBag.RoundNumber = roundNumber;

            try
            {
                return View(fixtures.GetYmcaFixtures(sport.FiXiId.GetValueOrDefault(), roundNumber).Tables[0]);
            }
            catch (Exception) {
                return View("SmissError");
            }
        }

        public ActionResult Ladder(int sportId, int? round) {
            var sport = _sportService.Get(sportId);

            if (sport == null) {
                return HttpNotFound();
            }

            var maxRoundNumber = MaximumRoundNumber(
                sport.StartDate.GetValueOrDefault(),
                sport.WeekOffset.GetValueOrDefault());

            var roundNumber = round ?? maxRoundNumber;

            ViewBag.Id = sport.Id;
            ViewBag.Name = sport.Name;
            ViewBag.RoundCount = sport.RoundCount;
            ViewBag.MaxRoundNumber = maxRoundNumber;
            ViewBag.RoundNumber = roundNumber;

            try {
                return View(fixtures.GetYmcaFixtures(sport.FiXiId.GetValueOrDefault(), roundNumber).Tables[1]);
            }
            catch (Exception) {
                return View("SmissError");
            }
        }

        public ActionResult Players() {
            return View(_playerService.Get());
        }

        public ActionResult PlayerStatistics(int playerId) {
            var player = _playerService.Get(playerId);
            if (player == null)
            {
                return HttpNotFound();
            }

            ViewBag.Player = player;

            var sports = _sportService.Get();
            if (sports == null) {
                return HttpNotFound();
            }
            
            var totals = new Dictionary<SportPart, SportTotalsViewModel>();
            var total = new SportTotalsViewModel();

            foreach (var sport in sports) {
                var games = _gameService.GetBySport(sport.Id);

                if (games == null) {
                    continue;
                }

                foreach (var game in games) {
                    foreach (var statistic in game.GameStatistics.Where(statistic => player.Id == statistic.PlayerPartRecord.Id)) {
                        if (!totals.ContainsKey(sport))
                        {
                            totals.Add(sport, new SportTotalsViewModel());
                        }

                        var fieldGoalsMade = statistic.FieldGoalsMade.GetValueOrDefault();
                        var threeFieldGoalsMade = statistic.ThreeFieldGoalsMade.GetValueOrDefault();
                        var freeThrowsMade = statistic.FreeThrowsMade.GetValueOrDefault();
                        var personalFouls = statistic.PersonalFouls.GetValueOrDefault();
                        var techFouls = statistic.TechFouls.GetValueOrDefault();

                        totals[sport].GamesPlayed++;
                        totals[sport].FieldGoalsMade += fieldGoalsMade;
                        totals[sport].ThreeFieldGoalsMade += threeFieldGoalsMade;
                        totals[sport].FreeThrowsMade += freeThrowsMade;
                        totals[sport].PersonalFouls += personalFouls;
                        totals[sport].TechFouls += techFouls;

                        total.GamesPlayed++;
                        total.FieldGoalsMade += fieldGoalsMade;
                        total.ThreeFieldGoalsMade += threeFieldGoalsMade;
                        total.FreeThrowsMade += freeThrowsMade;
                        total.PersonalFouls += personalFouls;
                        total.TechFouls += techFouls;

                        if (fieldGoalsMade + threeFieldGoalsMade + freeThrowsMade == 0) {
                            totals[sport].Donuts++;
                            total.Donuts++;
                        }

                        if (personalFouls >= 5) {
                            totals[sport].FoulOuts++;
                            total.FoulOuts++;
                        }
                    }
                }
            }

            var maxResults = new SportMaxViewModel
            {
                GamesPlayed = totals.Max(p => p.Value.GamesPlayed),
                FieldGoalsMade = totals.Max(p => p.Value.FieldGoalsMade),
                ThreeFieldGoalsMade = totals.Max(p => p.Value.ThreeFieldGoalsMade),
                FreeThrowsMade = totals.Max(p => p.Value.FreeThrowsMade),
                PersonalFouls = totals.Max(p => p.Value.PersonalFouls),
                TechFouls = totals.Max(p => p.Value.TechFouls),
                Donuts = totals.Max(p => p.Value.Donuts),
                DonutPercentage = totals.Max(p => p.Value.DonutPercentage),
                FoulsPerGame = totals.Max(p => p.Value.FoulsPerGame),
                FoulOuts = totals.Max(p => p.Value.FoulOuts),
                FoulOutsPercentage = totals.Max(p => p.Value.FoulOutsPercentage),
                Points = totals.Max(p => p.Value.Points),
                PointsPerGame = totals.Max(p => p.Value.PointsPerGame)
            };

            ViewBag.MaxResults = maxResults;
            ViewBag.Total = total;
            return View(totals); 
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

            var maxPlayers = new Dictionary<string, PlayerMaxViewModel> {
                {"fieldGoalsMade", null},
                {"threeFieldGoalsMade", null},
                {"freeThrowsMade", null},
                {"points", null}
            };

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
                        var personalFouls = statistic.PersonalFouls.GetValueOrDefault();
                        var techFouls = statistic.TechFouls.GetValueOrDefault();
                        var points = freeThrowsMade + (fieldGoalsMade*2) + (threeFieldGoalsMade*3);

                        players[player].GamesPlayed++;
                        players[player].FieldGoalsMade += fieldGoalsMade;
                        players[player].ThreeFieldGoalsMade += threeFieldGoalsMade;
                        players[player].FreeThrowsMade += freeThrowsMade;
                        players[player].PersonalFouls += personalFouls;
                        players[player].TechFouls += techFouls;

                        if (fieldGoalsMade + threeFieldGoalsMade + freeThrowsMade == 0) {
                            players[player].Donuts++;
                        }

                        if (personalFouls >= 5) {
                            players[player].FoulOuts++;
                        }

                        SetPlayerMax(maxPlayers, "fieldGoalsMade", fieldGoalsMade, player);
                        SetPlayerMax(maxPlayers, "threeFieldGoalsMade", threeFieldGoalsMade, player);
                        SetPlayerMax(maxPlayers, "freeThrowsMade", freeThrowsMade, player);
                        SetPlayerMax(maxPlayers, "points", points, player);
                    }
                }
            }

            var maxResults = new SportMaxViewModel
            {
                GamesPlayed = players.Max(p => p.Value.GamesPlayed),
                FieldGoalsMade = players.Max(p => p.Value.FieldGoalsMade),
                ThreeFieldGoalsMade = players.Max(p => p.Value.ThreeFieldGoalsMade),
                FreeThrowsMade = players.Max(p => p.Value.FreeThrowsMade),
                PersonalFouls = players.Max(p => p.Value.PersonalFouls),
                TechFouls = players.Max(p => p.Value.TechFouls),
                Donuts = players.Max(p => p.Value.Donuts),
                DonutPercentage = players.Max(p => p.Value.DonutPercentage),
                FoulsPerGame = players.Max(p => p.Value.FoulsPerGame),
                FoulOuts = players.Max(p => p.Value.FoulOuts),
                FoulOutsPercentage = players.Max(p => p.Value.FoulOutsPercentage),
                Points = players.Max(p => p.Value.Points),
                PointsPerGame = players.Max(p => p.Value.PointsPerGame)
            };

            ViewBag.Id = sportId;
            ViewBag.Name = sport.Name;
            ViewBag.GameId = gameId ?? 0;
            ViewBag.Games = gameCount;
            ViewBag.MaxPlayers = maxPlayers;
            ViewBag.MaxResults = maxResults;

            return View(players);
        }

        public ActionResult Summary() {
            var season = _seasonService.GetActive();
            var summaries = new List<SummaryViewModel>();

            foreach (var sport in _sportService.GetBySeason(season.Id, true)) {
                var maxRoundNumber = MaximumRoundNumber(
                    sport.StartDate.GetValueOrDefault(),
                    sport.WeekOffset.GetValueOrDefault());

                DataSet dataSet = null;

                try {
                    dataSet = fixtures.GetYmcaFixtures(sport.FiXiId.GetValueOrDefault(), maxRoundNumber);
                }
                catch (Exception)
                {
                    return View("SmissError");
                }

                if (dataSet == null ||
                    dataSet.Tables.Count <= 0)
                {
                    continue;
                }

                // Fixtures
                var fixtureQuery = from row in dataSet.Tables[0].AsEnumerable()
                    where row.Field<string>("hometeamname") == "GOODM8S" ||
                          row.Field<string>("awayteamname") == "GOODM8S"
                    select row;

                var fixtureRow = fixtureQuery.FirstOrDefault();

                if (fixtureRow == null) {
                    continue;
                }

                // Ladder
                var ladderQuery = from row in dataSet.Tables[1].AsEnumerable()
                    where row.Field<string>("teamname") == "GOODM8S"
                    select row;

                var ladderRow = ladderQuery.FirstOrDefault();

                if (ladderRow == null) {
                    continue;
                }

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
                    SportId = sport.Id,
                    Vs = vsRow?["teamname"].ToString() ?? string.Empty,
                    VsPoints = vsRow?["points"].ToString() ?? string.Empty,
                    VsPosition = vsRow != null ? Ordinal((int) vsRow["position"]) : string.Empty
                });
            }

            return View(summaries);
        }

        public ActionResult Vs(int sportId, int? round) {
            var sport = _sportService.Get(sportId);

            if (sport == null) {
                return HttpNotFound();
            }

            var maxRoundNumber = MaximumRoundNumber(
                sport.StartDate.GetValueOrDefault(),
                sport.WeekOffset.GetValueOrDefault());

            var roundNumber = round ?? maxRoundNumber;

            string vsTeamName;

            try {
                vsTeamName = Vs(sport.FiXiId.GetValueOrDefault(), roundNumber);
            }
            catch (Exception) {
                return View("SmissError");
            }

            ViewBag.Id = sport.Id;
            ViewBag.Name = sport.Name;
            ViewBag.RoundCount = sport.RoundCount;
            ViewBag.MaxRoundNumber = maxRoundNumber;
            ViewBag.RoundNumber = roundNumber;
            ViewBag.VsTeam = vsTeamName;

            var compareViewModel = new CompareViewModel();

            for (var i = 1; i <= roundNumber - 1; i++) {
                DataSet dataSet = null;

                try {
                    dataSet = fixtures.GetYmcaFixtures(sport.FiXiId.GetValueOrDefault(), i);
                }
                catch (Exception) {
                    return View("SmissError");
                }

                if (dataSet == null ||
                    dataSet.Tables.Count <= 0)
                {
                    continue;
                }

                var goodM8s = VsScore(dataSet, "GOODM8S");

                if (!goodM8s.Bye) {
                    if (!compareViewModel.Scores.ContainsKey(goodM8s.Name)) {
                        var score = new Dictionary<string, IList<CompareScore>> {
                            {
                                "GOODM8S", new List<CompareScore> {
                                    goodM8s.Score
                                }
                            }
                        };

                        compareViewModel.Scores.Add(goodM8s.Name, score);
                    }
                    else {
                        var scores = compareViewModel.Scores[goodM8s.Name];

                        if (!scores.ContainsKey("GOODM8S")) {
                            scores.Add("GOODM8S", new List<CompareScore> {
                                goodM8s.Score
                            });
                        }
                        else {
                            scores["GOODM8S"].Add(goodM8s.Score);
                        }
                    }
                }

                var vs = VsScore(dataSet, vsTeamName);

                if (vs.Bye) {
                    continue;
                }

                if (!compareViewModel.Scores.ContainsKey(vs.Name)) {
                    var score = new Dictionary<string, IList<CompareScore>> {
                        {
                            vsTeamName, new List<CompareScore> {
                                vs.Score
                            }
                        }
                    };

                    compareViewModel.Scores.Add(vs.Name, score);
                }
                else {
                    var scores = compareViewModel.Scores[vs.Name];

                    if (!scores.ContainsKey(vsTeamName)) {
                        scores.Add(vsTeamName, new List<CompareScore> {
                            vs.Score
                        });
                    }
                    else {
                        scores[vsTeamName].Add(vs.Score);
                    }
                }
            }

            return View(compareViewModel);
        }

        public ActionResult Overview(int sportId, bool? showPast) {
            var sport = _sportService.Get(sportId);

            if (sport == null) {
                return HttpNotFound();
            }

            ViewBag.Id = sport.Id;
            ViewBag.Name = sport.Name;
            ViewBag.CurrentDate = "1/1/1800";
            ViewBag.ShowPast = showPast.HasValue && showPast.Value;

            var maxRoundNumber = MaximumRoundNumber(
                sport.StartDate.GetValueOrDefault(),
                sport.WeekOffset.GetValueOrDefault());

            var firstRound = showPast.HasValue && showPast.Value
                ? 1
                : maxRoundNumber;

            var rows = new List<DataRow>();

            for (var i = firstRound; i <= MaxRounds - 1; i++) {
                DataSet dataSet = null;

                try {
                    dataSet = fixtures.GetYmcaFixtures(sport.FiXiId.GetValueOrDefault(), i);
                }
                catch (Exception)
                {
                    return View("SmissError");
                }

                if (dataSet == null ||
                    dataSet.Tables.Count <= 0)
                {
                    continue;
                }

                var vsQuery = from row in dataSet.Tables[0].AsEnumerable()
                    where row.Field<string>("hometeamname") == "GOODM8S" ||
                          row.Field<string>("awayteamname") == "GOODM8S"
                    select row;

                var goodm8s = vsQuery.FirstOrDefault();

                if (goodm8s == null)
                    continue;

                if (i == maxRoundNumber) {
                    ViewBag.CurrentDate = goodm8s["gamedate"];
                }

                if (goodm8s["hometeamname"].ToString() != "GOODM8S") {
                    // Make GOODM8S the home team
                    var awayTeamName = goodm8s["hometeamname"];
                    var awayTeamScore = goodm8s["hometeamscore"];

                    goodm8s["hometeamname"] = goodm8s["awayteamname"];
                    goodm8s["hometeamscore"] = goodm8s["awayteamscore"];
                    goodm8s["awayteamname"] = awayTeamName;
                    goodm8s["awayteamscore"] = awayTeamScore;
                }

                rows.Add(goodm8s);
            }

            return View(rows);
        }
    }
}