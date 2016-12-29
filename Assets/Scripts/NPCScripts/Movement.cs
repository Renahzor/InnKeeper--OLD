using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

    public int moveSpeed;

	public void MoveTowardTarget(Transform target)
    {
        float step = Time.deltaTime * moveSpeed;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }
}
