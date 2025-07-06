using _01_Work.HS.Core.Map;
using UnityEngine;

namespace _01_Work.HS.Core.GameManagement.States
{
    public class UnitControlState : SelectState
    {

        [SerializeField] private LayerMask whatIsCanMoveObject;
       


        public override void InitializeState(GameManager manager)
        {
            base.InitializeState(manager);
            _gameManager = manager;
            _inputSO = manager.InputSO;
        }

        public override void EnterState()
        {
            base.EnterState();
            print("Enter");
            _inputSO.OnRightClickEvent += HandleClickEvent;
        }


        public override void ExitState()
        {
            base.ExitState();
            print("Exit");
            _inputSO.OnRightClickEvent -= HandleClickEvent;
        }

        private void HandleClickEvent()
        {
            print("우클릭이요");
            RaycastHit hitInfo = _inputSO.GetHitInfo(whatIsCanMoveObject);
            if (hitInfo.collider) // 뭐가 있으면
            {
                if (hitInfo.collider.transform.TryGetComponent(out Ground ground))
                {
                    //_gameManager._controlUnitList.ForEach(unit => unit.));
                }
            }
        }





    }
}