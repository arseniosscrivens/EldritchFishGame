using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBehavior : MonoBehaviour
{
    //determine when food will expire
    float foodTimer = 2f;
    bool onGround = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //stop acceleration and velocity (freeze position) when it hits ground of aqaurium
        if (this.gameObject.transform.position.y < -6)
        {
            this.GetComponent<Rigidbody2D>().gravityScale = 0.0f;
            this.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            onGround = true;
        }

        if (onGround == true && foodTimer > 0)
        {
            foodTimer -= Time.deltaTime;
            this.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, foodTimer / 2f);
        }
        else if(onGround == true)
        {
            Destroy(this.gameObject);
        }
    }
}
