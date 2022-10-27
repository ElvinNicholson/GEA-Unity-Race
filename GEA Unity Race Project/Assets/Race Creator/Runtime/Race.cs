using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Race : MonoBehaviour
{
    // Name
    public string raceName;

    // Player Reference
    public Collider playerCol;

    // Timer
    private bool timeIsOn = false;
    public bool timer = false;
    public bool stopwatch = false;
    public float initialTime = 0;
    public float timePerPoint = 0;

    // Starting Line
    public GameObject startLine;

    // Finish Line
    public GameObject finishLine;

    // Checkpoints
    public GameObject checkpointParent;
    public Mesh checkpointModel;
    public Material checkpointMat;
    public List<GameObject> checkpoints = new List<GameObject>();

    // Gates
    public List<GameObject> gateOrder = new List<GameObject>();

    private Gate currentGate;
    private int currentGateNum = 0;

    private Gate lastGate;
    private int lastGateNum = 0;

    public GameObject raceInfoObject;
    public RaceInfo raceInfo;

    private void Start()
    {
        resetRace();
    }

    private void Update()
    {
        if (raceInfo.raceIsRunning)
        {
            UpdateRace();
        }
    }

    private void UpdateRace()
    {
        if (timer && timeIsOn)
        {
            UpdateTimer();
        }
        else if (stopwatch && timeIsOn)
        {
            UpdateStopwatch();
        }

        // Mark the current gate as passed if the last gate is passed or its the first gate
        if ((currentGate.isColliding && lastGate.passed) || (currentGate.isColliding && currentGateNum == 0))
        {
            Debug.Log("Passed Gate " + currentGateNum);

            timeIsOn = true;

            // Add time when passing checkpoint
            if (currentGateNum != 0 && timer)
            {
                raceInfo.time += timePerPoint;
            }

            // Mark current gate as passed
            currentGate.passed = true;
            currentGateNum++;
            lastGateNum = currentGateNum - 1;

            // Check if the current gate is the finish line
            if (currentGateNum == gateOrder.Count)
            {
                FinishLap();

                if (!raceInfo.raceIsRunning)
                {
                    raceInfo.raceIsWon = true;
                    return;
                }
            }

            currentGate = gateOrder[currentGateNum].GetComponent<Gate>();
            lastGate = gateOrder[lastGateNum].GetComponent<Gate>();
        }
    }

    private void UpdateTimer()
    {
        raceInfo.time -= Time.deltaTime;

        if (raceInfo.time <= 0)
        {
            raceInfo.time = 0;
            raceInfo.raceIsRunning = false;
            raceInfo.raceIsWon = false;

            Debug.Log("Ran out of time!");
        }
    }

    private void UpdateStopwatch()
    {
        raceInfo.time += Time.deltaTime;
    }

    /// <summary>
    /// Loads a new Default UI from prefab, will create a duplicate if Default UI already exists
    /// </summary>
    public void InitialiseUI()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Default UI");
        GameObject newUI = Instantiate(prefab);
        newUI.name = "Default UI";
    }

    private void InitialiseStartLine()
    {
        Mesh startMesh = Resources.Load<Mesh>("Models/StartFinishLine");
        Material startMat = Resources.Load<Material>("Materials/Starting_Line_Material");

        startLine = CreateGate(startMesh, startMat, "Starting Line", gameObject.transform);
    }

    private void InitialiseFinishLine()
    {
        Mesh finishMesh = Resources.Load<Mesh>("Models/StartFinishLine");
        Material finishMat = Resources.Load<Material>("Materials/Finish_Line_Material");

        finishLine = CreateGate(finishMesh, finishMat, "Finish Line", gameObject.transform);
    }

    private void InitialiseCheckpoints(int numberOfCheckpoint)
    {
        checkpointParent = new GameObject();
        checkpointParent.name = "Checkpoints";
        checkpointParent.transform.SetParent(gameObject.transform);

        checkpointModel = Resources.Load<Mesh>("Models/CheckpointRing");
        checkpointMat = Resources.Load<Material>("Materials/Checkpoint_Material");

        for (int i = 0; i < numberOfCheckpoint; i++)
        {
            AddCheckpoint(i);
        }
    }

    private void InitialiseRaceInfo()
    {
        raceInfoObject = new GameObject();
        raceInfoObject.name = "Race Info";
        raceInfoObject.transform.SetParent(gameObject.transform);
        raceInfoObject.AddComponent<RaceInfo>();

        raceInfo = raceInfoObject.GetComponent<RaceInfo>();
    }

    private void FinishLap()
    {
        Debug.Log("Lap Finished");
        raceInfo.lapsCurrent++;

        if (raceInfo.lapsCurrent > raceInfo.lapsTotal)
        {
            Debug.Log("Race Completed!");
            raceInfo.raceIsRunning = false;
        }

        currentGateNum = 0;
        foreach (GameObject gate in gateOrder)
        {
            gate.GetComponent<Gate>().passed = false;
        }
    }

    /// <summary>
    /// Creates a new race object
    /// </summary>
    /// <param name="numberOfCheckpoint">The number of checkpoint in the race, can be changed later</param>
    public void CreateRace(int numberOfCheckpoint)
    {
        raceName = gameObject.name;

        InitialiseRaceInfo();
        InitialiseStartLine();
        InitialiseFinishLine();
        InitialiseCheckpoints(numberOfCheckpoint);

        reorderGateOrder();
    }

    private GameObject CreateGate(Mesh gateMesh, Material gateMaterial, string gateName, Transform parent)
    {
        GameObject gateObject = new GameObject();
        gateObject.name = gateName;
        gateObject.transform.SetParent(parent);

        gateObject.AddComponent<BoxCollider>();
        gateObject.GetComponent<BoxCollider>().isTrigger = true;

        gateObject.AddComponent<MeshFilter>();
        gateObject.GetComponent<MeshFilter>().mesh = gateMesh;

        gateObject.AddComponent<MeshRenderer>();
        gateObject.GetComponent<MeshRenderer>().material = gateMaterial;

        gateObject.AddComponent<Gate>();

        FitCollider(gateObject.GetComponent<Renderer>(), gateObject.GetComponent<BoxCollider>());

        return gateObject;
    }

    /// <summary>
    /// Adds a new checkpoint after the last checkpoint
    /// </summary>
    /// <param name="index">The index of the checkpoint, only used for checkpoint naming</param>
    public void AddCheckpoint(int index)
    {
        string cpName = "Checkpoint " + index;

        GameObject newCheckpoint = CreateGate(checkpointModel, checkpointMat, cpName, checkpointParent.transform);
        checkpoints.Add(newCheckpoint);

        reorderGateOrder();
    }

    /// <summary>
    /// Fit box collider to object mesh
    /// </summary>
    /// <param name="renderer">The renderer of the object</param>
    /// <param name="collider">The Box Collider of the object</param>
    public void FitCollider(Renderer renderer, BoxCollider collider)
    {
        collider.center = renderer.localBounds.center;
        collider.size = renderer.localBounds.size;
    }

    /// <summary>
    /// Changes the name of the race
    /// </summary>
    /// <param name="newName">string of the new name</param>
    public void changeName(string newName)
    {
        gameObject.name = newName;
    }

    /// <summary>
    /// Arranges gateOrder list (Starting Line -> Checkpoints -> Finish Line)
    /// </summary>
    public void reorderGateOrder()
    {
        gateOrder.Clear();

        gateOrder.Add(startLine);
        gateOrder.AddRange(checkpoints);
        gateOrder.Add(finishLine);

        int i = 0;
        foreach (GameObject gate in gateOrder)
        {
            gate.GetComponent<Gate>().gateNum = i;
            i++;
        }
    }

    /// <summary>
    /// Starts the race
    /// </summary>
    public void startRace()
    {
        raceInfo.raceIsRunning = true;
    }

    /// <summary>
    /// Resets the race, call startRace() to start the race
    /// </summary>
    public void resetRace()
    {
        raceInfo.lapsCurrent = 1;
        timeIsOn = false;

        if (timer)
        {
            raceInfo.time = initialTime;
        }
        else if (stopwatch)
        {
            raceInfo.time = 0;
        }

        raceInfo.raceIsWon = false;

        currentGateNum = 0;
        currentGate = gateOrder[currentGateNum].GetComponent<Gate>();

        lastGateNum = 0;
        lastGate = gateOrder[lastGateNum].GetComponent<Gate>();
    }
}
