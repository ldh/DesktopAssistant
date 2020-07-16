using UnityEngine;
using System;
using System.Text.RegularExpressions;
using live2d.framework;
using live2d;


public class LAppModel :L2DBaseModel
{
    private ModelProxy parent;
    private LAppView view;
    
    private string modelHomeDir;
    private ModelSetting modelSetting = null;

    private Matrix4x4 matrixHitArea;
    
    private AudioSource asVoice;

    System.Random rand = new System.Random();

    private Bounds bounds;




    public LAppModel(ModelProxy p)
    {
        if (isInitialized()) return;
        parent = p;
        if (parent.GetComponent<AudioSource>() != null)
        {
			asVoice = parent.gameObject.GetComponent<AudioSource>();
            asVoice.playOnAwake = false;
        }
        else
        {
            if (LAppDefine.DEBUG_LOG)
            {
                Debug.Log("Live2D : AudioSource Component is NULL !");
            }
        }

        bounds = parent.GetComponent<MeshFilter>().sharedMesh.bounds;
        
        
        view = new LAppView(this, parent.transform, GameManager.Instance.Camera);
        view.StartAccel();


        //if (LAppDefine.DEBUG_LOG) mainMotionManager.setMotionDebugMode(true);
    }


    public void LoadFromStreamingAssets(String dir,String filename)
    {
        if (LAppDefine.DEBUG_LOG) Debug.Log(dir + filename);
        modelHomeDir = dir;
		var data = Live2DFramework.getPlatformManager().loadString(modelHomeDir + filename);
        Init(data);
    }


    /*
     * 初始化模型
     * @throws Exception
     */
    public void Init(String modelJson)
    {
        updating = true;
        initialized = false;

        modelSetting = new ModelSettingJson(modelJson);

        if (LAppDefine.DEBUG_LOG) Debug.Log("Start to load model");

        // Live2D Model
        if (modelSetting.GetModelFile() != null)
        {
            loadModelData(modelHomeDir + modelSetting.GetModelFile());

            var len = modelSetting.GetTextureNum();
            Debug.Log(len);
            for (int i = 0; i < len; i++)
            {
                loadTexture(i, modelHomeDir + modelSetting.GetTextureFile(i));
            }
        }
       
        // Expression
        if (modelSetting.GetExpressionNum() != 0)
        {
            var len = modelSetting.GetExpressionNum();
            for (int i = 0; i < len; i++)
            {
				loadExpression(modelSetting.GetExpressionName(i), modelHomeDir + modelSetting.GetExpressionFile(i));
            }
        }

        // Physics
        if (modelSetting.GetPhysicsFile()!=null)
        {
            loadPhysics(modelHomeDir + modelSetting.GetPhysicsFile());            
        }

        // Pose
        if (modelSetting.GetPoseFile()!=null)
        {
            loadPose(modelHomeDir + modelSetting.GetPoseFile());    
        }

        // 由于布局可以在Unity上设置，因此不会从JSON中读取
        //Dictionary<string, float> layout = new Dictionary<string, float>();
        //if (modelSetting.GetLayout(layout))
        //{
        //    if (layout.ContainsKey("width")) modelMatrix.setWidth(layout["width"]);
        //    if (layout.ContainsKey("height")) modelMatrix.setHeight(layout["height"]);
        //    if (layout.ContainsKey("x")) modelMatrix.setX(layout["x"]);
        //    if (layout.ContainsKey("y")) modelMatrix.setY(layout["y"]);
        //    if (layout.ContainsKey("center_x")) modelMatrix.centerX(layout["center_x"]);
        //    if (layout.ContainsKey("center_y")) modelMatrix.centerY(layout["center_y"]);
        //    if (layout.ContainsKey("top")) modelMatrix.top(layout["top"]);
        //    if (layout.ContainsKey("bottom")) modelMatrix.bottom(layout["bottom"]);
        //    if (layout.ContainsKey("left")) modelMatrix.left(layout["left"]);
        //    if (layout.ContainsKey("right")) modelMatrix.right(layout["right"]);
        //}


        // 初始参数
        for (int i = 0; i < modelSetting.GetInitParamNum(); i++)
        {
            string id = modelSetting.GetInitParamID(i);
            float value = modelSetting.GetInitParamValue(i);
            live2DModel.setParamFloat(id, value);
        }

        for (int i = 0; i < modelSetting.GetInitPartsVisibleNum(); i++)
        {
            string id = modelSetting.GetInitPartsVisibleID(i);
            float value = modelSetting.GetInitPartsVisibleValue(i);
            live2DModel.setPartsOpacity(id, value);
        }

        // 自动捕眼器
        eyeBlink = new L2DEyeBlink();

        view.SetupView(
            live2DModel.getCanvasWidth(),
            live2DModel.getCanvasHeight());

        updating = false;// 更新状態の完了
        initialized = true;// 初期化完了
    }


    /*
     * 更新
     */
    public void Update()
    {
        if ( ! isInitialized() || isUpdating())
        {
            return;
        }


        view.Update(Input.acceleration);
        if (live2DModel == null)
        {
            if (LAppDefine.DEBUG_LOG) Debug.Log("Can not update there is no model data");
            return;
        }

        if (!Application.isPlaying)
        {
            live2DModel.update();
            return;
        }

        long timeMSec = UtSystem.getUserTimeMSec() - startTimeMSec;
        double timeSec = timeMSec / 1000.0;
        double t = timeSec * 2 * Math.PI;// 2πt

        // 待机动作确定
        if (mainMotionManager.isFinished())
        {
            // 没有动作播放时，从待机动作中随机播放
            StartRandomMotion(LAppDefine.MOTION_GROUP_IDLE, LAppDefine.PRIORITY_IDLE);
        }
        //-----------------------------------------------------------------
        live2DModel.loadParam();// 加载以前保存的状态

        bool update = mainMotionManager.updateParam(live2DModel);// 更新动作

        if (!update)
        {
            // 当主要动作未更新时
            eyeBlink.updateParam(live2DModel);// 眼睛抓住
        }

        live2DModel.saveParam();// 保存状态
        //-----------------------------------------------------------------

        if (expressionManager != null) expressionManager.updateParam(live2DModel);//  带表达式的参数更新（相对更改）


        // ドラッグによる変化
        // ドラッグによる顔の向きの調整
        live2DModel.addToParamFloat(L2DStandardID.PARAM_ANGLE_X, dragX * 30, 1);// -30から30の値を加える
        live2DModel.addToParamFloat(L2DStandardID.PARAM_ANGLE_Y, dragY * 30, 1);
        live2DModel.addToParamFloat(L2DStandardID.PARAM_ANGLE_Z, (dragX * dragY) * -30, 1);

        // ドラッグによる体の向きの調整
        live2DModel.addToParamFloat(L2DStandardID.PARAM_BODY_ANGLE_X, dragX, 10);// -10から10の値を加える

        // ドラッグによる目の向きの調整
        live2DModel.addToParamFloat(L2DStandardID.PARAM_EYE_BALL_X, dragX, 1);// -1から1の値を加える
        live2DModel.addToParamFloat(L2DStandardID.PARAM_EYE_BALL_Y, dragY, 1);

        // 呼吸など
        live2DModel.addToParamFloat(L2DStandardID.PARAM_ANGLE_X, (float)(15 * Math.Sin(t / 6.5345)), 0.5f);
        live2DModel.addToParamFloat(L2DStandardID.PARAM_ANGLE_Y, (float)(8 * Math.Sin(t / 3.5345)), 0.5f);
        live2DModel.addToParamFloat(L2DStandardID.PARAM_ANGLE_Z, (float)(10 * Math.Sin(t / 5.5345)), 0.5f);
        live2DModel.addToParamFloat(L2DStandardID.PARAM_BODY_ANGLE_X, (float)(4 * Math.Sin(t / 15.5345)), 0.5f);
        live2DModel.setParamFloat(L2DStandardID.PARAM_BREATH, (float)(0.5f + 0.5f * Math.Sin(t / 3.2345)), 1);


        // 加速度による変化
        live2DModel.addToParamFloat(L2DStandardID.PARAM_ANGLE_X, 90 * accelX, 0.5f);
        live2DModel.addToParamFloat(L2DStandardID.PARAM_ANGLE_Z, 10 * accelX, 0.5f);


        if (physics != null) physics.updateParam(live2DModel);// 物理演算でパラメータ更新

        // リップシンクの設定
        if (lipSync)
        {
            live2DModel.setParamFloat(L2DStandardID.PARAM_MOUTH_OPEN_Y, lipSyncValue, 0.8f);
        }

        // ポーズの設定
        if (pose != null) pose.updateParam(live2DModel);

        live2DModel.update();
    }


    public void Draw()
    {
        Matrix4x4 planeLocalToWorld = parent.transform.localToWorldMatrix;

        //  modelMatrixによってPlaneの縮尺に合わせたモデルを、Planeの向きに合わせる変換
        Matrix4x4 rotateModelOnToPlane = Matrix4x4.identity;
        rotateModelOnToPlane.SetTRS(Vector3.zero, Quaternion.Euler(90, 0, 0), Vector3.one);

        Matrix4x4 scale2x2ToPlane = Matrix4x4.identity;
        // planeは xz平面を張っている。その向きに２ｘ２平面を回転した上でスケール
        Vector3 scale = new Vector3(bounds.size.x / 2.0f, -1, bounds.size.z / 2.0f);
        scale2x2ToPlane.SetTRS(Vector3.zero, Quaternion.identity, scale);

        //  -1..1 のサイズで描画されるマトリックス
        Matrix4x4 modelMatrix4x4 = Matrix4x4.identity;
        float[] matrix = modelMatrix.getArray();
        for (int i = 0; i < 16; i++)
        {
            modelMatrix4x4[i] = matrix[i];
        }

        Matrix4x4 modelCanvasToWorld = planeLocalToWorld * scale2x2ToPlane * rotateModelOnToPlane * modelMatrix4x4;

        GetLive2DModelUnity().setMatrix(modelCanvasToWorld);

        live2DModel.draw();

		matrixHitArea = modelCanvasToWorld;
    }


    /*
     * デバッグ用当たり判定の表示
     */
    public void DrawHitArea()
    {
        
        int len = modelSetting.GetHitAreasNum();
        for (int i = 0; i < len; i++)
        {
            string drawID = modelSetting.GetHitAreaID(i);
            float left = 0;
            float right = 0;
            float top = 0;
            float bottom = 0;

            if (!getSimpleRect(drawID, out left, out right, out top, out bottom))
            {
                continue;
            }

			HitAreaUtil.DrawRect(matrixHitArea,left, right, top, bottom);
        }
    }


    public void StartRandomMotion(string name, int priority)
    {
        int max = modelSetting.GetMotionNum(name);
        int no = (int)(rand.NextDouble() * max);
        StartMotion(name, no, priority);
    }


    /*
      *开始运动。
      *检查您是否可以播放，如果不能这样做，则无所事事。
      *如果您可以自动播放，请阅读文件并进行播放。
      *如果它有声音，它也会播放。
      *如果您有关于淡入和淡出的信息，请在此处设置。 如果没有初始值。
      */
    public void StartMotion(string group, int no, int priority)
    {
        string motionName = modelSetting.GetMotionFile(group, no);

        if (motionName == null || motionName.Equals(""))
        {
            if (LAppDefine.DEBUG_LOG) Debug.Log("Motion name is invalid");
            return;//
        }

        //将新动作的优先级与当前正在播放的动作，保留动作的优先级进行比较
        //如果可以预约（如果优先级高）则保留播放。
        //
        //在此帧中立即播放保留的新动作，或者如果需要加载声音等。
        //在以下帧开始播放。
        if (priority == LAppDefine.PRIORITY_FORCE)
        {
            mainMotionManager.setReservePriority(priority);
        }
        else if (!mainMotionManager.reserveMotion(priority))
        {
            if (LAppDefine.DEBUG_LOG) { Debug.Log("Do not play because book already playing, or playing a motion already." + motionName); }
            return;
        }

        AMotion motion=null;
        string name = group + "_" + no;

        if (motions.ContainsKey(name))
        {
            motion = motions[name];            
        }
        if (motion==null)
        {
            motion = loadMotion(name, modelHomeDir+motionName);
        }
        if (motion == null)
        {
            Debug.Log("Failed to read the motion."+motionName);
            mainMotionManager.setReservePriority(0);
            return;
        }

        // フェードイン、フェードアウトの設定
        motion.setFadeIn(modelSetting.GetMotionFadeIn(group, no));
        motion.setFadeOut(modelSetting.GetMotionFadeOut(group, no));


        if ((modelSetting.GetMotionSound(group, no)) == null)
        {
            // 音声が無いモーションは即時再生を開始します。
            if (LAppDefine.DEBUG_LOG) Debug.Log("Start motion : " + motionName);
            mainMotionManager.startMotionPrio(motion, priority);
        }
        else
        {
            // 音声があるモーションは音声のロードを待ってから再生を開始します。
            string soundPath = modelSetting.GetMotionSound(group, no);
            soundPath = Regex.Replace(soundPath, ".mp3$", "");// 不要な拡張子を削除

            AudioClip acVoice = FileManager.LoadAssetsSound(modelHomeDir+soundPath);
            if (LAppDefine.DEBUG_LOG) Debug.Log("Start motion : " + motionName + "  voice : " + soundPath);
            StartVoice( acVoice);
            mainMotionManager.startMotionPrio(motion, priority);
        }
    }


    /*
     * 音声とモーションの同時再生
     * @param motion
     * @param acVoice
     * @param priority 優先度。使用しないなら0で良い。
     */
    public void StartVoice( AudioClip acVoice)
    {
        if (asVoice == null)
        {
            Debug.Log("Live2D : AudioSource Component is NULL !");
            return;
        }
        asVoice.clip = acVoice;
		asVoice.loop = false;
#if UNITY_5
		asVoice.spatialBlend = 0;
#else
		asVoice.spatialBlend = 0;
#endif
        asVoice.Play();
    }


    /*
     * 表情を設定する
     * @param motion
     */
    public void SetExpression(string name)
    {
        if (!expressions.ContainsKey(name)) return;// 無効な指定ならなにもしない
        if (LAppDefine.DEBUG_LOG) Debug.Log("Setting expression : " + name);
        AMotion motion = expressions[name];
        expressionManager.startMotion(motion, false);
    }


    /*
     * 表情をランダムに切り替える
     */
    public void SetRandomExpression()
    {
        int no = (int)(rand.NextDouble() * expressions.Count);

        string[] keys = new string[expressions.Count];
        expressions.Keys.CopyTo(keys, 0);

        SetExpression(keys[no]);
    }


    /*
     * 当たり判定との簡易テスト。
     * 指定IDの頂点リストからそれらを含む最大の矩形を計算し、点がそこに含まれるか判定
     *
     * @param id
     * @param testX
     * @param testY
     * @return
     */
    public bool HitTest(string id, float testX, float testY)
    {
        if (modelSetting == null) return false;
        int len = modelSetting.GetHitAreasNum();
        for (int i = 0; i < len; i++)
        {
            if (id.Equals(modelSetting.GetHitAreaName(i)))
            {
                string drawID = modelSetting.GetHitAreaID(i);
                return hitTestSimple(drawID,testX,testY);
            }
        }
        return false;// 存在しない場合はfalse
    }
   

    public Live2DModelUnity GetLive2DModelUnity()
    {
        return (Live2DModelUnity)live2DModel;
    }


    public Bounds GetBounds()
    {
        return bounds;
    }

    
    /*
     * フリックした時のイベント
     *
     * LAppView側でフリックイベントを感知した時に呼ばれ
     * フリック時のモデルの動きを開始します。
     *
     * @param
     * @param
     * @param flickDist
     */
    public void FlickEvent(float x, float y)
    {
        if (LAppDefine.DEBUG_LOG) Debug.Log("flick x:" + x + " y:" + y);

        if (HitTest(LAppDefine.HIT_AREA_HEAD, x, y))
        {
            if (LAppDefine.DEBUG_LOG) Debug.Log("Flick face");
            StartRandomMotion(LAppDefine.MOTION_GROUP_FLICK_HEAD, LAppDefine.PRIORITY_NORMAL);
        }
    }


    /*
     * タップしたときのイベント
     * @param x	タップの座標 x
     * @param y	タップの座標 y
     * @return
     */
    public bool TapEvent(float x, float y)
    {
        if (LAppDefine.DEBUG_LOG) Debug.Log("tapEvent view x:" + x + " y:" + y);

        if (HitTest(LAppDefine.HIT_AREA_HEAD, x, y))
        {
            // 顔をタップしたら表情切り替え
            if (LAppDefine.DEBUG_LOG) Debug.Log("Tapped face");
            SetRandomExpression();
        }
        else if (HitTest(LAppDefine.HIT_AREA_BODY, x, y))
        {
            if (LAppDefine.DEBUG_LOG) Debug.Log("Tapped body");
            StartRandomMotion(LAppDefine.MOTION_GROUP_TAP_BODY, LAppDefine.PRIORITY_NORMAL);
        }
        return true;
    }


    /*
     * シェイクイベント
     *
     * LAppView側でシェイクイベントを感知した時に呼ばれ、
     * シェイク時のモデルの動きを開始します。
     */
    public void ShakeEvent()
    {
        if (LAppDefine.DEBUG_LOG) Debug.Log("Shake Event");

        StartRandomMotion(LAppDefine.MOTION_GROUP_SHAKE, LAppDefine.PRIORITY_FORCE);
    }


    internal void TouchesBegan(Vector3 inputPos)
    {
        view.TouchesBegan(inputPos);
    }

    internal void TouchesMoved(Vector3 inputPos)
    {
        view.TouchesMoved(inputPos);
    }

    internal void TouchesEnded(Vector3 inputPos)
    {
        view.TouchesEnded(inputPos);
    }
}