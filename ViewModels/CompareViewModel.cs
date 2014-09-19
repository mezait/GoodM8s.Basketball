using System.Collections.Generic;

namespace GoodM8s.Basketball.ViewModels
{
    public class CompareScore
    {
        public int For { get; set; }
        public int Against { get; set; }
    }

    public class CompareScoreVs
    {
        public bool Bye { get; set; }
        public string Name { get; set; }
        public CompareScore Score { get; set; }
    }

    public class CompareViewModel
    {
        public CompareViewModel()
        {
            Scores = new Dictionary<string, IDictionary<string, IList<CompareScore>>>();
        }

        public IDictionary<string, IDictionary<string, IList<CompareScore>>> Scores { get; set; }
    }
}