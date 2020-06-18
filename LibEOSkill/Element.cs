using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibEOSkill
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

    public bool Unknown1
    {
      get => BitfieldData[7];
      set => BitfieldData[7] = value;
    }


    public bool Unknown2
    {
      get => BitfieldData[8];
      set => BitfieldData[8] = value;
    }

    public Element() { }

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
      get => BitfieldData[14];
      set => BitfieldData[14] = value;
    }

    public DamageElement() { }

    public DamageElement(int bitfield) : base(bitfield)
    {

    }
  }
}
