#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

namespace SlotMaker
{
	[CreateAssetMenu(fileName="EditorSettings", menuName="SlotMaker/ScriptableObject/EditorSettings")]
	public class EditorSettings : ScriptableObjectSingletonForEditor<EditorSettings> 
	{
		public string googleWebServiceUrl;
		public string googleWebServicePassword;
	}
}
#endif