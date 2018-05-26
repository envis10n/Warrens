﻿using NetMud.Data.DataIntegrity;
using NetMud.DataAccess.Cache;
using NetMud.DataStructure.Base.Supporting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace NetMud.Data.LookupData
{
    /// <summary>
    /// Rocks, minable metals and dirt
    /// </summary>
    [Serializable]
    public class Mineral : NaturalResourceDataPartial, IMineral
    {
        /// <summary>
        /// How soluble the dirt is
        /// </summary>
        public int Solubility { get; set; }

        /// <summary>
        /// How fertile the dirt generally is
        /// </summary>
        public int Fertility { get; set; }

        [JsonProperty("Rock")]
        private BackingDataCacheKey _rock { get; set; }

        /// <summary>
        /// What is the solid, crystallized form of this
        /// </summary>
        [JsonIgnore]
        [ScriptIgnore]
        [NonNullableDataIntegrity("Rock must have a value.")]
        public IMaterial Rock
        {
            get
            {
                return BackingDataCache.Get<IMaterial>(_rock);
            }
            set
            {
                _rock = new BackingDataCacheKey(value);
            }
        }

        [JsonProperty("Dirt")]
        private BackingDataCacheKey _dirt { get; set; }

        /// <summary>
        /// What is the scattered, ground form of this
        /// </summary>
        [JsonIgnore]
        [ScriptIgnore]
        [NonNullableDataIntegrity("Dirt must have a value.")]
        public IMaterial Dirt
        {
            get
            {
                return BackingDataCache.Get<IMaterial>(_dirt);
            }
            set
            {
                _dirt = new BackingDataCacheKey(value);
            }
        }

        [JsonProperty("Ores")]
        private IEnumerable<BackingDataCacheKey> _ores { get; set; }

        /// <summary>
        /// What medium minerals this can spawn in
        /// </summary>
        [JsonIgnore]
        [ScriptIgnore]
        public IEnumerable<IMineral> Ores
        {
            get
            {
                if (_ores == null)
                    _ores = new HashSet<BackingDataCacheKey>();

                return BackingDataCache.GetMany<IMineral>(_ores);
            }
            set
            {
                _ores = value.Select(m => new BackingDataCacheKey(m));
            }
        }
    }
}
