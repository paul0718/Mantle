using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JsonModel
{
    [Serializable]
    public class Battle 
    {
        public string Enemy;
        public string Background;

        public Dictionary<int,Minigame> Minigames;
        public Dictionary<int,EnemyMinigame> EnemyMinigames;
        public string IntroDialoguePath;
        public string BarkDialoguePath;
        public string OutroDialoguePath;
        public string LoseSceneDialoguePath;

        public Dictionary<int, EndStates> endStates;
        public Vector2 StartPos;
        public Vector2 DisarmPos;
        
        public string BGM;

        public RepairParameters RepairParameters;
        public BlockParameters BlockParameters;
    }
    [Serializable]
    public class Minigame
    {
        public Vector2 WinEffect;
        public Vector2 LoseEffect;
    }
    [Serializable]
    public class EnemyMinigame
    {
        public Vector2 LoseEffect;
    }

    [Serializable]
    public class EndStates
    {
        public Vector2 Pos;
    }

    [Serializable]
    public class RepairParameters
    {
        public int KnockOffPerAttack;
        public int AttackInterval;
    }
    
    [Serializable]
    public class BlockParameters
    {
        public float AttackInterval;
    }
}

