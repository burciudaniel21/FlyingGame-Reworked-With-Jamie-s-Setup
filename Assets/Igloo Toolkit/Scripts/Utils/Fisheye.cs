using System;
using UnityEngine;

namespace Igloo
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class Fisheye : MonoBehaviour
    {
        public float strengthX = 0.05f;
        public float strengthY = 0.00f;
        public int numCameras = 1;

        public Shader fishEyeShader = null;
        public Material fisheyeMaterial = null;

        private float oneOverBaseSize = 0f;
        private float ar = 0f;
        private int sW = 1;
        private int sH = 1;

        public void setXY(Vector2 strengthXY)
        {
            strengthX = strengthXY.x;
            strengthY = strengthXY.y;
        }
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (fisheyeMaterial == null)
            {
                fisheyeMaterial = new Material(Shader.Find("IglooFisheyeShader"));

            }
            if (source.width != sW || source.height != sH)
            {
                oneOverBaseSize = 80.0f / 512.0f;
                ar = (source.width * 1.0f) / (source.height * 1.0f);
            }
            fisheyeMaterial.SetVector("intensity", new Vector4(strengthX * ar * oneOverBaseSize, strengthY * oneOverBaseSize, strengthX * ar * oneOverBaseSize, strengthY * oneOverBaseSize));
            Graphics.Blit(source, destination, fisheyeMaterial);
        }
    }
}
