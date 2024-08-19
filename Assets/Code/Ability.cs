using PubSub;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Ability : IActivatable
{
    public bool IsActive { get; private set; }
    [field:SerializeField] public string Name { get; set; } = string.Empty;
    public Arguments Arguments { get; private set; } = new();
    List<IActivatable> triggerEffects = new();
    List<ActiveEffect> activeEffects = new();

    public void AddTrigger<Context>(EventHub hub, Action<Arguments, Context> response)
    {
        var trigger = new TriggerEffect<Context>(response, hub, Arguments);
        triggerEffects.Add(trigger);

        if (IsActive)
            trigger.Activate();
    }

    public void AddActive(Action<Arguments, ActivationContext> response)
    {
        var active = new ActiveEffect(response, Arguments);
        activeEffects.Add(active);

        if (IsActive)
            active.Activate();
    }

    public void ExecuteActives(ActivationContext context)
    {
        foreach(var active in activeEffects)
        {
            active.Execute(context);
        }
    }

    public void Activate()
    {
        foreach(var trigger in triggerEffects)
        {
            trigger.Activate();
        }

        foreach(var active in activeEffects)
        {
            active.Activate();
        }
    }

    public void Deactivate()
    {
        foreach (var trigger in triggerEffects)
        {
            trigger.Deactivate();
        }

        foreach (var active in activeEffects)
        {
            active.Deactivate();
        }
    }
}

public interface IActivatable
{
    public void Activate();
    public void Deactivate();

    public bool IsActive { get; }
}

public class ActiveEffect : IActivatable
{
    public bool IsActive { get; private set; }

    Effect<ActivationContext> effect;

    public ActiveEffect(Action<Arguments, ActivationContext> onExecute, Arguments arguments)
    {
        effect = new Effect<ActivationContext>(onExecute, arguments);
    }

    public void Execute(ActivationContext context)
    {
        if(IsActive)
            effect.Execute(context);
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}

public class TriggerEffect<Context> : IActivatable
{
    public bool IsActive { get; private set; }

    Effect<Context> effect;
    EventHub hub;

    public TriggerEffect(Action<Arguments, Context> onExecute, EventHub hub, Arguments arguments)
    {       
        this.hub = hub;
        effect = new Effect<Context>(onExecute, arguments);
    }

    public void Activate()
    {
        if (!IsActive)
        {
            hub.SubEasy<Context>(effect.Execute);
            IsActive = true;
        }
    }
    public void Deactivate()
    {
        if (IsActive)
        {
            hub.Unsub<Context>(effect.Execute);
            IsActive = false;
        }
    }
}

public class Effect<Context>
{
    Arguments arguments;
    Action<Arguments, Context> OnExecute;

    public Effect(Action<Arguments, Context> onExecute, Arguments arguments = null)
    {
        this.arguments = arguments ?? (new());
        OnExecute = onExecute;
    }

    public void Execute(Context context)
    {
        OnExecute(arguments, context);
    }
}
