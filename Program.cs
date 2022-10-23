using McMaster.Extensions.CommandLineUtils;
using System;
using System.IO;

namespace CSVUtil
{
    class Program
    {
        public static void Main(string[] args)
        {
            // init new CLI app
            var app = new CommandLineApplication<Program>();

            var inputFileOption = app.Option<string>("-i|--input", "Input file to convert", CommandOptionType.SingleValue).IsRequired();
            var verboseOption = app.Option("-v|--verbose", "Display operation details", CommandOptionType.NoValue);

            #region convert-command
            app.Command("convert", (convert) =>
            {
                var outputOption = convert.Option("-o|--output", "Output file", CommandOptionType.SingleValue);

                var firstRowOption = convert.Option("-f|--first-row-header", "First row header", CommandOptionType.NoValue);

                convert.HelpOption("-? | -h | --help");

                convert.OnExecute(() =>
                {
                    if (verboseOption.HasValue())
                    {
                        Console.WriteLine($"Init process file {inputFileOption.ParsedValue}.");
                    }

                    var inputFile = GetInputFile(inputFileOption);

                    var outputFile = outputOption.Value() ?? Path.GetFileNameWithoutExtension(inputFile);

                    if (verboseOption.HasValue() && firstRowOption.HasValue())
                    {
                        Console.WriteLine("Find first row as header");
                    }

                    if (verboseOption.HasValue())
                    {
                        Console.WriteLine($"Convert to {outputFile}.xlsx");
                    }
                });
            });

            #endregion convert-command

            #region print-command
            app.Command("print", (print) =>
            {
                var tailOptions = print.Option<bool>("-t|--tail", "Show n rows from the tail.", CommandOptionType.NoValue);

                var rowOption = print.Option<int>("-r|--rows", "Number of rows to show", CommandOptionType.SingleValue);

                print.HelpOption("-? | -h | --help");

                print.OnExecute(() => 
                {
                    var inputFile = GetInputFile(inputFileOption);

                    // Zero = all rows
                    var rows = rowOption.HasValue() ? Math.Abs(rowOption.ParsedValue) : 0;

                    if (verboseOption.HasValue())
                    {
                        Console.WriteLine(tailOptions.HasValue() && rows > 0 ? $"Print {rows} records from the tail." : $"Print {rows} records from the top.");    
                    }

                    // print file
                    Console.WriteLine("print.");
                });
            });

            #endregion

            app.HelpOption("-h | --help | -?");
            app.Execute(args);
        }


        private static string GetInputFile(CommandOption<string> inputOption) {
            var inputFile = inputOption.ParsedValue;
            if (!Path.IsPathFullyQualified(inputOption.ParsedValue))
            {
                inputFile = Path.GetFullPath(inputOption.ParsedValue);
            }

            return inputFile;
        }
    }
}