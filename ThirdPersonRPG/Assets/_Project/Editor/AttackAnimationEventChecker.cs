using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Editor utility to scan AnimationClips in a folder and check for missing 'AttackEndEvent' animation events.
/// Helps ensure all attack animations properly signal the end of the attack action.
/// </summary>
public class AttackAnimationEventChecker : EditorWindow
{
    private string animationFolder = "Assets/_Project/Animations/PlayerAnimations";
    private List<AnimationClip> attackClips = new List<AnimationClip>();
    private List<AnimationClip> missingEventClips = new List<AnimationClip>();
    private Vector2 scrollPos;
    private bool scanPerformed = false;

    [MenuItem("Tools/Attack Animation Event Checker")]
    public static void ShowWindow()
    {
        GetWindow<AttackAnimationEventChecker>("Attack Event Checker");
    }

    private void OnGUI()
    {
        GUILayout.Label("Attack Animation Event Checker", EditorStyles.boldLabel);
        GUILayout.Space(5);
        EditorGUILayout.HelpBox("Scans all AnimationClips in the specified folder for the 'AttackEndEvent' animation event. Helps you ensure all attack animations end the action properly.", MessageType.Info);
        animationFolder = EditorGUILayout.TextField("Animation Folder", animationFolder);
        if (GUILayout.Button("Scan Folder"))
        {
            ScanForAttackEndEvents();
        }
        if (!scanPerformed) return;
        GUILayout.Space(10);
        GUILayout.Label($"Total Attack Clips Found: {attackClips.Count}", EditorStyles.label);
        GUILayout.Label($"Missing 'AttackEndEvent': {missingEventClips.Count}", EditorStyles.label);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(250));
        foreach (var clip in attackClips)
        {
            bool missing = missingEventClips.Contains(clip);
            GUIStyle style = new GUIStyle(EditorStyles.label);
            if (missing) style.normal.textColor = Color.red;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(clip.name, style);
            if (missing && GUILayout.Button("Select", GUILayout.Width(60)))
            {
                Selection.activeObject = clip;
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
        if (missingEventClips.Count > 0 && GUILayout.Button("Select All Missing Clips"))
        {
            Selection.objects = missingEventClips.ToArray();
        }
    }

    /// <summary>
    /// Scans the specified folder for AnimationClips and checks for the 'AttackEndEvent' event.
    /// </summary>
    private void ScanForAttackEndEvents()
    {
        attackClips.Clear();
        missingEventClips.Clear();
        scanPerformed = false;
        if (!Directory.Exists(animationFolder))
        {
            Debug.LogError($"Directory does not exist: {animationFolder}");
            return;
        }
        string[] guids = AssetDatabase.FindAssets("t:AnimationClip", new[] { animationFolder });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip == null) continue;
            // Heuristic: consider as attack if name contains 'Attack' (customize as needed)
            if (!clip.name.ToLower().Contains("attack")) continue;
            attackClips.Add(clip);
            bool hasAttackEndEvent = false;
            foreach (var evt in AnimationUtility.GetAnimationEvents(clip))
            {
                if (evt.functionName == "AttackEndEvent")
                {
                    hasAttackEndEvent = true;
                    break;
                }
            }
            if (!hasAttackEndEvent)
            {
                missingEventClips.Add(clip);
            }
        }
        scanPerformed = true;
    }
} 