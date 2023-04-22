using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Spawner : MonoBehaviour
{
    [SerializeField] Vector2[] boundaries = new Vector2[2];
    public GameObject[] enemies;
    public float spawnRate = 2f;
    private float spawnTimer = 0f;
    public float spawnRadius = 5f;
    [SerializeField] float sanityOffset = 0f;   //Increases based on how low sanity is;

    private GameObject player;
    private PlayerControls playerScript;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerControls>();
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnRate - (sanityOffset/40))
        {
            for(int i = 0; i < Random.Range(1 + (int)(sanityOffset/40), 3 + (int)(sanityOffset / 40)); i++)
            {
                SpawnEnemy();
            }
            spawnTimer = 0f;
        }

        sanityOffset = 100 - playerScript.GetSanity();
    }

    void SpawnEnemy()
    {
        Vector2 spawnPosition = new Vector2(Random.Range(boundaries[0].x, boundaries[1].x), Random.Range(boundaries[0].y, boundaries[1].y));
        while (Vector2.Distance(spawnPosition, player.transform.position) < spawnRadius)
        {
            spawnPosition = new Vector2(Random.Range(boundaries[0].x, boundaries[1].x), Random.Range(boundaries[0].y, boundaries[1].y));
        }
        Instantiate(enemies[0], spawnPosition, Quaternion.identity);
    }

    public void SetBoundaries()
    {
        RectTransform rect = GetComponent<RectTransform>();
        boundaries[0] = new Vector2(rect.anchoredPosition.x - rect.sizeDelta.x/2, rect.anchoredPosition.y + rect.sizeDelta.y/2);
        boundaries[1] = new Vector2(rect.anchoredPosition.x + rect.sizeDelta.x/2, rect.anchoredPosition.y - rect.sizeDelta.y/2);
    }
}

[CustomEditor(typeof(Spawner))]
public class SpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();    //This goes first

        Spawner spawner = (Spawner)target;    //The target script
        if (GUILayout.Button("Set Boundaries"))    // If the button is clicked
        {
            spawner.SetBoundaries();
        }
    }
}