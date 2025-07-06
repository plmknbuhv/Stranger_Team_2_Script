using _01_Work.HS.Building;
using _01_Work.HS.Building.BuildingSO;
using _01_Work.KHJ.CombatUnit;
using UnityEngine;

namespace _01_Work.HS.BuildingSystem.Building.Combat
{
    public class CrystalTower : BuildObject
    {
        [SerializeField] private EffectCreater _effect;
        [SerializeField] private Animator _anim;
        private AttackTowerDataSO _data;

        private float _curtime;
        private bool _isCool;
        public override void Build()
        {
            base.Build();
            _data = BuildingDataSO as AttackTowerDataSO; // 여기 추가함
        }

        private void Update()
        {
            if (_data == null) return;
            if (_isCool)
            {
                _curtime += Time.deltaTime;
                if (_data.CoolTime < _curtime)
                {
                    _curtime = 0;
                    _isCool = false;
                }
            }
            else
            {
                Collider[] hits = Physics.OverlapSphere(transform.position, _data.FindRange, _data.WhatIsUnit);
                float distance = float.MaxValue;
                EnemyUnit _unit = null;
                foreach (Collider hit in hits)
                {
                    if (hit.TryGetComponent(out EnemyUnit unit) && unit._isDeath == false)
                    {
                        float d = Vector3.Distance(unit.transform.position, unit.transform.position);
                        if (d < distance)
                        {
                            distance = d;
                            _unit = unit;
                        }
                    }
                }
                if (_unit != null)
                {
                    _isCool = true;
                    _anim.SetTrigger("ATTACK");
                }
            }
        }

        public void Attack()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, _data.FindRange, _data.WhatIsUnit);
            float distance = float.MaxValue;
            EnemyUnit _unit = null;
            foreach (Collider hit in hits)
            {
                if (hit.TryGetComponent(out EnemyUnit unit) && unit._isDeath == false)
                {
                    float d = Vector3.Distance(unit.transform.position, unit.transform.position);
                    if (d < distance)
                    {
                        distance = d;
                        _unit = unit;
                    }
                }
            }
            if (_unit != null)
            {
                _unit.Hit(_data.Damage, null);
                _effect.PlayEffect(_unit.transform);
                AudioManager.Instance.PlaySfx("TOWER_FIRE");
            }
        }
    }
}