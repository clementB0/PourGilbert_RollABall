using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Ce script ajoute "Outils > Installer victoire, défaite, sons et niveaux".
// Il installe tout dans les scènes. Il ne sert que dans l'éditeur.
public static class GameSetupCreator
{
    private const string OldScenePath = "Assets/Scenes/SampleScene.unity";
    private const string Level1Path = "Assets/Scenes/Level1.unity";
    private const string Level2Path = "Assets/Scenes/Level2.unity";
    private const string MenuPath = "Assets/Scenes/Menu.unity";
    private const string BallPrefabPath = "Assets/Prefabs/Ball.prefab";

    [MenuItem("Outils/Installer victoire, défaite, sons et niveaux")]
    private static void Install()
    {
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(Level1Path) != null)
        {
            Debug.LogWarning("C'est déjà installé : Level1 existe déjà.");
            return;
        }

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        AddSoundsToBall();

        // SampleScene devient le niveau 1, et on le copie pour faire le niveau 2
        AssetDatabase.MoveAsset(OldScenePath, Level1Path);
        AssetDatabase.CopyAsset(Level1Path, Level2Path);

        // Niveau 1 : 60 secondes pour faire 250 points, le mur apparaît à 100 points
        Scene level1 = EditorSceneManager.OpenScene(Level1Path);
        SetupLevel(60f, 250, 100, false);
        EditorSceneManager.SaveScene(level1);

        // Niveau 2 : plus de quilles, 45 secondes pour faire 600 points
        Scene level2 = EditorSceneManager.OpenScene(Level2Path);
        AddLevel2Pins();
        SetupLevel(45f, 600, 300, true);
        EditorSceneManager.SaveScene(level2);

        // Le bouton Start du menu lance le niveau 1
        Scene menu = EditorSceneManager.OpenScene(MenuPath);
        StartButton startButton = Object.FindFirstObjectByType<StartButton>();
        SetValue(startButton, "gameSceneName", "Level1");
        EditorSceneManager.SaveScene(menu);

        // Ordre du build : menu, niveau 1, niveau 2
        EditorBuildSettings.scenes = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene(MenuPath, true),
            new EditorBuildSettingsScene(Level1Path, true),
            new EditorBuildSettingsScene(Level2Path, true)
        };

        Debug.Log("Installation terminée !");
    }

    // ---------- Sons de la boule ----------

    private static void AddSoundsToBall()
    {
        GameObject ball = PrefabUtility.LoadPrefabContents(BallPrefabPath);

        BallHitSound hitSound = ball.AddComponent<BallHitSound>();
        SetValue(hitSound, "pinSound", CreateAudioSource(ball, "pin"));
        SetValue(hitSound, "wallSound", CreateAudioSource(ball, "wall"));

        PrefabUtility.SaveAsPrefabAsset(ball, BallPrefabPath);
        PrefabUtility.UnloadPrefabContents(ball);
    }

    private static AudioSource CreateAudioSource(GameObject target, string soundName)
    {
        AudioSource source = target.AddComponent<AudioSource>();
        source.clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/" + soundName + ".wav");
        source.playOnAwake = false;
        return source;
    }

    // ---------- Un niveau complet ----------

    private static void SetupLevel(float timeLimit, int scoreToWin, int wallScore, bool isLastLevel)
    {
        GameObject canvas = Object.FindFirstObjectByType<Canvas>().gameObject;
        ScoreManager scoreManager = Object.FindFirstObjectByType<ScoreManager>();

        GameObject manager = new GameObject("GameManager");

        // Victoire / défaite
        GameObject winPanel = CreateWinPanel(canvas, isLastLevel);
        GameObject losePanel = CreateLosePanel(canvas);

        GameResult gameResult = manager.AddComponent<GameResult>();
        SetValue(gameResult, "winPanel", winPanel);
        SetValue(gameResult, "losePanel", losePanel);
        SetValue(gameResult, "winSound", CreateAudioSource(manager, "win"));
        SetValue(gameResult, "loseSound", CreateAudioSource(manager, "lose"));

        // Timer : quand il arrive à 0, c'est perdu
        LevelTimer timer = manager.AddComponent<LevelTimer>();
        SetValue(timer, "gameResult", gameResult);
        SetValue(timer, "timeLimit", timeLimit);

        // Score à atteindre pour gagner
        WinByScore winByScore = manager.AddComponent<WinByScore>();
        SetValue(winByScore, "scoreManager", scoreManager);
        SetValue(winByScore, "gameResult", gameResult);
        SetValue(winByScore, "scoreToWin", scoreToWin);

        // Textes en haut au milieu de l'écran
        TMP_Text timerText = CreateTopText(canvas, "TimerText", -30);
        TimerDisplay timerDisplay = timerText.gameObject.AddComponent<TimerDisplay>();
        SetValue(timerDisplay, "levelTimer", timer);
        SetValue(timerDisplay, "timerText", timerText);

        TMP_Text goalText = CreateTopText(canvas, "GoalText", -75);
        ScoreGoalDisplay goalDisplay = goalText.gameObject.AddComponent<ScoreGoalDisplay>();
        SetValue(goalDisplay, "winByScore", winByScore);
        SetValue(goalDisplay, "goalText", goalText);

        // Mur qui apparaît quand on a assez de points
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "MurSurprise";
        wall.transform.position = new Vector3(6f, 3.25f, 0f);
        wall.transform.localScale = new Vector3(1f, 5f, 16f);
        wall.AddComponent<BallBouncer>();

        ShowWhenScore showWall = manager.AddComponent<ShowWhenScore>();
        SetValue(showWall, "scoreManager", scoreManager);
        SetValue(showWall, "scoreNeeded", wallScore);
        SetValue(showWall, "objectToShow", wall);

        // Les menus doivent être dessinés par-dessus le reste
        winPanel.transform.SetAsLastSibling();
        losePanel.transform.SetAsLastSibling();
    }

    private static void AddLevel2Pins()
    {
        AddPin("Pin_Green", new Vector3(-15f, 1.62f, -10f));
        AddPin("Pin_Green", new Vector3(10f, 1.62f, 18f));
        AddPin("Pin_Gold", new Vector3(-18f, 1.52f, 18f));
        AddPin("Pin_Red", new Vector3(0f, 1.46f, -15f));
        AddPin("Pin_Red", new Vector3(-10f, 1.46f, 8f));
    }

    private static void AddPin(string prefabName, Vector3 position)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/" + prefabName + ".prefab");
        GameObject pin = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        pin.transform.position = position;
    }

    // ---------- Menus de fin ----------

    private static GameObject CreateWinPanel(GameObject canvas, bool isLastLevel)
    {
        if (isLastLevel)
        {
            GameObject lastPanel = CreatePanel(canvas, "WinPanel", "Tu as fini le jeu !");
            AddMenuButton(lastPanel, -40);
            return lastPanel;
        }

        GameObject panel = CreatePanel(canvas, "WinPanel", "Victoire !");

        GameObject next = CreateButton(panel, "NextLevelButton", "Niveau suivant", 20);
        NextLevelButton nextButton = next.AddComponent<NextLevelButton>();
        UnityEventTools.AddPersistentListener(next.GetComponent<Button>().onClick, nextButton.LoadNextLevel);

        AddMenuButton(panel, -100);
        return panel;
    }

    private static GameObject CreateLosePanel(GameObject canvas)
    {
        GameObject panel = CreatePanel(canvas, "LosePanel", "Défaite... temps écoulé !");

        GameObject restart = CreateButton(panel, "RestartButton", "Rejouer", 20);
        RestartButton restartButton = restart.AddComponent<RestartButton>();
        UnityEventTools.AddPersistentListener(restart.GetComponent<Button>().onClick, restartButton.Restart);

        AddMenuButton(panel, -100);
        return panel;
    }

    private static void AddMenuButton(GameObject panel, float y)
    {
        GameObject menu = CreateButton(panel, "MainMenuButton", "Menu", y);
        MainMenuButton menuButton = menu.AddComponent<MainMenuButton>();
        UnityEventTools.AddPersistentListener(menu.GetComponent<Button>().onClick, menuButton.BackToMenu);
    }

    // ---------- Petits outils pour créer l'UI ----------

    private static GameObject CreatePanel(GameObject canvas, string name, string title)
    {
        // Fond noir transparent qui couvre tout l'écran
        GameObject panel = new GameObject(name, typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(canvas.transform, false);
        panel.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.75f);

        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        GameObject titleObject = TMP_DefaultControls.CreateText(new TMP_DefaultControls.Resources());
        titleObject.name = "Title";
        titleObject.transform.SetParent(panel.transform, false);

        TMP_Text titleText = titleObject.GetComponent<TMP_Text>();
        titleText.text = title;
        titleText.fontSize = 60;
        titleText.alignment = TextAlignmentOptions.Center;

        RectTransform titleRect = titleObject.GetComponent<RectTransform>();
        titleRect.sizeDelta = new Vector2(900, 100);
        titleRect.anchoredPosition = new Vector2(0, 140);

        return panel;
    }

    private static GameObject CreateButton(GameObject parent, string name, string text, float y)
    {
        TMP_DefaultControls.Resources resources = new TMP_DefaultControls.Resources();
        resources.standard = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");

        GameObject button = TMP_DefaultControls.CreateButton(resources);
        button.name = name;
        button.transform.SetParent(parent.transform, false);

        RectTransform rect = button.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(300, 70);
        rect.anchoredPosition = new Vector2(0, y);

        TMP_Text label = button.GetComponentInChildren<TMP_Text>();
        label.text = text;
        label.fontSize = 32;

        return button;
    }

    private static TMP_Text CreateTopText(GameObject canvas, string name, float y)
    {
        GameObject textObject = TMP_DefaultControls.CreateText(new TMP_DefaultControls.Resources());
        textObject.name = name;
        textObject.transform.SetParent(canvas.transform, false);

        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.sizeDelta = new Vector2(300, 45);
        rect.anchoredPosition = new Vector2(0, y);

        TMP_Text text = textObject.GetComponent<TMP_Text>();
        text.fontSize = 32;
        text.alignment = TextAlignmentOptions.Center;
        return text;
    }

    // ---------- Remplir les cases [SerializeField] private ----------

    private static void SetValue(Object component, string field, Object value)
    {
        SerializedObject serialized = new SerializedObject(component);
        serialized.FindProperty(field).objectReferenceValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetValue(Object component, string field, float value)
    {
        SerializedObject serialized = new SerializedObject(component);
        serialized.FindProperty(field).floatValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetValue(Object component, string field, int value)
    {
        SerializedObject serialized = new SerializedObject(component);
        serialized.FindProperty(field).intValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetValue(Object component, string field, string value)
    {
        SerializedObject serialized = new SerializedObject(component);
        serialized.FindProperty(field).stringValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }
}
