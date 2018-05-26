﻿using NetMud.DataStructure.Base.System;
using Newtonsoft.Json;
using System;
using System.Web.Script.Serialization;

namespace NetMud.DataAccess.Cache
{
    /// <summary>
    /// A cache key for live entities
    /// </summary>
    [Serializable]
    public class BackingDataCacheKey : ICacheKey
    {
        [JsonIgnore]
        [ScriptIgnore]
        public CacheType CacheType
        {
            get { return CacheType.BackingData; }
        }

        /// <summary>
        /// System type of the object being cached
        /// </summary>
        public Type ObjectType { get; set; }

        /// <summary>
        /// Unique signature for a live object
        /// </summary>
        public long BirthMark { get; set; }

        /// <summary>
        /// Generate a live key for a live object
        /// </summary>
        /// <param name="objectType">System type of the entity being cached</param>
        /// <param name="marker">Unique signature for a live entity</param>
        public BackingDataCacheKey(Type objectType, long marker)
        {
            ObjectType = objectType;
            BirthMark = marker;
        }

        /// <summary>
        /// Make a new cache key using the object
        /// </summary>
        /// <param name="data">the object</param>
        public BackingDataCacheKey(IData data)
        {
            ObjectType = data.GetType();
            BirthMark = data.Id;
        }

        /// <summary>
        /// Hash key used by the cache system
        /// </summary>
        /// <returns>the key's hash</returns>
        public string KeyHash()
        {
            var typeName = ObjectType.Name;

            //Normalize interfaces versus classnames
            if (ObjectType.IsInterface)
                typeName = typeName.Substring(1);

            return string.Format("{0}_{1}_{2}", CacheType.ToString(), typeName, BirthMark.ToString());
        }
    }
}
