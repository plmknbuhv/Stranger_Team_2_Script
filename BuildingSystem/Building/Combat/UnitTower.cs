using System;
using System.Collections.Generic;
using _01_Work.HS.Building;
using _01_Work.HS.Building.BuildingSO;
using _01_Work.KHJ.CombatUnit;
using NUnit.Framework;
using UnityEngine;

namespace _01_Work.HS.BuildingSystem.Building.Combat
{
    public class UnitTower : BuildObject
    {
        // [SerializeField] private UnitTowerDataSO _data; 이게 이전코드
        private UnitTowerDataSO _data; // 이걸로 바꿈

        private Vector3 _movePos;

        private float _curtime;
        private float _curtime2;
        private List<CombatUnit> _unitList;

        protected override void Awake()
        {
            base.Awake();
            _unitList = new List<CombatUnit>();
        }

        private void Start()
        {
            OnCanWorkEvent += StartUnitCreate;
        }

        public override void Build()
        {
            base.Build();
            _data = BuildingDataSO as UnitTowerDataSO; // 여기 추가함
        }

        private void StartUnitCreate()
        {
            foreach (CombatUnit unit in _unitList)
            {
                Destroy(unit.gameObject);
            }
            _unitList.Clear();

            for (int i = 0; i < _data.CombatUnitCount; i++)
            {
                UnitCreate();
            }
            CommendUnits();
        }



        private void Update()
        {
            if (_data == null) return;

            if (!CheckCanWork())
            {
                if (_unitList.Count > 0)
                {
                    foreach (CombatUnit unit in _unitList)
                    {
                        Destroy(unit.gameObject);
                    }
                    _unitList.Clear();
                }
                return;
            }

            if (_data.CombatUnitCount > _unitList.Count)
            {
                UnitRespawn();
            }

            _curtime2 += Time.deltaTime;
            if (_curtime2 > 6f)
            {
                _curtime2 = 0;
                CommendUnits();
            }

            DestroyDeathUnit();
        }

        public void DestroyDeathUnit()
        {
            foreach (CombatUnit unit in _unitList)
            {
                if (unit.GetState() == "DEATH")
                {
                    _unitList.Remove(unit);
                    break;
                }
            }
        }

        private void UnitRespawn()
        {
            if (ResourceManager.Instance.CheckCanUse(0, 0, 0, 5) && CheckCanWork())
            {
                _curtime += Time.deltaTime;
                if (_curtime > _data.UnitRespawnCoolTime)
                {
                    ResourceManager.Instance.UseResource(ResourceType.FOOD, 5);
                    _curtime = 0;
                    UnitCreate();
                }
            }
            else
            {
                _curtime = 0f;
            }
        }

        private void UnitCreate()
        {
            if (_unitList.Count >= _data.CombatUnitCount) return;
            CombatUnit unit = Instantiate(_data.SpawnUnit);
            unit.transform.position = transform.position;
            unit.SetTargetBuildObj(this);
            _unitList.Add(unit);
            CommendUnits();
        }

        public void CommendUnits() // 이동명령
        {
            float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            float dist = UnityEngine.Random.Range(0f, 2f);

            float x = Mathf.Cos(angle) * dist;
            float z = Mathf.Sin(angle) * dist;

            Vector3 pos = transform.position + new Vector3(x, 0f, z);

            Vector3[] positions = new Vector3[4];
            positions[0] = pos + new Vector3(-UnityEngine.Random.Range(0f, 0.3f), 0f, UnityEngine.Random.Range(0f, 0.3f));  // 좌측상단
            positions[1] = pos + new Vector3(UnityEngine.Random.Range(0f, 0.3f), 0f, UnityEngine.Random.Range(0f, 0.3f));   // 우측상단
            positions[2] = pos + new Vector3(-UnityEngine.Random.Range(0f, 0.3f), 0f, -UnityEngine.Random.Range(0f, 0.3f)); // 좌측하단
            positions[3] = pos + new Vector3(UnityEngine.Random.Range(0f, 0.3f), 0f, -UnityEngine.Random.Range(0f, 0.3f));  // 우측하단

            for (int i = 0; i < _unitList.Count; i++)
            {
                _unitList[i].SetMovePos(positions[i%4]);
            }
        }
    }
}