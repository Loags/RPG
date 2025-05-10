using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using LB.Loot;
using static LB.Loot.LootSourceTemplate;
using LB.Loot.Currency;
using LB.Loot.Experience;

namespace LB.Editor
{
    /// <summary>
    /// Editor window for managing loot templates and enemy loot configurations
    /// </summary>
    public class LootTemplateManagerWindow : EditorWindow
    {
        private Vector2 scrollPosition;
        private LootDatabase database;
        private LootSourceTemplate selectedTemplate;
        private LootSourceTemplate newTemplate;
        private bool showNewTemplateSection;
        private bool showDefaultTemplatesSection = true;
        private bool showLootConfigurationsSection = true;
        private string searchFilter = "";
        private Dictionary<LootSourceTemplate, bool> templateFoldouts = new();
        private Dictionary<LootDatabase.LootConfiguration, bool> configFoldouts = new();

        [MenuItem("Window/Loot/Loot Template Manager")]
        public static void ShowWindow()
        {
            GetWindow<LootTemplateManagerWindow>("Loot Template Manager");
        }

        private void OnEnable()
        {
            // Load the database from Resources
            database = Resources.Load<LootDatabase>("LootDatabase");
            if (database == null)
            {
                Debug.LogError("LootDatabase not found in Resources folder!");
                return;
            }
        }

        private void OnGUI()
        {
            if (database == null)
            {
                EditorGUILayout.HelpBox("LootDatabase not found! Make sure it exists in the Resources folder.", MessageType.Error);
                if (GUILayout.Button("Reload Data"))
                {
                    OnEnable();
                }
                return;
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            DrawHeader();
            DrawSearchBar();
            DrawNewTemplateSection();
            DrawDefaultTemplatesSection();
            DrawLootConfigurationsSection();

            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Loot Template Manager", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "This window allows you to manage loot templates and their configurations. " +
                "Templates define what items and currency can drop, while configurations link these templates to specific enemies.", 
                MessageType.Info);
            EditorGUILayout.Space(5);
        }

        private void DrawSearchBar()
        {
            EditorGUILayout.BeginHorizontal();
            searchFilter = EditorGUILayout.TextField(new GUIContent("Search Templates:", "Filter templates by name"), searchFilter);
            if (GUILayout.Button("Clear", GUILayout.Width(60)))
            {
                searchFilter = "";
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
        }

        private void DrawNewTemplateSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            showNewTemplateSection = EditorGUILayout.Foldout(showNewTemplateSection, "Create New Template", true);
            
            if (showNewTemplateSection)
            {
                EditorGUILayout.HelpBox(
                    "Create a new loot template that defines what items and currency can drop. " +
                    "Templates can be assigned to specific enemies or used as default templates based on enemy type and level.", 
                    MessageType.Info);
                EditorGUILayout.Space(5);
                
                if (newTemplate == null)
                {
                    newTemplate = ScriptableObject.CreateInstance<LootSourceTemplate>();
                }

                EditorGUI.BeginChangeCheck();
                
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                System.Reflection.FieldInfo templateNameField = typeof(LootSourceTemplate).GetField("templateName", 
                    System.Reflection.BindingFlags.NonPublic | 
                    System.Reflection.BindingFlags.Instance);
                System.Reflection.FieldInfo tierField = typeof(LootSourceTemplate).GetField("lootTier", 
                    System.Reflection.BindingFlags.NonPublic | 
                    System.Reflection.BindingFlags.Instance);
                System.Reflection.FieldInfo currencyDistributionField = typeof(LootSourceTemplate).GetField("currencyDistribution", 
                    System.Reflection.BindingFlags.NonPublic | 
                    System.Reflection.BindingFlags.Instance);
                System.Reflection.FieldInfo allowedLootCategoriesField = typeof(LootSourceTemplate).GetField("allowedLootCategories", 
                    System.Reflection.BindingFlags.NonPublic | 
                    System.Reflection.BindingFlags.Instance);
                System.Reflection.FieldInfo experienceSourceField = typeof(LootSourceTemplate).GetField("experienceSource", 
                    System.Reflection.BindingFlags.NonPublic | 
                    System.Reflection.BindingFlags.Instance);

                string currentTemplateName = (string)templateNameField.GetValue(newTemplate);
                string newTemplateName = EditorGUILayout.TextField(
                    new GUIContent("Template Name", "Unique identifier for this template"), 
                    currentTemplateName);
                if (newTemplateName != currentTemplateName)
                {
                    templateNameField.SetValue(newTemplate, newTemplateName);
                }

                LootTier currentTier = (LootTier)tierField.GetValue(newTemplate);
                LootTier newTier = (LootTier)EditorGUILayout.EnumPopup(
                    new GUIContent("Loot Tier", "Determines the quality and drop rates of items"), 
                    currentTier);
                if (newTier != currentTier)
                {
                    tierField.SetValue(newTemplate, newTier);
                }

                CurrencyDistribution currentCurrencyDistribution = (CurrencyDistribution)currencyDistributionField.GetValue(newTemplate);
                CurrencyDistribution newCurrencyDistribution = (CurrencyDistribution)EditorGUILayout.ObjectField(
                    new GUIContent("Currency Distribution", "Defines how currency drops are distributed"), 
                    currentCurrencyDistribution, 
                    typeof(CurrencyDistribution), 
                    false);
                if (newCurrencyDistribution != currentCurrencyDistribution)
                {
                    currencyDistributionField.SetValue(newTemplate, newCurrencyDistribution);
                }

                LootCategory currentAllowedCategories = (LootCategory)allowedLootCategoriesField.GetValue(newTemplate);
                LootCategory newAllowedCategories = (LootCategory)EditorGUILayout.EnumFlagsField(
                    new GUIContent("Allowed Categories", "Types of items that can drop from this template"), 
                    currentAllowedCategories);
                if (newAllowedCategories != currentAllowedCategories)
                {
                    allowedLootCategoriesField.SetValue(newTemplate, newAllowedCategories);
                }

                ExperienceSource currentExperienceSource = (ExperienceSource)experienceSourceField.GetValue(newTemplate);
                ExperienceSource newExperienceSource = (ExperienceSource)EditorGUILayout.ObjectField(
                    new GUIContent("Experience Source", "Defines how much experience is awarded when this template is used"), 
                    currentExperienceSource, 
                    typeof(ExperienceSource), 
                    false);
                if (newExperienceSource != currentExperienceSource)
                {
                    experienceSourceField.SetValue(newTemplate, newExperienceSource);
                }
                EditorGUILayout.EndVertical();

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(newTemplate);
                }

                EditorGUILayout.Space(5);
                
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Create Template"))
                {
                    CreateNewTemplate();
                }
                if (GUILayout.Button("Cancel"))
                {
                    newTemplate = null;
                    showNewTemplateSection = false;
                }
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);
        }

        private void DrawDefaultTemplatesSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            showDefaultTemplatesSection = EditorGUILayout.Foldout(showDefaultTemplatesSection, "Default Templates", true);
            
            if (showDefaultTemplatesSection)
            {
                EditorGUILayout.HelpBox(
                    "Default templates are used when no specific template is assigned to an enemy. " +
                    "They are selected based on enemy type and level ranges.", 
                    MessageType.Info);
                EditorGUILayout.Space(5);
                
                // Draw default template rules
                EditorGUILayout.LabelField("Default Template Rules", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(
                    "Rules define which template to use for enemies based on their type and level. " +
                    "Rules are checked in order, and the first matching rule is used.", 
                    MessageType.Info);

                System.Reflection.FieldInfo defaultRulesField = typeof(LootDatabase).GetField("defaultTemplateRules", 
                    System.Reflection.BindingFlags.NonPublic | 
                    System.Reflection.BindingFlags.Instance);
                List<LootDatabase.DefaultTemplateRule> defaultRules = (List<LootDatabase.DefaultTemplateRule>)defaultRulesField.GetValue(database);
                
                for (int i = 0; i < defaultRules.Count; i++)
                {
                    LootDatabase.DefaultTemplateRule rule = defaultRules[i];
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    
                    EditorGUI.BeginChangeCheck();

                    System.Reflection.FieldInfo enemyTypeField = typeof(LootDatabase.DefaultTemplateRule).GetField("enemyType", 
                        System.Reflection.BindingFlags.NonPublic | 
                        System.Reflection.BindingFlags.Instance);
                    System.Reflection.FieldInfo minLevelField = typeof(LootDatabase.DefaultTemplateRule).GetField("minLevel", 
                        System.Reflection.BindingFlags.NonPublic | 
                        System.Reflection.BindingFlags.Instance);
                    System.Reflection.FieldInfo maxLevelField = typeof(LootDatabase.DefaultTemplateRule).GetField("maxLevel", 
                        System.Reflection.BindingFlags.NonPublic | 
                        System.Reflection.BindingFlags.Instance);
                    System.Reflection.FieldInfo templateField = typeof(LootDatabase.DefaultTemplateRule).GetField("template", 
                        System.Reflection.BindingFlags.NonPublic | 
                        System.Reflection.BindingFlags.Instance);

                    EnemyType currentEnemyType = (EnemyType)enemyTypeField.GetValue(rule);
                    EnemyType newEnemyType = (EnemyType)EditorGUILayout.EnumPopup(
                        new GUIContent("Enemy Category", "Type of enemy this rule applies to"), 
                        currentEnemyType);
                    if (newEnemyType != currentEnemyType)
                    {
                        enemyTypeField.SetValue(rule, newEnemyType);
                    }

                    int currentMinLevel = (int)minLevelField.GetValue(rule);
                    int newMinLevel = EditorGUILayout.IntField(
                        new GUIContent("Min Level", "Minimum enemy level for this rule"), 
                        currentMinLevel);
                    if (newMinLevel != currentMinLevel)
                    {
                        minLevelField.SetValue(rule, newMinLevel);
                    }

                    int currentMaxLevel = (int)maxLevelField.GetValue(rule);
                    int newMaxLevel = EditorGUILayout.IntField(
                        new GUIContent("Max Level", "Maximum enemy level for this rule"), 
                        currentMaxLevel);
                    if (newMaxLevel != currentMaxLevel)
                    {
                        maxLevelField.SetValue(rule, newMaxLevel);
                    }

                    LootSourceTemplate currentTemplate = (LootSourceTemplate)templateField.GetValue(rule);
                    LootSourceTemplate newTemplate = (LootSourceTemplate)EditorGUILayout.ObjectField(
                        new GUIContent("Template", "Template to use for matching enemies"), 
                        currentTemplate, 
                        typeof(LootSourceTemplate), 
                        false);
                    if (newTemplate != currentTemplate)
                    {
                        templateField.SetValue(rule, newTemplate);
                    }
                    
                    if (EditorGUI.EndChangeCheck())
                    {
                        EditorUtility.SetDirty(database);
                    }
                    
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Remove Rule"))
                    {
                        defaultRules.RemoveAt(i);
                        EditorUtility.SetDirty(database);
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space(5);
                }
                
                if (GUILayout.Button("Add New Rule"))
                {
                    defaultRules.Add(new LootDatabase.DefaultTemplateRule());
                    EditorUtility.SetDirty(database);
                }
                
                EditorGUILayout.Space(10);
                
                // Draw existing templates
                EditorGUILayout.LabelField("Available Templates", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(
                    "These are all the loot templates available in the project. " +
                    "Use the search bar above to filter templates by name.", 
                    MessageType.Info);
                
                string[] guids = AssetDatabase.FindAssets("t:LootSourceTemplate");
                List<LootSourceTemplate> templates = new List<LootSourceTemplate>();
                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    LootSourceTemplate template = AssetDatabase.LoadAssetAtPath<LootSourceTemplate>(path);
                    if (template != null)
                    {
                        templates.Add(template);
                    }
                }

                List<LootSourceTemplate> filteredTemplates = templates
                    .Where(t => string.IsNullOrEmpty(searchFilter) || 
                               t.TemplateName.ToLower().Contains(searchFilter.ToLower()))
                    .ToList();

                foreach (LootSourceTemplate template in filteredTemplates)
                {
                    if (!templateFoldouts.ContainsKey(template))
                    {
                        templateFoldouts[template] = false;
                    }

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    templateFoldouts[template] = EditorGUILayout.Foldout(
                        templateFoldouts[template], 
                        template.TemplateName, 
                        true);

                    if (templateFoldouts[template])
                    {
                        EditorGUI.BeginChangeCheck();
                        
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        EditorGUILayout.LabelField(
                            new GUIContent("Template Name", "Unique identifier for this template"), 
                            template.TemplateName);
                        EditorGUILayout.LabelField(
                            new GUIContent("Loot Tier", "Determines the quality and drop rates of items"), 
                            template.Tier.ToString());
                        EditorGUILayout.ObjectField(
                            new GUIContent("Currency Distribution", "Defines how currency drops are distributed"), 
                            template.CurrencyDistribution, 
                            typeof(CurrencyDistribution), 
                            false);
                        EditorGUILayout.LabelField(
                            new GUIContent("Allowed Categories", "Types of items that can drop from this template"), 
                            template.AllowedLootCategories.ToString());
                        EditorGUILayout.ObjectField(
                            new GUIContent("Experience Source", "Defines how much experience is awarded when this template is used"), 
                            template.ExperienceSource, 
                            typeof(ExperienceSource), 
                            false);
                        EditorGUILayout.EndVertical();

                        if (EditorGUI.EndChangeCheck())
                        {
                            EditorUtility.SetDirty(template);
                        }

                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Delete Template"))
                        {
                            if (EditorUtility.DisplayDialog(
                                "Delete Template",
                                $"Are you sure you want to delete the template '{template.TemplateName}'?",
                                "Yes",
                                "No"))
                            {
                                DeleteTemplate(template);
                                break;
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space(5);
                }
            }
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);
        }

        private void DrawLootConfigurationsSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            showLootConfigurationsSection = EditorGUILayout.Foldout(showLootConfigurationsSection, "Loot Configurations", true);
            
            if (showLootConfigurationsSection)
            {
                EditorGUILayout.HelpBox(
                    "Loot configurations link specific enemy types to loot templates. " +
                    "When an enemy dies, it will use its assigned template or fall back to a default template based on its type and level.", 
                    MessageType.Info);
                EditorGUILayout.Space(5);

                System.Reflection.FieldInfo configurationsField = typeof(LootDatabase).GetField("lootConfigurations", 
                    System.Reflection.BindingFlags.NonPublic | 
                    System.Reflection.BindingFlags.Instance);
                List<LootDatabase.LootConfiguration> configurations = (List<LootDatabase.LootConfiguration>)configurationsField.GetValue(database);
                
                foreach (LootDatabase.LootConfiguration config in configurations)
                {
                    if (!configFoldouts.ContainsKey(config))
                    {
                        configFoldouts[config] = false;
                    }

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    configFoldouts[config] = EditorGUILayout.Foldout(
                        configFoldouts[config], 
                        $"Configuration: {config.EnemyType}", 
                        true);

                    if (configFoldouts[config])
                    {
                        EditorGUI.BeginChangeCheck();
                        
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        System.Reflection.FieldInfo enemyTypeField = typeof(LootDatabase.LootConfiguration).GetField("enemyType", 
                            System.Reflection.BindingFlags.NonPublic | 
                            System.Reflection.BindingFlags.Instance);
                        System.Reflection.FieldInfo templateField = typeof(LootDatabase.LootConfiguration).GetField("lootTemplate", 
                            System.Reflection.BindingFlags.NonPublic | 
                            System.Reflection.BindingFlags.Instance);

                        EnemyType currentEnemyType = (EnemyType)enemyTypeField.GetValue(config);
                        EnemyType newEnemyType = (EnemyType)EditorGUILayout.EnumPopup(
                            new GUIContent("Enemy Type", "Type of enemy this configuration applies to"), 
                            currentEnemyType);
                        if (newEnemyType != currentEnemyType)
                        {
                            enemyTypeField.SetValue(config, newEnemyType);
                        }

                        LootSourceTemplate currentTemplate = (LootSourceTemplate)templateField.GetValue(config);
                        LootSourceTemplate newTemplate = (LootSourceTemplate)EditorGUILayout.ObjectField(
                            new GUIContent("Template", "Template to use for this enemy type"), 
                            currentTemplate, 
                            typeof(LootSourceTemplate), 
                            false);
                        if (newTemplate != currentTemplate)
                        {
                            templateField.SetValue(config, newTemplate);
                        }
                        EditorGUILayout.EndVertical();

                        if (EditorGUI.EndChangeCheck())
                        {
                            EditorUtility.SetDirty(database);
                        }

                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Delete Configuration"))
                        {
                            if (EditorUtility.DisplayDialog(
                                "Delete Configuration",
                                $"Are you sure you want to delete the configuration for '{config.EnemyType}'?",
                                "Yes",
                                "No"))
                            {
                                configurations.Remove(config);
                                EditorUtility.SetDirty(database);
                                break;
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space(5);
                }
                
                if (GUILayout.Button("Add New Configuration"))
                {
                    configurations.Add(new LootDatabase.LootConfiguration());
                    EditorUtility.SetDirty(database);
                }
            }
            
            EditorGUILayout.EndVertical();
        }

        private void CreateNewTemplate()
        {
            if (string.IsNullOrEmpty(newTemplate.TemplateName))
            {
                EditorUtility.DisplayDialog("Error", "Template name cannot be empty!", "OK");
                return;
            }

            string[] guids = AssetDatabase.FindAssets("t:LootSourceTemplate");
            foreach (string guid in guids)
            {
                string pathID = AssetDatabase.GUIDToAssetPath(guid);
                LootSourceTemplate existingTemplate = AssetDatabase.LoadAssetAtPath<LootSourceTemplate>(pathID);
                if (existingTemplate != null && existingTemplate.TemplateName == newTemplate.TemplateName)
                {
                    EditorUtility.DisplayDialog("Error", "A template with this name already exists!", "OK");
                    return;
                }
            }

            string path = EditorUtility.SaveFilePanelInProject(
                "Save Template",
                newTemplate.TemplateName,
                "asset",
                "Please enter a file name to save the template to");

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            AssetDatabase.CreateAsset(newTemplate, path);
            EditorUtility.SetDirty(newTemplate);
            AssetDatabase.SaveAssets();

            newTemplate = null;
            showNewTemplateSection = false;
        }

        private void DeleteTemplate(LootSourceTemplate template)
        {
            // Remove from default template rules
            System.Reflection.FieldInfo defaultRulesField = typeof(LootDatabase).GetField("defaultTemplateRules", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);
            List<LootDatabase.DefaultTemplateRule> defaultRules = (List<LootDatabase.DefaultTemplateRule>)defaultRulesField.GetValue(database);
            
            for (int i = 0; i < defaultRules.Count; i++)
            {
                LootDatabase.DefaultTemplateRule rule = defaultRules[i];
                System.Reflection.FieldInfo templateField = typeof(LootDatabase.DefaultTemplateRule).GetField("template", 
                    System.Reflection.BindingFlags.NonPublic | 
                    System.Reflection.BindingFlags.Instance);
                if ((LootSourceTemplate)templateField.GetValue(rule) == template)
                {
                    templateField.SetValue(rule, null);
                }
            }

            // Remove from configurations
            System.Reflection.FieldInfo configurationsField = typeof(LootDatabase).GetField("lootConfigurations", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);
            List<LootDatabase.LootConfiguration> configurations = (List<LootDatabase.LootConfiguration>)configurationsField.GetValue(database);
            
            foreach (LootDatabase.LootConfiguration config in configurations)
            {
                System.Reflection.FieldInfo templateField = typeof(LootDatabase.LootConfiguration).GetField("lootTemplate", 
                    System.Reflection.BindingFlags.NonPublic | 
                    System.Reflection.BindingFlags.Instance);
                if ((LootSourceTemplate)templateField.GetValue(config) == template)
                {
                    templateField.SetValue(config, null);
                }
            }

            // Delete asset
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(template));
            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(database);
        }
    }
} 