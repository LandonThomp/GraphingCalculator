using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphingCalculator
{
    class CalculatorClient
    {
        private readonly string scriptPath;
        private readonly string storageScriptPath;

        public CalculatorClient()
        {
            // Path to the Python script in the project root directory
            scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "calculator.py");
            storageScriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "historymanager.py");
        }

        public async Task<string> CalculateAsync(string expression)
        {
            // Normalize the path
            string normalizedPath = Path.GetFullPath(scriptPath);

            if (!File.Exists(normalizedPath))
            {
                throw new FileNotFoundException($"The Python script wasn't found: {normalizedPath}");
            }

            string scriptDir = Path.GetDirectoryName(normalizedPath);

            // Construct the process start info
            var processStartInfo = new ProcessStartInfo("python", $"\"{scriptPath}\"")
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true, // Redirect standard error to capture any errors
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = scriptDir
            };

            using (var process = new Process { StartInfo = processStartInfo })
            {
                process.Start();

                // Write to standard input
                await process.StandardInput.WriteLineAsync(expression);
                process.StandardInput.Close();

                // Read the output from the Python script
                string result = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();

                // Wait for the process to exit
                await process.WaitForExitAsync();

                return result.Trim();
            }
        }
        public async Task<(List<string> userInputs, List<string> calcResponses)> GetLastThreeEntriesAsync()
        {
            string normalizedPath = Path.GetFullPath(storageScriptPath);

            if (!File.Exists(normalizedPath))
            {
                throw new FileNotFoundException($"The Python script wasn't found: {normalizedPath}");
            }

            string scriptDir = Path.GetDirectoryName(normalizedPath);

            var processStartInfo = new ProcessStartInfo("python", $"\"{storageScriptPath}\" get_last_three")
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = scriptDir
            };

            using (var process = new Process { StartInfo = processStartInfo })
            {
                process.Start();

                // Read the output from the Python script asynchronously
                var resultTask = process.StandardOutput.ReadToEndAsync();
                var errorTask = process.StandardError.ReadToEndAsync();

                // Wait for the process to exit and for the output/error reading to complete
                await Task.WhenAll(process.WaitForExitAsync(), resultTask, errorTask);

                string result = await resultTask;
                string error = await errorTask;

                // Log outputs for debugging
                Console.WriteLine("Python script result: " + result);
                Console.WriteLine("Python script error: " + error);

                if (!string.IsNullOrEmpty(error))
                {
                    throw new Exception($"Error from Python script: {error}");
                }

                // Split the result into lines
                var entries = result.Trim().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                var userInputs = new List<string>();
                var calcResponses = new List<string>();

                foreach (var entry in entries)
                {
                    var parts = entry.Split(new[] { "___" }, StringSplitOptions.None);
                    if (parts.Length == 2)
                    {
                        userInputs.Add(parts[0]);
                        calcResponses.Add(parts[1]);
                    }
                    else
                    {
                        Console.WriteLine($"Unexpected entry format: {entry}");
                    }
                }

                return (userInputs, calcResponses);
            }
        }


        public async Task<string> StoreStringsAsync(string command)
        {
            // Normalize the path
            string normalizedPath = Path.GetFullPath(storageScriptPath);

            if (!File.Exists(normalizedPath))
            {
                throw new FileNotFoundException($"The Python script wasn't found: {normalizedPath}");
            }

            string scriptDir = Path.GetDirectoryName(normalizedPath);

            // Construct the process start info
            var processStartInfo = new ProcessStartInfo("python", $"\"{storageScriptPath}\"")
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true, // Redirect standard error to capture any errors
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = scriptDir
            };

            using (var process = new Process { StartInfo = processStartInfo })
            {
                process.Start();

                // Write to standard input
                await process.StandardInput.WriteLineAsync(command);
                process.StandardInput.Close();

                // Read the output from the Python script
                string result = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();

                // Wait for the process to exit
                await process.WaitForExitAsync();

                if (!string.IsNullOrEmpty(error))
                {
                    throw new Exception($"Error from Python script: {error}");
                }

                return result.Trim();
            }
        }
    }
}
