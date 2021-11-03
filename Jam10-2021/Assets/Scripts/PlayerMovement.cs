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
    int jumps = 0;
    int midairjumps = 0;
    public int Midairjumps { get { return midairjumps; } set { midairjumps = value; jumps = midairjumps; } }
    [SerializeField]int sprintframes = 0;
    int recoveryframes = 0;
    int st = 100;
    public float sensitivity = 2;
    public float smoothing = 1.5f;
    Vector2 velocity;
    Vector2 frameVelocity;
    [SerializeField] bool sprintable = false;
    [SerializeField] bool sprinting = false;
    public bool unexhaustable = false;
    bool exhausted = false;
    bool landIncoming = false;
    public AudioClip[] terrainsounds;
    public AudioClip[] gasps;
    public AudioSource[] footsteps;
    public AudioSource mouth;
    public int doubleFootsteps = 0;
    int terrainsound = 0;
    float leghurt = 0.2f;
    public HUDManager hud;

    // Start is called before the first frame update
    void Start()
    {
        myrb = gameObject.GetComponent<Rigidbody>();
        ResetGravity();
        Cursor.lockState = CursorLockMode.Locked;
    }

    public static void ResetGravity()
    {
        Physics.gravity = new Vector3(0, -20f, 0);
    }

    private void Update()
    {
        Jump();
        HeadBop();
        CaptureMouse();
        OpenFiles();
    }

    void OpenFiles()
    {
        if (Input.GetKeyDown(KeyCode.Q)) { FileManager.OpenFileFolder(); }
        if (Input.GetKeyDown(KeyCode.Escape)) { hud.ShowQuit(true); }
    }

    void CaptureMouse()
    {
        // Get smooth velocity.
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
        velocity += frameVelocity;
        velocity.y = Mathf.Clamp(velocity.y, -90, 90);

        // Rotate camera up-down and controller left-right from velocity.
        Camera.main.transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
        transform.rotation = Quaternion.AngleAxis(velocity.x, Vector3.up);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
        //Rotate();
        CheckVelocity(repeatedchecks);
    }

    void Move(float thresholdForDir = 0.3f)
    {
        sprintframes--;
        float strafe = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(strafe) < 0.3f) { strafe = 0; }
        float speed = Input.GetAxisRaw("Vertical") * (1 - strafe*0.6f);
        if (speed > thresholdForDir) { speed = 1f; }
        else if (speed < -thresholdForDir) { speed = -0.8f; }
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
                if (!unexhaustable) { st -= 3; }
                if (st <= 0) { st = 0; mouth.PlayOneShot(gasps[0]); exhausted = true; }
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
        if (hbopscale < hboplow - 0.05f) { speed *= leghurt; }
        hud.SetStamina(st);
        myrb.AddForce(transform.forward * movementspeed * speed, ForceMode.Acceleration);
        myrb.AddForce(transform.right * strafe * movementspeed, ForceMode.Acceleration);
    }

    void Rotate()
    {
        transform.Rotate(new Vector3(0, Input.GetAxis("Horizontal") * rotationspeed, 0));
    }

    void Jump()
    {
        if (Input.GetButtonDown("Submit") && (!airborne || jumps > 0))
        {
            if (airborne) { jumps--; }
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
        if (myrb.velocity.y < -6f && recoveryframes == 0) { airborne = true; }
        if (recoveryframes > 0) { recoveryframes--; }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Mathf.Abs(collision.contacts[0].normal.y) > 0.7f && landIncoming)
        {
            float percentage = Mathf.Clamp(collision.impulse.magnitude / 20f, 0.1f, 1f);
            Debug.Log(percentage);
            sprinting = false;
            sprintable = false;
            airborne = false;
            sprintframes = 0;
            jumps = midairjumps;
            movespeed = 1f;
            if (percentage < 0.28f) { return; }
            GetSoundOf(FoliageSounds.GrassLand);
            footsteps[0].pitch = 1f - percentage * 0.2f;
            recoveryframes = 30;
            dir = -1;
            hboplow = 0f - 0.7f * percentage;
            hbopscale = hboplow;
        }
    }

    private void HeadBop()
    {
        hbopscale += hbopspeed * Time.deltaTime * dir * Mathf.Clamp(Mathf.Abs(hbopscale - hbophigh * 0.87f), 0.02f, 0.1f) * (movespeed * 500);
        if (hbopscale < hboplow)
        {
            if ((movespeed > 1 || landIncoming) && dir < 0)
            {
                if (!landIncoming) { footsteps[0].pitch = Random.Range(1.0f, 1.4f); }
                landIncoming = false;
                footsteps[0].PlayOneShot(terrainsounds[terrainsound]);
                if (doubleFootsteps > 0) { StartCoroutine(GhostStep(terrainsound)); }            }
            dir = 1;
            hboplow = 0f;
        }
        else if (hbopscale > hbophigh) { dir = -1; hbophigh = 0.13f; GetSoundOf(FoliageSounds.GrassWalk); }
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

    IEnumerator GhostStep(int b)
    {
        yield return new WaitForSeconds(12f + Random.Range(0, 0.3f));
        footsteps[1].pitch = 0.6f;
        footsteps[1].PlayOneShot(terrainsounds[b]);
        doubleFootsteps--;
    }
}
