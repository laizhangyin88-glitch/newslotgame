using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.Rendering.Clipping2D
{
	[ExecuteAlways]
	public class ClippingNode : RenderGroup
	{
		private ClippingNode _parent;
		public ClippingNode parent { get { return _parent; } }

		public ClippingMask mask;

	    [Flags]
	    public enum ClippingNodeCategory
	    {
	    	Category1 = (1 << 0),
	    	Category2 = (1 << 1),
	    	Category3 = (1 << 2),
	    	Category4 = (1 << 3),
	    	Category5 = (1 << 4),
	    	Category6 = (1 << 5),
	    	Category7 = (1 << 6),
	    	Category8 = (1 << 7),
	    	Category9 = (1 << 8),
	    	Category10 = (1 << 9),
	    	Category11 = (1 << 10),
	    	Category12 = (1 << 11),
	    	Category13 = (1 << 12),
	    	Category14 = (1 << 13),
	    	Category15 = (1 << 14),
	    	Category16 = (1 << 15),
	    }

	    [HideInInspector]
	    [SerializeField]
	    private ClippingNodeCategory _category = ClippingNodeCategory.Category1;
	    [ShowInInspector]
	    public ClippingNodeCategory category
	    {
	    	get { return _category; }
	    	set
	    	{
	    		if (_category != value)
	    		{
		    		lastCategory = _category = value;
		    		SetDirty();
	    		}
	    	}
	    }
	    private ClippingNodeCategory lastCategory;

		public enum DestroyPolicy
		{
			Disabled,
			Destroyed
		}
		[PropertyOrder(100)]
		public DestroyPolicy destroyPolicy = DestroyPolicy.Destroyed;

	    private Dictionary<ClippingNodeCategory, Dictionary<Material, Material>> cachedMaterials = new Dictionary<ClippingNodeCategory, Dictionary<Material, Material>>();

	    private bool dirty;

	    public override bool IsRoot()
	    {
	    	return _parent == null;
	    }

		public override RenderGroup GetParentColorGroup() { return _parent; }

	    public bool IsActive()
	    {
	    	return isActiveAndEnabled;
	    }

	    public bool MaskReady()
	    {
	    	return mask != null && !mask.IsDestroyed() && mask.IsActive();
	    }

	    public bool IsDestroyed()
	    {
	    	return this == null;
	    }

	    public void SetDirty()
	    {
	    	dirty = true;
	    }

	    public bool IsDirty()
	    {
	    	return dirty;
	    }

	    private void OnEnable()
	    {
	    	_parent = FindParent();
	    	SetDirty();

	    	ClippingUpdateRegistry.RegisterClippingNode(this);
	    }

	    private void OnDisable()
	    {
	    	if (destroyPolicy == DestroyPolicy.Disabled)
	    		ClearCachedMaterials();

	    	ClippingUpdateRegistry.UnRegisterClippingNode(this);
	    }

	    private void OnDestroy()
	    {
	    	ClearCachedMaterials();
	    }

	    private void OnTransformParentChanged()
	    {
	    	_parent = FindParent();
	    	SetDirty();
	    }

	    private void OnDidApplyAnimationProperties()
	    {
	    	if (_category != lastCategory)
	    	{
	    		lastCategory = _category;
	    		SetDirty();
	    	}
	    }

	    public void ClearCachedMaterials()
	    {
	    	foreach (var materials in cachedMaterials.Values)
	    	{
	    		foreach (var sharedMaterial in materials.Values)
	    		{
	    			sharedMaterial.DestroyThis();
	    		}
	    	}
	    	cachedMaterials.Clear();
	    }

	    private ClippingNode FindParent()
	    {
	    	if (transform.parent == null)
	    		return null;
    		return transform.parent.GetComponentInParent<ClippingNode>();
	    }

	    public void PreBuild()
	    {
	    	if (!IsActive()) 
	    		return;

	    	DetectParentDirty();

	    	if (MaskReady())
    			mask.Calculate();
	    }

	    public void Build()
	    {
			var col = GetOverridenColor();

	    	foreach (var cachedMaterial in cachedMaterials)
	    	{
	    		var categoryMask = cachedMaterial.Key;
	    		var materials = cachedMaterial.Value;

	    		int depthCount;
	    		var nodes = FindNodes(categoryMask, out depthCount);

	    		bool enableTextureMask = false;
	    		if (depthCount > 0 && nodes[0].MaskReady() && nodes[0].mask)
	    			enableTextureMask = nodes[0].mask.IsTextureMask();

	    		foreach (var sharedMaterial in materials.Values)
	    		{
	    			ShaderUtils.SetKeywords(sharedMaterial, ShaderUtils.KEYWORD_CLIPPING_DEPTH, depthCount);
    				ShaderUtils.SetKeyword(sharedMaterial, ShaderUtils.KEYWORD_CLIPPING_ENABLE_TEXTURE_MASK, enableTextureMask);
	    			
					sharedMaterial.SetColor(ShaderUtils.ID_COLOR, col);

					if (enableTextureMask)
	    				sharedMaterial.SetTexture(ShaderUtils.ID_TEXTURE_MASK, nodes[0].mask.clipTexture);

	    			for (int depth = 0; depth < depthCount; ++depth)
	    			{
	    				sharedMaterial.SetMatrix(ShaderUtils.ID_CLIPPING_MATRIX[depth], nodes[depth].mask.clipMatrix);
	    				sharedMaterial.SetVector(ShaderUtils.ID_CLIPPING_ARGS[depth], nodes[depth].mask.clipArgs);
	    			}
	    		}
	    	}

	    	dirty = false;
	    }

	    private void DetectParentDirty()
	    {
	    	if (!dirty && !IsRoot() && _parent.IsDirty())
	    		SetDirty();
	    }

	    public ClippingNode FindFirstNode(ClippingNodeCategory categoryMask, bool includeRoot)
	    {
	    	var node = this;
	    	while (((int)(node.category & categoryMask) == 0) || !node.MaskReady())
	    	{
	    		if (node.IsRoot())
	    			return includeRoot ? node : null;

    			node = node.parent;
	    	}
	    	return node;
	    }

	    private ClippingNode[] maskedNodes = new ClippingNode[3];
	    private ClippingNode[] FindNodes(ClippingNodeCategory categoryMask, out int count)
	    {
	    	count = 0;
	    	var node = this;
	    	for (int i = 0; i < 3; ++i)
	    	{
	    		node = node.FindFirstNode(categoryMask, false);
	    		if (node == null)
	    			break;

	    		maskedNodes[i] = node;
	    		++count;

	    		// TextureMask first
	    		if (i > 0 && maskedNodes[i].mask.IsTextureMask())
	    		{
	    			var temp = maskedNodes[0];
	    			maskedNodes[0] = maskedNodes[i];
	    			maskedNodes[i] = temp;
	    		}

	    		if (node.IsRoot())
	    			break;

	    		node = node.parent;
	    	}
	    	return maskedNodes;
	    }

	    public Material GetSharedMaterial(Material material, ClippingNodeCategory categoryMask)
	    {
	    	Dictionary<Material, Material> mats;
	    	if (!cachedMaterials.TryGetValue(categoryMask, out mats))
	    	{
	    		mats = new Dictionary<Material, Material>();
	    		cachedMaterials.Add(categoryMask, mats);
	    	}

	    	Material sharedMaterial;
	    	if (!mats.TryGetValue(material, out sharedMaterial))
	    	{
	    		sharedMaterial = new Material(material);
	    		sharedMaterial.name = string.Format("{0} ({1})", material.name, gameObject.name);
	    		sharedMaterial.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
	    		sharedMaterial.CopyPropertiesFromMaterial(material);
	    		mats.Add(material, sharedMaterial);
	    	}

	    	return sharedMaterial;
	    }
	}
}