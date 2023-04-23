using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee : Enemy
{
    public float speed = 3f;
    public int amount = 5;
    public float changeInterval = 2f;
    public float cameraTargetDistance = 2f;
    public Camera mainCamera;

    private Vector2 direction;
    private float timer;

    public float minFireRate = 0.5f;
    public float maxFireRate = 2f;
    private float fireRateTimer = 0f;
    public float moveDelay = 0.5f;
    private bool fired = false;
    private float moveDelayTimer = 0f;
    public GameObject stinger;

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

        if(state == State.moving)
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

            if(fireRateTimer <= 0)
            {
                moveDelayTimer = moveDelay;
                fired = false;
                state = State.attacking;
            }
            else
            {
                fireRateTimer -= Time.deltaTime;
            }
        }
        else if(state == State.attacking)
        {
            if (!fired)
            {
                Instantiate(stinger, transform.position, Quaternion.identity);
                fireRateTimer = Random.Range(minFireRate, maxFireRate);
                fired = true;
            }

            moveDelayTimer -= Time.deltaTime;
            if(moveDelayTimer <= 0)
            {
                state = State.moving;
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
}
