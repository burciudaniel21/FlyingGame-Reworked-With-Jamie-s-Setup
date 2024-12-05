using UnityEngine;
using UnityEngine.Rendering;
#if IGLOO_URP
using UnityEngine.Rendering.Universal;
#endif

namespace Igloo.Common
{
#if IGLOO_URP
    public class BlitMaterialFeature : ScriptableRendererFeature
    {
        class RenderPass : ScriptableRenderPass
        {
            private string profilingName;
            private Material material;
            private int materialPassIndex;
            private RenderTargetIdentifier sourceID;
            private RenderTargetHandle tempTextureHandle;

            public RenderPass(string profilingName, Material material, int passIndex) : base()
            {
                this.profilingName = profilingName;
                this.material = material;
                this.materialPassIndex = passIndex;
                tempTextureHandle.Init("_TempBlitMaterialTexture");
            }

            public void SetSource(RenderTargetIdentifier source)
            {
                this.sourceID = source;
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                CommandBuffer cmd = CommandBufferPool.Get(profilingName);

                RenderTextureDescriptor cameraTextureDesc = renderingData.cameraData.cameraTargetDescriptor;
                cameraTextureDesc.depthBufferBits = 0;

                cmd.GetTemporaryRT(tempTextureHandle.id, cameraTextureDesc, FilterMode.Bilinear);
                Blit(cmd, sourceID, tempTextureHandle.Identifier(), material, materialPassIndex);
                Blit(cmd, tempTextureHandle.Identifier(), sourceID);

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            public override void FrameCleanup(CommandBuffer cmd)
            {
                cmd.ReleaseTemporaryRT(tempTextureHandle.id);
            }
        }

        [System.Serializable]
        public class Settings
        {
            public Material material;
            public int materialPassIndex = -1;
            public RenderPassEvent renderEvent = RenderPassEvent.AfterRenderingOpaques;
        }

        [SerializeField] private Settings settings = new Settings();

        private RenderPass renderPass;

        public Material Material
        {
            get { return settings.material; }
            set { settings.material = value;  }
        }

        public override void Create()
        {
            this.renderPass = new RenderPass(name, settings.material, settings.materialPassIndex);
            renderPass.renderPassEvent = settings.renderEvent;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            // Only render on the camera tagged WarpBlendOutputCamera
            if (renderingData.cameraData.camera.CompareTag("WarpBlendOutputCamera"))
            {
                renderPass.SetSource(renderer.cameraColorTarget);
                renderer.EnqueuePass(renderPass);
            }
        }
    }
#else
    public class BlitMaterialFeature : MonoBehaviour {
    // This class is not used when not running URP
    }
#endif
}

