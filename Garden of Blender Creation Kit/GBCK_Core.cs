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
        public static string[] ReturnUserInputArr(string input)
        {
            char[] input_chars = input.ToCharArray();
            string[] results = new string[2];

            string buffer = "";
            for (int i = 0, j = 0; i < input_chars.Length; i++)
            {
                if (input_chars[i] != ' ' && j == 0)
                {
                    buffer += input_chars[i];
                }
                else if(j > 0)
                {
                    buffer += input_chars[i];
                }

                
                if (input_chars[i] == ' ' && j == 0)
                {
                    results[j] = buffer;

                    buffer = "";
                    j++;
                }
                else if (i == input_chars.Length - 1 && j == 0)
                {
                    results[j] = buffer;
                    break;
                }

                if (i == input_chars.Length - 1 && j > 0)
                {
                    Debug.WriteLine("before ::: " + buffer);
                    buffer = RemoveQuotes(buffer);
                    Debug.WriteLine("after ::: " + buffer);
                    results[j] = buffer;
                }
            }

            int buffer_count_null_elements = 0;
            for (int i = 0; i < results.Length; i++)
            {
                if (results[i] == null)
                {
                    buffer_count_null_elements++;
                }
            }
            if (buffer_count_null_elements > 0)
            {
                Array.Resize(ref results, buffer_count_null_elements);
            }

            for(int i = 0; i < results.Length; i++)
            {
                Debug.WriteLine(results[i]);
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

        //checks if both the project_dir and the template_dir exist and returns a bool
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

        public static void CopyFolder(string copyFolderDir, string pasteFolderDir, string projectName)
        {
            Stack<string> stackFolderDir = new Stack<string>();
            stackFolderDir.Push(copyFolderDir);
            pasteFolderDir = Path.Combine(pasteFolderDir, projectName);

            if (!Directory.Exists(pasteFolderDir))
            {
                Directory.CreateDirectory(pasteFolderDir);
            }

            while (stackFolderDir.Count > 0)
            {
                string currentFolderDir = stackFolderDir.Pop();
                string currentPasteFolderDir = currentFolderDir.Replace(copyFolderDir, pasteFolderDir);

                if (!Directory.Exists(currentPasteFolderDir))
                {
                    Directory.CreateDirectory(currentPasteFolderDir);
                }

                string[] files = Directory.GetFiles(currentFolderDir);

                foreach (string file in files)
                {
                    string fileNameExtension = Path.GetFileName(file);
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    string projectNameExtension = fileNameExtension.Replace(fileName, projectName);

                    string pasteFilePath = Path.Combine(currentPasteFolderDir, projectNameExtension);
                    File.Copy(file, pasteFilePath, true);
                }

                string[] subDir = Directory.GetDirectories(currentFolderDir);

                foreach (string str in subDir)
                {
                    stackFolderDir.Push(str);
                }
            }
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
            string help = @"cd <directory> ::: change current directory
cdt <directory> ::: change template directory
create <projectName> ::: create project in current directory
list ::: list all projects
exit ::: closes program
status ::: checks status";

            Console.WriteLine(help);
        }
    }
}
