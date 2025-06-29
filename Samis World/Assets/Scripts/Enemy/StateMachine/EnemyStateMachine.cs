using UnityEngine;

public class EnemyStateMachine
{
    private EnemyState currentState;

    public void Initialize(EnemyState startingState)
    {
        currentState = startingState;
        currentState?.Enter();
    }

    public void ChangeState(EnemyState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    public void Update()
    {
        currentState?.Update();
    }

    public void FixedUpdate()
    {
        currentState?.FixedUpdate();
    }

    public EnemyState GetCurrentState()
    {
        return currentState;
    }
}