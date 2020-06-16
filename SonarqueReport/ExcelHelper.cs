using System;
using System.Data;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace SonarqueReport
{
    class ExcelHelper
    {
        public static void DataTableToExcel(DataTable data, string fileName, bool isColumnWritten = true)
        {
            var fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            IWorkbook workbook = null;
            if (fileName.EndsWith(".xlsx")) // excel 2007
            {
                workbook = new XSSFWorkbook();
            }
            else if (fileName.EndsWith(".xls")) // excel 2003
            {
                workbook = new HSSFWorkbook();
            }
            else
            {
                throw new ArgumentException($"Please make sure the file extension for excel is correct!");
            }

            var sheet = workbook.CreateSheet();

            int count;
            int j;
            if (isColumnWritten) //output the column names
            {
                IRow row = sheet.CreateRow(0);
                for (j = 0; j < data.Columns.Count; ++j)
                {
                    row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                }

                count = 1;
            }
            else
            {
                count = 0;
            }

            int i;
            for (i = 0; i < data.Rows.Count; ++i)
            {
                IRow row = sheet.CreateRow(count);
                for (j = 0; j < data.Columns.Count; ++j)
                {
                    row.CreateCell(j).SetCellValue(data.Rows[i][j].ToString());
                    sheet.AutoSizeColumn(data.Columns[j].Ordinal);
                }

                ++count;
            }

            workbook.Write(fs);
        }

    }
}
