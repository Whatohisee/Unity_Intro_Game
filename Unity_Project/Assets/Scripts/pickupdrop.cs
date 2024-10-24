using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickupdrop : MonoBehaviour
{
    public PlayerController gunScript;
    public Rigidbody Rb;
    public BoxCollider coll;
    public Transform player;
    public Transform weapons;
    public Transform playerCam;

    public float pickUpRange;
    public float dropForwardForce, dropUpwardForce;
    public float drop;
    public float pickUp;

    public bool equipped;
    public static bool slotFull;

    private void Update()
    {
        //Check if player is in range and "E" is pressed
        Vector3 distanceToPlayer = player.position - transform.position;
        if (!equipped && distanceToPlayer.magnitude <= pickUpRange && Input.GetKeyDown(KeyCode.E) && !slotFull) PickUp();

        //Drop if equipped and "Q" is pressed
        if (equipped && Input.GetKeyDown(KeyCode.Q)) Drop();
    }

    private void PickUp()
    {
        equipped = true;
        slotFull = true;

        //Make Rigidbody kinematic and BoxCollider a trigger
        Rb.isKinematic = true;
        coll.isTrigger = true;

        //Enable script
        gunScript.enabled = true;
    }
   
    private void Drop()
    {
        equipped = false;
        slotFull = false;

        //Make Rigidbody not kinematic and BoxCollider normal
        Rb.isKinematic = false;
        coll.isTrigger = false;

        //disable script
        gunScript.enabled = false;
    }
}
