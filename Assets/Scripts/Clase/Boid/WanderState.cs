public class WanderState : States
{
    private readonly Boid boid;

    public WanderState(FiniteStateMachine f, Boid boid)
    {
        fsm = f;
        this.boid = boid;
    }

    public override void OnEnter() { }

    public override void OnExit() { }

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

        if (boid.HasNearbyBoids())
        {
            fsm.ChangeState(PlayerStates.Movement);
            return;
        }

        boid.OnWander();
    }
}
