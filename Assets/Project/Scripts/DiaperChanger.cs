using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiaperChanger : PlacementZone
{

    public override void PlaceObject(IHoldableObject holdableObject)
    {
        if (!holdableObject.ObjectBeingHeld().GetComponent<AIAgent>()) return;

        base.PlaceObject(holdableObject);

        AIAgent ai = holdableObject.ObjectBeingHeld().GetComponent<AIAgent>();
        if (ai != null)
        {
            ai.toilet.OnDiaperChanger = IsOccupied;
            if (ai.toilet.NeedsDiaperChange)
                ai.toilet.NoDisplayNeedUI(ai.ChildUI);
        }
    }

    public override void RemoveObject(IHoldableObject holdableObject)
    {
        base.RemoveObject(holdableObject);
        AIAgent ai = holdableObject.ObjectBeingHeld().GetComponent<AIAgent>();
        if (ai != null)
        {
            ai.toilet.OnDiaperChanger = IsOccupied;
            if (ai.toilet.NeedsDiaperChange)
                ai.toilet.DisplayNeedUI(ai.ChildUI);
        }
    }

}
