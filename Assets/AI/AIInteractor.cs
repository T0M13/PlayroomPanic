using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInteractor : InteractableObject
{
    [SerializeField] private AIAgent ai;

    private void OnValidate()
    {
        GetReferences();
    }

    private void Awake()
    {
        GetReferences();
    }

    private void GetReferences()
    {
        if (ai == null)
        {
            ai = GetComponent<AIAgent>();
        }
    }

    public override void Interact()
    {
        base.Interact();

        ai.toilet.DoDiaperChange(ai);
    }

    public override bool IsOnlyInteractableWhenNeeded()
    {
        return ai.toilet.NeedsDiaperChange; /*Add with "||" more when needed*/
    }

    public override NeedIcon InteractionIcon()
    {
        if (ai.toilet.NeedsDiaperChange)
            return ai.toilet.DiaperChangeIcon;
        else return base.InteractionIcon();
    }
}
