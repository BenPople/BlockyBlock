using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 6f;
    public float jumpForce = 1f;
    public float gravity = -9.81f;
    public float crouchHeight = 0.5f;
    public float reachDistance = 5f;
    public GameObject blockPrefab;

    private CharacterController controller;
    private Camera playerCamera;
    private GameObject highlightedBlock;
    private Color originalBlockColor;


    float velocityY = 0;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMovement();
        HandleInteraction();
    }

    void HandleMovement()
    {
        velocityY += gravity * Time.deltaTime;

        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        input = input.normalized;

        Vector3 movement = Vector3.zero;
        movement += transform.forward * input.z;
        movement += transform.right * input.x;
        movement = movement.normalized * speed;

        bool isGrounded = Physics.Raycast(transform.position, -Vector3.up, controller.height / 2 + 0.1f);
        if (isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                velocityY = jumpForce;
            }
        }

        movement.y = velocityY;
        controller.Move(movement * Time.deltaTime);
    }




    void HandleInteraction()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, reachDistance))
        {
            GameObject hitObject = hit.collider.gameObject;
            print(hitObject);
            if (hitObject != highlightedBlock && hitObject.CompareTag("Block"))
            {
                // Unhighlight previous block
                if (highlightedBlock != null)
                {
                    highlightedBlock.GetComponent<Renderer>().material.color = originalBlockColor;
                }

                // Highlight new block
                highlightedBlock = hitObject;
                originalBlockColor = hitObject.GetComponent<Renderer>().material.color;
                hitObject.GetComponent<Renderer>().material.color = originalBlockColor * 0.8f;
            }
        }
        else
        {
            // Unhighlight block if no block is hit by the raycast
            if (highlightedBlock != null)
            {
                highlightedBlock.GetComponent<Renderer>().material.color = originalBlockColor;
                highlightedBlock = null;
            }
        }

        if (Input.GetMouseButtonDown(0)) // Left click
        {
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, reachDistance))
            {
                if (hit.collider.CompareTag("Block"))
                {
                    Destroy(hit.collider.gameObject); // Destroy block
                }
            }
        }

        if (Input.GetMouseButtonDown(1)) // Right click
        {
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, reachDistance))
            {
                if (hit.collider.CompareTag("Block"))
                {
                    Vector3 blockPosition = hit.collider.transform.position;
                    Vector3 adjacentBlockPosition = blockPosition + hit.normal;

                    adjacentBlockPosition.x = Mathf.RoundToInt(adjacentBlockPosition.x);
                    adjacentBlockPosition.y = Mathf.RoundToInt(adjacentBlockPosition.y);
                    adjacentBlockPosition.z = Mathf.RoundToInt(adjacentBlockPosition.z);

                    Instantiate(blockPrefab, adjacentBlockPosition, Quaternion.identity); // Place block
                }
            }
        }
    }

}
