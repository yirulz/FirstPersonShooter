using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{

    public Transform lookAtTarget;

    public void Update()
    {
        transform.LookAt(lookAtTarget);
    }
}
