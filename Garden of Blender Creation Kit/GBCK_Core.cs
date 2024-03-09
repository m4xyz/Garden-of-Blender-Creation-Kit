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
