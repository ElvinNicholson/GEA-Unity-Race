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
    public float initialTime = 0;
    public float timePerPoint = 0;
    private float timeLeft;

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

    // Race Info
    public GameObject raceInfoObject;
    public RaceInfo raceInfo;

    private void Start()
    {
        raceIsRunning = true;

        currentGate = gateOrder[currentGateNum].GetComponent<Gate>();
        lastGate = gateOrder[lastGateNum].GetComponent<Gate>();

        timeLeft = initialTime;
    }

    private void Update()
    {
        if (raceIsRunning)
        {
            UpdateRace();
            UpdateRaceInfo();
        }
    }

    private void UpdateRace()
    {
        if (timer)
        {
            timeLeft -= Mathf.Clamp(Time.deltaTime, 0, float.MaxValue);
        }

        if ((currentGate.isColliding && lastGate.passed) || (currentGate.isColliding && currentGateNum == 0))
        {
            Debug.Log("Passed Gate " + currentGateNum);

            if (currentGateNum != 0 && currentGateNum != gateOrder.Count - 1)
            {
                timeLeft += timePerPoint;
            }

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

    private void UpdateRaceInfo()
    {
        if (laps > 0)
        {
            raceInfo.lapsCurrent = raceInfo.lapsTotal - laps + 1;
        }

        if (timer)
        {
            raceInfo.timeLeft = timeLeft;
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

        InitialiseRaceInfo();
        InitialiseStartLine();
        InitialiseFinishLine();
        InitialiseCheckpoints(numberOfCheckpoint);

        UpdateGateOrder();
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

    public void InitialiseUI()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Default UI");
        GameObject newUI = Instantiate(prefab);
        newUI.name = "Default UI";
        newUI.GetComponent<DefaultRaceUI>().raceInfo = raceInfo;
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

        raceInfo.lapsTotal = laps;
        raceInfo.lapsCurrent = 1;
        raceInfo.timeLeft = timeLeft;
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

    public void UpdateName(string newName)
    {
        gameObject.name = newName;
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
