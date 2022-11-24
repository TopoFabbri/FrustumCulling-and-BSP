using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    [SerializeField] private bool verticalMovement;
    private float speed = 2.0f;
    private float moveSpeed = 200.0f;
    private float decceleration = 0.01f;
    private Vector3 vel;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Rotate();
        Deccelerate();
        Move();
    }

    void Rotate()
    {
        Vector3 rot = transform.eulerAngles;

        transform.eulerAngles = new Vector3(verticalMovement ? (Input.GetAxis("MouseY") * -speed + rot.x) : rot.x,
            Input.GetAxis("MouseX") * speed + rot.y, rot.z);
    }

    void Move()
    {
        Vector3 moveFor = new Vector3();
        Vector3 moveSid = new Vector3();
        Vector3 moveUp = new Vector3();

        moveFor += Input.GetAxis("Walk") * transform.forward;
        moveUp += Input.GetAxis("Fly") * transform.up;
        moveSid += Input.GetAxis("Strafe") * transform.right;

        vel += moveFor * Time.deltaTime + moveSid * Time.deltaTime + moveUp * Time.deltaTime;

        transform.position += vel * Time.deltaTime * moveSpeed;
    }

    void Deccelerate()
    {
        vel *= Time.deltaTime * decceleration;
    }
}