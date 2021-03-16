using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    bool hasTransformedCameraStart = false;

    [SerializeField]
    PlayerScript playerScript;

    [SerializeField]
    KeyBindScript keybinds;

    public GameManager gm;
    public Rigidbody2D rb;
    public Animator animator;

    public GameObject escapeDisabled;

    public bool teleportActive = false;
    bool invulnerable = false;
    //bool shootingActive = false;
    public bool noMovement = false;

    public bool isMoving = false;
    public bool safeZone;
    public bool safeZoneMessageActive;
    public bool mainMenu;

    
    public float moveSpeed = 3.5f;
    public Vector3 moveDir;
    public Vector3 lastMoveDir;

    public GameObject mainMenuObject;

     

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        lastMoveDir = new Vector3(1, 0).normalized;



    }



    void Start()
    {


        //GameObject.Find("MainCamera").transform.position = new Vector3(-1.69f, -27.2f, -8);
     // Camera.main.GetComponent<CameraController>().target = transform;



        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        animator.SetFloat("lastMoveX", 1f);
        animator.SetFloat("lastMoveY", 0);
        safeZone = true;


    }

    void Update()
    {
        if (gm.gameOver || gm.handsOff)
        {
            return;
        }

        if (mainMenuObject.activeInHierarchy)
        {
            mainMenu = true;
        }

        else if(mainMenuObject.activeInHierarchy == false)
        {
            mainMenu = false;
        }

      

         if (Input.GetKeyDown(KeyCode.Escape) && mainMenu == false)
        {
            gm.EscapeScreen();
        }

        else if (Input.GetKeyDown(KeyCode.Escape) && safeZone == false && safeZoneMessageActive == false)
        {
            StartCoroutine(nameof(EscapeDisabled));
        }

        {
            float moveX = 0f;
            float moveY = 0f;

            // if shooting then no movement allowed
            if (Input.GetKey(KeyCode.O) || Input.GetMouseButtonDown(1))
            {
               // playerScript.cannotMove = true;
                StartCoroutine(nameof(CannotMoveShooting));
            }

            // if stopped shooting then can move
            if (Input.GetKeyUp(KeyCode.O) || Input.GetMouseButtonUp(1))
            {
               // playerScript.cannotMove = false;
            }

            // can only move if not teleporting, not shooting, or not holding down shoot key
            //if (playerScript.teleportScript.teleportActive == false && shootingActive == false 
            //&& noMovement == false && 
            //if(playerScript.cannotMove == false)
           // {
                if (Input.GetKey(keybinds.keys["Up"]))
                {
                    moveY = +1f;
                    isMoving = true;
                }

                if (Input.GetKey(keybinds.keys["Down"]))
                {
                    moveY = -1f;
                    isMoving = true;
                }

                if (Input.GetKey(keybinds.keys["Left"]))
                {
                    moveX = -1f;
                    isMoving = true;
                }

                if (Input.GetKey(keybinds.keys["Right"]))
                {
                    moveX = +1f;
                    isMoving = true;
                }
           //}

            if (Input.GetKeyUp(keybinds.keys["Up"]) && Input.GetKeyUp(keybinds.keys["Down"]) && Input.GetKeyUp(keybinds.keys["Left"]) && Input.GetKeyUp(keybinds.keys["Right"]))
            {
                isMoving = false;
            }



            // setting the move direction that the animator will use
            moveDir = new Vector3(moveX, moveY).normalized;
            if (moveX != 0 || moveY != 0)
            {
                //not idle
                lastMoveDir = moveDir;
            }

            // necessary for animation, animating in direction walking
            animator.SetFloat("moveX", rb.velocity.x);
            animator.SetFloat("moveY", rb.velocity.y);

            //set the player to keep looking in last moved direction when idling
           
            if(moveX != 0 || moveY !=0)
            {
                animator.SetFloat("lastMoveX", moveX);
                animator.SetFloat("lastMoveY", moveY);
                
            }
        }

        
    }

   private void FixedUpdate()
    {
        if (gm.gameOver || gm.handsOff)
        {
            return;
        }



        // the players actual movement
        if (playerScript.cannotMove == false)
        {


            rb.velocity = moveDir * moveSpeed;
        }

        else if (playerScript.cannotMove == true)
        {
            rb.velocity = new Vector2(0, 0);
        }

      
    }

  

    public IEnumerator EscapeDisabled()
    {
        safeZoneMessageActive = true;
        escapeDisabled.SetActive(true);
        yield return new WaitForSeconds(5f);
        escapeDisabled.SetActive(false);
        safeZoneMessageActive = false;

    }

    public IEnumerator CannotMoveShooting()
    {
        playerScript.cannotMove = true;
        yield return new WaitForSeconds(0.2f);
        if (Input.GetKey(KeyCode.O) == false)
        {
            playerScript.cannotMove = false;
        }
        
    }

}