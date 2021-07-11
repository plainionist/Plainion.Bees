using System;
using System.Collections.Generic;

namespace Plainion.WhiteRabbit.Presentation.Reports
{
    public partial class DayReport : ReportBase
    {
        public virtual string TransformText()
        {
            this.Write("\r\n\r\n<html>\r\n<head>\r\n    <title>WhiteRabbit</title>\r\n</head>\r\n<body>\r\n    <center>" +
                    "\r\n        <h2>\r\n            WhiteRabbit</h2>\r\n    </center>\r\n    <center>\r\n     " +
                    "   <h4>\r\n            ");

            this.Write(this.ToStringHelper.ToStringWithCulture(Day.Date.ToShortDateString()));

            this.Write("</h4>\r\n    </center>\r\n    <table border=\"0\" cellpadding=\"4\" cellspacing=\"0\">\r\n   " +
                    "     <tr>\r\n            <th>\r\n                Comment\r\n            </th>\r\n " +
                    "           <th style=\"width:75px;\">Time</th>\r\n        </tr>\r\n        ");


            TimeSpan sum = new TimeSpan();
            foreach (string comment in Data.Keys)
            {
                if (comment == "unknown")
                {
                    continue;
                }

                sum += Data[comment];

                this.Write("        <tr>\r\n            <td>");
                this.Write(this.ToStringHelper.ToStringWithCulture(comment));
                this.Write("</td>\r\n            <td align=\"right\">");
                this.Write(this.ToStringHelper.ToStringWithCulture(Data[comment].ToReportString()));
                this.Write("</td>\r\n        </tr>\r\n        ");
            }

            this.Write("        \r\n        ");

            if (Data["unknown"] != TimeSpan.Zero)
            {
                sum += Data["unknown"];

                this.Write("        <tr>\r\n            <td>unknown</td>\r\n            <td align=\"right\">");
                this.Write(this.ToStringHelper.ToStringWithCulture(Data["unknown"].ToReportString()));
                this.Write("</td>\r\n        </tr>\r\n        ");
            }

            this.Write("        <tr>\r\n            <td style=\"border-top:solid 2px #060\">\r\n               " +
                    " <b>Sum</b>\r\n            </td>\r\n            <td  align=\"right\" style=\"border-top" +
                    ":solid 2px #060\">\r\n                ");

            this.Write(this.ToStringHelper.ToStringWithCulture(sum.ToReportString()));

            this.Write("\r\n            </td>\r\n        </tr>\r\n    </table>\r\n    \r\n    <br />\r\n    \r\n    ");

            if (!IsComplete)
            {
                this.Write("    <font color=\"red\">The report is not complete because the data of the day is n" +
                        "ot complete.</font>\r\n");
            }

            this.Write("</body>\r\n</html>\r\n\r\n");
            return this.GenerationEnvironment.ToString();
        }

        public DateTime Day { get; set; }
        public Dictionary<string, TimeSpan> Data { get; set; }
        public bool IsComplete { get; set; }
    }
}
