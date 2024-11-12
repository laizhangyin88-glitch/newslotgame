using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.Rendering.Clipping2D
{
	[ExecuteAlways]
	public abstract class ClippingElement : MonoBehaviour
	{
		private ClippingNode _parentNode;
		public ClippingNode parentNode { get { return _parentNode; } }

		private ClippingNode _targetNode;
		public ClippingNode targetNode { get { return _targetNode; } }

		[HideInInspector]
		[SerializeField]
		protected Material _material;
		[ShowInInspector]
		public Material material
		{
			get { return _material; }
			set
			{
				if (_material != value)
				{
					_material = value;
					SetClippingDirty();
				}
			}
		}

		[HideInInspector]
		[SerializeField]
		protected ClippingNode.ClippingNodeCategory _clippingInteraction = ClippingNode.ClippingNodeCategory.Category1;
		[ShowInInspector]
		public ClippingNode.ClippingNodeCategory clippingInteraction
		{
			get { return _clippingInteraction; }
			set 
			{
				if (_clippingInteraction != value)
				{
					lastClippingInteraction = _clippingInteraction = value;
					SetClippingDirty();
				}
			}
		}
		private ClippingNode.ClippingNodeCategory lastClippingInteraction;

		protected bool clippingDirty;

		public bool IsActive()
	    {
	    	return isActiveAndEnabled;
	    }

	    public bool IsDestroyed()
	    {
	    	return this == null;
	    }

	    public void SetClippingDirty()
	    {
	    	clippingDirty = true;
	    }

	    protected virtual void OnEnable()
	    {
	    	_parentNode = GetComponentInParent<ClippingNode>();
	    	SetClippingDirty();

	    	ClippingUpdateRegistry.RegisterClippingElement(this);
	    }

	    protected virtual void OnDisable()
	    {
	    	ClippingUpdateRegistry.UnRegisterClippingElement(this);
	    }

	    protected virtual void OnTransformParentChanged()
	    {
	    	_parentNode = GetComponentInParent<ClippingNode>();
	    	SetClippingDirty();
	    }

	    private void OnDidApplyAnimationProperties()
	    {
	    	if (_clippingInteraction != lastClippingInteraction)
	    	{
	    		lastClippingInteraction = _clippingInteraction;
	    		SetClippingDirty();
	    	}
	    }

	    public void PreBuild()
	    {
	    	if (!IsActive())
	    		return;

	    	if ((_parentNode != null) && (clippingDirty || _parentNode.IsDirty()))
	    	{
	    		_targetNode = _parentNode.FindFirstNode(_clippingInteraction, true);
	    		if (_targetNode != null) SetClippingDirty();
	    	}
	    }

	    public void Build()
	    {
	    	if (clippingDirty && (_material != null))
	    	{
	    		if (_targetNode != null)
	    			OnPopulateMaterial(_targetNode.GetSharedMaterial(_material, clippingInteraction));
	    		else
	    			OnPopulateMaterial(_material);

    			clippingDirty = false;
	    	}
	    }

	    public abstract void OnPopulateMaterial(Material sharedMaterial);
	}
}