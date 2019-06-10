using System;
using System.Collections.Generic;

namespace n_dunn_project_00
{ 
    /// <summary>
    /// Abstact Command class for each command to inherit.
    /// </summary>
    public abstract class Command
    {

        // Executes the command
        public abstract void Do();

        // returns the opposite of the current command
        public abstract Command GetOpposite();

    }


    /// <summary>
    /// Appends a character to the end of a string.
    /// </summary>
    public class AppendEndCommand : Command
    {
        private char value;

        public AppendEndCommand(char value)
        {
            this.value = value;
        }

        public override void Do()
        {
            Program.test += value;
        }

        public override Command GetOpposite()
        {
            return new RemoveEndCommand();
        }

    }


    /// <summary>
    /// Appends a character to the beginning of a string.
    /// </summary>
    public class AppendStartCommand : Command
    {
        private char value;

        public AppendStartCommand(char value)
        {
            this.value = value;
        }

        public override void Do()
        {
            Program.test = value + Program.test;
        }

        public override Command GetOpposite()
        {
            return new RemoveStartCommand();
        }

    }


    /// <summary>
    /// Removes a character from the end of a string.
    /// </summary>
    public class RemoveEndCommand : Command
    {
        private char value;

        public override void Do()
        {
            value = Program.test[Program.test.Length - 1];
            Program.test = Program.test.Remove(Program.test.Length - 1, 1);
        }

        public override Command GetOpposite()
        {
            return new AppendEndCommand(value);
        }

    }


    /// <summary>
    /// Removes a character from the start of a string.
    /// </summary>
    public class RemoveStartCommand : Command
    {
        private char value;

        public override void Do()
        {
            value = Program.test[0];
            Program.test = Program.test.Remove(0, 1);
        }

        public override Command GetOpposite()
        {
            return new AppendStartCommand(value);
        }

    }


    /// <summary>
    /// Capitializes a character at a specified index of a string.
    /// </summary>
    public class CapitalizeCommand : Command
    {
        private int index;

        public CapitalizeCommand(int index)
        {
            this.index = index;
        }

        public override void Do()
        {
            Program.test = Program.test.Substring(0, index) + Program.test[index].ToString().
                ToUpper() + Program.test.Substring(index + 1);
        }

        public override Command GetOpposite()
        {
            return new LowercaseCommand(index);
        }

    }


    /// <summary>
    /// Lower cases a character at a specified index of a string.
    /// </summary>
    public class LowercaseCommand : Command
    {
        private int index;

        public LowercaseCommand(int index)
        {
            this.index = index;
        }

        public override void Do()
        {
            Program.test = Program.test.Substring(0, index) + Program.test[index].ToString().
                ToLower() + Program.test.Substring(index + 1);
        }

        public override Command GetOpposite()
        {
            return new CapitalizeCommand(index);
        }

    }


    /// <summary>
    /// Converts a string to title case.
    /// </summary>
    public class TitleCaseCommand : Command
    {

        private string previousString;

        public override void Do()
        {
            previousString = Program.test;

            bool capitalize = true;
            
            string newString  = "";

            // capitialize the first character of each word
            foreach(char c in Program.test)
            {
                if (capitalize)
                {
                    newString += c.ToString().ToUpper();
                    capitalize = false;
                }
                else
                {
                    newString += c.ToString().ToLower();
                }

                if(c == ' ')
                {
                    capitalize = true;
                }
            }

            Program.test = newString;
        }

        public override Command GetOpposite()
        {
            return new NonTitleCaseCommand(previousString);
        }

    }


    /// <summary>
    /// Restores the previous state of the string before titleCaseCommand is 
    /// called.
    /// </summary>
    public class NonTitleCaseCommand : Command
    {

        string previousString;
        public NonTitleCaseCommand(string previousString)
        {
            this.previousString = previousString;
        }

        public override void Do()
        {
            Program.test = previousString;
        }

        public override Command GetOpposite()
        {
            return new TitleCaseCommand();
        }

    }


    /// <summary>
    /// Manages the stacks for undoing and redoing commands.
    /// Responsible for dispatching and executing commands
    /// </summary>
    public class CommandManager
    {
        private static CommandManager manager = null;
        private Stack<Command> undoStack;
        private Stack<Command> redoStack;

        public CommandManager()
        {
            undoStack = new Stack<Command>();
            redoStack = new Stack<Command>();
        }

        public static CommandManager Manager
        {
            get
            {
                if(manager == null)
                {
                    manager = new CommandManager();
                }

                return manager;
            }
        }

        public void ExecuteCommand(Command c)
        {
            c.Do();
            undoStack.Push(c.GetOpposite());
            redoStack.Clear();

            Console.WriteLine(Program.test);
        }

        public void UndoCommand()
        {
            if(undoStack.Count > 0)
            {
                Command c = undoStack.Pop();
                c.Do();
                redoStack.Push(c.GetOpposite());
                Console.WriteLine(Program.test);
            }
        }

        public void RedoCommand()
        {
            if(redoStack.Count > 0)
            {
                Command c = redoStack.Pop();
                c.Do();
                undoStack.Push(c.GetOpposite());

                Console.WriteLine(Program.test);
            }
        }
    }

    public class Program
    {

        public static string test = "hello world";

        static void Main(string[] args)
        {
            CommandManager manager = CommandManager.Manager;

            Console.WriteLine("-------TESTING APPEND---------");
            manager.ExecuteCommand(new AppendEndCommand('a')); // hello worlda
            manager.ExecuteCommand(new AppendEndCommand('b')); // hello worldab
            manager.UndoCommand(); // hello worlda
            manager.UndoCommand(); // hello world
            manager.RedoCommand(); // hello worlda

            Console.WriteLine("-------TESTING REMOVE LAST CHARACTER---------");
            manager.ExecuteCommand(new RemoveEndCommand()); // hello world
            manager.ExecuteCommand(new RemoveEndCommand()); // hello worl
            manager.UndoCommand(); // hello world
            manager.UndoCommand(); // Hello worlda
            manager.RedoCommand(); // hello world
            manager.RedoCommand(); // hello worl
            manager.UndoCommand(); // hello world

            Console.WriteLine("-------TESTING REMOVE FIRST CHARACTER---------");
            manager.ExecuteCommand(new RemoveStartCommand()); // ello world
            manager.ExecuteCommand(new RemoveStartCommand()); // llo world
            manager.UndoCommand(); // ello world
            manager.UndoCommand(); // hello world
            manager.RedoCommand(); // ello world

            Console.WriteLine("-------TESTING CAPITALIZE CHARACTER---------");
            manager.ExecuteCommand(new CapitalizeCommand(2)); // elLo world
            manager.ExecuteCommand(new CapitalizeCommand(0)); // ElLo world
            manager.UndoCommand(); // elLo world
            manager.UndoCommand(); // ello world
            manager.RedoCommand(); // elLo world

            Console.WriteLine("-------TESTING LOWERCASE CHARACTER---------");
            manager.ExecuteCommand(new LowercaseCommand(2)); // ello world
            manager.UndoCommand(); // elLo world
            manager.RedoCommand(); // ello world

            Console.WriteLine("-------TESTING TITLE CASE---------");
            manager.ExecuteCommand(new TitleCaseCommand()); // Ello World
            manager.UndoCommand(); // ello world
            manager.RedoCommand(); // Ello World

            Console.WriteLine("-------RANDOM TESTING---------");
            manager.ExecuteCommand(new AppendEndCommand('s')); // Ello Worlds
            manager.ExecuteCommand(new AppendStartCommand('h')); // hEllo Worlds
            manager.ExecuteCommand(new RemoveEndCommand()); // hEllo World
            manager.ExecuteCommand(new LowercaseCommand(1)); // hello World
            manager.ExecuteCommand(new TitleCaseCommand()); // Hello World
            manager.UndoCommand(); // hello world
            manager.UndoCommand(); // hEllo World
            manager.UndoCommand(); // hEllo Worlds 
            manager.UndoCommand(); // Ello Worlds
            manager.UndoCommand(); // Ello World
            manager.RedoCommand(); // Ello Worlds
            manager.RedoCommand(); // hEllo Worlds

            Console.ReadKey();
        }
    }
}
