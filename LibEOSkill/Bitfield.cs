using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LibEOSkill
{
  /// <summary>
  /// A basic container for data stored in bitfields.
  /// </summary>
  public class Bitfield
  {
    /// <summary>
    /// The object that tracks the various flags in the bitfield.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected BitArray BitfieldData;

    /// <summary>
    /// Gets the solid binary representation of the bitfield.
    /// </summary>
    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int BinaryValue
    {
      get
      {
        var resultBuffer = new int[1];
        BitfieldData.CopyTo(resultBuffer, 0);
        return resultBuffer[0];
      }
    }

    public Bitfield() { BitfieldData = new BitArray(new int[] { 0 }); }

    /// <summary>
    /// For deserializing from a JSON file.
    /// </summary>
    public Bitfield(JsonReader reader) 
    {
      BitfieldData = new BitArray(new int[] { 0 });
      reader.Read(); reader.Read(); // Object name and object start.
    }

    /// <param name="bitfield">The int that represents the bitfield.</param>
    public Bitfield(int bitfield)
    {
      BitfieldData = new BitArray(new int[] { bitfield });
    }
  }
}
