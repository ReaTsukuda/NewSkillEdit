﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SkillCode.GameSpecific
{
  public class EO2USkill : Skill
  {
    public EO2USkill(BinaryReader reader, int skillId, OriginTablets.Types.Table nameTable) : base(reader, skillId, nameTable)
    {
    }

    public override void Serialize(BinaryWriter writer)
    {
      throw new NotImplementedException();
    }
  }
}
