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
        public static string GetTemplateDir()
        {
            string base_dir = AppDomain.CurrentDomain.BaseDirectory; //gets the whole path with the debug folder     
            char[] n_char_base_dir = base_dir.ToCharArray();
            char[] n_char_letters = new char[n_char_base_dir.Length];
            string[] n_string_words = new string[2];
            string newPath = null;

            for (int i = n_char_base_dir.Length, j = 0, x = 0; i > 0; i--, j++)
            {
                Debug.WriteLine($"i:{i}\nj:{j}\nx:{x}");
                if (x >= 2)
                {
                    break;
                }
                else if (j != 0 && (n_char_base_dir[i - 1] == '\\'))
                {
                    n_string_words[x] = new string(n_char_letters);
                    n_string_words[x] = "\\" + Tool.ClearBlanks(n_string_words[x]);

                    Array.Clear(n_char_letters, 0, j);
                    j = 0;
                    x++;

                    continue;
                }
                else if (n_char_base_dir[i - 1] == '\\')
                {
                    continue;
                }
                n_char_letters[j] = n_char_base_dir[i - 1];
            }

            foreach (string str in n_string_words)
            {
                newPath += str;
            }
            newPath = Tool.ReverseString(newPath);

            int template_dir_length = n_char_base_dir.Length - newPath.Length;

            char[] n_template_dir = new char[template_dir_length];
            for (int i = 0; i < template_dir_length; i++)
            {
                n_template_dir[i] = n_char_base_dir[i];
            }
            string template_dir = new string(n_template_dir);

            return template_dir + "ProjectTemplate";
        }

        public static bool Status(string currentDir, string templateDir, bool writeDir)
        {
            bool currentDirExists = Directory.Exists(currentDir);
            bool templateDirExists = Directory.Exists(templateDir);

            AnsiConsole.MarkupLine("[#eb7700]BlenderProjectManager[/] v.1.0.0");
            DrawLine();

            if (writeDir == true)
            {
                if (currentDirExists == true)
                {
                    AnsiConsole.Markup($"[#ea4f56]Current[/] Directory: {currentDir} -Exists:[#eb7700]{currentDirExists}[/]");
                }
                else
                {
                    AnsiConsole.Markup($"[#ea4f56]Current[/] Directory: {currentDir} -Exists:[#2f4858]{currentDirExists}[/]");
                }
                Console.WriteLine();

                if (templateDirExists == true)
                {
                    AnsiConsole.Markup($"[#ea4f56]Template[/] Directory: {templateDir} -Exists:[#eb7700]{templateDirExists}[/]");
                }
                else
                {
                    AnsiConsole.Markup($"[#ea4f56]Template[/] Directory: {templateDir} -Exists:[#2f4858]{templateDirExists}[/]");
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

        public static void WriteCurrentDirectory(string dir)
        {
            Console.Write($"current directory: ");
            AnsiConsole.Markup($"[#eb7700]{dir}\n[/]");
            Console.ResetColor();
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
            string changelog = @"cd <directory> ::: change current directory
cdt <directory> ::: change template directory
create <projectName> ::: create project in current directory
list ::: list all projects
exit ::: closes program
status ::: checks status";

            Console.WriteLine(changelog);
        }
    }
}
