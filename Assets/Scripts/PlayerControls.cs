using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerControls : MonoBehaviour
{
    public enum Weapon
    {
        nailgun,
        minigun,
        shotgun,
        pesticide,
        pan
    }
    public Weapon weapon;

    public float speed = 10f;
    private Vector2 direction;

    [Header("Weapons")]
    public GameObject rotationPoint;
    public GameObject weaponObject;
    private SpriteRenderer weaponSprite;
    public GameObject[] bullets;
    private bool firing = false;
    public float nailGunFireRate = 0.5f;
    public float miniGunFireRate = 0.02f;
    public float shotgunFireRate = 0.5f;
    public float pesticideFireRate = 1f;
    private float fireRateTimer = 0f;
    public Image weaponImage;
    public TextMeshProUGUI weaponText;

    [Header("Sanity")]
    public float maxSanity = 100;
    [SerializeField] private float curSanity;
    public float sanityDrainRate = 0.05f;
    public float drainRateIncreaseRate = 20f;   //every [] seconds, drain rate increases a certain amount
    private float drainRateIncreaseTimer = 0f;
    public float drainRateIncreaseAmount = 0.001f;
    public Slider sanityBar;
    public TextMeshProUGUI sanityText;
    public Animator eye;

    [Header("Last Wind")]
    [SerializeField] bool lastWind = false;
    [SerializeField] bool overdrive = false;
    public TextMeshProUGUI lastWindTimeText;
    public TextMeshProUGUI lastWindEnemyText;
    public float overdriveDuration = 10f;
    float overdriveTimer = 0f;
    int sanityNeeded = 5;

    [Header("Game Over")]
    float timeElapsed = 0f;
    public GameObject GameOverUI;
    public TextMeshProUGUI TimeText;
    bool gameOver = false;

    private Rigidbody2D rb;
    Camera mainCam;
    Vector3 mousePos;

    void Start()
    {
        weapon = Weapon.nailgun;
        rb = GetComponent<Rigidbody2D>();
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        weaponSprite = weaponObject.GetComponent<SpriteRenderer>();

        curSanity = maxSanity;
        sanityBar.maxValue = maxSanity;
        sanityBar.value = maxSanity;
        drainRateIncreaseTimer = drainRateIncreaseRate;

        lastWindTimeText.text = "";
        lastWindEnemyText.text = "";
        GameOverUI.SetActive(false);
    }

    void Update()
    {
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (mousePos.x > transform.position.x) weaponSprite.flipY = false;
        else weaponSprite.flipY = true;

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
        if (Input.GetKeyDown(KeyCode.Space) && gameOver)
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(0);
        }

        //Weapons
        if (firing)
        {
            if (weapon == Weapon.nailgun)
            {
                if (fireRateTimer < 0)
                {
                    Instantiate(bullets[0], weaponObject.transform.position, Quaternion.identity);
                    DropBulletCasing(bullets[0]);
                    fireRateTimer = nailGunFireRate;
                }

            }
            if(weapon == Weapon.minigun)
            {
                if (fireRateTimer < 0)
                {
                    Instantiate(bullets[1], weaponObject.transform.position, Quaternion.identity);
                    DropBulletCasing(bullets[1]);
                    rb.AddForce(rotationPoint.transform.rotation * Vector3.right * -25f);
                    rb.velocity = Vector2.ClampMagnitude(rb.velocity, 3);
                    fireRateTimer = miniGunFireRate;
                }
            }
            if (weapon == Weapon.shotgun)
            {
                if (fireRateTimer < 0)
                {
                    for(int i = 0; i < 15; i++)
                    {
                        Instantiate(bullets[2], weaponObject.transform.position, Quaternion.identity);
                        DropBulletCasing(bullets[2]);
                    }
                    rb.AddForce(rotationPoint.transform.rotation * Vector3.right * -300f);
                    fireRateTimer = shotgunFireRate;
                }
            }
            if (weapon == Weapon.pesticide)
            {
                if (fireRateTimer < 0)
                {
                    Instantiate(bullets[3], weaponObject.transform.position, Quaternion.identity);
                    fireRateTimer = pesticideFireRate;
                }
            }
        }

        //Timers
        if(fireRateTimer >= 0)
        {
            fireRateTimer -= Time.deltaTime;
        }
        if(drainRateIncreaseTimer >= 0)
        {
            drainRateIncreaseTimer -= Time.deltaTime;
        }
        else
        {
            sanityDrainRate += drainRateIncreaseAmount;
            drainRateIncreaseTimer = drainRateIncreaseRate;
        }
        if (overdrive)
        {
            overdriveTimer -= Time.deltaTime;
            lastWindTimeText.text = overdriveTimer.ToString("F2");

            if(overdriveTimer <= 0)
            {
                GameOver();
            }
        }
        timeElapsed += Time.deltaTime;

        //Sanity
        curSanity = Mathf.Clamp(curSanity - sanityDrainRate, 0, maxSanity);
        sanityText.text = curSanity.ToString("F2") + "%";
        sanityBar.value = curSanity;
        eye.SetFloat("sanity", curSanity);

        if (curSanity <= 0)
        {
            if (!lastWind)
            {
                EnableOverdrive();
            }
            else if(!overdrive)
            {
                GameOver();
            }
        }
    }

    void FixedUpdate()
    {
        transform.Translate(speed * Time.deltaTime * direction);
        RotateWeapon();
    }

    void GameOver()
    {
        //Game Over
        Debug.Log("Game Over: Lasted " + timeElapsed.ToString("F2") + " seconds");
        GameOverUI.SetActive(true);
        TimeText.text = "You lasted " + timeElapsed.ToString("F2") + " seconds!";
        gameOver = true;
        Time.timeScale = 0;
    }

    void RotateWeapon()
    {
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);         //Get mouse position (using world coordinates)
        Vector3 rotation = mousePos - transform.position;                   //Get direction vector from player to mouse
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;   //Get angle of direction vector (in degrees)
        rotationPoint.transform.rotation = Quaternion.Euler(0, 0, rotZ);    //Rotate position towards mouse using angle
    }

    void DropBulletCasing(GameObject bullet)
    {
        GameObject casing = Instantiate(bullet, weaponObject.transform.position, Quaternion.identity);
        casing.GetComponent<PlayerBullet>().casing = true;
        casing.GetComponent<PlayerBullet>().lifeTime = 0.75f;
        casing.GetComponent<BoxCollider2D>().enabled = false;
        casing.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        casing.GetComponent<Rigidbody2D>().gravityScale = 2;
        casing.GetComponent<Transform>().localScale /= 2;
        int xDir = (weaponSprite.flipY) ? 1 : -1;
        casing.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(60f, 100f) * xDir, Random.Range(150f, 250f)));
    }

    public void RestoreSanity(float amount)
    {
        if (!overdrive)
        {
            curSanity = Mathf.Clamp(curSanity + amount, 0, 100);
        }
        else
        {
            sanityNeeded--;
            lastWindEnemyText.text = "Collect sanity " + sanityNeeded + " more time(s) to stay sane!";

            if(sanityNeeded == 0)
            {
                DisableOverdrive();
            }
        }
    }

    public void TakeSanity(float amount)
    {
        if (!overdrive)
        {
            curSanity -= amount;
        }
    }

    void EnableOverdrive()
    {
        lastWind = true;
        overdrive = true;
        sanityNeeded = 10;
        overdriveTimer = overdriveDuration;
        lastWindTimeText.text = overdriveDuration.ToString("F2");
        lastWindEnemyText.text = "Collect sanity 10 more time(s) to stay sane!";

        nailGunFireRate /= 3;
        miniGunFireRate /= 3;
        shotgunFireRate /= 3;
        pesticideFireRate /= 3;
    }

    void DisableOverdrive()
    {
        overdrive = false;
        nailGunFireRate *= 3;
        miniGunFireRate *= 3;
        shotgunFireRate *= 3;
        pesticideFireRate *= 3;

        lastWindTimeText.text = "";
        lastWindEnemyText.text = "";
        RestoreSanity(70);
    }

    public void SetWeaponSprite(Sprite sprite)
    {
        weaponSprite.sprite = sprite;
        weaponImage.sprite = sprite;
    }

    public void SetWeaponText(string text)
    {
        weaponText.text = text;
    }

    public float GetSanity()
    {
        return curSanity;
    }
}
