using UnityEngine;

public class Enemy : MonoBehaviour
{
    private FSMEnemy _fsm;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _fsm = new FSMEnemy();
        //_fsm.AddStates(PlayerStates.Patrol, new PatrolState(this));
        //_fsm.ChangeState(PlayerStates.Patrol);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
