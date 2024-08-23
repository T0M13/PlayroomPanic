using UnityEngine;

namespace AI_Needs
{
    [System.Serializable]
    public class Need
    {
        private NeedType needType;
        [Range(1, 100)] public float initialValue;
        [SerializeField][ShowOnly] private float currentValue;
        [SerializeField][Range(1, 100)] private float decayRate;
        [SerializeField][Range(1, 100)] private float replenishRate;

        public NeedType NeedType { get => needType; set => needType = value; }
        public float CurrentValue { get => currentValue; set => currentValue = value; }
        public float DecayRate { get => decayRate; set => decayRate = value; }
        public float ReplenishRate { get => replenishRate; set => replenishRate = value; }


        public Need(NeedType type, float initialValue, float decayRate, float replenishRate)
        {
            this.NeedType = type;
            this.initialValue = initialValue;
            this.DecayRate = decayRate;
            this.CurrentValue = initialValue;
            this.ReplenishRate = replenishRate;
        }

        public void Initialize()
        {
            CurrentValue = initialValue;
        }

        public void Decrease(float multiplier)
        {
            CurrentValue = Mathf.Clamp(CurrentValue - DecayRate * Time.deltaTime * multiplier, 0, 100);
        }

        public void Replenish(float multiplier)
        {
            CurrentValue = Mathf.Clamp(CurrentValue + ReplenishRate * Time.deltaTime * multiplier, 0, 100);
        }

    }
}
