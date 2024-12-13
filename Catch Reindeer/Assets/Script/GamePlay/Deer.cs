using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deer : MonoBehaviour
{
    public float speed = 2f; // Tốc độ di chuyển của hươu
    public float verticalAmplitude = 0.5f; // Biên độ dao động theo trục y
    public float verticalFrequency = 1f; // Tần số dao động theo trục y
    private float screenWidth;
    private bool movingRight = true;
    private float initialY;
    private bool isCaught = false;
    private Charecter player ;
    private Transform pen; // Chuồng của player
    SpriteRenderer sprite;
    public DeerSpawner spawner;
    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }
    public void Initialize(float screenWidth)
    {
        this.screenWidth = screenWidth;
        initialY = transform.position.y;
        StartCoroutine(MoveDeer());
    }

    IEnumerator MoveDeer()
    {
        while (!isCaught)
        {
            float newY = initialY + Mathf.Sin(Time.time * verticalFrequency) * verticalAmplitude;

            if (movingRight)
            {
                sprite.flipX = true;
                transform.Translate(Vector3.right * speed * Time.deltaTime);
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
                if (transform.position.x > screenWidth+3 )
                {
                    movingRight = false;
                    speed = Random.Range(1, 3);
                }
            }
            else
            {
                sprite.flipX = false;
                transform.Translate(Vector3.left * speed * Time.deltaTime);
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
                if (transform.position.x <= -screenWidth-3)
                {
                    movingRight = true;
                    speed = Random.Range(1, 3);
                }
            }
            yield return null;
        }

        while (isCaught && player != null)
        {
            // Di chuyển theo player
            if (player.GetComponent<Charecter>().Caught)
            {
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 3* Time.deltaTime);
            }
            else
            {
                float randomPos = Random.Range(-1f, 0.9f);
                Vector3 CagePos = new Vector3(player.Cage.transform.position.x + randomPos, -3.15f, 0);
                transform.position = CagePos;
                player = null;
                spawner.RemoveDeer(this.gameObject);
                break;
            }
            
            yield return null;
        }
      
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.CompareTag("Player")|| collision.CompareTag("Enemy")) && !isCaught && !collision.GetComponent<Charecter>().Caught)
        {
            isCaught = true;
            player = collision.GetComponent<Charecter>();
            collision.GetComponent<Charecter>().Caught = true;
            SoundManager.Instance.PlayVFXSound(0);
        } 
    }

    

  





}
