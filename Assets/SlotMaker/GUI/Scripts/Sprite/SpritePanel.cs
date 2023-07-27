using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using SlotMaker.Rendering;

namespace SlotMaker
{
    [ExecuteInEditMode]
    [AddComponentMenu("SlotMaker/UI/Sprite Panel")]
    public class SpritePanel : UIBehaviour
    {
    	public enum Clipping
    	{
    		None,
    		TextureMask,
    		SoftClip,
    		Distortion
    	};

    	[SerializeField] private Clipping _clipping;
    	[SerializeField] private Vector4 _clipRange = new Vector4(0f, 0f, 300f, 200f);
    	[SerializeField] private Vector4 _clipArgs = new Vector4(0f, 0f, 0f, 0f);
    	[SerializeField] private Vector2 _clipSoftness = new Vector2(0f, 0f);
    	[SerializeField] private Texture2D _clipTexture = null;
    	[SerializeField] private float _cutoff = -1f;
    	[SerializeField] private float _cutoffSoftness = 1f;
    	[SerializeField] private Material _material;
        [SerializeField] private Color _color = Color.white;
    	[SerializeField] private Vector4 _scale = new Vector4(1f, 1f, 1f, 1f);

    	private bool rebuildMaterial;

    	public Clipping clipping
    	{
    		get { return _clipping; }
    		set
    		{
    			if (_clipping != value)
    			{
    				_clipping = value;
    			}
    		}
    	}

        public Vector4 clipRange
    	{
    		get { return _clipRange; }
    		set
    		{
    			if (_clipRange != value)
    			{
    				_clipRange = value;
    			}
    		}
    	}

        public Vector4 clipArgs
    	{
    		get { return _clipArgs; }
    		set
    		{
    			if (_clipArgs != value)
    			{
    				_clipArgs = value;
    			}
    		}
    	}

        public Vector2 clipSoftness
    	{
    		get { return _clipSoftness; }
    		set
    		{
    			if (_clipSoftness != value)
    			{
    				_clipSoftness = value;
    			}
    		}
    	}

    	public Texture2D clipTexture
    	{
    		get { return _clipTexture; }
    		set
    		{
    			if (_clipTexture != value)
    			{
    				_clipTexture = value;
    			}
    		}
    	}

    	public float cutoff
    	{
    		get { return _cutoff; }
    		set
    		{
    			if (_cutoff != value)
    			{
    				_cutoff = value;
    			}
    		}
    	}

    	public float cutoffSoftness
    	{
    		get { return _cutoffSoftness; }
    		set
    		{
    			if (_cutoffSoftness != value)
    			{
    				_cutoffSoftness = value;
    			}
    		}
    	}

        public Color color
    	{
    		get { return _color; }
    		set
    		{
    			if (_color != value)
    			{
    				_color = value;
    			}
    		}
    	}

     	public Material material
     	{
     		get
     		{
     			return _material;
     		}
     		set
     		{
                if (_material != value)
                {
         			_material = value;
         			rebuildMaterial = true;
                }
     		}
     	}

        public Vector4 scale
    	{
    		get { return _scale; }
    		set
    		{
    			if (_scale != value)
    			{
    				_scale = value;
    			}
    		}
    	}

     	public Material dynamicMaterial { get; protected set; }

        public SpriteGroup spriteGroup = null;

     	protected override void Awake()
     	{
     		RebuildMaterial();
     	}

        protected override void OnEnable()
        {
            UpdateSpriteGroup();
        }

        protected override void OnTransformParentChanged()
        {
            if (gameObject.activeSelf)
                UpdateSpriteGroup();    
        }

        private void UpdateSpriteGroup()
        {
            spriteGroup = gameObject.GetComponentInParent<SpriteGroup>();
        }

     	protected override void OnDestroy()
     	{
     		dynamicMaterial.DestroyThis();
     		dynamicMaterial = null;
     	}

        private void RebuildMaterial()
    	{
    		dynamicMaterial.DestroyThis();

    		dynamicMaterial = new Material(material);
    		dynamicMaterial.name = material.name;
    		dynamicMaterial.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
    		dynamicMaterial.CopyPropertiesFromMaterial(material);
    		string[] keywords = material.shaderKeywords;
    		for (int i = 0; i < keywords.Length; ++i)
    			dynamicMaterial.EnableKeyword(keywords[i]);
    	}

     	protected virtual void LateUpdate()
    	{
    		if (material != null && rebuildMaterial)
    		{
    			RebuildMaterial();
    			rebuildMaterial = false;
    		}

            UpdateDrawCall();
    	}

    	public void UpdateDrawCall()
    	{
    		UpdateDrawCall(dynamicMaterial, 0);
    	}

    	public void UpdateDrawCall(Material mat, int depth)
    	{
    		if (mat == null) return;

    		if (_clipping != Clipping.None)
    		{
                Vector3 pos = transform.position;
                Vector3 scale = transform.lossyScale;
                Vector4 cr = _clipRange;

                cr.x *= scale.x;
                cr.y *= scale.y;
                cr.z *= 0.5f * scale.x;
                cr.w *= 0.5f * scale.y;

                if (_clipping == Clipping.SoftClip)
                {
                	Vector2 sharpness = new Vector2(1000.0f, 1000.0f);
                	if (_clipSoftness.x > 0f) sharpness.x = cr.z / _clipSoftness.x;
                	if (_clipSoftness.y > 0f) sharpness.y = cr.w / _clipSoftness.y;

                    mat.SetVector(SpriteShaderUtils.CLIP_ARGS_PROPERTY_ID[depth], new Vector4(sharpness.x, sharpness.y, 0f, 0f));
                }
                else if (_clipping == Clipping.TextureMask)
                {
                	float sharpness = 1f;
                	if (_cutoffSoftness > 0f) sharpness = 1f / _cutoffSoftness;

                	mat.SetTexture(SpriteShaderUtils.CLIP_TEX_PROPERTY_ID[depth], _clipTexture);
                	mat.SetFloat(SpriteShaderUtils.CLIP_CUTOFF_PROPERTY_ID[depth], _cutoff);
                	mat.SetFloat(SpriteShaderUtils.CLIP_CUTOFF_SHARPNESS_PROPERTY_ID[depth], sharpness);
                }
				else if (_clipping == Clipping.Distortion)
                {
					bool enableVertexScaling = _scale.x > 1 || _scale.y > 1;
					ShaderUtils.SetKeyword(mat, ShaderUtils.KEYWORD_ENABLE_VERTEX_SCALING, enableVertexScaling);
					mat.SetVector(SpriteShaderUtils.SCALE_PROPERTY_ID[depth], _scale);
                    mat.SetVector(SpriteShaderUtils.CLIP_ARGS_PROPERTY_ID[depth], new Vector4(1f / _clipArgs.x, _clipArgs.y, _clipArgs.z * 0.05f, _clipArgs.w));
                }

                mat.SetVector(SpriteShaderUtils.CLIP_PIVOT_PROPERTY_ID[depth], new Vector4(pos.x, pos.y, pos.z, 0f));
                mat.SetMatrix(SpriteShaderUtils.CLIP_ROTATATION_PROPERTY_ID[depth], Matrix4x4.TRS(Vector3.zero, Quaternion.Inverse(transform.rotation), Vector3.one));
                mat.SetVector(SpriteShaderUtils.CLIP_RANGE_PROPERTY_ID[depth], new Vector4(-cr.x / cr.z, -cr.y / cr.w, 1f / cr.z, 1f / cr.w));

                var parent = transform.parent;
                if (parent == null) return;

                var panel = parent.GetComponentInParent<SpritePanel>();
                if (panel != null)
                    panel.UpdateDrawCall(mat, depth + 1);
    		}

            if (depth == 0)
            {
                if (spriteGroup == null)
                    mat.SetColor(SpriteShaderUtils.COLOR_PROPERTY_ID, _color);
                else
                    mat.SetColor(SpriteShaderUtils.COLOR_PROPERTY_ID, _color * spriteGroup.color);    
            }
    	}
    }
}
