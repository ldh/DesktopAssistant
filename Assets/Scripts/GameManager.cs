using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Camera Camera => Camera.main;

    public Collider2D HitCollider { get; private set; }

    private void Awake()
    {
        Instance = this;

        Application.runInBackground = true;
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
        WindowsAPI.InitWindow();
#endif
    }

    private void Update()
    {
        
        HitCollider = Physics2D.OverlapPoint(Camera.ScreenToWorldPoint(Input.mousePosition));
        
        WindowsAPI.SetClickThrough(HitCollider);
    }
    
    
}
