using Microsoft.Extensions.DependencyInjection;
using Stealth.Model;
using Stealth.Persistence;

namespace Stealth
{
    internal class Program
    {
        static int Main(string[] args)
        {
            string path;
            IFileManager? fileManager = null;
            do
            {
                Console.Write("Please enter a valid text or pdf file path: ");
                path = Console.ReadLine() ?? "";
                if (System.IO.File.Exists(path))
                {
                    fileManager = FileManagerFactory.CreateForPath(path);
                }
            }
            while (fileManager == null);

            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<IGrid, Grid>();
            services.AddSingleton<IFileManager>(fileManager);
            using ServiceProvider serviceProvider = services.BuildServiceProvider();

            IGrid Grid = serviceProvider.GetRequiredService<IGrid>();

            try
            {
                Grid.Load();
            }
            catch (FileManagerException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                return -1;
            }
            int lepes = 0;
            int orlepes = 2;
            bool success = false;

            while (!success)
            {
                Grid.Writeout();
                int status = 0;
                do
                {   
                    string command = Console.ReadLine()!;

                    try
                    {
                        if (command != null)
                        {
                            status = Grid.MovePlayer(command.ToCharArray()[0]);
                            if (status == 2) { Console.WriteLine("FAL!!!"); }
                            if (status == 1) { Console.WriteLine("NYERTEL!!!"); success = true; lepes++; }
                        }
                        else
                        {
                            throw new Exception() ;
                        }
                        
                    }
                    catch (Exception) {
                        Console.WriteLine("Ures input!!!");
                    }

                } while (status == 2);


                if (lepes % orlepes == 0)
                {
                    success = Grid.MoveGuards();
                }

            }
            if (success) {
                Grid.Writeout();
                Console.WriteLine("ELKAPTAK!!!");
            }
            return 0;
        }
    }
}