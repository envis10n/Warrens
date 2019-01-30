﻿using NetMud.Authentication;
using NetMud.DataStructure.Architectural;
using NetMud.DataStructure.Linguistic;
using System.ComponentModel.DataAnnotations;

namespace NetMud.Models.Admin
{
    public partial class OccurrenceViewModel : LexicaViewModel, IBaseViewModel
    {
        public ApplicationUser authedUser { get; set; }

        public string AdminTypeName { get; set; }
        public string DataUnitTitle { get; set; }

        [UIHint("SensoryEvent")]
        public ISensoryEvent SensoryEventDataObject { get; set; }
        public IKeyedData DataObject { get; set; }
    }

    public partial class LexicaViewModel
    {
        public ILexica LexicaDataObject { get; set; }
    }
}