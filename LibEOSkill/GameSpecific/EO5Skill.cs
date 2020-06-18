using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibEOSkill.GameSpecific
{
  public class EO5Skill : Skill
  {
    public EO5Skill(BinaryReader reader, int skillId, OriginTablets.Types.Table nameTable) : base(reader, skillId, nameTable)
    {
    }

    public override void Serialize(BinaryWriter writer)
    {
      throw new NotImplementedException();
    }
  }
}
