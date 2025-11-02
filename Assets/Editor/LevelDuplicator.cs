using UnityEditor;
using UnityEngine;

// Editor utility to duplicate an existing level GameObject (level2) into level3 inside the open scene.
// Usage: open the scene `Assets/Scene/Game.unity` in Unity Editor, then in the menu Tools -> Duplicate Level 2 -> To Level 3
// This avoids dangerous manual YAML edits and preserves correct fileIDs and references.
public class LevelDuplicator : EditorWindow
{
    [MenuItem("Tools/Duplicate Level 2/To Level 3")]
    public static void DuplicateLevel2ToLevel3()
    {
        var level2 = GameObject.Find("level2");
        if (level2 == null)
        {
            EditorUtility.DisplayDialog("Duplicate Level", "Could not find a GameObject named 'level2' in the open scene.", "OK");
            return;
        }

        // Check if level3 exists already
        var existing = GameObject.Find("level3");
        if (existing != null)
        {
            if (!EditorUtility.DisplayDialog("Duplicate Level", "A GameObject named 'level3' already exists. Overwrite?", "Yes", "No"))
                return;
            DestroyImmediate(existing);
        }

        // Duplicate
        var newLevel = Instantiate(level2);
        newLevel.name = "level3";
        // Keep it inactive by default (match previous placeholder)
        newLevel.SetActive(false);

        // Register undo
        Undo.RegisterCreatedObjectUndo(newLevel, "Duplicate level2 to level3");

        // Try to update GameLevelLoader.levelRoots array if present
        var loader = Object.FindObjectOfType<MonoBehaviour>();
        // Find GameLevelLoader by type name to avoid compile-time dependency
        foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (go.name == "GameLevelLoader")
            {
                var comp = go.GetComponent("GameLevelLoader");
                if (comp != null)
                {
                    var so = new SerializedObject(comp);
                    var prop = so.FindProperty("levelRoots");
                    if (prop != null && prop.isArray)
                    {
                        int oldSize = prop.arraySize;
                        prop.arraySize = oldSize + 1;
                        var elem = prop.GetArrayElementAtIndex(oldSize);
                        elem.objectReferenceValue = newLevel;
                        so.ApplyModifiedProperties();
                        Debug.Log("GameLevelLoader.levelRoots updated to include level3.");
                    }
                }
            }
        }

        // Update GameManager serialized totalLevels if present
        foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (go.name == "GameManager")
            {
                var gm = go.GetComponent("GameManager");
                if (gm != null)
                {
                    var so = new SerializedObject(gm);
                    var prop = so.FindProperty("totalLevels");
                    if (prop != null && prop.propertyType == SerializedPropertyType.Integer)
                    {
                        prop.intValue = Mathf.Max(prop.intValue, 4);
                        so.ApplyModifiedProperties();
                        Debug.Log("GameManager.totalLevels set to " + prop.intValue);
                    }
                }
            }
        }

        EditorUtility.DisplayDialog("Duplicate Level", "Duplicated 'level2' -> 'level3'. Open the Hierarchy in the Editor to review and then save the scene.", "OK");
    }
}
