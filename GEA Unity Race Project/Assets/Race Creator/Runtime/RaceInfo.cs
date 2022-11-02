using UnityEngine;

public class RaceInfo : MonoBehaviour
{
    // Race
    public bool raceIsRunning;
    public bool raceIsWon;

    // Laps
    public int lapsTotal;
    public int lapsCurrent;

    // Timer
    public float time;

    public Transform currentGate;
}
