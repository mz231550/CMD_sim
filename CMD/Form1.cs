using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CMD
{
    public partial class Form1: Form
    {
        List<string> tasks = new List<string>();
        string currentPath = "C:/Users/User";
        string currentEditingFile = null;
        string prompt = "C:/Users/User>";
        Dictionary<string, List<string>> fileContents = new Dictionary<string, List<string>>();
        int promptLength;
        List<string> commandHistory = new List<string>();
        Dictionary<string, List<string>> virtualFileSystem = new Dictionary<string, List<string>>();
        public Form1()
        {
            InitializeComponent();
            tasks.Add("Print your full name on the scrren");
            tasks.Add("Close the application");
            tasks.Add("Print the current date");
            tasks.Add("Create a new directory and a create a text file inside of it");
            tasks.Add("Write your name in a newly created text file");
            tasks.Add("Change the current directory");
            tasks.Add("Fill a text file with lines and then print them");
            tasks.Add("Create and save a text file with nano");
            tasks.Add("Print the name of the form");

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            
        }
        private void help()
        {
            listBox1.Items.Add("help: Displays all available commands");
            listBox1.Items.Add("cls: Clears the screen of all text");
            listBox1.Items.Add("date: Displays current date");
            listBox1.Items.Add("time: Displays current time");
            listBox1.Items.Add("get_task: Gives randomly selected task");
            listBox1.Items.Add("exit: Closes application");
            listBox1.Items.Add("whoami: Displays form name");
            listBox1.Items.Add("ls: Displays all files in current directory");
            listBox1.Items.Add("mkdir: Creates new directory");
            listBox1.Items.Add("cd: Changes current directory");
            listBox1.Items.Add("echo: Prints the text on the screen");
            listBox1.Items.Add("touch: Create a file in current directory");
            listBox1.Items.Add("nano: Create and edit the contents of a file");
            listBox1.Items.Add("cat: prints the contents of a file");
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            richTextBox1.Text = prompt;
            promptLength = prompt.Length;
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.KeyDown += richTextBox1_KeyDown;
        }

        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back && richTextBox1.SelectionStart <= prompt.Length)
            {
                e.SuppressKeyPress = true;
            }

            if (e.KeyCode == Keys.Left && richTextBox1.SelectionStart <= prompt.Length)
            {
                e.SuppressKeyPress = true;
            }

            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                string fullText = richTextBox1.Text;
                string command = fullText.Substring(prompt.Length).Trim();

                if (!string.IsNullOrWhiteSpace(command))
                {
                    commandHistory.Add(command);
                }
                if (currentEditingFile != null)
                {
                    if (command.ToLower() == "confirm")
                    {
                        currentEditingFile = null;
                        prompt = currentPath + ">";
                        promptLength = prompt.Length;
                        listBox1.Items.Clear();
                    }
                    else
                    {
                        fileContents[currentEditingFile].Add(command);
                        listBox1.Items.Add(command);
                    }

                    listBox1.TopIndex = listBox1.Items.Count - 1;
                    richTextBox1.Text = prompt;
                }
                else
                {
                    if (!command.Contains(' '))
                    {
                        HandleCommand(command);
                    }
                    else
                    {
                        HandleBigCommand(command);
                    }
                }

              
                
                
                    richTextBox1.SelectionStart = richTextBox1.Text.Length;
            }
            if(e.KeyCode == Keys.Up)
            {
                if (commandHistory.Count > 0)
                {
                    richTextBox1.Text = prompt + $"{commandHistory[commandHistory.Count() - 1]}";
                }
            }
        }
        private void HandleBigCommand(String cmd)
        {
            string[] parts = cmd.Split(' ');
            string command = parts[0];
            string dir = parts[1];
            switch (command)
            {
                case "mkdir":
                    if (!string.IsNullOrWhiteSpace(dir))
                    {
                        if (!virtualFileSystem.ContainsKey(currentPath))
                            virtualFileSystem[currentPath] = new List<string>();
                        virtualFileSystem[currentPath].Add(dir);
                        break;
                    }
                    else
                    {
                        listBox1.Items.Add("Need to specify name of directory");
                        break;
                    }
                case "cd":
                    if (!string.IsNullOrWhiteSpace(dir))
                    {
                        if (dir == "..")
                        {
                            int lastSlashIndex = currentPath.LastIndexOf('/');
                            if (lastSlashIndex > 0)
                            {
                                currentPath = currentPath.Substring(0, lastSlashIndex);
                            }
                            else
                            {
                                currentPath = "C:";
                            }
                        }
                        else
                        {
                            string newPath = currentPath + "/" + dir;
                            currentPath = newPath;

                            if (!virtualFileSystem.ContainsKey(currentPath))
                                virtualFileSystem[currentPath] = new List<string>();
                        }

                        prompt = currentPath + ">";
                        promptLength = prompt.Length;
                    }
                    else
                    {
                        listBox1.Items.Add("Need to specify name of directory");
                    }
                    break;
                case "echo":
                    listBox1.Items.Add(dir);
                    break;
                case "touch":
                    if (!dir.Contains('.'))
                    {
                        listBox1.Items.Add("Invalid file format");
                    }
                    else
                    {
                        if (!virtualFileSystem.ContainsKey(currentPath))
                            virtualFileSystem[currentPath] = new List<string>();
                        virtualFileSystem[currentPath].Add(dir);
                    }
                        break;
                case "cat":
                    string filePath = currentPath + "/" + dir;
                    if (fileContents.ContainsKey(filePath))
                    {
                        foreach (string line in fileContents[filePath])
                            listBox1.Items.Add(line);
                    }
                    else
                    {
                        listBox1.Items.Add("File not found.");
                    }
                    break;
                case "nano":
                    if (!dir.Contains('.'))
                    {
                        listBox1.Items.Add("Invalid file format");
                    }
                    else
                    {
                        string filepPath = currentPath + "/" + dir;
                        currentEditingFile = filepPath;

                        if (!fileContents.ContainsKey(filepPath))
                            fileContents[filepPath] = new List<string>();

                        listBox1.Items.Add($"[Editing {dir}]. Type lines. Type 'confirm' to save and quit.");
                        prompt = $"nano({dir})> ";
                        if (!virtualFileSystem.ContainsKey(currentPath))
                            virtualFileSystem[currentPath] = new List<string>();
                        virtualFileSystem[currentPath].Add(dir);
                        promptLength = prompt.Length;
                    }
                    break;

            }
            listBox1.TopIndex = listBox1.Items.Count - 1;
            richTextBox1.Text = prompt;
        }
        private void HandleCommand(string cmd)
        {
            switch (cmd.ToLower())
            {
                case "help":
                    help();
                    break;
                case "cls":
                    listBox1.Items.Clear();
                    break;
                case "exit":
                    Application.Exit();
                    break;
                case "whoami":
                    listBox1.Items.Add(this.Name);
                    break;
                case "date":
                    listBox1.Items.Add(DateTime.Now.ToShortDateString());
                    break;
                case "time":
                    listBox1.Items.Add(DateTime.Now.ToShortTimeString());
                    break;
                case "ls":
                    if (virtualFileSystem.ContainsKey(currentPath))
                    {
                        foreach (string f in virtualFileSystem[currentPath])
                            listBox1.Items.Add(f);
                    }
                    else
                    {
                        listBox1.Items.Add("No files found.");
                    }
                    break;
                case "get_task":
                    GetTask();
                    break;
                default:
                    //listBox1.Items.Add("error");
                    break;
            }
            listBox1.TopIndex = listBox1.Items.Count - 1;
            richTextBox1.Text = prompt;
        }

        private void richTextBox1_SelectionChanged(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionStart < prompt.Length)
            {
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
            }
        }
        private void GetTask()
        {
            Random rand = new Random();
            if (tasks.Count > 0)
            {
                int x = rand.Next(tasks.Count());
                listBox1.Items.Add(tasks[x]);
                tasks.RemoveAt(x);
            }
            else
            {
                listBox1.Items.Add("No more tasks left");
            }
        }
    }
}
