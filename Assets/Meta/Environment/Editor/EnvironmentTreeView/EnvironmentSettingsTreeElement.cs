using System;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class EnvironmentSettingsTreeElement
	{
		[SerializeField] int m_ID;
		[SerializeField] string m_Name;
		[SerializeField] int m_Depth;
		[NonSerialized] EnvironmentSettingsTreeElement m_Parent;
		[NonSerialized] List<EnvironmentSettingsTreeElement> m_Children;

		public int depth
		{
			get { return m_Depth; }
			set { m_Depth = value; }
		}

		public EnvironmentSettingsTreeElement parent
		{
			get { return m_Parent; }
			set { m_Parent = value; }
		}

		public List<EnvironmentSettingsTreeElement> children
		{
			get { return m_Children; }
			set { m_Children = value; }
		}

		public bool hasChildren
		{
			get { return children != null && children.Count > 0; }
		}

		public string name
		{
			get { return m_Name; }
			set { m_Name = value; }
		}

		public int id
		{
			get { return m_ID; }
			set { m_ID = value; }
		}

		public EnvironmentSettingsTreeElement()
		{
		}

		public EnvironmentSettingsTreeElement(string name, int depth, int id)
		{
			m_Name = name;
			m_ID = id;
			m_Depth = depth;
		}
	}
}