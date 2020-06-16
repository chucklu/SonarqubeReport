using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace SonarqueReport
{
    class Program
    {
        private static Uri _uri;
        private static string columnId = "Id";
        private static string columnComponent = "Component";
        private static string columnComponentCount = "ComponentCount";
        private static string columnComplexity = "Complexity";

        static async Task<int> Main()
        {
            var baseUrl = "http://172.31.211.17:9000/";
            _uri = new Uri(baseUrl);

            var scheme = "Basic";
            var userName = "clu";
            var password = "password";
            var parameter = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));
            var authValue = new AuthenticationHeaderValue(scheme, parameter);
            var urlPath = "api/issues/search?componentKeys=UK.Connect.Dev_nxt&s=FILE_LINE&languages=cs&resolved=false&severities=CRITICAL&ps=200&organization=default-organization&facets=severities%2Ctypes&additionalFields=_all";
            try
            {
                var content = await Get(authValue, urlPath);
                Console.WriteLine(content);
                dynamic json = JsonConvert.DeserializeObject(content);
                if (json == null)
                {
                    throw new Exception($"Can not parse {content} to json");
                }

                var issues = json.issues;
                Console.WriteLine();
                DataTable table = CreateDataTable();
                Dictionary<string, int> dic = new Dictionary<string, int>();
                foreach (var issue in issues)
                {
                    Console.WriteLine(issue.component);
                    var component = issue.component.ToString();
                    if (!dic.ContainsKey(component))
                    {
                        dic.Add(component, 1);
                    }
                    else
                    {
                        var count = dic[component] + 1;
                        dic[component] = count;
                    }

                    var row = table.NewRow();
                    var index = table.Rows.Count;
                    row[columnId] = index + 1;
                    row[columnComponent] = component;
                    var message = issue.message.ToString();
                    row[columnComplexity] = GetCyclomaticComplexity(message);
                    table.Rows.Add(row);
                }

                foreach (DataRow row in table.Rows)
                {
                    var key = row[columnComponent].ToString();
                    row[columnComponentCount] = dic[key];
                }


                var fileName = $"SonarqubeReport-{DateTime.Now:yyyyMMddHHmmssfff}.xlsx";
                var folder = GetDownloadFolderPath();
                var filePath = Path.Combine(folder, fileName);
                DataTableToExcel(table, filePath);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex);
            }

            return 0;
        }

        static async Task<string> Get(AuthenticationHeaderValue authenticationHeaderValue, string path)
        {
            var client = new HttpClient()
            {
                DefaultRequestHeaders = { Authorization = authenticationHeaderValue }
            };

            client.BaseAddress = _uri;

            var response = await client.GetAsync(path);
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            return stringResponse;
        }

        static DataTable CreateDataTable()
        {
            DataColumn column;
            DataTable table = new DataTable();

            // Create new DataColumn, set DataType,
            // ColumnName and add to DataTable.
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = columnId;
            column.ReadOnly = true;
            column.Unique = true;
            // Add the Column to the DataColumnCollection.
            table.Columns.Add(column);

            // Create second column.
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = columnComponent;
            column.AutoIncrement = false;
            column.ReadOnly = false;
            column.Unique = false;
            // Add the column to the table.
            table.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = columnComponentCount;
            column.ReadOnly = false;
            column.Unique = false;
            // Add the Column to the DataColumnCollection.
            table.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = columnComplexity;
            column.ReadOnly = false;
            column.Unique = false;
            // Add the Column to the DataColumnCollection.
            table.Columns.Add(column);

            return table;
        }

        public static int DataTableToExcel(DataTable data, string fileName, bool isColumnWritten = true)
        {
            int i = 0;
            int j = 0;
            int count = 0;
            ISheet sheet = null;

            var fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            IWorkbook workbook = null;
            if (fileName.EndsWith(".xlsx")) // 2007版本
                workbook = new XSSFWorkbook();
            else if (fileName.EndsWith(".xls")) // 2003版本
                workbook = new HSSFWorkbook();

            try
            {
                if (workbook != null)
                {
                    sheet = workbook.CreateSheet();
                }
                else
                {
                    return -1;
                }

                if (isColumnWritten == true) //写入DataTable的列名
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

                workbook.Write(fs); //写入到excel
                return count;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return -1;
            }
        }

        public static string GetDownloadFolderPath()
        {
            return System.Convert.ToString(
                Microsoft.Win32.Registry.GetValue(
                    @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders"
                    , "{374DE290-123F-4565-9164-39C4925E467B}"
                    , String.Empty
                )
            );
        }

        /// <summary>
        /// "message": "Refactor this method to reduce its Cognitive Complexity from 26 to the 15 allowed.",
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static int GetCyclomaticComplexity(string message)
        {
            if (!message.Contains("Refactor this method to reduce its Cognitive Complexity"))
            {
                return 0;
            }

            var array = message.Split(" ");
            int count = 10;
            var str = array[count - 1];
            var complexity = Convert.ToInt32(str);
            return complexity;
        }
    }
}
