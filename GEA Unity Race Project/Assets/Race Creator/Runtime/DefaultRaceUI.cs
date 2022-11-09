using UnityEngine;
using UnityEngine.UI;

public class DefaultRaceUI : MonoBehaviour
{
    [SerializeField] private RaceInfo raceInfo;
    [SerializeField] private Text timerText;
    [SerializeField] private Text lapText;
    [SerializeField] private GameObject waypoint;
    [SerializeField] private Transform playerIcon;
    [SerializeField] private Transform waypointIcon;
    [SerializeField] private Camera minimapCam;
    [SerializeField] private float minimapSize;

    public Vector3 test;

    public float testX;
    public float testZ;

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

    private void LateUpdate()
    {
        if (raceInfo.raceIsRunning)
        {
            if (raceInfo.time > 0)
            {
                timerText.enabled = true;
                updateTimerText();
            }

            if (raceInfo.lapsTotal > 1)
            {
                lapText.enabled = true;
                updateLapText();
            }

            waypoint.SetActive(true);
            updateWaypoint();
        }
        else
        {
            waypoint.SetActive(false);
        }

        updateMinimapCam();
    }

    private void updateTimerText()
    {
        float minutes = Mathf.FloorToInt(raceInfo.time / 60);
        float seconds = Mathf.FloorToInt(raceInfo.time % 60);
        float milliseconds = raceInfo.time % 1 * 1000;

        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    private void updateLapText()
    {
        lapText.text = string.Format("Lap {0}/{1}", raceInfo.lapsCurrent, raceInfo.lapsTotal);
    }

    private void updateWaypoint()
    {
        float angle = 0;
        Vector2 newPos = Camera.main.WorldToScreenPoint(raceInfo.currentGate.position);
        Vector2 flagPos = new Vector2(0, 10);

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
            flagPos = new Vector2(-4, 8);
            angle = -90;
        }
        else if (waypoint.transform.position.x >= maxX)
        {
            // Touching right edge of screen
            newPos.y = Screen.height / 2;
            flagPos = new Vector2(4, 8);
            angle = 90;
        }

        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);

        waypoint.transform.position = newPos;
        waypoint.transform.localEulerAngles = new Vector3(0, 0, angle);
        waypoint.transform.GetChild(0).transform.localEulerAngles = new Vector3(0, 0, -angle);
        waypoint.transform.GetChild(0).transform.localPosition = flagPos;
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

    private void updateMinimapCam()
    {
        Vector3 camPos = raceInfo.player.position;
        camPos.y = minimapCam.transform.position.y;
        minimapCam.transform.position = camPos;

        Vector3 checkpointPos = raceInfo.currentGate.position;

        Vector3 localPos = minimapCam.transform.InverseTransformPoint(checkpointPos);
        //test = localPos;
        //localPos.z = localPos.y;

        //float boundX = Mathf.Abs(Mathf.Sqrt(Mathf.Pow(minimapSize, 2) - Mathf.Pow(localPos.z, 2)));
        //float boundZ = Mathf.Abs(Mathf.Sqrt(Mathf.Pow(minimapSize, 2) - Mathf.Pow(localPos.x, 2)));

        //testX = boundX;
        //testZ = boundZ;

        //if (float.IsNaN(boundX) && float.IsNaN(boundZ))
        //{
        //    Debug.Log("AAAAA");
        //}

        checkpointPos.x = Mathf.Clamp(checkpointPos.x, camPos.x - minimapSize, camPos.x + minimapSize);
        checkpointPos.y = 20;
        checkpointPos.z = Mathf.Clamp(checkpointPos.z, camPos.z - minimapSize, camPos.z + minimapSize);

        waypointIcon.transform.position = checkpointPos;

        Vector3 playerPos = raceInfo.player.position;
        playerPos.y = 10;

        Vector2 playerAngle = raceInfo.player.eulerAngles;
        playerAngle.x = 90;

        playerIcon.transform.position = playerPos;
        playerIcon.transform.eulerAngles = playerAngle;
    }
}
