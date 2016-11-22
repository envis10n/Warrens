﻿using NetMud.DataStructure.Base.Place;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMud.Cartography.ProceduralGeneration
{
    /// <summary>
    /// Generate some zones procedurally
    /// </summary>
    public class ZoneGenerator
    {
        private Random _randomizer;
        private const string roomSymbol = "*";

        /// <summary>
        /// The rand seed
        /// </summary>
        public int Seed { get; private set; }

        /// <summary>
        /// Width of the zone (X axis)
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Length of the zone (Y axis)
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// Height of zone above center line
        /// </summary>
        public int Elevation { get; private set; }

        /// <summary>
        /// Height of zone under center line
        /// </summary>
        public int Depth { get; private set; }

        /// <summary>
        /// Diameter of the zone array
        /// </summary>
        public int Diameter { get; private set; }

        /// <summary>
        /// Radius of the zone array
        /// </summary>
        public int Radius { get; private set; }

        /// <summary>
        /// Center room x,y,z of the array
        /// </summary>
        public Tuple<int, int, int> Center { get; private set; }

        /// <summary>
        /// The zone we're filling
        /// </summary>
        public IZone Zone { get; private set; }

        /// <summary>
        /// The room map array
        /// </summary>
        public long[, ,] RoomMap { get; private set; }

        /// <summary>
        /// Is this zone ready to generate rooms for
        /// </summary>
        public bool Primed { get; private set; }

        public ZoneGenerator(int seed, IZone zone, int width, int length, int elevation, int depth)
        {
            VerifyZone(zone);
            VerifyDimensions(width, length, elevation, depth);

            _randomizer = new Random(Seed);

            Seed = seed;
            Zone = zone;

            Width = width;
            Length = length;
            Elevation = elevation;
            Depth = depth;

            RoomMap = new long[width * 3 + 1, length * 3 + 1, (elevation + depth) * 3 + 1];
        }

        public ZoneGenerator(IZone zone, int width, int length, int elevation, int depth)
        {
            VerifyZone(zone);
            VerifyDimensions(width, length, elevation, depth);

            var rand = new System.Random();
            _randomizer = new Random(Seed);

            Seed = rand.Next(10000);
            Zone = zone;
            Width = width;
            Length = length;
            Elevation = elevation;
            Depth = depth;

            RoomMap = new long[width * 3 + 1, length * 3 + 1, (elevation + depth) * 3 + 1];
        }

        /// <summary>
        /// Creates the map, but only the array, does not create the rooms
        /// </summary>
        public void FillMap()
        {
            //We'll build the potential rooms first just using strings
            var prototypeMap = new string[Width * 3 + 1, Length * 3 + 1, (Elevation + Depth) * 3 + 1];
            var center = new Tuple<int, int, int>(Width * 3 / 2 + 1, Length * 3 / 2 + 1, (Elevation + Depth) * 3 / 2 + 1);

            //Find the absolute max boundings
            var maxX = prototypeMap.GetUpperBound(0);
            var maxY = prototypeMap.GetUpperBound(1);
            var maxZ = prototypeMap.GetUpperBound(2);

            //Set up center room
            prototypeMap[center.Item1, center.Item2, center.Item3] = roomSymbol;

            var currentX = center.Item1;
            var currentY = center.Item2;
            var currentZ = center.Item3;

            //Do 4 point cardinal directions
            for (var variance = 1; currentX < maxX || currentY < maxY || currentZ < maxZ; variance++)
            {
                //Room or pathway?
                bool isRoom = variance % 3 == 0;

                //Do X, don't do it if we're at or over max bounding for X specifically
                if(currentX + 1 < maxX)
                {
                    var roll = _randomizer.Next(1, 100);

                    if(isRoom && roll >= 25)
                    {
                        prototypeMap[center.Item1 + variance, currentY, currentZ] = roomSymbol;

                        if(roll >= 50)
                            prototypeMap[center.Item1 - variance, currentY, currentZ] = roomSymbol;
                    }
                    else if(roll >= 50)
                    {
                        prototypeMap[center.Item1 + variance, currentY, currentZ] = "-";

                        if (roll >= 75)
                            prototypeMap[center.Item1 - variance, currentY, currentZ] = "-";
                    }

                    currentX++;
                }

                //Do Y
                if (currentY + 1 < maxY)
                {
                    var roll = _randomizer.Next(1, 100);

                    if (isRoom && roll >= 25)
                    {
                        prototypeMap[center.Item1, currentY + variance, currentZ] = roomSymbol;

                        if (roll >= 50)
                            prototypeMap[center.Item1, currentY - variance, currentZ] = roomSymbol;
                    }
                    else if (roll >= 50)
                    {
                        prototypeMap[center.Item1, currentY + variance, currentZ] = "|";

                        if (roll >= 75)
                            prototypeMap[center.Item1, currentY - variance, currentZ] = "|";
                    }

                    currentY++;
                }

                //Do Z
                if (currentZ + 1 < maxZ)
                {
                    var roll = _randomizer.Next(1, 100);

                    if (isRoom && roll >= 25)
                    {
                        prototypeMap[center.Item1, currentY, currentZ + variance] = roomSymbol;

                        if (roll >= 50)
                            prototypeMap[center.Item1, currentY, currentZ - variance] = roomSymbol;
                    }
                    else if (roll >= 50)
                    {
                        prototypeMap[center.Item1, currentY, currentZ + variance] = "^";

                        if (roll >= 75)
                            prototypeMap[center.Item1, currentY, currentZ - variance] = "v";
                    }

                    currentZ++;
                }
            }

            //Go through and do diag cardinal directions.

            //Do "cave" entrances (sloped down) and hills (sloped up)

            //Verify grid

            //Prime it after verification
            Primed = true;
        }

        public void ExecuteMap()
        {
            if (!Primed)
                throw new AccessViolationException("Map is not primed yet.");

            //Create rooms and pathways
        }

        /// <summary>
        /// We just throw errors, no need for a return value
        /// </summary>
        private void VerifyZone(IZone zone)
        {
            if (zone == null)
                throw new ArgumentNullException("Zone must not be null.");

            if (zone.Rooms().Any())
                throw new ArgumentOutOfRangeException("Zone must be devoid of rooms.");

            if (zone.FitnessProblems)
                throw new ArgumentOutOfRangeException("Zone must have data integrity.");
        }

        private void VerifyDimensions(int width, int length, int elevation, int depth)
        {
            if (width < 1 || length < 1 || elevation + depth < 0)
                throw new ArgumentOutOfRangeException("Width and length must be at least 1 and elevation + depth must be at least zero.");

            if (width > 100 || length > 100 || elevation > 100 || depth > 100)
                throw new ArgumentOutOfRangeException("None of the dimensions can be greater than 100.");
        }
    }
}
