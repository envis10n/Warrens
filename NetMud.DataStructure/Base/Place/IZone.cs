﻿using NetMud.DataStructure.Base.Supporting;
using NetMud.DataStructure.Base.System;
using NetMud.DataStructure.Behaviors.Rendering;
using NetMud.DataStructure.Behaviors.System;
using System.Collections.Generic;

namespace NetMud.DataStructure.Base.Place
{
    /// <summary>
    /// Collector of locales, used for weather and herd patterning
    /// </summary>
    public interface IZone : IActor, ILocation, IDiscoverable, ISpawnAsSingleton
    {
        /// <summary>
        /// Locales within this zone
        /// </summary>
        HashSet<IHorizon<IRoom>> Horizons { get; set; }

        /// <summary>
        /// Create a new randomized locale based on the template requested
        /// </summary>
        /// <param name="name">The name of the template requested, blank = use random</param>
        /// <returns>The locale generated</returns>
        ILocale GenerateAdventure(string templateName = "");

        /// <summary>
        /// Get the zones this exits to (factors in visibility)
        /// </summary>
        /// <param name="viewer">the entity looking</param>
        /// <returns>valid zones you can go to</returns>
        IEnumerable<IHorizon<IZone>> GetVisibleZoneHorizons(IEntity viewer);

        /// <summary>
        /// Get the locales this exits to (factors in visibility)
        /// </summary>
        /// <param name="viewer">the entity looking</param>
        /// <returns>valid locales you can go to</returns>
        IEnumerable<IHorizon<IRoom>> GetVisibleLocaleHorizons(IEntity viewer);
    }
}
