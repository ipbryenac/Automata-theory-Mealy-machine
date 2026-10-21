using System;
using System.Collections.Generic;
using System.Text;

// Mealy machine for C-style comments: M = (Q, Σ, Δ, δ, λ, q0)
//   Q  = { q0, q1, q2, q3, q4 }     (q4 is the trap state)
//   Σ  = { a, *, / }
//   Δ  = { 0, 1 }
//   q0 = start state
//   (No F: a Mealy machine has no final states.)
//
// Output rule: print 1 only on the move that completes the closing "*/"
// (q3 --/--> q4); every other move prints 0.
// The input is exactly one comment  <=>  the output string ends in 1.
class CCommentMealy
{
    static readonly char[] Sigma = { 'a', '*', '/' };

    const string Start = "q0";

    // Table: (state, symbol) -> (next state, output).
    // Same as the transition table, where each cell reads "next / output".
    static readonly Dictionary<(string, char), (string Next, char Out)> delta = new()
    {
        //            a                 *                 /
        { ("q0", 'a'), ("q4", '0') }, { ("q0", '*'), ("q4", '0') }, { ("q0", '/'), ("q1", '0') },
        { ("q1", 'a'), ("q4", '0') }, { ("q1", '*'), ("q2", '0') }, { ("q1", '/'), ("q4", '0') },
        { ("q2", 'a'), ("q2", '0') }, { ("q2", '*'), ("q3", '0') }, { ("q2", '/'), ("q2", '0') },
        { ("q3", 'a'), ("q2", '0') }, { ("q3", '*'), ("q3", '0') }, { ("q3", '/'), ("q4", '1') },
        { ("q4", 'a'), ("q4", '0') }, { ("q4", '*'), ("q4", '0') }, { ("q4", '/'), ("q4", '0') },
    };

    static bool InSigma(string w)
    {
        foreach (char c in w)
            if (Array.IndexOf(Sigma, c) < 0) return false;
        return true;
    }

    // Runs the machine; returns the output string (always the same length as w).
    static string Run(string w, bool trace = false)
    {
        string state = Start;
        var output = new StringBuilder();
        foreach (char c in w)
        {
            var (next, o) = delta[(state, c)];
            if (trace) Console.WriteLine($"  {state} --{c}/{o}--> {next}");
            output.Append(o);
            state = next;
        }
        return output.ToString();
    }

    // Output ends in 1  <=>  w is exactly one complete comment.
    static bool IsComment(string output) =>
        output.Length > 0 && output[output.Length - 1] == '1';

    static void Main()
    {
        string[] accept = { "/*a*/", "/**/", "/***/", "/*aaa*aaa*/", "/*a/a*/" };
        string[] reject = { "/**", "/**/a/*aa*/", "aaa/**/aa", "/*/", "/**a/", "//aaaa" };

        Console.WriteLine("Should be a COMMENT (output ends in 1):");
        foreach (var w in accept)
        {
            string o = Run(w);
            Console.WriteLine($"  {w,-15} -> {o,-12} {(IsComment(o) ? "Comment" : "Not a comment")}");
        }

        Console.WriteLine("\nShould NOT be a comment (output ends in 0):");
        foreach (var w in reject)
        {
            string o = Run(w);
            Console.WriteLine($"  {w,-15} -> {o,-12} {(IsComment(o) ? "Comment" : "Not a comment")}");
        }

        Console.WriteLine("\nEnter a string over {a, *, /} to trace (blank to quit):");
        string input;
        while (!string.IsNullOrEmpty(input = Console.ReadLine()))
        {
            if (!InSigma(input))
            {
                Console.WriteLine("  Only the symbols a, * and / are allowed.");
                continue;
            }
            string o = Run(input, trace: true);
            Console.WriteLine($"  Output: {o}  ->  {(IsComment(o) ? "Comment" : "Not a comment")}");
        }
    }
}