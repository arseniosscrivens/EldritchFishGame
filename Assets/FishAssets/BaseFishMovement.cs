using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseFishMovement : MonoBehaviour
{
    //get scripts
    Thoughts script_thoughts;

    //create gameobject and parts of fish for data storage/manipulation
    public GameObject fish;
    Animator anim;
    Rigidbody2D rb;

    //bool to tell if fish is dead or not
    //bool changed from Thoughts.cs
    //bool is NOT changed from this script
    public bool isDead = false;

    //bool to tell if action is happening or not
    //when false, action needs to be set
    bool ongoingAction;

    //bool to tell if at a wall or not
    bool leftWall = false;
    bool rightWall = false;

    //bool to tell if animation of chaseFood has been set yet
    bool chaseFood = false;

    //duration of action
    float actionTime;

    //random type of action
    int actionDecision = 0;

    //name of type of action for animation purposes (triggers)
    public string actionDecisionType;

    //random y velocity for fish to move in
    float randomYvel, yTop, yBottom;

    // Start is called before the first frame update
    // Set default look of fish.
    void Start()
    {
        //set scripts
        script_thoughts = this.GetComponent<Thoughts>();

        //get Animator and Rigidbody2D for later
        anim = fish.GetComponent<Animator>();
        rb = fish.GetComponent<Rigidbody2D>();

        //set random actionDecisionType on startup for script to work in Update()
        actionDecision = Random.Range(0, 2);
        if(actionDecision == 0)
            actionDecisionType = "LookLeft";
        else
            actionDecisionType = "LookRight";

        //action needs to be decided
        ongoingAction = false;
    }

    // Update is called once per frame
    void Update()
    {
        //check if fish is dead
        if(isDead)
        {
            //if dead, destroy fish
            Destroy(fish);
            Destroy(this);
        }

        //get location of food particle if exists, null if not exists
        var foodParticle = GameObject.FindWithTag("FoodParticle");

        //if: move fish based off action, or stop action
        //else: decide action and animate fish appropriately
        if (ongoingAction)
        {
            //dont set anim trigger, assume it has been set already
            //move fish according to action
            if ((script_thoughts.isHungry == true) && (foodParticle != null))
            {
                //get position of food
                var targetPos = foodParticle.transform.position;

                //fish is hungry and food exists
                //check if fish has correct animation on
                if (chaseFood)
                {
                    //move towards food
                    //TODO
                    //(when food is eaten while fish is tracking said food, it will switch to another food but NOT switch its animation, due to the below code)
                    rb.position = Vector2.MoveTowards(rb.position, targetPos, 5f * Time.deltaTime);
                }
                else
                {
                    //fish does not have correct animation
                    ongoingAction = false;
                }

                //check if fish is close enough to food to eat it
                if ((Mathf.Abs(targetPos.x - rb.position.x) < 2) && (Mathf.Abs(targetPos.y - rb.position.y) < 1))
                {
                    //eat food and destroy food
                    Destroy(foodParticle);
                    
                    //fish is now full
                    script_thoughts.isFull = true;
                    script_thoughts.isHungry = false;

                    //reset action
                    ongoingAction = false;
                    chaseFood = false;
                }

            }
            else
            {
                //fish is hungry and food doesn't exist
                //or
                //fish is not hungry

                //this if statement will only trigger when food dissapears before fish can eat it
                if (chaseFood)
                {
                    chaseFood = false;
                    ongoingAction = false;
                }

                //check actionTime
                if (actionTime > 0)
                {
                    //check if fish is colliding with floor or ceiling
                    if(randomYvel > 0)
                    {
                        //fish is moving up, check if hitting ceiling
                        if (rb.position.y >= 10.62)
                            randomYvel = -randomYvel;
                    }
                    else
                    {
                        //fish is moving down, check if hitting floor
                        if (rb.position.y <= -5.13)
                            randomYvel = -randomYvel;
                    }


                    //move fish based off of movement decision
                    if (actionDecisionType == "MoveLeft")
                    {
                        //is fish colliding with left wall?
                        if(rb.position.x <= -17.67)
                        {
                            //fish is colliding with left wall, reset action
                            ongoingAction = false;
                            leftWall = true;
                        }
                        else
                        {
                            //fish is not colliding with left wall, continue action
                            //left velocity
                            rb.velocity = new Vector2(-3, randomYvel);
                        }
                    }
                    else if (actionDecisionType == "MoveRight")
                    {
                        //is fish colliding with right wall?
                        if(rb.position.x >= 10.25)
                        {
                            //fish is colliding with right wall, reset action
                            ongoingAction = false;
                            rightWall = true;
                        }
                        else
                        {
                            //fish is not colliding with right wall, continue action
                            //right velocity
                            rb.velocity = new Vector2(3, randomYvel);
                        }
                    }
                    else
                    {
                        //reset velocity to 0 b/c standing still
                        rb.velocity = new Vector2(0, 0);
                    }
                }
                else
                {
                    //stop action and decide new one
                    ongoingAction = false;
                }

                //decrease actionTime relative to real time
                actionTime -= Time.deltaTime;
            }
        }
        else
        {
            //create special bool to only be used so
            //anim.SetTrigger doesn't trigger the same anim twice
            //this will cause bugs if it triggers twice
            bool dontTrigger = false;

            //anim trigger needs to be set here
            //check if fish is hungry and food exists
            if ((script_thoughts.isHungry == true) && (foodParticle != null))
            {
                //fish is hungry and food exists
                //animate appropriately
                var targetPos = foodParticle.transform.position;
                if (targetPos.x < rb.position.x)
                {
                    //check if actionDecisionType is already correct
                    if(actionDecisionType == "MoveLeft")
                    {
                        //dont setTrigger, will cause animation bugs
                        dontTrigger = true;
                    }
                    else
                        actionDecisionType = "MoveLeft";
                }
                else
                {
                    //check if actionDecisionType is already correct
                    if (actionDecisionType == "MoveRight")
                    {
                        //dont setTrigger, will cause animation bugs
                        dontTrigger = true;
                    }
                    else
                        actionDecisionType = "MoveRight";
                }

                chaseFood = true;
            }
            else
            {
                //decide random action based off of previous action
                //set duration of action
                actionTime = Random.Range(2f, 9f);

                //preset yTop and yBottom
                yTop = 1.5f; yBottom = -1.5f;

                //determine random y velocity based off of height
                if (rb.position.y >= 10.62)
                    yTop = 0f;
                else
                    yTop = 1.5f;

                if (rb.position.y <= -5.13)
                    yBottom = 0f;
                else
                    yBottom = -1.5f;

                randomYvel = Random.Range(yBottom, yTop);

                //set random action based off of previous action (refer to animator for full list of possiblities)
                //do not move left if leftWall
                //do not move right if rightWall
                //no need to check for wall on move
                if (actionDecisionType == "LookLeft")
                {
                    if (leftWall)
                        actionDecision = Random.Range(1, 3);
                    else if (rightWall)
                        actionDecision = Random.Range(0, 2);
                    else
                        actionDecision = Random.Range(0, 3);

                    if (actionDecision == 0)
                        actionDecisionType = "MoveLeft";
                    else if (actionDecision == 1)
                        actionDecisionType = "LookRight";
                    else
                        actionDecisionType = "MoveRight";
                }
                else if (actionDecisionType == "LookRight")
                {
                    if (leftWall)
                        actionDecision = Random.Range(1, 3);
                    else if (rightWall)
                        actionDecision = Random.Range(0, 2);
                    else
                        actionDecision = Random.Range(0, 3);

                    if (actionDecision == 0)
                        actionDecisionType = "MoveLeft";
                    else if (actionDecision == 1)
                        actionDecisionType = "LookLeft";
                    else
                        actionDecisionType = "MoveRight";
                }
                else if (actionDecisionType == "MoveLeft")
                {
                    actionDecision = Random.Range(0, 2);

                    if (actionDecision == 0)
                        actionDecisionType = "LookLeft";
                    else
                        actionDecisionType = "MoveRight";
                }
                else if (actionDecisionType == "MoveRight")
                {
                    actionDecision = Random.Range(0, 2);

                    if (actionDecision == 0)
                        actionDecisionType = "LookRight";
                    else
                        actionDecisionType = "MoveLeft";
                }
            }

            //set animation according to actionDecisionType
            if(!dontTrigger)
            {
                anim.SetTrigger(actionDecisionType);
            }

            //action is ongoing now
            ongoingAction = true;

            //reset wall values
            rightWall = false;
            leftWall = false;
        }
    }
}
