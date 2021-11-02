using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathingObject : MonoBehaviour
{
    float breathScale = 1f;
    [SerializeField] float breathScaleMin = 0.9f;
    [SerializeField] float breathScaleMax = 1.1f;
    [SerializeField] float breathScaleSpeed = 0.01f;
    int dir = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Breathe();
    }

    void Breathe()
    {
        breathScale += breathScaleSpeed * dir * Mathf.Clamp((breathScale - breathScaleMin), 0.02f, 1);
        if (breathScale < breathScaleMin) { dir = 1; }
        else if (breathScale > breathScaleMax) { dir = -1; }
        transform.localScale = Vector3.one * breathScale;
    }
}
