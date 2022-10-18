using UnityEngine;
using UnityEngine.UI;

public class DefaultRaceUI : MonoBehaviour
{
    public RaceInfo raceInfo;
    [SerializeField] private Text timerText;
    [SerializeField] private Text lapText;

    private void Update()
    {
        if (raceInfo.raceIsRunning)
        {
            if (raceInfo.timeLeft > 0)
            {
                timerText.enabled = true;
                UpdateTimerText();
            }

            if (raceInfo.lapsTotal > 1)
            {
                lapText.enabled = true;
                UpdateLapText();
            }
        }
    }

    private void UpdateTimerText()
    {
        float minutes = Mathf.FloorToInt(raceInfo.timeLeft / 60);
        float seconds = Mathf.FloorToInt(raceInfo.timeLeft % 60);
        float milliseconds = raceInfo.timeLeft % 1 * 1000;

        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    private void UpdateLapText()
    {
        lapText.text = string.Format("Lap {0}/{1}", raceInfo.lapsCurrent, raceInfo.lapsTotal);
    }
}
