// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="D. Brown Management">
//   Copyright 2017 - D. Brown Management
// </copyright>
// <summary>
//   Inserts data into a date dimension table
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;

namespace DimensionGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;    

    public class Program
    {
        public static void Main(string[] args)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["sqlConnection"].ConnectionString;

            InsertDateInfo(connectionString);
        }
        
        private static void InsertDateInfo(string connectionString)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            var startDate = DateTime.Parse("1990-01-01");
            var endDate = DateTime.Parse("2029-12-31");
            
            using (var copy = new SqlBulkCopy(conn))
            {
                copy.DestinationTableName = "DateDimension";
                var table = new DataTable("DateDimension");
                table.Columns.Add("DateId", typeof(DateTime));
                table.Columns.Add("DateName", typeof(string));
                table.Columns.Add("DateNameAbbrev", typeof(string));
                table.Columns.Add("DayOYear", typeof(int));
                table.Columns.Add("CalendarYearBegin", typeof(DateTime));
                table.Columns.Add("CalendarYearEnd", typeof(DateTime));
                table.Columns.Add("CalendarYear", typeof(int));
                table.Columns.Add("CalendarMonthBegin", typeof(DateTime));
                table.Columns.Add("CalendarMonthEnd", typeof(DateTime));
                table.Columns.Add("CalendarMonthNumber", typeof(int));
                table.Columns.Add("CalendarMonthName", typeof(string));
                table.Columns.Add("CalendarMonthNameAbbrev", typeof(string));
                table.Columns.Add("FiscalYear", typeof(int));
                table.Columns.Add("FiscalYearBegin", typeof(DateTime));
                table.Columns.Add("FiscalYearEnd", typeof(DateTime));
                table.Columns.Add("WeekBeginDate", typeof(DateTime));
                table.Columns.Add("WeekEndDate", typeof(DateTime));
                table.Columns.Add("WeekNumber", typeof(int));
                table.Columns.Add("WeekKeyString", typeof(string));
                table.Columns.Add("PayrollWorkedCheckDate", typeof(DateTime));
                table.Columns.Add("PayrollCurrentCheckDate", typeof(DateTime));
                table.Columns.Add("IsWeekend", typeof(bool));

                foreach (var date in EachDate(startDate, endDate))
                {
                    var dateName = date.ToString("dddd");
                    var dayOfYear = date.DayOfYear;
                    var dateNameAbbrev = date.ToString("ddd");
                    var calYearBegin = new DateTime(date.Year, 1, 1);
                    var calYearEnd = new DateTime(date.Year, 12, 31);
                    var calYear = date.Year;
                    var calMthBegin = new DateTime(date.Year, date.Month, 1);
                    var calMthEnd = new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
                    var calMthNum = date.Month;
                    var calMthName = date.ToString("MMMM");
                    var calMthNameAbbrev = date.ToString("MMM");                    

                    var fiscalYear = FiscalYear(date);
                    var fiscalYearBegin = FiscalYearBegin(date);
                    var fiscalYearEnd = FiscalYearEnd(date);

                    var weekBeginDate = StartOfWeek(date);
                    var weekEndDate = EndOfWeek(date);

                    var weekNumber = WeekNumber(date);
                    var weekKeyString = $"{calYear}-{weekNumber:00}";
                    var payrollWorkedCheckDate = PayrollWorkedCheckDate(date);
                    var payrollCurrentCheckDate = PayrollCurrentCheckDate(date);
                    //var weekKeyString = string.Format("{0}-{1:00}", calYear, weekNumber);

                    var isWeekend = date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday;

                    table.Rows.Add(
                        date,
                        dateName,
                        dateNameAbbrev,
                        dayOfYear,
                        calYearBegin,
                        calYearEnd,
                        calYear,
                        calMthBegin,
                        calMthEnd,
                        calMthNum,
                        calMthName,
                        calMthNameAbbrev,
                        fiscalYear,
                        fiscalYearBegin,
                        fiscalYearEnd,
                        weekBeginDate,
                        weekEndDate,
                        weekNumber,
                        weekKeyString,
                        payrollWorkedCheckDate,
                        payrollCurrentCheckDate,
                        isWeekend);
                }

                copy.WriteToServer(table);
            }
        }

        private static IEnumerable<DateTime> EachDate(DateTime start, DateTime end)
        {
            for (var day = start.Date; day.Date <= end.Date; day = day.AddDays(1))
            { 
                yield return day;
            }
        }

        private static int FiscalYear(DateTime actualDate)
        {
            /*
            if (actualDate.Month >= 4)
            {
                return actualDate.Year + 1;
            }*/

            return actualDate.Year;
        }

        private static DateTime FiscalYearBegin(DateTime actualDate)
        {
            /*
            if (actualDate.Month >= 4)
            {
                return new DateTime(actualDate.Year, 4, 1);
            }
            */
            return new DateTime(actualDate.Year, 1, 1);                
        }
    
        private static DateTime FiscalYearEnd(DateTime actualDate)
        {
            /*if (actualDate.Month >= 4)
            {
                return new DateTime(actualDate.Year + 1, 3, 31);
            }*/

            return new DateTime(actualDate.Year, 12, 31);
        }

        private static DateTime StartOfWeek(DateTime actualDate)
        {
            int diff = actualDate.DayOfWeek - DayOfWeek.Monday;
            if (diff < 0)
            {
                diff += 7;
            }

            return actualDate.AddDays(-1 * diff).Date;
        }

        private static DateTime EndOfWeek(DateTime actualDate)
        {
            return actualDate.DayOfWeek == DayOfWeek.Sunday ? actualDate : actualDate.AddDays(7 - (int)actualDate.DayOfWeek);
        }

        private static int WeekNumber(DateTime actualDate)
        {
            DateTimeFormatInfo dateTimeFormat = DateTimeFormatInfo.CurrentInfo;
            if (dateTimeFormat == null)
            {
                return 0;
            }
            Calendar cal = dateTimeFormat.Calendar;
            return cal.GetWeekOfYear(actualDate, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }

        private static DateTime PayrollWorkedCheckDate(DateTime actualDate)
        {
            return StartOfWeek(actualDate).AddDays(11);
        }

        private static DateTime PayrollCurrentCheckDate(DateTime actualDate)
        {
            return StartOfWeek(actualDate).AddDays(4);
        }
    }
}
