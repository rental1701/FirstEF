using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using static ACS.Infrastructure.ParseSQLData;

namespace ACS.Model
{
    public class LogDataIO
    {
        /// <summary>Первый вход</summary>
        public DateTime? FirstInput { get; set; }

        /// <summary>Последний выход</summary>
        private DateTime? _LastOutput;

        public DateTime? LastOutput
        {
            get { return _LastOutput; }
            set
            {
                _LastOutput = value;
            }
        }

        public bool IsLateEntry { get; set; }
        public bool IsEarlyExit { get; set; }
        public TimeSpan? Worktime
        {
            get { return LastOutput - FirstInput; }
        }

        /// <summary>Id работника</summary>
        public int? HozOrgan { get; set; } = null!;

        private string? _SurName;
        /// <summary>Фамилия</summary>
        
        public string? SurName
        {
            get => _SurName != null ? Regex.Replace(_SurName, "\\d+_", "") : "Неизвестно";
            set => _SurName = value;
        }
        /// <summary>Подразделение</summary>
        public string? Division { get; set; } = null!;

        public static List<LogDataIO> GetLogDataIO(DataTable data)
        {
            if (data.Rows.Count != 0)
            {
                List<LogDataIO> logData = new();
                int indexRow = 0;
                while (indexRow < data.Rows.Count)
                {
                    logData.Add(new(data, indexRow));
                    indexRow++;
                }
                return logData;
            }
            else
                throw new ArgumentException("Данные за выбранный период для этого сотрудника отсутствуют!");//TODO:Создать окно сообщений

        }

        public static void InitialLastOutput(List<LogDataIO> logDatas, DataTable data)
        {
            foreach (LogDataIO log in logDatas)
            {
                int row = 0;
                while (row < data.Rows.Count)
                {            
                    DateTime? date = ConvertToDateTime(data.Rows[row].ItemArray[1]);
                    if (date is DateTime d  && d.Date == log.FirstInput?.Date 
                        && TryParseIntBD(data.Rows[row].ItemArray[0]) == log.HozOrgan)
                    {
                        log.LastOutput = date;
                    }  
                    row++;
                }
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj is LogDataIO log)
            {
                return this.HozOrgan == log.HozOrgan;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public LogDataIO(DataTable data, int row)
        {
            int c = 0;
            while (c < data.Columns.Count)
            {
                switch (data.Columns[c].ColumnName)
                {
                    case "Id":
                        HozOrgan = TryParseIntBD(data.Rows[row].ItemArray[c]);
                        break;
                    case "SurName":
                        SurName = TryParseStringBD(data.Rows[row].ItemArray[c]);
                        break;
                    case "Division":
                        Division = TryParseStringBD(data.Rows[row].ItemArray[c]);
                        break;
                    case "input":
                        FirstInput = ConvertToDateTime(data.Rows[row].ItemArray[c]);
                        break;
                    case "output":
                        LastOutput = ConvertToDateTime(data.Rows[row].ItemArray[c]);
                        break;
                }
                c++;
            }
        }
    }
}
