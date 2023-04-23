using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BARRYBEE : Enemy
{
    public float speed = 3f;
    public int amount = 5;
    public float changeInterval = 2f;
    public float cameraTargetDistance = 2f;
    public Camera mainCamera;

    private Vector2 direction;
    private float timer;

    new void Start()
    {
        base.Start();
        // Set a random initial direction
        direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

        // Find the main camera if it's not specified
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    new void Update()
    {
        base.Update();

        if (state == State.moving)
        {
            // Calculate the direction towards the camera
            Vector2 cameraDirection = ((Vector2)mainCamera.transform.position - (Vector2)transform.position).normalized;

            // Calculate the distance from the enemy to the camera
            float distanceToCamera = Vector2.Distance(transform.position, mainCamera.transform.position);

            // Move the enemy in a combination of its current direction and the direction towards the camera
            Vector2 moveDirection = (direction + cameraDirection.normalized * cameraTargetDistance).normalized;
            transform.position += (Vector3)moveDirection * speed * Time.deltaTime;

            // Update the timer
            timer += Time.deltaTime;

            // Check if it's time to change direction
            if (timer > changeInterval)
            {
                // Set a new random direction
                direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

                // Reset the timer
                timer = 0f;
            }
        }

    }

    public override void Die()
    {
        GameObject sanityDrop = Instantiate(sanity, transform.position, Quaternion.identity);
        sanityDrop.GetComponent<SanityDrop>().SetAmount(amount);

        float RNG = Random.Range(0f, 1f);
        if (RNG < 0.05f) Instantiate(pickups[Random.Range(0, pickups.Length)], transform.position, Quaternion.identity);
        FindObjectOfType<PlayerControls>().PlaySound(deathSound, 0.4f);
        Destroy(this.gameObject);
    }

    private new void OnTriggerEnter2D(Collider2D col)
    {
        base.OnTriggerEnter2D(col);

        if (col.CompareTag("Player"))
        {
            col.GetComponent<PlayerControls>().TakeSanity(50);
        }
    }
}
