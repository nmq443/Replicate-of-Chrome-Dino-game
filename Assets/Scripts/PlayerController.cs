using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    [SerializeField] float velocity = 1.0f;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void Jumping() {
        rb.AddForce(Vector2.up * velocity, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Obstacle") {
            GameController.Instance.gameState = GameState.Ending;
        }
    }

    public void StopMoving() {
        animator.enabled = false;
    }
}
