﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using Plainion.WhiteRabbit.Model;
using Plainion.WhiteRabbit.Presentation.Reports;
using Plainion.WhiteRabbit.Properties;
using Plainion.WhiteRabbit.View;

namespace Plainion.WhiteRabbit.Presentation
{
    public class Controller
    {
        private Recorder myRecorder = null;

        public Controller(Type initialView)
        {
            if (Settings.Default.DBStore.IsNullOrTrimmedEmpty())
            {
                Settings.Default.DBStore = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "WhiteRabbit");
                Settings.Default.Save();
            }

            Database = new Database(Settings.Default.DBStore);

            MainView = Activator.CreateInstance(initialView, this) as IView;
        }

        public IView MainView
        {
            get;
            private set;
        }

        public IView TimerView
        {
            get;
            private set;
        }

        public DataTable CurrentDayData
        {
            get;
            private set;
        }

        public DateTime CurrentDay
        {
            get;
            private set;
        }

        public void ChangeDay(DateTime day)
        {
            CurrentDay = day;
            CurrentDayData = GetTableByDay(day);
        }

        public void DeleteDayEntry(int idx)
        {
            CurrentDayData.Rows.RemoveAt(idx);
            CurrentDayData.AcceptChanges();

            Database.StoreTable(CurrentDayData);
        }

        public DataTable GetTableByDay(DateTime day)
        {
            var table = Database.LoadTable(day);
            if (table == null)
            {
                table = Database.CreateTable(day);
            }

            return table;
        }

        public void StartTimeMeasurement()
        {
            DayEntry entry = null;
            if (CurrentDayData.Rows.Count > 0)
            {
                DataRow dr = CurrentDayData.Rows[CurrentDayData.Rows.Count - 1];

                entry = DayEntry.Parse(dr);
            }
            else
            {
                entry = new DayEntry();
            }

            (MainView as Form).Hide();

            if (TimerView == null)
            {
                TimerView = new SlimForm(this);
            }
            (TimerView as SlimForm).Show();
            (TimerView as SlimForm).Start(entry);

            myRecorder = new Recorder(TimerView.Channel);

            if (entry.End == null)
            {
                if (null == entry.Begin)
                {
                    myRecorder.Start();
                }
                else
                {
                    myRecorder.Start(entry.Begin.Value);
                }
            }
            else
            {
                myRecorder.Start();
            }
        }

        public void StopTimeMeasurement(string comment)
        {
            (TimerView as Form).Hide();
            (MainView as Form).Show();

            myRecorder.Stop();

            // if last record is not yet completed
            // -> complete this one
            DataRow dr = null;
            if (CurrentDayData.Rows.Count > 0)
            {
                dr = CurrentDayData.Rows[CurrentDayData.Rows.Count - 1];
            }

            if (dr != null && dr["End"].IsEmpty())
            {
                dr["End"] = myRecorder.StopTime.ToShortTimeString();

                dr[ColumnNames.COMMENT] = comment;

                if (dr["Begin"].IsEmpty())
                {
                    dr["Begin"] = myRecorder.StartTime.ToShortTimeString();
                }
            }
            else
            {
                // add recorded data to table
                dr = CurrentDayData.NewRow();
                CurrentDayData.Rows.Add(dr);
                CurrentDayData.AcceptChanges();

                dr["Begin"] = myRecorder.StartTime.ToShortTimeString();
                dr["End"] = myRecorder.StopTime.ToShortTimeString();
                dr[ColumnNames.COMMENT] = comment;
            }

            Database.StoreTable(CurrentDayData);

            myRecorder = null;
        }

        /// <summary>
        /// Generates the report for the given day and returns the
        /// URL to the generated report.
        /// </summary>
        public string GenerateDayReport(DateTime day)
        {
            string file = Path.GetTempFileName();

            bool isComplete;
            var data = GetDetails(day, out isComplete);

            // generate index.html
            var report = new DayReport();
            report.Day = day;
            report.Data = data;
            report.IsComplete = isComplete;
            File.WriteAllText(file, report.TransformText());

            return file;
        }

        /// <summary>
        /// Generates the report for the week specified by the day and returns the
        /// URL to the generated report.
        /// </summary>
        public string GenerateWeekReport(DateTime day)
        {
            string file = Path.GetTempFileName();

            var overview = new Dictionary<string, TimeSpan>();
            overview["unknown"] = new TimeSpan();

            var details = new Dictionary<DateTime, Dictionary<string, TimeSpan>>();

            bool isAllComplete = true;

            var begin = day.GetBeginOfWeek();
            var end = day.GetEndOfWeek();
            for (var date = begin; date <= end; date = date.AddDays(1))
            {
                bool isComplete;
                details[date] = GetDetails(date, out isComplete);

                // handle overview
                if (!isComplete)
                {
                    isAllComplete = false;
                }

                foreach (var entry in details[date])
                {
                    AddTimeSpan(overview, entry.Key, entry.Value);
                }
            }

            // generate index.html
            var report = new WeekReport();
            report.Begin = begin;
            report.End = end;
            report.Overview = overview;
            report.Details = details;
            report.IsComplete = isAllComplete;
            File.WriteAllText(file, report.TransformText());

            return file;
        }

        public Database Database
        {
            get;
            private set;
        }

        private Dictionary<string, TimeSpan> GetDetails(DateTime day, out bool isComplete)
        {
            var data = new Dictionary<string, TimeSpan>();
            data["unknown"] = new TimeSpan();

            isComplete = true;

            DataTable table = Database.LoadTable(day);
            foreach (DataRow row in table.Rows)
            {
                var entry = DayEntry.Parse(row);
                var usedTime = entry.GetUsedTime();
                if (usedTime == null)
                {
                    isComplete = false;
                    continue;
                }

                var comment = entry.Comment ?? "unknown";

                AddTimeSpan(data, comment, usedTime.Value);
            }

            return data;
        }

        private void AddTimeSpan(Dictionary<string, TimeSpan> data, string comment, TimeSpan time)
        {
            if (!data.ContainsKey(comment))
            {
                data[comment] = time;
            }
            else
            {
                data[comment] += time;
            }
        }
    }
}
