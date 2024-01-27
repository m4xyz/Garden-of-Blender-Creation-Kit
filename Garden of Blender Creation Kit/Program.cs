using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;
using MaxKit;
using System.Diagnostics;

namespace Garden_of_Blender_Creation_Kit
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int ProgramCounter = 0;
            string userProjectDir = null;
            string projectTemplateDir = GBCK_Core.GetTemplateDir();
            string csvFileName = "Data.csv";
            string projectPath = @"C:\Users\maxfx\Documents\Dev\C#\Projects\Garden of Blender Creation Kit\Garden of Blender Creation Kit";

            //reads the Data.csv file to load in previous entered paths
            if (File.Exists($@"{projectPath}\{csvFileName}"))
            {
                using (StreamReader sr = new StreamReader($@"C:\Users\maxfx\Documents\Dev\C#\Projects\Garden of Blender Creation Kit\Garden of Blender Creation Kit\{csvFileName}"))
                {
                    string line;
                    string[] n_line_split;

                    //as long as the current line isn't blank
                    while ((line = sr.ReadLine()) != null)
                    {
                        n_line_split = line.Split(';');

                        if (n_line_split.Length == 2)
                        {
                            userProjectDir = n_line_split[0];
                            projectTemplateDir = n_line_split[1];
                        }
                    }
                }
            }
            else
            {
                //File.Create($@"C:\Users\maxfx\Documents\Dev\C#\Projects\Garden of Blender Creation Kit\Garden of Blender Creation Kit\{csvFileName}");
            }

            while (true)
            {
                ProgramCounter++;

                try
                {
                    while (!Directory.Exists(userProjectDir))
                    {
                        Console.Write("Please enter a viable path for your projects: ");
                        userProjectDir = Console.ReadLine();
                    }

                    while (!Directory.Exists(projectTemplateDir))
                    {
                        Console.Write("Please locate the template folder for your projects: ");
                        projectTemplateDir = Console.ReadLine();
                    }

                    using (StreamWriter sw = new StreamWriter($@"{projectPath}\{csvFileName}"))
                    {
                        sw.WriteLine($"{userProjectDir};{projectTemplateDir}");
                    }

                    if (ProgramCounter == 1 && (GBCK_Core.Status(userProjectDir, projectTemplateDir, false) == true))
                    {
                        AnsiConsole.MarkupLine("[#00ff51]STATUS OK[/]");
                    }
                    GBCK_Core.DrawLine();
                    Console.WriteLine("\nType 'help' to list all available commands");

                    string userInput = Console.ReadLine();
                    string[] arrUserInput = userInput.Split(' ');
                    string cmdBase = arrUserInput[0];

                    switch (cmdBase)
                    {
                        //lists all available commands
                        case "help":
                            GBCK_Core.WriteHelp();
                            break;

                        //checks if the entered directory exists, if it does 'currentDir' will be replaced with the entered one
                        case "cd":
                            if (Directory.Exists(arrUserInput[1]))
                            {
                                userProjectDir = arrUserInput[1];
                            }
                            else
                            {
                                Console.WriteLine($"The path you have entered could not be found >{arrUserInput[1]}<");
                            }
                            break;

                        case "cdt":
                            if (Directory.Exists(arrUserInput[1]))
                            {
                                projectTemplateDir = arrUserInput[1];
                            }
                            else
                            {
                                Console.WriteLine($"The path you have entered could not be found >{arrUserInput[1]}<");
                            }
                            break;

                        case "create":
                            if (Directory.Exists(userProjectDir))
                            {
                                AnsiConsole.Progress().Start(ctx =>
                                {
                                    var task1 = ctx.AddTask($"[#d4af37]Creating project '{arrUserInput[1]}'[/]");
                                    GBCK_Core.CopyFolder(projectTemplateDir, userProjectDir, arrUserInput[1]);

                                    while (!ctx.IsFinished)
                                    {
                                        task1.Increment(2.5);
                                    }
                                });
                                Console.WriteLine($"Successfully created [{arrUserInput[1]}]");
                            }
                            else
                            {
                                Console.WriteLine("Current directory could not be found");
                            }
                            break;

                        case "list":
                            string[] projectsArr;
                            projectsArr = Directory.GetDirectories(userProjectDir);

                            var projects = new Table();
                            projects.Border(TableBorder.Heavy);

                            projects.AddColumn(new TableColumn("Project").Centered());

                            for (int i = 0; i < projectsArr.Length; i++)
                            {
                                projectsArr[i] = Path.GetFileNameWithoutExtension(projectsArr[i]);
                                projects.AddRow(projectsArr[i]);
                            }

                            AnsiConsole.Write(projects);
                            break;

                        case "exit":
                            Environment.Exit(0);
                            break;

                        case "status":
                            GBCK_Core.Status(userProjectDir, projectTemplateDir, true);
                            break;

                        default:
                            Console.WriteLine("wrong input please use 'help' to see all available commands");
                            break;
                    }
                }
                catch (Exception exception)
                {
                    Tool.DrawColoredLine("red");
                    Console.WriteLine($"{exception}");
                    Tool.DrawColoredLine("red");
                    continue;
                }
                GBCK_Core.DrawLine();
            }
        }
    }
}
