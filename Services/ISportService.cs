using System.Collections.Generic;
using GoodM8s.Basketball.Models;
using GoodM8s.Basketball.ViewModels;
using Orchard.ContentManagement;

namespace GoodM8s.Basketball.Services {
    public interface ISportService : IBaseService<SportPart, SportPartRecord> {
        IEnumerable<SportPart> GetBySeason(int seasonId);
        void UpdateForContentItem(IContent item, SportEditViewModel model);
    }
}