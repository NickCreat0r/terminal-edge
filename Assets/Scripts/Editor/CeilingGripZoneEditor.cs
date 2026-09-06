#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CeilingGripZone))]
public class CeilingGripZoneEditor : Editor
{
    private void OnSceneGUI()
    {
        // 1. Get reference to the target script on the selected object
        CeilingGripZone grip = (CeilingGripZone)target;
        Transform t = grip.transform;

        Vector3 pos = t.position;
        float currentWidth = grip.Width;
        float currentHeight = grip.Height;

        // Visual color for the interactive drag dots
        Handles.color = Color.cyan;

        EditorGUI.BeginChangeCheck();

        // 2. Position the handles at the left and right edges
        Vector3 rightHandlePos = pos + t.right * (currentWidth * 0.5f);
        Vector3 leftHandlePos = pos - t.right * (currentWidth * 0.5f);

        // 3. Draw slider handles in the Scene View
        Vector3 newRightPos = Handles.Slider(rightHandlePos, t.right, 0.15f, Handles.DotHandleCap, 0.05f);
        Vector3 newLeftPos = Handles.Slider(leftHandlePos, -t.right, 0.15f, Handles.DotHandleCap, 0.05f);

        // 4. If the user dragged either handle, update the object
        if (EditorGUI.EndChangeCheck())
        {
            // Register an undo entry so Ctrl+Z / Cmd+Z works
            Undo.RecordObject(grip, "Resize Ceiling Grip Zone");

            float deltaRight = Vector3.Dot(newRightPos - rightHandlePos, t.right);
            float deltaLeft = Vector3.Dot(newLeftPos - leftHandlePos, -t.right);

            if (Mathf.Abs(deltaRight) > 0.001f)
            {
                // Dragging right handle expands symmetrically or you can offset position
                float newWidth = currentWidth + (deltaRight * 2f);
                grip.SetDimensions(newWidth, currentHeight);
            }
            else if (Mathf.Abs(deltaLeft) > 0.001f)
            {
                // Dragging left handle expands
                float newWidth = currentWidth + (deltaLeft * 2f);
                grip.SetDimensions(newWidth, currentHeight);
            }

            // Mark the scene as dirty so changes save properly
            EditorUtility.SetDirty(grip);
        }
    }
}
#endif