using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject asteroidPrefab;

    [Header("Settings")]
    [SerializeField] private float radiusCheck;
    [SerializeField] private float maxSpawnTimer;
    [SerializeField] private float spawnOffset;


    private float spawnTimer;
    private int numAsteroids;

    private void Awake()
    {
        spawnTimer = 0;
        numAsteroids = 0;
    }

    private void Update()
    {
        if(spawnTimer <= 0)
        {
            if (numAsteroids < GameManager.instance.MaxAsteroids) SpawnAsteroid();
            else GameManager.instance.increaseTimer = false;
        }
        else
        {
            spawnTimer -= Time.deltaTime;
        }
    }

    private void SpawnAsteroid()
    {
        GameManager.instance.increaseTimer = true;
        float halfWidth = GameManager.instance.ScreenWidth / 2.0f + spawnOffset;
        float halfHeight = GameManager.instance.ScreenHeight / 2.0f + spawnOffset;
        Vector3 spawnPoint = Vector3.zero;
        bool valid = false;
        while (!valid)
        {
            spawnPoint = new(Random.Range(-halfWidth, halfWidth), Random.Range(-halfHeight, halfHeight), 0.0f);
            valid = !Physics2D.OverlapCircle(spawnPoint, radiusCheck);
        }

        var asteroid = Instantiate(asteroidPrefab, spawnPoint, Quaternion.identity, transform).GetComponent<Asteroid>();
        asteroid.gameObject.name = asteroidPrefab.name;
        asteroid.gameObject.transform.localScale = Vector3.one * GameManager.instance.asteroidScales[^1];
        float health = GameManager.instance.asteroidStartingHealth * GameManager.instance.GrowthFactor / 2.5f;
        if (health < 3) health = 3;
        asteroid.Init(health, GameManager.instance.asteroidScales.Length - 1, GameManager.instance.asteroidOffset, asteroidPrefab, this);
        numAsteroids++;
        spawnTimer = Mathf.Min(maxSpawnTimer / GameManager.instance.GrowthFactor, maxSpawnTimer);

    }

    public void DespawnAsteroid()
    {
        numAsteroids--;
    }
}
