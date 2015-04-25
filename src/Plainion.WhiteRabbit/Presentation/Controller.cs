using System;
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

        public Controller( Type initialView )
        {
            if ( Settings.Default.DBStore.IsNullOrTrimmedEmpty() )
            {
                Settings.Default.DBStore = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.Personal ), "WhiteRabbit" );
                Settings.Default.Save();
            }

            Database = new Database( Settings.Default.DBStore );

            LoadCategories();

            MainView = Activator.CreateInstance( initialView, this ) as IView;
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

        public DataTable Categories
        {
            get;
            private set;
        }

        public void LoadCategories()
        {
            Categories = Database.LoadCategories();
        }

        public void RenameCategory( string oldName, string newName )
        {
            if ( !RenameCategory( Categories.Rows, Categories.Columns[ 0 ], oldName, newName ) )
            {
                throw new Exception( "Category not found: " + oldName );
            }

            Database.StoreCategories( Categories );

            foreach ( var day in Database.GetAllDays() )
            {
                if ( RenameCategory( day.Rows, day.Columns[ "Category" ], oldName, newName ) )
                {
                    Database.StoreTable( day );
                }
            }
        }

        private bool RenameCategory( DataRowCollection rows, DataColumn col, string oldName, string newName )
        {
            var found = false;
            foreach ( DataRow row in rows )
            {
                if ( (string)row[ col ] == oldName )
                {
                    row[ col ] = newName;
                    found = true;
                }
            }

            return found;
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

        public void ChangeDay( DateTime day )
        {
            CurrentDay = day;
            CurrentDayData = GetTableByDay( day );
        }

        public void DeleteDayEntry( int idx )
        {
            CurrentDayData.Rows.RemoveAt( idx );
            CurrentDayData.AcceptChanges();

            Database.StoreTable( CurrentDayData );
        }

        public DataTable GetTableByDay( DateTime day )
        {
            var table = Database.LoadTable( day );
            if ( table == null )
            {
                table = Database.CreateTable( day );
            }

            return table;
        }

        public TimeSpan GetJitterSum()
        {
            TimeSpan allJitter = new TimeSpan();

            foreach ( DataRow row in CurrentDayData.Rows )
            {
                string s = row[ 2 ] as string;
                if ( !s.IsNullOrTrimmedEmpty() )
                {
                    TimeSpan result;
                    if ( TimeSpan.TryParse( s.Trim(), out result ) )
                    {
                        allJitter += result;
                    }
                }
            }

            return allJitter;
        }

        public void StartTimeMeasurement()
        {
            DayEntry entry = null;
            if ( CurrentDayData.Rows.Count > 0 )
            {
                DataRow dr = CurrentDayData.Rows[ CurrentDayData.Rows.Count - 1 ];

                entry = DayEntry.Parse( dr, Categories );
            }
            else
            {
                entry = new DayEntry();
            }

            ( MainView as Form ).Hide();

            if ( TimerView == null )
            {
                TimerView = new SlimForm( this );
            }
            ( TimerView as SlimForm ).Show();
            ( TimerView as SlimForm ).Start( entry );

            myRecorder = new Recorder( TimerView.Channel );

            if ( entry.End == null )
            {
                if ( null == entry.Begin )
                {
                    myRecorder.Start();
                }
                else
                {
                    myRecorder.Start( entry.Begin.Value, entry.Jitter );
                }
            }
            else
            {
                myRecorder.Start();
            }
        }

        public void PauseTimeMeasurement( bool on )
        {
            if ( on )
            {
                myRecorder.Pause();
            }
            else
            {
                myRecorder.Start();
            }
        }

        public void StopTimeMeasurement( int category, string task )
        {
            ( TimerView as Form ).Hide();
            ( MainView as Form ).Show();

            myRecorder.Stop();

            // if last record is not yet completed
            // -> complete this one
            DataRow dr = null;
            if ( CurrentDayData.Rows.Count > 0 )
            {
                dr = CurrentDayData.Rows[ CurrentDayData.Rows.Count - 1 ];
            }

            if ( dr != null && dr[ "End" ].IsEmpty() )
            {
                dr[ "End" ] = myRecorder.StopTime.ToShortTimeString();
                dr[ "Jitter" ] = "" + myRecorder.Jitter.Hours + ":" + myRecorder.Jitter.Minutes;

                if ( category != -1 )
                {
                    dr[ "Category" ] = Categories.Rows[ category ][ 0 ] as string;
                }
                dr[ "Task" ] = task;

                if ( dr[ "Begin" ].IsEmpty() )
                {
                    dr[ "Begin" ] = myRecorder.StartTime.ToShortTimeString();
                }
            }
            else
            {
                // add recorded data to table
                dr = CurrentDayData.NewRow();
                CurrentDayData.Rows.Add( dr );
                CurrentDayData.AcceptChanges();

                dr[ "Begin" ] = myRecorder.StartTime.ToShortTimeString();
                dr[ "End" ] = myRecorder.StopTime.ToShortTimeString();
                dr[ "Jitter" ] = "" + myRecorder.Jitter.Hours + ":" + myRecorder.Jitter.Minutes;
                if ( category != -1 )
                {
                    dr[ "Category" ] = Categories.Rows[ category ][ 0 ] as string;
                }
                dr[ "Task" ] = task;
            }

            Database.StoreTable( CurrentDayData );

            myRecorder = null;
        }

        /// <summary>
        /// Generates the report for the given day and returns the
        /// URL to the generated report.
        /// </summary>
        public string GenerateDayReport( DateTime day )
        {
            string file = Path.GetTempFileName();

            bool isComplete;
            var data = GetDayDetails( day, out isComplete );

            // generate index.html
            var report = new DayReport();
            report.Day= day;
            report.Data=data;
            report.IsComplete = isComplete;
            File.WriteAllText( file, report.TransformText() );

            return file;
        }

        /// <summary>
        /// Generates the report for the week specified by the day and returns the
        /// URL to the generated report.
        /// </summary>
        public string GenerateWeekReport( DateTime day )
        {
            string file = Path.GetTempFileName();

            var overview = new Dictionary<string, TimeSpan>();
            overview[ "unknown" ] = new TimeSpan();

            var details = new Dictionary<DateTime, Dictionary<string, TimeSpan>>();

            bool isAllComplete = true;

            var begin = day.GetBeginOfWeek();
            var end = day.GetEndOfWeek();
            for ( var date = begin; date <= end; date = date.AddDays( 1 ) )
            {
                bool isComplete;
                details[ date ] = GetDayOverview( date, out isComplete );

                // handle overview
                if ( !isComplete )
                {
                    isAllComplete = false;
                }

                foreach ( var entry in details[ date ] )
                {
                    AddTimeSpan( overview, entry.Key, entry.Value );
                }
            }

            // generate index.html
            var report = new WeekReport();
            report.Begin = begin;
            report.End = end;
            report.Overview = overview;
            report.Details = details;
            report.IsComplete = isAllComplete;
            File.WriteAllText( file, report.TransformText() );

            return file;
        }

        public Database Database
        {
            get;
            private set;
        }

        private Dictionary<string, Dictionary<string, TimeSpan>> GetDayDetails( DateTime day, out bool isComplete )
        {
            var data = new Dictionary<string, Dictionary<string, TimeSpan>>();
            data[ "unknown" ] = new Dictionary<string, TimeSpan>();

            isComplete = true;

            DataTable table = Database.LoadTable( day );
            foreach ( DataRow row in table.Rows )
            {
                var entry = DayEntry.Parse( row, Categories );
                var usedTime = entry.GetUsedTime();
                if ( usedTime == null )
                {
                    isComplete = false;
                    continue;
                }

                string cat = ( entry.CategoryString != null ? entry.CategoryString : "unknown" );
                if ( !data.ContainsKey( cat ) )
                {
                    data[ cat ] = new Dictionary<string, TimeSpan>();
                }

                string task = ( entry.Task != null ? entry.Task : "unknown" );
                if ( !data[ cat ].ContainsKey( task ) )
                {
                    data[ cat ][ task ] = usedTime.Value;
                }
                else
                {
                    data[ cat ][ task ] += usedTime.Value;
                }
            }

            return data;
        }

        private Dictionary<string, TimeSpan> GetDayOverview( DateTime day, out bool isComplete )
        {
            var data = new Dictionary<string, TimeSpan>();
            data[ "unknown" ] = new TimeSpan();

            isComplete = true;

            DataTable table = Database.LoadTable( day );
            foreach ( DataRow row in table.Rows )
            {
                var entry = DayEntry.Parse( row, Categories );
                var usedTime = entry.GetUsedTime();
                if ( usedTime == null )
                {
                    isComplete = false;
                    continue;
                }

                string cat = ( entry.CategoryString != null ? entry.CategoryString : "unknown" );

                AddTimeSpan( data, cat, usedTime.Value );
            }

            return data;
        }

        private void AddTimeSpan( Dictionary<string, TimeSpan> data, string category, TimeSpan time )
        {
            if ( !data.ContainsKey( category ) )
            {
                data[ category ] = time;
            }
            else
            {
                data[ category ] += time;
            }
        }
    }
}
