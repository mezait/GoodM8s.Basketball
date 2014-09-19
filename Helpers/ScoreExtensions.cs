using System;
using System.Text;
using System.Web.Mvc;
using GoodM8s.Basketball.ViewModels;

namespace GoodM8s.Basketball.Helpers {
    public static class ScoreExtensions {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public static MvcHtmlString Score(this HtmlHelper helper, CompareScore score) {
            var max = Math.Max(score.For, score.Against);

            var stringBuilder = new StringBuilder();

            if (max == score.For) {
                stringBuilder.AppendFormat("<strong>{0}</strong>", score.For);
            }
            else {
                stringBuilder.Append(score.For);
            }

            stringBuilder.Append(" vs ");

            if (max == score.Against) {
                stringBuilder.AppendFormat("<strong>{0}</strong>", score.Against);
            }
            else {
                stringBuilder.Append(score.Against);
            }

            return new MvcHtmlString(stringBuilder.ToString());
        }
    }
}