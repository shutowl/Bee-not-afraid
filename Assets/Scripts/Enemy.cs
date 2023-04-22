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


    public void Start()
    {
        state = State.moving;
    }

    public void Update()
    {
        
    }

    public virtual void TakeDamage(int damage)
    {
        hp -= damage;

        if(hp <= 0)
        {
            Instantiate(sanity, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("PlayerBullet"))
        {
            TakeDamage(col.GetComponent<PlayerBullet>().damage);
            Destroy(col.gameObject);
        }
    }
}
