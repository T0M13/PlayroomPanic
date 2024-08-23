using UnityEngine;
namespace AI_Traits
{
    [System.Serializable]
    public class Playfulness : Trait
    {
        public Playfulness() : base(TraitType.Playfulness, 0.5f) { }
    }
}