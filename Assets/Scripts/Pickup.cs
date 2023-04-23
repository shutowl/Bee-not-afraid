using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum Weapon
    {
        nailgun,
        minigun,
        shotgun,
        pesticide,
        pan
    }
    public Weapon weapon;
    public string text;
    public float lifeTime = 10f;
    public AudioClip clip;

    void Update()
    {
        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0) Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            col.GetComponent<PlayerControls>().weapon = (PlayerControls.Weapon)this.weapon;
            col.GetComponent<PlayerControls>().SetWeaponSprite(GetComponent<SpriteRenderer>().sprite);
            col.GetComponent<PlayerControls>().SetWeaponText(text);
            col.GetComponent<PlayerControls>().RestoreSanity(20);
            col.GetComponent<PlayerControls>().PlaySound(clip, 0.2f);
            Destroy(this.gameObject);
        }
    }
}
