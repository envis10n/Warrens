﻿using NetMud.Data.DataIntegrity;
using NetMud.Data.EntityBackingData;
using NetMud.Data.System;
using NetMud.DataAccess.Cache;
using NetMud.DataStructure.Base.Place;
using NetMud.DataStructure.Base.System;
using NetMud.DataStructure.Behaviors.Existential;
using NetMud.DataStructure.Behaviors.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace NetMud.Data.Game
{
    /// <summary>
    /// Live locale (collection of rooms in a zone)
    /// </summary>
    public class Locale : EntityPartial, ILocale
    {
        /// <summary>
        /// The name of the object in the data template
        /// </summary>
        public override string DataTemplateName
        {
            get
            {
                if (DataTemplate<ILocaleData>() == null)
                    return String.Empty;

                return DataTemplate<ILocaleData>().Name;
            }
        }

        /// <summary>
        /// The name used in the tag for discovery checking
        /// </summary>
        public string DiscoveryName
        {
            get
            {
                return "Locale_" + DataTemplateName;
            }
        }

        /// <summary>
        /// The interior map of the locale
        /// </summary>
        [JsonIgnore]
        [ScriptIgnore]
        public IMap Interior { get; set; }

        [JsonProperty("ParentLocation")]
        private LiveCacheKey _affiliation { get; set; }

        /// <summary>
        /// The zone this belongs to
        /// </summary>
        [ScriptIgnore]
        [JsonIgnore]
        [NonNullableDataIntegrity("Locales must have a zone affiliation.")]
        public IZone ParentLocation
        {
            get
            {
                return LiveCache.Get<IZone>(_affiliation);
            }
            set
            {
                if (value != null)
                    _affiliation = new LiveCacheKey(typeof(IZone), value.BirthMark);
            }
        }

        [JsonProperty("Rooms")]
        private IEnumerable<string> _rooms { get; set; }

        /// <summary>
        /// Live rooms in this locale
        /// </summary>
        [ScriptIgnore]
        [JsonIgnore]
        public IEnumerable<IRoom> Rooms
        {
            get
            {
                if (_rooms != null)
                    return new HashSet<IRoom>(LiveCache.GetMany<IRoom>(_rooms));

                return null;
            }
            set
            {
                if (value == null)
                    return;

                _rooms = new HashSet<string>(value.Select(k => k.BirthMark));
            }
        }

        /// <summary>
        /// New up a "blank" zone entry
        /// </summary>
        public Locale()
        {
        }

        /// <summary>
        /// News up an entity with its backing data
        /// </summary>
        /// <param name="room">the backing data</param>
        public Locale(ILocaleData locale)
        {
            DataTemplateId = locale.ID;

            GetFromWorldOrSpawn();
        }

        /// <summary>
        /// Does this entity know about this thing
        /// </summary>
        /// <param name="discoverer">The onlooker</param>
        /// <returns>If this is known to the discoverer</returns>
        public bool IsDiscovered(IEntity discoverer)
        {
            if (DataTemplate<ILocaleData>().AlwaysDiscovered)
                return true;

            //TODO

            //discoverer.HasAccomplishment(DiscoveryName);

            //For now
            return true;
        }

        /// <summary>
        /// The center room of the specific zindex plane. TODO: Not sure if this should be a thing
        /// </summary>
        /// <param name="zIndex">The Z plane to find the central room for</param>
        /// <returns>The central room</returns>
        public IRoom CentralRoom(int zIndex = -1)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// How big (on average) this is in all 3 dimensions
        /// </summary>
        /// <returns>dimensional size</returns>
        public Tuple<int, int, int> Diameter()
        {
            //TODO
            return new Tuple<int, int, int>(1, 1, 1);
        }

        /// <summary>
        /// Absolute max dimensions in each direction
        /// </summary>
        /// <returns>absolute max dimensional size</returns>
        public Tuple<int, int, int> FullDimensions()
        {
            //TODO
            return new Tuple<int, int, int>(1, 1, 1);
        }

        /// <summary>
        /// Render the locale to a specific look
        /// </summary>
        /// <param name="actor">Who is looking</param>
        /// <returns>The locale's description</returns>
        public override IEnumerable<string> RenderToLook(IEntity actor)
        {
            yield return string.Empty;
        }

        /// <summary>
        /// Spawn this into the world and live cache
        /// </summary>
        public override void SpawnNewInWorld()
        {
            SpawnNewInWorld(new GlobalPosition(ParentLocation));
        }

        /// <summary>
        /// Spawn this into the world and live cache
        /// </summary>
        public override void SpawnNewInWorld(IGlobalPosition spawnTo)
        {
            var dataTemplate = DataTemplate<ILocaleData>();

            BirthMark = LiveCache.GetUniqueIdentifier(dataTemplate);
            Keywords = new string[] { dataTemplate.Name.ToLower() };
            Birthdate = DateTime.Now;
        }

        /// <summary>
        /// Get this from the world or make a new one and put it in
        /// </summary>
        public void GetFromWorldOrSpawn()
        {
            //Try to see if they are already there
            var me = LiveCache.Get<ILocale>(DataTemplateId, typeof(LocaleData));

            //Isn't in the world currently
            if (me == default(ILocale))
                SpawnNewInWorld();
            else
            {
                BirthMark = me.BirthMark;
                Birthdate = me.Birthdate;
                DataTemplateId = me.DataTemplateId;
                Keywords = me.Keywords;
                CurrentLocation = me.CurrentLocation;
            }
        }

        /// <summary>
        /// Gets the model dimensions, actually a passthru to FullDimensions
        /// </summary>
        /// <returns></returns>
        public override Tuple<int, int, int> GetModelDimensions()
        {
            return FullDimensions();
        }

        /// <summary>
        /// Renders the map
        /// </summary>
        /// <param name="zIndex">the Z plane to render flat</param>
        /// <param name="forAdmin">Is this visibility agnostic</param>
        /// <returns>The rendered flat map</returns>
        public string RenderMap(int zIndex, bool forAdmin = false)
        {
            return string.Empty;
        }

        /// <summary>
        /// Get adjascent surrounding locales and zones
        /// </summary>
        /// <returns>The adjascent locales and zones</returns>
        public IEnumerable<ILocation> GetSurroundings()
        {
            //TODO
            return Enumerable.Empty<ILocation>();
        }

        /// <summary>
        /// Get the live version of this in the world
        /// </summary>
        /// <returns>The live data</returns>
        public ILocale GetLiveInstance()
        {
            return this;
        }
    }
}
