﻿using System.Collections.Generic;
using GoodM8s.Basketball.Models;
using GoodM8s.Basketball.ViewModels;
using GoodM8s.Core.Services;
using Orchard;
using Orchard.ContentManagement;

namespace GoodM8s.Basketball.Services {
    public class SportService : BaseService<SportPart, SportPartRecord>, ISportService {
        public SportService(IOrchardServices orchardServices) : base(orchardServices) {}

        public IEnumerable<SportPart> GetBySeason(int seasonId, bool activeOnly) {
            var query = activeOnly
                ? GetQuery().Where(s => s.SeasonId == seasonId && s.IsActive)
                : GetQuery().Where(s => s.SeasonId == seasonId);

            return query.List<SportPart>();
        }

        public void UpdateForContentItem(IContent item, SportEditViewModel model) {
            var sportPart = item.As<SportPart>();

            sportPart.FiXiId = model.FiXiId;
            sportPart.Name = model.Name;
            sportPart.SeasonId = model.SeasonId;
            sportPart.StartDate = model.StartDate;
            sportPart.WeekOffset = model.WeekOffset;
            sportPart.RoundCount = model.RoundCount;
            sportPart.IsActive = model.IsActive;
        }
    }
}