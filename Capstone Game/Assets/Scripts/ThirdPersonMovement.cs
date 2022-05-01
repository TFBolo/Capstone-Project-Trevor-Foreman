using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    public LayerMask groundMask;
    public Transform groundCheck;

    public float speed = 40f;
    public float dash = 2.5f;
    public float baseDash = 2.5f;
    public float dashLimit = 5f;
    public float dashCharge = 0.1f;
    public float jumpHeight = 2f;
    public float maxSpeed = 5f;
    public float dashCooldown = 1f;

    float gravity = -19.62f;
    float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    float groundDistance = 0.4f;

    Vector3 gravityVelocity;
    Vector3 velocity; 
    
    bool isGrounded;
    bool dashReady = true;

    public Animator animator;
    private string currentState;

    public InventoryObject inventory;
    public InteractableController UIText;

    private void Start()
    {
        UIText = GameObject.FindGameObjectWithTag("UI").GetComponent<InteractableController>();
        dash = baseDash;
        dashLimit = 2 * baseDash;
        dashCharge = baseDash * 0.7f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0) return;
        
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        gravityVelocity.y += gravity * Time.deltaTime;
        controller.Move(gravityVelocity * Time.deltaTime);

        if (isGrounded && gravityVelocity.y < 0f)
        {
            gravityVelocity.y = -4f;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            //ChangeAnimationState("Jump");
            gravityVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        if (!isGrounded && gravityVelocity.y < 0)
        {
            ChangeAnimationState("Falling Idle");
        }

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            if (isGrounded)
            {
                velocity += (moveDir.normalized * speed * Time.deltaTime);
            }
            else
            {
                velocity += (moveDir.normalized * speed * 0.2f * Time.deltaTime);
            }
        }
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        if (velocity.magnitude > 0.1 && isGrounded)
        {
            ChangeAnimationState("Running");
        }
        else if (velocity.magnitude < 0.1 && isGrounded)
        {
            ChangeAnimationState("Idle");
        }
        controller.Move(velocity * Time.deltaTime);
        /*
        if (Input.GetButtonDown("Dodge") && isGrounded && dashReady)
        {
            dashReady = false;
            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                Vector3 target = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                Dash(transform.position + (target * dash));
            }
            else
            {
                Vector3 target = transform.position + (transform.forward * dash);
                Dash(target);
            }
            StartCoroutine("DashWait");
        }
        */
        if (Input.GetButton("Dodge") /*&& isGrounded*/ && dashReady)
        {
            if (dash < baseDash) dash = baseDash;
            dash += dashCharge * Time.deltaTime;
            if (dash > dashLimit) dash = dashLimit;
        }
        if (Input.GetButtonUp("Dodge") /*&& isGrounded*/ && dashReady)
        {
            dashReady = false;
            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                Vector3 target = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                Dash(transform.position + (target * dash));
            }
            else
            {
                Vector3 target = transform.position + ((transform.forward) * dash);
                Dash(target);
            }
            StartCoroutine("DashWait");
        }
        if (Input.GetButtonDown("Interact"))
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 5f, LayerMask.GetMask("Interact"));
            if (colliders.Length != 0)
            {
                colliders[0].transform.gameObject.GetComponent<ObjectIdentify>().PlayerCall();
            }
            /*
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit interactHit))
            {
                if (interactHit.collider.tag == "Interact" && Vector3.Distance(transform.position, interactHit.collider.transform.position) <= 5f)
                {
                    interactHit.transform.gameObject.GetComponent<ObjectIdentify>().PlayerCall();
                }
            }*/
        }

        if (velocity.magnitude >= 0f && direction.magnitude == 0)
        {
            if (isGrounded)
            {
                velocity -= velocity * 0.3f;
            }
        }
    }

    IEnumerator DashWait()
    {
        yield return new WaitForSeconds(dashCooldown);
        dashReady = true;
    }

    private void Dash(Vector3 dashPosition)
    {
        RaycastHit dashCast;
        Physics.Linecast(transform.position, dashPosition, out dashCast, groundMask);
        if (dashCast.collider != null)
        {
            dashPosition = dashCast.point + (-transform.forward * 0.5f);
        }
        transform.position = dashPosition;
        dash = baseDash;
    }

    void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        animator.Play(newState);

        currentState = newState;
    }

    public void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponent<Item>();
        if (item)
        {
            inventory.AddItem(item.item, item.amount);
            UIText.MoneyAmount(item.amount);
            other.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        inventory.container.Clear();
    }
}
