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
            //p1
            Debug.WriteLine("");
            AnsiConsole.MarkupLine("[#eb7700]G.B.C.K.[/] v.1.0.0");
            //GBCK_Core.DrawLine();

            int program_counter = 0;

            //the path where the new project gets created
            string projects_path = null;

            //path to the program directory
            string program_path = AppDomain.CurrentDomain.BaseDirectory;
            program_path = program_path.Replace(@"\bin\Debug\", "");
            Debug.WriteLine($"program_dir_path ::: {program_path}");

            //the path of the folder which is used to create a project
            string template_path = $@"{program_path}\ProjectTemplate";
            Debug.WriteLine($"project_template_path ::: {template_path}");

            //name of the .csv to save the paths
            string csv_file_path = $@"{program_path}\Data.csv";
            Debug.WriteLine($"csv_file_path ::: {csv_file_path}");

            //reads the Data.csv file to load in previous entered paths
            if (File.Exists(csv_file_path))
            {
                using (StreamReader sr = new StreamReader(csv_file_path))
                {
                    //skipping heading-line
                    string head = sr.ReadLine();

                    string line;
                    string[] n_line_split;

                    //as long as the current line isn't blank
                    while ((line = sr.ReadLine()) != null)
                    {
                        n_line_split = line.Split(';');

                        if (n_line_split.Length == 2)
                        {
                            projects_path = n_line_split[0];
                            template_path = n_line_split[1];
                        }
                    }
                }
            }
            else
            {
                using (File.Create(csv_file_path))
                {
                    Debug.WriteLine($"Data.csv created - location: {csv_file_path}");
                }
            }

            while (true)
            {
                program_counter++;

                try
                {
                    Debug.WriteLine("user_project_path ::: " + projects_path);
                    while (Directory.Exists(projects_path) == false)
                    {
                        Console.Write("Enter path to the directory of your projects: ");
                        projects_path = GBCK_Core.RemoveQuotes(Console.ReadLine());
                    }

                    //If Data.csv is empty, it doesn't get executed
                    Debug.WriteLine(template_path);
                    while (Directory.Exists(template_path) == false)
                    {
                        Console.Write("Enter path to the directory of the template: ");
                        template_path = GBCK_Core.RemoveQuotes(Console.ReadLine());
                    }

                    //writing into Data.csv
                    using (StreamWriter sw = new StreamWriter(csv_file_path))
                    {
                        sw.WriteLine("Project-path;Template-path");
                        sw.WriteLine($"{projects_path};{template_path}");
                    }

                    if (program_counter == 1)
                    {
                        GBCK_Core.Status(projects_path, template_path, true);
                        Console.WriteLine("\nType 'help' to list all available commands");
                    }

                    //p2
                    string user_input = Console.ReadLine();

                    //REWORK
                    string[] user_inputs = GBCK_Core.ReturnUserInputArr(user_input);
                    //

                    string user_command = user_inputs[0];

                    for (int i = 0; i < user_inputs.Length; i++)
                    {
                        Debug.WriteLine($"user_inputs[{i}] ::: {user_inputs[i]}");
                    }

                    Console.WriteLine();
                    switch (user_command)
                    {
                        //lists all available commands
                        case "help":
                            GBCK_Core.WriteHelp();
                            break;

                        //checks if the entered directory exists, if it does 'user_project_path' will be replaced with the entered one
                        case "cd":
                            if (Directory.Exists(user_inputs[1]))
                            {
                                projects_path = user_inputs[1];
                                Console.WriteLine($"Changed project-directory to: {projects_path}");
                            }
                            else
                            {
                                Console.WriteLine($"Could not find the entered path: {user_inputs[1]}");
                            }
                            break;

                        case "cdt":
                            if (Directory.Exists(user_inputs[1]))
                            {
                                template_path = user_inputs[1];
                                Console.WriteLine($"Changed template-directory to: {template_path}");
                            }
                            else
                            {
                                Console.WriteLine($"The directory you have entered could not be found <{user_inputs[1]}>");
                            }
                            break;

                        case "create":
                            if (Directory.Exists(projects_path))
                            {
                                AnsiConsole.Progress().Start(ctx =>
                                {
                                    var task1 = ctx.AddTask($"[#eb7700]Creating project '{user_inputs[1]}'[/]");
                                    GBCK_Core.CopyFolder(template_path, projects_path, user_inputs[1]);

                                    while (ctx.IsFinished == false)
                                    {
                                        task1.Increment(2.5);
                                    }
                                });
                                Console.WriteLine($"Successfully created: {user_inputs[1]}");
                            }
                            else
                            {
                                Console.WriteLine("The path to your project-folder could not be found");
                            }
                            break;

                        case "list":
                            string[] projectsArr;
                            projectsArr = Directory.GetDirectories(projects_path);

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

                        case "status":
                            GBCK_Core.Status(projects_path, template_path, true);
                            break;

                        case "exit":
                            Environment.Exit(0);
                            break;

                        default:
                            Console.WriteLine("Wrong input");
                            break;
                    }
                }
                catch (Exception exception)
                {
                    //Tool.DrawColoredLine("red");
                    Console.WriteLine($"{exception}\n\n");
                    continue;
                }
            }
        }
    }
}
