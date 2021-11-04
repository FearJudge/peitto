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
    public int invertedControls = 0;
    public float sensitivity = 2;
    public float smoothing = 1.5f;
    Vector2 velocity;
    Vector2 frameVelocity;
    [SerializeField] bool sprintable = false;
    [SerializeField] bool sprinting = false;
    public bool unexhaustable = false;
    bool exhausted = false;
    float moved = 0f;
    Vector3 prevFrame = new Vector3();
    bool landIncoming = false;
    public AudioClip[] terrainsounds;
    public AudioClip[] gasps;
    public AudioSource[] footsteps;
    public AudioSource mouth;
    public int doubleFootsteps = 0;
    int terrainsound = 0;
    float leghurt = 0.2f;
    float startangle = 0;
    public HUDManager hud;
    public static List<int> powersUnlocked = new List<int>();
    public static List<string> powerDescriptions = new List<string>()
    {
        "Double Jump",
        "Infinite Stamina"
    };
    Reason mind;

    // Start is called before the first frame update
    void Start()
    {
        mind = gameObject.GetComponent<Reason>();
        myrb = gameObject.GetComponent<Rigidbody>();
        ResetGravity();
        Cursor.lockState = CursorLockMode.Locked;
        startangle = Quaternion.Angle(new Quaternion(0, 0, 0, 1), transform.rotation);
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
        if (Input.GetKeyDown(KeyCode.Alpha1) && powersUnlocked.Contains(0)) { UsePower(0); }
        if (Input.GetKeyDown(KeyCode.Alpha2) && powersUnlocked.Contains(1)) { UsePower(1); }
        if (Input.GetKeyDown(KeyCode.Escape)) { hud.ShowQuit(true); }
    }

    public void UsePower(int index)
    {
        switch (index)
        {
            case 0:
                mind.StartEffect(Reason.Reasoning.JumpOnce);
                break;
            case 1:
                mind.StartEffect(Reason.Reasoning.InfinteRun);
                break;
            default:
                break;
        }
        hud.ShowQuit(false);
    }

    public static bool GainPower(int index, bool remove=false)
    {
        GameObject.Find("Player").GetComponentInChildren<HUDManager>().Think(string.Format("Press [{0}] to activate {1}", index + 1, powerDescriptions[index]));
        int POWERSINGAME = powerDescriptions.Count;
        if (!powersUnlocked.Contains(index) && index >= 0 && index < POWERSINGAME && !remove) { powersUnlocked.Add(index); return true; }
        else if (powersUnlocked.Contains(index) && remove) { powersUnlocked.Remove(index); return true; }
        return false;
    }

    void CaptureMouse()
    {
        if (hud.quitButton.gameObject.activeSelf == true) { return; }
        // Get smooth velocity.
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
        if (invertedControls > 0) { velocity -= frameVelocity; }
        else { velocity += frameVelocity; }
        velocity.y = Mathf.Clamp(velocity.y, -90, 90);

        // Rotate camera up-down and controller left-right from velocity.
        Camera.main.transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
        transform.rotation = Quaternion.AngleAxis(startangle + velocity.x, Vector3.up);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
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
        if (((Mathf.Abs(speed) > thresholdForDir) || (Mathf.Abs(strafe) > thresholdForDir)) && !airborne)
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
        Debug.Log(Vector3.Distance(prevFrame, transform.position));
        if (Vector3.Distance(prevFrame, transform.position) <= 0.05f && airborne) { repeatedchecks++; }
        else { repeatedchecks = 0; }
        if (repeatedchecks > 300) { airborne = false; }
        if (transform.position.y < -300) { transform.position = Vector3.up * 40; }
        if (myrb.velocity.y < -6f && recoveryframes == 0) { airborne = true; }
        if (recoveryframes > 0) { recoveryframes--; }
        prevFrame = transform.position;
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
            leghurt = 0.8f - (percentage * 0.4f);
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
        if (invertedControls > 0) { invertedControls--; }
    }

    private void GetSoundOf(FoliageSounds soundbyte)
    {
        int minoffset = 0;
        int maxoffset = 0;
        RaycastHit rch;
        Ray ray = new Ray(transform.position, Vector3.down);
        bool hit = Physics.Raycast(ray, out rch, 7);
        if (!hit) { return; }
        switch (rch.transform.tag)
        {
            case "untagged":
                break;
            case "Rock":
                minoffset = 6;
                maxoffset = 6;
                break;
            case "Wood":
                minoffset = 12;
                maxoffset = 12;
                break;
            case "Mud":
                minoffset = 18;
                maxoffset = 17;
                break;
            default:
                break;
        }
            
        switch (soundbyte)
        {
            case FoliageSounds.GrassWalk:
                terrainsound = Random.Range(0 + minoffset, 2 + maxoffset);
                return;
            case FoliageSounds.GrassLand:
                terrainsound = Random.Range(3 + minoffset, 5 + maxoffset);
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
