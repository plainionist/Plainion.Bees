using System;
using System.Collections.Generic;
using System.IO;

namespace Plainion.WhiteRabbit.Presentation.Reports
{
    public partial class RangeReport 
    {
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        public Dictionary<string, TimeSpan> Overview { get; set; }
        public Dictionary<DateTime, Dictionary<string, TimeSpan>> Details { get; set; }
        public bool IsComplete { get; set; }

        public void Generate(TextWriter writer)
        {
            writer.WriteLine($"<html><head><title>WhiteRabbit</title></head>");
            writer.WriteLine($"<body>");
            writer.WriteLine($"  <center><h2>{Begin.Date.ToShortDateString()} - {End.Date.ToShortDateString()}</h2></center>");

            writer.WriteLine($"  <table border='0' cellpadding='4' cellspacing='0'>");
            writer.WriteLine($"    <tr>");
            writer.WriteLine($"      <th>Comment</th>");
            writer.WriteLine($"      <th style='width:75px;' align='right'>Time</th>");
            writer.WriteLine($"    </tr>");

            var sum = new TimeSpan();
            foreach (string cat in Overview.Keys)
            {
                if (cat == "unknown")
                {
                    continue;
                }

                writer.WriteLine($"    <tr>");
                writer.WriteLine($"      <td>{cat}</td>");
                writer.WriteLine($"      <td align='right'>{Overview[cat].ToReportString()}</td>");
                writer.WriteLine($"    </tr>");

                sum += Overview[cat];
            }

            if (Overview["unknown"] != TimeSpan.Zero)
            {
                writer.WriteLine($"    <tr>");
                writer.WriteLine($"      <td><b>Unassigned</b></td>");
                writer.WriteLine($"      <td>{Overview["unknown"].ToReportString()}</td>");
                writer.WriteLine($"    </tr>");
            }

            writer.WriteLine($"    <tr>");
            writer.WriteLine($"      <td style='border-top:solid 2px #060'><b>Sum</b></td>");
            writer.WriteLine($"      <td style='border-top:solid 2px #060'>{sum.ToReportString()}</td>");
            writer.WriteLine($"    </tr>");
            writer.WriteLine($"  </table>");

            writer.WriteLine($"  <br/>");

            writer.WriteLine($"  <table cellpadding='4' cellspacing='0'>");
            writer.WriteLine($"    <tr>");
            writer.WriteLine($"      <th>Comment</th>");

            var sums = new Dictionary<DateTime, TimeSpan>();
            for (var date = Begin; date <= End; date = date.AddDays(1))
            {
                sums[date] = TimeSpan.Zero;

                writer.WriteLine($"      <th>{date.Day}.{date.Month}.</th>");
            }

            writer.WriteLine($"    </tr>");

            foreach (var cat in Overview.Keys)
            {
                writer.WriteLine($"    <tr>");
                writer.WriteLine($"      <td>{cat}</td>");

                for (var date = Begin; date <= End; date = date.AddDays(1))
                {
                    if (Details[date].ContainsKey(cat))
                    {
                        sums[date] += Details[date][cat];

                        writer.WriteLine($"      <td align='center'>{Details[date][cat].ToReportString()}</td>");
                    }
                    else
                    {
                        writer.WriteLine($"      <td align='center'>-</td>");
                    }
                }

                writer.WriteLine($"    </tr>");
            }

            writer.WriteLine($"    <tr>");
            writer.WriteLine($"      <td style='border-top:solid 2px #060'><b>Sum</b></td>");

            for (var date = Begin; date <= End; date = date.AddDays(1))
            {
                writer.WriteLine($"      <td align='center' style='border-top:solid 2px #060'>{sums[date].ToReportString()}</td>");
            }

            writer.WriteLine($"    </tr>");
            writer.WriteLine($"  </table>");

            if (!IsComplete)
            {
                writer.WriteLine($"  <font color='red'>The report is not complete because the data of the day is not complete.</font>");
            }

            writer.WriteLine($"</body></html>");
        }
    }
}
