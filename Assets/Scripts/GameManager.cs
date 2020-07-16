using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private ModelProxy currentModel;
    public Camera Camera => Camera.main;

    /// <summary>
    /// 鼠标当前覆盖的Collider，你也可以使用3DCollider，修改一下下面的获取方法就行了
    /// </summary>
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
        
        
        //如果你的鼠标在一个Collider上，意味着你需要和程序里的一些东西交互，那此时你就不希望你的程序是课穿透的，反之亦然
        //别忘了给所有你想用鼠标点到的东西添加Collider
        WindowsAPI.SetClickThrough(HitCollider);
    }


    public void ChangeModel()
    {
        //TODO: Complete the function.
    }
    
}
