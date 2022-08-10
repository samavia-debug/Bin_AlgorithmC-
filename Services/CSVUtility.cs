using BinCompletionAlgorithm.Models;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinCompletionAlgorithm.Services
{
    public class CSVUtility
    {
        public List<Item> ReadCSVItems(string path)
        {
            using (TextFieldParser csvParser = new TextFieldParser(path))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { ",", ";" });
                csvParser.HasFieldsEnclosedInQuotes = true;

                // Skip the row with the column names
                csvParser.ReadLine();
                var elts = new List<Item>();
                while (!csvParser.EndOfData)
                {
                    // Read current line fields, pointer moves to the next line.
                    string[] fields = csvParser.ReadFields();
                    try
                    {
                        elts.Add(new()
                        {
                            Value = int.Parse(fields[0]),
                            Label = fields[1],
                        });
                    }
                    catch
                    {
                        Console.WriteLine();
                    }
                }
                return elts;
            }
        }

        public List<Bin> ReadCSVBins(string path)
        {
            using (TextFieldParser csvParser = new TextFieldParser(path))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { ",", ";" });
                csvParser.HasFieldsEnclosedInQuotes = true;

                // Skip the row with the column names
                csvParser.ReadLine();
                var bins = new List<Bin>();
                while (!csvParser.EndOfData)
                {
                    // Read current line fields, pointer moves to the next line.
                    string[] fields = csvParser.ReadFields();
                    try
                    {
                        bins.Add(new()
                        {
                            Capacity = int.Parse(fields[0]),
                            Label = fields[1],
                        });
                    }
                    catch
                    {
                        Console.WriteLine();
                    }
                }
                return bins;
            }
        }

        public async Task WriteCSVResult(string path, List<Bin> Result)
        {
            // TODO : Try To Make Every item In A Column And Give Every Column A Number Like Elt1 ... etc
            var columnsneeded = Result.Select(b => b.Items.Count()).Max();
            var csv = "Bin Label;Bin Size;Items\n";
            foreach (var bin in Result)
            {
                csv += $"{bin.Label};{bin.Capacity};\"[{String.Join(',', bin.Items.Select(i => i.Value))}]\"\n";
            }
            await File.WriteAllTextAsync(path, csv);
        }

    }
}
