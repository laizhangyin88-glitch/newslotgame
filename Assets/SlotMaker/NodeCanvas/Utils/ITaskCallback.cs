using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{

public interface ITaskCallback
{
	Action endAction { get; set; }
}

}
