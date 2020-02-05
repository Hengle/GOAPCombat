using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator _animator;
    float rotSpeed = 40;
    [SerializeField]
    float walkSpeed = 5;
    [SerializeField]
    float runSpeed = 10;
    [SerializeField]
    float moveSpeed = 5;
    [SerializeField]
    float camSpeed = 4;
    [SerializeField]
    Transform camT;
    [SerializeField]
    PInventory inv;
    [SerializeField]
    Health health;
    public Rigidbody rb;

    [SerializeField]
    float mineDist = 2.0f;
    [SerializeField]
    float attackDist = 3.0f;
    [SerializeField]
    Transform mineHeight;

    [SerializeField]
    AudioSource FootSteps;
    [SerializeField]
    AudioClip[] FootStepClips;

    [SerializeField]
    AudioSource sfx;
    [SerializeField]
    AudioClip whoosh;

    public bool isGrounded;
    public Vector3 jump;
    public float jumpForce = 10.0f;

    static public bool inMenu; //player is in a menu 

    [SerializeField]
    float cooldown = 1.0f; //cooldown after action
    bool canAction1 = true; // Primary action
    bool canAction2 = true; // Secondary action


    // Start is called before the first frame update
    void Start()
    {
        inMenu = false; //player is not in a menu on start
        _animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        jump = new Vector3(0.0f, 2.0f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (inMenu) // no movement is allowed while in menus
        {
            return;
        }
        
        if (_animator == null) return;
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");
        transform.Rotate(0, Input.GetAxis("Mouse X") * Time.deltaTime * rotSpeed, 0);
        camT.Rotate(Input.GetAxis("Mouse Y") * Time.deltaTime * camSpeed, 0, 0);
        


        if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Walk") &&
            !_animator.GetCurrentAnimatorStateInfo(0).IsName("Run") && 
            !_animator.GetCurrentAnimatorStateInfo(0).IsName("Roll") &&
            !_animator.GetCurrentAnimatorStateInfo(0).IsName("Jump")) //dont allow the animation to move the player
        {
            rb.isKinematic = true;
            FootSteps.Stop(); // Stop Playing Footsteps sfx
            
        }
        else
        {
            Move(x, y);
            rb.isKinematic = false;
            if (!FootSteps.isPlaying)
            {
                FootSteps.clip = FootStepClips[Random.Range(0, FootStepClips.Length - 1)];
                FootSteps.Play();
            }
        }

        FootSteps.volume = (Mathf.Abs(x) + Mathf.Abs(y))/2; //set footstep volume based on movement

        if (Input.GetButtonDown("Pickup"))  // pickup/drink action
        {
            RaycastHit hit;
            if (Physics.Raycast(mineHeight.position, transform.TransformDirection(new Vector3(0, -1, 1)), out hit, mineDist))
            {
                Debug.DrawRay(mineHeight.position, transform.TransformDirection(new Vector3(0, -1, 1)) * hit.distance, Color.yellow);
                Debug.Log(hit.collider.tag);
                if (hit.collider.tag == "Water") // drink the water
                {
                    _animator.SetTrigger("Gather");
                    health.hydration += 5;
                    if (health.hydration > 100)
                    {
                        health.hydration = 100;
                    }
                    health.update_Txt();
                }
            }
        }

        if (Input.GetButtonDown("Roll"))  // roll action
        {
            _animator.SetTrigger("Roll");
        }

        if (Input.GetButtonDown("Jump"))  // Jump action
        {
            _animator.SetTrigger("Jump");
            rb.AddForce(jump * jumpForce, ForceMode.Impulse);
        }

        if (Input.GetButton("Sprint")) // check if running or walking
        {
            _animator.SetBool("Sprint", true);
            FootSteps.pitch = 1;
            moveSpeed = runSpeed;
        }
        else
        {
            _animator.SetBool("Sprint", false);
            FootSteps.pitch = .75f;
            moveSpeed = walkSpeed;
        }

        if (Input.GetButtonDown("Fire1") && canAction1) // attack/action - Primary Hand (Right)
        {
            StartCoroutine(CoolDown(canAction1,cooldown));
            Debug.Log("Fire1");
            _animator.SetBool("Primary", true); 
            if (health.Stamina(5)) //uses stamina
            {
                PlayAnim('R'); //play animation & sfx
                RaycastHit hit;
                if (Physics.Raycast(mineHeight.position, transform.TransformDirection(Vector3.forward), out hit, attackDist))
                {
                    Debug.DrawRay(mineHeight.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                    Debug.Log(hit.collider.tag);
                    if (hit.collider.tag == "Enemy" && inv.handR.handSlot.attack > 0)
                    {
                        hit.collider.gameObject.GetComponent<EnemyHealth>().hit(inv.handR.handSlot.attack);
                    }
                    else if (hit.collider.tag == "Mineable" && inv.handR.handSlot.canMine)
                    {
                        StartCoroutine(hit.collider.gameObject.GetComponent<mineable>().hit(hit.point));
                    }
                }
            }
            
        }else if (Input.GetButtonDown("Fire2") && canAction2) // attack/action - Secondary Hand (Left)
        {
            StartCoroutine(CoolDown(canAction2, cooldown));
            Debug.Log("Fire2");
            _animator.SetBool("Primary", false);
            if (health.Stamina(5)) //uses stamina
            {
                PlayAnim('L'); //play animation & sfx
                RaycastHit hit;
                if (Physics.Raycast(mineHeight.position, transform.TransformDirection(Vector3.forward), out hit, attackDist))
                {
                    Debug.DrawRay(mineHeight.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                    Debug.Log(hit.collider.tag);
                    if (hit.collider.tag == "Enemy" && inv.handL.handSlot.attack > 0)
                    {
                        hit.collider.gameObject.GetComponent<EnemyHealth>().hit(inv.handL.handSlot.attack);
                    }
                    else if (hit.collider.tag == "Mineable" && inv.handL.handSlot.canMine)
                    {
                        StartCoroutine(hit.collider.gameObject.GetComponent<mineable>().hit(hit.point));
                    }
                }
            }

        }

    }

    IEnumerator CoolDown(bool action, float n)
    {
        action = false;
        yield return new WaitForSeconds(n);
        action = true;
    }

    void OnCollisionStay() // check if grounded
    {
        isGrounded = true;
    }

    void PlayAnim(char LR) // play an animation based on the id of the item in the specifide handslot
    {
        if (LR == 'R') //play anim based on item in handR
        {
            int n = inv.handR.handSlot.randAnim;
            if (n > 0)
            {
                _animator.SetFloat("Rand", (float)Random.Range(0, n)); // play random animation from subset
            }
            sfx.clip = inv.handR.handSlot.sfx;
            sfx.Play();
            _animator.SetTrigger(inv.handR.handSlot.animTrigger);
        }else if (LR == 'L') //play anim based on item in handR
        {
            int n = inv.handL.handSlot.randAnim;
            if (n > 0)
            {
                _animator.SetFloat("Rand", (float)Random.Range(0, n)); // play random animation from subset
            }
            sfx.clip = inv.handL.handSlot.sfx;
            sfx.Play();
            _animator.SetTrigger(inv.handL.handSlot.animTrigger);
        }
    }

    private void Move(float x, float y)
    {
        _animator.SetFloat("x", x);
        _animator.SetFloat("y", y);

        rb.MovePosition(transform.position + transform.right * moveSpeed * x * Time.deltaTime + transform.forward * moveSpeed * y * Time.deltaTime);
        //rb.MovePosition(transform.position + transform.forward * moveSpeed * y * Time.deltaTime);
    }
}
