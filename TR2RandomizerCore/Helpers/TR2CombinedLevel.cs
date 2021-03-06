﻿using System.Collections.Generic;
using System.Linq;
using TR2RandomizerCore.Utilities;
using TRGE.Core;
using TRLevelReader.Helpers;
using TRLevelReader.Model;
using TRLevelReader.Model.Enums;

namespace TR2RandomizerCore.Helpers
{
    public class TR2CombinedLevel
    {
        /// <summary>
        /// The main level data stored in the corresponding .TR2 file.
        /// </summary>
        public TR2Level Data { get; set; }

        /// <summary>
        /// The scripting information for the level stored in TOMBPC.dat.
        /// </summary>
        public TR23ScriptedLevel Script { get; set; }

        /// <summary>
        /// The uppercase base file name of the level e.g. KEEL.TR2
        /// </summary>
        public string Name => Script.LevelFileBaseName.ToUpper();

        /// <summary>
        /// Compares the given file name or path against the base file name of the level (case-insensitive).
        /// </summary>
        public bool Is(string levelFileName) => Script.Is(levelFileName);

        /// <summary>
        /// Determines if the given level is specific to UKBox. This currently applies only to Floating Islands which differs between UKBox and EPC/Multipatch.
        /// </summary>
        public bool IsUKBox => Is(LevelNames.FLOATER) && Data.NumBoxes < 859; // This is a bit arbitrary, but EPC and Multipatch have 859, UKBox has 857

        /// <summary>
        /// Returns {Name}-UKBox if this level is for UKBox, otherwise just {Name}.
        /// </summary>
        public string JsonID => IsUKBox ? Name + "-UKBox" : Name;

        /// <summary>
        /// Checks if the current level is the assault course.
        /// </summary>
        public bool IsAssault => Is(LevelNames.ASSAULT);

        public bool CanPerformDraining(short room)
        {
            foreach (List<int> area in RoomWaterUtilities.RoomRemovalWaterMap[Name])
            {
                if (area.Contains(room))
                {
                    return true;
                }
            }

            return false;
        }

        public bool PerformDraining(short room)
        {
            foreach (List<int> area in RoomWaterUtilities.RoomRemovalWaterMap[Name])
            {
                if (area.Contains(room))
                {
                    foreach (int filledRoom in area)
                    {
                        Data.Rooms[filledRoom].Drain();
                    }

                    return true;
                }
            }

            return false;
        }

        public List<TR2Entity> GetEnemyEntities()
        {
            List<TR2Entities> allEnemies = TR2EntityUtilities.GetFullListOfEnemies();
            List<TR2Entity> levelEntities = new List<TR2Entity>();
            for (int i = 0; i < Data.NumEntities; i++)
            {
                TR2Entity entity = Data.Entities[i];
                if (allEnemies.Contains((TR2Entities)entity.TypeID))
                {
                    levelEntities.Add(entity);
                }
            }
            return levelEntities;
        }

        public int GetMaximumEntityLimit()
        {
            int limit = 256;

            // #153 The game creates a black skidoo for each skidoo driver when the level
            // is loaded, so there needs to be space in the entity array for these.
            List<TR2Entity> entities = Data.Entities.ToList();
            limit -= entities.FindAll(e => e.TypeID == (short)TR2Entities.MercSnowmobDriver).Count;

            // If there is a dragon, we need an extra 7 slots for the front bones, 
            // back bones etc. This is going by what's seen in Dragon.c
            if (entities.FindIndex(e => e.TypeID == (short)TR2Entities.MarcoBartoli) != -1)
            {
                limit -= 7;
            }

            return limit;
        }
    }
}