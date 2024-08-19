using PubSub;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Hub")]
public class EventHubSO : ScriptableObject
{
    public EventHub Instance { get; } = new EventHub();
}
