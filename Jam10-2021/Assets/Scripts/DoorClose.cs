using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorClose : MonoBehaviour
{
    public Transform door;
    [SerializeField] private Vector3 open;
    [SerializeField] private Vector3 closed;
    public bool doorClosed = false;
    private bool action = false;
    public bool onceOnly = false;
    private bool available = true;
    [SerializeField] private GameObject hideOnClose;
    private float t;
    public float openAfterClose = 0f;


    // Update is called once per frame
    void FixedUpdate()
    {
        if (!action) { return; }
        AudioScript.PlaySFX(1, 0.7f, 1f);
        t += 0.01f;
        if (!doorClosed) { door.localRotation = Quaternion.Lerp(Quaternion.Euler(open), Quaternion.Euler(closed), t); }
        else { door.localRotation = Quaternion.Lerp(Quaternion.Euler(closed), Quaternion.Euler(open), t); }
        if (t >= 1) { action = false; t = 0; if (!doorClosed) { StartCoroutine(OpenAgain(openAfterClose)); } if (!doorClosed && hideOnClose != null) { hideOnClose.SetActive(false); } else { if (!onceOnly) { available = true; } } }
    }

    IEnumerator OpenAgain(float t)
    {
        if (t == 0) { yield break; }
        yield return new WaitForSeconds(t);
        doorClosed = !doorClosed;
        action = true;
        if (!onceOnly) { available = true; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 7) { return; }
        available = false;
        doorClosed = !doorClosed;
        action = true;
    }
}
