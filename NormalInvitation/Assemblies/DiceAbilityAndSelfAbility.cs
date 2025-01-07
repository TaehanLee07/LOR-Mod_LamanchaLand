using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using LOR_DiceSystem;
using Sound;
using UI;

// 파괴 불가 주사위
public class DiceCardAbility_DestroyImmuneDice : DiceCardAbilityBase
{
    public override string[] Keywords => new string[1] { "DestroyImmuneDice" };

    public override bool IsImmuneDestory => true;

    public override void OnLoseParrying()
    {
        base.OnLoseParrying();
        BattleDiceBehavior battleDiceBehavior = new BattleDiceBehavior();
        battleDiceBehavior.behaviourInCard = behavior.behaviourInCard.Copy();
        battleDiceBehavior.SetIndex(this.card.GetDiceBehaviorList().Last().Index + 1);
        if (battleDiceBehavior.behaviourInCard.Script != string.Empty)
        {
            DiceCardAbilityBase diceCardAbilityBase = Singleton<AssemblyManager>.Instance.CreateInstance_DiceCardAbility(battleDiceBehavior.behaviourInCard.Script);
            if (diceCardAbilityBase != null)
            {
                battleDiceBehavior.AddAbility(diceCardAbilityBase);
            }
        }
        card.AddDice(battleDiceBehavior);

    }
    public static string Desc = "<color=#A374DB>파괴불가 주사위</color>";
}

public class DiceCardSelfAbility_DisabledClashDice : DiceCardSelfAbilityBase
{
    public override void OnUseCard()
    {
        base.OnUseCard();
        if (this.card != null && this.card.target != null && this.card.target.currentDiceAction != null)
        {
            this.card.target.currentDiceAction.DestroyDice(DiceMatch.AllDice, DiceUITiming.Start);
            Singleton<StageController>.Instance.AddAllCardListInBattle(this.card.target.currentDiceAction, base.owner, -1);
        }
    }

    public static string Desc = "<color=#A374DB>합 불가능 책장</color>";
}

public class DiceCardSelfAbility_MultiTargetDice : DiceCardSelfAbilityBase
{

    public override void OnUseCard()
    {
        base.owner.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.Bleeding, 20, base.owner);
    }
    public override void OnApplyCard()
    {
        this.FirstTarget = this.card.target;
        bool flag = this.card.subTargets.Count > 2;
        if (flag)
        {
            List<BattlePlayingCardDataInUnitModel.SubTarget> subTargets = this.card.subTargets;
            List<BattlePlayingCardDataInUnitModel.SubTarget> list = new List<BattlePlayingCardDataInUnitModel.SubTarget>();
            BattlePlayingCardDataInUnitModel.SubTarget subTarget = RandomUtil.SelectOne<BattlePlayingCardDataInUnitModel.SubTarget>(subTargets);
            list.Add(subTarget);
            subTargets.Remove(subTarget);
            list.Add(RandomUtil.SelectOne<BattlePlayingCardDataInUnitModel.SubTarget>(subTargets));
            this.card.subTargets = list;
        }
    }

    public static string Desc = "[다중 대상 지정 책장] 이 책장은 2명의 적을 타겟으로 한다.\n자신에게 출혈 20 부여";

    private BattleUnitModel FirstTarget;
}

public class DiceCardAbility_DmgByPercentDmg : DiceCardAbilityBase
{
    public static string Desc = "[적중 시] 대상의 최대 체력의 20%만큼 피해를 줌";
    public override void OnSucceedAttack(BattleUnitModel target)
    {
        if (target != null)
        {
            int value = target.MaxHp % 20;
            base.card.target?.TakeDamage(value, DamageType.Card_Ability, base.owner);
        }
    }
}

public class DiceCardAbility_DmgByPercentBDmg : DiceCardAbilityBase
{
    public static string Desc = "[적중 시] 대상의 최대 흐트러짐의 20%만큼 피해를 줌";
    public override void OnSucceedAttack(BattleUnitModel target)
    {
        if (target != null)
        {
            int value = target.MaxBreakLife % 20;
            base.card.target?.TakeBreakDamage(value, DamageType.Card_Ability, base.owner);
        }
    }
}

public class DiceCardAbility_Bleeding2WithDecay1 : DiceCardAbilityBase
{
    public static string Desc = "[적중 시] 출혈 2 부여 파열 1 부여";
    public override void OnSucceedAttack()
    {
        base.OnSucceedAttack();
        BattleUnitModel battleUnitModel = base.card?.target;
        if (battleUnitModel != null)
        {
            battleUnitModel.bufListDetail.AddKeywordBufByCard(KeywordBuf.Bleeding, 2, base.owner);
            battleUnitModel.bufListDetail.AddKeywordBufByCard(KeywordBuf.Decay, 1, base.owner);
        }
    }
}

public class DiceCardAbility_WinParryingBleeding4WithDeacy4WithUnBreakAbleDice : DiceCardAbilityBase
{
    public static string Desc = "[합 승리 시] 대상에게 출혈 4 부여 파열 4 부여\n[합 패배 시] 자신에게 출혈 20 부여\n<color=#A374DB>파괴불가 주사위</color>";
    public override void OnWinParrying()
    {
        base.OnWinParrying();
        BattleUnitModel battleUnitModel = base.card?.target;
        if (battleUnitModel != null)
        {
            battleUnitModel.bufListDetail.AddKeywordBufByCard(KeywordBuf.Bleeding, 4, base.owner);
            battleUnitModel.bufListDetail.AddKeywordBufByCard(KeywordBuf.Decay, 4, base.owner);
        }
    }
    public override string[] Keywords => new string[1] { "DestroyImmuneDice" };

    public override bool IsImmuneDestory => true;

    public override void OnLoseParrying()
    {
        base.OnLoseParrying();
        base.owner.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.Bleeding, 20, base.owner);
        BattleDiceBehavior battleDiceBehavior = new BattleDiceBehavior();
        battleDiceBehavior.behaviourInCard = behavior.behaviourInCard.Copy();
        battleDiceBehavior.SetIndex(this.card.GetDiceBehaviorList().Last().Index + 1);

        if (battleDiceBehavior.behaviourInCard.Script != string.Empty)
        {
            DiceCardAbilityBase diceCardAbilityBase = Singleton<AssemblyManager>.Instance.CreateInstance_DiceCardAbility(battleDiceBehavior.behaviourInCard.Script);
            if (diceCardAbilityBase != null)
            {
                battleDiceBehavior.AddAbility(diceCardAbilityBase);
            }
        }
        card.AddDice(battleDiceBehavior);

    }
}

public class DiceCardAbility_OnSucceedAtkBleeding3WithUnBreakAbleDice : DiceCardAbilityBase
{
    public static string Desc = "[적중 시] 출혈 3 부여\n<color=#A374DB>파괴불가 주사위</color>";
    public override void OnSucceedAttack()
    {
        base.OnSucceedAttack();
        BattleUnitModel battleUnitModel = base.card?.target;
        if (battleUnitModel != null)
        {
            battleUnitModel.bufListDetail.AddKeywordBufByCard(KeywordBuf.Bleeding, 3, base.owner);
        }
    }
    public override string[] Keywords => new string[1] { "DestroyImmuneDice" };

    public override bool IsImmuneDestory => true;

    public override void OnLoseParrying()
    {
        base.OnLoseParrying();
        BattleDiceBehavior battleDiceBehavior = new BattleDiceBehavior();
        battleDiceBehavior.behaviourInCard = behavior.behaviourInCard.Copy();
        battleDiceBehavior.SetIndex(this.card.GetDiceBehaviorList().Last().Index + 1);
        if (battleDiceBehavior.behaviourInCard.Script != string.Empty)
        {
            DiceCardAbilityBase diceCardAbilityBase = Singleton<AssemblyManager>.Instance.CreateInstance_DiceCardAbility(battleDiceBehavior.behaviourInCard.Script);
            if (diceCardAbilityBase != null)
            {
                battleDiceBehavior.AddAbility(diceCardAbilityBase);
            }
        }
        card.AddDice(battleDiceBehavior);

    }
}



public class DiceCardAbility_WinParryingBleeding1WithUnBreakAbleDice : DiceCardAbilityBase
{
    public static string Desc = "[적중 시] 대상에게 출혈 2 부여\n<color=#A374DB>파괴불가 주사위</color>";
    public override void OnSucceedAttack()
    {
        base.OnSucceedAttack();
        BattleUnitModel battleUnitModel = base.card?.target;
        if (battleUnitModel != null)
        {
            battleUnitModel.bufListDetail.AddKeywordBufByCard(KeywordBuf.Bleeding, 2, base.owner);
        }
    }
    public override string[] Keywords => new string[1] { "DestroyImmuneDice" };

    public override bool IsImmuneDestory => true;

    public override void OnLoseParrying()
    {
        base.OnLoseParrying();
        BattleDiceBehavior battleDiceBehavior = new BattleDiceBehavior();
        battleDiceBehavior.behaviourInCard = behavior.behaviourInCard.Copy();
        battleDiceBehavior.SetIndex(this.card.GetDiceBehaviorList().Last().Index + 1);
        if (battleDiceBehavior.behaviourInCard.Script != string.Empty)
        {
            DiceCardAbilityBase diceCardAbilityBase = Singleton<AssemblyManager>.Instance.CreateInstance_DiceCardAbility(battleDiceBehavior.behaviourInCard.Script);
            if (diceCardAbilityBase != null)
            {
                battleDiceBehavior.AddAbility(diceCardAbilityBase);
            }
        }
        card.AddDice(battleDiceBehavior);

    }

    public override void OnDrawParrying()
    {
        base.OnDrawParrying();
        BattleDiceBehavior battleDiceBehavior = new BattleDiceBehavior();
        battleDiceBehavior.behaviourInCard = behavior.behaviourInCard.Copy();
        battleDiceBehavior.SetIndex(this.card.GetDiceBehaviorList().Last().Index + 1);
        if (battleDiceBehavior.behaviourInCard.Script != string.Empty)
        {
            DiceCardAbilityBase diceCardAbilityBase = Singleton<AssemblyManager>.Instance.CreateInstance_DiceCardAbility(battleDiceBehavior.behaviourInCard.Script);
            if (diceCardAbilityBase != null)
            {
                battleDiceBehavior.AddAbility(diceCardAbilityBase);
            }
        }
        card.AddDice(battleDiceBehavior);

    }

}


public class PassiveAbility_atkByMostBleedingEnmey : PassiveAbilityBase
{
    // 출혈이 있는 대상 우선 지정\n자신에게 출혈이 10 이상 있다면 힘 1, 신속 1 얻음\n자신에게 출혈이 20 이상 있다면 힘 2, 속박 2 얻음\n자신에게 출혈이 30 이상 있다면 힘 3, 속박 3, 취약 3, 흐트러짐 취약 3을 얻는다.\상태이상을 받을 때, 받는 수치가 절반이 된다.(자신이 얻는 경우 제외, 소수점 이하 버림, 0 이하로 내려가지 않음)
    public override BattleUnitModel ChangeAttackTarget(BattleDiceCardModel card, int idx)
    {
        return BattleObjectManager.instance.GetAliveList((owner.faction == Faction.Enemy) ? Faction.Player : Faction.Enemy).Find((BattleUnitModel x) => x.bufListDetail.GetActivatedBuf(KeywordBuf.Bleeding) != null && x.IsTargetable(owner));
    }

    public override void OnRoundStart()
    {
        if (owner.bufListDetail.GetKewordBufStack(KeywordBuf.Bleeding) >= 10)
        {
            owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Strength, 1, owner);
            owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Quickness, 1, owner);
        }
        if (owner.bufListDetail.GetKewordBufStack(KeywordBuf.Bleeding) >= 20)
        {
            owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Strength, 2, owner);
            owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Binding, 2, owner);
        }
        if (owner.bufListDetail.GetKewordBufStack(KeywordBuf.Bleeding) >= 30)
        {
            int stack = 3;

            owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Strength, stack, owner);
            owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Binding, stack, owner);
            owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Vulnerable, stack, owner);
            owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Vulnerable_break, stack, owner);
        }
    }
    public override bool IsImmune(KeywordBuf buf)
    {
        return base.IsImmune(buf);
    }
}

public class PassiveAbility_Don_Quixote: PassiveAbilityBase
{
    // 공격 적중 시 체력을 5 회복하고, 자신에게 출혈이 있다면 모든 주사위 위력 + 1\n이 효과로 체력을 75 이상 회복했다면 다음 턴에 속박 3 얻는다.

    private int _recoveredAmount;

    private const int _RECOVER_AMOUNT = 15;

    public override void OnRoundStart()
    {
        _recoveredAmount = 0;
    }

    public override void OnRecoverHp(int amount)
    {
        _recoveredAmount += amount;
    }

    public override void OnSucceedAttack(BattleDiceBehavior behavior)
    {
        owner.RecoverHP(15);
        owner.battleCardResultLog?.SetPassiveAbility(this);
    }

    public override void OnRoundEndTheLast()
    {
        base.OnRoundEndTheLast();
        if (_recoveredAmount >= 75)
        {
            owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Binding, 3);
        }
        _recoveredAmount = 0;
    }

    private const int _pow = 1;

    public override void BeforeRollDice(BattleDiceBehavior behavior)
    {
        base.BeforeRollDice(behavior);
        if (CheckCondition() && IsAttackDice(behavior.Detail))
        {
            behavior?.ApplyDiceStatBonus(new DiceStatBonus
            {
                power = 1
            });
        }
    }

    private bool CheckCondition()
    {
        BattleUnitModel battleUnitModel = owner;
        if (battleUnitModel != null && battleUnitModel.bufListDetail.GetActivatedBuf(KeywordBuf.Bleeding)?.stack > 0)
        {
            return true;
        }
        return false;
    }
}

public class PassiveAbility_for_famiiy : PassiveAbilityBase
{
    // 막 종료 시 흐트러짐 상태면, 다음 막 시작 시 흐트러짐을 해제하고, 출혈을 40 만큼 얻고 신속을 1 얻는다. (1회)

    private bool canfixedbreak;

    public override void OnWaveStart()
    {
        canfixedbreak = true;
    }

    public override bool OnBreakGageZero()
    {
        if (canfixedbreak)
        {
            owner.breakDetail.ResetGauge();
            owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Bleeding, 40, owner);
            owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Quickness, 1, owner);
            return true;
        }
        return false;
    }

}

public class PassiveAbility_spearBleed : PassiveAbilityBase
{
    // "출혈을 부여할 때, 부여하는 출혈 수치 +1";

    public override int OnGiveKeywordBufByCard(BattleUnitBuf buf, int stack, BattleUnitModel target)
    {
        if (buf.bufType == KeywordBuf.Bleeding)
        {
            owner.battleCardResultLog?.SetPassiveAbility(this);
            return 1;
        }
        return 0;
    }
}

public class PassiveAbility_bloodArmor : PassiveAbilityBase
{
    // 피격시 공격자에게 출혈 1-2 부여
    public override void OnTakeDamageByAttack(BattleDiceBehavior atkDice, int dmg)
    {
        owner.battleCardResultLog?.SetPassiveAbility(this);
        int stack = RandomUtil.Range(1, 2);
        atkDice.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Bleeding, stack, owner);
    }
}


public class PassiveAbility_leader_of_family : PassiveAbilityBase
{
    // 공격 주사위로 피해를 주지 못하면 흐트러짐 피해를 5-10 만큼 받는다. 출혈 2 얻음

    private const int _breakDmgMin = 5;

    private const int _breakDmgMax = 10;

    private int BreakDmg => RandomUtil.Range(5, 10);

    public override void OnLoseParrying(BattleDiceBehavior behavior)
    {
        base.OnLoseParrying(behavior);
        if (IsAttackDice(behavior.Detail))
        {
            owner.TakeBreakDamage(BreakDmg, DamageType.Passive, owner);
            owner.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.Bleeding, 2, base.owner);
        }
    }

    public override void OnDrawParrying(BattleDiceBehavior behavior)
    {
        base.OnDrawParrying(behavior);
        if (IsAttackDice(behavior.Detail))
        {
            owner.TakeBreakDamage(BreakDmg, DamageType.Passive, owner);
            owner.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.Bleeding, 2, base.owner);
        }
    }
}

public class DiceCardSelfAbility_selfbleeding5powerupM2 : DiceCardSelfAbilityBase
{
    public static string Desc = "[사용 시] 자신의 출혈 5 당 모든 주사위 위력 +1 (최대 2)\n자신에게 출혈 10 부여";
    public override void OnUseCard()
    {
        base.owner.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.Bleeding, 10, base.owner);
        if (owner.bufListDetail.GetKewordBufStack(KeywordBuf.Bleeding) >= 5)
        {
            base.owner.currentDiceAction?.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
            {
                power = 1
            });
        }
        else if (owner.bufListDetail.GetKewordBufStack(KeywordBuf.Bleeding) >= 5)
        {
            base.owner.currentDiceAction?.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
            {
                power = 2
            });
        }
    }
}

public class PassiveAbility_pieceOfDream : PassiveAbilityBase
{
    // 막 시작 시 모든 사서 에게 속박 1 과 무장해제 1 부여
    public override void OnRoundStart()
    {
        foreach (BattleUnitModel alive in BattleObjectManager.instance.GetAliveList((owner.faction != Faction.Player) ? Faction.Player : Faction.Enemy))
        {
            alive.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Binding, 1);
            alive.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Disarm, 1);
        }
    }
}

public class PassiveAbility_throw_family : PassiveAbilityBase
{
        // 사망 시 다음 막 모든 사서에게 힘, 신속, 인내를 3 만큼 부여함
        public override void OnRoundStart()
        {
            owner.emotionDetail.SetMaxEmotionLevel(0);
        }

        public override void OnDie()
        {
            Faction faction = ((owner.faction == Faction.Enemy) ? Faction.Player : Faction.Enemy);
            List<BattleUnitModel> aliveList = BattleObjectManager.instance.GetAliveList(faction);
            int stack = 3;
            if (aliveList.Count == 0)
            {
                return;
            }
            foreach (BattleUnitModel item in aliveList)
            {
                item.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Strength, stack, owner);
                item.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Quickness, stack, owner);
                item.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Endurance, stack, owner);
            }
        }
}

public class PassiveAbility_atfer_thinkOf_adventure : PassiveAbilityBase
{
    // 사망 시 다음 막 에 꿈꾸는 모험의 기억 생성
}
public class DiceCardSelfAbility_selfbleeding5powerupM3 : DiceCardSelfAbilityBase
{
    public static string Desc = "[사용 시] 자신의 출혈 5 당 모든 주사위 위력 +1 (최대 3)\n자신에게 출혈 15 부여";
    public override void OnUseCard()
    {
        base.owner.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.Bleeding, 10, base.owner);
        if (owner.bufListDetail.GetKewordBufStack(KeywordBuf.Bleeding) >= 5)
        {
            base.owner.currentDiceAction?.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
            {
                power = 1
            });
        }
        else if (owner.bufListDetail.GetKewordBufStack(KeywordBuf.Bleeding) >= 10)
        {
            base.owner.currentDiceAction?.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
            {
                power = 2
            });
        }
        else if (owner.bufListDetail.GetKewordBufStack(KeywordBuf.Bleeding) >= 15)
        {
            base.owner.currentDiceAction?.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
            {
                power = 3
            });
        }
    }
}

public class DiceCardSelfAbility_selfbleeding20powerup : DiceCardSelfAbilityBase
{
    public static string Desc = "[사용 시] 자신에게 출혈 20 부여";
    public override void OnUseCard()
    {
        base.owner.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.Bleeding, 20, base.owner);
        
    }
}

public class DiceCardSelfAbility_endRoundBreak : DiceCardSelfAbilityBase
{
    public static string Desc = "[사용 시] 막 종료에 흐트러짐 상태가 된다";

    public class BattleUnitBuf_BreakSelf : BattleUnitBuf
    {
        public override void OnRoundEnd()
        {
            _owner.TakeBreakDamage(999, DamageType.ETC);
            Destroy();
        }
    }

    public override void OnUseCard()
    {
        base.owner.bufListDetail.AddBuf(new BattleUnitBuf_BreakSelf());
    }
}

public class PassiveAbility_ilLungImPhase_one : PassiveAbilityBase
{

    // 매 막 마다 손과 덱에 있는 모든 책장을 소멸시키고 사용할 책장을 손에 추가. 모든 책장의 비용이 0이 된다.
    public override int SpeedDiceNumAdder()
    {
        return 3;
    }

    public override void OnRoundStart()
    {
        int count = owner.allyCardDetail.GetHand().Count;
        int num = 6 - count;
        if (num > 0)
        {
            owner.allyCardDetail.DrawCards(num);
        }
    }
}

public class PassiveAbility_ilLungImPhase_two : PassiveAbilityBase
{

    // 매 막 마다 손과 덱에 있는 모든 책장을 소멸시키고 사용할 책장을 손에 추가. 모든 책장의 비용이 0이 된다.
    public override int SpeedDiceNumAdder()
    {
        return 5;
    }

    public override void OnRoundStart()
    {
        int count = owner.allyCardDetail.GetHand().Count;
        int num = 6 - count;
        if (num > 0)
        {
            owner.allyCardDetail.DrawCards(num);
        }
    }
}


public class PassiveAbility_bloodMaryGoAround : PassiveAbilityBase
{
    // 막 종료 시 체력이 50% 이하면 체력 20 감소 및 힘 2, 취약 2, 속박 2 얻음 
    public override void OnRoundEnd()
    {
        if (owner.hp <= (float)owner.MaxHp * 0.5f)
        {
            owner.LoseHp(20);
            owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Strength, 2, owner);
            owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Vulnerable, 2, owner);
            owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Binding, 2, owner);
        }
    }
}


public class PassiveAbility_bloodSee : PassiveAbilityBase
{
    // 모든 주사위 위력 - 1 , 막 시작 시 모든 적에게 출혈 3 부여
    public override void OnRoundStart()
    {
        List<BattleUnitModel> aliveList = BattleObjectManager.instance.GetAliveList((owner.faction != Faction.Player) ? Faction.Player : Faction.Enemy);
        foreach (BattleUnitModel item in aliveList)
        {
            item.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Bleeding, 3);
        }
    }

    public override void BeforeRollDice(BattleDiceBehavior behavior)
    {
        behavior.ApplyDiceStatBonus(new DiceStatBonus
        {
            power = -1
        });
    }

    private int _dmgreduction;

    // 공격 시 대상이 출혈 상태라면 피해량 + 2 피격 시 대상이 출혈 상태라면 피해량 -2 적 처치 시 최대 체력의 10% 만큼 체력을 회복한다.

    public override void BeforeGiveDamage(BattleDiceBehavior behavior)
    {
        BattleUnitModel target = behavior.card.target;
        if (target != null && target.bufListDetail.GetKewordBufStack(KeywordBuf.Bleeding) > 0)
        {
            owner.battleCardResultLog?.SetPassiveAbility(this);
            behavior.ApplyDiceStatBonus(new DiceStatBonus
            {
                dmg = 2
            });
        }
    }

    public override bool BeforeTakeDamage(BattleUnitModel attacker, int dmg)
    {
        _dmgreduction = 0;
        if (attacker != null && attacker.bufListDetail.GetKewordBufStack(KeywordBuf.Bleeding) > 0)
        {
            _dmgreduction = 2;
        }
        return base.BeforeTakeDamage(attacker, dmg);
    }

    public override int GetDamageReductionAll()
    {
        return _dmgreduction;
    }

    public override void OnKill(BattleUnitModel target)
    {
        int num = owner.MaxHp / 10;
        owner.ShowPassiveTypo(this);
        owner.RecoverHP(num);
    }
}

public class PassiveAbility_bloodCup : PassiveAbilityBase
{

    private const int FIRST_FROZEN_DREAM_ID = 3;

    private const int SECOND_FROZEN_DREAM_ID = 3;

    private const int THIRD_FROZEN_DREAM_ID = 3;

    private const int FOURTH_FROZEN_DREAM_ID = 3;

    private const int _HP_LIMIT = 100;

    private int _dmgReduction;

    public override bool isImmortal
    {
        get
        {
            if (!IsAliveAlly(5))
            {
                return IsAliveAlly(5);
            }
            return true;
        }
    }

    public override int SpeedDiceNumAdder()
    {
        if (PassiveAbility_1410014.IsBattleEnd())
        {
            return 0;
        }
        int num = 0;
        if (!IsAliveAlly(5))
        {
            num++;
        }
        if (!IsAliveAlly(5))
        {
            num++;
        }
        return num;
    }

    public override bool BeforeTakeDamage(BattleUnitModel attacker, int dmg)
    {
        if (PassiveAbility_1410014.IsBattleEnd())
        {
            return false;
        }
        if (IsAliveAlly(5) || IsAliveAlly(5) || IsAliveAlly(5) || IsAliveAlly(5))
        {
            _dmgReduction = 0;
            if (owner.hp - (float)dmg <= 100f)
            {
                _dmgReduction = (int)(100f - (owner.hp - (float)dmg));
            }
        }
        return base.BeforeTakeDamage(attacker, dmg);
    }

    public override int GetDamageReductionAll()
    {
        if (PassiveAbility_1410014.IsBattleEnd())
        {
            return 0;
        }
        int result = 0;
        if (IsAliveAlly(5) || IsAliveAlly(5) || IsAliveAlly(5) || IsAliveAlly(5))
        {
            result = ((!(owner.hp <= 100f)) ? _dmgReduction : 99999);
        }
        return result;
    }

    private bool IsAliveAlly(int id)
    {
        return BattleObjectManager.instance.GetAliveList(owner.faction).Exists((BattleUnitModel x) => x.UnitData.unitData.EnemyUnitId == id);
    }
}


public class DiceCardSelfAbility_AreaCardAdded : DiceCardSelfAbilityBase
{
    public static string Desc = "[사용 시] 이 책장의 모든 주사위 제거. 다음 막 사용할 수 있는 광역 공격 책장을 손에 추가.";

    public class BattleDiceCardBuf_lowel : BattleDiceCardBuf
    {
        public override void OnRoundStart()
        {
            _card.temporary = true;
        }
    }
    public override void OnUseCard()
    {
        base.owner.allyCardDetail.AddNewCard(9).AddBuf(new BattleDiceCardBuf_lowel());
        card.RemoveAllDice();
        card.card.exhaust = true;
    }
}

public class PassiveAbility_ProtectionSpear : PassiveAbilityBase
{
    public override void OnRoundStart()
    {
        if (owner.IsDead())
        {
            return;
        }
        foreach (BattleUnitModel alive in BattleObjectManager.instance.GetAliveList(owner.faction))
        {
            if (alive != owner && !alive.IsDead())
            {
                alive.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Protection, 1, owner);
                alive.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.BreakProtection, 1, owner);
            }
        }
    }
}

public class PassiveAbility_BuffSpear : PassiveAbilityBase
{
    public override void OnRoundStart()
    {
        foreach (BattleUnitModel alive in BattleObjectManager.instance.GetAliveList(Faction.Enemy))
            {
                    alive.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Quickness, 1, owner);  
            }
        }
    }

public class PassiveAbility_VulnerableSpear : PassiveAbilityBase
{
    public override void OnRoundStart()
    {
        if (owner.IsDead())
        {
            return;
        }
        foreach (BattleUnitModel alive in BattleObjectManager.instance.GetAliveList(owner.faction))
        {
            if (alive != owner && !alive.IsDead())
            {
                alive.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Vulnerable, 1, owner);
                alive.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Strength, 1, owner);
            }
        }
    }
}

public class PassiveAbility_StackSpear : PassiveAbilityBase
{
    private bool istakeDamaged;
    public override void OnRoundStart()
    {
        base.OnRoundStart();
        istakeDamaged = false;
    }

    public override void AfterTakeDamage(BattleUnitModel attacker, int dmg)
    {
        istakeDamaged = true;
    }

    public override void OnRoundEndTheLast()
    {
        if (!istakeDamaged)
        {
            owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Strength, 10, base.owner);
            owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Quickness, 10, base.owner);
            owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Protection, 10, base.owner);
            owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.BreakProtection, 10, base.owner);
            owner.RecoverHP(999);
        }
    }
}


public class PassiveAbility_bloodSpearBinding : PassiveAbilityBase
{
    public override void OnDie()
    {
        Faction faction = ((owner.faction == Faction.Enemy) ? Faction.Player : Faction.Enemy);
        List<BattleUnitModel> aliveList = BattleObjectManager.instance.GetAliveList(faction);
        int stack = 3;
        if (aliveList.Count == 0)
        {
            return;
        }
        foreach (BattleUnitModel item in aliveList)
        {
            item.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Binding, stack, owner);
        }
    }
}
