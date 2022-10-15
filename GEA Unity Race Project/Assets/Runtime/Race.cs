using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Race : MonoBehaviour
{
    // Name
    public string raceName;

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
    public List<GameObject> checkpoints = new List<GameObject>();

    public void createRace(int numberOfCheckpoint)
    {
        raceName = gameObject.name;

        startLine = new GameObject();
        startLine.name = "Starting Line";
        startLine.transform.SetParent(gameObject.transform);

        finishLine = new GameObject();
        finishLine.name = " Finish Line";
        finishLine.transform.SetParent(gameObject.transform);

        initialiseCheckpoints(numberOfCheckpoint);
    }

    private void initialiseCheckpoints(int numberOfCheckpoint)
    {
        GameObject checkpointParent = new GameObject();
        checkpointParent.name = "Checkpoints";
        checkpointParent.transform.SetParent(gameObject.transform);

        for (int i = 0; i < numberOfCheckpoint; i++)
        {
            // Create new checkpoint GameObject and add to list
            GameObject newCheckpoint = new GameObject();
            newCheckpoint.name = "Checkpoint " + (i + 1);
            newCheckpoint.transform.SetParent(checkpointParent.transform);
            newCheckpoint.AddComponent<Checkpoint>();
            checkpoints.Add(newCheckpoint);
        }
    }

    public void updateName(string newName)
    {
        gameObject.name = newName;
    }
}
