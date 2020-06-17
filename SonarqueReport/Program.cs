using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SonarqueReport
{
    class Program
    {
        private static Uri _uri;
        static async Task<int> Main()
        {
            var baseUrl = "http://172.31.211.17:9000/";
            _uri = new Uri(baseUrl);

            var scheme = "Basic";
            var userName = "clu";
            var password = "password";
            var parameter = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));
            var authValue = new AuthenticationHeaderValue(scheme, parameter);
            var pageSize = 500;
            int pageIndex = 1;
            var urlPathFormat = "api/issues/search?componentKeys=LISA.Core.6.0.master&s=FILE_LINE&languages=cs&resolved=false&rules=csharpsquid%3AS3776&severities=CRITICAL&ps={0}&organization=default-organization&p={1}&additionalFields=_all";
            try
            {
                List<Issue> issues = new List<Issue>();
                bool flag = true;
                while (flag)
                {
                    var urlPath = string.Format(urlPathFormat, pageSize, pageIndex);
                    var content = await GetReport(authValue, urlPath);
                    Rootobject json = JsonConvert.DeserializeObject<Rootobject>(content);
                    if (json == null)
                    {
                        throw new Exception($"Can not parse {content} to json");
                    }
                    issues.AddRange(json.issues);
                    var paging = json.paging;

                    if ( paging.pageIndex * pageSize<=paging.total )
                    {
                        flag = false;
                    }
                    else
                    {
                        pageIndex++;
                    }
                }

                DataTable table = DataTableHelper.CreateDataTable();
                Dictionary<string, int> dic = new Dictionary<string, int>();
                foreach (var issue in issues)
                {
                    var component = issue.component;
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
                    row[DataTableHelper.columnId] = index + 1;
                    row[DataTableHelper.columnComponent] = component;
                    var message = issue.message;
                    row[DataTableHelper.columnComplexity] = GetCyclomaticComplexity(message);
                    table.Rows.Add(row);
                }

                foreach (DataRow row in table.Rows)
                {
                    var key = row[DataTableHelper.columnComponent].ToString();
                    row[DataTableHelper.columnComponentCount] = dic[key];
                }


                var fileName = $"SonarqubeReport-{DateTime.Now:yyyyMMddHHmmssfff}.xlsx";
                var folder = PathHelper.GetDownloadFolderPath();
                var filePath = Path.Combine(folder, fileName);
                ExcelHelper.DataTableToExcel(table, filePath);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex);
            }

            return 0;
        }

        static async Task<string> GetReport(AuthenticationHeaderValue authenticationHeaderValue, string path)
        {
            var client = new HttpClient
            {
                DefaultRequestHeaders = {Authorization = authenticationHeaderValue},
                BaseAddress = _uri
            };


            var response = await client.GetAsync(path);
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            return stringResponse;
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
