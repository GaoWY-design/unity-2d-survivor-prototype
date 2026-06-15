using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Home : MonoBehaviour
{
    private bool isNotEnter=false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
   
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player" && isNotEnter)
        {
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(0).gameObject.SetActive(true);
            isNotEnter = false;
        }else if (collision.transform.tag == "Player"&&!isNotEnter)
        {
            transform.GetChild(1).gameObject.SetActive(true);
            transform.GetChild(0).gameObject.SetActive(false);
            isNotEnter = true;
        }
      
    }
}
