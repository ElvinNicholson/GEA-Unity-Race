using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Race))]
public class RaceInspector : Editor
{
    Race selectedRace;

    // Name
    string raceName;

    // Player Reference
    Collider playerCol;

    // Timer
    bool foldoutTimer = true;
    bool timer;
    bool stopwatch;
    float initialTime;
    float timePerPoint;

    // Laps
    bool foldoutLaps = true;
    int laps;

    // Advanced Settings
    bool foldoutSettings = true;
    bool startOnAwake;

    // Starting Line
    bool foldoutStart = true;
    Mesh startModel;
    Material startMat;
    GameObject startLine;

    // Finish Line
    bool foldoutFinish = true;
    Mesh finishModel;
    Material finishMat;
    GameObject finishLine;

    // Checkpoints
    bool foldoutCheckpoint = true;
    Mesh checkpointModel;
    Mesh currentCheckpointModel;
    Mesh nextCheckpointModel;
    Material checkpointMat;
    Material currentCheckpointMat;
    Material nextCheckpointMat;
    List<GameObject> checkpoints;

    private void OnEnable()
    {
        selectedRace = (Race)target;

        // Name
        raceName = selectedRace.name;

        // Player Reference
        playerCol = selectedRace.playerCol;

        // Timer
        timer = selectedRace.timer;
        stopwatch = selectedRace.stopwatch;
        initialTime = selectedRace.initialTime;
        timePerPoint = selectedRace.timePerPoint;

        // Laps
        laps = selectedRace.raceInfo.lapsTotal;

        // Advanced Settings
        startOnAwake = selectedRace.raceInfo.raceIsRunning;

        // Starting Line
        startLine = selectedRace.startLine;
        startModel = startLine.GetComponent<MeshFilter>().sharedMesh;
        startMat = startLine.GetComponent<MeshRenderer>().sharedMaterial;

        // Finish Line
        finishLine = selectedRace.finishLine;
        finishModel = finishLine.GetComponent<MeshFilter>().sharedMesh;
        finishMat = finishLine.GetComponent<MeshRenderer>().sharedMaterial;

        // Checkpoint
        checkpoints = selectedRace.checkpoints;
        checkpointModel = selectedRace.checkpointModel;
        checkpointMat = selectedRace.checkpointMat;

        currentCheckpointModel = selectedRace.currentCheckpointModel;
        currentCheckpointMat = selectedRace.currentCheckpointMat;

        nextCheckpointModel = selectedRace.nextCheckpointModel;
        nextCheckpointMat = selectedRace.nextCheckpointMat;
    }

    public override void OnInspectorGUI()
    {
        NameGUI();
        PlayerGUI();
        LapsGUI();
        TimerGUI();
        SettingsGUI();
        StartGUI();
        FinishGUI();
        CheckpointGUI();

        //base.OnInspectorGUI();
    }

    private void NameGUI()
    {
        raceName = EditorGUILayout.TextField("Race Name", raceName);
        selectedRace.changeName(raceName);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Start Race"))
        {
            selectedRace.startRace();
        }

        if (GUILayout.Button("Restart Race"))
        {
            selectedRace.resetRace();
            selectedRace.startRace();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
    }

    private void TimerGUI()
    {
        foldoutTimer = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutTimer, "Race Timer");
        EditorGUI.indentLevel++;

        if (foldoutTimer)
        {
            stopwatch = EditorGUILayout.Toggle("Stopwatch", stopwatch);
            selectedRace.stopwatch = stopwatch;
            if (stopwatch)
            {
                timer = false;

                initialTime = 0;
                selectedRace.initialTime = 0;

                timePerPoint = 0;
                selectedRace.timePerPoint = 0;
            }

            EditorGUILayout.Space();

            timer = EditorGUILayout.Toggle("Timer", timer);
            selectedRace.timer = timer;

            if (timer)
            {
                stopwatch = false;

                initialTime = Mathf.Clamp(EditorGUILayout.FloatField("Initial Time", initialTime), 0, float.MaxValue);
                selectedRace.initialTime = initialTime;

                timePerPoint = Mathf.Clamp(EditorGUILayout.FloatField("Time per Checkpoint", timePerPoint), 0, float.MaxValue);
                selectedRace.timePerPoint = timePerPoint;
            }
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space();
    }

    private void PlayerGUI()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.PrefixLabel("Player Collider");

        EditorGUI.BeginChangeCheck();
        playerCol = (Collider)EditorGUILayout.ObjectField(playerCol, typeof(Collider), true);
        if (EditorGUI.EndChangeCheck())
        {
            selectedRace.playerCol = playerCol;
            foreach (GameObject gateObject in selectedRace.gateOrder)
            {
                gateObject.GetComponent<Gate>().playerCol = playerCol;
            }
        }

        EditorGUILayout.EndHorizontal();
        
        if (!playerCol)
        {
            EditorGUILayout.HelpBox("Player Collider is not assigned", MessageType.Warning);
        }
    }

    private void LapsGUI()
    {
        foldoutLaps = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutLaps, "Laps");
        EditorGUI.indentLevel++;

        if (foldoutLaps)
        {
            EditorGUI.BeginChangeCheck();
            laps = Mathf.Clamp(EditorGUILayout.IntField("Number of Laps", laps), 1, int.MaxValue);
            if (EditorGUI.EndChangeCheck())
            {
                //selectedRace.lapsTotal = laps;
                selectedRace.raceInfo.lapsTotal = laps;
            }
            else
            {
                laps = selectedRace.raceInfo.lapsTotal;
            }
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space();
    }

    private void SettingsGUI()
    {
        foldoutSettings = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutSettings, "Advanced Settings");
        EditorGUI.indentLevel++;

        if (foldoutSettings)
        {
            EditorGUI.BeginChangeCheck();
            startOnAwake = EditorGUILayout.Toggle("Start Race on Awake", startOnAwake);
            if (EditorGUI.EndChangeCheck())
            {
                selectedRace.raceInfo.raceIsRunning = startOnAwake;
            }

            if (GUILayout.Button("Combine Starting Line with Finish Line"))
            {
                finishLine.transform.parent = startLine.transform;
                finishLine.transform.localPosition = Vector3.zero;
                finishLine.GetComponent<MeshRenderer>().enabled = false;
            }
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space();
    }

    private void StartGUI()
    {
        foldoutStart = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutStart, "Starting Line");
        EditorGUI.indentLevel++;

        if (foldoutStart)
        {
            EditorGUILayout.BeginHorizontal();

            // Mesh
            EditorGUILayout.PrefixLabel("Starting Line Mesh");
            EditorGUI.BeginChangeCheck();
            startModel = (Mesh)EditorGUILayout.ObjectField(startModel, typeof(Mesh), true);
            if (EditorGUI.EndChangeCheck())
            {
                selectedRace.startLine.GetComponent<MeshFilter>().mesh = startModel;
                selectedRace.FitCollider(startLine.GetComponent<Renderer>(), startLine.GetComponent<BoxCollider>());
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            // Material
            EditorGUILayout.PrefixLabel("Starting Line Material");
            EditorGUI.BeginChangeCheck();
            startMat = (Material)EditorGUILayout.ObjectField(startMat, typeof(Material), true);
            if (EditorGUI.EndChangeCheck())
            {
                selectedRace.startLine.GetComponent<MeshRenderer>().material = startMat;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            // GameObject
            EditorGUILayout.PrefixLabel("Starting Line Object");
            startLine = (GameObject)EditorGUILayout.ObjectField(startLine, typeof(GameObject), true);

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

            // Model
            EditorGUILayout.PrefixLabel("Finish Line Mesh");
            EditorGUI.BeginChangeCheck();
            finishModel = (Mesh)EditorGUILayout.ObjectField(finishModel, typeof(Mesh), true);
            if (EditorGUI.EndChangeCheck())
            {
                selectedRace.finishLine.GetComponent<MeshFilter>().mesh = finishModel;
                selectedRace.FitCollider(finishLine.GetComponent<Renderer>(), finishLine.GetComponent<BoxCollider>());
            }
            

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            // Material
            EditorGUILayout.PrefixLabel("Finish Line Material");
            EditorGUI.BeginChangeCheck();
            finishMat = (Material)EditorGUILayout.ObjectField(finishMat, typeof(Material), true);
            if (EditorGUI.EndChangeCheck())
            {
                selectedRace.finishLine.GetComponent<MeshRenderer>().material = finishMat;
            }
            

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            // GameObject
            EditorGUILayout.PrefixLabel("Finish Line Object");
            finishLine = (GameObject)EditorGUILayout.ObjectField(finishLine, typeof(GameObject), true);

            EditorGUILayout.EndHorizontal();
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space();
    }

    private void CheckpointGUI()
    {
        foldoutCheckpoint = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutCheckpoint, "Checkpoints");
        EditorGUI.indentLevel++;

        if (foldoutCheckpoint)
        {
            EditorGUILayout.BeginHorizontal();

            // Model
            EditorGUILayout.PrefixLabel("Inactive Mesh");
            checkpointModel = (Mesh)EditorGUILayout.ObjectField(checkpointModel, typeof(Mesh), true);
            if (checkpointModel != selectedRace.checkpointModel)
            {
                selectedRace.checkpointModel = checkpointModel;
                foreach (GameObject cpObject in checkpoints)
                {
                    cpObject.GetComponent<MeshFilter>().mesh = checkpointModel;
                    selectedRace.FitCollider(cpObject.GetComponent<Renderer>(), cpObject.GetComponent<BoxCollider>());
                }
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            // Material
            EditorGUILayout.PrefixLabel("Inactive Material");
            checkpointMat = (Material)EditorGUILayout.ObjectField(checkpointMat, typeof(Material), true);
            if (checkpointMat != selectedRace.checkpointMat)
            {
                selectedRace.checkpointMat = checkpointMat;
                foreach (GameObject cpObject in checkpoints)
                {
                    cpObject.GetComponent<MeshRenderer>().material = checkpointMat;
                }
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            // Current Model
            EditorGUILayout.PrefixLabel("Active Mesh");
            currentCheckpointModel = (Mesh)EditorGUILayout.ObjectField(currentCheckpointModel, typeof(Mesh), true);
            if (currentCheckpointModel != selectedRace.currentCheckpointModel)
            {
                selectedRace.currentCheckpointModel = currentCheckpointModel;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            // Current Material
            EditorGUILayout.PrefixLabel("Active Material");
            currentCheckpointMat = (Material)EditorGUILayout.ObjectField(currentCheckpointMat, typeof(Material), true);
            if (currentCheckpointMat != selectedRace.currentCheckpointMat)
            {
                selectedRace.currentCheckpointMat = currentCheckpointMat;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            // Next Model
            EditorGUILayout.PrefixLabel("Next Active Mesh");
            nextCheckpointModel = (Mesh)EditorGUILayout.ObjectField(nextCheckpointModel, typeof(Mesh), true);
            if (nextCheckpointModel != selectedRace.nextCheckpointModel)
            {
                selectedRace.nextCheckpointModel = nextCheckpointModel;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            // Next Material
            EditorGUILayout.PrefixLabel("Next Active Material");
            nextCheckpointMat = (Material)EditorGUILayout.ObjectField(nextCheckpointMat, typeof(Material), true);
            if (nextCheckpointMat != selectedRace.nextCheckpointMat)
            {
                selectedRace.nextCheckpointMat = nextCheckpointMat;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();


            int i = 0;
            int indexToRemove = 0;
            bool remove = false;

            // Loop through each checkpoint to display in inspector
            foreach (GameObject cpObject in checkpoints)
            {
                EditorGUILayout.BeginHorizontal();

                GameObject newCpObject = cpObject;
                newCpObject = (GameObject)EditorGUILayout.ObjectField(newCpObject, typeof(GameObject), true);

                if (GUILayout.Button("Remove"))
                {
                    indexToRemove = i;
                    remove = true;
                }

                i++;

                EditorGUILayout.EndHorizontal();
            }

            if (remove)
            {
                RemoveCheckpoint(indexToRemove);
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Add New Checkpoint"))
            {
                selectedRace.AddCheckpoint(checkpoints.Count);
                selectedRace.checkpoints[checkpoints.Count - 1].GetComponent<Gate>().playerCol = playerCol;
            }
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUI.indentLevel--;
    }

    private void RemoveCheckpoint(int index)
    {
        DestroyImmediate(selectedRace.checkpoints[index]);
        selectedRace.checkpoints.RemoveAt(index);
        UpdateCheckpointName();
        selectedRace.reorderGateOrder();

        Debug.Log("Removed Checkpoint " + (index));
    }

    private void UpdateCheckpointName()
    {
        int i = 0;
        foreach (GameObject cpObject in selectedRace.checkpoints)
        {
            cpObject.name = "Checkpoint " + i;
            i++;
        }
    }
}
