using UnityEngine;

namespace _01_Work.HS.Core.GameManagement
{
    public enum GameStateType
    {
        Build, Select, UnitControl, Start, Drag
    }
    
    public abstract class GameState : MonoBehaviour
    {
        public abstract GameStateType StateType { get; protected set; }
        public abstract void InitializeState(GameManager manager);
        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void ExitState();
    }
}