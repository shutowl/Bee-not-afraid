using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public enum BulletType
    {
        nail,
        minigun,
        acid,
    }
    public BulletType type;
    public float lifeTime = 1f;
    public float speed = 5f;
    public int damage = 10;
    public float bulletSpread = 4f;
    private Rigidbody2D rb;

    void Start()
    {
        if(type == BulletType.nail)
        {
            rb = GetComponent<Rigidbody2D>();
            Camera mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);         //Get mouse position (using world coordinates)
            Vector3 rotation = mousePos - transform.position;                   //Get direction vector from player to mouse
            float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;   //Get angle of direction vector (in degrees)
            transform.rotation = Quaternion.Euler(0, 0, rotZ);                  //Rotate position towards mouse using angle

            rb.velocity = new Vector2(rotation.x, rotation.y).normalized * speed;
        }
        if(type == BulletType.minigun)
        {
            rb = GetComponent<Rigidbody2D>();
            Camera mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition) + new Vector3(Random.Range(-bulletSpread, bulletSpread), Random.Range(-bulletSpread, bulletSpread));         
            Vector3 rotation = mousePos - transform.position;                   //Get direction vector from player to mouse
            float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;   //Get angle of direction vector (in degrees)
            transform.rotation = Quaternion.Euler(0, 0, rotZ);                  //Rotate position towards mouse using angle

            rb.velocity = new Vector2(rotation.x, rotation.y).normalized * speed;
        }
    }

    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if(lifeTime < 0)
        {
            Destroy(this.gameObject);
        }
    }

}
