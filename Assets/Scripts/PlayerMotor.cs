using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController _controller;
    private float _jumpForce = 4.0f;
    private float _gravity = 12.0f;
    private float _verticalVelocity;
    private int _desiredLane = 1; //0 = Left, 1 = Middle, 2 = Right
    private const float LANE_DISTANCE = 2.5f;
    private const float TURN_SPEED = 0.05f;
    
    //Speed Modifier
    private float _originalSpeed = 7.0f;
    private float _speed;
    private float _speedIncreaseLastTick;
    private float _speedIncreaseTime = 2.5f;
    private float _speedIncreaseAmount = 0.1f;

    private bool _isRunning = false;

    //Animation
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _speed = _originalSpeed;
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isRunning)
            return;

        if (Time.time - _speedIncreaseLastTick > _speedIncreaseTime)
        {
            _speedIncreaseLastTick = Time.time;
            _speed += _speedIncreaseAmount;
            //Change Modifier speed
            GameManager.Instance.UpdateModifier(_speed - _originalSpeed);
        }
        
        //Gather the inputs on where we should be
        if (MobileInput.Instance.SwipeLeft)
        {
            //Move Left
            MoveLane(false);
        }

        if (MobileInput.Instance.SwipeRight)
        {
            //Move Right
            MoveLane(true);
        }

        //Calculate Where we should be
        Vector3 targetPosition = transform.position.z * Vector3.forward;
        if (_desiredLane == 0)
            targetPosition += Vector3.left * LANE_DISTANCE;
        else if (_desiredLane == 2)
            targetPosition += Vector3.right * LANE_DISTANCE;

        //Calculate Move delta
        Vector3 moveVector = Vector3.zero;
        moveVector.x = (targetPosition - transform.position).normalized.x * _speed;

        bool isGrounded = IsGrounded();
        _animator.SetBool("Grounded", isGrounded);
        //Calculate Y
        if (isGrounded) // If Grounded
        {
            _verticalVelocity = -0.1f;
            if (MobileInput.Instance.SwipeUp)
            {
                //Jump
                _animator.SetTrigger("Jump");
                _verticalVelocity = _jumpForce;
            }
            else if (MobileInput.Instance.SwipeDown)
            {
                //Slide
                StartSliding();
                Invoke("StopSliding", 1.0f);
            }
        }
        else
        {
            _verticalVelocity -= (_gravity * Time.deltaTime);

            //Fast Falling mechanics
            if (MobileInput.Instance.SwipeDown)
            {
                _verticalVelocity = -_jumpForce;
            }
        }

        moveVector.y = _verticalVelocity;
        moveVector.z = _speed;

        //Move Pengu
        _controller.Move(moveVector * Time.deltaTime);

        //Rotate Pengu slightly to where is going
        Vector3 dir = _controller.velocity;
        if (dir != Vector3.zero)
        {
            dir.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, dir, TURN_SPEED);
        }
    }

    private void StartSliding()
    {
        _animator.SetBool("Sliding", true);
        _controller.height /= 2;
        _controller.center = new Vector3(_controller.center.x, 
            _controller.center.y / 2,
                _controller.center.z);
    }

    private void StopSliding()
    {
        _animator.SetBool("Sliding", false);
        _controller.height *= 2;
        _controller.center = new Vector3(_controller.center.x, 
            _controller.center.y * 2,
            _controller.center.z);
    }

    private void MoveLane(bool goingRight)
    {
        _desiredLane += goingRight ? 1 : -1;
        Debug.Log("The DESIRED LANE => " + _desiredLane);
        _desiredLane = Mathf.Clamp(_desiredLane, 0, 2);
    }

    private bool IsGrounded()
    {
        Ray groundRay = new Ray(
            new Vector3(_controller.bounds.center.x,
                (_controller.bounds.center.y - _controller.bounds.extents.y) + 0.2f,
                _controller.bounds.center.z), Vector3.down);
        Debug.DrawRay(groundRay.origin, groundRay.direction, Color.cyan, 1.0f);

        return Physics.Raycast(groundRay, 0.2f + 0.1f);
    }

    public void StartRunning()
    {
        _isRunning = true;
        _animator.SetTrigger("StartRunning");
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        switch (hit.gameObject.tag)
        {
            case "Obstacle":
                Crash();
                break;
        }
    }

    private void Crash()
    {
        _animator.SetTrigger("Death");
        _isRunning = false;
        GameManager.Instance.OnDeath();
    }
}