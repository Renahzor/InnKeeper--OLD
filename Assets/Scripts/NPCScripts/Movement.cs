using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

    public int moveSpeed;

	public void MoveTowardTarget(Transform target)
    {
        float step = Time.deltaTime * moveSpeed;
        transform.position = Vector2.MoveTowards(transform.position, target.position, step);
    }

    public void MoveAwayFromTarget(Transform target)
    {
        float step = Time.deltaTime * moveSpeed;
        Vector3 positionToMove = transform.position - target.transform.position;
        positionToMove = positionToMove.normalized;
        positionToMove = -positionToMove;
        transform.position = Vector2.MoveTowards(transform.position, positionToMove, step);
    }
}
