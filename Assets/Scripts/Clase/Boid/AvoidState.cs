public class AvoidState : States
{
    Boid boid;
    public AvoidState(FiniteStateMachine f, Boid _boid)
    {
        fsm = f;
        boid = _boid;
    }

    public override void OnEnter()
    {
        boid.SetSeparationWeight(0.3f);
    }

    public override void OnExit()
    {
        boid.ResetSeparationWeight();
    }

    public override void OnUpdate()
    {
        boid.OnAvoiding();
    }
}
