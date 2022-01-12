/*
*	CS365 Project 1
*
*	Contributors:
*		Austyn Simons
*		Brandon Yen
*		Charles Rizzo
*
*	fileWrite.cs is a static class that has a fileWriter function. The function takes a list of IInstruction type 	
*	objects and writes each object's value to an output file (statically declared and stored in the outputFile string).
*
*/
using Cs365Project1Rewrite.Code.Util;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class FileWrite
{
    public static void WriteFile(IEnumerable<uint?> instrs, string path)
    {
        using var bw = new BinaryWriter(File.OpenWrite(path));

        bw.Write(0xefbeedfe);

        instrs.ToList().ForEach(x => x.Finally(bw.Write));
    }
}
