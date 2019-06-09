using System;
using System.Collections.Generic;

namespace n_dunn_project_00
{ 
    public abstract class Command
    {

        public abstract void Do();
        public abstract Command GetOpposite();

    }

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

        public static string test = "hello";

        static void Main(string[] args)
        {
            CommandManager manager = CommandManager.Manager;

            Console.WriteLine("-------TESTING APPEND---------");
            manager.ExecuteCommand(new AppendEndCommand('W')); // helloW
            manager.ExecuteCommand(new AppendEndCommand('o')); // helloWo
            manager.UndoCommand(); // helloW
            manager.UndoCommand(); // hello
            manager.RedoCommand(); // helloW

            Console.WriteLine("-------TESTING REMOVE LAST CHARACTER---------");
            manager.ExecuteCommand(new RemoveEndCommand()); // hello
            manager.ExecuteCommand(new RemoveEndCommand()); // hell
            manager.UndoCommand(); // hello
            manager.UndoCommand(); // HelloW
            manager.RedoCommand(); // hello
            manager.RedoCommand(); // hell
            

            Console.ReadKey();
        }
    }
}
