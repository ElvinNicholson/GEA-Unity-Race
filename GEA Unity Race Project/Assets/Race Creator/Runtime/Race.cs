using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Race : MonoBehaviour
{
    bool raceIsRunning;

    // Name
    public string raceName;

    // Player Reference
    public Collider playerCol;

    // Timer
    public bool timer = false;
    public int initialTime = 0;
    public int timePerPoint = 0;

    // Laps
    public int laps = 0;

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

    Gate currentGate;
    int currentGateNum = 0;

    Gate lastGate;
    int lastGateNum = 0;

    private void Start()
    {
        raceIsRunning = true;

        currentGate = gateOrder[currentGateNum].GetComponent<Gate>();
        lastGate = gateOrder[lastGateNum].GetComponent<Gate>();
    }

    private void Update()
    {
        if (raceIsRunning)
        {
            UpdateRace();
        }
    }

    private void UpdateRace()
    {
        if ((currentGate.isColliding && lastGate.passed) || (currentGate.isColliding && currentGateNum == 0))
        {
            Debug.Log("Passed Gate " + currentGateNum);
            currentGate.passed = true;
            currentGateNum++;
            lastGateNum = currentGateNum - 1;

            if (currentGateNum == gateOrder.Count)
            {
                FinishLap();

                if (!raceIsRunning)
                {
                    return;
                }
            }

            currentGate = gateOrder[currentGateNum].GetComponent<Gate>();
            lastGate = gateOrder[lastGateNum].GetComponent<Gate>();
        }
    }

    private void FinishLap()
    {
        Debug.Log("Lap Finished");
        if (laps != 0)
        {
            laps--;

            if (laps == 0)
            {
                Debug.Log("Race Completed!");
                raceIsRunning = false;
            }

            currentGateNum = 0;

            foreach (GameObject gate in gateOrder)
            {
                gate.GetComponent<Gate>().passed = false;
            }
        }
        else
        {
            Debug.Log("Race Completed!");
            raceIsRunning = false;
        }
    }

    public void CreateRace(int numberOfCheckpoint)
    {
        raceName = gameObject.name;

        InitialiseStartLine();
        InitialiseFinishLine();
        InitialiseCheckpoints(numberOfCheckpoint);

        UpdateGateOrder();
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

    public void UpdateName(string newName)
    {
        gameObject.name = newName;
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

    public void AddCheckpoint(int index)
    {
        string cpName = "Checkpoint " + index;

        GameObject newCheckpoint = CreateGate(checkpointModel, checkpointMat, cpName, checkpointParent.transform);
        checkpoints.Add(newCheckpoint);

        UpdateGateOrder();
    }

    public void FitCollider(Renderer renderer, BoxCollider collider)
    {
        collider.center = renderer.localBounds.center;
        collider.size = renderer.localBounds.size;
    }

    public void UpdateGateOrder()
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
}
