using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Plainion.WhiteRabbit.Presentation
{
    public class DayEntry
    {
        public DayEntry()
        {
            Begin = null;
            End = null;
            Jitter = TimeSpan.Zero;
            Category = -1;
            Task = null;
        }

        public static DayEntry Parse( DataRow row, DataTable categories )
        {
            DayEntry entry = new DayEntry();
            if ( row == null )
            {
                return entry;
            }

            if ( !row[ "Begin" ].IsEmpty() )
            {
                entry.Begin = DateTime.Parse( (string)row[ "Begin" ] );
            }

            if ( !row[ "End" ].IsEmpty() )
            {
                entry.End = DateTime.Parse( (string)row[ "End" ] );
            }

            if ( !row[ "Jitter" ].IsEmpty() )
            {
                entry.Jitter = TimeSpan.Parse( (string)row[ "Jitter" ] );
            }

            if ( !row[ "Category" ].IsEmpty() )
            {
                int pos = 0;
                foreach ( DataRow r in categories.Rows )
                {
                    if ( (string)r[ 0 ] == (string)row[ "Category" ] )
                    {
                        break;
                    }
                    ++pos;
                }

                entry.Category = pos;
                entry.CategoryString = row[ "Category" ].ToString();
            }

            if ( !row[ "Task" ].IsEmpty() )
            {
                entry.Task = (string)row[ "Task" ];
            }

            return entry;
        }

        /// <summary>
        /// Returns the used time without jitter
        /// </summary>
        public TimeSpan? GetUsedTime()
        {
            if ( Begin == null || End == null )
            {
                return null;
            }

            return End - Begin - ( Jitter == null ? TimeSpan.Zero : Jitter.Value );
        }

        public DateTime? Begin
        {
            get;
            set;
        }
        public DateTime? End
        {
            get;
            set;
        }

        public TimeSpan? Jitter
        {
            get;
            set;
        }

        public int Category
        {
            get;
            set;
        }

        /// <summary>
        /// Contains the string originally parsed from the data row.
        /// </summary>
        public string CategoryString
        {
            get;
            private set;
        }

        public string Task
        {
            get;
            set;
        }

    }
}
