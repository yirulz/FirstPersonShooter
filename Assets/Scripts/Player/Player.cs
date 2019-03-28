using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour, IKillable
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

    public int maxJumps = 2;
    private int jumps = 0;

    void DrawRay(Ray ray, float distance)
    {
        Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * distance);

    }
    void OnDrawGizmosSelected()
    {
        Ray interactRay = attachedCamera.ViewportPointToRay(new Vector2(0.5f, 0.5f));
        Gizmos.color = Color.blue;
        DrawRay(interactRay, interactRange);

        Gizmos.DrawRay(interactRay);

        Gizmos.color = Color.red;
        Ray groundRay = new Ray(transform.position, -transform.up);
        DrawRay(groundRay, groundRayDistance);
    }
    [Header("UI")]
    public GameObject interactUIPrefab; // Prefab of text to show up when interacting
    public Transform interactUIParent; // Transform (panel) to attach it to on start

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

    //UI
    private GameObject interactUI; //Store the instantiated UI prefab
    private TextMeshProUGUI interactText; // Get component from copy of prefab

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        CreateUI();
        RegisterWeapons();
    }
    // Use this for initialization
    void Start()
    {
        SelectWeapon(0);
    }

    #region Initialization
    void CreateUI()
    {
        interactUI = Instantiate(interactUIPrefab, interactUIParent);
        interactText = interactUI.GetComponentInChildren<TextMeshProUGUI>();
    }

    void RegisterWeapons()
    {
        weapons = new List<Weapons>(GetComponentsInChildren<Weapons>());
        foreach (Weapons weapon in weapons)
        {
            AttachWeapon(weapon);
        }
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

    void AttachWeapon(Weapons weaponToAttach)
    {
        //Call pickup on weapon
        weaponToAttach.Pickup(); 
        //Get transform
        Transform WeaponTransform = weaponToAttach.transform;
        //Attach weapon to hand
        WeaponTransform.SetParent(hand);

        WeaponTransform.localRotation = Quaternion.identity;
        WeaponTransform.localPosition = Vector3.zero;
    }
    #region Combat
    /// <summary>
    /// Changes weapon with given direction
    /// </summary>
    /// <param name="directoin">-1 to 1 number for list selection</param>
    void SwitchWeapon(int direction)
    {
        // Offset weapon index with direction
        currentWeaponIndex += direction;
        // Check if index is below zero
        if (currentWeaponIndex < 0)
        {
            // Loop back to end
            currentWeaponIndex = weapons.Count - 1;
        }
        // Check if index is exceeding length
        if (currentWeaponIndex >= weapons.Count)
        {
            // Reset back to zero
            currentWeaponIndex = 0;
        }
        // Select weapon
        SelectWeapon(currentWeaponIndex);

    }
    /// <summary>
    /// Disables GameObjects of every attached weapon
    /// </summary>
    void DisableAllWeapons()
    {
        // Loop through all weapons
        foreach (var item in weapons)
        {
            // Deactivate it!
            item.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// Adds weapon to list and attaches to player's hand
    /// </summary>
    /// <param name="weaponToPickup">Weapon to place in hand</param>
    void Pickup(Weapons weaponToPickup)
    {
        AttachWeapon(weaponToPickup);
        //Add to weapon list
        weapons.Add(weaponToPickup);
        //Select new weapon
        SelectWeapon(weapons.Count - 1);
    }
    /// <summary>
    /// Removes weapon from list and removes from player's hand
    /// </summary>
    /// <param name="weaponToDrop"></param>
    void Drop(Weapons weaponToDrop)
    {
        //Drop weapon
        weaponToDrop.Drop();
        //Get the transform
        Transform weapsonTransform = weaponToDrop.transform;
        //Remove weapon from list
        weapons.Remove(weaponToDrop);
    }
    /// <summary>
    /// Sets currentWeapon to weapon at given index
    /// </summary>
    /// <param name="index"></param>
    void SelectWeapon(int index)
    {
        //Is index in range?
        if(index >= 0 && index < weapons.Count)
        {
            //Disable all weapons
            DisableAllWeapons();
        }
        //Select weapon
        currerntWeapon = weapons[index];
        //Enable current weapon using index
        currerntWeapon.gameObject.SetActive(true);
        //Update current index
        currentWeaponIndex = index;
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

        bool isGrounded = Physics.Raycast(groundRay, out hit, groundRayDistance);
        bool isJumping = Input.GetButtonDown("Jump");
        bool canJump = jumps < maxJumps;

        if(isGrounded)
        {
            //If jump is pressed
            if (isJumping)
            {
                jumps = 1;
                //Move controller up
                movement.y = jumpHeight;
            }
        }
        else
        {
            if(isJumping && canJump)
            {
                movement.y = jumpHeight;
                jumps++;
            }
        }
        //Apply gravity
        movement.y -= gravity * Time.deltaTime;
        //Limit the gravity
        movement.y = Mathf.Max(movement.y, -gravity);
        //Move the controller
        controller.Move(movement * Time.deltaTime);
    }
    /// <summary>
    /// Interact with items
    /// </summary>
    void Interact()
    {
        //Disable interact UI
        interactUI.SetActive(false);
        //Create ray from center of screen
        Ray interactRay = attachedCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f));
        RaycastHit hit;  

        if(Physics.Raycast(interactRay, out hit, interactRange))
        {
            Interactable interact = hit.collider.GetComponent<Interactable>();
            if(interact != null)
            {
                //Enable UI
                interactUI.SetActive(true);
                //Change the text to item's title
                interactText.text = interact.GetTitle();

                if (Input.GetKeyDown(KeyCode.E))
                {
                    //Check if the thing we hit is a weapson
                    Weapons weapon = hit.collider.GetComponent<Weapons>();
                    if (weapon)
                    {
                        //pick up the weapon
                        Pickup(weapon);

                    }
                }
            }
        }
    }
    /// <summary>
    /// Shooting the guns in your hand
    /// </summary>
    void Shooting()
    {
        //Is a current weapon selected
        if(currerntWeapon)
        {
            //If fire button is pressed
            if(Input.GetButton("Fire1"))
            {
                //Shoot with current weapon
                currerntWeapon.Shoot();
            }
        }
    }
    /// <summary>
    /// Switching between weapons 
    /// </summary>
    void Switching()
    {
        //If there is more than one weapon
        if(weapons.Count > 1)
        {
            float inputScroll = Input.GetAxis("Mouse ScrollWheel");
            //If scroll input has been made
            if(inputScroll != 0)
            {
                int direction = inputScroll > 0 ? Mathf.CeilToInt(inputScroll) : Mathf.FloorToInt(inputScroll);
                //Switch weapons up or down
                SwitchWeapon(direction);
            }


        }

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

    public void TakeDamage(int damage)
    {
        throw new System.NotImplementedException();
    }

    public void Kill()
    {
        throw new System.NotImplementedException();
    }
}
