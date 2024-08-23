using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIBrain : ScriptableObject
{
    public abstract void DoAction(AIAgent ai);
    public abstract void DrawGizmos(AIAgent ai);
    public abstract float CalculatePriority(AIAgent ai);
    public abstract bool ShouldSwitch(AIAgent ai);

}
