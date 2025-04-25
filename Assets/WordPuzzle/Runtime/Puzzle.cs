using System;
using System.Collections.Generic;
using UnityEngine;

namespace WordPuzzle
{
    [Serializable]
    public class Puzzle
    {
        // With JsonUtility properties names must match the JSON properties exactly
        [SerializeField]
        private int word_length;
        
        [SerializeField]
        private string[] words;

        public IReadOnlyList<string> Words => words;

        public int WordLength => word_length;

        public Puzzle() { }

        public Puzzle(int wordLength, string[] words)
        {
            this.word_length = wordLength;
            this.words = words;
        }
    }
}