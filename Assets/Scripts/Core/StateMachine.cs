namespace Core
{
    /// <summary>
    /// 狀態基類
    /// </summary>
    public abstract class State<T>
    {
        protected T context;

        public virtual void Enter(T context)
        {
            this.context = context;
        }

        public abstract void Update(float deltaTime);

        public virtual void Exit() { }
    }

    /// <summary>
    /// 狀態機
    /// </summary>
    public class StateMachine<T>
    {
        private State<T> currentState;
        private readonly T context;

        public State<T> CurrentState => currentState;

        public StateMachine(T context)
        {
            this.context = context;
        }

        public void ChangeState(State<T> newState)
        {
            currentState?.Exit();
            currentState = newState;
            currentState?.Enter(context);
        }

        public void Update(float deltaTime)
        {
            currentState?.Update(deltaTime);
        }
    }
}
