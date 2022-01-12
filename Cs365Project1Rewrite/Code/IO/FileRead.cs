/*
*	CS365 Project 1
*
*	Contributors:
*		Austyn Simons
*		Brandon Yen
*		Charles Rizzo
*
*	fileRead.cs creates an object of type fileRead and ultimately builds a list of IInstructions. It also constructs
*	an internal dictionary of Labels in order to associate an address with each label in the file. 
*
*/
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Cs365Project1Rewrite.Code.Util;

public static class FileRead
{
    //This function uses Regex to get the Instruction and a possible argument from each line by stripping away a variable
    //amount of whitespace. We try to parse the argument as an Int and a hex before concluding if its not either of those
    //it must be a label.
    public static IEnumerable<uint?> ExtractInstructions(string filename) 
		=> ReadLines(filename)
            .Where(line => line.Any() && !line.StartsWith('#') && !line.StartsWith("//") && line[^1] != ':')
            .Select((line, i) => 
				(Regex.Match(line, @"([A-Za-z0-9\-]+)[\s]+([A-Za-z0-9\-]+)$", RegexOptions.IgnoreCase), 
				 Regex.Match(line, @"[\s]*([A-Za-z0-9\-]+)[\s]*$"),
				 i * 4))
            .Select(match => CreateInstruction(match.Item1, match.Item2, match.Item3, LocateLabels(filename)))
            .ToList();

    private static uint? CreateInstruction(Match match, Match nonArgumentMatch, int address, IReadOnlyDictionary<string, int> labelDict) 
		=> CreateObject(match.ParseCommand(nonArgumentMatch), match.ParseArgument(labelDict) ?? 0, address);

    private static string ParseCommand(this Match argumentMatch, Match nonArgumentMatch) 
		=> (argumentMatch.ToNullable(m => m.Success ? m : null) ?? nonArgumentMatch).Groups[1].Value;

    private static int? ParseArgument(this Match argumentMatch, IReadOnlyDictionary<string, int> labelDict)
		=> argumentMatch
            .Groups
            .ValueOrDefault(2)?
            .Value
            .BindReferenceToValue<string, int>(argVal => argVal.ParseOrNull() ?? argVal.ParseHexOrNull() ?? labelDict[argVal]);

    private static T ValueOrDefault<T>(this IList<T> list, int index)
		=> index < list.Count ? list[index] : default;

    private static int? ParseHexOrNull(this string arg)
		=> arg.StartsWith("0x") ? arg[2..].ParseOrNull(System.Globalization.NumberStyles.HexNumber, null) : null;

    //Iterates through the file locating each label and creates a Label object for each one spotted. The label object
    //and its name are then added to a dictionary of Label objects. Regex is used to extract the label names.
    private static IReadOnlyDictionary<string, int> LocateLabels(string filename) 
		=> ReadLines(filename).Where(l => l.Any() && !l.StartsWith('#') && !l.StartsWith("//"))
                              .Select(l => Regex.Match(l, @"[\s]*([A-Za-z0-9\-]+):[\s]*$"))
                              .ToList() // this is necessary for some reason
                              .SelectWithPrevious()
                              .Select(selection => (selection.Last().Groups[1].Value, selection.Where(s => !s.Success).Count() * 4))
                              .Where(match => match.Value.Any())
                              .ToDictionary(match => match.Value, match => match.Item2);

    //This function now uses a switch statement to take a command name and translate it to the proper instruction value (returns null if command not currently supported)
    private static uint? CreateObject(string comm, int? valToUse, int currentInstruc)
		=> comm switch
		{
			"exit" => valToUse.Transform(v => 0 | (uint)v),
			"swap" => 1 << 24,
			"inpt" => 1 << 25,
			"nop" => 3 << 24,
			"pop" => 1 << 28,
			"add" => 1 << 29,
			"sub" => 33 << 24,
			"mul" => ((1 << 4) + 1) << 25,
			"div" => ((((1 << 4) + 1) << 1) + 1) << 24,
			"rem" => ((1 << 3) + 1) << 26,
			"and" => ((((1 << 3) + 1) << 2) + 1) << 24,
			"or" => ((((1 << 3) + 1) << 1) + 1) << 25,
			"xor" => ((((((1 << 3) + 1) << 1) + 1) << 1) + 1) << 24,
			"neg" => ((1 << 1) + 1) << 28,
			"not" => 49 << 24,
			"goto" => valToUse.Transform(v => (uint)((7 << 28) | valToUse)),
			"ifeq" => valToUse.Transform(v => (uint) ((8 << 28) | (0 << 24) | ((valToUse - currentInstruc) & 0xffffff))),
			"ifne" => valToUse.Transform(v => (uint)((8 << 28) | (1 << 24) | ((valToUse - currentInstruc) & 0xffffff))),
			"iflt" => valToUse.Transform(v => (uint)((8 << 28) | (2 << 24) | ((valToUse - currentInstruc) & 0xffffff))),
			"ifgt" => valToUse.Transform(v => (uint)((8 << 28) | (3 << 24) | ((valToUse - currentInstruc) & 0xffffff))),
			"ifle" => valToUse.Transform(v => (uint)((8 << 28) | (4 << 24) | ((valToUse - currentInstruc) & 0xffffff))),
			"ifge" => valToUse.Transform(v => (uint)((8 << 28) | (5 << 24) | ((valToUse - currentInstruc) & 0xffffff))),
			"ifez" => valToUse.Transform(v => (uint)((9 << 28) | (0 << 24) | ((valToUse - currentInstruc) & 0xffffff))),
			"ifnz" => valToUse.Transform(v => (uint)((9 << 28) | (1 << 24) | ((valToUse - currentInstruc) & 0xffffff))),
			"ifmi" => valToUse.Transform(v => (uint)((9 << 28) | (2 << 24) | ((valToUse - currentInstruc) & 0xffffff))),
			"ifpl" => valToUse.Transform(v => (uint)((9 << 28) | (3 << 24) | ((valToUse - currentInstruc) & 0xffffff))),
			"dup" => valToUse.Transform(v => (uint)((12 << 28) | (valToUse << 2))),
			"print" => unchecked((uint)(13 << 28)),
			"dump" => unchecked((uint)(14 << 28)),
			"push" => valToUse.Transform(v => (uint)((15 << 28) | valToUse)),
			_ => null,
		};

	private static IEnumerable<string> ReadLines(string filename)
		=> UnfoldCalculation
			.UnfoldWhile(
				new StreamReader(File.OpenRead(filename)),
				state => state,
				state => state.Peek() >= 0,
				step => step,
				state => state.ReadLine()?.ToLower());
}
