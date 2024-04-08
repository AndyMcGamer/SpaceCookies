using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Spaceship : MonoBehaviour
{
    [SerializeField] private Collider2D col;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform gunPosition;
    [SerializeField] private Transform bulletContainer;

    [SerializeField] private float invincibilityTime;

    [SerializeField] private int defaultCapacity;
    [SerializeField] private int maxCapacity;

    private IObjectPool<Bullet> objectPool;
    private float shootTimer;

    private void Awake()
    {
        objectPool = new ObjectPool<Bullet>(CreateProjectile, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject, true, defaultCapacity, maxCapacity);
    }

    private Bullet CreateProjectile()
    {
        var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity, bulletContainer).GetComponent<Bullet>();
        bullet.Pool = objectPool;
        bullet.gameObject.SetActive(false);
        return bullet;
    }

    private void OnReleaseToPool(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
    }

    private void OnGetFromPool(Bullet bullet)
    {
        bullet.transform.position = transform.position;
        bullet.gameObject.SetActive(true);
    }

    private void OnDestroyPooledObject(Bullet bullet)
    {
        Destroy(bullet.gameObject);
    }


    private void Update()
    {
        if(shootTimer > 0)
        {
            shootTimer -= Time.deltaTime;
        }
        else if ((Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) && objectPool != null && !GameManager.instance.coolingDown)
        {
            Bullet bullet = objectPool.Get();

            if(bullet != null)
            {
                AudioManager.instance.Play("Shoot");
                GameManager.instance.ShakeCamera(0.05f);
                GameManager.instance.currentOverload += GameManager.instance.overloadRate;
                bullet.Init(gunPosition.position, transform.rotation, GameManager.instance.bulletSpeed, GameManager.instance.bulletDamage, GameManager.instance.powerups["Wrap"]);
            }

            shootTimer = GameManager.instance.fireRate;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Asteroid"))
        {
            var asteroid = collision.gameObject.GetComponent<Asteroid>();
            asteroid.TakeDamage(GameManager.instance.bulletDamage * 2f);
            AudioManager.instance.Play("Hit");
            GameManager.instance.ShakeCamera(0.15f);
            Vector2 dir = transform.position - asteroid.transform.position;
            GetComponent<Rigidbody2D>().AddForce(dir.normalized * 50, ForceMode2D.Impulse);
            if (GameManager.instance.coolingDown || !GameManager.instance.powerups["Speed"] || !Input.GetKey(KeyCode.LeftShift))
            {
                StartCoroutine(TakeDamage(asteroid));
            }
        }
    }

    private IEnumerator TakeDamage(Asteroid a)
    {
        GameManager.instance.playerHealth -= a.damage;
        col.enabled = false;
        if (GameManager.instance.playerHealth <= 0)
        {
            GameManager.instance.playerHealth = 0;
            AudioManager.instance.Play("Death");
            yield return new WaitForSeconds(0.5f);
            if (GameManager.instance.playerHealth <= 0) GameManager.instance.GameOver();
            else col.enabled = true;
        }
        else
        {
            float t = 0;
            float interval = 0.1f;
            float nextInterval = interval;
            Color original = sprite.color;
            Color transparent = original;
            transparent.a = 0;
            bool isOriginal = false;
            sprite.color = transparent;
            while (t < invincibilityTime)
            {
                t += Time.deltaTime;
                if (t > nextInterval)
                {
                    nextInterval += interval;
                    sprite.color = isOriginal ? transparent : original;
                    isOriginal = !isOriginal;
                }
                yield return null;
            }
            col.enabled = true;
            sprite.color = original;

        }
        
        
    }
}
