using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody myRb;
    Camera playerCam;

    Vector2 camRotation;

    public int health = 5;
    public int maxHealth = 10;
    public int healthPickAmt = 5;

    public Transform weaponSlot;
    public GameObject shot;
    public float shotVel = 0;
    public int weaponID = -0;
    public int fireMode = 0;
    public float currentClip = 0;
    public float clipSize = 0;
    public float maxAmmo = 0;
    public float currentAmmo = 0;
    public float reloadAmt = 0;
    public float bulletLifspan = 0;
    public bool canFire = true;


    public bool sprinting = false;
    public bool sprintToggle = false;
    public float sprintmult = 1.5f;
    public float speed = 10f;
    public float jumphight = 5f;

    public float mouseSensitivity = 2.0f;
    public float Xsensitivity = 2.0f;
    public float Ysensitivity = 2.0f;
    public float grounddetection = 1f;

    // Start is called before the first frame update
    void Start()
    {
        myRb = GetComponent<Rigidbody>();
        playerCam = gameObject.transform.GetChild(0).GetComponent<Camera>();
        camRotation = Vector2.zero;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }

    // Update is called once per frame
    void Update()
    {

        camRotation.x += Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        camRotation.y += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        camRotation.y = Mathf.Clamp(camRotation.y, -90, 90);

        playerCam.transform.localRotation = Quaternion.AngleAxis(camRotation.y, Vector3.left);
        transform.localRotation = Quaternion.AngleAxis(camRotation.x, Vector3.up);

        if (Input.GetMouseButton(0) && camFire && currentClip > 0 && weaponID >= 0)
        {
            GameObject s = Instantiate(shot, weaponSlot.position, weaponSlot.rotation);
            s.GetComponent<Rigidbody>().AddForce(playerCam.transform.forward * shotVel);
            Destory(s, bulletlifespan);

            canFire = false;
            currentClip--;
            StartCoroutine("cooldownFire");

        }

        if (Input.GetKeyDown(KeyCode.R))
            reloadClip();

        if (!sprinting && !sprintToggle && Input.GetKey(KeyCode.LeftShift))
            sprinting = true;

        if (sprinting && sprintToggle && (Input.GetAxisRaw("Vertical") > 0) && Input.GetKey(KeyCode.LeftShift))
            sprinting = true;

        Vector3 temp = myRb.velocity;

        temp.x = Input.GetAxisRaw("Horizontal") * speed;
        temp.z = Input.GetAxisRaw("Vertical") * speed;

        if (sprinting)
            temp.z *= sprintmult;

        if (sprinting && !sprintToggle && Input.GetAxisRaw("Vertical") <= 0)
            sprinting = false;

        if (sprinting && !sprintToggle && Input.GetKeyUp(KeyCode.LeftShift))
            sprinting = false;

        if (Input.GetKeyDown(KeyCode.Space) && Physics.Raycast(transform.position, -transform.up, grounddetection))
            temp.y = jumphight;

        myRb.velocity = (transform.forward * temp.z) + (transform.right * temp.x) + (transform.up * temp.y);
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

            switch(other.gameObject.name)
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
             
         
    public void reloudClip()
    {
        if (currentClip >= clipSize)
            return;

        else
        {
            flout reloudCount = clipSize - currentClip;

            if (currentAmmo < reloudCount)
            {
                currentClip += currentAmmo;
                currentAmmo = 0;
                return;
            }

            else
            {
                currentClip += reloudCount;
                currentAmmo -= reloudCount;
                return;
            }
        }
    }
    
    IEnumerator cooldownFire()
    {
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }
    
}
