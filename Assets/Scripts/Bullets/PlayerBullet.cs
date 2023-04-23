using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public enum BulletType
    {
        nail,
        minigun,
        shotgun,
        acid,
    }
    public BulletType type;
    public float lifeTime = 1f;
    private float lifeTimeTimer = 0f;
    public float speed = 5f;
    public int damage = 10;
    public float bulletSpread = 4f;
    public bool casing = false;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    public GameObject acidCloud;

    public AudioClip clip;

    public float x = 0, y = -1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Camera mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);         //Get mouse position (using world coordinates)
        Vector3 rotation = mousePos - transform.position;                   //Get direction vector from player to mouse
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;   //Get angle of direction vector (in degrees)
        transform.rotation = Quaternion.Euler(0, 0, rotZ - 90);                  //Rotate position towards mouse using angle

        if (!casing)
        {
            if (type == BulletType.nail || type == BulletType.acid)
            {
                rb.velocity = new Vector2(rotation.x, rotation.y).normalized * speed;
            }
            if (type == BulletType.minigun || type == BulletType.shotgun)
            {
                rb.velocity = new Vector2(rotation.x, rotation.y).normalized * speed + new Vector2(Random.Range(-bulletSpread, bulletSpread), Random.Range(-bulletSpread, bulletSpread));
            }
        }
        else
        {
            sprite = GetComponent<SpriteRenderer>();
        }

        lifeTimeTimer = lifeTime;
    }

    private void Update()
    {
        lifeTimeTimer -= Time.deltaTime;
        if (lifeTimeTimer < 0)
        {
            if (type == BulletType.acid)
            {
                FindObjectOfType<PlayerControls>().PlaySound(clip, 0.3f);
                Instantiate(acidCloud, transform.position, Quaternion.identity);
            }
            Destroy(this.gameObject);
        }
        if (casing)
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, lifeTimeTimer / lifeTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Bee") && type == BulletType.acid)
        {
            FindObjectOfType<PlayerControls>().PlaySound(clip, 0.3f);
            Instantiate(acidCloud, transform.position, Quaternion.identity);
        }
    }
}
