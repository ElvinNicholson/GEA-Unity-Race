using UnityEngine;
using UnityEditor;

public class RaceCreator : EditorWindow
{
    // Name
    string raceName = "New Race";

    // Timer
    bool foldoutTimer = true;
    bool timer = false;
    int initialTime = 0;
    int timePerPoint = 0;

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
    }

    private void TimerGUI()
    {
        foldoutTimer = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutTimer, "Race Timer");
        EditorGUI.indentLevel++;

        if (foldoutTimer)
        {
            EditorGUILayout.LabelField("Adds a timer to the race");
            timer = EditorGUILayout.Toggle("Timer", timer);

            if (timer)
            {
                initialTime = Mathf.Clamp(EditorGUILayout.IntField("Initial Time", initialTime), 0, int.MaxValue);
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("The amount of time a checkpoint gives");
                timePerPoint = Mathf.Clamp(EditorGUILayout.IntField("Time per Checkpoint", timePerPoint), 0, int.MaxValue);
            }
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space();
    }

    private void LapsGUI()
    {
        laps = Mathf.Clamp(EditorGUILayout.IntField("Number of Laps", laps), 0, int.MaxValue);

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
        newRaceComponent.timer = timer;
        newRaceComponent.initialTime = initialTime;
        newRaceComponent.timePerPoint = timePerPoint;
        newRaceComponent.laps = laps;
        //newRaceComponent.checkpointNum = checkpointNum;

        newRaceComponent.CreateRace(checkpointNum);
    }
}
