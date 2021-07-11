using System;
using System.Collections.Generic;
using System.IO;

namespace Plainion.WhiteRabbit.Presentation.Reports
{
    public partial class WeekReport 
    {
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        public Dictionary<string, TimeSpan> Overview { get; set; }
        public Dictionary<DateTime, Dictionary<string, TimeSpan>> Details { get; set; }
        public bool IsComplete { get; set; }

        public void Generate(TextWriter writer)
        {
            writer.Write("\r\n\r\n<html>\r\n<head>\r\n    <title>WhiteRabbit</title>\r\n</head>\r\n<body>\r\n    <center>" +
                    "\r\n        <h2>\r\n            WhiteRabbit</h2>\r\n    </center>\r\n    <center>\r\n     " +
                    "   <h4>\r\n            ");
            writer.Write(Begin.Date.ToShortDateString());
            writer.Write(" - ");
            writer.Write(End.Date.ToShortDateString());
            writer.Write("\r\n        </h4>\r\n    </center>\r\n    <table border=\"0\" cellpadding=\"4\" cellspacing" +
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

                writer.Write("        <tr>\r\n            <td>\r\n                ");
                writer.Write(cat);
                writer.Write("\r\n            </td>\r\n            <td>\r\n                ");
                writer.Write(Overview[cat].ToReportString());
                writer.Write("\r\n            </td>\r\n        </tr>\r\n        ");

                sum += Overview[cat];
            }

            writer.Write("        \r\n        ");

            if (Overview["unknown"] != TimeSpan.Zero)
            {
                writer.Write("        <tr>\r\n            <td>\r\n                <b>Unassigned</b>\r\n            </" +
                        "td>\r\n            <td>\r\n                ");

                writer.Write(Overview["unknown"].ToReportString());

                writer.Write("\r\n            </td>\r\n        </tr>\r\n        ");
            }

            writer.Write("        <tr>\r\n            <td style=\"border-top:solid 2px #060\">\r\n               " +
                    " <b>Sum</b>\r\n            </td>\r\n            <td style=\"border-top:solid 2px #060" +
                    "\">\r\n                ");

            writer.Write(sum.ToReportString());

            writer.Write("\r\n            </td>\r\n        </tr>\r\n    </table>\r\n    \r\n    <br />\r\n    \r\n    <ta" +
                    "ble cellpadding=\"4\" cellspacing=\"0\">\r\n        <tr>\r\n            <th>Comment</th" +
                    ">\r\n            ");

            var sums = new Dictionary<DateTime, TimeSpan>();
            for (var date = Begin; date <= End; date = date.AddDays(1))
            {
                sums[date] = TimeSpan.Zero;

                writer.Write("            <th>");
                writer.Write(date.Day.ToString());
                writer.Write(".");
                writer.Write(date.Month.ToString());
                writer.Write(".</th>\r\n            ");
            }

            writer.Write("        </tr>\r\n        ");

            foreach (var cat in Overview.Keys)
            {
                writer.Write("            <tr>\r\n                <td>");
                writer.Write(cat);
                writer.Write("</td>\r\n        ");

                for (var date = Begin; date <= End; date = date.AddDays(1))
                {
                    if (Details[date].ContainsKey(cat))
                    {
                        sums[date] += Details[date][cat];

                        writer.Write("                <td align=\"center\">");
                        writer.Write(Details[date][cat].ToReportString());
                        writer.Write("</td>\r\n        ");
                    }
                    else
                    {
                        writer.Write("               <td align=\"center\">-</td>\r\n        ");
                    }

                    writer.Write("                    \r\n        ");
                }

                writer.Write("            </tr>\r\n        ");
            }

            writer.Write("        \r\n        <tr>\r\n            <td style=\"border-top:solid 2px #060\">\r\n     " +
                    "           <b>Sum</b>\r\n            </td>\r\n        ");

            for (var date = Begin; date <= End; date = date.AddDays(1))
            {
                writer.Write("            <td align=\"center\" style=\"border-top:solid 2px #060\">\r\n              " +
                        "  ");
                writer.Write(sums[date].ToReportString());
                writer.Write("\r\n            </td>\r\n        ");
            }

            writer.Write("        </tr>\r\n    </table>\r\n    \r\n    <br />\r\n    \r\n    ");

            if (!IsComplete)
            {
                writer.Write("    <font color=\"red\">The report is not complete because the data of the day is n" +
                        "ot complete.</font>\r\n    ");
            }

            writer.Write("</body>\r\n</html>\r\n\r\n");
        }
    }
}
