﻿using NetMud.DataStructure.Base.Supporting;
using NetMud.DataStructure.Base.System;
using NetMud.DataStructure.Behaviors.System;
using System.Collections.Generic;

namespace NetMud.DataStructure.Base.Place
{
    /// <summary>
    /// Collector of rooms, used for weather patterning
    /// </summary>
    public interface IZoneData : IEntityBackingData, IEnvironmentData, IDiscoverableData, ISingleton
    {
        /// <summary>
        /// The midline elevation point "sea level" for this zone
        /// </summary>
        int BaseElevation { get; set; }

        /// <summary>
        /// Templates for generating randomized locales for zones
        /// </summary>
        HashSet<IAdventureTemplate> Templates { get; set; }

        /// <summary>
        /// Perm locales within this zone should the system need to regenerate from scratch
        /// </summary>
        HashSet<IHorizonData<ILocaleData>> Locales { get; set; }

        /// What other zones does this zone exit to and are they initially visible
        /// </summary>
        HashSet<IHorizonData<IZoneData>> ConnectedZones { get; set; }
    }
}
