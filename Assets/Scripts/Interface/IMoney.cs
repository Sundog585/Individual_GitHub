using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoney
{
    Action<float> onMoneyChange { get; set; }
    Action<float> onDamageChange { get; set; }
    Action<float> onDefenceChange { get; set; }
}

