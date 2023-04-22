using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidCloud : MonoBehaviour
{
    public float duration = 3f;
    float lifeTimeTimer = 0f;
    public int damage = 1;
    public float tickRate = 0.2f;
    float tickTimer = 0f;
    ParticleSystem particles;
    ParticleSystem.MainModule main;

    void Start()
    {
        particles = GetComponentInChildren<ParticleSystem>();
        particles.Stop();
        main = GetComponentInChildren<ParticleSystem>().main;
        main.duration = duration;
        particles.Play();

        lifeTimeTimer = duration;
    }

    // Update is called once per frame
    void Update()
    {
        if(tickTimer >= 0)
            tickTimer -= Time.deltaTime;

        lifeTimeTimer -= Time.deltaTime;
        if(lifeTimeTimer <= -1)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Bee"))
        {
            if(tickTimer < 0)
            {
                col.GetComponent<Enemy>().TakeDamage(damage);
                tickTimer = tickRate;
            }
        }
    }
}
