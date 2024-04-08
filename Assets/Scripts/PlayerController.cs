using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Settings")]
    [SerializeField] private float offset;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float thrust;

    private Vector3 input;
    private float thrustPower;

    private void Update()
    {
        input = Vector3.zero;

        if (Input.GetKey(KeyCode.A))
        {
            input.x = 1;
        }
        if (Input.GetKey(KeyCode.D)) 
        {
            input.y = 1;
        }
        if (Input.GetKey(KeyCode.W))
        {
            input.z = 1;
        }
        
        thrustPower = Input.GetKey(KeyCode.LeftShift) && GameManager.instance.powerups["Speed"] && !GameManager.instance.coolingDown ? GameManager.instance.maxThrust : GameManager.instance.thrust;
    }

    private void FixedUpdate()
    {
        float turnAmount = (input.x * turnSpeed - input.y * turnSpeed) * Time.fixedDeltaTime;
        rb.MoveRotation(Quaternion.Euler(0, 0, rb.rotation) * Quaternion.Euler(0, 0, turnAmount));
        rb.AddForce(input.z * thrustPower * transform.up);
        if(rb.velocity.sqrMagnitude > maxSpeed * maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
        CheckPosition();
    }

    private void CheckPosition()
    {
        float halfWidth = GameManager.instance.ScreenWidth / 2.0f + offset;
        float halfHeight = GameManager.instance.ScreenHeight / 2.0f + offset;

        if (rb.position.x > halfWidth)
        {
            rb.MovePosition(new Vector2(-halfWidth + offset, rb.position.y));
        }else if(rb.position.x < -halfWidth)
        {
            rb.MovePosition(new Vector2(halfWidth - offset, rb.position.y));
        }

        if(rb.position.y > halfHeight)
        {
            rb.MovePosition(new Vector2(rb.position.x, -halfHeight + offset));
        }else if(rb.position.y < -halfHeight)
        {
            rb.MovePosition(new Vector2(rb.position.x, halfHeight - offset));
        }
    }
}
