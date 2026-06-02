
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace LiteRP
{
    [CreateAssetMenu(menuName = "Lite Render Pipeline/Lite Render Pipeline Asset")] //这样就可以在菜单Asset - Create -  Lite Render Pipeline 找到Lite Render Pipeline Asset
    public class LiteRPAsset : RenderPipelineAsset<LiteRenderPipeline>  // 这就是管线配置资源（磁盘上的 .asset 文件）  作用：存储管线的配置数据（以后写阴影、MSAA、后处理开关等参数都存在这个 asset 里）
    {
        
        protected override RenderPipeline CreatePipeline() //这个函数是 Unity 引擎自动调用  当你在ProjectSettings-Graphics把这个.asset资源赋值给管线设置后，进入播放模式 / 打开 Scene 视图时，Unity 自动执行这个方法，在这里new LiteRenderPipeline()创建运行实例。
        {

            return new LiteRenderPipeline();


        }
        
    }


    

}
