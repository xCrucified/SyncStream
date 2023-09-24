using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

public class TextAnalysisResult
{
    public int Words { get; set; }
    public int Lines { get; set; }
    public int Punctuation { get; set; }
}

public class Program
{
    private static object lockObj = new object();

    public static void Main(string[] args)
    {
        string directoryPath = @"C:\Users\Bobde\Desktop\background";

        if (!Directory.Exists(directoryPath))
        {
            Console.WriteLine("Directory is not avaible.");
            return;
        }

        string[] files = Directory.GetFiles(directoryPath);

        List<Thread> threads = new List<Thread>();

        foreach (var file in files)
        {
            Thread thread = new Thread(() =>
            {
                var result = AnalyzeFile(file);
                DisplayResult(file, result);
            });

            threads.Add(thread);
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }
    }

    public static TextAnalysisResult AnalyzeFile(string filePath)
    {
        TextAnalysisResult result = new TextAnalysisResult();

        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                result.Lines++;
                result.Words += line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
                result.Punctuation += CountPunctuation(line);
            }
        }

        return result;
    }

    public static int CountPunctuation(string text)
    {
        char[] punctuationChars = { '.', ',', ';', ':', '–', '—', '‒', '…', '!', '?', '"', '\'', '«', '»', '(', ')', '{', '}', '[', ']', '<', '>', '/' };
        return text.Count(c => punctuationChars.Contains(c));
    }

    public static void DisplayResult(string fileName, TextAnalysisResult result)
    {
        lock (lockObj)
        {
            Console.WriteLine($"File: {fileName}");
            Console.WriteLine($"Words count: {result.Words}");
            Console.WriteLine($"Rows count: {result.Lines}");
            Console.WriteLine($"Count of separating signs: {result.Punctuation}");
        }
    }
}
