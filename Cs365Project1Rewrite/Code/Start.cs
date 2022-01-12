/*
*	CS365 Project 1
*
*	Contributors:
*		Austyn Simons
*		Brandon Yen
*		Charles Rizzo
*
*	Start.cs kicks off the program and is structured so that anything that goes wrong will be caught by the try-catch block.
*	a fileRead object is created, which compiles a list of IInstructions. That list is then passed to the fileWrited function
*	to use in order to write each Instruction's binary value to an output file specified in fileWrite.cs.
*/
using Cs365Project1Rewrite.Code.Util;
using Exceptable;
using System;

public class Start{

	public static int Main(string[] args)
	{
		var result =
			((Action)(() => FileWrite.WriteFile(FileRead.ExtractInstructions(args[0]), "byteCodeNew.bin")))
				.ToUnitFunc()
				.ToExceptable();

		result.DoIfException(ex => Console.WriteLine($"Error: {ex.Message}"));

		return result.Unlift(_ => 0, _ => 1);
	}
}
