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
            AnsiConsole.MarkupLine("[#eb7700]G.B.C.K.[/] v.1.0.0.4");

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
                    while (Directory.Exists(projects_path) == false)
                    {
                        Console.Write("Enter path to the directory of your projects: ");
                        projects_path = GBCK_Core.RemoveQuotes(Console.ReadLine());
                    }
                    Debug.WriteLine($"projects_path ::: {projects_path}");

                    //If Data.csv is empty, it doesn't get executed
                    while (Directory.Exists(template_path) == false)
                    {
                        Console.Write("Enter path to the directory of the template: ");
                        template_path = GBCK_Core.RemoveQuotes(Console.ReadLine());
                    }
                    Debug.WriteLine($"template_path ::: {template_path}");

                    //writing into Data.csv
                    using (StreamWriter sw = new StreamWriter(csv_file_path))
                    {
                        sw.WriteLine("Projects-path;Template-path");
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
                    string[] user_inputs = GBCK_Core.ReturnUserInputArrV2(user_input);
                    //

                    string user_command = user_inputs[0];
                    string user_parameter = user_inputs[1];

                    if (user_parameter != null)
                    {
                        user_parameter = GBCK_Core.RemoveQuotes(user_parameter);
                    }

                    Debug.WriteLine($"user_command ::: {user_command}");
                    Debug.WriteLine($"user_parameter ::: {user_parameter}");

                    Console.WriteLine();
                    switch (user_command)
                    {
                        //lists all available commands
                        case "help":
                            GBCK_Core.WriteHelp();
                            break;

                        //checks if the entered directory exists, if it does 'user_project_path' will be replaced with the entered one
                        case "cd":
                            if (Directory.Exists(user_parameter))
                            {
                                projects_path = user_parameter;
                                Console.WriteLine($"Changed project-directory to: {projects_path}");
                            }
                            else
                            {
                                Console.WriteLine($"Could not find the entered path to the directory of your projects: {user_parameter}");
                            }
                            break;

                        case "cdt":
                            if (Directory.Exists(user_parameter))
                            {
                                template_path = user_parameter;
                                Console.WriteLine($"Changed template-directory to: {template_path}");
                            }
                            else
                            {
                                Console.WriteLine($"Could not find the entered path to the directory of the template: {user_parameter}");
                            }
                            break;

                        case "create":
                            if(Directory.Exists($@"{projects_path}\{user_parameter}"))
                            {
                                string choice = "";
                                Console.Write($"'{user_parameter}' already exists, are you sure you still want to create a new project? (old files could be overwritten!)|(y/n):");
                                choice = Console.ReadLine();

                                if(choice == "n")
                                {
                                    break;
                                }
                            }

                            if (Directory.Exists(projects_path))
                            {
                                bool task_finished = false;
                                AnsiConsole.Progress().Start(ctx =>
                                {
                                    var task1 = ctx.AddTask($"[#eb7700]Creating project '{user_parameter}'...[/]");
                                    bool task_start = GBCK_Core.CopyFolder(template_path, projects_path, user_parameter);

                                    while (ctx.IsFinished == false && task_start == true)
                                    {
                                        task1.Increment(2.5);
                                    }

                                    if(task1.IsFinished)
                                    {
                                        task_finished = true;
                                    }
                                    Debug.WriteLine($"task_finished ::: {task_finished}");
                                });

                                if (task_finished == true)
                                {
                                    Console.WriteLine($"Successfully created the project-folder: {user_parameter}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Could not find the entered path to the directory of your projects :(");
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
                            Console.WriteLine("The command you have entered does not exist");
                            break;
                    }
                }
                catch (Exception exception)
                {
                    AnsiConsole.WriteException(exception);
                    continue;
                }
            }
        }
    }
}
