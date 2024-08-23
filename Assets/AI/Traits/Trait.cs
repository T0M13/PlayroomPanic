using UnityEngine;
namespace AI_Traits
{

    [System.Serializable]
    public class Trait
    {
        private TraitType traitType;  
        [Range(0, 1)] public float value;

        public TraitType TraitType { get => traitType; set => traitType = value; }

        public Trait(TraitType type, float initialValue)
        {
            this.TraitType = type;
            this.value = initialValue;
        }
        
        public void Randomize(float minValue, float maxValue)
        {
            value = Random.Range(minValue, maxValue);
        }
    }
}