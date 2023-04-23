using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wasp : Enemy
{
    public float speed = 3f;
    public int damage = 10;
    public float maxDistance = 5.0f;
    private Vector3 targetPosition;
    public float chargeSpeed = 10f;
    public float chargeDuration = 3f;
    float chargeDurationTimer = 0f;
    public float minChargeRate = 1f;
    public float maxChargeRate = 3f;
    bool fired = false;
    float chargeTimer = 0f;
    public int amount = 7;

    public float moveDelay = 0.5f;
    private float moveDelayTimer = 0f;

    Rigidbody2D rb;

    new void Start()
    {
        base.Start();
        chargeTimer = Random.Range(minChargeRate, maxChargeRate);
        rb = GetComponent<Rigidbody2D>();

        SetNewTarget();
    }

    new void Update()
    {
        base.Update();

        if(state == State.moving)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                SetNewTarget();
            }

            chargeTimer -= Time.deltaTime;
            if(chargeTimer <= 0)
            {
                chargeDurationTimer = chargeDuration;
                moveDelayTimer = moveDelay;
                fired = false;
                state = State.attacking;
            }
        }
        else if(state == State.attacking)
        {
            if (!fired)
            {
                Vector2 direction = GameObject.FindGameObjectWithTag("Player").transform.position - transform.position;
                rb.velocity = new Vector2(direction.x, direction.y).normalized * chargeSpeed;

                chargeTimer = Random.Range(minChargeRate, maxChargeRate);
                fired = true;
            }

            chargeDurationTimer -= Time.deltaTime;
            if(chargeDurationTimer <= 0)
            {
                rb.velocity = Vector2.zero;

                moveDelayTimer -= Time.deltaTime;
                if (moveDelayTimer <= 0)
                {
                    state = State.moving;
                }
            }
        }
    }

    void SetNewTarget()
    {
        targetPosition = new Vector3(Random.Range(-maxDistance, maxDistance), Random.Range(-maxDistance, maxDistance), 0);
    }

    public override void Die()
    {
        GameObject sanityDrop = Instantiate(sanity, transform.position, Quaternion.identity);
        sanityDrop.GetComponent<SanityDrop>().SetAmount(amount);

        float RNG = Random.Range(0f, 1f);
        if (RNG < 0.1f) Instantiate(pickups[Random.Range(0, pickups.Length)], transform.position, Quaternion.identity);
        FindObjectOfType<PlayerControls>().PlaySound(deathSound, 0.4f);
        Destroy(this.gameObject);
    }

    private new void OnTriggerEnter2D(Collider2D col)
    {
        base.OnTriggerEnter2D(col);

        if (col.CompareTag("Player"))
        {
            col.GetComponent<PlayerControls>().TakeSanity(10);
        }
    }
}
