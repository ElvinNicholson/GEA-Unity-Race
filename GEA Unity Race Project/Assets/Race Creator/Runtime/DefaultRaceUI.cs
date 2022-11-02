using UnityEngine;
using UnityEngine.UI;

public class DefaultRaceUI : MonoBehaviour
{
    [SerializeField] private RaceInfo raceInfo;
    [SerializeField] private Text timerText;
    [SerializeField] private Text lapText;
    [SerializeField] private Image waypoint;

    private float minX;
    private float minY;
    private float maxX;
    private float maxY;

    private void Start()
    {
        findRaceInfo();
        minX = 100;
        minY = 100;

        maxX = Screen.width - minX;
        maxY = Screen.height - minY;
    }

    private void Update()
    {
        if (raceInfo.raceIsRunning)
        {
            if (raceInfo.time > 0)
            {
                timerText.enabled = true;
                UpdateTimerText();
            }

            if (raceInfo.lapsTotal > 1)
            {
                lapText.enabled = true;
                UpdateLapText();
            }

            waypoint.enabled = true;
            updateWaypoint();
        }
        else
        {
            waypoint.enabled = false;
        }
    }

    private void UpdateTimerText()
    {
        float minutes = Mathf.FloorToInt(raceInfo.time / 60);
        float seconds = Mathf.FloorToInt(raceInfo.time % 60);
        float milliseconds = raceInfo.time % 1 * 1000;

        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    private void UpdateLapText()
    {
        lapText.text = string.Format("Lap {0}/{1}", raceInfo.lapsCurrent, raceInfo.lapsTotal);
    }

    private void updateWaypoint()
    {
        float angle = 0;
        Vector2 newPos = Camera.main.WorldToScreenPoint(raceInfo.currentGate.position);

        if (Vector3.Dot((raceInfo.currentGate.position - Camera.main.transform.position), Camera.main.transform.forward) < 0)
        {
            // Behind camera
            if (newPos.x < Screen.width/2)
            {
                // Right
                newPos.x = maxX;
            }
            else
            {
                // Left
                newPos.x = minX;
            }
        }


        if (waypoint.transform.position.x <= minX)
        {
            // Touching left edge of screen
            newPos.y = Screen.height / 2;
            angle = -90;
        }
        else if (waypoint.transform.position.x >= maxX)
        {
            // Touching right edge of screen
            newPos.y = Screen.height / 2;
            angle = 90;
        }

        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);

        waypoint.transform.position = newPos;
        waypoint.transform.localEulerAngles = new Vector3(0, 0, angle);
    }

    private void findRaceInfo()
    {
        raceInfo = FindObjectOfType<RaceInfo>();

        if (raceInfo)
        {
            Debug.Log("RaceInfo found");
        }
        else
        {
            Debug.Log("RaceInfo not found");
        }
    }
}
