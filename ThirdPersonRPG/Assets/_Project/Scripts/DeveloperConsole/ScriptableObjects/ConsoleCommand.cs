using System.Collections.Generic;
using UnityEngine;

namespace LB.Utilities.DeveloperConsole.Commands
{
	public abstract class ConsoleCommand : ScriptableObject, IConsoleCommand
	{
		[SerializeField] private string commandWord = string.Empty;
		[SerializeField] protected List<string> possibleCommands = new();
		public string CommandWord => commandWord;

		public virtual List<string> GetPossibleCommands() => possibleCommands;

		public virtual List<string> GetPreviewCommands(string[] tokens)
		{
			// Default: do nothing.
			return null;
		}

		public abstract bool Process(string[] args);

		protected string[] GetSplitInput(string[] args) => string.Join(" ", args).Split(' ');
	}
}