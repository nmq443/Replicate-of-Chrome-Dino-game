using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void MovingLeft() {
        rb.velocity = Vector2.left * GameController.Instance.gameSpeed;
    }

    public bool OutOfBound() {
        return transform.position.x <= -10;
    }

    public void StopMoving() {
        rb.velocity = Vector2.zero;
    }
}
