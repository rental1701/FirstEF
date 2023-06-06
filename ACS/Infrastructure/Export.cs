using System;
using System.Collections.Generic;
using System.Reflection;
using Excel = Microsoft.Office.Interop.Excel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACS.Model;

namespace ACS.Infrastructure
{
    static class Export
    {
        public static void ExportToExcel(List<LogDataIO> list, string path)
        {
            try
            {
                Excel.Application excelApp = new Excel.Application();
                excelApp.Workbooks.Add();
                Excel.Worksheet ws = (Excel.Worksheet)excelApp.ActiveSheet;
                ws.Cells[1, "A"] = "Фамилия";
                ws.Cells[1, "B"] = "Подразделение";
                ws.Cells[1, "C"] = "Время входа";
                ws.Cells[1, "D"] = "Время выхода";
                ws.Cells[1, "E"] = "Разница";
                int row = 1;
                foreach (LogDataIO l in list)
                {
                    row++;
                    ws.Cells[row, "A"] = l.SurName;
                    ws.Cells[row, "B"] = l.Division;
                    ws.Cells[row, "C"] = l.FirstInput;
                    ws.Cells[row, "D"] = l.LastOutput;
                    ws.Cells[row, "E"] = l.Worktime;
                }
               
                ws.SaveAs(path+".xlsx");
                excelApp.Visible = true;
            }

            catch (Exception)
            {
                throw new Exception("Не удалось");
            }
        }
    }
}
