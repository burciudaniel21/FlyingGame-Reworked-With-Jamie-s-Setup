using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Collectable : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {

        //Destroy piggie based on force applied
        if (this.GetComponent<Rigidbody>().velocity.magnitude > 0.8f)
        {
           
            GameObject.Destroy(this.gameObject);

            //enabled = false;
        }
    }
}
