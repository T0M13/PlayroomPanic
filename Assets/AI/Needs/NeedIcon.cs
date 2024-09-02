using AI_Needs;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Needs/Need Icon")]
public class NeedIcon : ScriptableObject
{
    public Sprite icon; 
    public NeedType needType;
}
