using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using LibEOSkill.GameSpecific;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using OriginTablets.Types;
using System.Runtime.InteropServices;

namespace LibEOSkill
{
  public abstract class Skill
  {
    /// <summary>
    /// A lookup table for what type of skill class to use for which game.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public static Dictionary<SkillModes, Type> GameSkillTypes = new Dictionary<SkillModes, Type>()
    {
      { SkillModes.EO3, typeof(EO3Skill) },
      { SkillModes.EO4, typeof(EO4Skill) },
      { SkillModes.EOU, typeof(EOUSkill) },
      { SkillModes.EO2U, typeof(EO2USkill) },
      { SkillModes.EO5, typeof(EO5Skill) },
      { SkillModes.EON, typeof(EONSkill) },
    };

    /// <summary>
    /// What game we're working with here.
    /// </summary>
    [JsonIgnore]
    public SkillModes Game { get; protected set; }

    [JsonProperty(Order = -100)]
    private int SkillId;

    private string _Name;
    /// <summary>
    /// The skill's name.
    /// </summary>
    [JsonProperty(Order = -99)]
    public string Name
    {
      get => _Name;
      set => _Name = value;
    }

    /// <summary>
    /// The skill's maximum level.
    /// </summary>
    [JsonProperty(Order = -98)]
    public byte MaxLevel { get; set; }

    /// <summary>
    /// The skill's targeting requirements.
    /// </summary>
    [JsonProperty(Order = -95)]
    public TargetStatus TargetStatus { get; protected set; }

    /// <summary>
    /// The skill's target type, i.e. what entities it targets.
    /// </summary>
    [JsonProperty(Order = -94)]
    public TargetTypes TargetType { get; protected set; } = TargetTypes.Single;

    /// <summary>
    /// What team the skill targets.
    /// </summary>
    [JsonProperty(Order = -93)]
    public TargetTeams TargetTeam { get; protected set; } = TargetTeams.Allies;

    /// <summary>
    /// In what context the skill can be used.
    /// </summary>
    [JsonProperty(Order = -92)]
    public UsableLocation UsableLocation { get; protected set; }

    /// <summary>
    /// What element(s) the modifier affects.
    /// </summary>
    [JsonProperty(Order = -89)]
    public Element ModifierElement { get; protected set; }

    /// <summary>
    /// The damage type of this skill.
    /// </summary>
    [JsonProperty(Order = -88)]
    public DamageElement DamageElement { get; protected set; }

    /// <summary>
    /// What this skill does with disables.
    /// </summary>
    [JsonProperty(Order = -87)]
    public InflictionStatus InflictionStatus { get; protected set; } = InflictionStatus.None;

    /// <summary>
    /// The disables that this skill operates on.
    /// </summary>
    [JsonProperty(Order = -86)]
    public Disables AssociatedDisables { get; protected set; }

    /// <summary>
    /// The skill's data sections.
    /// </summary>
    public DataSections DataSections { get; protected set; }

    /// <summary>
    /// Defines the targeting type of the skill.
    /// </summary>
    public enum TargetTypes
    {
      Single = 0x01,
      All = 0x02,
      MultiHitRandomTarget = 0x03,
      MultiHitOneTarget = 0x04,
      Self = 0x0A,
      Row = 0x10
    }

    /// <summary>
    /// Defines what teams a skill can target.
    /// </summary>
    public enum TargetTeams
    {
      Allies = 0x01,
      Enemies = 0x02,
      AllCombatants = 0x03
    }

    /// <summary>
    /// A lookup for how many bytes a skill consists of, by default, in all of the supported games.
    /// </summary>
    public static readonly Dictionary<SkillModes, int> GameSkillLengths = new Dictionary<SkillModes, int>()
    {
      { SkillModes.EO3, 0x178 },
      { SkillModes.EO4, 0x178 },
      { SkillModes.EOU, 0x298 },
      { SkillModes.EO2U, 0x408 },
      { SkillModes.EO5, 0x260 },
      { SkillModes.EON, 0x264 }
    };

    public Skill() 
    {
      DataSections = new DataSections();
    }

    public Skill(BinaryReader reader, int skillId, Table nameTable)
    {
      Name = nameTable[skillId];
      SkillId = skillId;
    }

    public abstract void Serialize(BinaryWriter writer);

    public override string ToString() => Name;
  }

  /// <summary>
  /// Since the games each have their own differences in skill format, what format we're working in
  /// needs to be defined.
  /// </summary>
  public enum SkillModes
  {
    EO3,
    EO4,
    EOU,
    EO2U,
    EO5,
    EON
  }

  /// <summary>
  /// Requirements about the target that must be fulfilled for them to be targeted. These are common types that are consistent
  /// through every EO3 engine game.
  /// </summary>
  public class TargetStatus : Bitfield
  {
    /// <summary>
    /// Targets only allies who are dead.
    /// </summary>
    public bool Dead
    {
      get => BitfieldData[0];
      set => BitfieldData[0] = value;
    }

    /// <summary>
    /// Targets only enemies that had a disable inflicted on them this turn.
    /// </summary>
    public bool InflictedEnemies
    {
      get => BitfieldData[1];
      set => BitfieldData[1] = value;
    }

    public bool Unknown1
    {
      get => BitfieldData[2];
      set => BitfieldData[2] = value;
    }

    /// <summary>
    /// Targets only entities with buffs. (Needs testing.)
    /// </summary>
    public bool HasBuff
    {
      get => BitfieldData[4];
      set => BitfieldData[4] = value;
    }

    /// <summary>
    /// Targeting ignores death, and allows use on any entity.
    /// </summary>
    public bool IgnoreDeath
    {
      get => BitfieldData[15];
      set => BitfieldData[15] = value;
    }

    public TargetStatus() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    public TargetStatus(JsonReader reader) : base(reader)
    {
      reader.Read(); reader.Read(); BitfieldData[0] = (bool)reader.Value; // Dead
      reader.Read(); reader.Read(); BitfieldData[1] = (bool)reader.Value; // InflictedEnemies
      reader.Read(); reader.Read(); BitfieldData[4] = (bool)reader.Value; // HasBuff
      reader.Read(); reader.Read(); BitfieldData[15] = (bool)reader.Value; // IgnoreDeath
      reader.Read(); // Object end.
    }

    /// <summary>
    /// For deserializing from a binary table.
    /// </summary>
    public TargetStatus(int bitfield) : base(bitfield) { }
  }

  /// <summary>
  /// Defines where the skill can be used.
  /// </summary>
  public class UsableLocation : Bitfield
  {
    /// <summary>
    /// Can the skill be used in town?
    /// </summary>
    public bool Town
    {
      get => BitfieldData[0];
      set => BitfieldData[0] = value;
    }

    /// <summary>
    /// Can the skill be used while in the labyrinth?
    /// </summary>
    public bool Dungeon
    {
      get => BitfieldData[1];
      set => BitfieldData[1] = value;
    }

    /// <summary>
    /// Can the skill be used in battle?
    /// </summary>
    public bool Battle
    {
      get => BitfieldData[2];
      set => BitfieldData[2] = value;
    }

    public UsableLocation() { }

    /// <summary>
    /// For deserializing from a JSON file.
    /// </summary>
    /// <param name="reader"></param>
    public UsableLocation(JsonReader reader) : base(reader)
    {
      reader.Read(); reader.Read(); BitfieldData[0] = (bool)reader.Value;
      reader.Read(); reader.Read(); BitfieldData[1] = (bool)reader.Value;
      reader.Read(); reader.Read(); BitfieldData[2] = (bool)reader.Value;
      reader.Read(); // Object end.
    }

    public UsableLocation(int bitfield) : base(bitfield) { }
  }

  /// <summary>
  /// The conditions that need to be fulfilled for a skill to be usable. These are common types that are consistent
  /// through every EO3 engine game.
  /// </summary>
  public class UseRequirements : Bitfield
  {
    /// <summary>
    /// Can only be used if the user's head is not bound.
    /// </summary>
    [JsonProperty(Order = -200)]
    public bool Head
    {
      get => BitfieldData[0];
      set => BitfieldData[0] = value;
    }

    /// <summary>
    /// Can only be used if the user's arms are not bound.
    /// </summary>
    [JsonProperty(Order = -199)]
    public bool Arms
    {
      get => BitfieldData[1];
      set => BitfieldData[1] = value;
    }

    /// <summary>
    /// Can only be used if the user's legs are not bound.
    /// </summary>
    [JsonProperty(Order = -198)]
    public bool Legs
    {
      get => BitfieldData[2];
      set => BitfieldData[2] = value;
    }

    public UseRequirements() { }

    public UseRequirements(int bitfield) : base(bitfield)
    {

    }
  }

  /// <summary>
  /// Defines the disables this skill inflicts/removes/operates on.
  /// </summary>
  public class Disables : Bitfield
  {
    public bool Death
    {
      get => BitfieldData[0];
      set => BitfieldData[0] = value;
    }
    public bool Petrification
    {
      get => BitfieldData[1];
      set => BitfieldData[1] = value;
    }
    public bool Sleep
    {
      get => BitfieldData[2];
      set => BitfieldData[2] = value;
    }
    public bool Panic
    {
      get => BitfieldData[3];
      set => BitfieldData[3] = value;
    }
    public bool Plague
    {
      get => BitfieldData[4];
      set => BitfieldData[4] = value;
    }
    public bool Poison
    {
      get => BitfieldData[5];
      set => BitfieldData[5] = value;
    }
    public bool Blind
    {
      get => BitfieldData[6];
      set => BitfieldData[6] = value;
    }
    public bool Curse
    {
      get => BitfieldData[7];
      set => BitfieldData[7] = value;
    }
    public bool Paralysis
    {
      get => BitfieldData[8];
      set => BitfieldData[8] = value;
    }
    public bool Stun
    {
      get => BitfieldData[9];
      set => BitfieldData[9] = value;
    }
    public bool HeadBind
    {
      get => BitfieldData[10];
      set => BitfieldData[10] = value;
    }
    public bool ArmBind
    {
      get => BitfieldData[11];
      set => BitfieldData[11] = value;
    }
    public bool LegBind
    {
      get => BitfieldData[12];
      set => BitfieldData[12] = value;
    }
    public bool Fear
    {
      get => BitfieldData[13];
      set => BitfieldData[13] = value;
    }

    public Disables() { }

    public Disables(int bitfield) : base(bitfield)
    {

    }
  }

  /// <summary>
  /// The types of interactions with disables a skill can have.
  /// </summary>
  public enum InflictionStatus
  {
    /// <summary>
    /// The skill does not do anything with disables.
    /// </summary>
    None = 0x0,
    /// <summary>
    /// The skill attempts to inflict a disable.
    /// </summary>
    Inflicts = 0x1,
    /// <summary>
    /// The skill removes a disable.
    /// </summary>
    Cures = 0x2,
    /// <summary>
    /// The skill spreads the target's disables to other enemies.
    /// </summary>
    SpreadsToEnemies = 0x3,
    /// <summary>
    /// The skill attempts to transfer disables from the user to a target.
    /// </summary>
    Transfer = 0x4
  }
}
