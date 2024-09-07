using UnityEditor;
using UnityEngine;

public class AutoDuplicator : EditorWindow
{
    public GameObject objectToDuplicate;
    public int numberOfDuplicates = 1000;
    public Vector3 positionOffset = new Vector3(1, 0, 0); // Incremental offset per duplication
    public bool useRandomSpread = false;
    public Vector3 randomSpreadRange = new Vector3(5, 0, 5); // Range for random spread

    [MenuItem("Tools/Auto Duplicator")]
    public static void ShowWindow()
    {
        GetWindow<AutoDuplicator>("Auto Duplicator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Auto Duplicator", EditorStyles.boldLabel);
        objectToDuplicate = (GameObject)EditorGUILayout.ObjectField("Object to Duplicate", objectToDuplicate, typeof(GameObject), true);
        numberOfDuplicates = EditorGUILayout.IntField("Number of Duplicates", numberOfDuplicates);
        positionOffset = EditorGUILayout.Vector3Field("Incremental Position Offset", positionOffset);
        useRandomSpread = EditorGUILayout.Toggle("Use Random Spread", useRandomSpread);

        if (useRandomSpread)
        {
            randomSpreadRange = EditorGUILayout.Vector3Field("Random Spread Range", randomSpreadRange);
        }

        if (GUILayout.Button("Duplicate"))
        {
            DuplicateObjects();
        }
    }

    private void DuplicateObjects()
    {
        if (objectToDuplicate == null)
        {
            Debug.LogError("Please assign an object to duplicate.");
            return;
        }

        string baseName = objectToDuplicate.name;

        // Remove any existing "(Clone)" suffix from the base name
        if (baseName.EndsWith("(Clone)"))
        {
            baseName = baseName.Replace("(Clone)", "").Trim();
        }

        Vector3 currentPosition = objectToDuplicate.transform.position;

        for (int i = 0; i < numberOfDuplicates; i++)
        {
            GameObject newObject = Instantiate(objectToDuplicate);
            Undo.RegisterCreatedObjectUndo(newObject, "Duplicate Object");

            // Apply position offset or random spread
            if (useRandomSpread)
            {
                Vector3 randomOffset = new Vector3(
                    Random.Range(-randomSpreadRange.x, randomSpreadRange.x),
                    Random.Range(-randomSpreadRange.y, randomSpreadRange.y),
                    Random.Range(-randomSpreadRange.z, randomSpreadRange.z)
                );
                newObject.transform.position = currentPosition + randomOffset;
            }
            else
            {
                currentPosition += positionOffset;
                newObject.transform.position = currentPosition;
            }

            // Rename the new object
            newObject.name = $"{baseName}{i + 1}";

            Selection.activeGameObject = newObject;
        }

        Debug.Log($"{numberOfDuplicates} objects created with names {baseName}1, {baseName}2, ..., {baseName}{numberOfDuplicates}.");
    }
}