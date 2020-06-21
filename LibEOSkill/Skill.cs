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

    /// <summary>
    /// This isn't used in the code itself, but is left here for reference purposes in the outputted JSON files.
    /// </summary>
    [JsonProperty(Order = -101)]
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable IDE0052 // Remove unread private members
    private int SkillId;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore IDE0044 // Add readonly modifier

    /// <summary>
    /// The skill's type.
    /// </summary>
    [JsonProperty(Order = -100)]
    public int Type { get; set; }

    /// <summary>
    /// The skill's name.
    /// </summary>
    [JsonProperty(Order = -99)]
    public string Name { get; set; }

    /// <summary>
    /// The skill's maximum level.
    /// </summary>
    [JsonProperty(Order = -98)]
    public byte MaxLevel { get; set; }

    [JsonProperty(Order = -98)]
    public Bitfield UseRequirements { get; protected set; }

    /// <summary>
    /// The skill's targeting requirements.
    /// </summary>
    [JsonProperty(Order = -95)]
    public TargetStatus TargetStatus { get; protected set; }

    /// <summary>
    /// The skill's target type, i.e. what entities it targets.
    /// </summary>
    [JsonProperty(Order = -94)]
    public int TargetType { get; protected set; }

    /// <summary>
    /// What team the skill targets.
    /// </summary>
    [JsonProperty(Order = -93)]
    public int TargetTeam { get; protected set; }

    /// <summary>
    /// In what context the skill can be used.
    /// </summary>
    [JsonProperty(Order = -92)]
    public UsableLocation UsableLocation { get; protected set; }

    /// <summary>
    /// How this skill interacts with modifiers—applies a buff, applies a debuff, etc.
    /// </summary>
    [JsonProperty(Order = -91)]
    public int ModifierStatus { get; set; }

    /// <summary>
    /// What type of modifier this skill works with.
    /// </summary>
    [JsonProperty(Order = -90)]
    public int ModifierType { get; set; }

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
    public int InflictionStatus { get; set; }

    /// <summary>
    /// The disables that this skill operates on.
    /// </summary>
    [JsonProperty(Order = -86)]
    public Disables AssociatedDisables { get; protected set; }

    public Bitfield SkillFlags { get; protected set; }

    /// <summary>
    /// The skill's data sections.
    /// </summary>
    public DataSections DataSections { get; protected set; }

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

    /// <summary>
    /// For serialization into a JSON file.
    /// </summary>
    public Skill() { DataSections = new DataSections(); }

    /// <summary>
    /// For deserialization from a table file.
    /// </summary>
    /// <param name="_">The BinaryReader that is passed to the derived class that calls this.</param>
    /// <param name="skillId">The skill's ID internally.</param>
    /// <param name="nameTable">A deserialized playerskillnametable.tbl.</param>
    public Skill(BinaryReader _, int skillId, Table nameTable)
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

    /// <summary>
    /// For serialization into a JSON file.
    /// </summary>
    public TargetStatus() { }

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

    /// <summary>
    /// For serializing into a JSON file.
    /// </summary>
    public UsableLocation() { }

    /// <summary>
    /// For deserializing from a binary table.
    /// </summary>
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

    /// <summary>
    /// For serialization into a JSON file.
    /// </summary>
    public UseRequirements() { }

    /// <summary>
    /// For deserialization from a binary table.
    /// </summary>
    /// <param name="bitfield"></param>
    public UseRequirements(int bitfield) : base(bitfield) { }
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

    /// <summary>
    /// For deserializing from a JSON file.
    /// </summary>
    public Disables() { }

    /// <summary>
    /// For deserialization from a binary table.
    /// </summary>
    public Disables(int bitfield) : base(bitfield) { }
  }
}
