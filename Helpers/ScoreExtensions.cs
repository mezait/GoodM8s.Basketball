using System;
using System.Globalization;
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

            stringBuilder.AppendFormat(max == score.For
                ? "<span class=\"for win\">{0}</span>"
                : "<span class=\"for\">{0}</span>", score.For);

            stringBuilder.Append(" vs ");

            stringBuilder.AppendFormat(max == score.Against
                ? "<span class=\"against win\">{0}</span>"
                : "<span class=\"against\">{0}</span>", score.Against);

            return new MvcHtmlString(stringBuilder.ToString());
        }

        public static MvcHtmlString Max(this HtmlHelper helper, double a, double b) {
            return a == b
                ? new MvcHtmlString(String.Format("<span class=\"max\">{0}</span>", a.ToString("0.00")))
                : new MvcHtmlString(a.ToString("0.00"));
        }

        public static MvcHtmlString Max(this HtmlHelper helper, decimal a, decimal b) {
            return a == b
                ? new MvcHtmlString(String.Format("<span class=\"max\">{0}</span>", a.ToString("P")))
                : new MvcHtmlString(a.ToString("P"));
        }

        public static MvcHtmlString Max(this HtmlHelper helper, int a, int b) {
            return a == b
                ? new MvcHtmlString(String.Format("<span class=\"max\">{0}</span>", a))
                : new MvcHtmlString(a.ToString(CultureInfo.InvariantCulture));
        }

        public static MvcHtmlString Win(this HtmlHelper helper, int a, int b)
        {
            return a > 0 && a >= b
                ? new MvcHtmlString(String.Format("<span class=\"win\">{0}</span>", a))
                : new MvcHtmlString(a.ToString(CultureInfo.InvariantCulture));
        }
    }
}