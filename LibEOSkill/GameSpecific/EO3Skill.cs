using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OriginTablets.Types;

namespace LibEOSkill.GameSpecific
{
  /// <summary>
  /// A skill from EO3.
  /// </summary>
  public class EO3Skill : Skill
  {
    /// <summary>
    /// The skill's type, using the types defined for EO3.
    /// </summary>
    public EO3SkillTypes Type { get; set; }

    /// <summary>
    /// What conditions need to be fulfilled for the skill to be usable, using the requirements defined for EO3.
    /// </summary>
    public EO3UseRequirements UseRequirements { get; protected set; }

    /// <summary>
    /// What this skill's modifier status is, using the types defined for EO3.
    /// </summary>
    public EO3ModifierStatuses ModifierStatus { get; set; }

    /// <summary>
    /// What type of modifier this skill has, using the types defined for EO3.
    /// </summary>
    public EO3ModifierTypes ModifierType { get; set; }

    /// <summary>
    /// Special flags that affect skill behavior, using the types defined for EO3.
    /// </summary>
    public EO3SkillFlags SkillFlags { get; protected set; }

    public EO3Skill() 
    {
      UseRequirements = new EO3UseRequirements();
      SkillFlags = new EO3SkillFlags();
    }

    /// <summary>
    /// For constructing an EO3Skill by deserializing a portion of playerskilltable.
    /// </summary>
    public EO3Skill(BinaryReader reader, int skillId, Table nameTable) : base(reader, skillId, nameTable)
    {
      long sob = reader.BaseStream.Position;
      Game = SkillModes.EO3;
      MaxLevel = reader.ReadByte();
      Type = (EO3SkillTypes)reader.ReadByte();
      UseRequirements = new EO3UseRequirements(reader.ReadUInt16());
      TargetStatus = new TargetStatus(reader.ReadUInt16());
      TargetType = (TargetTypes)reader.ReadByte();
      TargetTeam = (TargetTeams)reader.ReadByte();
      UsableLocation = new UsableLocation(reader.ReadByte());
      ModifierStatus = (EO3ModifierStatuses)reader.ReadByte();
      ModifierType = (EO3ModifierTypes)reader.ReadByte();
      reader.ReadByte(); // Unknown.
      ModifierElement = new Element(reader.ReadUInt16());
      DamageElement = new DamageElement(reader.ReadUInt16());
      InflictionStatus = (InflictionStatus)reader.ReadUInt16();
      AssociatedDisables = new Disables(reader.ReadUInt16());
      SkillFlags = new EO3SkillFlags(reader.ReadInt32());
      DataSections = new DataSections(reader, Game);
    }

    public override void Serialize(BinaryWriter writer)
    {
      writer.Write(MaxLevel);
      writer.Write((byte)Type);
      writer.Write((ushort)UseRequirements.BinaryValue);
      writer.Write((ushort)TargetStatus.BinaryValue);
      writer.Write((byte)TargetType);
      writer.Write((byte)TargetTeam);
      writer.Write((byte)UsableLocation.BinaryValue);
      writer.Write((byte)ModifierStatus);
      writer.Write((byte)ModifierType);
      writer.Write((byte)0); // Unknown.
      writer.Write((ushort)ModifierElement.BinaryValue);
      writer.Write((ushort)DamageElement.BinaryValue);
      writer.Write((ushort)InflictionStatus);
      writer.Write((ushort)AssociatedDisables.BinaryValue);
      writer.Write(SkillFlags.BinaryValue);
      DataSections.Serialize(writer, SkillModes.EO3);
    }
  }

  /// <summary>
  /// Defines the various types a skill can have in EO3.
  /// </summary>
  public enum EO3SkillTypes
  {
    /// <summary>
    /// An attack skill that uses the STR damage formula.
    /// </summary>
    STRAttack = 0x00,
    /// <summary>
    /// An attack skill that uses the TEC damage formula.
    /// </summary>
    TECAttack = 0x01,
    /// <summary>
    /// A passive that is checked when the user is attacking.
    /// </summary>
    AttackPassive = 0x02,
    /// <summary>
    /// A skill that restores a fixed amount of HP.
    /// </summary>
    FixedHPRestore = 0x03,
    /// <summary>
    /// A skill that restores a fixed amount of TP.
    /// </summary>
    FixedTPRestore = 0x04,
    /// <summary>
    /// A skill that restores a fixed amount of both HP and TP.
    /// </summary>
    FixedCombinedRestore = 0x05,
    /// <summary>
    /// A skill that restores a variable amount of HP.
    /// </summary>
    HPRestore = 0x06,
    /// <summary>
    /// A skill that restores a variable amount of TP.
    /// </summary>
    TPRestore = 0x07,
    Unknown1 = 0x09,
    /// <summary>
    /// A skill that removes ailments or binds.
    /// </summary>
    CuresAilmentOrBind = 0x0A,
    /// <summary>
    /// Duplicates the functionality of CuresAilmentOrBind.
    /// </summary>
    CuresAilmentOrBindDuplicate = 0x0B,
    /// <summary>
    /// Used on revival skills, though it may not be strictly necessary?
    /// </summary>
    Revive = 0x0D,
    /// <summary>
    /// A skill that attempts to inflict a disable without attacking.
    /// </summary>
    DisableWithoutAttack = 0x0F,
    /// <summary>
    /// A skill that applies a modifier without attacking.
    /// </summary>
    ModifierWithoutAttack = 0x10,
    /// <summary>
    /// Used on the skill that Discharge links to.
    /// </summary>
    DischargeLink = 0x11,
    /// <summary>
    /// A skill that applies the Knighthood effect to its targets.
    /// </summary>
    Knighthood = 0x12,
    /// <summary>
    /// A skill that applies an attack element modifier to its targets.
    /// </summary>
    AttackElementModifier = 0x13,
    /// <summary>
    /// A charge skill.
    /// </summary>
    Charge = 0x15,
    /// <summary>
    /// A skill that creates a clone entity of the user.
    /// </summary>
    NinpoDouble = 0x17,
    /// <summary>
    /// A skill that creates an uncontrollable enmity dummy with the user's name.
    /// </summary>
    NinpoMirage = 0x18,
    /// <summary>
    /// A skill that summons a bot entity.
    /// </summary>
    Bot = 0x19,
    /// <summary>
    /// Used on some skills that relate to poison damage.
    /// </summary>
    PoisonDamageLink = 0x1A,
    /// <summary>
    /// A delayed attack skill, i.e. Cloudbuster.
    /// </summary>
    DelayedAttack = 0x1B,
    /// <summary>
    /// A skill that links to another skill when a trigger is fired.
    /// </summary>
    LinkOnTrigger = 0x1C,
    /// <summary>
    /// A skill that links to another skill when a condition is fulfilled.
    /// </summary>
    ConditionalSkill = 0x1D,
    /// <summary>
    /// A skill that reduces damage to ally entities, such as most of Hoplite's skillset.
    /// </summary>
    Guard = 0x1E,
    /// <summary>
    /// A skill that counters attacks.
    /// </summary>
    Counter = 0x1F,
    /// <summary>
    /// A skill that summons a beast.
    /// </summary>
    WildlingSummon = 0x20,
    /// <summary>
    /// A skill that dismisses a summoned beast.
    /// </summary>
    DismissSummon = 0x21,
    /// <summary>
    /// A skill that applies a damage bonus when using a particular weapon type.
    /// </summary>
    WeaponMastery = 0x22,
    /// <summary>
    /// A skill that allows the party to escape from battle.
    /// </summary>
    Flee = 0x23,
    /// <summary>
    /// Regal Radiance's skill type.
    /// </summary>
    RegalRadiance = 0x24,
    /// <summary>
    /// Used on Call Allies and Submerged Move.
    /// </summary>
    Unknown_AffectsBattleEntities = 0x25,
    /// <summary>
    /// HP Cannon's skill type.
    /// </summary>
    HPCannon = 0x26,
    /// <summary>
    /// Marsh Dive's skill type.
    /// </summary>
    MarshDive = 0x27,
    /// <summary>
    /// An attack skill that uses the STR damage formula and attacks the target's TP.
    /// </summary>
    STRAttackDamagesTP = 0x28,
    /// <summary>
    /// An attack skill whose damage is based on the last instance of damage dealt in the battle.
    /// </summary>
    AttackBasedOnLastDamageInstance = 0x2A,
    /// <summary>
    /// Ruin Caller's Regenerate skill type.
    /// </summary>
    RuinCallerRegenerate = 0x2B,
    /// <summary>
    /// A skill that redirects damage instances from one entity to another.
    /// </summary>
    Cover = 0x2C,
    /// <summary>
    /// A skill that causes the user to redirect damage directed at allies at a certain HP threshold to themself.
    /// </summary>
    RedirectsDamageAgainstLowHPAllies = 0x2D,
    /// <summary>
    /// A skill that makes targets recover from ailments at the end of their turn.
    /// </summary>
    RecoverFromAilmentsAtEndOfTurn = 0x2E,
    Unknown2 = 0x2F,
    Unknown3 = 0x30,
    /// <summary>
    /// A skill that retaliates against attacks with its own attack.
    /// </summary>
    Bait = 0x31,
    Unknown4 = 0x32,
    /// <summary>
    /// A skill that gives a chance to nullify attacks against the user.
    /// </summary>
    ChanceToBlockSkill = 0x33,
    /// <summary>
    /// The skill type for Gatekeeper's Focus skill.
    /// </summary>
    GatekeeperReassemble = 0x34,
    /// <summary>
    /// The skill type for Gatekeeper's Disperse skill.
    /// </summary>
    GatekeeperDisassemble = 0x35,
    Unknown_Summon = 0x36,
    /// <summary>
    /// Lay Egg's skill type.
    /// </summary>
    LayEgg = 0x37,
    Unknown_Change = 0x38,
    /// <summary>
    /// Causes targeted player units to forcibly have their rows switched.
    /// </summary>
    ForcedRowSwitch = 0x39,
    /// <summary>
    /// A passive that makes the user recover TP if they are bound.
    /// </summary>
    RestoreTPIfBound = 0x3A,
    Unknown5 = 0x3D,
    Unknown6 = 0x3E,
    Unknown7 = 0x3F,
    Unknown8 = 0x40,
    Unknown9 = 0x41,
    Unknown10 = 0x42,
    // Below here are skill types for skills that are not used in battle.
    /// <summary>
    /// Grants the user some uses of Chop.
    /// </summary>
    Chop = 0x43,
    /// <summary>
    /// Grants the user some uses of Take.
    /// </summary>
    Take = 0x44,
    /// <summary>
    /// Grants the user some uses of Mine.
    /// </summary>
    Mine = 0x45,
    /// <summary>
    /// A skill that affects the danger value of tiles.
    /// </summary>
    DangerModifier = 0x46,
    /// <summary>
    /// A skill that sets danger values of tiles to 0.
    /// </summary>
    SetDangerToZero = 0x47,
    /// <summary>
    /// A skill that reduces damage taken from damage tiles.
    /// </summary>
    DamageTileReduction = 0x48,
    /// <summary>
    /// A skill that nullifies both damage tiles and mud tiles.
    /// </summary>
    IgnoreDamageAndMudTiles = 0x49,
    /// <summary>
    /// A skill that reveals FOEs on the map.
    /// </summary>
    RevealFOEs = 0x4A,
    /// <summary>
    /// A duplicate of RevealFOEs.
    /// </summary>
    RevealFOEsDuplicate = 0x4B,
    /// <summary>
    /// A skill that increases the player's chance of getting a preemptive attack.
    /// </summary>
    IncreasePreemptiveChance = 0x4C,
    /// <summary>
    /// A skill that increases the user's uses of Chop, Take, and Mine.
    /// </summary>
    AllGatherTypesUp = 0x4F,
    /// <summary>
    /// A skill that returns the party to town.
    /// </summary>
    ReturnToTown = 0x50,
    /// <summary>
    /// A skill that advances time.
    /// </summary>
    PassTime = 0x52,
    Unknown_CampMastery = 0x53
  };

  /// <summary>
  /// Defines the weapon and/or body parts needed to use a skill. When serializing to JSON, these are organized
  /// based on in-game order, hence the scattered JsonProperty orders.
  /// </summary>
  public class EO3UseRequirements : UseRequirements
  {
    /// <summary>
    /// Can only be used if the user is equipped with a tome.
    /// </summary>
    [JsonProperty(Order = -92)]
    public bool Tome
    {
      get => BitfieldData[3];
      set => BitfieldData[3] = value;
    }

    /// <summary>
    /// Can only be used if the user is equipped with a rapier.
    /// </summary>
    [JsonProperty(Order = -99)]
    public bool Rapier
    {
      get => BitfieldData[4];
      set => BitfieldData[4] = value;
    }

    /// <summary>
    /// Can only be used if the user is equipped with a sword.
    /// </summary>
    [JsonProperty(Order = -100)]
    public bool Sword
    {
      get => BitfieldData[5];
      set => BitfieldData[5] = value;
    }

    /// <summary>
    /// Can only be used if the user is equipped with a club.
    /// </summary>
    [JsonProperty(Order = -94)]
    public bool Club
    {
      get => BitfieldData[6];
      set => BitfieldData[6] = value;
    }

    /// <summary>
    /// Can only be used if the user is equipped with a shield.
    /// </summary>
    [JsonProperty(Order = -91)]
    public bool Shield
    {
      get => BitfieldData[7];
      set => BitfieldData[7] = value;
    }

    /// <summary>
    /// Can only be used if the user is equipped with a spear.
    /// </summary>
    [JsonProperty(Order = -98)]
    public bool Spear
    {
      get => BitfieldData[8];
      set => BitfieldData[8] = value;
    }

    /// <summary>
    /// Can only be used if the user is unarmed.
    /// </summary>
    [JsonProperty(Order = -90)]
    public bool Unarmed
    {
      get => BitfieldData[9];
      set => BitfieldData[9] = value;
    }

    /// <summary>
    /// Can only be used if the user is equipped with a gun.
    /// </summary>
    [JsonProperty(Order = -93)]
    public bool Gun
    {
      get => BitfieldData[10];
      set => BitfieldData[10] = value;
    }

    /// <summary>
    /// Can only be used if the user is equipped with a crossbow.
    /// </summary>
    [JsonProperty(Order = -95)]
    public bool Crossbow
    {
      get => BitfieldData[11];
      set => BitfieldData[11] = value;
    }

    /// <summary>
    /// Can only be used if the user is equipped with a katana.
    /// </summary>
    [JsonProperty(Order = -96)]
    public bool Katana
    {
      get => BitfieldData[12];
      set => BitfieldData[12] = value;
    }

    /// <summary>
    /// Can only be used if the user is equipped with a knife.
    /// </summary>
    [JsonProperty(Order = -97)]
    public bool Knife
    {
      get => BitfieldData[13];
      set => BitfieldData[13] = value;
    }

    /// <summary>
    /// Can only be used if the user is afflicted with any bind.
    /// </summary>
    public bool AnyBind
    {
      get => BitfieldData[14];
      set => BitfieldData[14] = value;
    }

    /// <summary>
    /// Can only be used if the user is fully bound.
    /// </summary>
    public bool FullyBound
    {
      get => BitfieldData[15];
      set => BitfieldData[15] = value;
    }

    public EO3UseRequirements() : base() { }

    public EO3UseRequirements(int bitfield) : base(bitfield)
    {
    }
  }

  /// <summary>
  /// Determines what this skill does with regards to modifiers; does it apply a buff? Does it apply a debuff? Does it remove them?
  /// </summary>
  public enum EO3ModifierStatuses
  {
    None = 0x0,
    Buff = 0x1,
    Debuff = 0x2,
    PurgeBuffs = 0x3,
    PurgeDebuffs = 0x4
  }

  /// <summary>
  /// Defines the types of modifiers. This is only used for determining buff overwrites.
  /// </summary>
  public enum EO3ModifierTypes
  {
    None = 0x0,
    Attack = 0x1,
    Status = 0x2,
    Evasion = 0x3,
    AttackDefense = 0x4,
    Defense = 0x5,
    Regen = 0x6,
    Reduction = 0x7,
    MaximumHP = 0x8,
    Accuracy = 0x9,
    Enmity = 0xA
  }

  /// <summary>
  /// Special flags that affect skill behavior.
  /// </summary>
  public class EO3SkillFlags : Bitfield
  {
    public bool Unknown0
    {
      get => BitfieldData[0];
      set => BitfieldData[0] = value;
    }

    /// <summary>
    /// Is the skill a limit skill?
    /// </summary>
    public bool LimitSkill
    {
      get => BitfieldData[1];
      set => BitfieldData[1] = value;
    }

    public bool Unknown2
    {
      get => BitfieldData[2];
      set => BitfieldData[2] = value;
    }

    public bool Unknown3
    {
      get => BitfieldData[3];
      set => BitfieldData[3] = value;
    }

    public bool Unknown4
    {
      get => BitfieldData[4];
      set => BitfieldData[4] = value;
    }

    /// <summary>
    /// Is the skill a link skill?
    /// </summary>
    public bool LinkSkill
    {
      get => BitfieldData[5];
      set => BitfieldData[5] = value;
    }

    public bool Unknown6
    {
      get => BitfieldData[6];
      set => BitfieldData[6] = value;
    }

    public bool Unknown7
    {
      get => BitfieldData[7];
      set => BitfieldData[7] = value;
    }

    public bool Unknown8
    {
      get => BitfieldData[8];
      set => BitfieldData[8] = value;
    }

    public bool Unknown9
    {
      get => BitfieldData[9];
      set => BitfieldData[9] = value;
    }

    /// <summary>
    /// Is the skill a class-exclusive skill?
    /// </summary>
    public bool ClassSkill
    {
      get => BitfieldData[10];
      set => BitfieldData[10] = value;
    }

    public bool Unknown11
    {
      get => BitfieldData[11];
      set => BitfieldData[11] = value;
    }

    /// <summary>
    /// Is the skill a common skill?
    /// </summary>
    public bool CommonSkill
    {
      get => BitfieldData[12];
      set => BitfieldData[12] = value;
    }

    /// <summary>
    /// Is the skill a passive?
    /// </summary>
    public bool Passive
    {
      get => BitfieldData[13];
      set => BitfieldData[13] = value;
    }

    public bool Unknown14
    {
      get => BitfieldData[14];
      set => BitfieldData[14] = value;
    }

    public bool Unknown15
    {
      get => BitfieldData[15];
      set => BitfieldData[15] = value;
    }

    public EO3SkillFlags() : base() { }

    public EO3SkillFlags(int bitfield) : base(bitfield)
    {

    }
  }
}
