using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using Newtonsoft.Json;

namespace ExportAmazonFromEvernote
{
    internal static class Program
    {
        private static async Task Main()
        {
            Console.WriteLine($"■ Info: {ViewDateTime(DateTime.Now)} 処理開始");
            var data = await GetAmazonUrl();
            Console.WriteLine($"■ Info: {ViewDateTime(DateTime.Now)} ファイルの解析完了");
            Console.WriteLine($"■ Info: データ件数{data.Count}");
            var data2 = ChangeUrl(data);

            await File.WriteAllTextAsync(@"D:\temp\data\@log.txt",
                JsonConvert.SerializeObject(data2, Formatting.Indented));
        }

        /// <summary>
        /// 対象のファイルからAmazonのURLを取得。
        /// </summary>
        private static async Task<List<DataModel>> GetAmazonUrl()
        {
            var targetFiles = Directory.GetFiles(@"D:\temp\data", "*.html", SearchOption.AllDirectories);

            var config = Configuration.Default;
            var context = BrowsingContext.New(config);

            var data = new List<DataModel>();
            foreach (var file in targetFiles)
            {
                // HTMLファイルを開く
                var document = await context.OpenAsync(req => req.Content(File.ReadAllText(file)));

                // URLをすべて取得
                var link = document
                    .QuerySelectorAll("a")
                    .Attr("href")
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList();

                if (link.Any(x => !x.Contains("www.amazon.co.jp") && !x.Contains("www.amazon.com")))
                {
                    Console.WriteLine($"■ Warning: amazon以外のURLが記載されています。file: {file}");
                }
                else
                {
                    var dist = link.Distinct().ToList();
                    if (dist.Count > 1)
                    {
                        Console.WriteLine($"■ Warning: 1つのファイルに複数のURLが記載されています。file: {file}");
                    }

                    if (dist.Count == 0)
                    {
                        Console.WriteLine($"■ Warning: URLが1つも記載されていません。file: {file}");
                    }

                    var originUrl = dist.ElementAt(0);
                    if (!originUrl.EndsWith("/"))
                    {
                        originUrl += "/";
                    }

                    data.Add(new DataModel
                    {
                        Path = file,
                        OriginUrl = originUrl
                    });
                }
            }

            return data;
        }

        /// <summary>
        /// amazonのURLから余分な情報を除く。
        /// </summary>
        private static IEnumerable<DataModel> ChangeUrl(IEnumerable<DataModel> data)
        {
            var regex = new Regex(@"/dp/[0-9a-zA-Z]+/");
            var rtnList = new List<DataModel>();

            foreach (var m in data)
            {
                if (!m.OriginUrl.Contains("/dp/"))
                {
                    Console.WriteLine($"■ Warning: URLに/dp/が含まれていません。url={m.Path}");
                }

                var url = "";
                if (m.OriginUrl.Contains("https://www.amazon.co.jp"))
                {
                    url += "https://www.amazon.co.jp";
                }

                if (m.OriginUrl.Contains("https://www.amazon.com"))
                {
                    url += "https://www.amazon.com";
                }

                var match = regex.Match(m.OriginUrl);
                url += match.Value;

                rtnList.Add(new DataModel
                {
                    Path = m.Path,
                    OriginUrl = m.OriginUrl,
                    Url = url
                });
            }

            return rtnList;
        }

        /// <summary>
        /// DateTimeをわかりやすく
        /// </summary>
        private static string ViewDateTime(DateTime dt)
        {
            return dt.ToString("yyyy/MM/dd HH:mm:ss.fff");
        }
    }
}