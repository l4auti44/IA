using UnityEngine;

[RequireComponent(typeof(Boid))]
public class Character : MonoBehaviour
{
    FiniteStateMachine _fsm;
    private Boid _boid;

    void Start()
    {
        _boid = GetComponent<Boid>();
        _fsm = new FiniteStateMachine();

        _fsm.AddStates(PlayerStates.SeekFood, new SeekFoodState(_fsm, _boid));
        _fsm.AddStates(PlayerStates.Movement, new MovementState(_fsm, _boid));
        _fsm.AddStates(PlayerStates.Flee, new FleeState(_fsm, _boid));
        _fsm.AddStates(PlayerStates.Wander, new WanderState(_fsm, _boid));


        _fsm.ChangeState(PlayerStates.Movement);
    }

    private void Update()
    {
        _fsm.Update();
    }

}
