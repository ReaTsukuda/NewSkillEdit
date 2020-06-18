using CommandLine;
using CommandLine.Text;
using Newtonsoft.Json;
using OriginTablets.Types;
using LibEOSkill;
using LibEOSkill.GameSpecific;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SkillCLI
{
  class Options
  {
    [Option('g', "game",
      Required = true,
      HelpText = "The game this skill table is for.\n\tValid values: EO3, EO4, EOU, EO2U, EO5, EON.")]
    public SkillModes Game { get; set; }
  }

  [Verb("disassemble", HelpText = "Parses the provided table and outputs it to a JSON file.")]
  class DisassembleOptions : Options
  {
    [Option('i', "inputFile",
      Required = true,
      HelpText = "The playerskilltable.tbl file to disassemble.")]
    public string FilePath { get; set; }

    [Option('n', "nameTableFile",
      Required = true,
      HelpText = "The playerskillnametable.tbl file associated with the input table.")]
    public string NameTablePath { get; set; }

    [Option('o', "outputFile",
      Required = false,
      Default = "Output.json",
      HelpText = "Where to output the JSON file to.")]
    public string OutputPath { get; set; }
  }

  [Verb("assemble", HelpText = "Serializes the provided skill JSON file back into a playerskilltable.tbl.")]
  class AssembleOptions : Options
  {

    [Option('i', "inputFile",
      Required = true,
      HelpText = "The JSON file to reassemble back into a playerskilltable.tbl.")]
    public string FilePath { get; set; }

    [Option('o', "outputDirectory",
      Required = true,
      HelpText = "The directory to output the resulting files back to.")]
    public string OutputDirectory { get; set; }
  }

  class Program
  {
    static void Main(string[] args)
    {
      var result = Parser.Default.ParseArguments<DisassembleOptions, AssembleOptions>(args)
        .WithParsed(options =>
        {
          if (options.GetType() == typeof(DisassembleOptions)) { Disassemble(options as DisassembleOptions); }
          else if (options.GetType() == typeof(AssembleOptions)) { Assemble(options as AssembleOptions); }
        });
    }

    static void Disassemble(DisassembleOptions options)
    {
      Type gameSkillType = Skill.GameSkillTypes[options.Game];
      var skills = new List<EO3Skill>();
      var nameTable = new OriginTablets.Types.Table(options.NameTablePath, false);
      var gameSkillConstructorInfo = gameSkillType.GetConstructor(new[] { typeof(BinaryReader), typeof(int), typeof(Table) });
      using (var reader = new BinaryReader(new FileStream(options.FilePath, FileMode.Open)))
      {
        int numberOfSkills = (int)reader.BaseStream.Length / Skill.GameSkillLengths[options.Game];
        for (int skillIndex = 0; skillIndex < numberOfSkills; skillIndex += 1)
        {
          skills.Add(gameSkillConstructorInfo.Invoke(new object[] { reader, skillIndex, nameTable }) as EO3Skill);
        }
      }
      File.WriteAllLines(options.OutputPath, new string[]
      {
        JsonConvert.SerializeObject(skills, 
        Formatting.Indented
      )});
    }

    static void Assemble(AssembleOptions options)
    {
      var skills = JsonConvert.DeserializeObject(
        string.Join(string.Empty, File.ReadAllLines(options.FilePath)),
        typeof(List<EO3Skill>)
      ) as List<EO3Skill>;
      var nameTable = new Table();
      nameTable.AddRange(skills.Select(skill => skill.Name));
      using (var writer = new BinaryWriter(new FileStream(
        Path.Combine(options.OutputDirectory, "playerskilltable.tbl"),
        FileMode.Create)))
      {
        foreach (var skill in skills) { skill.Serialize(writer); }
      }
      nameTable.WriteToFile(Path.Combine(options.OutputDirectory, "playerskillnametable.tbl"), false);
    }
  }
}
