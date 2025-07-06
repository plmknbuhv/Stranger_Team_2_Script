using System.Collections.Generic;
using _01_Work.HS.Building;
using _01_Work.HS.Core.Map;
using UnityEngine;

namespace _01_Work.HS.Core.GameManagement.States
{
    public class SelectState : GameState
    {
        [field:SerializeField] public override GameStateType StateType { get; protected set; }
                
        protected GameManager _gameManager;
        protected InputSO _inputSO;
        
        [SerializeField] private LayerMask whatIsFocusObject;
        [SerializeField] private LayerMask whatIsGround;
        
        [SerializeField] private float selectionSpaceSpeed = 12.5f;
        
        private SelectionSpace _selectionSpaceObj;
        private SelectObject _beforeFocusObj;
        private SelectObject _currentSelectObject;
        private List<Ground> _selectGroundList = new List<Ground>();
        
        private bool _isFocusing;
        private bool _isPressed;
        private bool _isCanCutTree;
        private bool _isSelecting;
        private Vector3 _startDragPoint;
        
        public override void InitializeState(GameManager manager)
        {
            _gameManager = manager;
            _selectionSpaceObj = manager.SelectionSpaceObj;
            _inputSO = manager.InputSO;
        }

        public override void EnterState()
        {
            _isSelecting = true;
            _inputSO.OnSelectEvent += HandleSelectEvent;
            
            ActiveSelectionSpace(false);
            _beforeFocusObj?.CancelFocus();
        }

        public override void UpdateState()
        {
            if (_isPressed && CheckIsDrag())
            {
                if (_currentSelectObject is not null)
                    OffSelect();
                
                _gameManager.ChangeState(GameStateType.Drag);
            }
            
            FocusObject();
        }

        public override void ExitState()
        {
            _isSelecting = false;
            _isFocusing = false;
            _isPressed = false;
            _inputSO.OnSelectEvent -= HandleSelectEvent;

            if (_currentSelectObject is not null)
                OffSelect();
            ActiveSelectionSpace(false);
            _beforeFocusObj?.CancelFocus();
            _beforeFocusObj = null;
        }
        
        private void HandleSelectEvent(bool isPressed)
        {
            if (isPressed)
            {
                RaycastHit hitInfo = _inputSO.GetHitInfo(whatIsGround);
                _startDragPoint = hitInfo.point;
            }
            else
            { 
                if (_currentSelectObject is not null)
                    OffSelect();
                
                _currentSelectObject = _beforeFocusObj;
                _gameManager.ChangeSelectObj(_currentSelectObject);
                
                if (_beforeFocusObj is not null)
                {
                    _currentSelectObject.ChangeColor(Color.cyan);
                    _currentSelectObject.Focus();
                    _beforeFocusObj = null;
                }
            }

            _isPressed = isPressed;
        }
        
        private void OffSelect()
        {
            _currentSelectObject.ChangeColor(Color.white);
            _currentSelectObject.CancelFocus();
            _currentSelectObject = null;
        }
        
        bool CheckIsDrag()
        {
            RaycastHit hitInfo = _inputSO.GetHitInfo(whatIsGround);
            return Vector3.Distance(_startDragPoint, hitInfo.point) >= 0.08f;
        }

        #region Focusing
        private void FocusObject()
        {
            RaycastHit hitInfo = _inputSO.GetHitInfo(whatIsFocusObject); // 부딫힌 거 가져오기

            if (hitInfo.collider is null)
            {
                _beforeFocusObj?.CancelFocus();
                ActiveSelectionSpace(false);
                return;
            }
            
            if (!_isFocusing) // 꺼져있었으면 키고 위치 조정
                ActiveSelectionSpace(true); 
                
            if (hitInfo.collider.TryGetComponent(out SelectObject newSelectObj))
            {   // 만약 선택 가능 오브젝트이고
                if (newSelectObj is BuildObject buildObject) // 건물이면 땅이랑 같이 Focusing
                {
                    FocusingGround(buildObject.transform, buildObject);
                    FocusingSelectObject(buildObject);
                }
                if (newSelectObj is FriendlyUnit friendlyUnit) // 유닛이면 유닛만 Focusing
                {
                    FocusingSelectObject(friendlyUnit);
                    ActiveSelectionSpace(false);
                }
            }
            else if (hitInfo.collider.TryGetComponent(out Ground ground))
            {
                var buildObject = ground.PlaceObject as BuildObject;
                FocusingGround(ground.transform, buildObject);
                FocusingSelectObject(buildObject);
                // 땅 위에 건물 있으면 그거도 포커싱
            }
        }

        private void FocusingGround(Transform targetTrm, BuildObject buildObj)
        {
            Vector3 targetPos;
            
            if (buildObj is not null)
            {
                targetPos = new Vector3(
                    buildObj.transform.position.x,
                    _selectionSpaceObj.transform.position.y,
                    buildObj.transform.position.z);
            }
            else
            {
                targetPos = new Vector3(
                    targetTrm.position.x,
                    _selectionSpaceObj.transform.position.y,
                    targetTrm.position.z);
            }
                
            var selectionPos = Vector3.Lerp(_selectionSpaceObj.transform.position,
                targetPos, Time.fixedDeltaTime * selectionSpaceSpeed);

            if (buildObj is not null)
            {
                _selectionSpaceObj.SetSize(selectionPos,
                    buildObj.ActualSize.y+1,
                    buildObj.ActualSize.x+1);
            }
            else
                _selectionSpaceObj.SetSize(selectionPos);
        }

        private void FocusingSelectObject(SelectObject selectObject)
        {
            if (!_isSelecting) return;
            if (_currentSelectObject is not null && selectObject == _currentSelectObject)
                return;
            if (_beforeFocusObj != selectObject)
            {
                _beforeFocusObj?.CancelFocus();
                selectObject?.Focus();
                _beforeFocusObj = selectObject;
            }
        }
        #endregion

        private void ActiveSelectionSpace(bool isActive)
        {
            _isFocusing = isActive;
           
            _selectionSpaceObj.gameObject.SetActive(isActive);

            if (isActive)
            {
                RaycastHit hitInfo = _inputSO.GetHitInfo(whatIsFocusObject);
                var targetPos = _gameManager.GetCenterCellPosition(_gameManager.GetCellPosition(hitInfo.point));
                
                var resultPos = new Vector3(
                    targetPos.x,
                    _selectionSpaceObj.transform.position.y,
                    targetPos.z);
                _selectionSpaceObj.transform.position = resultPos;
            }
        }
    }
}
