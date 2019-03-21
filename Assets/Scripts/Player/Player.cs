using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Mechinics")]
    public int health = 100;
    public float runSpeed = 7.5f;
    public float walkSpeed = 6f;
    public float gravity = 10f;
    public float crouchSpeed = 4f;
    public float jumpHeight = 20f;
    public float interactRange = 10f;
    public float groundRayDistance = 1.1f;

    void OnDrawGizmos()
    {
        Ray groundRay = new Ray(transform.position, -transform.up);
        Gizmos.DrawLine(groundRay.origin, groundRay.direction * groundRayDistance);
    }

    [Header("References")]
    public Camera attachedCamera;
    public Transform hand;

    //animation
    private Animator anim;

    //Movement
    private CharacterController controller;
    private Vector3 movement;

    //Weapons
    public Weapons currerntWeapon;
    private List<Weapons> weapons = new List<Weapons>();
    private int currentWeaponIndex = 0;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }
    // Use this for initialization
    void Start()
    {
        SelectWeapon(0);
    }

    #region Initialization
    void CreateUI()
    {

    }

    void RegisterWeapons()
    {

    }
    #endregion

    #region Controls
    /// <summary>
    /// Moves the character controller in direction of input
    /// </summary>
    /// <param name="inputH">Horizontal Input</param>
    /// <param name="inputV">Vertical Input</param>
    void Move(float inputH, float inputV)
    {
        //Create direction form input
        Vector3 input = new Vector3(inputH, 0, inputV);
        //Localise direction to players transform
        input = transform.TransformDirection(input);
        //Set move speed
        float moveSpeed = walkSpeed;
        //Apply movement
        movement.x = input.x * moveSpeed;
        movement.z = input.z * moveSpeed;
    }
    #endregion

    #region Combat
    /// <summary>
    /// Changes weapon with given direction
    /// </summary>
    /// <param name="directoin">-1 to 1 number for list selection</param>
    void SwitchWeapon(int directoin)
    {

    }
    /// <summary>
    /// Disables GameObjects of every attached weapon
    /// </summary>
    void DisableAllWeapons()
    {

    }
    /// <summary>
    /// Adds weapon to list and attaches to player's hand
    /// </summary>
    /// <param name="weaponToPickup">Weapon to place in hand</param>
    void Pickup(Weapons weaponToPickup)
    {

    }
    /// <summary>
    /// Removes weapon from list and removes from player's hand
    /// </summary>
    /// <param name="weaponToDrop"></param>
    void Drop(Weapons weaponToDrop)
    {

    }
    /// <summary>
    /// Sets currentWeapon to weapon at given index
    /// </summary>
    /// <param name="index"></param>
    void SelectWeapon(int index)
    {

    }
    #endregion

    #region Action
    /// <summary>
    /// Handles movement
    /// </summary>
    void Movement()
    {
        //Get Input from user
        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");
        Move(inputH, inputV);
        //Is the controller grounded
        Ray groundRay = new Ray(transform.position, -transform.up);
        RaycastHit hit;
        if(Physics.Raycast(groundRay, out hit, groundRayDistance))
        {
            //If jump is pressed
            if (Input.GetButtonDown("Jump"))
            {
                //Move controller up
                movement.y = jumpHeight;
            }
        }
        else
        {
            //Limit the gravity
            movement.y = Mathf.Max(movement.y, -gravity);
        }
        
        
        //Apply gravity
        movement.y -= gravity * Time.deltaTime;
       
        //Move the controller
        controller.Move(movement * Time.deltaTime);
    }
    /// <summary>
    /// Interact with items
    /// </summary>
    void Interact()
    {

    }
    /// <summary>
    /// Shooting the guns in your hand
    /// </summary>
    void Shooting()
    {

    }
    /// <summary>
    /// Switching between weapons 
    /// </summary>
    void Switching()
    {

    }
    #endregion 



    // Update is called once per frame
    void Update()
    {
        Movement();
        Interact();
        Shooting();
        Switching();
    }
}
