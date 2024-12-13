using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingPad : MonoBehaviour
{
    public float bounceForce = 10f; // Lực đẩy lên
    public Animator padAnimator; // Animator cho hiệu ứng đệm nhún

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")|| collision.gameObject.CompareTag("Enemy"))
        {
            Rigidbody2D Rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (Rb != null)
            {
                // Đẩy người chơi lên
                Rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
                SoundManager.Instance.PlayVFXSound(3);
                // Kích hoạt hiệu ứng đệm nhún
                if (padAnimator != null)
                {
                    padAnimator.SetTrigger("bounce");
                }
            }
        }
    }
}
