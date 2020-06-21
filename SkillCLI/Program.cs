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
using System.Collections;

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
      HelpText = "The skill table file to disassemble.")]
    public string FilePath { get; set; }

    [Option('n', "nameTableFile",
      Required = true,
      HelpText = "The name table file associated with the input table.")]
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
      HelpText = "The JSON file to reassemble back into a skill table.")]
    public string FilePath { get; set; }

    [Option('t', "tableFile",
      Required = true,
      HelpText = "The name of the resulting skill table file.")]
    public string TableFilePath { get; set; }

    [Option('n', "nameTableFile",
      Required = true,
      HelpText = "The name of the resulting name table file.")]
    public string NameTableFilePath { get; set; }
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
      var skills = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(gameSkillType));
      var nameTable = new Table(options.NameTablePath, false);
      var gameSkillConstructorInfo = gameSkillType.GetConstructor(new[] { typeof(BinaryReader), typeof(int), typeof(Table) });
      using (var reader = new BinaryReader(new FileStream(options.FilePath, FileMode.Open)))
      {
        int numberOfSkills = (int)reader.BaseStream.Length / Skill.GameSkillLengths[options.Game];
        for (int skillIndex = 0; skillIndex < numberOfSkills; skillIndex += 1)
        {
          skills.Add(gameSkillConstructorInfo.Invoke(new object[] { reader, skillIndex, nameTable }));
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
      Type gameSkillType = Skill.GameSkillTypes[options.Game];
      Type skillListType = typeof(List<>).MakeGenericType(gameSkillType);
      var skills = (IList)JsonConvert.DeserializeObject(
        string.Join(string.Empty, File.ReadAllLines(options.FilePath)),
        skillListType);
      var nameTable = new Table();
      nameTable.AddRange(skills.Cast<object>().Select(skill => (skill as Skill).Name));
      using (var writer = new BinaryWriter(new FileStream(options.TableFilePath, FileMode.Create)))
      {
        foreach (var skill in skills) { (skill as Skill).Serialize(writer); }
      }
      nameTable.WriteToFile(options.NameTableFilePath, false);
    }
  }
}
