using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*This goes on the player to move it left and right and jump
requires 3 axes to be set up for input. 2 for horizontal movement (left stick and d pad)

Here are the values for the input
Horizontal 
Gravity: 3 
Dead: 1 
Sensitivity: 1 
XAxis

altHorizontal 
Gravity: 0 
Dead: .1 
Sensitivity: 1 
6thAxis

Jump
Gravity: 1000
Dead: .01
Sensitivity: 1000

Here are the values I found work well in the Serialize Field
Speed: 10
Jump Cooldown: .2
Jump Strength: 12
Fall Multiplier: 3
Normal Gravity Multiplier: 3

This script also relies on a Layermask that determines what the player can jump on. 
To make an object the player can jump on, it needs to be on a layer called "ground"
 */

public class MovePlayer : MonoBehaviour
{//Movement stuff
    [SerializeField]
    private string horizontalAxisName;

    [SerializeField]
    private string altHorizontalAxisName;

    [SerializeField]
    private float speed;

    [SerializeField]
    private string jumpAxisName;

//Jump stuff
    [SerializeField]
    private float jumpCooldownNumber;

    [SerializeField]
    float jumpStrength;

    [SerializeField]
    private float fallMultiplier;//gravity on the way down

    [SerializeField]
    private float normalGravityMultiplier;//gravity on the way up

    [SerializeField]
    private LayerMask ground;//This is how we know what the player can jump on

    private float horizontalAxisValue;
    private float jumpAxisValue;

    private Vector2 jumpForce;
    private Rigidbody2D rigidbody;
    RaycastHit2D rayHit;
    private BoxCollider2D collider;

    private bool canJump = true;//a bool that manages a cooldown time after 1 jump;


	void Start ()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        rayHit = new RaycastHit2D();
        CheckWhichControllerConnected();
        jumpForce = new Vector2(0, jumpStrength);
    }
	
	void FixedUpdate ()
    {
        GetInput();
        Move();
        Jump();
    }

    private bool isOnGround()//this sees right below the player's feet and if it is on the ground layer, it returns true
    {
        rayHit = Physics2D.Raycast(transform.position, Vector2.down, GetComponent<Renderer>().bounds.extents.y + .1f, ground);
        if (rayHit.collider != null)
        {
            return true;
        }
        return false;
    }

    void GetInput()
    {
        if (Mathf.Abs(Input.GetAxis(horizontalAxisName)) < Mathf.Abs(Input.GetAxis(altHorizontalAxisName)))
        {
            horizontalAxisValue = Input.GetAxis(altHorizontalAxisName);
        }
        else
            horizontalAxisValue = Input.GetAxis(horizontalAxisName);

        jumpAxisValue = Input.GetAxis(jumpAxisName);
    }

    void Move()
    {
        transform.Translate(horizontalAxisValue * speed * Time.deltaTime, 0, 0);
    }

    void Jump()
    {
        if (rigidbody.velocity.y < 0)
        {
            rigidbody.gravityScale = fallMultiplier;//this controls how fast the player falls
        }
        else
        {
            rigidbody.gravityScale = normalGravityMultiplier;
        }
        if (canJump && isOnGround() && Input.GetAxis("Jump") > 0)
        {
            rigidbody.AddForce(jumpForce, ForceMode2D.Impulse);
            //play jump sound
            //change jump animation
            StartCoroutine(JumpCooldown());
        }
    }

    private IEnumerator JumpCooldown()
    {
        canJump = false;
        yield return new WaitForSeconds(jumpCooldownNumber);
        canJump = true;
    }

    void CheckWhichControllerConnected()
    {
        string[] names = Input.GetJoystickNames();
        for (int x = 0; x < names.Length; x++)
        {
            Debug.Log(names.Length.ToString());
            if (names[x].Length == 33)
                Debug.Log("XBOX ONE CONTROLLER IS CONNECTED");

            else
                Debug.Log("NO CONTROLLERS DETECTED");
        }
    }    
}
