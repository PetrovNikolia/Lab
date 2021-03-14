using System;
using System.Collections.Generic;
using System.IO;

namespace Lab2
{
    public static class Utils_2
    {
        public static IEnumerable<string> GetFilesAndFolders(string rootPath)
        {
            yield return rootPath;

            foreach (var files in Directory.GetFiles(rootPath))
            {
                yield return files;
            }

            yield return "\n"; // Визуальное разделение разных каталогов

            foreach (var dirs in Directory.GetDirectories(rootPath))
            {
                foreach (var file in GetFilesAndFolders(dirs))
                {
                    yield return file;
                }
            }

        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            foreach (string item in Utils_2.GetFilesAndFolders(AppDomain.CurrentDomain.BaseDirectory))
            {
                Console.WriteLine(item);
            }

            Console.ReadLine();
        }


    }
}
