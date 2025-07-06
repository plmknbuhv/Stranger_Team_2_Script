using System;
using System.Collections;
using _01_Work.HS.Building.BuildingSO;
using _01_Work.HS.Core.GameManagement;
using _01_Work.KHJ.CombatUnit;
using _01_Work.KWJ._01_Scripes.WorkingUnit;
using _01_Work.LCM._01.Scripts.BuildResources.Resource;
using _01_Work.LCM._01.Scripts.Day;
using DG.Tweening;
using UnityEngine;

namespace _01_Work.HS.BuildingSystem.Building
{
    public class Castle : EconomyBuilding, IHittable
    {
        private CastleDataSO _castleDataSO;
        
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        
        [SerializeField] private EffectPlayer healEffect;
        [SerializeField] private EffectPlayer upgradeEffect;
        [SerializeField] private EffectPlayer destroyEffect;
        [SerializeField] private HitEffect hitEffect;
        
        public int Health {get; private set;}

        private bool _isCanShowText = true;
        private bool _isCanPunch = true;
        private int _level = 1;

        protected override void Awake()
        {
            base.Awake();
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        public override void Build()
        {
            base.Build();
            _castleDataSO = BuildingDataSO as CastleDataSO;
            
            Health = _castleDataSO.health;
            GameManager.Instance.OnChangeHealthEvent?.Invoke(Health);
            DayManager.Instance.StartTime();
            BuildResourceManager.Instance.CreateResources();
            AllUIToggle.Instance.UnAllSetup();
            WorkingUnitManager.Instance.AddMaxWorkingUnit(5, transform);
        }

        protected override void MakeMoney()
        {
            ResourceManager.Instance.AddResorce(ResourceType.GOLD, _castleDataSO.salaryByLevel[_level-1]);
        }
        
        public void Heal(int healValue)
        {
            Health += healValue;
            Health = Mathf.Clamp(Health, 0, _castleDataSO.health);
            
            GameManager.Instance.OnChangeHealthEvent?.Invoke(Health);
            
            EffectPlayer effect = Instantiate(healEffect);
            effect.PlayEffect(transform.position);
        }

        public void Upgrade()
        {
            if (CheckCanUpgrade() == false) return;
            if (_level >= 5) return;
            
            UseResource();
            
            _level++;
            EffectPlayer effect = Instantiate(upgradeEffect);
            effect.transform.localScale = new Vector3(7.5f, 7.5f, 1);
            effect.PlayEffect(transform.position + (Vector3.up / 3f), 25);
            AddMaxResource();
            SmallAlarmChat.Instance.AddChatMessage(
                $"성을 업그레이드 하여 성의 레벨이 <color=blue>{_level}레벨</color>이 되었습니다.");
                
            if (_level != 5)
            {
                _meshFilter.mesh = _castleDataSO.castleMeshes[_level-1];
            }
            else // 게임 클리어
            { 
                AudioManager.Instance.PlaySfx("GAME_CLEAR");
                InGameFade.Instance.ClearFadeOut();
            }
            GameManager.Instance.OnLevelUpEvent.Invoke(_level-1);
        }

        private void UseResource()
        {
            ResourceManager.Instance.UseResource(ResourceType.WOOD, _castleDataSO.upgradeDataList[_level-1].wood);
            ResourceManager.Instance.UseResource(ResourceType.STONE, _castleDataSO.upgradeDataList[_level-1].stone);
            ResourceManager.Instance.UseResource(ResourceType.CRYSTAL, _castleDataSO.upgradeDataList[_level-1].crystal);
            ResourceManager.Instance.UseResource(ResourceType.GOLD, _castleDataSO.upgradeDataList[_level-1].gold);
        }

        private void AddMaxResource()
        {
            ResourceManager.Instance.AddMaxCount(ResourceType.WOOD, 20);
            ResourceManager.Instance.AddMaxCount(ResourceType.STONE, 20);
            ResourceManager.Instance.AddMaxCount(ResourceType.CRYSTAL, 20);
            ResourceManager.Instance.AddMaxCount(ResourceType.FOOD, 10);
        }

        private bool CheckCanUpgrade()
        {
            AudioManager.Instance.PlaySfx("CASTLE_UPGRADE");
            CastleUpgradeData upgradeData = _castleDataSO.upgradeDataList[_level-1];
            bool isCanUpgrade = (ResourceManager.Instance.CheckCanUse(upgradeData.stone, upgradeData.wood, upgradeData.crystal, 0)
                && ResourceManager.Instance.CheckCanGoldUse(upgradeData.gold))
                && WorkingUnitManager.Instance.MaxWorkingUnitCount >= upgradeData.person;

            return isCanUpgrade;
        }

        public void Hit(float damage, CombatUnit combatUnit)
        {
            if (Health <= 0) return;
            hitEffect.EffectPlay();
            AudioManager.Instance.PlaySfx("CASTLE_HIT");
            Health = Mathf.Clamp(Health - (int)damage, 0, _castleDataSO.health);
            GameManager.Instance.OnChangeHealthEvent?.Invoke(Health);

            if (_isCanPunch)
            {
                _isCanPunch = false;
                transform.DOPunchScale(transform.localScale * 0.1f,0.13f,1);
                StartCoroutine(DelayCoroutine(1.3f, () => _isCanPunch = true));
            }
            if (_isCanShowText)
            {
                SmallAlarmChat.Instance.AddChatMessage(
                    "성이 적들에게 <color=red>공격</color> 받고 있습니다. 어서 적들을 물리치세요.");
                _isCanShowText = false;
                StartCoroutine(DelayCoroutine(10f, () => _isCanShowText = true));
            }

            if (Health <= 0)
            {
                GameManager.Instance.OnGameOverEvent.Invoke();
                Death();
            }
        }

        private IEnumerator DelayCoroutine(float duration, Func<bool> func)
        {
            yield return new WaitForSeconds(duration);
            func?.Invoke();
        }

        public void Death()
        {
            AudioManager.Instance.PlaySfx("CASTLE_BREAK");
            _meshFilter.mesh = _castleDataSO.castleMeshes[0];
            EffectPlayer effect = Instantiate(destroyEffect, transform);
            effect.PlayEffect(transform.position + Vector3.up / 3f);
            GameManager.Instance.OnGameOverEvent?.Invoke();
            InGameFade.Instance.GameOverFadeOut();
        }
    }
}