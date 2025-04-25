using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace WordPuzzle
{
    public class RandomWordCutter : IWordCutter
    {
        private const int max_failed_cuts = 10;
        private const int MIN_CUT = 1;
        private const int MAX_CUT = 4;
        
        public int MinCut { get; set; } = MIN_CUT;
        public int MaxCut { get; set;} = MAX_CUT;
     
        public string[] Cut(IEnumerable<string> words)
        {
            var cuts = ListPool<string>.Get();
            
            var currentlyCut = ListPool<char>.Get();
            foreach (var word in words)
            {
                currentlyCut.AddRange(word);
                while (currentlyCut.Count > 0)
                {
                    var cutSize = GetCutSize(MinCut, MaxCut, currentlyCut.Count);
                    cuts.Add(new string(currentlyCut.GetRange(0, cutSize).ToArray()));
                    currentlyCut.RemoveRange(0, cutSize);
                }
            }
            ListPool<char>.Release(currentlyCut);
            return cuts.ToArray();
        }

        private int GetCutSize(int min, int max, int currentLength)
        {
            var maxCut = Mathf.Min(max, currentLength);
            var cut = Random.Range(min, maxCut + 1);
            var remainingLength = currentLength - cut;
            if (remainingLength == 0 || remainingLength >= min)
                return cut;
            
            do
            {
                cut = GetCutSize(min, remainingLength, currentLength);
                remainingLength = currentLength - cut;
                
            } while (remainingLength < min);

            return cut;
        }
    }
    

    internal class ImpossibleWordCutException : Exception
    {
        public override string Message { get; }

        public ImpossibleWordCutException(string word, string partToCut, int minCut, int maxCut)
        {
            this.Message = $"Impossible to cut the word's '{word}' remaining part: '{partToCut}' with settings: " +
                           $"MinCut {minCut}, MaxCut: {maxCut}";
        }
    }
}