using ACS.Commands.BaseCommand;
using ACS.Infrastructure;
using ACS.Model;
using ACS.Model.DataContext;
using ACS.ViewModels.Base;
using ACS.ViewModels.WorkSchedules;
using Microsoft.EntityFrameworkCore;
using Microsoft.Windows.Themes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ACS.ViewModels
{
    public class СheckpointVM : ViewModel
    {
        #region файловая система
        public WorkSchedulesVM Dir { get; } = new WorkSchedulesVM("C:\\Users\\kiselev\\EF\\FirstEF\\ACS\\WorkPdf");

        private WorkSchedulesVM? _SelectedDirectory;

        public WorkSchedulesVM? SelectedDirectory
        {
            get { return _SelectedDirectory; }
            set => Set(ref _SelectedDirectory, value);
        }


        private string _SelectedPdfFile;

        public string SelectedPdfFile
        {
            get { return _SelectedPdfFile; }
            set => Set(ref _SelectedPdfFile, value);
        }

        #endregion

        #region Группировка
        private List<string> _GroupList = null!;

        public List<string> GroupList
        {
            get => _GroupList;
            set => Set(ref _GroupList, value);
        }


        #region Выбранная группа
        private string _SelectedGroup;

        public string SelectedGroup
        {
            get => _SelectedGroup;
            set
            {
                if (Set(ref _SelectedGroup, value))
                {
                    if (SelectedGroup == "По подразделениям")
                    {
                        _ = Task.Run(() =>
                        {
                            using (DataUserContext db = new())
                            {
                                Divisions = db.Divisions.Include(d => d.Persons).ToList();
                                Division? temp = Divisions.FirstOrDefault();
                                if (temp != null)
                                {
                                    SelectedDivision = temp;
                                    IsVisiableTools = true;
                                }
                            }
                        });
                    }
                    else
                        SelectedDivision = null;
                    IsVisiableTools = false;
                }
            }
        }
        #endregion

        #region Тригер для визуализации элементов
        private bool _IsVisiableTools;

        public bool IsVisiableTools
        {
            get => _IsVisiableTools;
            set => Set(ref _IsVisiableTools, value);
        }
        #endregion

        #region Подразделения
        private List<Division>? _Divisions;

        public List<Division>? Divisions
        {
            get => _Divisions;
            set => Set(ref _Divisions, value);
        }
        #endregion

        #region Выбранное подразделение
        private Division? _SelectedDivision;

        public Division? SelectedDivision
        {
            get => _SelectedDivision;
            set => Set(ref _SelectedDivision, value);
        }

        #endregion
        #region Выбранный сотрудник для общего поиска
        private Person? _SelectedPersonFromDivison;

        public Person? SelectedPersonFromDivison
        {
            get => _SelectedPersonFromDivison;
            set
            {
                Set(ref _SelectedPersonFromDivison, value);
                if (SelectedPersonFromDivison != null)
                    IsAllPerson = false;
            }
        }

        #endregion

        #region Поиск всех сотрудников
        private bool _IsAllPerson = true;
        /// <summary>Определяет выбраны все сотрудники подразделения или нет</summary>
        public bool IsAllPerson
        {
            get => _IsAllPerson;
            set => Set(ref _IsAllPerson, value);
        }
        #endregion

        #region Рабочее время
        private DateTime _StartTime = new DateTime(2023, 01, 01, 8, 0, 0);
        /// <summary>Время входа/summary>
        public DateTime StartTime
        {
            get => _StartTime;
            set => Set(ref _StartTime, value);
        }
        private DateTime _ExitTime = new DateTime(2023, 01, 01, 17, 0, 0);
        /// <summary>Время выхода/summary>
        public DateTime ExitTime
        {
            get => _ExitTime;
            set => Set(ref _ExitTime, value);
        }

        #endregion
        #endregion

        #region Диапозон дат
        private DateTime? _startDate = DateTime.Today;

        public DateTime? StartDate
        {
            get => _startDate;
            set => Set(ref _startDate, value);
        }

        private DateTime? _finishDate = DateTime.Now;

        public DateTime? FinishDate
        {
            get => _finishDate;
            set => Set(ref _finishDate, value);
        }
        #endregion

        #region Список LogDataIO
        private List<LogDataIO>? _ListLogData;

        public List<LogDataIO>? ListLogData
        {
            get => _ListLogData;
            set => Set(ref _ListLogData, value);
        }
        #endregion

        private bool _IsVisibleAllColumn = true;

        public bool IsVisibleAllColumn
        {
            get => _IsVisibleAllColumn;
            set => Set(ref _IsVisibleAllColumn, value);
        }

        private bool _IsVisibleColumnInput = true;
        /// <summary>Определяет видимость столбца входа в таблице</summary>
        public bool IsVisibleColumnInput
        {
            get => _IsVisibleColumnInput;
            set => Set(ref _IsVisibleColumnInput, value);
        }

        private bool _IsVisibleColumnOutput = false;
        /// <summary>Определяет видимость столбца выхода в таблице</summary>
        public bool IsVisibleColumnOutput
        {
            get => _IsVisibleColumnOutput;
            set => Set(ref _IsVisibleColumnOutput, value);
        }

        #region Выбранный сотрудник
        private Person? _SelectedPerson;

        public Person? SelectedPerson
        {
            get => _SelectedPerson;
            set => Set(ref _SelectedPerson, value);
        }

        private LogDataIO? _SelectedPersonIO;

        public LogDataIO? SelectedPersonIO
        {
            get => _SelectedPersonIO;
            set
            {
                if (Set(ref _SelectedPersonIO, value) && _SelectedPersonIO != null)
                {
                    Person e = new();
                    using (DataUserContext user = new DataUserContext())
                    {
                        var person = user.Persons
                            .Include(p => p.Company)
                            .Include(p => p.Post).Where(p => p.ID == _SelectedPersonIO.HozOrgan)
                            .Include(d => d.Division)
                            .ToList();
                        e = (Person)person[0].Clone();

                    }
                    SelectedPerson = e;
                    OnFormSelectedPersonCommandExecute(false);
                }
            }
        }

        #region Диапозон дат для выбранного сотрудника
        private DateTime? _StartDatePerson = DateTime.Today;

        public DateTime? StartDatePerson
        {
            get => _StartDatePerson;
            set => Set(ref _StartDatePerson, value);
        }

        private DateTime? _FinishDatePerson = DateTime.Now;

        public DateTime? FinishDatePerson
        {
            get => _FinishDatePerson;
            set => Set(ref _FinishDatePerson, value);
        }
        #endregion

        #region Данные событий
        private List<LogData>? _ListDataPerson;

        public List<LogData>? ListDataPerson
        {
            get => _ListDataPerson;
            set => Set(ref _ListDataPerson, value);
        }


        #endregion
        #region Рабочее время
        private DateTime _StartTimePerson = new DateTime(2023, 01, 01, 8, 0, 0);
        /// <summary>Начало рабочего дня сотрудника/summary>
        public DateTime StartTimePerson
        {
            get => _StartTimePerson;
            set => Set(ref _StartTimePerson, value);
        }
        private DateTime _ExitTimePerson = new DateTime(2023, 01, 01, 17, 0, 0);
        /// <summary>конец рабочего дня сотрудника/summary>
        public DateTime ExitTimePerson
        {
            get => _ExitTimePerson;
            set => Set(ref _ExitTimePerson, value);
        }

        private const int _MAXDURATION = 120;
        private int _BreakDuration = 48;

        public int BreakDuration
        {
            get => _BreakDuration > _MAXDURATION ? _MAXDURATION : _BreakDuration;
            set => Set(ref _BreakDuration, value);
        }

        private double _TotalWorkTimeHour;

        public double TotalWorkTimeHour
        {
            get => Math.Round(_TotalWorkTimeHour, 1);
            set => Set(ref _TotalWorkTimeHour, value);
        }
        /// <summary>Рабочее время без учета обеда</summary>
        public double TotalWorkTimeMinute
        {
            get => ExitTimePerson.TimeOfDay.TotalMinutes - StartTimePerson.TimeOfDay.TotalMinutes - BreakDuration;
        }
        #endregion
        #endregion

        #region Команды
        #region Формирование списка опоздавших
        private ICommand? _ListListOfUndisciplinedCommand;
        public ICommand? ListListOfUndisciplinedCommand => _ListListOfUndisciplinedCommand ??=
            new LambdaCommand(OnListListOfUndisciplinedCommandExecute, CanListListOfUndisciplinedCommandExecute);
        private bool CanListListOfUndisciplinedCommandExecute(object? arg) => ListLogData != null;

        private void OnListListOfUndisciplinedCommandExecute(object? obj)
        {
            List<LogDataIO> list = new();
            foreach (LogDataIO log in ListLogData!)
            {
                if (log.FirstInput is DateTime d1 && d1.TimeOfDay > StartTime.TimeOfDay)
                {
                    log.IsLateEntry = true;
                }
                else
                    log.IsLateEntry = false;
                if (log.LastOutput is DateTime d2 && d2.TimeOfDay < ExitTime.TimeOfDay)
                {
                    log.IsEarlyExit = true;
                }
                else
                    log.IsEarlyExit = false;
                list.Add(log);
            }
            ListLogData = list;
        }
        #endregion
        #region Формирование общей таблицы данных
        private ICommand? _FormCommand;
        public ICommand? FormCommand => _FormCommand ??= new LambdaCommand(OnFormCommandExecute, CanFormCommandExecute);

        private bool CanFormCommandExecute(object? arg) => true;


        private void OnFormCommandExecute(object? obj)
        {
            Task.Run(() =>
            {
                try
                {
                    string queryOne = @$"With input as(
                               Select p.HozOrgan, p.mode, p.TimeVal, 
                               Row_number() Over(Partition by Convert(date, p.TimeVal), p.HozOrgan Order by p.TimeVal asc) first 
                               From pLogData p 
                               Where (p.TimeVal >= '{StartDate}' AND p.TimeVal <= '{FinishDate}') AND p.Event = 32 AND p.Mode = 1) 

                               SElect  i.HozOrgan Id,  l.Name SurName, d.Name Division,  i.TimeVal input From input i 
                               Join pList l on(l.ID = i.HozOrgan) 
                               Join PDivision d on(d.ID = l.Section) 
                               Where i.first = 1 Order by i.TimeVal";
                    string queryTwo = @$"With output as(
                               Select p.HozOrgan, p.TimeVal,
                               Row_number() Over(Partition by Convert(date, p.TimeVal), p.HozOrgan Order by p.TimeVal desc) last 
                               From pLogData p 
                               Where (p.TimeVal >= '{StartDate}' AND p.TimeVal <= '{FinishDate}') AND p.Event = 32 AND p.Mode = 2) 

                               Select o.HozOrgan Id, o.TimeVal output from output o 
                               Where o.last = 1 Order by o.TimeVal";
                    if (SelectedGroup == GroupList[0])
                    {
                        ListLogData = GetListLDIO(queryOne, queryTwo);
                    }
                    else if (SelectedGroup == GroupList[1])
                    {

                        if (IsAllPerson && SelectedDivision != null)
                        {

                            queryOne = @$"With input as(
                               Select p.HozOrgan, p.mode, p.TimeVal, 
                               Row_number() Over(Partition by Convert(date, p.TimeVal), p.HozOrgan Order by p.TimeVal asc) first 
                               From pLogData p 
                               Join pList l On(p.HozOrgan = l.ID)
                               Join PDivision d On(l.Section = d.ID)
                               Where (p.TimeVal >= '{StartDate}' AND p.TimeVal <= '{FinishDate}') AND p.Event = 32 AND p.Mode = 1 AND d.ID = {SelectedDivision?.ID}) 

                               SElect  i.HozOrgan Id,  l.Name SurName, d.Name Division,  i.TimeVal input From input i 
                               Join pList l on(l.ID = i.HozOrgan) 
                               Join PDivision d on(d.ID = l.Section) 
                               Where i.first = 1 Order by i.TimeVal";
                            queryTwo = @$"With output as(
                               Select p.HozOrgan, p.TimeVal,
                               Row_number() Over(Partition by Convert(date, p.TimeVal), p.HozOrgan Order by p.TimeVal desc) last 
                               From pLogData p 
                               Join pList l On(p.HozOrgan = l.ID)
                               Join PDivision d On(l.Section = d.ID)
                               Where (p.TimeVal >= '{StartDate}' AND p.TimeVal <= '{FinishDate}') AND p.Event = 32 AND p.Mode = 2 AND d.ID = {SelectedDivision?.ID}) 

                               Select o.HozOrgan Id, o.TimeVal output from output o 
                               Where o.last = 1 Order by o.TimeVal";
                            ListLogData = GetListLDIO(queryOne, queryTwo);
                        }
                        else if (SelectedDivision != null)
                        {
                            queryOne = @$"With input as(
                               Select p.HozOrgan, p.mode, p.TimeVal, 
                               Row_number() Over(Partition by Convert(date, p.TimeVal), p.HozOrgan Order by p.TimeVal asc) first 
                               From pLogData p 
                               Join pList l On(p.HozOrgan = l.ID)
                               Join PDivision d On(l.Section = d.ID)
                               Where (p.TimeVal >= '{StartDate}' AND p.TimeVal <= '{FinishDate}') AND p.Event = 32 AND p.Mode = 1 AND l.ID = {SelectedPersonFromDivison?.ID}) 

                               SElect  i.HozOrgan Id,  l.Name SurName, d.Name Division,  i.TimeVal input From input i 
                               Join pList l on(l.ID = i.HozOrgan) 
                               Join PDivision d on(d.ID = l.Section) 
                               Where i.first = 1 Order by i.TimeVal";
                            queryTwo = @$"With output as(
                               Select p.HozOrgan, p.TimeVal,
                               Row_number() Over(Partition by Convert(date, p.TimeVal), p.HozOrgan Order by p.TimeVal desc) last 
                               From pLogData p 
                               Join pList l On(p.HozOrgan = l.ID)
                               Join PDivision d On(l.Section = d.ID)
                               Where (p.TimeVal >= '{StartDate}' AND p.TimeVal <= '{FinishDate}') AND p.Event = 32 AND p.Mode = 2 AND l.ID = {SelectedPersonFromDivison?.ID}) 

                               Select o.HozOrgan Id, o.TimeVal output from output o 
                               Where o.last = 1 Order by o.TimeVal";
                            ListLogData = GetListLDIO(queryOne, queryTwo);
                        }
                    }
                    else if (SelectedGroup == GroupList[2])
                    {

                        queryOne = @$"With input as(
                               Select p.HozOrgan, p.mode, p.TimeVal, 
                               Row_number() Over(Partition by Convert(date, p.TimeVal), p.HozOrgan Order by p.TimeVal asc) first 
                               From pLogData p 
                               Where (p.TimeVal >= '{StartDate}' AND p.TimeVal <= '{FinishDate}') AND p.Event = 32 AND p.Mode = 1) 

                               SElect  i.HozOrgan Id,  l.Name SurName, d.Name Division,  i.TimeVal input From input i 
                               Join pList l on(l.ID = i.HozOrgan) 
                               Join PDivision d on(d.ID = l.Section) 
                               Where i.first = 1 Order by i.TimeVal";

                        //using (LogDataContext data = new LogDataContext())
                        //{
                        //    var logs = data.LogData
                        //        .Where(p => p.TimeVal >= StartDate).Where(p => p.TimeVal <= FinishDate)
                        //        .Where(p => p.Mode == 1).Where(p => p.Event == 32)
                        //        .Join(data.Persons, l => l.HozOrgan,
                        //        p => p.ID, (l, p) => new pLogData
                        //        {
                        //            HozOrgan = l.HozOrgan,
                        //            LastName = p.Name,
                        //            TimeVal = l.TimeVal,
                        //            Remark = l.Remark

                        //        }).ToList();
                        //}
                        ListLogData = GetListLDIO(queryOne);
                    }
                    else if (SelectedGroup == GroupList[3])
                    {

                        queryOne = @$"With output as(
                               Select p.HozOrgan, p.mode, p.TimeVal, 
                               Row_number() Over(Partition by Convert(date, p.TimeVal), p.HozOrgan Order by p.TimeVal asc) first 
                               From pLogData p 
                               Where (p.TimeVal >= '{StartDate}' AND p.TimeVal <= '{FinishDate}') AND p.Event = 32 AND p.Mode = 2) 

                               SElect  o.HozOrgan Id,  l.Name SurName, d.Name Division,  o.TimeVal output From output o 
                               Join pList l on(l.ID = o.HozOrgan) 
                               Join PDivision d on(d.ID = l.Section) 
                               Where o.first = 1 Order by o.TimeVal";
                        ListLogData = GetListLDIO(queryOne);
                    }
                    List<LogDataIO> GetListLDIO(string queryOne, string? queryTwo = null, bool isOutput = false)
                    {
                        DataTable data = SclDataConnection.GetData(queryOne);
                        List<LogDataIO> list = LogDataIO.GetLogDataIO(data);
                        if (queryTwo != null)
                        {
                            data = SclDataConnection.GetData(queryTwo);
                            LogDataIO.InitialLastOutput(list, data);
                            foreach (var item in list) //формирование главного списка
                            {
                                if (item.FirstInput is DateTime d1 && d1.TimeOfDay > StartTime.TimeOfDay)
                                {
                                    item.IsLateEntry = true;
                                }
                                if (item.LastOutput is DateTime d2 && d2.TimeOfDay < ExitTime.TimeOfDay)
                                {
                                    item.IsEarlyExit = true;
                                }
                            }
                        }
                        return list;
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            });
        }
        #endregion

        #region Формирование таблицы для выбранного сотрудника
        private ICommand? _FormSelectedPersonCommand;
        public ICommand? FormSelectedPersonCommand => _FormSelectedPersonCommand
            ??= new LambdaCommand(OnFormSelectedPersonCommandExecute, CanFormSelectedPersonCommandexecute);

        private bool CanFormSelectedPersonCommandexecute(object? arg) => SelectedPerson != null;


        private void OnFormSelectedPersonCommandExecute(object? obj)
        {
            string queryOne = @$"With input as(
                               Select p.HozOrgan, p.TimeVal, 
                               Row_number() Over(Partition by Convert(date, p.TimeVal), p.HozOrgan Order by p.TimeVal asc) first 
                               From pLogData p 
                               Where (p.TimeVal >= '{StartDatePerson}' AND p.TimeVal <= '{FinishDatePerson}') 
                               AND p.Event = 32 AND p.Mode = 1 AND p.HozOrgan = {SelectedPerson?.ID}) 

                               SElect i.TimeVal input From input i                              
                               Where i.first = 1 Order by i.TimeVal";



            string queryTwo = @$"With output as(
                               Select p.HozOrgan, p.TimeVal,
                               Row_number() Over(Partition by Convert(date, p.TimeVal), p.HozOrgan Order by p.TimeVal desc) last 
                               From pLogData p 
                               Where (p.TimeVal >= '{StartDatePerson}' AND p.TimeVal <= '{FinishDatePerson}') 
                               AND p.Event = 32 AND p.Mode = 2 AND p.HozOrgan = {SelectedPerson?.ID}) 

                               Select o.TimeVal output from output o 
                               Where o.last = 1 Order by o.TimeVal";

           

            var list = LogData.GetLogDataFromDataTable(queryOne, queryTwo);
            foreach (var item in list)
            {
                if (item.EntryTime is DateTime d1 && d1.TimeOfDay > StartTimePerson.TimeOfDay)
                {
                    item.IsLateEntry = true;
                }
                if (item.ExitTime is DateTime d2 && d2.TimeOfDay < ExitTimePerson.TimeOfDay)
                {
                    item.IsEarlyExit = true;
                }
            }
            ListDataPerson = list;      
            double k = 0;
            foreach (var item in ListDataPerson)
            {
                var m = TotalWorkTimeMinute;
                if (item.EntryTime is DateTime i)
                {
                    var t = StartTimePerson.TimeOfDay - i.TimeOfDay;
                    if (t.TotalMinutes < 0)
                    {
                        m += t.TotalMinutes;
                    }
                }
                if (item.ExitTime is DateTime e)
                {
                    var t = e.TimeOfDay - ExitTimePerson.TimeOfDay;
                    if (t.TotalMinutes < 0)
                    {
                        m += t.TotalMinutes;
                    }
                }
                k += m;
            }
            TotalWorkTimeHour = k / 60;
        }
        #endregion
        #endregion

        public СheckpointVM()
        {
            GroupList = new List<string>
            {"Первый вход - Последний выход",
            "По подразделениям",
            "Только вход",
            "Только выход"
            };
            _SelectedGroup = GroupList[0];
            Dir.Checkpoint = this;
        }
    }
}
