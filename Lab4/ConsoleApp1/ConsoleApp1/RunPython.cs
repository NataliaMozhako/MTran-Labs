using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    class RunPython
    {
        private static void NetOutputDataHandler(object sendingProcess,
            DataReceivedEventArgs outLine)
        {
            // Collect the net view command output.
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                // Add the text to the collected output.
                String d = (Environment.NewLine + "  " + outLine.Data);
                Console.WriteLine(d);
            }
        }


        public static SemanticErrorException DoSemanticCheck()
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {             // D:\Education\Laboratory_works\МТран\Lab4\ConsoleApp1
                    FileName = @"D:/Education/Laboratory_works/МТран/Lab4/ConsoleApp1/python-3.7.3-embed-win32/python",
                    Arguments = @"D:/Education/Laboratory_works/МТран/Lab4/ConsoleApp1/test.py",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
            Regex errorPositioningMessageRegex = new Regex(
                @"\s*File [\""].+[\""], line (\d+), in .+",
                RegexOptions.Compiled | RegexOptions.IgnoreCase
                );
            Regex errorTypeRegex = new Regex(@"^\s*(\w+)[:](.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            
            proc.Start();

            bool hasError = false;
            bool nextLineIsCodeLine = false;
            string errorCodeLine = "";
            string errorLineNumber = "";
            string errorType = "";
            string description = "";
            while (!proc.StandardError.EndOfStream)
            {
                hasError = true;
                string line = proc.StandardError.ReadLine();

                Match errorPositioningMessageMatch = errorPositioningMessageRegex.Match(line);
                if (errorPositioningMessageMatch.Success)
                {
                    errorLineNumber = errorPositioningMessageMatch.Groups[1].Value;
                    nextLineIsCodeLine = true;
                    continue;
                }
                if (nextLineIsCodeLine)
                {
                    errorCodeLine = line;
                    nextLineIsCodeLine = false;
                    continue;
                }
                Match errorTypeMatch = errorTypeRegex.Match(line);
                if (errorTypeMatch.Success)
                {
                    errorType = errorTypeMatch.Groups[1].Value;
                    description = errorTypeMatch.Groups[2].Value;
                }
            }
            if (hasError)
                return new SemanticErrorException(errorType, description, int.Parse(errorLineNumber), errorCodeLine);
            return null;
        }
    }
}