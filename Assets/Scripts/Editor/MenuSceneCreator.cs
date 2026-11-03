using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Ce script ajoute un menu "Outils > Créer la scène Menu" dans Unity.
// Il fabrique la scène du menu tout seul. Il ne sert que dans l'éditeur.
public static class MenuSceneCreator
{
    private const string MenuScenePath = "Assets/Scenes/Menu.unity";

    [MenuItem("Outils/Créer la scène Menu")]
    private static void CreateMenuScene()
    {
        // Propose de sauvegarder la scène ouverte avant de changer
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        // Nouvelle scène avec une caméra et une lumière
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        // Le Canvas qui contient les boutons
        GameObject canvas = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // L'EventSystem permet de cliquer sur les boutons
        new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));

        // Bouton Start
        GameObject startObject = CreateButton(canvas, "StartButton", "Start", 60);
        StartButton startButton = startObject.AddComponent<StartButton>();
        UnityEventTools.AddPersistentListener(startObject.GetComponent<Button>().onClick, startButton.StartGame);

        // Bouton Quitter
        GameObject quitObject = CreateButton(canvas, "QuitButton", "Quitter", -60);
        QuitButton quitButton = quitObject.AddComponent<QuitButton>();
        UnityEventTools.AddPersistentListener(quitObject.GetComponent<Button>().onClick, quitButton.QuitGame);

        EditorSceneManager.SaveScene(scene, MenuScenePath);
        PutMenuFirstInBuild();

        Debug.Log("Scène Menu créée !");
    }

    private static GameObject CreateButton(GameObject canvas, string name, string text, float y)
    {
        TMP_DefaultControls.Resources resources = new TMP_DefaultControls.Resources();
        resources.standard = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");

        GameObject button = TMP_DefaultControls.CreateButton(resources);
        button.name = name;
        button.transform.SetParent(canvas.transform, false);

        RectTransform rect = button.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(300, 80);
        rect.anchoredPosition = new Vector2(0, y);

        TMP_Text label = button.GetComponentInChildren<TMP_Text>();
        label.text = text;
        label.fontSize = 36;

        return button;
    }

    // Met la scène Menu en premier dans la liste du build,
    // comme ça c'est elle qui se lance au démarrage du jeu
    private static void PutMenuFirstInBuild()
    {
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();
        scenes.Add(new EditorBuildSettingsScene(MenuScenePath, true));

        foreach (EditorBuildSettingsScene oldScene in EditorBuildSettings.scenes)
        {
            if (oldScene.path != MenuScenePath)
                scenes.Add(oldScene);
        }

        EditorBuildSettings.scenes = scenes.ToArray();
    }
}
