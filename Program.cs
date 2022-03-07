using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using Newtonsoft.Json.Linq;
using CsvHelper;

namespace Got2Insure.Technical.Interview202203
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var inputFileName = args[0];
            var outputFolder = args[1];
            var batchSize = int.Parse(args[2]);

            var inputText = File.ReadAllText(inputFileName);
            var inputJson = JObject.Parse(inputText);
            var batchNumber = 1;
            foreach (var inputQuoteBatch in Batch(inputJson["Quotes"], batchSize))
            {
                batchNumber++;

                var processedQuoteBatch = inputQuoteBatch.Select(quote => new
                {
                    FirstName = quote["FirstName"].Value<string>(),
                    Surname = quote["Surname"].Value<string>(),
                    YearsWithNoClaims = quote["YearsWithNoClaims"].Value<int>(),
                    MotorPremium = quote["MotorPremium"].Value<decimal>(),
                    NoClaimsDiscountProtectionPremium = quote["NoClaimsDiscountProtectionPremium"].Value<decimal>()
                });
                
                var outputFileName = GetOutputFileName(inputFileName, outputFolder, batchNumber);
                var outputFile = File.OpenWrite(outputFileName);
                var textWriter = new StreamWriter(outputFile);
                var csvWriter = new CsvWriter(textWriter, CultureInfo.InvariantCulture);
                csvWriter.WriteRecords(processedQuoteBatch);
            }
        }

        private static IEnumerable<IEnumerable<T>> Batch<T>(IEnumerable<T> source, int batchSize)
        {
            var startIndex = 0;
            for (int i = 0; i <= source.Count(); i += batchSize)
            {
                var batch = source.Skip(startIndex).Take(batchSize).ToArray();
                yield return batch;
                startIndex += batchSize;
            }
        }

        private static string GetOutputFileName(string inputFileName, string outputFolder, int batchNumber)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(inputFileName);
            var outputFileName = outputFolder + @"\" + fileNameWithoutExtension + "_" + batchNumber + ".csv";
            return outputFileName;
        }
    }
}
