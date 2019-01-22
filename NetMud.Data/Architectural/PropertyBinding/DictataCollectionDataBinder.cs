﻿using NetMud.DataAccess.Cache;
using NetMud.DataStructure.Architectural.PropertyBinding;
using NetMud.DataStructure.Linguistic;
using System.Collections.Generic;
using System.Linq;

namespace NetMud.Data.Architectural.PropertyBinding
{
    public class DictataCollectionDataBinder : PropertyBinderAttribute
    {
        public override object Convert(object input)
        {
            if (input == null)
                return null;

            var valueCollection = input as IEnumerable<string>;

            var collective = new HashSet<IDictata>(valueCollection.Select(str => ConfigDataCache.Get<IDictata>(str)));

            return collective;
        }
    }
}
