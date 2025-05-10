using UnityEngine;

namespace LB.Utilities.DeveloperConsole.Commands
{
    [CreateAssetMenu(menuName = "Utilities/Developer/Commands/Help Command")]
    public class HelpCommand : ConsoleCommand
    {
        [TextArea(5, 20)] [SerializeField] private string helpText;

        public override bool Process(string[] args)
        {
            DeveloperConsoleCommandHistory developerConsoleCommandHistory =
                DeveloperConsoleBehaviour.instance.developerConsoleCommandHistory;

            string logText = string.Join(" ", args);

            developerConsoleCommandHistory.AddConsoleHistoryEntry("Help command was successful!");
            developerConsoleCommandHistory.AddConsoleDebugHistoryEntry(helpText);

            Debug.Log(logText);
            return true;
        }
    }
}