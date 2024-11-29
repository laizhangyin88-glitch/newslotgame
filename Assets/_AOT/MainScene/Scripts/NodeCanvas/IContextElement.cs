using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace SlotMaker
{
	public enum ContextSearchingType
	{
		ChildrenSearch,
	    ChildrenDeepSearch,
	    FullNameSearch,
	    SelfContext
	};

	public interface IContextElement : IEnumerable
	{
		ContextElement Root { get; }

		ContextElement Parent { get; set; }

		string ContextName { get; }

		bool IsRoot { get; }
		bool IsLeaf { get; }

		void AddContextElement(ContextElement element);
		void RemoveContextElement(ContextElement element);
		void DestroyElement();

		ContextElement Find(string name, bool deepSearch);

		ContextElement FindWithFullName(string fullName);

		int ChildCount { get; }

		ContextElement GetChildElement(int index);

		void SetDirty();

		bool IsDirty();

		void UpdateContext(bool forceUpdate);
	}

	public interface IContextList {}
	public interface IContextCompositor {}
	public interface IContextIgnoreGroup {}

	public interface IContextPlayer
	{
	    void Play();
	}

	public interface IContextText
	{
		void SetText(string text);
		string GetText();
	}

	public interface IContextInputField
	{
		int characterLimit { get; set; }
		InputField.ContentType contentType { get; set; }
	}

	public interface IContextImage
	{
		void SetSprite(Sprite sprite);
	    bool CheckHash(string hashCode);
	    void SetHash(string hashCode);
	    void SetColor(Color color);
		// void AddListenerOnChangedSprite(UnityAction<ContextElement> action);
	}

	public interface IContextBooleanProperty
	{
		void SetBooleanProperty(bool value);
		bool GetBooleanProperty();
	}

	public interface IContextIntProperty
	{
		void SetIntProperty(int value);
		int  GetIntProperty();
	}

	public interface IContextFloatProperty
	{
		void SetFloatProperty(float value);
		float GetFloatProperty();
	}

	public interface IContextClearable
	{
		void ClearContext();
	}

	public interface IContextInteractable {}

	public interface IContextClickable : IContextInteractable
	{
		void AddListenerOnClick(UnityAction<ContextElement> action);
	    void RemoveAllListener();
		void DoClick();
	}

	public interface IContextListenable<T>
	{
	    void AddListener(UnityAction<T> action);
	}

    public interface IContextPressable : IContextInteractable
    {
        void AddListenerOnPress(UnityAction<ContextElement> action);
        void DoPress();
    }
}
