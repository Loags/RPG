using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LB.Utilities.DeveloperConsole.Commands;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LB.Utilities.DeveloperConsole
{
	public class DeveloperConsoleBehaviour : MonoBehaviour
	{
		[SerializeField] private string prefix = string.Empty;
		[SerializeField] private ConsoleCommand[] commands = Array.Empty<ConsoleCommand>();
		public DeveloperConsoleCommandHistory developerConsoleCommandHistory;

		[Header("UI")] [SerializeField] private GameObject uiCanvas = null;
		[SerializeField] private TMP_InputField inputField = null;
		[SerializeField] private GameObject previewTextPrefab;
		[SerializeField] private GameObject previewTextTarget;

		private List<PreviewData> spawnedPreviewData = new();
		
		public static DeveloperConsoleBehaviour instance;
		private DeveloperConsole developerConsole;

		private Coroutine spawnCommandWordPreviewsRoutine;
		private Coroutine spawnPossibleCommandPreviewsRoutine;
		private Coroutine spawnPreviewsRoutine;

		private int selectedPreviewIndex = -1;
		private int possibleCommandIndex = -1;

		//InputSystem
		public @RPGInputs rpgInputs;

		private bool toggleConsoleAction;
		private bool navigateDownAction;
		private bool navigateUpAction;
		private bool choosePreviewAction;

		public delegate void ConsoleToggled(bool isOpen);
		public event ConsoleToggled OnConsoleToggled;

		private DeveloperConsole DeveloperConsole
		{
			get
			{
				if (developerConsole != null)
				{
					return developerConsole;
				}

				return developerConsole = new DeveloperConsole(prefix, commands);
			}
		}

		private void Awake()
		{
			if (instance != null && instance != this)
			{
				Destroy(gameObject);
				return;
			}

			instance = this;
			rpgInputs = new @RPGInputs();
			rpgInputs.Console.Enable();
			DontDestroyOnLoad(gameObject);
		}

		private void Update()
		{
			Inputs();
		}

		private void Inputs()
		{
			try
			{
				toggleConsoleAction = rpgInputs.Console.ToggleConsole.WasPressedThisFrame();
				navigateDownAction = rpgInputs.Console.NavigateDown.WasPressedThisFrame();
				navigateUpAction = rpgInputs.Console.NavigateUp.WasPressedThisFrame();
				choosePreviewAction = rpgInputs.Console.ChoosePreview.WasPressedThisFrame();

				if (toggleConsoleAction) Toggle();
				if (navigateDownAction) NavigatePreviews(true);
				if (navigateUpAction) NavigatePreviews(false);
				if (choosePreviewAction) ChosePreview();
			}
			catch (Exception)
			{
				Debug.LogError("Inputs not found! Check your Input System settings.");
			}
		}

		private void ChosePreview()
		{
			if (spawnedPreviewData.Count > 0 && selectedPreviewIndex >= 0)
			{
				inputField.text = spawnedPreviewData[selectedPreviewIndex].OnPreviewChosen();
				inputField.caretPosition = inputField.text.Length;
				ResetIndexPreviewAndSelect();
			}
		}

		private void NavigatePreviews(bool _isDownArrow)
		{
			int previousIndex = selectedPreviewIndex;
			if (previousIndex < 0) previousIndex = 0;
			if (_isDownArrow)
			{
				selectedPreviewIndex++;
				if (selectedPreviewIndex >= spawnedPreviewData.Count)
					selectedPreviewIndex = 0;
			}
			else
			{
				selectedPreviewIndex--;
				if (selectedPreviewIndex < 0)
					selectedPreviewIndex = spawnedPreviewData.Count - 1;
			}

			if (spawnedPreviewData.Count <= 0) return;
			spawnedPreviewData[previousIndex].DeselectPreview();
			spawnedPreviewData[selectedPreviewIndex].SelectPreview();
		}

		private void ResetIndexPreviewAndSelect()
		{
			selectedPreviewIndex = -1;
			NavigatePreviews(true);
		}

		private void Toggle()
		{
			if (uiCanvas.activeSelf)
			{
				uiCanvas.SetActive(false);
				OnConsoleToggled?.Invoke(false);
				selectedPreviewIndex = -1;
				spawnedPreviewData.ForEach(entry => entry.DeselectPreview());
			}
			else
			{
				uiCanvas.SetActive(true);
				inputField.ActivateInputField();
				OnConsoleToggled?.Invoke(true);
				UpdateCommandPreview(inputField.text);
				ResetIndexPreviewAndSelect();
			}
		}

		public void ProcessCommand(string inputValue)
		{
			DeveloperConsole.ProcessCommand(inputValue);
			inputField.text = string.Empty;
			inputField.ActivateInputField();
			ResetIndexPreviewAndSelect();
		}

		public void UpdateCommandPreview(string inputValue)
		{
			// Clear any previously spawned preview entries.
			ClearSpawnedPrefabs();

			// If input is empty or just "/", show all base command words.
			if (string.IsNullOrWhiteSpace(inputValue) || inputValue.Trim() == "/")
			{
				List<string> commandWords = GetCommandWords();
				SpawnPreviews(commandWords, prefix);
				return;
			}

			// Remove the leading slash if present.
			string sanitizedInput = inputValue.Trim();
			if (sanitizedInput.StartsWith("/"))
				sanitizedInput = sanitizedInput.Substring(1).TrimStart();

			// Ensure tokens are properly split
			string[] tokens = sanitizedInput.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			if (tokens.Length == 0)
			{
				// If no tokens, treat it as if "/" was entered.
				List<string> commandWords = GetCommandWords();
				SpawnPreviews(commandWords, prefix);
				return;
			}

			// Get all commands that match the first token
			List<string> matchingCommands = GetMatchingConsoleCommands(tokens[0]);

			// If a single exact match exists, show its subcommands
			if (matchingCommands.Count == 1 &&
			    matchingCommands[0].Equals(tokens[0], StringComparison.OrdinalIgnoreCase))
			{
				ConsoleCommand matchingCommand = commands.FirstOrDefault(cmd =>
					cmd.CommandWord.Equals(matchingCommands[0], StringComparison.OrdinalIgnoreCase));

				if (matchingCommand != null)
				{
					List<string> suggestions = matchingCommand.GetPreviewCommands(tokens);

					// Ensure we only show previews if there are subcommands
					if (suggestions != null && suggestions.Count > 0)
					{
						SpawnPreviews(suggestions, prefix);
					}
					else
					{
						// If there are no subcommands, show the original matching command instead
						SpawnPreviews(matchingCommands, prefix);
					}

					return;
				}
			}


			// Otherwise, show all partial matches
			SpawnPreviews(matchingCommands, prefix);
		}


		private List<string> GetCommandWords()
		{
			List<string> commandWords = new();

			foreach (ConsoleCommand consoleCommand in commands)
			{
				commandWords.Add(consoleCommand.CommandWord);
			}

			commandWords.Sort((a, b) => string.Compare(a, b, StringComparison.OrdinalIgnoreCase));
			return commandWords;
		}

		private List<string> GetMatchingConsoleCommands(string input)
		{
			Debug.Log("Input: " + input);
			List<string> matches = new();

			if (input == string.Empty)
			{
				matches.AddRange(commands.Select(consoleCommand => consoleCommand.CommandWord));
				matches.Sort(StringComparer.OrdinalIgnoreCase);

				return matches;
			}

			matches.AddRange(commands
				.Where(consoleCommand =>
					consoleCommand.CommandWord.IndexOf(input, StringComparison.OrdinalIgnoreCase) >= 0)
				.Select(consoleCommand => consoleCommand.CommandWord));

			// ✅ Sort results to prioritize commands that start with the input
			matches.Sort((a, b) =>
			{
				bool aStarts = a.StartsWith(input, StringComparison.OrdinalIgnoreCase);
				bool bStarts = b.StartsWith(input, StringComparison.OrdinalIgnoreCase);

				if (aStarts && !bStarts) return -1;
				if (bStarts && !aStarts) return 1;
				return string.Compare(a, b, StringComparison.Ordinal);
			});
			Debug.Log("Matching commands: " + string.Join(", ", matches));

			return matches;
		}

		private void SpawnPreviews(List<string> inputValues, string prefix = null)
		{
			if (spawnPreviewsRoutine != null)
				StopCoroutine(spawnPreviewsRoutine);

			Debug.Log("Calling SpawnPreviews with: " + string.Join(", ", inputValues));

			spawnPreviewsRoutine = StartCoroutine(SpawnPreviewsCoroutine(inputValues, prefix));
		}

		private IEnumerator SpawnPreviewsCoroutine(List<string> inputValues, string prefix = null)
		{
			int iterationsPerFrame = 5;
			int counter = 0;

			Debug.Log("Starting SpawnPreviewsCoroutine with " + inputValues.Count + " entries.");

			foreach (string value in inputValues)
			{
				counter++;

				GameObject spawnedPrefab = Instantiate(previewTextPrefab, previewTextTarget.transform);
				PreviewData previewData = spawnedPrefab.AddComponent<PreviewData>();

                string displayText = (prefix != null ? prefix : "") + value;
                string insertionText = (prefix != null ? prefix : "") + value;

				Debug.Log("Creating preview: " + displayText);

				previewData.InitializeWithText(displayText, insertionText);
				spawnedPreviewData.Add(previewData);

				if (counter >= iterationsPerFrame)
					yield return null;
			}

			Debug.Log("Finished spawning previews.");
			spawnPreviewsRoutine = null;
			ResetIndexPreviewAndSelect();
		}

		private void ClearSpawnedPrefabs()
		{
			foreach (PreviewData data in spawnedPreviewData)
			{
				data.DestroyItSelf();
			}

			StopAndResetCoroutine(ref spawnPreviewsRoutine);

			spawnedPreviewData.Clear();
		}

		private void StopAndResetCoroutine(ref Coroutine coroutine)
		{
			if (coroutine != null)
			{
				StopCoroutine(coroutine);
				coroutine = null;
			}
		}
	}
}

[Serializable]
public class PreviewData : MonoBehaviour
{
	public TMP_Text previewTextField;
	public Image previewBackground;
	public string previewText; // Shown text (may include extra info)
	public string insertionText; // The text to insert on selection

	private Color normalColor = new Color(1f, 1f, 1f, 0f);
	private Color highlightColor = new Color(1f, 1f, 1f, 0.35f);

	public void InitializeWithText(string value)
	{
		previewTextField = GetComponentInChildren<TMP_Text>();
		previewBackground = GetComponentInChildren<Image>();

		previewTextField.text = value;
		previewText = value;
		insertionText = value;
	}

	public void InitializeWithText(string displayText, string insertionText)
	{
		previewTextField = GetComponentInChildren<TMP_Text>();
		previewBackground = GetComponentInChildren<Image>();

		previewTextField.text = displayText;
		previewText = displayText;
		this.insertionText = insertionText;
	}

	public string OnPreviewChosen() => insertionText;
	public void DestroyItSelf() => Destroy(gameObject);
	public void SelectPreview() => previewBackground.color = highlightColor;
	public void DeselectPreview() => previewBackground.color = normalColor;
}