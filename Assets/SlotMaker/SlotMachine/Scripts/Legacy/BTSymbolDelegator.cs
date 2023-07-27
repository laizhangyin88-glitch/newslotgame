using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;

namespace SlotMaker
{
    [RequireComponent(typeof(SymbolEventDispatcher))]
    public class BTSymbolDelegator : MonoBehaviour
    {
    	private GameObject _delegator;
        public GameObject delegator
        {
            get { return _delegator; }
            set { Clear(ref _delegator); _delegator = value; }
        }

    	public Animator delegatorAnimator
    	{
    		get
    		{
    			return (_delegator != null) ? _delegator.GetComponent<Animator>() : null;
    		}
    	}

        private GameObject _decorator;
        public GameObject decorator
        {
            get { return _decorator; }
            set { Clear(ref _decorator); _decorator = value; }
        }

    	public Animator decoratorAnimator { get { return decorator.GetComponent<Animator>(); } }

    	private SymbolEventDispatcher _dispatcher;
    	protected SymbolEventDispatcher dispatcher { get { return _dispatcher ?? (_dispatcher = GetComponent<SymbolEventDispatcher>()); } }

    	private const string SKIP_ANIMATION_NAME = "Skip";

    	public void Clear()
    	{
    		ClearDelegator();
    		ClearDecorator();
    	}

    	public void ClearDelegator()
    	{
    		delegator = null;
    	}

    	public void ClearDecorator()
    	{
    		decorator = null;
    	}

    	private void Clear(ref GameObject go)
        {
            if (go == null)
                return;

            var po = go.GetComponent<PooledObject>();
            if (po == null)
                UnityEngine.Object.Destroy(go);
            else
                po.ReturnToPool();

            go = null;
        }

    	public void Play(BaseSymbol symbol, string animationName)
    	{
    		dispatcher.Dispatch(new EventData(animationName));
    	}

    	public void Play(string animationName)
    	{
    		dispatcher.Dispatch(new EventData(animationName));
    	}

    	public void Skip(BaseSymbol symbol)
    	{
    		dispatcher.Dispatch(new EventData(SKIP_ANIMATION_NAME));
    	}
    }
}
