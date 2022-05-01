using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyAI))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        EnemyAI fov = (EnemyAI)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.chasingRange);

        Handles.color = Color.red;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.wanderDist);

        Vector3 viewAngle1 = DirectionFromAngle(fov.transform.eulerAngles.y, -fov.chaseAngle / 2);
        Vector3 viewAngle2 = DirectionFromAngle(fov.transform.eulerAngles.y, fov.chaseAngle / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle1 * fov.chasingRange);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle2 * fov.chasingRange);
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
