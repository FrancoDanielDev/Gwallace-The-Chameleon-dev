using System;
using System.Collections.Generic;
using UnityEngine;

public enum Events
{
    Pause,
    Unpause,
    ResetParameters,
    StartPinShaking,
    StopPinShaking
}

public class EventManager: MonoBehaviour
{
    public static EventManager instance;

    private Dictionary<Events, Action> _events = new();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void Subscribe(Action listener, Events toEvent)
    {
        if (!_events.ContainsKey(toEvent))
        {
            _events.Add(toEvent, listener);
        }
        else
        {
            _events[toEvent] += listener;
        }
    }

    public void Unsubscribe(Action listener, Events fromEvent)
    {
        if (_events.ContainsKey(fromEvent))
        {
            _events[fromEvent] -= listener;
        }
    }

    public void Trigger(Events toEvent)
    {
        if (_events.ContainsKey(toEvent))
        {
            _events[toEvent].Invoke();
        }
    }
}
