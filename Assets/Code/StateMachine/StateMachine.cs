using System;
using System.Collections.Generic;

public class StateMachine
{
    public int TicksSinceLastStateChange { get; private set; } = 0;

    private IState currentState = null;
    private Action currentStateAction = null;

    private Dictionary<IState, List<Transition>> transitionsMap = new();
    private Dictionary<IState, Action> onStateTicks = new();
    private List<Transition> currentTransitions = new();
    private List<Transition> anyTransitions = new();

    private static readonly List<Transition> emptyTransitions = new(0);

    public void Tick()
    {
        UpdateState();

        if (currentState == null)
            return;

        currentState.Tick();
        currentStateAction?.Invoke();

        TicksSinceLastStateChange++;
    }

    public void UpdateState()
    {
        if (TryGetTransition(out var transition))
        {
            transition.OnTransition();
            ChangeState(transition.To);
        }
    }

    public void ChangeState(IState state)
    {
        if (state == currentState)
            return;

        currentState?.OnExit();
        currentState = state;

        SetCurrentStateAction();
        SetCurrentTransitions();

        currentState.OnEnter();

        TicksSinceLastStateChange = 0;
    }

    public void AddTransition(IState from, IState to, Func<bool> predicate, Action onTransition = null)
    {
        if (transitionsMap.TryGetValue(from, out var transitions) == false)
        {
            transitions = new List<Transition>();
            transitionsMap[from] = transitions;
        }

        onTransition ??= Transition.DefaultOnTransition;
        transitions.Add(new Transition(to, predicate, onTransition));

        SetCurrentTransitions();
    }

    private void SetCurrentTransitions()
    {
        if (currentState == null)
            return;

        transitionsMap.TryGetValue(currentState, out currentTransitions);
        currentTransitions ??= emptyTransitions;
    }

    public void AddStateTickAction(IState state, Action action)
    {
        if (!onStateTicks.ContainsKey(state))
            onStateTicks.Add(state, action);
        else
            onStateTicks[state] += action;

        SetCurrentStateAction();
    }

    private void SetCurrentStateAction()
    {
        if (currentState == null)
            return;

        onStateTicks.TryGetValue(currentState, out var action);
        currentStateAction = action;
    }

    public void AddAnyTransition(IState state, Func<bool> predicate, Action onTransition = null)
    {
        onTransition ??= Transition.DefaultOnTransition;
        anyTransitions.Add(new Transition(state, predicate, onTransition));
    }

    private class Transition
    {
        public IState To { get; }
        public Func<bool> Condition { get; }
        public Action OnTransition { get; }

        public readonly static Action DefaultOnTransition = () => { };

        public Transition(IState to, Func<bool> condition, Action onTransition)
        {
            To = to;
            Condition = condition;
            OnTransition = onTransition;
        }
    }

    private bool TryGetTransition(out Transition transitionResult)
    {
        foreach (var transition in anyTransitions)
        {
            if (transition.Condition())
            {
                transitionResult = transition;
                return true;
            }
        }

        foreach (var transition in currentTransitions)
        {
            if (transition.Condition())
            {
                transitionResult = transition;
                return true;
            }
        }

        transitionResult = null;
        return false;
    }
}
