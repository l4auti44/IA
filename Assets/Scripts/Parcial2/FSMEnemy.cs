using System.Collections.Generic;

public class FSMEnemy
{
    Dictionary<PlayerStates, States> _allStates = new Dictionary<PlayerStates, States>();
    States _currentState;

    public void Update()
    {
        _currentState?.OnUpdate();
    }

    public void AddStates(PlayerStates name, States state)
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

    public void ChangeState(PlayerStates name)
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
