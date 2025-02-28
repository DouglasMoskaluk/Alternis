using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalMovement : MonoBehaviour
{
    [SerializeField] GameObject playerSprite;


    public float walkSpeed;
    public float friction;
    public float maxGravity;
    public float jumpHeight;
    public float ffSpeed;
    public float sprintSpeed;
    public float slideSpeed;
    public float crouchWalkSpeed;
    public float controllerSense;
    public float distToGround;
    public float disToWall;

    float jumpSpeed;
    float airSpeed;
    float curwalkSpeed;
    float idleTime;
    float jumpCount;
    float groundTimer;
    bool isSprinting;
    bool isRight = false;
    bool isLeft = false;
    bool isJumping;
    bool disableStand;
    bool walking;
    bool doubleJump;
    bool crouch;
    bool stopMomentum;
    public bool slide;
    float localX;
    public bool isGrounded;
    Vector3 move;
    Rigidbody rb;
    EssentialGameObjects essentialGameObjects;

    //Check If On Ground
    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag == "Ground" && Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "DisableUncrouch" && this.gameObject.tag == "Player")
        {
            disableStand = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "DisableUncrouch" && this.gameObject.tag == "Player")
        {
            disableStand = false;
        }
    }

    //Check if in Air
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }
    }

    void Start()
    {
        essentialGameObjects = GameObject.FindWithTag("Dont Destroy").GetComponent<EssentialGameObjects>();
        essentialGameObjects.GetComponent<PauseMenu>().canPause = true;
        rb = GetComponent<Rigidbody>();
        localX = transform.localScale.x;
        crouch = false;
        slide = false;
        jumpCount = 0;
        groundTimer = 0;
        disableStand = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(essentialGameObjects.GetComponent<PauseMenu>().isPaused == false)
        {
            //Walk
            if (Input.GetAxis("Horizontal") > controllerSense && !crouch)
            {
                if (!playerSprite.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Walking") && !playerSprite.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("ShifterActivate"))
                    playerSprite.GetComponent<Animator>().SetTrigger("Walk");
                walking = true;
                idleTime = 0;
                rb.velocity = new Vector3(curwalkSpeed, rb.velocity.y, 0);

            }
            else if (Input.GetAxis("Horizontal") < -controllerSense && !crouch)
            {
                if (!playerSprite.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Walking") && !playerSprite.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("ShifterActivate"))
                    playerSprite.GetComponent<Animator>().SetTrigger("Walk");
                walking = true;
                idleTime = 0;
                rb.velocity = new Vector3(-curwalkSpeed, rb.velocity.y, 0);
            }

            //Idle time so players can hold momentum when changing directions
            if (Input.GetAxis("Horizontal") == 0 && walking)
            {
                idleTime++;
            }
            if (!crouch && idleTime > 10)
            {
                walking = false;
                idleTime = 0;

                if (!playerSprite.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("ShifterActivate"))
                    playerSprite.GetComponent<Animator>().SetTrigger("Idle");

                playerSprite.GetComponent<Animator>().SetTrigger("IdleFeet");


            }
            else if (Input.GetAxis("Horizontal") < controllerSense && Input.GetAxis("Horizontal") > -controllerSense && !isGrounded)
            {
                walking = false;
                idleTime = 0;


                if (!playerSprite.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("ShifterActivate"))
                    playerSprite.GetComponent<Animator>().SetTrigger("Idle");

                playerSprite.GetComponent<Animator>().SetTrigger("IdleFeet");
            }


            //Jump
            if (Input.GetButtonDown("Jump") && jumpSpeed <= 7 && jumpCount < 2 && !slide && !crouch)
            {
                jumpCount++;

                rb.velocity = new Vector3(rb.velocity.x, jumpSpeed, 0);

                jumpSpeed = jumpHeight;
                isJumping = true;
            }

            //Sprint
            if (Input.GetButtonDown("Dash") && walking && !crouch && isGrounded)
            {
                isSprinting = true;
                airSpeed = sprintSpeed;
                walking = false;
            }
            if (rb.velocity.x > 0 && isSprinting) //facing right
            {
                rb.velocity = new Vector3(sprintSpeed, rb.velocity.y, 0);
            }
            if (rb.velocity.x < 0 && isSprinting) // facing left
            {
                rb.velocity = new Vector3(-sprintSpeed, rb.velocity.y, 0);
            }

            if (Input.GetAxis("Horizontal") < 0.4 && Input.GetAxis("Horizontal") > -0.4)
            {
                isSprinting = false;
            }

            //Sprint Jump
            if (isSprinting && !isGrounded && (rb.velocity.x > walkSpeed || rb.velocity.x < -walkSpeed) && !crouch)
            {
                if (isRight)
                {
                    rb.velocity = new Vector3(airSpeed, rb.velocity.y, 0);
                }
                if (isLeft)
                {
                    rb.velocity = new Vector3(-airSpeed, rb.velocity.y, 0);
                }
            }
            else if (rb.velocity.x <= walkSpeed && rb.velocity.x >= -walkSpeed && isSprinting)
            {
                isSprinting = false;
            }
            if (airSpeed <= walkSpeed)
            {
                isSprinting = false;
            }

            //Right & Left
            if (Input.GetAxis("Horizontal") > controllerSense)
            {
                isRight = true;
                isLeft = false;
            }
            if (Input.GetAxis("Horizontal") < -controllerSense)
            {
                isRight = false;
                isLeft = true;
            }

            //Crouch
            if (Input.GetAxis("Vertical") < -0.3 && isGrounded && groundTimer > 10)
            {
                transform.localScale = new Vector3(transform.localScale.x, 0.5f, transform.localScale.z);
            }
            else if (Input.GetAxis("Vertical") > -0.3 && !disableStand)
            {
                crouch = false;
                transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
            }
            if (isGrounded && transform.localScale == new Vector3(transform.localScale.x, 0.5f, transform.localScale.z))
            {
                crouch = true;
            }

            if (crouch == false && transform.localScale != new Vector3(transform.localScale.x, 1f, transform.localScale.z))
            {
                transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
            }

            //Crouch Walk
            if (Input.GetAxis("Horizontal") > controllerSense && crouch && !slide)
            {
                isSprinting = false;
                rb.velocity = new Vector3(crouchWalkSpeed, rb.velocity.y, 0);
            }
            else if (Input.GetAxis("Horizontal") < -controllerSense && crouch && !slide)
            {
                isSprinting = false;
                rb.velocity = new Vector3(-crouchWalkSpeed, rb.velocity.y, 0);
            }

            //Slide
            //if (crouch && Input.GetAxis("Horizontal") > 0.2 && Input.GetButtonDown("Dash") && !slide)
            // {
            //     slide = true;
            //    rb.velocity = new Vector3(slideSpeed, rb.velocity.y, 0);
            // } else if (crouch && Input.GetAxis("Horizontal") < -0.2 && Input.GetButtonDown("Dash") && !slide)
            //  {
            //      slide = true;
            //      rb.velocity = new Vector3(-slideSpeed, rb.velocity.y, 0);
            //  }
            //  if (slide && rb.velocity.x < 2 && rb.velocity.x > -2)
            //  {
            //     slide = false;
            //  }

            //Disable Sprint
            if (rb.velocity.x <= walkSpeed && rb.velocity.x >= -walkSpeed && isSprinting)
            {
                isSprinting = false;
            }

            //Reset Current Walk Speed
            if (!walking && !isSprinting)
            {
                curwalkSpeed = 0;
            }


            //Sprite Change Direction
            if (isRight && !slide)
            {
                transform.localScale = new Vector3(localX, transform.localScale.y, transform.localScale.z);
            }
            else if (isLeft && !slide)
            {
                transform.localScale = new Vector3(-localX, transform.localScale.y, transform.localScale.z);
            }

            //Stop Running Into Walls
            if (!crouch)
            {
                if (isRight)
                {
                    for (float i = -1.25f; i < 1f; i += 0.05f)
                    {
                        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + i, transform.position.z), Vector3.right, disToWall + 0.1f))
                        {
                            stopMomentum = true;
                            break;
                        }
                    }
                }
                if (isLeft)
                {
                    for (float i = -1.25f; i < 1f; i += 0.05f)
                    {
                         if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + i, transform.position.z), Vector3.left, disToWall + 0.1f))
                        {
                            stopMomentum = true;
                            break;
                        }
                    }
                }

                //if ((Physics.Raycast(new Vector3(transform.position.x, transform.position.y - 1, transform.position.z), Vector3.right, disToWall + 0.1f) || Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Vector3.right, disToWall + 0.1f)) && isRight)
                if (stopMomentum)
                {
                    rb.velocity = new Vector3(0, rb.velocity.y, 0);
                    stopMomentum = false;
                }
                //if ((Physics.Raycast(new Vector3(transform.position.x, transform.position.y - 1, transform.position.z), Vector3.left, disToWall + 0.1f) || Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Vector3.left, disToWall + 0.1f)) && isLeft)
                //{
               //     rb.velocity = new Vector3(0, rb.velocity.y, 0);
               // }
            }
        }
        
    }

    void FixedUpdate()
    {
        //Ground Timer
        if (isGrounded)
        {
            groundTimer++;
        } else
        {
            groundTimer = 0;
        }


        //Walk Speed Increase
        if (curwalkSpeed < walkSpeed && walking)
        {
            curwalkSpeed += 0.5f;
        }

        //Gravity
        if (!isGrounded && rb.velocity.y > maxGravity)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - 2, 0);
        } else if (!isGrounded && rb.velocity.y <= maxGravity)
        {
            rb.velocity = new Vector3(rb.velocity.x, maxGravity, 0);
        } else if (isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, 0);
        }


        //FastFall
        if (!isGrounded && Input.GetAxis("Vertical") < -0.4)
        {
            rb.velocity = new Vector3(rb.velocity.x, ffSpeed, 0);
        }

        //Friction
        if (rb.velocity.x > friction)
            rb.velocity = new Vector3(rb.velocity.x-friction, rb.velocity.y, 0);
        else if (rb.velocity.x < -friction)
            rb.velocity = new Vector3(rb.velocity.x+friction, rb.velocity.y, 0);
        else
            rb.velocity = new Vector3(0, rb.velocity.y, 0);


        //Reduce Air Sprint Speed
        if (isSprinting && !isGrounded && airSpeed > walkSpeed)
        {
            airSpeed -= 0.25f;
        }

        //Jumping Height
        if (isJumping)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpSpeed, 0);

            jumpSpeed--;
            if (jumpSpeed <= 0) // how soon you can double jump after jumping once
            {
                isJumping = false;
            }
        }
        //Reset Double Jump
        if (isGrounded && !isJumping)
            jumpCount = 0;
    }
}
