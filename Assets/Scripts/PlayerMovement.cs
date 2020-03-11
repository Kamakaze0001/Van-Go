﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController controller;

    [SerializeField]
    private float speed = 12f,
    maxSpeed = 12,
    baseSpeed = 8,
    gravity = -25f,
    jumpHeight = 2f,
    acceleration = 0.5f,
    decelleration = 1.0f,
    throwForce = 10f;

	[SerializeField]
	private GameObject pot;
	private bool isHoldingPot = true;

	[SerializeField]
	private GameObject paintGlob;
	[SerializeField]
	private GameObject paintBrush;
	[SerializeField]
	private int maxStrokes = 2;
	private int strokeLeft;

	public Vector3 velocity;
    bool isGrounded;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

	//Lerp Info
	private Vector3 startMarker;
	private Vector3 endMarker;
	private float t;




	// Start is called before the first frame update
	void Start()
    {
		Debug.Assert(pot != null, "No pot attached to player");
		ReAttachPot();

		throwForce = GetComponent<PlayerMovement>().GetThrowForce();
		//paintGlob.SetActive(false);
		strokeLeft = maxStrokes;
	}

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }


        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        

        Vector3 move = ((transform.right * x) + transform.forward * z);

        if(speed < maxSpeed && z > 0)
        {
            speed += acceleration / 50;
        }

        if (z == 0 && speed > baseSpeed)
        {
            speed -= decelleration / 50;
        }


        controller.Move(move * speed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

		if (t < 0.1f) {
			transform.position = Vector3.Lerp(startMarker, endMarker, t * 10.0f);
			t += Time.deltaTime;
		}
		else {

			if (Input.GetButtonDown("ThrowPot"))
			{
				if (isHoldingPot)
				{
					//Throw pot
					isHoldingPot = false;
					pot.transform.parent = null;

					pot.GetComponent<BoxCollider>().isTrigger = false;
					pot.GetComponent<Rigidbody>().isKinematic = false;

					pot.GetComponent<Rigidbody>().AddForce(GetComponentInChildren<Camera>().transform.forward * throwForce);
				}
				else
				{
					//Teleport to pot
					startMarker = transform.position;
					endMarker = pot.transform.position;
					t = 0.0f;
	
					ReAttachPot();
				}
			}
			else if (Input.GetButtonDown("RegrabPot") && !isHoldingPot)
			{
				ReAttachPot();
			}
		}
		if (isHoldingPot) strokeLeft = maxStrokes;

		if (Input.GetButtonDown("Fire1") && !paintGlob.activeSelf && maxStrokes > 0)
		{
			paintGlob.SetActive(true);
			paintGlob.transform.position = paintBrush.transform.position;
			paintGlob.GetComponent<Rigidbody>().velocity = GetComponentInChildren<Camera>().transform.forward * throwForce;
			strokeLeft--;
		}
	}

	private void ReAttachPot()
	{
		Debug.Log("Get pot");
		isHoldingPot = true;
		pot.transform.parent = transform;
		pot.transform.localPosition = new Vector3(0.0f, GetComponent<CharacterController>().height/2.0f, 0.0f);
		pot.GetComponent<BoxCollider>().isTrigger = true;
		pot.GetComponent<Rigidbody>().isKinematic = true;
		//pot.transform.position = theBone.transform.position;
		//pot.transform.parent = theBone.transform;
	}

	public float GetThrowForce()
    {
        return throwForce;
    }
}
