using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _01_Work.HS.Core.GameManagement
{
    public class StateMachine : MonoBehaviour
    {
        private GameManager _gameManager;
        
        private Dictionary<GameStateType, GameState> _gameStates;
        
        [field:SerializeField] public GameState CurrentState { get; private set; }

        public void Initialize(GameManager gameManager)
        {
            _gameStates = new Dictionary<GameStateType, GameState>();
            _gameManager = gameManager;
            
            InitGameState();
        }
        
        private void Update()
        {
            CurrentState?.UpdateState();
        }

        private void InitGameState()
        {
            GetComponentsInChildren<GameState>().ToList().ForEach(state => _gameStates.Add(state.StateType, state));

            foreach (GameState state in _gameStates.Values)
                state.InitializeState(_gameManager);
        }
        
        public void ChangeState(GameStateType newStateType)
        {
            CurrentState?.ExitState();
            GameState newState = _gameStates.GetValueOrDefault(newStateType);
            Debug.Assert(newState != default, "Game state was not found");
            
            CurrentState = newState;
            CurrentState.EnterState();
        }
    }
}