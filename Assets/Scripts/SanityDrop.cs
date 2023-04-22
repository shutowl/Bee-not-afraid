using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanityDrop : MonoBehaviour
{
    public float delay = 2f;
    public float amount = 5f;
    public float speed = 10f;
    private Rigidbody2D rb;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if(delay < 0)
        {
            // Calculate the position the object should move towards
            Vector3 targetPosition = new(player.position.x, player.position.y, transform.position.z);

            // Move the object towards the target position using lerp
            transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.fixedDeltaTime);

            speed += 0.05f;
        }
        else
        {
            delay -= Time.fixedDeltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            col.GetComponent<PlayerControls>().RestoreSanity(amount);
            Destroy(this.gameObject);
        }
    }
}
