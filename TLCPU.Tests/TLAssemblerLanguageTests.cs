using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TLCPU;
using System.Collections.Generic;
using System.IO;

namespace TLCPU.Tests
{
    [TestClass]
    public class TLAssemblerLanguageTests
    {
        [TestMethod]
        public void TestDisassembler()
        {
            var startAddress = 0;
            var cpu = CPU.Boot(5, 50);
            var asm = new Assembler(cpu);
            var sourceFile = "Source\\Source001.tlasm";

            var source = File.ReadAllText(sourceFile);

            var programLength = asm.Assemble(source, startAddress);
            Console.WriteLine();

            var labels = new Dictionary<int, string> {
                {0x07, "ApplicationStart"},
                {0x0B, "TopOfLoop"},
                {0x11, "Subroutine"},
                {0x16, "EndProgram"}
            };
            var disassembly = asm.Disassemble(0, programLength, labels, new DisassemblyOptions { IncludeInitialAddress = false });
            Console.WriteLine(disassembly);
            //cpu.Dump("Pre-Execute");
            cpu.Execute(startAddress);
            //cpu.Dump("Post-Execute");
            var s1 = source.StripWhitespace();
            var s2 = disassembly.StripWhitespace();
            Assert.AreEqual(s1, s1);
        }
    }

    static class Extensions
    {
        public static string StripWhitespace(this string input)
        {
            return input
                .Replace('\n', ' ')
                .Replace('\r', ' ')
                .Replace('\t', ' ')
                .Replace(" ", "");
        }
    }
}
