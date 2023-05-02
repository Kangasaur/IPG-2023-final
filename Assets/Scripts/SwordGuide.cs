using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordGuide : MonoBehaviour
{
    RaycastHit hit;
    [SerializeField] Transform parent;

    void FixedUpdate()
    {
        bool a = Physics.Raycast(parent.TransformPoint(Vector3.zero), transform.TransformDirection(Vector3.forward), out hit);
        Debug.DrawRay(parent.TransformPoint(Vector3.zero), transform.TransformDirection(Vector3.forward) * 40, Color.red);
        if (a)
        {
            transform.position = hit.point;
        }
    }
}
