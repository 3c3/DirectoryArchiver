using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryArchiver
{
    class Program
    {
        /* 
         * Command line options
         * -t : time since last archived [30d;12w;1m;1y; default = 1m]
         * -a : archive directory [default = "Archive"]
         * -c : compress [default = no] <not implemented>
         */
        static void Main(string[] args)
        {
            string rootDir = Directory.GetCurrentDirectory();
            Console.WriteLine(rootDir);

            int days = 30;
            string archiveDir = "Archive";
            bool compress = false;

            string arg;
            for (int i = 0; i < args.Length; i++)
            {
                arg = args[i];
                if (arg == "-t")
                {
                    if (i + 1 < args.Length)
                    {
                        string inTime = args[i + 1];
                        char last = inTime.Last();

                        if (last >= 'a' && last <= 'z')
                        {
                            int multiplier = 1;
                            switch (last)
                            {
                                case 'w':
                                    multiplier = 7;
                                    break;
                                case 'm':
                                    multiplier = 30;
                                    break;
                                case 'y':
                                    multiplier = 365;
                                    break;
                            }

                            days = int.Parse(inTime.Substring(0, inTime.Length - 1)) * multiplier;
                        }
                    }
                }
                else if (arg == "-a")
                {
                    if (i + 1 < args.Length) archiveDir = args[i + 1];
                }
                else if (arg == "-c") compress = true;
            }

            Console.WriteLine("Starting time: {0} days", days);
            Console.WriteLine("Archive dir: {0}", archiveDir);
            Console.WriteLine("Compress: {0}", compress ? "yes" : "no");

            DirectoryInfo start = new DirectoryInfo(rootDir);
            var dirs = start.EnumerateDirectories();
            List<string> mvDirs = new List<string>();

            DateTime now = DateTime.Now;
            foreach (var dirInfo in dirs)
            {
                DateTime lastModified = dirInfo.LastAccessTime;
                int lastAccessDays = (now - lastModified).Days;
                if (lastAccessDays < days) continue;
                Console.WriteLine("{0} \t---\t{1} days", dirInfo.Name, lastAccessDays);
                mvDirs.Add(dirInfo.Name);
            }

            Console.WriteLine("Continue? [y/n]");
            string input = Console.ReadLine();
            if (input.Length < 1 || input[0] != 'y') return;

            archiveDir = rootDir + "\\" + archiveDir + "\\";

            if (!Directory.Exists(archiveDir)) Directory.CreateDirectory(archiveDir);

            for (int i = 0; i < mvDirs.Count; i++)
            {
                Directory.Move(rootDir + "\\" + mvDirs[i], archiveDir + mvDirs[i]);
            }
        }
    }
}
