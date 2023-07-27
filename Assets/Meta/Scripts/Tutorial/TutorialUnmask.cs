using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;


namespace BagelCode
{
    
	public class TutorialUnmask : MonoBehaviour, IMaterialModifier
	{
		static readonly Vector2 s_Center = new Vector2(0.5f, 0.5f);


		[SerializeField] RectTransform m_FitTarget;
		[SerializeField] bool m_FitOnLateUpdate;
		[SerializeField] bool m_OnlyForChildren = false;
		[SerializeField] bool m_ShowUnmaskGraphic = false;


		public Graphic graphic{ get { return _graphic ?? (_graphic = GetComponent<Graphic>()); } }

		public RectTransform fitTarget
		{
			get { return m_FitTarget; }
			set
			{
				m_FitTarget = value;
				FitTo(m_FitTarget);
			}
		}

		public bool fitOnLateUpdate{ get { return m_FitOnLateUpdate; } set { m_FitOnLateUpdate = value; } }

		public bool showUnmaskGraphic
		{
			get { return m_ShowUnmaskGraphic; }
			set
			{
				m_ShowUnmaskGraphic = value;
				SetDirty();
			}
		}

		public bool onlyForChildren
		{
			get { return m_OnlyForChildren; }
			set
			{
				m_OnlyForChildren = value;
				SetDirty ();
			}
		}

		public Material GetModifiedMaterial(Material baseMaterial)
		{
			if (!isActiveAndEnabled)
			{
				return baseMaterial;
			}

			Transform stopAfter = MaskUtilities.FindRootSortOverrideCanvas(transform);
			var stencilDepth = MaskUtilities.GetStencilDepth(transform, stopAfter);

			StencilMaterial.Remove(_unmaskMaterial);
			_unmaskMaterial = StencilMaterial.Add(baseMaterial, (1 << stencilDepth) - 1, StencilOp.Zero, CompareFunction.Always, m_ShowUnmaskGraphic ? ColorWriteMask.All : (ColorWriteMask)0, 0, (1 << stencilDepth) - 1);

			var canvasRenderer = graphic.canvasRenderer;
			if (m_OnlyForChildren)
			{
				StencilMaterial.Remove (_revertUnmaskMaterial);
				_revertUnmaskMaterial = StencilMaterial.Add (baseMaterial, (1 << stencilDepth) - 1, StencilOp.Replace, CompareFunction.NotEqual, (ColorWriteMask)0);
				canvasRenderer.hasPopInstruction = true;
				canvasRenderer.popMaterialCount = 1;
				canvasRenderer.SetPopMaterial (_revertUnmaskMaterial, 0);
			}
			else
			{
				canvasRenderer.hasPopInstruction = false;
				canvasRenderer.popMaterialCount = 0;
			}

			return _unmaskMaterial;
		}

    
		public void FitTo(RectTransform target)
		{
			var rt = transform as RectTransform;

			rt.position = target.position;
			rt.rotation = target.rotation;

			var s1 = target.lossyScale;
			var s2 = rt.parent.lossyScale;
			rt.localScale = new Vector3(s1.x / s2.x, s1.y / s2.y, s1.z / s2.z);
			rt.sizeDelta = target.rect.size;
			rt.anchorMax = rt.anchorMin = s_Center;
		}

    
		Material _unmaskMaterial;
		Material _revertUnmaskMaterial;
		Graphic _graphic;


		void OnEnable()
		{
			if (m_FitTarget)
			{
				FitTo(m_FitTarget);
			}
			SetDirty();
		}

		void OnDisable()
		{
			StencilMaterial.Remove (_unmaskMaterial);
			StencilMaterial.Remove (_revertUnmaskMaterial);
			_unmaskMaterial = null;
			_revertUnmaskMaterial = null;

			if (graphic)
			{
				var canvasRenderer = graphic.canvasRenderer;
				canvasRenderer.hasPopInstruction = false;
				canvasRenderer.popMaterialCount = 0;
				graphic.SetMaterialDirty();
			}
			SetDirty ();
		}

		void LateUpdate()
		{
#if UNITY_EDITOR
			if (m_FitTarget && (m_FitOnLateUpdate || !Application.isPlaying))
#else
			if (m_FitTarget && m_FitOnLateUpdate)
#endif
			{
				FitTo(m_FitTarget);
			}
		}

#if UNITY_EDITOR

		void OnValidate()
		{
			SetDirty();
		}
#endif

		void SetDirty()
		{
			if (graphic)
			{
				graphic.SetMaterialDirty();
			}
		}
	}
}