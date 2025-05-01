using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace WordPuzzle
{
    public class RandomWordCutter : IWordCutter
    {
        private const int max_failed_cuts = 10;
        private const int MIN_CUT = 2;
        private const int MAX_CUT = 4;
        
        public int MinCut { get; set; } = MIN_CUT;
        public int MaxCut { get; set;} = MAX_CUT;
     
        public string[] Cut(IEnumerable<string> words)
        {
            var cuts = ListPool<string>.Get();
            
            var currentlyCut = ListPool<char>.Get();
            foreach (var word in words)
            {
                Cut(word, MinCut, MaxCut, cuts);
            }
            ListPool<char>.Release(currentlyCut);
            return cuts.ToArray();
        }

        private void Cut(string word, int minCut, int maxCut, List<string> cuts)
        {
            var cuttingBoard = ListPool<char>.Get();
            cuttingBoard.AddRange(word);
            
            while (cuttingBoard.Count > 0)
            {
                var cutSize = Random.Range(minCut, Mathf.Min(maxCut, cuttingBoard.Count) + 1);
                var remainingLength = cuttingBoard.Count - cutSize;
                if (remainingLength < minCut)
                {
                    cutSize = minCut;
                    remainingLength = cuttingBoard.Count - cutSize;
                    while (remainingLength > 0 && remainingLength < minCut && cutSize < maxCut)
                    {
                        cutSize++;
                        remainingLength = cuttingBoard.Count - cutSize;
                        if (remainingLength == 0 || remainingLength >= minCut)
                            break;
                    }
                }
                cuts.Add(new string(cuttingBoard.GetRange(0, cutSize).ToArray()));
                cuttingBoard.RemoveRange(0, cutSize);
            }
            
            ListPool<char>.Release(cuttingBoard);
        }
    }
}