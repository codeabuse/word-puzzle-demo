using System;
using Codeabuse;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WordPuzzle
{
    public class ConditionalOperation : MonoBehaviour, IStartupProcedure
    {
        [SerializeField]
        private ConditionType _conditionType;

        [SerializeField]
        private PlayerPrefsMatch _playerPrefsMatch;

        [SerializeField, WithInterface(typeof(IStartupProcedure))]
        private Component _optionA;
        [SerializeField, WithInterface(typeof(IStartupProcedure))]
        private Component _optionB;
        
        protected IStartupProcedure optionA => _optionA as IStartupProcedure;
        protected IStartupProcedure optionB => _optionB as IStartupProcedure;

        protected virtual bool Match()
        {
            return true;
        }

        public UniTask Load()
        {
            var match = _conditionType switch {
                    ConditionType.PlayerPrefs => _playerPrefsMatch.Match(),
                    ConditionType.Code => Match(),
                    _ => throw new ArgumentOutOfRangeException()
            };
            return match ? optionA.Load() : optionB.Load();
        }
    }

    [Serializable]
    public struct PlayerPrefsMatch
    {
        [SerializeField]
        private string _keyName;
        [SerializeField]
        private PrefValueType _valueType;
        [SerializeField]
        private string _stringValue;
        [SerializeField]
        private string _defaultSstringValue;
        [SerializeField]
        private int _intValue;
        [SerializeField]
        private int _defaultIntValue;
        [SerializeField]
        private float _floatValue;
        [SerializeField]
        private float _defaultFloatValue;

        [SerializeField]
        private ComparisonType _comparison;

        public bool Match()
        {
            switch (_valueType)
            {
                case PrefValueType.Int:
                    var intPref = PlayerPrefs.GetInt(_keyName, _defaultIntValue);
                    switch (_comparison)
                    {
                        case ComparisonType.Less:
                            return intPref < _intValue;
                        
                        case ComparisonType.LessOrEqual:
                            return intPref <= _intValue;
                        
                        case ComparisonType.Equal:
                            return intPref == _intValue;
                        
                        case ComparisonType.GreaterOrEqual:
                            
                            return intPref >= _intValue;
                        
                        case ComparisonType.Greater:
                            return intPref > _intValue;
                        
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case PrefValueType.Float:
                    var floatPref = PlayerPrefs.GetFloat(_keyName, _defaultFloatValue);
                    switch (_comparison)
                    {
                        case ComparisonType.Less:
                            return floatPref < _floatValue;
                        
                        case ComparisonType.LessOrEqual:
                            return floatPref <= _floatValue;
                        
                        case ComparisonType.Equal:
                            return Mathf.Approximately(floatPref, _floatValue);
                        
                        case ComparisonType.GreaterOrEqual:
                            return floatPref >= _floatValue;
                        
                        case ComparisonType.Greater:
                            return floatPref > _floatValue;
                        
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case PrefValueType.String:
                    return PlayerPrefs.GetString(_keyName, _defaultSstringValue) == _stringValue;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    internal enum PrefValueType
    {
        Int,
        Float,
        String
    }

    internal enum ComparisonType
    {
        Less,
        LessOrEqual,
        Equal,
        GreaterOrEqual,
        Greater
    }

    internal enum ConditionType
    {
        PlayerPrefs,
        Code
    }
}