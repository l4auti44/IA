using UnityEngine;

public class MovementState : States
{
    private Boid boid;
    public MovementState(FiniteStateMachine f, Boid _boid)
    {
        fsm = f;
        boid = _boid;
    }

    public override void OnEnter()
    {
        foreach (Renderer rend in boid.GetComponentsInChildren<Renderer>())
        {
            foreach (Material material in rend.materials)
            {
                material.color = Color.gray;
            }
        }
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
        if (boid.FoodInRange())
        {
            fsm.ChangeState(PlayerStates.SeekFood);
            return;
        }

        if (boid.IsThreatInView())
        {
            fsm.ChangeState(PlayerStates.Flee);
            return;
        }

        if (!boid.HasNearbyBoids())
        {
            fsm.ChangeState(PlayerStates.Wander);
            return;
        }

        boid.OnMovement();
    }
}
