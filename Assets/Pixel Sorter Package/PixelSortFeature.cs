using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelSortFeature : ScriptableRendererFeature
{
    [SerializeField] private ComputeShader shader;
    [SerializeField] private Material shaderMaterial;

    class CustomRenderPass : ScriptableRenderPass
    {
        private ComputeShader pixelSorter;
        private Material shaderMaterial;

        private RenderTargetIdentifier source;
        private RenderTexture colorTex;
        private RTHandle tempTexture;
        private FilteringSettings filteringSettings;
        private bool _initialized = false;
        
        public CustomRenderPass(ComputeShader shader, Material shaderMaterial) : base()
        {
            this.pixelSorter = shader;
            this.shaderMaterial = shaderMaterial;

            colorTex = new RenderTexture(Screen.width, Screen.height, 1, UnityEngine.Experimental.Rendering.DefaultFormat.LDR);
            colorTex.enableRandomWrite = true;
            shaderMaterial.SetTexture("_MainTex", colorTex);

            // tempTexture = RTHandles.Alloc(Vector2.one, depthBufferBits: DepthBits.Depth32, dimension: TextureDimension.Tex2D, name: "PixelSortTexture");
            filteringSettings = new FilteringSettings(RenderQueueRange.all);
        }

        void Dispose()
        {
            tempTexture?.Release();
        }

        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            base.OnCameraSetup(cmd, ref renderingData);
            source = renderingData.cameraData.renderer.cameraColorTarget;
            ConfigureTarget(renderingData.cameraData.renderer.cameraColorTarget);
        }

        public void SetSource(RenderTargetIdentifier source)
        {
            this.source = source;
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("PixelSortFeature");

            RenderTextureDescriptor cameraTextureDesc = renderingData.cameraData.cameraTargetDescriptor;
            cameraTextureDesc.depthBufferBits = 0;

            RTHandle rtCamera = renderingData.cameraData.renderer.cameraColorTargetHandle;
            SortingCriteria sortingCriteria = renderingData.cameraData.defaultOpaqueSortFlags;
            DrawingSettings drawingSettings = CreateDrawingSettings(new ShaderTagId("SRPDefaultUnit"), ref renderingData, sortingCriteria);

            if(!_initialized)
            {
                Blit(cmd, source, colorTex, shaderMaterial);
                // Blitter.BlitCameraTexture(cmd, rtCamera, tempTexture, shaderMaterial, 0);
                _initialized = true;
            }
            else
            {
                Blit(cmd, colorTex, colorTex, shaderMaterial);
                // Blitter.BlitCameraTexture(cmd, tempTexture, tempTexture, shaderMaterial, 0);
            }
            Blit(cmd, colorTex, source);
            // Blitter.BlitCameraTexture(cmd, tempTexture, rtCamera);

            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }

    CustomRenderPass m_ScriptablePass;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass(shader, shaderMaterial);

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


