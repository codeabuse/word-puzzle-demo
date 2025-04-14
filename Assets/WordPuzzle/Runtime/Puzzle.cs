using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace WordPuzzle
{
    [Serializable]
    public class Puzzle
    {
        public const string word_length_property = nameof(word_length);
        public const string words_property = nameof(words);
        
        public const int max_word_length = 12;
        public const int min_word_length = 4;

        [FormerlySerializedAs("_wordLength")]
        [SerializeField]
        private int word_length;
        
        [FormerlySerializedAs("_words")]
        [SerializeField]
        private string[] words;

        public string[] Words => words;

        public int WordLength => word_length;

        public Puzzle() { }

        public Puzzle(int wordLength, string[] words)
        {
            this.word_length = wordLength;
            this.words = words;
        }
    }
}