using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeManip : MonoBehaviour
{
    public GameObject root;
    public GameObject fakesRoot;
    public GameObject trappedObj;
    public bool state;
    public bool TrapPositiveX = true;
    public bool TrapNegativeX = true;
    public bool TrapPositiveZ = true;
    public bool TrapNegativeZ = true;
    public Vector3 dimensionsOfRoot;

    // Start is called before the first frame update
    void Start()
    {
        if (root.GetComponentInChildren<FakeManip>() != null)
        {
            throw new ArgumentException("Root cannot contain FakeManipScript!!!");
        }
        for (int a = 0; a < 5; a++)
        {
            for (int b = 0; b < 5; b++)
            {
                if (a == 2 && b == 2) { continue; }
                GameObject fake = Instantiate(root, fakesRoot.transform);
                fake.transform.position = new Vector3(dimensionsOfRoot.x * (a - 2), 0, dimensionsOfRoot.z* (b - 2));
            }
        }
    }

    private void Update()
    {
        Vector3 exitpoint = trappedObj.transform.position - root.transform.position;
        if (trappedObj.transform.position.x - root.transform.position.x >= dimensionsOfRoot.x / 2 && TrapPositiveX)
        {
            exitpoint.x -= dimensionsOfRoot.x;
        }
        else if (trappedObj.transform.position.x - root.transform.position.x <= -dimensionsOfRoot.x / 2 && TrapNegativeX)
        {
            exitpoint.x += dimensionsOfRoot.x;
        }
        if (trappedObj.transform.position.z - root.transform.position.z >= dimensionsOfRoot.z / 2 && TrapPositiveZ)
        {
            exitpoint.z -= dimensionsOfRoot.z;
        }
        else if (trappedObj.transform.position.z - root.transform.position.z <= -dimensionsOfRoot.z / 2 && TrapNegativeZ)
        {
            exitpoint.z += dimensionsOfRoot.z;
        }
        trappedObj.transform.position = exitpoint;
    }
}
