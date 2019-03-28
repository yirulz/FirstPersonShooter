using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IKillable
{

    public int health = 100;
    public int damage = 25;

    public void Kill()
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(int damage)
    {
        //Reduce health by damage
        health = -damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
