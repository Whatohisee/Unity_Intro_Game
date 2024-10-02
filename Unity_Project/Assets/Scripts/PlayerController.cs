using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    GameManager gm;


    Rigidbody myRb;
    Camera playerCam;
    Transform camHolder;

    Vector2 camRotation;

    public int health = 5;
    public int maxHealth = 10;
    public int healthPickAmt = 5;

    public Transform weaponSlot;
    public GameObject shot;
    public float shotVel = 0;
    public int weaponID = -1;
    public int fireMode = 0;
    public float fireRate = 0;
    public float currentClip = 0;
    public float clipSize = 0;
    public float maxAmmo = 0;
    public float currentAmmo = 0;
    public float reloadAmt = 0;
    public float bulletLifespan = 0;
    public bool canFire = true;
    public bool takenDamage = false;
    public float damageCooldownTime = .5f;


    public bool sprinting = false;
    public float speed = 10f;
    public float sprintMult = 1.5f;
    public float jumpHeight = 5f;
    public float groundDetection = 1f;

    public bool sprintToggle = false;
    public float mouseSensitivity = 2.0f;
    public float Xsensitivity = 2.0f;
    public float Ysensitivity = 2.0f;
    public float camRotationLimit = 90f;


    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        myRb = GetComponent<Rigidbody>();
        playerCam = Camera.main;
        camHolder = transform.GetChild(0);

        camRotation = Vector2.zero;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }

    // Update is called once per frame
    void Update()
    {
        if (!gm.isPaused)
        {
            playerCam.transform.position = camHolder.position;

            camRotation.x += Input.GetAxisRaw("Mouse X") * mouseSensitivity;
            camRotation.y += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;


            camRotation.y = Mathf.Clamp(camRotation.y, -camRotationLimit, camRotationLimit);


            playerCam.transform.rotation = Quaternion.Euler(-camRotation.y, camRotation.x, 0);
            transform.localRotation = Quaternion.AngleAxis(camRotation.x, Vector3.up);

            if (Input.GetMouseButton(0) && canFire && currentClip > 0 && weaponID >= 0)
            {
                GameObject s = Instantiate(shot, weaponSlot.position, weaponSlot.rotation);
                s.GetComponent<Rigidbody>().AddForce(playerCam.transform.forward * shotVel);
                Destroy(s, bulletLifespan);

                canFire = false;
                currentClip--;
                StartCoroutine("cooldownFire");
            }

            if (Input.GetKeyDown(KeyCode.R))
                reloadClip();



            if ((!sprinting) && ((!sprintToggle && Input.GetKey(KeyCode.LeftShift)) || (sprintToggle && Input.GetKey(KeyCode.LeftShift) && (Input.GetAxisRaw("Vertical") > 0))))
                sprinting = true;



            Vector3 temp = myRb.velocity;

            temp.x = Input.GetAxisRaw("Horizontal") * speed;
            temp.z = Input.GetAxisRaw("Vertical") * speed;

            if (sprinting)
            {
                temp.z *= sprintMult;

                if ((sprintToggle && (Input.GetAxisRaw("Vertical") <= 0)) || (!sprintToggle && Input.GetKeyUp(KeyCode.LeftShift)))
                    sprinting = false;
            }


            if (Input.GetKeyDown(KeyCode.Space) && Physics.Raycast(transform.position, -transform.up, groundDetection))
                temp.y = jumpHeight;

            myRb.velocity = (transform.forward * temp.z) + (transform.right * temp.x) + (transform.up * temp.y);

        }

      
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Pickup" && health < maxHealth)
        {
            if (health + healthPickAmt > maxHealth)
                health = maxHealth;
            else health += healthPickAmt;
            Destroy(collision.gameObject);
        }
        if ((collision.gameObject.tag == "ammoPickup") && currentAmmo < maxAmmo)
        {
            if (currentAmmo + reloadAmt > maxAmmo)
                currentAmmo = maxAmmo;

            else
                currentAmmo += reloadAmt;

            Destroy(collision.gameObject);

        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "weapon")
        {
            other.transform.SetPositionAndRotation(weaponSlot.position, weaponSlot.rotation);

            other.transform.SetParent(weaponSlot);

            switch (other.gameObject.name)
            {
                case "weapon":
                    weaponID = 0;
                    shotVel = 10000;
                    fireMode = 0;
                    fireRate = 0.1f;
                    currentClip = 20;
                    clipSize = 400;
                    maxAmmo = 200;
                    reloadAmt = 20;
                    bulletLifespan = .5f;
                    break;

                default:
                    break;
            }
        }
    }


    public void reloadClip()
    {
        if (currentClip >= clipSize)
            return;

        else
        {
            float reloadCount = clipSize - currentClip;

            if (currentAmmo < reloadCount)
            {
                currentClip += currentAmmo;
                currentAmmo = 0;
                return;
            }

            else
            {
                currentClip += reloadCount;
                currentAmmo -= reloadCount;
                return;
            }
        }
    }

    IEnumerator cooldownFire()
    {
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }

    IEnumerator cooldownDamage()
    {
        yield return new WaitForSeconds(damageCooldownTime);
        takenDamage = false;
    }

}
