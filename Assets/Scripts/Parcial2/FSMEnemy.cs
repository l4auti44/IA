using System.Collections.Generic;

public class FSMEnemy
{
    Dictionary<EnemyStates, States> _allStates = new Dictionary<EnemyStates, States>();
    States _currentState;

    public void Update()
    {
        _currentState?.OnUpdate();
    }

    public void AddStates(EnemyStates name, States state)
    {
        if (!_allStates.ContainsKey(name)) 
        {
            _allStates.Add(name, state);
        }
        else
        {
            _allStates[name] = state;
        }
    }

    public void ChangeState(EnemyStates name)
    {
        if (!_allStates.TryGetValue(name, out States nextState) || nextState == null)
        {
            throw new System.InvalidOperationException(
                $"El estado {name} no esta registrado.");
        }

        if (ReferenceEquals(_currentState, nextState)) return;

        _currentState?.OnExit();
        _currentState = nextState;
        _currentState.OnEnter();
    }

}

public enum EnemyStates
{
    Patrol,
    Chase,
    Pathfinding
}
