using UnityEngine;

public class PatrolState : States
{
    Hunter hunter;
    Color originalColor;

    public PatrolState(FiniteStateMachine f, Hunter _hunter)
    {
        fsm = f;
        hunter = _hunter;
        originalColor = hunter.GetComponent<Renderer>().material.color;
    }

    public override void OnEnter()
    {
        foreach (Renderer rend in hunter.GetComponentsInChildren<Renderer>())
        {
            foreach (Material material in rend.materials)
            {
                material.color = originalColor;
            }
        }
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        hunter.energy -= Time.deltaTime;
        if (hunter.energy <= 0f)
        {
            fsm.ChangeState(PlayerStates.Idle);
            return;
        }

        if (hunter.HasNearbyBoids())
        {
            fsm.ChangeState(PlayerStates.Hunt);
            return;
        }

        hunter.OnPatrol();
    }
}
