using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

//把这玩意挂在摄像机上  然后
public class NewBehaviourScript : MonoBehaviour //只有继承这个MonoBehaviour类 才能挂载在组件上
{
    public RenderPipelineAsset currRenderPipelineAsset;
    private void OnEnable()  //运行游戏使用相应管线  此回调函数是组件所在物体被启用（进入播放模式、物体激活）时触发
    {
        GraphicsSettings.renderPipelineAsset = currRenderPipelineAsset;
    }
    
    private void OnValidate() //在编辑场景下修改面板参数、保存脚本时就自动执行
    {
        GraphicsSettings.renderPipelineAsset = currRenderPipelineAsset;
    }
    
}
