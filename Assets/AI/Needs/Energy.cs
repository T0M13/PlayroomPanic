using UnityEngine;

namespace AI_Needs
{
    [System.Serializable]
    public class Energy : Need
    {
        [SerializeField][Range(1, 100)] private float tiredThreshold = 20f;
        [SerializeField][Range(1, 100)] private float activeThreshold = 80f;

        public float TiredThreshold { get => tiredThreshold; set => tiredThreshold = value; }
        public float ActiveThreshold { get => activeThreshold; set => activeThreshold = value; }

        public Energy() : base(NeedType.Energy, 100f, 2f, 2f) { }

    }

}