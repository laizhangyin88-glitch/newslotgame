using UnityEngine;
using System.Collections;

namespace SlotMaker
{

public interface IMemento
{
	void Do();
	void UnDo();
}

}