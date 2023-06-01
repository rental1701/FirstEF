using ACS.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ACS.Infrastructure.ParseSQLData;

namespace ACS.Model
{
    //[Table("pLogData")]
    public class LogData
    {
        public DateTime? EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }

        public bool IsLateEntry { get; set; }

        public bool IsEarlyExit { get; set; }

        public TimeSpan? WorkTime
        {
            get => ExitTime - EntryTime;
        }
        public static List<LogData> GetLogDataFromDataTable(string queryOne, string queryTwo)
        {
            try
            {
                List<LogData> logs = new();
                DataTable data = SclDataConnection.GetData(queryOne);
                if (data.Rows.Count != 0)
                {

                    int row = 0;
                    while (row < data.Rows.Count)
                    {
                        LogData temp = new() 
                                      { EntryTime = ConvertToDateTime(data.Rows[row].ItemArray[0])};
                        logs.Add(temp);
                        row++;
                    }
                }
                data = SclDataConnection.GetData(queryTwo);
                if (data.Rows.Count != 0)
                {
                    foreach (LogData log in logs)
                    {
                        int row = 0;
                        while (row < data.Rows.Count)
                        {
                            DateTime? date = ConvertToDateTime(data.Rows[row].ItemArray[0]);
                            if (date is DateTime t && log.EntryTime?.Date == t.Date)
                            {
                                log.ExitTime = date;
                            }
                            row++;
                        }
                    }
                }
                return logs;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public LogData()
        {

        }
    }
}
