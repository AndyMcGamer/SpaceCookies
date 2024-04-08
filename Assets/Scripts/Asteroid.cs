using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    public float offset;
    public float damage;
    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;

    private int masterHealth;
    private float health, maxHealth;
    private float childOffset;

    private GameObject prefab;
    private AsteroidSpawner spawner;

    private void Awake()
    {
        rb.velocity = Quaternion.Euler(0, 0, Random.Range(0, 360)) * new Vector3(Random.Range(minSpeed, maxSpeed), 0, 0);
    }

    public void Init(float health, int masterHealth, float childOffset, GameObject asteroidPrefab, AsteroidSpawner spawner = null)
    {
        this.health = health;
        maxHealth = health;
        this.childOffset = childOffset;
        this.masterHealth = masterHealth;
        prefab = asteroidPrefab;
        this.spawner = spawner;
    }

    private void FixedUpdate()
    {
        float halfWidth = GameManager.instance.ScreenWidth / 2.0f + offset;
        float halfHeight = GameManager.instance.ScreenHeight / 2.0f + offset;

        if (rb.position.x > halfWidth)
        {
            rb.MovePosition(new Vector2(-halfWidth + offset, rb.position.y));
        }
        else if (rb.position.x < -halfWidth)
        {
            rb.MovePosition(new Vector2(halfWidth - offset, rb.position.y));
        }

        if (rb.position.y > halfHeight)
        {
            rb.MovePosition(new Vector2(rb.position.x, -halfHeight + offset));
        }
        else if (rb.position.y < -halfHeight)
        {
            rb.MovePosition(new Vector2(rb.position.x, halfHeight - offset));
        }
        if (rb.velocity.sqrMagnitude > maxSpeed * maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if(health <= 0)
        {
            if(masterHealth - 1 >= 0)
            {
                SpawnChildAsteroids();
            }
            if(spawner != null)
            {
                spawner.DespawnAsteroid();
            }
            GameManager.instance.currentExp += GameManager.instance.asteroidExp[masterHealth];
            AudioManager.instance.Play("AsteroidDeath");
            Destroy(gameObject);
        }
    }

    private void SpawnChildAsteroids()
    {
        int numChildren = Random.Range(2, 4);

        for (int i = 0; i < numChildren; ++i)
        {
            Vector3 dir = Quaternion.Euler(0, 0, Random.Range(0, 360)) * Vector3.right;
            dir.Normalize();
            var child = Instantiate(prefab, transform.position + dir * childOffset, Quaternion.identity, transform.parent);
            child.transform.localScale = Vector3.one * GameManager.instance.asteroidScales[masterHealth - 1];
            child.name = gameObject.name;
            var childAsteroid = child.GetComponent<Asteroid>();
            float reducedRatio = GameManager.instance.asteroidScales[masterHealth - 1] / GameManager.instance.asteroidScales[masterHealth];
            childAsteroid.offset *= reducedRatio;
            childAsteroid.damage *= reducedRatio;
            childAsteroid.Init(maxHealth * reducedRatio, masterHealth - 1, childOffset * reducedRatio, prefab);
            var childRb = child.GetComponent<Rigidbody2D>();
            childRb.mass *= reducedRatio * reducedRatio;
            childRb.AddForce(dir * childOffset / reducedRatio / reducedRatio);
        }
    }
}
