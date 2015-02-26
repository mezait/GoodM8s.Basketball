using System.Collections.Generic;
using GoodM8s.Basketball.Models;
using GoodM8s.Basketball.ViewModels;
using GoodM8s.Core.Services;
using Orchard.ContentManagement;

namespace GoodM8s.Basketball.Services {
    public interface IGameService : IBaseService<GamePart, GamePartRecord> {
        IEnumerable<GamePart> GetBySport(int sportId);
        void UpdateForContentItem(IContent item, GameEditViewModel model);
    }
}