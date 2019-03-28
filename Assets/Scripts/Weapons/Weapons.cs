using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(SphereCollider))]
public class Weapons : MonoBehaviour, Interactable
{
    public int damage = 10;
    public int maxAmmo = 500;
    public int maxClip = 30;
    public float range = 10f;
    public float shootRate = .2f;
    public float lineDelay = .1f;
    public Transform shotOrigin;

    private int ammo = 0;
    private int clip = 0;
    private float shootTimer = 0f;
    private bool canShoot = false;

    private Rigidbody rigid;
    private BoxCollider boxCollider;
    private LineRenderer lineRenderer;
    private SphereCollider sphereCollider;
    void GetReferences()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        lineRenderer = GetComponent<LineRenderer>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    void Reset()
    {
        GetReferences();

        // Collect all bounds inside of children
        Renderer[] children = GetComponentsInChildren<MeshRenderer>();
        Bounds bounds = new Bounds(transform.position, Vector3.zero);
        foreach (Renderer rend in children)
        {
            bounds.Encapsulate(rend.bounds);
        }

        //Apply bounds to box collider
        boxCollider.center = bounds.center - transform.position;
        boxCollider.size = bounds.size;
        //Turn off line renderer
        lineRenderer.enabled = false;
        //Sets rigidbody to kimematic
        rigid.isKinematic = false;
        //Enable Trigger
        sphereCollider.isTrigger = true;
        sphereCollider.center = boxCollider.center;
        sphereCollider.radius = boxCollider.size.magnitude * .5f;
    }
    void Awake()
    {
        GetReferences();
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //increase shoot timer
        shootTimer += Time.deltaTime;
        //If time reaches rate
        if(shootTimer >= shootRate)
        {
            //We can shoot
            canShoot = true;
        }
    }

    public void Pickup()
    {
        //Disable rigidbody
        rigid.isKinematic = true;
        sphereCollider.enabled = false;
    }

    public void Drop()
    {
        //enable rigidbody
        rigid.isKinematic = false;
    }

    IEnumerator ShotLine(Ray bulletRay, float lineDelay)
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, bulletRay.origin);
        lineRenderer.SetPosition(1, bulletRay.origin + bulletRay.direction * range);

        yield return new WaitForSeconds(lineDelay);

        lineRenderer.enabled = false;
    }

    public virtual void Reload()
    {
        clip += ammo;
        ammo -= maxClip;
    }

    public virtual void Shoot()
    {
        //Can shoot
        if(canShoot)
        {
            //create bullet ray
            Ray bulletRay = new Ray(shotOrigin.position, shotOrigin.forward);
            RaycastHit hit;

            if(Physics.Raycast(bulletRay, out hit, range))
            {
                IKillable killable = hit.collider.GetComponent<IKillable>();
                if(killable != null)
                {
                    //deal damage to enemy
                    killable.TakeDamage(damage);
                }
            }

            StartCoroutine(ShotLine(bulletRay, lineDelay));

            shootTimer = 0;
            //Can't shoot anymore
            canShoot = false;
        }
    }

    public virtual string GetTitle()
    {
        return "Weapon";
    }
}
