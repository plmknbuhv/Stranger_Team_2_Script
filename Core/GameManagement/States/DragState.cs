using System.Collections.Generic;
using System.Linq;
using _01_Work.HS.Core.Map;
using _01_Work.KWJ._01_Scripes.WorkingUnit;
using _01_Work.LCM._01.Scripts.BuildResources.Resource;
using UnityEngine;

namespace _01_Work.HS.Core.GameManagement.States
{
    public class DragState : GameState
    {
        [field:SerializeField] public override GameStateType StateType { get; protected set; }
        
        private GameManager _gameManager;
        private InputSO _inputSO;
        
        private SelectionSpace _selectionSpaceObj;
        
        [SerializeField] private LayerMask whatIsGround;
        
        private Vector3Int _startDragCellPoint;
        private Vector3 _spaceOffset;
        
        public override void InitializeState(GameManager manager)
        {
            _gameManager = manager;
            _selectionSpaceObj = manager.SelectionSpaceObj;
            _inputSO = manager.InputSO;
        }

        private void HandleSelectEvent(bool isPressed)
        {
            if (isPressed == false)
            {
                Select();
                _gameManager.ChangeState(GameStateType.Select);
            }
        }

        private void Select()
        {
            if (!_gameManager.IsGathering) return;
            
            RaycastHit hitInfo = _inputSO.GetHitInfo(whatIsGround);
            
            if (hitInfo.collider is not null && hitInfo.collider.TryGetComponent(out Ground ground))
            {
                List<Ground> grounds = new List<Ground>();
                
                for (int i = 0; i < (int)Mathf.Abs(_spaceOffset.z) + 1; i++)
                {
                    for (int j = 0; j < (int)Mathf.Abs(_spaceOffset.x) + 1; j++)
                    { 
                        var groundCellPos = new Vector3(
                            ground.transform.position.x + (j * Mathf.Sign(_spaceOffset.x) * -1) * 0.4f, 0, 
                            ground.transform.position.z + (i * Mathf.Sign(_spaceOffset.z) * -1) * 0.4f);
                     
                        var targetGround = _gameManager.GetGroundData(groundCellPos);
                        grounds.Add(targetGround);
                    }
                }

                List<Resource> resources = grounds.Where(g => g.PlaceObject is Resource)
                    .Select(g => g.PlaceObject as Resource).ToList();
                
                WorkingUnitManager.Instance.AddResources(resources);
            }

            _gameManager.IsGathering = false;
        }

        public override void EnterState()
        {
            _inputSO.OnSelectEvent += HandleSelectEvent;

            _selectionSpaceObj.ActiveQuad(true);
            StartDrag();
        }

        public override void UpdateState()
        {
            ShowDragSpace();
        }

        public override void ExitState()
        {
            _selectionSpaceObj.ActiveQuad(false);
            _selectionSpaceObj.ActiveTool(false);
            
            _inputSO.OnSelectEvent -= HandleSelectEvent;
        }
        
        private void ShowDragSpace()
        {
            RaycastHit hitInfo = _inputSO.GetHitInfo(whatIsGround);
                
            if (hitInfo.collider is not null && hitInfo.collider.TryGetComponent(out Ground ground))
            {
                var centerPos 
                    = (ground.transform.position + _gameManager.GetCenterCellPosition(_startDragCellPoint)) / 2;
                _spaceOffset = _gameManager.GetCellPosition(ground.transform.position) - _startDragCellPoint;
                
                _selectionSpaceObj.SetSize(centerPos, (int)Mathf.Abs(_spaceOffset.z) + 1, (int)Mathf.Abs(_spaceOffset.x) + 1);
            }
            else
                _gameManager.ChangeState(GameStateType.Select);
        }
        
        
        private void StartDrag()
        {
            RaycastHit hitInfo = _inputSO.GetHitInfo(whatIsGround);
               
            if (hitInfo.collider is null) return;
            
            _startDragCellPoint = _gameManager.GetCellPosition(hitInfo.point);
        }
    }
}