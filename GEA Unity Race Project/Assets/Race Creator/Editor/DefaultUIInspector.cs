using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DefaultRaceUI))]
public class DefaultUIInspector : Editor
{
    DefaultRaceUI raceUI;
    RaceInfo raceInfo;

    private void OnEnable()
    {
        raceUI = (DefaultRaceUI)target;
        raceInfo = raceUI.raceInfo;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox(" Assign Race Info of active race here", MessageType.Info);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.PrefixLabel("Active Race Info");
        raceInfo = (RaceInfo)EditorGUILayout.ObjectField(raceInfo, typeof(RaceInfo), true);
        raceUI.raceInfo = raceInfo;

        EditorGUILayout.EndHorizontal();

        //base.OnInspectorGUI();
    }
}
