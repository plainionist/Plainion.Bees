using System;
using System.Data;
using System.Windows.Forms;

namespace Plainion.WhiteRabbit.Model
{
    public class DayEntry
    {
        public DayEntry()
        {
            Begin = null;
            End = null;
            Category = -1;
            Comment = null;
        }

        public static DayEntry Parse(DataRow row, DataTable categories)
        {
            DayEntry entry = new DayEntry();
            if (row == null)
            {
                return entry;
            }

            if (!row["Begin"].IsEmpty())
            {
                entry.Begin = DateTime.Parse((string)row["Begin"]);
            }

            if (!row["End"].IsEmpty())
            {
                entry.End = DateTime.Parse((string)row["End"]);
            }

            if (!row["Category"].IsEmpty())
            {
                int pos = 0;
                foreach (DataRow r in categories.Rows)
                {
                    if ((string)r[0] == (string)row["Category"])
                    {
                        break;
                    }
                    ++pos;
                }

                entry.Category = pos;
                entry.CategoryString = row["Category"].ToString();
            }

            if (row.Table.Columns.Contains("Comment") && !row["Comment"].IsEmpty())
            {
                entry.Comment = (string)row["Comment"];
            }

            if (!row["Task"].IsEmpty())
            {
                entry.Comment = (string)row["Task"];
            }

            return entry;
        }

        public TimeSpan? GetUsedTime()
        {
            if (Begin == null || End == null)
            {
                return null;
            }

            return GetDuration(Begin.Value, End.Value);
        }

        public static TimeSpan GetDuration(DateTime begin, DateTime end)
        {
            TimeSpan rawDiff;
            if (end >= begin)
            {
                rawDiff = end - begin;
            }
            else
            {
                rawDiff = (DateTime.Today.AddDays(1) - begin) + end.TimeOfDay;
            }
            return rawDiff;
        }

        public DateTime? Begin { get; set; }

        public DateTime? End { get; set; }

        public int Category { get; set; }

        /// <summary>(
        /// Contains the string originally parsed from the data row.
        /// </summary>
        public string CategoryString { get; private set; }

        public string Comment { get; set; }

    }
}
