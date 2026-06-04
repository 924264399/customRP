using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json.Schema;
using UnityEngine;
using UnityEngine.Rendering;

namespace LiteRP
{
    public class LiteRenderPipeline : RenderPipeline  //运行时渲染管线实例（内存里的 C# 对象）
    {
        
        private readonly static ShaderTagId s_ShaderTagId = new ShaderTagId("SRPDefaultUnlit");
        
        //老的接口 为了兼容
        protected override void Render(ScriptableRenderContext context, Camera[] cameras)  
        {
            
        }
        
        
        //新的接口
        //虚函数复写 render函数 render函数是RenderPipeline类的 
        //ScriptableRenderContext：渲染指令的 “调度中心”，负责和 GPU 通信；  更类似 跨语言通用画图说明书 
        protected override void Render(ScriptableRenderContext context, List<Camera> cameras)  //每渲染一帧，Unity 自动调用这个实例的 Render 方法  SceneView  和 GameView  每一帧都会调用
        {
            //开始渲染上下文
            BeginContextRendering(context, cameras);  //是让引擎中 RenderPipelineManger 全局事件开关 简单理解为让外部代码能在渲染前后插入自定义逻辑就行了
            
            
            //渲染相机 (遍历) 
            for (int i = 0; i < cameras.Count; i++)
            {
                Camera camera = cameras[i];  //因为有多个相机嘛~~   cameras列表包含：Game 窗口相机 + Scene 视图相机 + 预览 / 探针相机等多个相机
                RenderCamera(context, camera);
                
            }
          
            
            //结束渲染上下文
            EndContextRendering(context, cameras);
        }


        private void RenderCamera(ScriptableRenderContext context, Camera camera)
        {
           //开始渲染相机 注册事件回调  
           BeginCameraRendering(context, camera);
           
           //获取相机剔除参数，并进行提出，使用引擎内的固定流程提出
           //应用层 CPU 视锥剔除
           ScriptableCullingParameters cullingParameters;
           if(!camera.TryGetCullingParameters(out cullingParameters))
               return;
           CullingResults cullingResults = context.Cull(ref cullingParameters);
           
           //为相机创建CommandBuffere
           
           CommandBuffer cmd = CommandBufferPool.Get(camera.name); //这个CommandBufferPool在外要添加两个程序集的 是nity.RenderPipelines.Core.Runtime
           
           
           //设置相机属性参数
           //对应 dx12 设置视口 / 裁剪矩形  把相机 VP 矩阵写入根常量 / 常量缓冲  绑定当前相机对应的屏幕 RTV 颜色缓冲、DSV 深度缓冲
           context.SetupCameraProperties(camera);
           
           //清理渲染目标
           cmd.ClearRenderTarget(true, true, CoreUtils.ConvertLinearToActiveColorSpace(camera.backgroundColor)); //清理然后设置颜色为backgroundColor （顺便转化一下色彩空间）
           
           
           
           //指定渲染排序设置 SortSetting
           //剩下物体做排序 → 近到远 / 远到近排绘制顺序 （不透明物体近的先画、远的后画） 目前传入camera应该是默认规则
           var sortSettings = new SortingSettings(camera); 
           
           //指定渲染状态设置 DrawSetting
           //DrawingSettings【绘制 Pass 设置：用 Shader 里哪个 Pass 去画物体】
           // 去物体 Shader 里找 LightMode = SRPDefaultUnlit 的 Pass
           var drawSetting = new DrawingSettings(s_ShaderTagId, sortSettings);
           
           
           
           //绘制不透明物体
           sortSettings.criteria = SortingCriteria.CommonOpaque; //绘制不透明物体
           
           //指定渲染过滤设置 FilterSetting
           //过滤设置：筛掉 “不该被画的物体”  enderQueueRange：按渲染队列筛选  RenderQueueRange.opaque：只拿不透明物体 (0~2500)
           var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
           
           //创建渲染列表 打包上面三大配置 + 剔除结果
           var rendererListParams = new RendererListParams(cullingResults,drawSetting, filteringSettings);
           var rendererList = context.CreateRendererList(ref rendererListParams);
           
           //绘制渲染列表
           cmd.DrawRendererList(rendererList);
           
           
           
           
           //绘制透明物体 再来一遍
           sortSettings.criteria = SortingCriteria.CommonTransparent;
           
           filteringSettings = new FilteringSettings(RenderQueueRange.transparent);
           
           rendererListParams = new RendererListParams(cullingResults,drawSetting, filteringSettings);
           
           rendererList = context.CreateRendererList(ref rendererListParams);
           
           cmd.DrawRendererList(rendererList);
           
           
           
           
           //提交命令缓冲区
           context.ExecuteCommandBuffer(cmd);
           
           //释放命令缓冲区
           cmd.Clear();
           CommandBufferPool.Release(cmd);
           
           //提交渲染上下文 这就是类似DX12的command queue了  commandlist装满了 发车了发车了
           context.Submit();
           
           //结束渲染相机 注册事件回调
           EndCameraRendering(context, camera);
            
            
        }
        
        
        
    }

}