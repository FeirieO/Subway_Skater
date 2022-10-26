using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileInput : MonoBehaviour
{
    public static MobileInput Instance { set; get; }
    private bool _tap, _swipeLeft, _swipeRight, _swipeUp, _swipeDown;
    private Vector2 _swipeDelta, _startTouch;
    private const float DEADZONE = 100.0f;

    public bool Tap
    {
        get { return _tap; }
    }

    public Vector2 SwipeDelta
    {
        get { return _swipeDelta; }
    }

    public bool SwipeLeft
    {
        get { return _swipeLeft; }
    }

    public bool SwipeRight
    {
        get { return _swipeRight; }
    }

    public bool SwipeUp
    {
        get { return _swipeUp; }
    }

    public bool SwipeDown
    {
        get { return _swipeDown; }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        //Resetting all the booleans
        _tap = _swipeLeft = _swipeRight = _swipeDown = _swipeUp = false;

        //Check for Input

        #region Standalone Inputs

        if (Input.GetMouseButtonDown(0))
        {
            _tap = true;
            _startTouch = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _startTouch = _swipeDelta = Vector2.zero;
        }

        #endregion

        #region Mobile Inputs

        if (Input.touches.Length != 0)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                _tap = true;
                _startTouch = Input.touches[0].position;
            }
            else if (Input.touches[0].phase == TouchPhase.Ended ||
                     Input.touches[0].phase == TouchPhase.Canceled)
            {
                _startTouch = _swipeDelta = Vector2.zero;
            }
        }

        #endregion

        //Calculate distance
        _swipeDelta = Vector2.zero;
        if (_startTouch != Vector2.zero)
        {
            //let's check with mobile
            if (Input.touches.Length != 0)
            {
                _swipeDelta = Input.touches[0].position - _startTouch;
            }
            //Let's check with standalone
            else if (Input.GetMouseButton(0))
            {
                _swipeDelta = (Vector2) Input.mousePosition - _startTouch;
            }
        }

        //Check if we're beyond dead zone
        if (_swipeDelta.magnitude > DEADZONE)
        {
            //This is a confirmed swipe
            float x = _swipeDelta.x;
            float y = _swipeDelta.y;

            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                //Left or Right
                if (x < 0)
                {
                    _swipeLeft = true;
                }
                else
                {
                    _swipeRight = true;
                }
            }
            else
            {
                //Up or Down
                if (y < 0)
                {
                    _swipeDown = true;
                }
                else
                {
                    _swipeUp = true;
                }
            }

            _startTouch = _swipeDelta = Vector2.zero;
        }
    }
}