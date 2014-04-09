using System.Collections.Generic;
using System.Linq;

namespace GoodM8s.Basketball.ViewModels {
    public class DynamicIndexViewModel {
        public dynamic Pager { get; set; }
        public IList<dynamic> List { get; set; }

        public DynamicIndexViewModel(IEnumerable<dynamic> list, dynamic pager) {
            Pager = pager;
            List = list.ToArray();
        }
    }
}