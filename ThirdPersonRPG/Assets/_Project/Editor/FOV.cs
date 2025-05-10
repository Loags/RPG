using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FieldOfView))]
public class FOV : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target;
        if (fov == null) return;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.radius);
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.radiusSmallArea);

        Vector3 viewAngle01 = DirectionFromAngle(fov.transform.eulerAngles.y, -fov.fieldOfViewDegrees / 2);
        Vector3 viewAngle02 = DirectionFromAngle(fov.transform.eulerAngles.y, fov.fieldOfViewDegrees / 2);
        Vector3 viewAngleSmall01 = DirectionFromAngle(fov.transform.eulerAngles.y, -fov.fieldOfViewDegreesSmallArea / 2);
        Vector3 viewAngleSmall02 = DirectionFromAngle(fov.transform.eulerAngles.y, fov.fieldOfViewDegreesSmallArea / 2);

        if (fov == null) return;
        Handles.color = Color.yellow;
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle01 * fov.radius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle02 * fov.radius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleSmall01 * fov.radiusSmallArea);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleSmall02 * fov.radiusSmallArea);

        if (fov.canSeeEnemy)
        {
            if (fov == null) return;
            Handles.color = Color.green;
            Handles.DrawLine(fov.transform.position, fov.targetTransform.position);
        }
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
