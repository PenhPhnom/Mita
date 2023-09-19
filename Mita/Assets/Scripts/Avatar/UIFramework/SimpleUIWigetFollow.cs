using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleUIWigetFollow : MonoBehaviour
{
    public RectTransform target;

    void OnEnable()
    {
        if (target == null)
        {
            Debug.LogError($"没有跟随对象 [{this}]", gameObject);
        }
    }
    void Update()
    {
        if (target == null)
            return;

        // transform.position = target.position;
        transform.position = Vector3.Lerp(transform.position, target.position, 0.7f);
    }

}