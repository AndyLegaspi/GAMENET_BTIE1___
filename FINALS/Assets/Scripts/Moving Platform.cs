using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform platform;
    public Transform startPos;
    public Transform endPos;

    int direction = 1;
    public float speed;

    private void Update() 
    {
        Vector2 target = CurrentMovementTarget();

        platform.position = Vector2.Lerp(platform.position, target, speed * Time.deltaTime);

        float distance = (target - (Vector2)platform.position).magnitude;

        if(distance <= .1f){
            direction *= -1;
        }
    }

    Vector2 CurrentMovementTarget()
    {
        if(direction == 1){
            return startPos.position;
        }
        else{
            return endPos.position;
        }
    }

    private void OnDrawGizmos() 
    {
        if(platform != null && startPos != null && endPos != null){
            Gizmos.DrawLine(platform.transform.position, startPos.transform.position);
            Gizmos.DrawLine(platform.transform.position, endPos.transform.position);
        }
    }
}
