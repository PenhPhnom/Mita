using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISystem : MonoBehaviour
{
    public static UISystem Instance;

    public GameObject UIRoot;
    public Canvas UIRootCanvas;
    public Camera UICamera;
    private void Awake()
    {
        Instance = this;
    }
}
