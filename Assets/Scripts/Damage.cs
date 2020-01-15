using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Damage : MonoBehaviour
{
    // Crude Script to deal damage to player unit when he's alive and kill him when his weakness is attacking. 
    PlayerUnit PlayerObject;
    public float dmg = 1f;
    public float tick = 1f;
    public AttackType atkType = AttackType.elemental;

    public void Start()
    {
        PlayerObject = gameObject.GetComponent<PlayerUnit>(); 
    }

    public void Update()
    {
        if (PlayerObject != null)
        {
            Attacking();
        }
    }

    public void Attacking()
    {
        tick -= Time.deltaTime;
        if (tick <= 0)
        {
            tick = 1;
            if (PlayerObject.GetState() == UnitState.alive || PlayerObject.GetState() == UnitState.clinging) // Deals the damage only if the players alive. 
            {
                PlayerObject.TakeDmg(dmg, atkType);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerUnit")
        {
            PlayerObject = other.gameObject.GetComponent<PlayerUnit>();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "PlayerUnit")
        {
            PlayerObject = null;
        }
    }


}
