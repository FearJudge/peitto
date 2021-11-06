using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMovingScript2 : MonoBehaviour
{
    public float speed = 2f;
    public float timer;
    public float timeToChange = 1f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        if (timer >= timeToChange)
        {
            speed = -speed;
            timer = 0f;
        }
    }
}
