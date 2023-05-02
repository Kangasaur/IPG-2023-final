using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] float speed = 4f;
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float jumpSpeed = 8f;
    [SerializeField] float gravity = 15.0f;
    [SerializeField] Vector3 baseScale = new Vector3(1f, 1f, 1f);
    [SerializeField] Camera cam;
    int hp = 5;
    
    [SerializeField] float curSpeed;
    [SerializeField] Vector3 curScale;
    private CharacterController controller;
    private Vector2 moveInput = Vector2.zero;
    private Vector3 movement = Vector3.zero;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift)) curSpeed = runSpeed;
        else curSpeed = speed;
        curScale = baseScale;

        transform.localScale = curScale;
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (moveInput.sqrMagnitude > 1.0f) moveInput.Normalize();

        movement = new Vector3(moveInput.x * curSpeed, movement.y, moveInput.y * curSpeed);

        if (controller.isGrounded)
        {
            movement.y = 0f;
            if (Input.GetButton("Jump"))
            {
                movement.y = jumpSpeed;
            }
        }

        if ((controller.collisionFlags & CollisionFlags.Above) != 0 && movement.y > 0f)
        {
            movement.y = 0f;
        }

        movement.y -= gravity * Time.deltaTime;

        movement = transform.rotation * movement;

        controller.Move(movement * Time.deltaTime);
    }

    public void Hurt()
    {
        hp--;
        if (hp <= 0) Death();
    }
    void Death()
    {
        SceneManager.LoadScene("Death");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Exit"))
        {
            if (QuestManager.instance.enemiesDead) SceneManager.LoadScene("Victory");
            else SceneManager.LoadScene("Coward");
        }
    }
}
