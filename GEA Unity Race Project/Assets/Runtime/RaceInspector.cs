using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Race))]
public class RaceInspector : Editor
{
    Race selectedRace;

    // Name
    string raceName;

    // Timer
    bool foldoutTimer = true;
    bool timer;
    int initialTime;
    int timePerPoint;

    // Laps
    int laps;

    // Starting Line
    bool foldoutStart = true;
        // Starting Line Model
    GameObject startLine;

    // Finish Line
    bool foldoutFinish = true;
        // Finish Line Model
    GameObject finishLine;

    // Checkpoints
    bool foldoutCheckpoint = true;
    List<GameObject> checkpoints;

    public override void OnInspectorGUI()
    {
        selectedRace = (Race)target;

        NameGUI();
        TimerGUI();
        LapsGUI();
        StartGUI();
        finishGUI();
        checkpointGUI();

        //base.OnInspectorGUI();

        //EditorGUILayout.HelpBox(" Only modify the checkpoint list using the buttons above!", MessageType.Warning);
    }

    private void NameGUI()
    {
        raceName = selectedRace.name;
        raceName = EditorGUILayout.TextField("Race Name", raceName);
        selectedRace.updateName(raceName);
        EditorGUILayout.Space();
    }

    private void TimerGUI()
    {
        foldoutTimer = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutTimer, "Race Timer");
        EditorGUI.indentLevel++;

        if (foldoutTimer)
        {
            timer = selectedRace.timer;
            timer = EditorGUILayout.Toggle("Timer", timer);
            selectedRace.timer = timer;

            if (timer)
            {
                initialTime = selectedRace.initialTime;
                initialTime = Mathf.Clamp(EditorGUILayout.IntField("Initial Time", initialTime), 0, int.MaxValue);
                selectedRace.initialTime = initialTime;

                timePerPoint = selectedRace.timePerPoint;
                timePerPoint = Mathf.Clamp(EditorGUILayout.IntField("Time per Checkpoint", timePerPoint), 0, int.MaxValue);
                selectedRace.timePerPoint = timePerPoint;
            }
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space();
    }

    private void LapsGUI()
    {
        laps = selectedRace.laps;
        laps = Mathf.Clamp(EditorGUILayout.IntField("Number of Laps", laps), 0, int.MaxValue);
        selectedRace.laps = laps;

        EditorGUILayout.Space();
    }

    private void StartGUI()
    {
        foldoutStart = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutStart, "Starting Line");
        EditorGUI.indentLevel++;

        if (foldoutStart)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel("Starting Line Object");

            startLine = selectedRace.startLine;
            startLine = (GameObject)EditorGUILayout.ObjectField(startLine, typeof(GameObject), true);

            EditorGUILayout.EndHorizontal();
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space();
    }

    private void finishGUI()
    {
        foldoutFinish = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutFinish, "Finish Line");
        EditorGUI.indentLevel++;

        if (foldoutFinish)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel("Finish Line Object");

            finishLine = selectedRace.finishLine;
            finishLine = (GameObject)EditorGUILayout.ObjectField(finishLine, typeof(GameObject), true);

            EditorGUILayout.EndHorizontal();
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space();
    }

    private void checkpointGUI()
    {
        foldoutCheckpoint = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutCheckpoint, "Checkpoints");
        EditorGUI.indentLevel++;

        if (foldoutCheckpoint)
        {
            checkpoints = selectedRace.checkpoints;

            int i = 0;
            foreach (GameObject cpObject in checkpoints)
            {
                EditorGUILayout.BeginHorizontal();

                //EditorGUILayout.PrefixLabel(i + ". ");

                GameObject newCpObject = cpObject;
                newCpObject = (GameObject)EditorGUILayout.ObjectField(newCpObject, typeof(GameObject), true);

                if (GUILayout.Button("Remove"))
                {
                    removeCheckpoint(i);
                }

                i++;

                EditorGUILayout.EndHorizontal();
            }
        }
    }

    private void removeCheckpoint(int index)
    {
        Debug.Log("Removed Checkpoint " + index);
    }
}
