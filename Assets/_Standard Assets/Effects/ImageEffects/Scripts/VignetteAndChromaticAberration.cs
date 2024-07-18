using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    [RequireComponent (typeof(Camera))]
    [AddComponentMenu ("Image Effects/Camera/Vignette and Chromatic Aberration")]
    public class VignetteAndChromaticAberration : PostEffectsBase
    {
        public float intensity = 0.036f;                    // intensity == 0 disables pre pass (optimization)
        public Shader vignetteShader;


        private Material m_VignetteMaterial;


        public override bool CheckResources ()
        {
            CheckSupport (false);

            m_VignetteMaterial = CheckShaderAndCreateMaterial (vignetteShader, m_VignetteMaterial);

            if (!isSupported)
                ReportAutoDisable ();
            return isSupported;
        }


        void OnRenderImage (RenderTexture source, RenderTexture destination)
        {
            if ( CheckResources () == false)
            {
                Graphics.Blit (source, destination);
                return;
            }

            int rtW = source.width;
            int rtH = source.height;

            bool  doPrepass = intensity > 0.0f;

            RenderTexture color = null;
            RenderTexture color2A = null;

            if (doPrepass)
            {
                color = RenderTexture.GetTemporary (rtW, rtH, 0, source.format);

                m_VignetteMaterial.SetFloat("_Intensity", (1.0f / (1.0f - intensity) - 1.0f));		// intensity for vignette
                m_VignetteMaterial.SetTexture ("_VignetteTex", color2A);	// blurred texture

                Graphics.Blit (source, destination, m_VignetteMaterial, 0);			// prepass blit: darken & blur corners
            }
            else
            {
                Graphics.Blit (source, destination);
            }

            RenderTexture.ReleaseTemporary (color);
            RenderTexture.ReleaseTemporary (color2A);
        }
    }
}
