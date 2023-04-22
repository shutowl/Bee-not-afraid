using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerControls : MonoBehaviour
{
    public enum Weapon
    {
        nailgun,
        minigun,
        pesticide,
        pan
    }
    public Weapon weapon;

    public float speed = 10f;
    private Vector2 direction;

    [Header("Weapons")]
    public GameObject rotationPoint;
    public Transform weaponPosition;
    public GameObject[] bullets;
    private bool firing = false;
    public float nailGunFireRate = 0.5f;
    public float miniGunFireRate = 0.02f;
    private float fireRateTimer = 0f;

    [Header("Sanity")]
    public float maxSanity = 100;
    [SerializeField] private float curSanity;
    public float sanityDrainRate = 0.05f;
    public Slider sanityBar;
    public TextMeshProUGUI sanityText;

    [Header("Last Wind")]
    [SerializeField] bool overdrive = false;
    public float overdriveDuration = 10f;

    private Rigidbody2D rb;
    Camera mainCam;
    Vector3 mousePos;

    void Start()
    {
        weapon = Weapon.nailgun;
        rb = GetComponent<Rigidbody2D>();
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        curSanity = maxSanity;
        sanityBar.maxValue = maxSanity;
        sanityBar.value = maxSanity;
    }

    void Update()
    {
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        //Controls
        if (Input.GetKeyDown(KeyCode.Mouse0))   //Fire button held down
        {
            firing = true;
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))   //Fire button let go
        {
            rb.velocity = Vector2.zero;
            firing = false;
        }

        //Weapons
        if (firing)
        {
            if (weapon == Weapon.nailgun)
            {
                if (fireRateTimer < 0)
                {
                    Instantiate(bullets[0], weaponPosition.position, Quaternion.identity);
                    fireRateTimer = nailGunFireRate;
                }

            }
            if(weapon == Weapon.minigun)
            {
                if (fireRateTimer < 0)
                {
                    Instantiate(bullets[1], weaponPosition.position, Quaternion.identity);
                    rb.AddForce(rotationPoint.transform.rotation * Vector3.right * -50);
                    rb.velocity = Vector2.ClampMagnitude(rb.velocity, 7);
                    fireRateTimer = miniGunFireRate;
                }
            }
        }

        //Timers
        if(fireRateTimer >= 0)
        {
            fireRateTimer -= Time.deltaTime;
        }

        //Sanity
        curSanity = Mathf.Clamp(curSanity - sanityDrainRate, 0, maxSanity);
        sanityText.text = curSanity.ToString("F2") + "%";
        sanityBar.value = curSanity;
    }

    void FixedUpdate()
    {
        transform.Translate(speed * Time.deltaTime * direction);
        RotateWeapon();
    }

    void RotateWeapon()
    {
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);         //Get mouse position (using world coordinates)
        Vector3 rotation = mousePos - transform.position;                   //Get direction vector from player to mouse
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;   //Get angle of direction vector (in degrees)
        rotationPoint.transform.rotation = Quaternion.Euler(0, 0, rotZ);    //Rotate position towards mouse using angle
    }

    public void RestoreSanity(float amount)
    {
        curSanity = Mathf.Clamp(curSanity + amount, 0, 100);
    }

    public void TakeSanity(float amount)
    {
        curSanity -= amount;

        if(curSanity <= 0)
        {
            overdrive = true;
        }
    }

    public float GetSanity()
    {
        return curSanity;
    }
}
