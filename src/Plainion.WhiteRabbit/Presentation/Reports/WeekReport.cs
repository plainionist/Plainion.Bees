using System;
using System.Collections.Generic;

namespace Plainion.WhiteRabbit.Presentation.Reports
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "12.0.0.0")]
    public partial class WeekReport : ReportBase
    {
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write("\r\n\r\n<html>\r\n<head>\r\n    <title>WhiteRabbit</title>\r\n</head>\r\n<body>\r\n    <center>" +
                    "\r\n        <h2>\r\n            WhiteRabbit</h2>\r\n    </center>\r\n    <center>\r\n     " +
                    "   <h4>\r\n            ");
            this.Write(this.ToStringHelper.ToStringWithCulture(Begin.Date.ToShortDateString()));
            this.Write(" - ");
            this.Write(this.ToStringHelper.ToStringWithCulture(End.Date.ToShortDateString()));
            this.Write("\r\n        </h4>\r\n    </center>\r\n    <table border=\"0\" cellpadding=\"4\" cellspacing" +
                    "=\"0\">\r\n        <tr>\r\n            <th>\r\n                Comment\r\n            </t" +
                    "h>\r\n            <th>\r\n                Time\r\n            </th>\r\n        </tr>\r\n  " +
                    "      ");

            TimeSpan sum = new TimeSpan();
            foreach (string cat in Overview.Keys)
            {
                if (cat == "unknown")
                {
                    continue;
                }

                this.Write("        <tr>\r\n            <td>\r\n                ");
                this.Write(this.ToStringHelper.ToStringWithCulture(cat));
                this.Write("\r\n            </td>\r\n            <td>\r\n                ");
                this.Write(this.ToStringHelper.ToStringWithCulture(Overview[cat].ToReportString()));
                this.Write("\r\n            </td>\r\n        </tr>\r\n        ");

                sum += Overview[cat];
            }

            this.Write("        \r\n        ");

            if (Overview["unknown"] != TimeSpan.Zero)
            {
                this.Write("        <tr>\r\n            <td>\r\n                <b>Unassigned</b>\r\n            </" +
                        "td>\r\n            <td>\r\n                ");

                this.Write(this.ToStringHelper.ToStringWithCulture(Overview["unknown"].ToReportString()));

                this.Write("\r\n            </td>\r\n        </tr>\r\n        ");
            }

            this.Write("        <tr>\r\n            <td style=\"border-top:solid 2px #060\">\r\n               " +
                    " <b>Sum</b>\r\n            </td>\r\n            <td style=\"border-top:solid 2px #060" +
                    "\">\r\n                ");

            this.Write(this.ToStringHelper.ToStringWithCulture(sum.ToReportString()));

            this.Write("\r\n            </td>\r\n        </tr>\r\n    </table>\r\n    \r\n    <br />\r\n    \r\n    <ta" +
                    "ble cellpadding=\"4\" cellspacing=\"0\">\r\n        <tr>\r\n            <th>Comment</th" +
                    ">\r\n            ");

            var sums = new Dictionary<DateTime, TimeSpan>();
            for (var date = Begin; date <= End; date = date.AddDays(1))
            {
                sums[date] = TimeSpan.Zero;

                this.Write("            <th>");
                this.Write(this.ToStringHelper.ToStringWithCulture(date.Day));
                this.Write(".");
                this.Write(this.ToStringHelper.ToStringWithCulture(date.Month));
                this.Write(".</th>\r\n            ");
            }

            this.Write("        </tr>\r\n        ");

            foreach (var cat in Overview.Keys)
            {
                this.Write("            <tr>\r\n                <td>");
                this.Write(this.ToStringHelper.ToStringWithCulture(cat));
                this.Write("</td>\r\n        ");

                for (var date = Begin; date <= End; date = date.AddDays(1))
                {
                    if (Details[date].ContainsKey(cat))
                    {
                        sums[date] += Details[date][cat];

                        this.Write("                <td align=\"center\">");
                        this.Write(this.ToStringHelper.ToStringWithCulture(Details[date][cat].ToReportString()));
                        this.Write("</td>\r\n        ");
                    }
                    else
                    {
                        this.Write("               <td align=\"center\">-</td>\r\n        ");
                    }

                    this.Write("                    \r\n        ");
                }

                this.Write("            </tr>\r\n        ");
            }

            this.Write("        \r\n        <tr>\r\n            <td style=\"border-top:solid 2px #060\">\r\n     " +
                    "           <b>Sum</b>\r\n            </td>\r\n        ");

            for (var date = Begin; date <= End; date = date.AddDays(1))
            {
                this.Write("            <td align=\"center\" style=\"border-top:solid 2px #060\">\r\n              " +
                        "  ");
                this.Write(this.ToStringHelper.ToStringWithCulture(sums[date].ToReportString()));
                this.Write("\r\n            </td>\r\n        ");
            }

            this.Write("        </tr>\r\n    </table>\r\n    \r\n    <br />\r\n    \r\n    ");

            if (!IsComplete)
            {
                this.Write("    <font color=\"red\">The report is not complete because the data of the day is n" +
                        "ot complete.</font>\r\n    ");
            }

            this.Write("</body>\r\n</html>\r\n\r\n");
            return this.GenerationEnvironment.ToString();
        }

        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        public Dictionary<string, TimeSpan> Overview { get; set; }
        public Dictionary<DateTime, Dictionary<string, TimeSpan>> Details { get; set; }
        public bool IsComplete { get; set; }
    }
}
