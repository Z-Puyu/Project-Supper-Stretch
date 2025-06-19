using UnityEditor;
using UnityEngine;

namespace Editor;

public static class UIFrameWorkUtils {
    private const string CoreComponentsDirectory = "Assets/Resources/Prefabs/UI/CoreComponents/";

    [MenuItem("GameObject/Custom UI Framework/Containers/View", false, 0)]
    public static void CreateView(MenuCommand command) {
        UIFrameWorkUtils.Create(command, UIFrameWorkUtils.CoreComponentsDirectory + "Containers/View.prefab");
    }

    [MenuItem("GameObject/Custom UI Framework/Containers/Box Container", false, 0)]
    public static void CreateBoxContainer(MenuCommand command) {
        UIFrameWorkUtils.Create(command, UIFrameWorkUtils.CoreComponentsDirectory + "Containers/BoxContainer.prefab");
    }
    
    [MenuItem("GameObject/Custom UI Framework/Containers/Free-Sized Panel", false, 0)]
    public static void CreateFreeSizePanel(MenuCommand command) {
        UIFrameWorkUtils.Create(command, UIFrameWorkUtils.CoreComponentsDirectory + "Containers/FreeSizePanel.prefab");
    }
    
    [MenuItem("GameObject/Custom UI Framework/Containers/Bounding Box", false, 0)]
    public static void CreateBoundingBox(MenuCommand command) {
        UIFrameWorkUtils.Create(command, UIFrameWorkUtils.CoreComponentsDirectory + "Containers/BoundingBox.prefab");
    }
    
    [MenuItem("GameObject/Custom UI Framework/Containers/Vertical List", false, 0)]
    public static void CreateVerticalList(MenuCommand command) {
        UIFrameWorkUtils.Create(command, UIFrameWorkUtils.CoreComponentsDirectory + "Containers/VerticalList.prefab");
    }
    
    [MenuItem("GameObject/Custom UI Framework/Containers/Horizontal List", false, 0)]
    public static void CreateHorizontalList(MenuCommand command) {
        UIFrameWorkUtils.Create(command, UIFrameWorkUtils.CoreComponentsDirectory + "Containers/HorizontalList.prefab");
    }
    
    [MenuItem("GameObject/Custom UI Framework/Buttons/Button", false, 0)]
    public static void CreateButton(MenuCommand command) {
        UIFrameWorkUtils.Create(command, UIFrameWorkUtils.CoreComponentsDirectory + "Buttons/Button.prefab");
    }
    
    [MenuItem("GameObject/Custom UI Framework/Buttons/RadioButton", false, 0)]
    public static void CreateRadioButton(MenuCommand command) {
        UIFrameWorkUtils.Create(command, UIFrameWorkUtils.CoreComponentsDirectory + "Buttons/RadioButton.prefab");
    }

    
    [MenuItem("GameObject/Custom UI Framework/Buttons/Radio Buttons Group", false, 0)]
    public static void CreateRadioButtonsGroup(MenuCommand command) {
        UIFrameWorkUtils.Create(command, UIFrameWorkUtils.CoreComponentsDirectory + "Buttons/RadioButtonsGroup.prefab");
    }
    
    [MenuItem("GameObject/Custom UI Framework/Texts/Text", false, 0)]
    public static void CreateText(MenuCommand command) {
        UIFrameWorkUtils.Create(command, UIFrameWorkUtils.CoreComponentsDirectory + "Texts/Text.prefab");
    }

    [MenuItem("GameObject/Custom UI Framework/Texts/Text Box", false, 0)]
    public static void CreateTextBox(MenuCommand command) {
        UIFrameWorkUtils.Create(command, UIFrameWorkUtils.CoreComponentsDirectory + "Texts/TextBox.prefab");
    }
    
    [MenuItem("GameObject/Custom UI Framework/Texts/UI Label", false, 0)]
    public static void CreateLabel(MenuCommand command) {
        UIFrameWorkUtils.Create(command, UIFrameWorkUtils.CoreComponentsDirectory + "Texts/LabelText.prefab");
    }
    
    [MenuItem("GameObject/Custom UI Framework/World Space/World Space Widget Component", false, 0)]
    public static void CreateWorldSpaceWidget(MenuCommand command) {
        UIFrameWorkUtils.Create(command, UIFrameWorkUtils.CoreComponentsDirectory + "WorldSpace/WorldSpaceWidget.prefab");
    }
    
    [MenuItem("GameObject/Custom UI Framework/World Space/Interaction Prompt", false, 0)]
    public static void CreateInteractionPrompt(MenuCommand command) {
        UIFrameWorkUtils.Create(command, UIFrameWorkUtils.CoreComponentsDirectory + "WorldSpace/InteractionPrompt.prefab");
    }

    [MenuItem("GameObject/Custom UI Framework/Health Bar", false, 0)]
    public static void CreateHealthBar(MenuCommand command) {
        UIFrameWorkUtils.Create(command, UIFrameWorkUtils.CoreComponentsDirectory + "ProgressBars/HealthBar.prefab");
    }

    private static void Create(MenuCommand cmd, string path) {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (!prefab) {
            Debug.LogError("Prefab not found at path: " + path);
            return;
        }

        GameObject? parent = cmd.context as GameObject;
        if (!parent) {
            parent = Selection.activeGameObject;
        }

        Object instance = PrefabUtility.InstantiatePrefab(prefab, parent.transform);
        Undo.RegisterCreatedObjectUndo(instance, "Create " + prefab.name);
        Selection.activeGameObject = instance as GameObject;
    }
}
