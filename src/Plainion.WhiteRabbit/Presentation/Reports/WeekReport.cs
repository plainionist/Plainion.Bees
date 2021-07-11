using System;
using System.Collections.Generic;

namespace Plainion.WhiteRabbit.Presentation.Reports
{
    public partial class WeekReport : ReportBase
    {
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        public Dictionary<string, TimeSpan> Overview { get; set; }
        public Dictionary<DateTime, Dictionary<string, TimeSpan>> Details { get; set; }
        public bool IsComplete { get; set; }

        public virtual string TransformText()
        {
            Write("\r\n\r\n<html>\r\n<head>\r\n    <title>WhiteRabbit</title>\r\n</head>\r\n<body>\r\n    <center>" +
                    "\r\n        <h2>\r\n            WhiteRabbit</h2>\r\n    </center>\r\n    <center>\r\n     " +
                    "   <h4>\r\n            ");
            Write(ToStringWithCulture(Begin.Date.ToShortDateString()));
            Write(" - ");
            Write(ToStringWithCulture(End.Date.ToShortDateString()));
            Write("\r\n        </h4>\r\n    </center>\r\n    <table border=\"0\" cellpadding=\"4\" cellspacing" +
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

                Write("        <tr>\r\n            <td>\r\n                ");
                Write(ToStringWithCulture(cat));
                Write("\r\n            </td>\r\n            <td>\r\n                ");
                Write(ToStringWithCulture(Overview[cat].ToReportString()));
                Write("\r\n            </td>\r\n        </tr>\r\n        ");

                sum += Overview[cat];
            }

            Write("        \r\n        ");

            if (Overview["unknown"] != TimeSpan.Zero)
            {
                Write("        <tr>\r\n            <td>\r\n                <b>Unassigned</b>\r\n            </" +
                        "td>\r\n            <td>\r\n                ");

                Write(ToStringWithCulture(Overview["unknown"].ToReportString()));

                Write("\r\n            </td>\r\n        </tr>\r\n        ");
            }

            Write("        <tr>\r\n            <td style=\"border-top:solid 2px #060\">\r\n               " +
                    " <b>Sum</b>\r\n            </td>\r\n            <td style=\"border-top:solid 2px #060" +
                    "\">\r\n                ");

            Write(ToStringWithCulture(sum.ToReportString()));

            Write("\r\n            </td>\r\n        </tr>\r\n    </table>\r\n    \r\n    <br />\r\n    \r\n    <ta" +
                    "ble cellpadding=\"4\" cellspacing=\"0\">\r\n        <tr>\r\n            <th>Comment</th" +
                    ">\r\n            ");

            var sums = new Dictionary<DateTime, TimeSpan>();
            for (var date = Begin; date <= End; date = date.AddDays(1))
            {
                sums[date] = TimeSpan.Zero;

                Write("            <th>");
                Write(ToStringWithCulture(date.Day));
                Write(".");
                Write(ToStringWithCulture(date.Month));
                Write(".</th>\r\n            ");
            }

            Write("        </tr>\r\n        ");

            foreach (var cat in Overview.Keys)
            {
                Write("            <tr>\r\n                <td>");
                Write(ToStringWithCulture(cat));
                Write("</td>\r\n        ");

                for (var date = Begin; date <= End; date = date.AddDays(1))
                {
                    if (Details[date].ContainsKey(cat))
                    {
                        sums[date] += Details[date][cat];

                        Write("                <td align=\"center\">");
                        Write(ToStringWithCulture(Details[date][cat].ToReportString()));
                        Write("</td>\r\n        ");
                    }
                    else
                    {
                        Write("               <td align=\"center\">-</td>\r\n        ");
                    }

                    Write("                    \r\n        ");
                }

                Write("            </tr>\r\n        ");
            }

            Write("        \r\n        <tr>\r\n            <td style=\"border-top:solid 2px #060\">\r\n     " +
                    "           <b>Sum</b>\r\n            </td>\r\n        ");

            for (var date = Begin; date <= End; date = date.AddDays(1))
            {
                Write("            <td align=\"center\" style=\"border-top:solid 2px #060\">\r\n              " +
                        "  ");
                Write(ToStringWithCulture(sums[date].ToReportString()));
                Write("\r\n            </td>\r\n        ");
            }

            Write("        </tr>\r\n    </table>\r\n    \r\n    <br />\r\n    \r\n    ");

            if (!IsComplete)
            {
                Write("    <font color=\"red\">The report is not complete because the data of the day is n" +
                        "ot complete.</font>\r\n    ");
            }

            Write("</body>\r\n</html>\r\n\r\n");
            return GenerationEnvironment.ToString();
        }
    }
}
