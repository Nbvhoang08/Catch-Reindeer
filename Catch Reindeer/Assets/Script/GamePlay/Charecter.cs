using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Charecter : MonoBehaviour
{
    // Start is called before the first frame update
    public bool Caught;
    public GameObject Cage;
    public int numDeer;
   
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Cage")) 
        {
            if(collision.gameObject == Cage && Caught)
            {
                Caught = false;
                SoundManager.Instance.PlayVFXSound(1);
                numDeer ++;
            }
        }
    }
}

