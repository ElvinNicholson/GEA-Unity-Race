using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public Collider playerCol;

    public bool passed = false;
    public bool isColliding = false;

    public int gateNum;

    private void OnTriggerEnter(Collider other)
    {
        if (other == playerCol)
        {
            isColliding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == playerCol)
        {
            isColliding = false;
        }
    }
}
