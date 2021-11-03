using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMovingScript : MonoBehaviour
{
    public float speed = 1f;
    public float timer;
    public float timeToChange = 2f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        if (timer >= timeToChange)
        {
            speed = -speed;
            timer = 0f;
        }
    }
}