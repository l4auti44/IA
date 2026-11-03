using UnityEngine;

public class FleeState : States
{
    Boid boid;
    public FleeState(FiniteStateMachine f, Boid _boid)
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
                material.color = Color.red;
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

        if (!boid.IsThreatInView())
        {
            fsm.ChangeState(boid.HasNearbyBoids() ? PlayerStates.Movement : PlayerStates.Wander);
            return;
        }

        boid.OnFlee();
    }
}
