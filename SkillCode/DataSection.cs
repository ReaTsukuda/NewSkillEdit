using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SkillCode
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
    public ValueStore Values { get; }

    /// <summary>
    /// For deserializing from a skills JSON.
    /// </summary>
    [JsonConstructor]
    public DataSection() { }

    /// <summary>
    /// For deserializing from playerskilltable.tbl.
    /// </summary>
    public DataSection(BinaryReader reader, SkillModes game)
    {
      // Reduce ths ize of Values by 1 if we're dealing with a game with "level 0." We can just auto-write level 1 to the dummy level slot instead of needing the user to manually specify it.
      int numberOfValues = DataSectionTypeLookup[game] == DataSectionTypes.DummyLevel ? NumberOfLevelsLookup[game] - 1 : NumberOfLevelsLookup[game];
      Values = new ValueStore(numberOfValues);
      Type = reader.ReadInt32();
      for (int valueIndex = 0; valueIndex < numberOfValues; valueIndex += 1)
      {
        Values[valueIndex] = reader.ReadInt32();
      }
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

    /// <summary>
    /// A specialized store for DataSection values.
    /// </summary>
    public class ValueStore : IList<int>
    {
      /// <summary>
      /// Where the values are stored internally.
      /// </summary>
      private int[] Values { get; }

      public int this[int index] 
      {
        get => Values[index];
        set => Values[index] = value; 
      }

      public int Count => Values.Length;

      public bool IsReadOnly => false;

      public void Add(int item)
      {
        throw new NotImplementedException();
      }

      public void Clear()
      {
        throw new InvalidOperationException();
      }

      public bool Contains(int item)
      {
        throw new InvalidOperationException();
      }

      public void CopyTo(int[] array, int arrayIndex)
      {
        throw new InvalidOperationException();
      }

      public IEnumerator<int> GetEnumerator() => (IEnumerator<int>)Values.GetEnumerator();

      public int IndexOf(int item)
      {
        throw new InvalidOperationException();
      }

      public void Insert(int index, int item)
      {
        throw new InvalidOperationException();
      }

      public bool Remove(int item)
      {
        throw new InvalidOperationException();
      }

      public void RemoveAt(int index)
      {
        throw new InvalidOperationException();
      }

      IEnumerator IEnumerable.GetEnumerator() => Values.GetEnumerator();

      public ValueStore(int capacity)
      {
        Values = new int[capacity];
      }
    }
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
  }
}
