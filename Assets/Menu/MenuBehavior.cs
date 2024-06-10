using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MenuBehavior : MonoBehaviour
{
    //create variable to register first position of menuItem
    private float startX, startY;

    //understand that an object is currently being held (mouse button held down)
    private bool isBeingHeld = false;
    
    //create objects to hold data/copy data to
    private GameObject menuItemCopy;
    public GameObject food, fish;
    
    //create variables to register bounds area
    //TODO
    public GameObject background;
    float minX, maxX, minY, maxY;

    // Start is called before the first frame update
    // Set bounds area
    void Start()
    {
        //TODO
        background.GetComponent<BoxCollider2D>()
    }

    // Update is called once per frame
    void Update()
    {
        //if object is held, move with mouse
        if(isBeingHeld == true)
        {
            //get mouse position
            Vector3 mouse;
            mouse = Input.mousePosition;
            mouse = Camera.main.ScreenToWorldPoint(mouse);

            //move menuItem with mouse
            menuItemCopy.transform.localPosition = new Vector3(mouse.x - startX, mouse.y - startY, 0);       
        }
    }

    // Called when mouse button is clicked
    private void OnMouseDown()
    {
        //if mouse button is held down
        if (Input.GetMouseButtonDown(0))
        {
            //get mouse position
            Vector3 mouse;
            mouse = Input.mousePosition;
            mouse = Camera.main.ScreenToWorldPoint(mouse);

            //get first mouse position
            startX = mouse.x - this.transform.localPosition.x;
            startY = mouse.y - this.transform.localPosition.y;

            //create copy of menuItem to move around (NOT ACTUAL OBJECT/PREFAB)
            menuItemCopy = GameObject.Instantiate(this.gameObject);
            menuItemCopy.transform.localScale += new Vector3(-.25f, -.25f, 0f);

            //is being held
            isBeingHeld = true;

            //disable hitbox
            menuItemCopy.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    // Called when mouse button is released
    private void OnMouseUp()
    {
        //nothing is being held so set to false
        isBeingHeld = false;

        //only create object if in position with the aquarium and bounded by box collider of spawning object NOT menuCopy
        var copyXL = menuItemCopy.transform.position.x - (this.GetComponent<BoxCollider2D>().size.x / 2);
        var copyXR = menuItemCopy.transform.position.x + (this.GetComponent<BoxCollider2D>().size.x / 2);
        var copyYT = menuItemCopy.transform.position.y + (this.GetComponent<BoxCollider2D>().size.y / 2);
        var copyYB = menuItemCopy.transform.position.y - (this.GetComponent<BoxCollider2D>().size.y / 2);

        if (copyXR < 11.66 && copyXL > -19 && copyYB > -6 && copyYT < 12)
        {
            //perform if else scenarios to instantiate objects based on menu choice
            if (this.gameObject.name == "Menu_Food")
            {
                GameObject.Instantiate(food, menuItemCopy.transform.position, menuItemCopy.transform.rotation);
            }
            else if(this.gameObject.name == "Menu_Fish")
            {
                GameObject.Instantiate(fish, menuItemCopy.transform.position, menuItemCopy.transform.rotation);
            }
        }

        //destroy held item to avoid duplicates
        Destroy(menuItemCopy);
    }
}
