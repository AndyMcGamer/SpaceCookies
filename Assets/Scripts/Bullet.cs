using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    private IObjectPool<Bullet> pool;

    public IObjectPool<Bullet> Pool { set =>  pool = value; }

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float offset;

    private Coroutine disableCoroutine;
    private float damage;
    private bool wrap;
    
    public void Init(Vector3 position, Quaternion rotation, float speed, float damage, bool wrapBullet)
    {
        rb.position = position;
        transform.rotation = rotation;
        rb.velocity = transform.up * speed;
        this.damage = damage;
        wrap = wrapBullet;
        disableCoroutine = StartCoroutine(Deactivate());
    }

    private void FixedUpdate()
    {
        if (!wrap) return;
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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Asteroid"))
        {
            if (disableCoroutine != null)
            {
                StopCoroutine(disableCoroutine);
            }
            AudioManager.instance.Play("Hit");
            var asteroid = collision.GetComponent<Asteroid>();
            asteroid.TakeDamage(damage);
            pool.Release(this);
        }
    }

    private IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(GameManager.instance.bulletDuration);

        pool.Release(this);
    }
}
