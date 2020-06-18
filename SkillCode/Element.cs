using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillCode
{
  /// <summary>
  /// A collection of elements for damage types and buff elements.
  /// </summary>
  public class Element : Bitfield
  {
    /// <summary>
    /// Does the element include cut?
    /// </summary>
    public bool Cut
    {
      get => BitfieldData[0];
      set => BitfieldData[0] = value;
    }

    /// <summary>
    /// Does the element include bash?
    /// </summary>
    public bool Bash
    {
      get => BitfieldData[1];
      set => BitfieldData[1] = value;
    }

    /// <summary>
    /// Does the element include stab?
    /// </summary>
    public bool Stab
    {
      get => BitfieldData[2];
      set => BitfieldData[2] = value;
    }

    /// <summary>
    /// Does the element include fire?
    /// </summary>
    public bool Fire
    {
      get => BitfieldData[3];
      set => BitfieldData[3] = value;
    }

    /// <summary>
    /// Does the element include ice?
    /// </summary>
    public bool Ice
    {
      get => BitfieldData[4];
      set => BitfieldData[4] = value;
    }

    /// <summary>
    /// Does the element include volt?
    /// </summary>
    public bool Volt
    {
      get => BitfieldData[5];
      set => BitfieldData[5] = value;
    }

    /// <summary>
    /// Does the element include almighty?
    /// </summary>
    public bool Almighty
    {
      get => BitfieldData[6];
      set => BitfieldData[6] = value;
    }

    public Element() { }

    /// <summary>
    /// For deseriailzing from a JSON file.
    /// </summary>
    public Element(JsonReader reader) : base(reader)
    {
      reader.Read(); reader.Read(); BitfieldData[0] = (bool)reader.Value; // Cut
      reader.Read(); reader.Read(); BitfieldData[1] = (bool)reader.Value; // Bash
      reader.Read(); reader.Read(); BitfieldData[2] = (bool)reader.Value; // Stab
      reader.Read(); reader.Read(); BitfieldData[3] = (bool)reader.Value; // Fire
      reader.Read(); reader.Read(); BitfieldData[4] = (bool)reader.Value; // Ice
      reader.Read(); reader.Read(); BitfieldData[5] = (bool)reader.Value; // Volt
      reader.Read(); reader.Read(); BitfieldData[6] = (bool)reader.Value; // Almighty
    }

    public Element(int bitfield) : base(bitfield) { }
  }

  /// <summary>
  /// Much the same as an Element, but it includes the "no penalty" flag for damage types.
  /// </summary>
  public class DamageElement : Element
  {
    /// <summary>
    /// Is this damage type flagged as "no penalty"? In EO3, this flags the damage to skip the range penalty, the arm bind penalty, and the sleep damage bonus.
    /// </summary>
    [JsonProperty(Order = 1)]
    public bool NoPenalty
    {
      get => BitfieldData[15];
      set => BitfieldData[15] = value;
    }

    public DamageElement() { }

    /// <summary>
    /// For deseriailzing from a JSON file.
    /// </summary>
    public DamageElement(JsonReader reader) : base(reader)
    {
      reader.Read(); reader.Read(); BitfieldData[7] = (bool)reader.Value; // NoPenalty
    }

    public DamageElement(int bitfield) : base(bitfield)
    {

    }
  }
}
