using UnityEngine;

public class HuntState : States
{
    Hunter hunter;
    public HuntState(FiniteStateMachine f, Hunter _hunter)
    {
        fsm = f;
        hunter = _hunter;
    }

    public override void OnEnter()
    {
        foreach (Renderer rend in hunter.GetComponentsInChildren<Renderer>())
        {
            foreach (Material material in rend.materials)
            {
                material.color = Color.magenta;
            }
        }
    }

    public override void OnExit()
    {
        hunter.ClearTarget();
    }

    public override void OnUpdate()
    {
        hunter.energy -= Time.deltaTime;
        if (hunter.energy <= 0f)
        {
            fsm.ChangeState(PlayerStates.Idle);
            return;
        }

        hunter.OnHunt();
        if (hunter.targetBoid == null)
            fsm.ChangeState(PlayerStates.Patrol);
    }

}
