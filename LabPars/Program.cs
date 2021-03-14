using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace LabPars
{
    public class SheduleItem
    {
        public string Time { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
    }

    public static class Agility
    {
        private static Regex SheduleItem = new Regex(@"href=""(?<link>.*?)"".*?<time[^>]*>(?<time>.*?)<\/.*?channel-schedule__text[^>]*>(?<title>.*?)<\/");
        private static Match GetSheduleItem(string html) => SheduleItem.Match(html);

        public static IEnumerable<SheduleItem> GetShedule(string url)
        {
            var web = new HtmlWeb();
            var doc = web.Load(url);

            var xPath = "//li[contains(@class,'channel-schedule__event')]";

            if (doc.DocumentNode.SelectNodes(xPath) != null)
            {
                foreach (var link in doc.DocumentNode.SelectNodes(xPath))
                {
                    var sheduleItems = GetSheduleItem(link.InnerHtml);

                    yield return new SheduleItem
                    {
                        Time = sheduleItems.Groups["time"].Value,
                        Title = sheduleItems.Groups["title"].Value,
                        Link = "https://tv.yandex.ru" + sheduleItems.Groups["link"].Value
                    };
                }
            }
        }
    }

    class Program
    {
        static Dictionary<string, dynamic> afisha = new Dictionary<string, dynamic>
        {
            {"1",  new {title = "Первый",       link = "https://tv.yandex.ru/channel/pervyy-16" } },
            {"2",  new {title = "Россия 1",     link = "https://tv.yandex.ru/channel/rossiya-1-31" } },
            {"3",  new {title = "Матч!",        link = "https://tv.yandex.ru/channel/match-tv-49" } },
            {"4",  new {title = "НТВ",          link = "https://tv.yandex.ru/channel/ntv-11" } },
            {"5",  new {title = "Культура",     link = "https://tv.yandex.ru/channel/kultura-14" } },
            {"6",  new {title = "Россия 24",    link = "https://tv.yandex.ru/channel/rossiya-24-3" } },
            {"7",  new {title = "ТВ Центр",     link = "https://tv.yandex.ru/channel/tv-centr-32" } },
            {"8",  new {title = "Рен ТВ",       link = "https://tv.yandex.ru/channel/ren-30" } },
            {"9",  new {title = "СТС",          link = "https://tv.yandex.ru/channel/sts-8" } },
            {"10", new {title = "ТВ-3",         link = "https://tv.yandex.ru/channel/tv-3-17" } },
            {"11", new {title = "Пятница",      link = "https://tv.yandex.ru/channel/pyatnica-42" } }
        };

        static void Main(string[] args)
        {
            Console.Write("Выберите предпочтительный канал:\n" +
                $"{string.Join("\n", afisha.Select(x => $"{x.Key} - {x.Value.title}"))}\n\n" +
                 "> ");
            var channel = Console.ReadLine();
            var link = afisha[channel].link;

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, afisha[channel].title + ".md");

            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine($"## {afisha[channel].title}");
                sw.WriteLine($"#### {DateTime.Now.ToLongDateString()}");
                foreach (SheduleItem item in Agility.GetShedule(link))
                {
                    Console.WriteLine($"{item.Time} - {item.Title}");
                    sw.WriteLine($"[{item.Time} - {item.Title}]({item.Link})\n");
                }
            }

            Console.WriteLine($"\nДанные успешно записаны в файл\n{path}");
            Console.ReadKey();
        }
    }
}
