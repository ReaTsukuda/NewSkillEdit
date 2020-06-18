using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibEOSkill
{
  /// <summary>
  /// A part of the skill that defines data for a specific function, i.e. damage or healing power.
  /// </summary>
  public class DataSection
  {
    /// <summary>
    /// What type of data this data section contains.
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// The actual values for this part of the skill.
    /// </summary>
    public List<int> Values { get; } = new List<int>();

    /// <summary>
    /// For deserializing from a skills JSON.
    /// </summary>
    [JsonConstructor]
    public DataSection() 
    {
    }

    /// <summary>
    /// For deserializing from playerskilltable.tbl.
    /// </summary>
    public DataSection(BinaryReader reader, SkillModes game)
    {
      // Reduce ths ize of Values by 1 if we're dealing with a game with "level 0." We can just auto-write level 1 to the dummy level slot instead of needing the user to manually specify it.
      int numberOfValues = DataSectionTypeLookup[game] == DataSectionTypes.DummyLevel ? NumberOfLevelsLookup[game] - 1 : NumberOfLevelsLookup[game];
      Type = reader.ReadInt32();
      for (int valueIndex = 0; valueIndex < numberOfValues; valueIndex += 1)
      {
        Values.Add(reader.ReadInt32());
      }
    }

    public void Serialize(BinaryWriter writer)
    {
      writer.Write(Type);
      foreach (int value in Values) { writer.Write(value); }
    }

    /// <summary>
    /// An enum for differentiating between skills that do not have a "level 0" (EO3, EO4, EOU, EO2U) and games that do
    /// (EO5, EON).
    /// </summary>
    public enum DataSectionTypes
    { 
      NoDummyLevel,
      DummyLevel
    }

    /// <summary>
    /// A lookup for programatically determining if the current game type uses a dummy level or not.
    /// </summary>
    public static readonly Dictionary<SkillModes, DataSectionTypes> DataSectionTypeLookup = new Dictionary<SkillModes, DataSectionTypes>()
    {
      { SkillModes.EO3, DataSectionTypes.NoDummyLevel },
      { SkillModes.EO4, DataSectionTypes.NoDummyLevel },
      { SkillModes.EOU, DataSectionTypes.NoDummyLevel },
      { SkillModes.EO2U, DataSectionTypes.NoDummyLevel },
      { SkillModes.EO5, DataSectionTypes.DummyLevel },
      { SkillModes.EON, DataSectionTypes.DummyLevel },
    };

    /// <summary>
    /// How many levels comprise a data section in each game. Note that although a skill's max level can be less than
    /// the number indicated here, it must still have data entries for levels it cannot attain.
    /// </summary>
    public static readonly Dictionary<SkillModes, int> NumberOfLevelsLookup = new Dictionary<SkillModes, int>()
    {
      { SkillModes.EO3, 10 },
      { SkillModes.EO4, 10 },
      { SkillModes.EOU, 15 }, // 10 base levels, plus 5 for using boost when skills are level 6 and beyond.
      { SkillModes.EO2U, 20 }, // 10 base levels, 10 grimoire overlevels.
      { SkillModes.EO5, 11 }, // Includes "level 0."
      { SkillModes.EON, 11 }, // Includes "level 0."
    };

    /// <summary>
    /// How many data sections a skill can have.
    /// </summary>
    public static readonly Dictionary<SkillModes, int> NumberOfDataSectionsLookup = new Dictionary<SkillModes, int>()
    {
      { SkillModes.EO3, 8 },
      { SkillModes.EO4, 8 },
      { SkillModes.EOU, 10 },
      { SkillModes.EO2U, 12 },
      { SkillModes.EO5, 12 },
      { SkillModes.EON, 12 }
    };
  }

  /// <summary>
  /// The actual collection of data sections that make up a skill's data.
  /// </summary>
  public class DataSections : List<DataSection>
  {
    /// <summary>
    /// For deserializing data sections from a JSON file.
    /// </summary>
    public DataSections() { }

    /// <summary>
    /// For deserializing data sections from playerskilltable.tbl.
    /// </summary>
    public DataSections(BinaryReader reader, SkillModes game)
    {
      for (int dataSectionIndex = 0; dataSectionIndex < DataSection.NumberOfDataSectionsLookup[game]; dataSectionIndex += 1)
      {
        int type = reader.ReadInt32();
        // Skip over the rest of this if this is a null data section.
        if (type == 0x0) { reader.ReadBytes(4 * DataSection.NumberOfLevelsLookup[game]); }
        else
        {
          // Jump the reader back to where we were, and construct a new data section.
          reader.BaseStream.Position -= 4;
          Add(new DataSection(reader, game));
        }
      }
    }

    public void Serialize(BinaryWriter writer, SkillModes game)
    {
      foreach (var section in this) { section.Serialize(writer); }
      int numberOfNullSections = DataSection.NumberOfDataSectionsLookup[game] - Count;
      for (int nullSection = 0; nullSection < numberOfNullSections; nullSection += 1)
      {
        for (int place = 0; place < 1 + DataSection.NumberOfLevelsLookup[game]; place += 1)
        {
          writer.Write(0);
        }
      }
    }
  }
}
