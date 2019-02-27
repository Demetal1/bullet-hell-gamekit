using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerCharacter))]
public class PlayerCharacterEditor : Editor
{
    GUIStyle style;
    private void OnEnable()
    {
        style = new GUIStyle ();
        style.richText = true;
    }
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(10f);
        GUILayout.Label("<b>Select Bullet Prefab</b>",style);
        if (GUILayout.Button("Select Prefab"))
        {
            Selection.activeObject = PrefabUtility.GetCorrespondingObjectFromOriginalSource(((PlayerCharacter)target).bulletPool.prefab);
        }
    }

}
