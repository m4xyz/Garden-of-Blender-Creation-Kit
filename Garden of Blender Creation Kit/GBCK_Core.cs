using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using Spectre.Console;
using MaxKit;

namespace Garden_of_Blender_Creation_Kit
{
    public class GBCK_Core
    {
        public static string[] ReturnUserInputArrV2(string input)
        {
            char[] input_chars = input.ToCharArray();
            string[] results = new string[2];

            //only entered command
            if (HasSpaces(input) == false)
            {
                //results[1] == NULL

                results[0] = input;
                return results;
            }

            //entered command and parameter
            for (int i = 0, j = 0; i < input_chars.Length; i++)
            {
                //j == 0 -> command
                //j == 1 -> parameter

                if ((input_chars[i] == ' ' && j == 0) && i != input_chars.Length - 1) //skip to parameter
                {
                    i++; //to skip the blank-space
                    j++;
                }
                results[j] += input_chars[i];
            }
            return results;
        }

        public static string RemoveQuotes(string input)
        {
            char[] input_chars = input.ToCharArray();

            string result = "";
            for (int i = 0; i < input_chars.Length; i++)
            {
                if (input_chars[i] != '"')
                {
                    result += input_chars[i];
                }
            }

            return result;
        }

        //checks if both the project_dir and the template_dir exist and returns a boolean
        public static bool Status(string project_dir, string template_dir, bool write_dir)
        {
            bool currentDirExists = Directory.Exists(project_dir);
            bool templateDirExists = Directory.Exists(template_dir);

            if (write_dir == true)
            {
                if (currentDirExists == true)
                {
                    AnsiConsole.Markup($"Project directory: {project_dir} -Exists:[#eb7700]{currentDirExists}[/]\n");
                }
                else
                {
                    AnsiConsole.Markup($"Project directory: {project_dir} -Exists:[#2f4858]{currentDirExists}[/]\n");
                }

                if (templateDirExists == true)
                {
                    AnsiConsole.Markup($"Template directory: {template_dir} -Exists:[#eb7700]{templateDirExists}[/]\n");
                }
                else
                {
                    AnsiConsole.Markup($"Template directory: {template_dir} -Exists:[#2f4858]{templateDirExists}[/]\n");
                }
                Console.WriteLine();
            }

            if (currentDirExists == true && templateDirExists == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CopyFolder(string template_dir, string destination_dir, string project_name)
        {
            bool copying_possible = false;

            if (template_dir == destination_dir)
            {
                Console.WriteLine(@"Can not create the project, it would have caused to create the project infinity times.
The projects-path and template-path must be different");
                return copying_possible;
            }

            Stack<string> stackFolderDir = new Stack<string>();
            stackFolderDir.Push(template_dir);
            destination_dir = Path.Combine(destination_dir, project_name);

            if (!Directory.Exists(destination_dir))
            {
                Directory.CreateDirectory(destination_dir);
            }

            while (stackFolderDir.Count > 0)
            {
                string currentFolderDir = stackFolderDir.Pop();
                string currentPasteFolderDir = currentFolderDir.Replace(template_dir, destination_dir);

                if (!Directory.Exists(currentPasteFolderDir))
                {
                    Directory.CreateDirectory(currentPasteFolderDir);
                }

                string[] files = Directory.GetFiles(currentFolderDir);

                foreach (string file in files)
                {
                    string fileNameExtension = Path.GetFileName(file);
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    string projectNameExtension = fileNameExtension.Replace(fileName, project_name);

                    string pasteFilePath = Path.Combine(currentPasteFolderDir, projectNameExtension);
                    File.Copy(file, pasteFilePath, true);
                }

                string[] subDir = Directory.GetDirectories(currentFolderDir);

                foreach (string str in subDir)
                {
                    stackFolderDir.Push(str);
                }
            }
            copying_possible = true;
            return copying_possible;
        }

        public static void DrawLine()
        {
            //Console.SetCursorPosition(0, Console.CursorTop + 1);
            //Console.WriteLine();

            for (int i = 0; i < Console.BufferWidth; i++)
            {
                if (!(i % 2 == 1))
                {
                    AnsiConsole.Markup("[#eb7700]-[/]");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write("-");
                }
            }
            Console.ResetColor();
        }

        public static void WriteHelp()
        {
            string help = @"cd <directory> ::: change project-directory
cdt <directory> ::: change template-directory
create <projectName> ::: create project in project-directory
list ::: list all projects
exit ::: closes program
status ::: checks status";

            Console.WriteLine(help);
        }

        public static bool HasSpaces(string input)
        {
            char[] input_chars = input.ToCharArray();

            for (int i = 0; i < input_chars.Length; i++)
            {
                if (input_chars[i] == ' ')
                {
                    return true;
                }
            }
            return false;
        }
    }
}
