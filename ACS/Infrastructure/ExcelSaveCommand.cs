using ACS.Commands.BaseCommand;
using ACS.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ACS.Infrastructure
{
    class ExcelSaveCommand
    {
        public ICommand SaveCommand { get; }

        private bool CanSaveCommandExecuted(object? arg) =>
             arg is List<LogDataIO> list && list.Count > 0;

        private void OnSaveCommandExecuted(object? obj)
        {
            try
            {
                if (obj is List<LogDataIO> list)
                {
                    string filePath = "Книга Excel.xlsx";
                    var save = new SaveFileDialog
                    {
                        Title = "Сохранение фала",
                        Filter = "Книга Excel(*.xlsx)|*.xlsx",
                        FileName = filePath,
                        RestoreDirectory = true
                    };

                    if (save.ShowDialog() != true)
                        return;
                    filePath = save.FileName;


                    Export.ExportToExcel(list, filePath);
                }
            }
            catch (Exception ex)
            {
                DialogWindow window = new();
                window.ShowDialog(ex.Message);
            }
        }

        public ExcelSaveCommand()
        {
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecuted);
        }
    }
}
