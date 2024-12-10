using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingPad : MonoBehaviour
{
    public float bounceForce = 10f; // Lực đẩy lên
    public Animator padAnimator; // Animator cho hiệu ứng đệm nhún

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                // Đẩy người chơi lên
                playerRb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);

                // Kích hoạt hiệu ứng đệm nhún
                if (padAnimator != null)
                {
                    padAnimator.SetTrigger("bounce");
                }
            }
        }
    }
}
