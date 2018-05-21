﻿using NetMud.Data.Game;
using NetMud.DataAccess.Cache;
using NetMud.DataStructure.Base.Place;
using NetMud.DataStructure.Base.Supporting;
using NetMud.DataStructure.Behaviors.Rendering;
using NetMud.DataStructure.Behaviors.System;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace NetMud.Data.EntityBackingData
{
    public class ZoneData : LocationDataEntityPartial, IZoneData
    {
        /// <summary>
        /// The system type of data this attaches to
        /// </summary>
        public override Type EntityClass
        {
            get { return typeof(Zone); }
        }

        /// <summary>
        /// Base elevation used in generating locales
        /// </summary>
        public int BaseElevation { get; set; }

        /// <summary>
        /// Temperature variance for generating locales
        /// </summary>
        public int TemperatureCoefficient { get; set; }

        /// <summary>
        /// Barometric variance for generating locales
        /// </summary>
        public int PressureCoefficient { get; set; }

        /// <summary>
        /// What the natural biome is for generating locales
        /// </summary>
        public Biome BaseBiome { get; set; }

        /// <summary>
        /// Is this zone discoverable?
        /// </summary>
        public bool AlwaysDiscovered { get; set; }

        [JsonProperty("Templates")]
        private HashSet<long> _templates { get; set; }

        /// <summary>
        /// Adventure templates valid for this zone
        /// </summary>
        [ScriptIgnore]
        [JsonIgnore]
        public HashSet<IAdventureTemplate> Templates
        {
            get
            {
                if (_templates != null)
                    return new HashSet<IAdventureTemplate>(BackingDataCache.GetMany<IAdventureTemplate>(_templates));

                return null;
            }
            set
            {
                if (value == null)
                    return;

                _templates = new HashSet<long>(value.Select(k => k.ID));
            }
        }

        [JsonProperty("NaturalResourceSpawn")]
        private IDictionary<long, int> _naturalResourceSpawn { get; set; }

        /// <summary>
        /// Collection of model section name to material composition mappings
        /// </summary>
        [ScriptIgnore]
        [JsonIgnore]
        public IDictionary<INaturalResource, int> NaturalResourceSpawn
        {
            get
            {
                if (_naturalResourceSpawn != null)
                    return _naturalResourceSpawn.ToDictionary(k => BackingDataCache.Get<INaturalResource>(k.Key), k => k.Value);

                return null;
            }
            set
            {
                if (value == null)
                    return;

                _naturalResourceSpawn = value.ToDictionary(k => k.Key.ID, k => k.Value);
            }
        }

        /// <summary>
        /// Blank constructor
        /// </summary>
        public ZoneData()
        {
            Templates = new HashSet<IAdventureTemplate>();
            NaturalResourceSpawn = new Dictionary<INaturalResource, int>();
        }

        /// <summary>
        /// Get the total rough dimensions of the zone
        /// </summary>
        /// <returns></returns>
        public override Tuple<int, int, int> GetModelDimensions()
        {
            //TODO
            return new Tuple<int, int, int>(1, 1, 1);
        }

        /// <summary>
        /// Get the live version of this in the world
        /// </summary>
        /// <returns>The live data</returns>
        public override ILocation GetLiveInstance()
        {
            return LiveCache.Get<IZone>(ID);
        }
    }
}
