using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Manager_Combat))]
public class ManagerCombatEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Manager_Combat myScript = (Manager_Combat)target;

        if (GUILayout.Button("Save Encounter"))
        {
            myScript.SaveCombatEncounter();
        }

        if (GUILayout.Button("Load Encounter Prep"))
        {
            myScript.LoadCombatPrep();
        }

        if (GUILayout.Button("Remove Encounter Prep"))
        {
            myScript.RemoveCombatPrep();
        }
    }
}
