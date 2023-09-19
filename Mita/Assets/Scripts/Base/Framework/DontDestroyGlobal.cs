using System;
using System.Collections.Generic;
using UnityEngine;


public class DontDestroyGlobal : MonoBehaviour
{
    public List<GameObject> m_Objs;

    public static DontDestroyGlobal Instance { get; private set; }

    public static event Action eDisable;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (m_Objs != null)
        {
            foreach (var go in m_Objs)
            {
                if (go)
                {
                    DontDestroyOnLoad(go);
                }
            }
        }
    }

    private void OnDisable()
    {
        eDisable?.Invoke();
    }
}
