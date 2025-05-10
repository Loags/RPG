using System;
using System.Collections.Generic;
using System.Linq;
using LB.Utilities.DeveloperConsole.Commands;

namespace LB.Utilities.DeveloperConsole
{
    public class DeveloperConsole
    {
        private readonly string prefix;
        private readonly IEnumerable<IConsoleCommand> commands;

        public DeveloperConsole(string prefix, IEnumerable<IConsoleCommand> commands)
        {
            this.prefix = prefix;
            this.commands = commands;
        }

        private void ProcessCommand(string commandInput, string[] args)
        {
            foreach (IConsoleCommand command in commands)
            {
                if (!commandInput.Equals(command.CommandWord, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (command.Process(args))
                {
                    return;
                }
            }
        }

        public void ProcessCommand(string inputValue)
        {
            if (!inputValue.StartsWith(prefix))
            {
                if (inputValue.Trim().Length > 0)
                    DeveloperConsoleBehaviour.instance.developerConsoleCommandHistory.AddConsoleHistoryEntry(
                        "Invalid command format!\nYou can use '/help' to check out possible commands!");
                return;
            }

            inputValue = inputValue.Remove(0, prefix.Length);

            string[] inputSplit = inputValue.Split(' ');

            string commandInput = inputSplit[0];
            string[] args = inputSplit.Skip(1).ToArray();

            ProcessCommand(commandInput, args);
        }
    }
}