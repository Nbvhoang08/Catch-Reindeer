﻿using JetBrains.Annotations;
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
                if (transform.position.x > screenWidth )
                {
                    movingRight = false;
                }
            }
            else
            {
                sprite.flipX = false;
                transform.Translate(Vector3.left * speed * Time.deltaTime);
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
                if (transform.position.x < -screenWidth)
                {
                    movingRight = true;
                }
            }
            yield return null;
        }

        while (isCaught && player != null)
        {
            // Di chuyển theo player
            if (player.GetComponent<Charecter>().Caught)
            {
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
            }
            else
            {
                transform.position = player.Cage.transform.position;
                player = null;
                break;
            }
            
            yield return null;
        }
      
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isCaught && !collision.GetComponent<Charecter>().Caught)
        {
            isCaught = true;
            player = collision.GetComponent<Charecter>();
            collision.GetComponent<Charecter>().Caught = true;
        }
        

        
    }

    

  





}