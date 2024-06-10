using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thoughts : MonoBehaviour
{
    //create a timer variable for fish hunger.
    public float hungerTimer = 3f;
    public float deathTimer = 5f;
    public bool isHungry = false;
    public bool isFull = false;

    //get objects for spawning
    public GameObject thoughtBubblePrefab;
    public GameObject deadFishPrefab;
    GameObject thoughtBubble;
    Animator anim;
    Vector3 bubblePos;
    SpriteRenderer sr;
    float offset;

    //get object for tracking
    Rigidbody2D fishrb;

    //bool to tell if thoughtBubble exists or not
    bool thinking = false;

    //get scripts
    BaseFishMovement script_fishMovement;

    // Start is called before the first frame update
    void Start()
    {
        //set scripts
        script_fishMovement = this.GetComponent<BaseFishMovement>();

        //get animator
        anim = thoughtBubblePrefab.GetComponent<Animator>();

        //get rigidbody2D of fish
        fishrb = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //lower hungerTimer according to time passed
        if(hungerTimer > 0)
        {
            hungerTimer -= Time.deltaTime;
        }
        else
        {
            //fish is hungry
            isHungry = true;
        }

        //lower deathTime according to time passed since hunger
        if(isHungry && deathTimer > 0)
        {
            deathTimer -= Time.deltaTime;
        }
        else if(isHungry)
        {
            //fish is dead
            //TODO
            GameObject.Instantiate(deadFishPrefab, this.transform.position, this.transform.rotation);
            Destroy(thoughtBubble);
            script_fishMovement.isDead = true;
            Destroy(this);
        }



        //activate hunger thought bubble if hungry
        if(isHungry && (thinking != true))
        {
            bubblePos = new Vector3(this.transform.position.x + fishrb.position.x, this.transform.position.y + fishrb.position.y, this.transform.position.z);
            thoughtBubble = GameObject.Instantiate(thoughtBubblePrefab, bubblePos, this.transform.rotation);
            sr = thoughtBubble.GetComponent<SpriteRenderer>();
            anim = thoughtBubble.GetComponent<Animator>();
            anim.SetTrigger("Hunger");
            thinking = true;
            offset = 1;
        }

        //control bubble sprite to follow fish
        if (thinking)
        {
            if (fishrb.position.x > 8 && sr.flipX != true)
            {
                sr.flipX = true;
                offset = -1;
            }
            else if (fishrb.position.x < -14 && sr.flipX != false)
            {
                sr.flipX = false;
                offset = 1;
            }

            bubblePos = new Vector3(fishrb.position.x + (2*offset), fishrb.position.y + 2, this.transform.position.z);
            thoughtBubble.transform.localPosition = bubblePos;
        }
        
        //destroy bubble upon eating
        if(isFull)
        {
            Destroy(thoughtBubble);
            isHungry = false;
            hungerTimer = 3f;
            deathTimer = 5f;
            isFull = false;
            thinking = false;
        }
    }
}
