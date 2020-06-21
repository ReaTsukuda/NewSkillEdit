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
    /// What conditions need to be fulfilled for the skill to be usable, using the requirements defined for EO3.
    /// </summary>
    public new EO3UseRequirements UseRequirements { get; protected set; }

    /// <summary>
    /// Special flags that affect skill behavior, using the types defined for EO3.
    /// </summary>
    public new EO3SkillFlags SkillFlags { get; protected set; }

    /// <summary>
    /// For deserialization from a JSON file.
    /// </summary>
    public EO3Skill() 
    {
      UseRequirements = new EO3UseRequirements();
      SkillFlags = new EO3SkillFlags();
    }

    /// <summary>
    /// For deserialization from a binary table.
    /// </summary>
    public EO3Skill(BinaryReader reader, int skillId, Table nameTable) : base(reader, skillId, nameTable)
    {
      Game = SkillModes.EO3;
      MaxLevel = reader.ReadByte();
      Type = reader.ReadByte();
      UseRequirements = new EO3UseRequirements(reader.ReadUInt16());
      TargetStatus = new TargetStatus(reader.ReadUInt16());
      TargetType = reader.ReadByte();
      TargetTeam = reader.ReadByte();
      UsableLocation = new UsableLocation(reader.ReadByte());
      ModifierStatus = reader.ReadByte();
      ModifierType = reader.ReadByte();
      reader.ReadByte(); // Unknown.
      ModifierElement = new Element(reader.ReadUInt16());
      DamageElement = new DamageElement(reader.ReadUInt16());
      InflictionStatus = reader.ReadUInt16();
      AssociatedDisables = new Disables(reader.ReadUInt16());
      SkillFlags = new EO3SkillFlags(reader.ReadInt32());
      DataSections = new DataSections(reader, Game);
    }

    /// <summary>
    /// For serialization back to a binary table.
    /// </summary>
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

    /// <summary>
    /// For deserialization from a JSON file.
    /// </summary>
    public EO3SkillFlags() : base() { }

    /// <summary>
    /// For deserialization from a binary table.
    /// </summary>
    public EO3SkillFlags(int bitfield) : base(bitfield) { }
  }
}
