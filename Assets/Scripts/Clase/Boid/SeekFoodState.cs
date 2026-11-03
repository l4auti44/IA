using UnityEngine;

public class SeekFoodState : States
{
    Boid boid;
    public SeekFoodState(FiniteStateMachine f, Boid _boid)
    {
        fsm = f;
        boid = _boid;
    }

    public override void OnEnter()
    {
        boid.SetSeparationWeight(0.3f);
        foreach (Renderer rend in boid.GetComponentsInChildren<Renderer>())
        {
            foreach (Material material in rend.materials)
            {
                material.color = Color.green;
            }
        }
    }

    public override void OnExit()
    {
        boid.ResetSeparationWeight();
    }

    public override void OnUpdate()
    {
        if (!boid.FoodInRange())
        {
            if (boid.IsThreatInView())
                fsm.ChangeState(PlayerStates.Flee);
            else
                fsm.ChangeState(boid.HasNearbyBoids() ? PlayerStates.Movement : PlayerStates.Wander);
            return;
        }

        boid.SeekFood();
    }
}
