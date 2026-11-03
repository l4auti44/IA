using UnityEngine;

public class IdleState : States
{
    Hunter hunter;
    float restTime;
    float remainingRestTime;
    public IdleState(FiniteStateMachine f, Hunter _hunter, float _restTime)
    {
        fsm = f;
        hunter = _hunter;
        restTime = _restTime;
    }

    public override void OnEnter()
    {
        remainingRestTime = restTime;
        hunter.StopMovement();
        foreach (Renderer rend in hunter.GetComponentsInChildren<Renderer>())
        {
            foreach (Material material in rend.materials)
            {
                material.color = Color.black;
            }
        }
    }

    public override void OnExit()
    {
        hunter.ResetEnergy();
    }

    public override void OnUpdate()
    {
        remainingRestTime -= Time.deltaTime;
        if (remainingRestTime <= 0f)
            fsm.ChangeState(PlayerStates.Patrol);
    }
}
