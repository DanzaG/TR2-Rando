﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRLevelReader;
using TRLevelReader.Helpers;
using TRLevelReader.Model;
using TRLevelReader.Model.Enums;

namespace TR2Randomizer
{
    public class EnemyRandomizer
    {
        private TR2LevelReader _reader;
        private TR2LevelWriter _writer;
        private TR2Level _levelInstance;
        private Random _generator;
        private List<string> _levels;

        public EnemyRandomizer()
        {
            _levels = LevelNames.AsList;

            _reader = new TR2LevelReader();
            _writer = new TR2LevelWriter();
        }

        public void Randomize(int seed)
        {
            ReplacementStatusManager.CanRandomize = false;

            _generator = new Random(seed);

            foreach (string lvl in _levels)
            {
                //Read the level into a level object
                _levelInstance = _reader.ReadLevel(lvl);

                //Apply the modifications
                RandomizeEnemyTypes(lvl);

                //Write back the level file
                _writer.WriteLevelToFile(_levelInstance, lvl);

                ReplacementStatusManager.LevelProgress++;
            }

            ReplacementStatusManager.LevelProgress = 0;
            ReplacementStatusManager.CanRandomize = true;
        }

        private void RandomizeEnemyTypes(string lvl)
        {
            List<TR2Entities> EnemyTypes = TR2EntityUtilities.GetEnemyTypeDictionary()[lvl];

            for (int i = 0; i < _levelInstance.Entities.Count(); i++)
            {
                if (EnemyTypes.Contains((TR2Entities)_levelInstance.Entities[i].TypeID))
                {
                    _levelInstance.Entities[i].TypeID = (short)EnemyTypes[_generator.Next(0, EnemyTypes.Count)];
                }
            }
        }
    }
}
