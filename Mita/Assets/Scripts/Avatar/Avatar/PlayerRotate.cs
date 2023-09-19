using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    private Vector2 m_TouchDeltaPosition;
    private void Update()
    {
        RotateByTouchMove();
    }

    private void RotateByTouchMove()
    {
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Moved)
        {
            m_TouchDeltaPosition = Input.GetTouch(0).deltaPosition;
            transform.Rotate(0, -m_TouchDeltaPosition.x, 0);
        }
    }

    public void ResetRotate()
    {
        this.transform.localEulerAngles = Vector3.zero;
    }

    private void OnDestroy()
    {
        m_TouchDeltaPosition = new Vector2();
    }
}
