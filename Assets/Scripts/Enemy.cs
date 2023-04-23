using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum State
    {
        moving,
        attacking
    }
    public State state;
    public int hp;
    public GameObject sanity;
    public GameObject[] pickups;
    bool alreadyDead;

    SpriteRenderer sprite;
    GameObject player;

    public AudioClip deathSound;

    public void Start()
    {
        state = State.moving;
        sprite = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Update()
    {
        if (transform.position.x > player.transform.position.x) sprite.flipX = true;
        else sprite.flipX = false;
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;

        if(hp <= 0 && !alreadyDead)
        {
            Die();
            alreadyDead = true;
        }
    }

    public virtual void Die()
    {
        /*
        Instantiate(sanity, transform.position, Quaternion.identity);

        float RNG = Random.Range(0f, 1f);
        if (RNG < 0.1f) Instantiate(pickups[Random.Range(0, pickups.Length)], transform.position, Quaternion.identity);
        Destroy(this.gameObject);
        */
    }

    public virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("PlayerBullet"))
        {
            TakeDamage(col.GetComponent<PlayerBullet>().damage);
            Destroy(col.gameObject);
        }
    }
}
