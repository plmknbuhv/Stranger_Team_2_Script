using System.Collections.Generic;
using _01_Work.HS.Building;
using _01_Work.HS.Building.BuildingSO;
using _01_Work.HS.Core.Map;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace _01_Work.HS.Core.GameManagement.States
{
    public class BuildState : GameState
    {
        [field:SerializeField] public override GameStateType StateType { get; protected set; }
        
        protected GameManager _gameManager;
        private InputSO _inputSO;
        
        [SerializeField] private LayerMask whatIsGround;
        
        [SerializeField] private BuildingDataSOList buildingDataList;
        private Dictionary<BuildingType, BuildingDataSO> _buildingDataDictionary;

        #region Building Data
        private BuildingDataSO _currentBuildingData;
        protected BuildObject _building;
        private Vector2 _buildingOffset;
        #endregion

        private bool _isRotating;
        private bool _isShowingPreview;

        [SerializeField] private float rotationDuration = 0.2f;
        [SerializeField] private EffectPlayer buildEffectPrefab;
        
        public UnityEvent OnBuildEvent;
        
        public override void InitializeState(GameManager manager)
        {
            _gameManager = manager;
            _inputSO = manager.InputSO;
            
            _buildingDataDictionary = new Dictionary<BuildingType, BuildingDataSO>();
            
            foreach (var buildingData in buildingDataList.buildDataSOList)
                _buildingDataDictionary[buildingData.buildingType] = buildingData;
        }

        public override void EnterState()
        {
            _inputSO.OnRotateEvent += HandleRotateEvent;
            _inputSO.OnClickEvent += HandleBuildEvent;
            _gameManager.TargetBuildingType.OnValueChanged += HandleChangeBuildingType;

            ShopManager.Instance.SetMoving(false);
            PrepareBuilding(_gameManager.TargetBuildingType.Value);
        }
        

        public override void UpdateState()
        {
            ShowBuildPreview();
        }

        public override void ExitState()
        {
            _inputSO.OnClickEvent -= HandleBuildEvent;
            _inputSO.OnRotateEvent -= HandleRotateEvent;
            _gameManager.TargetBuildingType.OnValueChanged -= HandleChangeBuildingType;
            
            BuildWarningText.Instance.ShowWarningText("", false);
            ShopManager.Instance.SetMoving(true);
        }
        
        protected virtual void HandleChangeBuildingType(BuildingType prev, BuildingType next)
        {
            if (prev != next)
                PrepareBuilding(_gameManager.TargetBuildingType.Value);
        }
        
        private void HandleRotateEvent()
        {
            if (_isRotating) return;
            _isRotating = true;
            
            var tempX = _buildingOffset.x;
            var tempY = -_buildingOffset.y;

            DOTween.To(() => _buildingOffset.x, x => _buildingOffset.x = x, tempY, rotationDuration);
            DOTween.To(() => _buildingOffset.y, y => _buildingOffset.y = y, tempX, rotationDuration);
            
            _building.transform.DORotate(new Vector3(
                _building.transform.localEulerAngles.x,
                _building.transform.localEulerAngles.y - 90,
                _building.transform.localEulerAngles.z), rotationDuration).OnComplete(() => _isRotating = false);
        }
        
        protected virtual void HandleBuildEvent()
        {
            _building.transform.DOPunchScale(_building.transform.localScale * 0.08f,0.12f,1);

            RaycastHit hitInfo = _inputSO.GetHitInfo(whatIsGround);

            if (hitInfo.collider is not null && (hitInfo.collider.TryGetComponent(out Ground hitGround)))
            {
                if (_isRotating) return;
                if (CheckCanBuild(hitGround) == false) return;

                Build(hitGround, hitInfo);
            }
        }

        protected virtual void Build(Ground hitGround, RaycastHit hitInfo)
        {
            for (int i = 0; i < (int)Mathf.Abs(_buildingOffset.y) + 1; i++)
            {
                for (int j = 0; j < (int)Mathf.Abs(_buildingOffset.x) + 1; j++)
                {
                    var groundCell = new Vector3(
                        hitGround.transform.position.x + (j * Mathf.Sign(_buildingOffset.x)) * 0.4f, 0,
                        hitGround.transform.position.z + (i * Mathf.Sign(_buildingOffset.y)) * 0.4f);

                    var ground = _gameManager.GetGroundData(groundCell);
                    ground.SetPlaceObject(_building);
                    _building.AddGround(ground);
                }
            }

            var absoluteX = Mathf.Abs(_buildingOffset.x);
            var absoluteY = Mathf.Abs(_buildingOffset.y);
                
            var effect = Instantiate(buildEffectPrefab, transform);
            effect.transform.localScale = new Vector3(1 + (absoluteX * 2), 1 + (absoluteY * 2), 1);
            effect.PlayEffect(GetCenterPos(hitGround, hitInfo), 10 + (int)(absoluteX + absoluteY) * 5);
                
            OnBuildEvent?.Invoke();
            
            _building.ChangeSize(new Vector2Int((int)absoluteX, (int)absoluteY));
            _building.Build();
            if (_currentBuildingData.isCanOnlyOne)
                _gameManager.onlyOneBuilding.Add(_currentBuildingData.buildingType);
            _building = null;
            _gameManager.ChangeState(GameStateType.Select);
            UseResource();
        }

        private void UseResource()
        {
            ResourceManager.Instance.UseResource(ResourceType.WOOD, _currentBuildingData.needWood);
            ResourceManager.Instance.UseResource(ResourceType.STONE, _currentBuildingData.needStone);
            ResourceManager.Instance.UseResource(ResourceType.CRYSTAL, _currentBuildingData.needCrystal);
            ResourceManager.Instance.UseResource(ResourceType.FOOD, _currentBuildingData.needFood);
            ResourceManager.Instance.UseResource(ResourceType.GOLD, _currentBuildingData.needGold);
        }

        protected void PrepareBuilding(BuildingType buildingType)
        {
            if (_building is not null)
                Destroy(_building.gameObject);
            
            _isShowingPreview = true;
            _currentBuildingData = _buildingDataDictionary[buildingType];
            
            _building = Instantiate(_currentBuildingData.buildingPrefab, transform);
            _building.PrepareBuild(_currentBuildingData);

            _buildingOffset = new Vector2Int(
                _currentBuildingData.width - 1,
                _currentBuildingData.height - 1);
            
            ShowBuildPreview();
        }
        
        private void ShowBuildPreview()
        {
            RaycastHit hitInfo = _inputSO.GetHitInfo(whatIsGround);
                
            if (hitInfo.collider is not null && (hitInfo.collider.TryGetComponent(out Ground hitGround)))
            {
                if (_isShowingPreview == false)
                    ActiveBuildPreview(true);
                
                _building.transform.position = GetCenterPos(hitGround, hitInfo);

                BuildWarningText.Instance.ShowWarningText("이곳엔 건물을 지을 수 없습니다.", !CheckCanBuild(hitGround));
                _building.ChangeColor(CheckCanBuild(hitGround) ? Color.cyan : Color.red);
            }
            else
            {
                BuildWarningText.Instance.ShowWarningText("이곳엔 건물을 지을 수 없습니다.", true);
                ActiveBuildPreview(false);
            }
        }

        private Vector3 GetCenterPos(Ground hitGround, RaycastHit hitInfo)
        {
            var previewCenterPos = new Vector3(
                hitGround.transform.position.x + (0.2f * _buildingOffset.x), hitInfo.point.y, 
                hitGround.transform.position.z + (0.2f * _buildingOffset.y));
            
            return previewCenterPos;
        }

        private bool CheckCanBuild(Ground ground)
        {
            if (_isShowingPreview == false) return false;
            
            List<Ground> grounds = new List<Ground>();
            for (int i = 0; i < (int)Mathf.Abs(_buildingOffset.y) + 1; i++)
            {
                for (int j = 0; j < (int)Mathf.Abs(_buildingOffset.x) + 1; j++)
                { 
                    var groundCellPos = new Vector3(
                        ground.transform.position.x + (j * Mathf.Sign(_buildingOffset.x)) * 0.4f, 0, 
                        ground.transform.position.z + (i * Mathf.Sign(_buildingOffset.y)) * 0.4f);
                     
                    var targetGround = _gameManager.GetGroundData(groundCellPos);
                    grounds.Add(targetGround);
                    if (targetGround is null || targetGround.IsCanPlace == false)
                        return false;
                }
            }
            
            return _building.CheckCanPlaceBuilding(grounds);
        }

        private void ActiveBuildPreview(bool isActive)
        {
            _isShowingPreview = isActive;
            _building.gameObject?.SetActive(isActive);
        }
    }
}