using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{
    // Singleton Instance
    public static GameManager Instance { get; private set; }

    [Header("Time Settings")]
    [SerializeField] private float timeScale = 1f;
    [SerializeField][Range(0, 24)] private float currentHour = 7f;
    [SerializeField] private float timePerHourInSeconds = 90f;
    [SerializeField][ShowOnly] private float timeAccumulator = 0f;

    [Header("Day Events")]
    [SerializeField] private List<DayEvent> dayEvents = new List<DayEvent>();

    [Header("Time of Day Settings")]
    [SerializeField][ShowOnly] private TimeOfDay currentTimeOfDay;
    private TimeOfDay previousTimeOfDay;

    [Header("Time Sections")]
    [SerializeField] private Vector2 openingTime = new Vector2(6, 7);
    [SerializeField] private Vector2 morningTime = new Vector2(7, 9);
    [SerializeField] private Vector2 midMorningTime = new Vector2(9, 12);
    [SerializeField] private Vector2 afternoonTime = new Vector2(12, 15);
    [SerializeField] private Vector2 lateAfternoonTime = new Vector2(15, 16);
    [SerializeField] private Vector2 closingTime = new Vector2(16, 17);

    private bool isTimeOfDayChanged = false;

    public Action onTimeOfDayChange;
    public Action onHourIncrement;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateTimeOfDay(true);
        StartCoroutine(TimeProgression());
    }

    private IEnumerator TimeProgression()
    {
        while (true)
        {
            timeAccumulator += Time.deltaTime * timeScale;

            if (timeAccumulator >= timePerHourInSeconds)
            {
                timeAccumulator = 0;
                IncrementHour();
            }

            yield return null;
        }
    }

    private void IncrementHour()
    {
        currentHour++;
        if (currentHour >= 24) currentHour = 0;
        HourIncremented();
        UpdateTimeOfDay(false);
        CheckEventsForCurrentHour();
    }

    private void UpdateTimeOfDay(bool forceUpdate = false)
    {
        TimeOfDay newTimeOfDay = DetermineTimeOfDay();

        isTimeOfDayChanged = (newTimeOfDay != currentTimeOfDay) || forceUpdate;

        if (isTimeOfDayChanged)
        {
            previousTimeOfDay = currentTimeOfDay;
            currentTimeOfDay = newTimeOfDay;
            OnTimeOfDayChanged();
        }
    }

    private TimeOfDay DetermineTimeOfDay()
    {
        if (currentHour >= openingTime.x && currentHour < openingTime.y)
        {
            return TimeOfDay.Opening;
        }
        else if (currentHour >= morningTime.x && currentHour < morningTime.y)
        {
            return TimeOfDay.Morning;
        }
        else if (currentHour >= midMorningTime.x && currentHour < midMorningTime.y)
        {
            return TimeOfDay.MidMorning;
        }
        else if (currentHour >= afternoonTime.x && currentHour < afternoonTime.y)
        {
            return TimeOfDay.Afternoon;
        }
        else if (currentHour >= lateAfternoonTime.x && currentHour < lateAfternoonTime.y)
        {
            return TimeOfDay.LateAfternoon;
        }
        else if (currentHour >= closingTime.x && currentHour < closingTime.y)
        {
            return TimeOfDay.Closing;
        }
        else
        {
            return TimeOfDay.Closed;
        }
    }

    private void HourIncremented()
    {
        //string ampm = currentHour >= 12 ? "PM" : "AM";
        //int hour = Mathf.FloorToInt(currentHour) % 12;
        //if (hour == 0) hour = 12; 
        //string timeText = $"{hour}:{(currentHour % 1) * 60:00} {ampm}";
        //Debug.Log("Hour Incremented - Current Time: " + timeText);
        onHourIncrement?.Invoke();
    }

    private void OnTimeOfDayChanged()
    {
        //Debug.Log("Time of Day Changed: " + currentTimeOfDay);
        onTimeOfDayChange?.Invoke();
    }

    private void CheckEventsForCurrentHour()
    {
        foreach (var dayEvent in dayEvents)
        {
            if (Mathf.Floor(currentHour) == dayEvent.hour)
            {
                dayEvent.TriggerEvent(); // Trigger specific game events
            }
        }
    }

    // Displays current time - call this only when necessary to avoid performance impact
    private void DisplayCurrentTime()
    {
        string ampm = currentHour >= 12 ? "PM" : "AM";
        int hour = Mathf.FloorToInt(currentHour) % 12;
        if (hour == 0) hour = 12; // Handle midnight/noon edge cases
        string timeText = $"{hour}:{(currentHour % 1) * 60:00} {ampm}";
        // Debug.Log("Current Time: " + timeText); // Use UI to display instead of continuous logging
    }
}

[System.Serializable]
public class DayEvent
{
    public string name;
    public int hour;
    public GameEvent gameEvent;

    public void TriggerEvent()
    {
        gameEvent?.ExecuteEvent();
    }
}

public abstract class GameEvent : ScriptableObject
{
    public abstract void ExecuteEvent();
}
