using UnityEngine;

namespace LB.Utilities.DeveloperConsole.Commands
{
    [CreateAssetMenu(menuName = "Utilities/Developer/Commands/Log Command")]
    public class LogCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            string logText = string.Join(" ", args);
            Debug.Log(logText);
            return true;
        }
    }
}