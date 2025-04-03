using System;
using UnityEngine;

namespace Excel_TO_SO.Scripts
{
    [CreateAssetMenu]
    public class TestReader : ReaderSoBase<TestData>
    {
        
    }
    
    [Serializable]
    public class TestData : IParseable
    {
        public int ID;
        public string name;
        public int health;
        public int attack;
        
        public void ParseDataAndInit(string[] fields)
        {
            ID = int.Parse(fields[0].Trim()); // Trim移除空格
            name = fields[1].Trim();
            health = int.Parse(fields[2].Trim()); // Trim移除空格
            attack = int.Parse(fields[3].Trim()); // Trim移除空格
        }
    }
}