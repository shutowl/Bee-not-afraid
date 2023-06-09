using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public enum BulletType
    {
        stinger,
        honeybomb,

    }
    public BulletType type;
    public float speed = 10f;           //Projectile speed of
    public float lifeTime = 3f;         //Time before bullet destroys itself
    public float damage = 5f;
    public bool follow = false;
    private float lifeTimeTimer = 0f;

    private Rigidbody2D rb;
    public float x = 0, y = -1;
    private Vector2 direction;

    private void Awake()
    {
        SetDirection(x, y);
    }

    void Start()
    {
        lifeTimeTimer = lifeTime;
        rb = GetComponent<Rigidbody2D>();

        if (type == BulletType.stinger)
        {
            if (!follow)
            {
                Vector3 rotation = -direction;
                rb.velocity = direction * speed;
                float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, rot + 90);
            }
            else
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                rb = GetComponent<Rigidbody2D>();
                Vector3 playerPos = player.transform.position;
                Vector3 direction = playerPos - transform.position;
                Vector3 rotation = transform.position - playerPos;
                rb.velocity = new Vector2(direction.x, direction.y).normalized * speed;
                float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, rot + 90);
            }
        }
    }

    void Update()
    {
        lifeTimeTimer -= Time.deltaTime;
        if (lifeTimeTimer <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void SetDirection(float x, float y)
    {
        direction = new Vector2(x, y).normalized;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Hitbox"))
        {
            col.GetComponentInParent<PlayerControls>().TakeSanity(damage);
            Destroy(this.gameObject);
        }
    }
}
