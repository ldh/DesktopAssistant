using UnityEngine;
using live2d;
using live2d.framework;


/// <summary>
/// 这是Live2D框架的入口类
/// </summary>
[ExecuteInEditMode]
public class ModelProxy : MonoBehaviour
{
    public string path = "";
    private LAppModel model;

    private float lastX = -1;
    private float lastY = -1;

    private int curClothIndex;

    private void Start()
    {
        if (path == "") return;

#if !UNITY_EDITOR
        LAppDefine.DEBUG_LOG = false;
        LAppDefine.DEBUG_TOUCH_LOG = false;
#endif
        model = new LAppModel(this);
        

        Live2D.init();
        Live2DFramework.setPlatformManager(new PlatformManager());

        var filename = FileManager.getFilename(path);
        var dir = FileManager.getDirName(path);
        model.LoadFromStreamingAssets(dir, filename);
        
        // model.loadTexture(2, "haru/haru_01.1024/texture_03.png");
    }


    private void OnRenderObject()
    {
        if (model == null) return;
        
        model.Draw();

        if (LAppDefine.DEBUG_DRAW_HIT_AREA)
        {
            model.DrawHitArea();
        }
    }

    // public void ChangeCloth()
    // {
    //     string paht = curClothIndex % 2 == 0? "haru/haru_01.1024/texture_03.png" : "haru/haru_01.1024/texture_02.png";
    //     curClothIndex ++;
    //     model.loadTexture(2, paht);
    // }


    private void Update()
    {
        if (model == null) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // model.ShakeEvent();
            model.StartRandomMotion(LAppDefine.MOTION_GROUP_PINCH_OUT, LAppDefine.PRIORITY_FORCE);
        }
        
        Vector2 mousePosition = Input.mousePosition;

        model.Update();
        
        if (Input.GetMouseButtonDown(0))
        {
            lastX = mousePosition.x;
            lastY = mousePosition.y;
            TouchesBegan(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            lastX = -1;
            lastY = -1;
            TouchesEnded(Input.mousePosition);
        }

        if (lastX == mousePosition.x && lastY == mousePosition.y) return;

        //
        lastX = Input.mousePosition.x;
        lastY = Input.mousePosition.y;
        TouchesMoved(Input.mousePosition);
    }


    private void TouchesBegan(Vector3 inputPos)
    {
        model.TouchesBegan(inputPos);
    }

    private void TouchesMoved(Vector3 inputPos)
    {
        model.TouchesMoved(inputPos);
    }

    private void TouchesEnded(Vector3 inputPos)
    {
        model.TouchesEnded(inputPos);
    }
}