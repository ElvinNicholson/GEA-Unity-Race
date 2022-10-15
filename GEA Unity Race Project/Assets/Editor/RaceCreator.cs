using UnityEngine;
using UnityEditor;

public class RaceCreator : EditorWindow
{

    // Timer
    bool foldoutTimer = true;
    bool timer = false;
    int initialTime = 0;
    int timePerPoint = 0;

    // Laps
    int laps = 0;

    // Starting Line
    bool foldoutStart = true;
    Object startModel;
    GameObject startObject;

    // Finish Line
    bool foldoutFinish = true;
    Object finishModel;
    GameObject finishObject;

    // Checkpoint
    bool foldoutCheckpoint = true;


    [MenuItem("GameObject/Race Creator")]
    public static void ShowWindow()
    {
        GetWindow<RaceCreator>("Race Creator");
    }

    private void OnGUI()
    {
        TimerGUI();
        LapsGUI();
        StartGUI();
        FinishGUI();
    }

    private void TimerGUI()
    {
        foldoutTimer = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutTimer, "Race Timer");
        EditorGUI.indentLevel++;

        if (foldoutTimer)
        {
            timer = EditorGUILayout.Toggle("Timer", timer);

            if (timer)
            {
                initialTime = Mathf.Clamp(EditorGUILayout.IntField("Initial Time", initialTime), 0, int.MaxValue);
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

    private void StartGUI()
    {
        foldoutStart = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutStart, "Starting Line");
        EditorGUI.indentLevel++;

        if (foldoutStart)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Model");
            startModel = EditorGUILayout.ObjectField(startModel, typeof(Object), true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("GameObject");
            startObject = (GameObject)EditorGUILayout.ObjectField(startObject, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space();
    }

    private void FinishGUI()
    {
        foldoutFinish = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutFinish, "Finish Line");
        EditorGUI.indentLevel++;

        if (foldoutFinish)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Model");
            finishModel = EditorGUILayout.ObjectField(finishModel, typeof(Object), true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Gameobject");
            finishObject = (GameObject)EditorGUILayout.ObjectField(finishObject, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space();
    }

    private void CheckpointGUI()
    {

    }
}
