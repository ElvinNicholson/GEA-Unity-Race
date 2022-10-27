using UnityEngine;
using UnityEditor;

public class RaceCreator : EditorWindow
{
    // Name
    string raceName = "New Race";

    bool addUI = false;

    // Timer
    bool foldoutTimer = true;
    bool stopwatch = false;
    bool timer = false;
    float initialTime = 0;
    float timePerPoint = 0;

    // Laps
    int laps = 0;

    // Checkpoint
    int checkpointNum = 0;


    [MenuItem("GameObject/Race Creator")]
    public static void ShowWindow()
    {
        GetWindow<RaceCreator>("Race Creator");
    }

    private void OnGUI()
    {
        NameGUI();
        LapsGUI();
        TimerGUI();
        CheckpointGUI();
        ButtonGUI();
    }

    private void NameGUI()
    {
        GUILayout.Label("Create a new race");
        EditorGUILayout.Space();
        raceName = EditorGUILayout.TextField("Race Name", raceName);

        EditorGUILayout.Space();

        addUI = EditorGUILayout.Toggle("Add Default UI", addUI);
    }

    private void TimerGUI()
    {
        foldoutTimer = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutTimer, "Race Timer");
        EditorGUI.indentLevel++;

        if (foldoutTimer)
        {
            EditorGUILayout.LabelField("Adds a stopwatch to the race");
            stopwatch = EditorGUILayout.Toggle("Stopwatch", stopwatch);
            if (stopwatch)
            {
                timer = false;

                initialTime = 0;
                timePerPoint = 0;
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Adds a timer to the race");
            timer = EditorGUILayout.Toggle("Timer", timer);

            if (timer)
            {
                stopwatch = false;

                initialTime = Mathf.Clamp(EditorGUILayout.FloatField("Initial Time", initialTime), 0, int.MaxValue);
                timePerPoint = Mathf.Clamp(EditorGUILayout.FloatField("Time per Checkpoint", timePerPoint), 0, int.MaxValue);
            }
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space();
    }

    private void LapsGUI()
    {
        laps = Mathf.Clamp(EditorGUILayout.IntField("Number of Laps", laps), 1, int.MaxValue);

        EditorGUILayout.Space();
    }

    private void CheckpointGUI()
    {
        checkpointNum = Mathf.Clamp(EditorGUILayout.IntField("Number of Checkpoints", checkpointNum), 0, int.MaxValue);
        EditorGUILayout.Space();
    }

    private void ButtonGUI()
    {
        if (GUILayout.Button("Create New Race"))
        {
            Debug.Log("Created new race");
            createNewRace();
        }
    }

    private void createNewRace()
    {
        GameObject newRace = new GameObject();
        newRace.name = raceName;
        newRace.AddComponent<Race>();

        Race newRaceComponent = newRace.GetComponent<Race>();
        newRaceComponent.CreateRace(checkpointNum);

        newRaceComponent.timer = timer;
        newRaceComponent.stopwatch = stopwatch;
        newRaceComponent.initialTime = initialTime;
        newRaceComponent.timePerPoint = timePerPoint;
        newRaceComponent.raceInfo.lapsTotal = laps;

        if (addUI)
        {
            newRaceComponent.InitialiseUI();
        }
    }
}
