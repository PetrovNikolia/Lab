using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;


namespace Lab1
{
    public class CsvData
    {
        public string Name { get; set; }
        public string Mask { get; set; }
    }

    public static class Utils
    {
        /// <summary>
        /// Переводит из двоичной в десятичную систему
        /// </summary>
        /// <param name="sBinary">Строка с числом в двоичном представлении</param>
        /// <returns>Десятичное число</returns>
        public static int ToDec(string sBinary)
        {
            if (string.IsNullOrWhiteSpace(sBinary)) return 0;
            return sBinary.Last() - 48 + 2 * ToDec(sBinary.Substring(0, sBinary.Length - 1));
        }

        /// <summary>
        /// Переводит из десятичной в двоичную систему
        /// </summary>
        /// <param name="dec">Десятичное число</param>
        /// <returns>Строка с числом в двоичном представлении</returns>
        public static string ToBin(int dec)
        {
            if (dec == 0) return "";
            return ToBin(dec >> 1) + (dec & 1).ToString();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            if (args?.Length > 0)
            {
                if (args.Length == 2 &&
                   (args[0] == "d" || args[0] == "b") &&
                   !string.IsNullOrWhiteSpace(args[1]))
                {
                    if (File.Exists(args[1]))
                    {
                        var data = GetList(args[1]);

                        Console.WriteLine($"Содержимое файла:\n{new string('*', 14)}");
                        Console.WriteLine(string.Join("\n", data.Select(x => $"{x.Name,-7} \t {x.Mask,5}")));
                        Console.WriteLine(new string('*', 14));

                        if (args[0] == "d" && !CheckListOnBinary(data))
                        {
                            Console.WriteLine("Данные представлены не в виде двоичной последовательности");
                        }
                        else
                        {
                            data = data.Select(x =>
                            {
                                if (args[0] == "d")
                                {
                                    x.Mask = Utils.ToDec(x.Mask).ToString();
                                }
                                else if (args[0] == "b")
                                {
                                    x.Mask = Utils.ToBin(int.Parse(x.Mask));
                                }

                                return x;
                            }).ToList();

                            WriteList(data, args[1]);
                            Console.WriteLine("Данные успешно изменены");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Указанный файл отсутсвует");
                    }
                }
                else
                {
                    Console.WriteLine("Ошибка ввода команды.\nФормат команды: <mono> <exe> <d|b> <data_path>");
                }
            }
            else
            {
                Console.Write("Из какой в какую хотите перевести\nd - из двоичной в десятичную\nb - из десятичной в двоичную\n> ");
                var sln = Console.ReadLine();

                Console.WriteLine("Введите число:");
                var number = Console.ReadLine();
                var result = "";

                if (sln == "b")
                {
                    result = Utils.ToBin(int.Parse(number));
                }
                else if (sln == "d" && CheckBinary(number))
                {
                    result = Utils.ToDec(number).ToString();
                }
                else
                {
                    Console.WriteLine("Неверный ввод данных");
                }
                Console.WriteLine(result);
            }
            Console.ReadLine();
        }

        static bool CheckListOnBinary(List<CsvData> data) => data.All(x => CheckBinary(x.Mask));
        static bool CheckBinary(string str) => new Regex(@"[01]").IsMatch(str);

        static List<CsvData> GetList(string path)
        {
            var result = new List<CsvData>();

            using (var sr = new StreamReader(path))
            using (var cr = new CsvReader(sr, new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = true
            }))
            {

                result = cr.GetRecords<CsvData>().ToList();
            }

            return result;
        }

        static void WriteList(List<CsvData> data, string path)
        {
            using (var sw = new StreamWriter(path))
            using (var cw = new CsvWriter(sw, new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = true
            }))
            {
                cw.WriteRecords(data);
            }
        }

    }
}
