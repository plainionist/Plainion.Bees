using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;

namespace Plainion.WhiteRabbit.Model
{
    public class Database
    {
        private readonly string myStorePath;
        private readonly string myCategoriesFile;

        public Database( string storePath )
        {
            if( !Directory.Exists( storePath ) )
            {
                Directory.CreateDirectory( storePath );
            }

            myStorePath = storePath;
            myCategoriesFile = Path.Combine( myStorePath, "categories.xml" );
        }

        /// <summary>
        /// Returns null if the table does not exists.
        /// Call CreateTable() then.
        /// </summary>
        public DataTable LoadTable( DateTime date )
        {
            string tableName = GetTableName( date );

            if( !TableExists( tableName ) )
            {
                return CreateTable( date );
            }

            // TODO: implement caching for this tables

            DataTable table = new DataTable();
            table.ReadXml( GetTableFile( tableName ) );

            // for old files, add an "duration" column which does not get serialized
            if( !table.Columns.Contains( ColumnNames.DURATION ) )
            {
                DataColumn durationCol = new DataColumn( ColumnNames.DURATION, typeof( string ) ) { ColumnMapping = MappingType.Hidden };
                table.Columns.Add( durationCol );
                table.AcceptChanges();
            }

            FillDurationCol( table );

            return table;
        }

        public void StoreTable( DataTable table )
        {
            if( table == null )
            {
                throw new ArgumentNullException( "table" );
            }
            FillDurationCol( table );
            FileStream stream = new FileStream( GetTableFile( table.TableName ), FileMode.Create );
            using( XmlTextWriter xmlWriter = new XmlTextWriter( stream, Encoding.Unicode ) )
            {
                table.WriteXml( xmlWriter, XmlWriteMode.WriteSchema );
            }
        }

        public DataTable CreateTable( DateTime date )
        {
            string tableName = GetTableName( date );

            if( TableExists( tableName ) )
            {
                throw new ArgumentException( "Table already exists: " + date );
            }

            DataTable table = new DataTable( tableName );
            table.Columns.Add( ColumnNames.BEGIN, typeof( string ) );
            table.Columns.Add( ColumnNames.END, typeof( string ) );
            table.Columns.Add( ColumnNames.CATEGORY, typeof( string ) );
            table.Columns.Add( ColumnNames.COMMENT, typeof( string ) );

            // add an "duration" column which does not get serialized
            DataColumn durationCol = new DataColumn( ColumnNames.DURATION, typeof( string ) ) { ColumnMapping = MappingType.Hidden };
            table.Columns.Add( durationCol );

            table.AcceptChanges();

            return table;
        }

        public DataTable LoadCategories()
        {
            if( !File.Exists( myCategoriesFile ) )
            {
                DataTable table = new DataTable( "Categories" );
                table.Columns.Add( "Name", typeof( string ) );

                DataRow row = table.NewRow();
                row[ 0 ] = "<none>";
                table.Rows.Add( row );

                table.AcceptChanges();

                return table;
            }
            else
            {
                DataTable table = new DataTable();
                table.ReadXml( myCategoriesFile );

                return table;
            }
        }

        public void StoreCategories( DataTable table )
        {
            if( table == null )
            {
                throw new ArgumentNullException( "table" );
            }

            FileStream stream = new FileStream( myCategoriesFile, FileMode.Create );
            using( XmlTextWriter xmlWriter = new XmlTextWriter( stream, Encoding.Unicode ) )
            {
                table.WriteXml( xmlWriter, XmlWriteMode.WriteSchema );
            }
        }

        public IEnumerable<DataTable> GetAllDays()
        {
            foreach( var file in Directory.GetFiles( myStorePath, "DAY_*.xml" ) )
            {
                DataTable table = new DataTable();
                table.ReadXml( file );
                yield return table;
            }
        }

        private void FillDurationCol( DataTable table )
        {
            if( table == null )
            {
                throw new ArgumentNullException( "table" );
            }

            foreach( DataRow row in table.Rows )
            {
                if( !row[ ColumnNames.BEGIN ].IsEmpty() &&
                    !row[ ColumnNames.END ].IsEmpty() )
                {
                    DateTime begin;
                    string beginString = ( string )row[ ColumnNames.BEGIN ];
                    DateTime end;
                    string endString = ( string )row[ ColumnNames.END ];

                    if( DateTime.TryParse( beginString, out begin ) &&
                        DateTime.TryParse( endString, out end ) )
                    {
                        var duration = DayEntry.GetDuration( begin, end );
                        StringBuilder durationString = new StringBuilder();
                        durationString.AppendFormat( "{0:D2}:{1:D2}", duration.Hours, duration.Minutes );
                        row[ ColumnNames.DURATION ] = durationString.ToString();
                    }
                }
            }
        }

        private bool TableExists( string tableName )
        {
            return File.Exists( GetTableFile( tableName ) );
        }

        private string GetTableName( DateTime date )
        {
            return "DAY_" + date.Year + "-" + date.Month + "-" + date.Day;
        }

        private string GetTableFile( string tableName )
        {
            return Path.Combine( myStorePath, tableName + ".xml" );
        }
    }
}