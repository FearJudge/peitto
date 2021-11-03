using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    enum FoliageSounds
    {
        GrassWalk,
        GrassLand
    }
    public enum GravityType
    {
        Basic,
        Floaty,
        Inverted,
        PosX,
        PosZ,
        NegX,
        NegZ
    }
    public float movementspeed = 30f;
    public float rotationspeed = 4f;
    Rigidbody myrb;
    public GameObject headCamera;
    public Vector3 offsetCam;
    bool airborne = true;
    float movespeed = 1;
    int repeatedchecks = 0;
    [SerializeField] float hbopscale = 0f;
    float hbopspeed = 0.003f;
    float hboplow = 0f;
    float hbophigh = 0.13f;
    int dir = 1;
    [SerializeField]int sprintframes = 0;
    int st = 100;
    [SerializeField] bool sprintable = false;
    [SerializeField] bool sprinting = false;
    bool exhausted = false;
    bool landIncoming = false;
    public AudioClip[] terrainsounds;
    public AudioClip[] gasps;
    public AudioSource footsteps;
    public AudioSource mouth;
    int terrainsound = 0;
    public HUDManager hud;

    // Start is called before the first frame update
    void Start()
    {
        myrb = gameObject.GetComponent<Rigidbody>();
        ResetGravity();
    }

    public static void ResetGravity()
    {
        Physics.gravity = new Vector3(0, -20f, 0);
    }

    private void Update()
    {
        Jump();
        HeadBop();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveForward();
        Rotate();
        CheckVelocity(repeatedchecks);
    }

    void MoveForward(float thresholdForDir = 0.3f)
    {
        sprintframes--;
        float speed = Input.GetAxisRaw("Vertical");
        if (speed > thresholdForDir) { speed = 1f; }
        else if (speed < -thresholdForDir) { speed = -0.5f; }
        else { speed = 0f; }
        if (exhausted && st > 200) { exhausted = false; }
        // Ran out of opportunity to sprint:
        if (sprintframes < 0 ) { sprintframes = 0; sprintable = false; sprinting = false; }
        // Letting go of up allows for Sprint
        if (Mathf.Abs(speed) < thresholdForDir && sprintframes > 0) { sprintable = true; }
        //  Check to see if doubletap has happened!
        else if (speed > thresholdForDir && sprintframes > 0 && sprintable) { sprinting = true; sprintframes = 20; }
        // Holding up starts sprint countdown
        else if (speed > thresholdForDir) { sprintframes = 30; }
        if (Mathf.Abs(speed) > thresholdForDir && !airborne)
        {
            if (sprinting && st > 0 && !exhausted)
            {
                st -= 3; if (st <= 0) { st = 0; mouth.PlayOneShot(gasps[0]); exhausted = true; }
                movespeed = 15;
                speed *= 1.7f;
            }
            else
            {
                st++;
                movespeed = 7;
            }
        }
        else if (!airborne && !exhausted) { movespeed = 1f; st += 5; if (st > 1000) { st = 1000; } }
        else if (airborne) { landIncoming = true; movespeed = 0.01f; }
        else { st += 1; if (st > 1000) { st = 1000; } }
        if (hbopscale < hboplow - 0.05f) { speed *= 0.2f; }
        hud.SetStamina(st);
        myrb.AddForce(transform.forward * movementspeed * speed, ForceMode.Acceleration);
    }

    void Rotate()
    {
        transform.Rotate(new Vector3(0, Input.GetAxis("Horizontal") * rotationspeed, 0));
    }

    void Jump()
    {
        if (Input.GetButtonDown("Submit") && !airborne)
        {
            airborne = true;
            movespeed = 0;
            myrb.AddForce(new Vector3(0, 12f, 0), ForceMode.Impulse);
            repeatedchecks = 0;
        }
    }

    private void CheckVelocity(int check)
    {
        if (myrb.velocity.y == 0 && airborne) { repeatedchecks++; }
        else { repeatedchecks = 0; }
        if (repeatedchecks > 3) { airborne = false; }
        if (transform.position.y < -300) { transform.position = Vector3.zero; }
        if (myrb.velocity.y < -2f) { airborne = true; }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Mathf.Abs(collision.contacts[0].normal.y) > 0.7f && landIncoming)
        {
            GetSoundOf(FoliageSounds.GrassLand);
            footsteps.pitch = 0.9f;
            sprinting = false;
            sprintable = false;
            airborne = false;
            sprintframes = 0;
            dir = -100;
            hboplow = -0.2f;
            movespeed = 1f;
        }
    }

    private void HeadBop()
    {
        hbopscale += hbopspeed * Time.deltaTime * dir * Mathf.Clamp(Mathf.Abs(hbopscale - hbophigh * 0.87f), 0.02f, 0.1f) * (movespeed * 500);
        if (hbopscale < hboplow) { if ((movespeed > 1 || landIncoming) && dir < 0) { landIncoming = false; footsteps.PlayOneShot(terrainsounds[terrainsound]); } dir = 1; hboplow = 0f; }
        else if (hbopscale > hbophigh) { dir = -1; hbophigh = 0.13f; footsteps.pitch = Random.Range(1.0f, 1.4f); GetSoundOf(FoliageSounds.GrassWalk); }
        headCamera.transform.localPosition = Vector3.up * hbopscale + offsetCam;
    }

    private void GetSoundOf(FoliageSounds soundbyte)
    {
        switch (soundbyte)
        {
            case FoliageSounds.GrassWalk:
                terrainsound = Random.Range(0, 2);
                return;
            case FoliageSounds.GrassLand:
                terrainsound = Random.Range(3, 5);
                return;
            default:
                terrainsound = 0;
                return;
        }
    }
}
