using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace AI_Needs
{
    [System.Serializable]
    public class ToiletNeed : Need
    {
        [SerializeField][Range(1, 100)] private float urgentThreshold = 20f;
        [SerializeField][Range(1, 100)] private float sufficientThreshold = 90f;
        [SerializeField][Range(0f, 1f)] private float decreaseChance = 0.55f;
        [SerializeField][Range(1f, 100f)] private float decreaseAmount = 7.5f;
        [SerializeField] private AIBrain toiletUseTask;
        [SerializeField][ShowOnly] private int acceptableAgeForToilet = 3;
        [Header("Above Age for Toilet Usage")]
        [SerializeField][ShowOnly] private bool isUsingToilet = false;
        [SerializeField][ShowOnly] private bool isWaitingForToilet = false;
        [SerializeField][ShowOnly] private ToiletZone currentToilet = null;
        [Header("Below Age for Toilet Usage")]
        [SerializeField][ShowOnly] private bool needsDiaperChange = false;
        [SerializeField][ShowOnly] private bool onDiaperChanger = false;
        [Header("Other")]
        [SerializeField] private NeedIcon diaperChangeIcon;
        public float UrgentThreshold { get => urgentThreshold; set => urgentThreshold = value; }
        public float DecreaseChance { get => decreaseChance; set => decreaseChance = value; }
        public float SufficientThreshold { get => sufficientThreshold; set => sufficientThreshold = value; }
        public bool IsUsingToilet { get => isUsingToilet; set => isUsingToilet = value; }
        public ToiletZone CurrentToilet { get => currentToilet; set => currentToilet = value; }
        public bool IsWaitingForToilet { get => isWaitingForToilet; set => isWaitingForToilet = value; }
        public bool NeedsDiaperChange { get => needsDiaperChange; set => needsDiaperChange = value; }
        public bool OnDiaperChanger { get => onDiaperChanger; set => onDiaperChanger = value; }
        public int AcceptableAgeForToilet { get => acceptableAgeForToilet; set => acceptableAgeForToilet = value; }
        public NeedIcon DiaperChangeIcon { get => diaperChangeIcon; set => diaperChangeIcon = value; }

        public ToiletNeed() : base(NeedType.Toilet, 100f, 2f, 2f) { }


        public void TryDecrease(AIAgent ai)
        {
            if (ai.energy.IsResting) return;
            if (IsUsingToilet) return;
            if (onDiaperChanger && ai.Age < AcceptableAgeForToilet) return;

            if (Random.value < DecreaseChance)
            {
                Decrease(decreaseAmount);
                CheckToiletNeed(ai);
            }
        }

        private void CheckToiletNeed(AIAgent ai)
        {
            if (CurrentValue <= urgentThreshold)
            {
                ai.SetUrgentTask(toiletUseTask);
            }
        }

        public void DoDiaperChange(AIAgent ai)
        {
            if (needsDiaperChange && onDiaperChanger)
            {
                ReplenishAll();
                needsDiaperChange = false;
                ai.Aiinteractor.SetInteractable(needsDiaperChange);
                NoDisplayNeedUI(ai.ChildUI);
            }
        }

        public void DisplayNeedUI(ChildUI childUI)
        {
            childUI.SetNeedIcon(DiaperChangeIcon);
        }

        public void NoDisplayNeedUI(ChildUI childUI)
        {
            childUI.SetDefaultIconAndOff();
        }
    }
}
