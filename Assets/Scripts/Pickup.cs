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
            Destroy(this.gameObject);
        }
    }
}
