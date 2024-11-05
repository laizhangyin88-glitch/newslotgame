using UnityEngine;

namespace SlotMaker.Layout
{
	[ExecuteInEditMode]
	[RequireComponent (typeof (SpriteRenderer))]
	public class SpriteSizeFitter : MonoBehaviour
	{
		public RectTransform target;

		public enum FitMode
		{
			Unconstrained,
			FitToOrigin,
			FitToTarget
		}
		public FitMode horizontalFit = FitMode.Unconstrained;
		public FitMode verticalFit 	 = FitMode.Unconstrained;

		private SpriteRenderer _spriteRenderer;
		public SpriteRenderer spriteRenderer
		{
			get
			{
				if (_spriteRenderer == null)
					_spriteRenderer = GetComponent<SpriteRenderer>();
				return _spriteRenderer;
			}
		}

		private void Update()
		{
			if (spriteRenderer.sprite == null || target == null) return;

			switch (horizontalFit)
			{
			case FitMode.FitToOrigin:
				SetOriginHorizontal();
				break;
			case FitMode.FitToTarget:
				SetLayoutHorizontal();
				break;
			}

			switch (verticalFit)
			{
			case FitMode.FitToOrigin:
				SetOriginVertical();
				break;
			case FitMode.FitToTarget:
				SetLayoutVertical();
				break;
			}
		}
		
		private float GetFitSize(int axis)
		{
			float size = (axis == 0) ? target.rect.width : target.rect.height;
			return size;
		}

		private float GetSize(int axis)
		{
			float size = (axis == 0) ? spriteRenderer.sprite.rect.width : spriteRenderer.sprite.rect.height;
			return size;
		}

		private void HandleSelfFittingAlongAxis(int axis)
		{
			FitMode fitting = (axis == 0) ? horizontalFit : verticalFit;
			if (fitting == FitMode.Unconstrained) return;

			Vector3 sizeDelta = transform.localScale;

			sizeDelta[axis] = GetFitSize(axis) / GetSize(axis);
			transform.localScale = sizeDelta;
		}
		
		public void SetLayoutHorizontal ()
		{
			HandleSelfFittingAlongAxis (0);
		}
		
		public void SetLayoutVertical ()
		{
			HandleSelfFittingAlongAxis (1);
		}

		public void SetOriginHorizontal()
		{
			Vector3 sizeDelta = transform.localScale;
			sizeDelta[0] = 1.0f;
			transform.localScale = sizeDelta;
		}

		public void SetOriginVertical()
		{
			Vector3 sizeDelta = transform.localScale;
			sizeDelta[1] = 1;
			transform.localScale = sizeDelta;
		}
	}
}