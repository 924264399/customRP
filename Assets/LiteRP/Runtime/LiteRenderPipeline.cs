using UnityEngine;
using UnityEngine.Rendering;

namespace LiteRP
{
    public class LiteRenderPipeline : RenderPipeline  //运行时渲染管线实例（内存里的 C# 对象）
    {
        
        //虚函数复写 render函数 render函数是RenderPipeline类的
        protected override void Render(ScriptableRenderContext context, Camera[] cameras)  //每渲染一帧，Unity 自动调用这个实例的 Render 方法，所有画面绘制逻辑全写在这里，是真正干活渲染画面的主体对象
        {
            
        }
        
        
    }

}