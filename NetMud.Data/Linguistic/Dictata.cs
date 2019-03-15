﻿using NetMud.Communication.Lexical;
using NetMud.Data.Architectural;
using NetMud.Data.Architectural.ActorBase;
using NetMud.Data.Architectural.PropertyBinding;
using NetMud.DataAccess;
using NetMud.DataAccess.Cache;
using NetMud.DataStructure.Administrative;
using NetMud.DataStructure.Architectural;
using NetMud.DataStructure.Linguistic;
using NetMud.DataStructure.Player;
using NetMud.DataStructure.System;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Script.Serialization;

namespace NetMud.Data.Linguistic
{
    /// <summary>
    /// Sort of a partial class of Lexica so it can get stored more easily and work for the processor
    /// </summary>
    [Serializable]
    public class Dictata : ConfigData, IDictata, IComparable<IDictata>, IEquatable<IDictata>, IEqualityComparer<IDictata>
    {
        [ScriptIgnore]
        [JsonIgnore]
        public override ContentApprovalType ApprovalType => ContentApprovalType.ReviewOnly;

        /// <summary>
        /// The unique key used to identify, store and retrieve data
        /// </summary>
        [ScriptIgnore]
        [JsonIgnore]
        public override string UniqueKey => string.Format("{0}_{1}_{2}", Language.Name, WordType.ToString(), Name);

        /// <summary>
        /// Type of configuation data this is
        /// </summary>
        [ScriptIgnore]
        [JsonIgnore]
        public override ConfigDataType Type => ConfigDataType.Dictionary;

        /// <summary>
        /// The unique name of this configuration data
        /// </summary>
        [StringLength(200, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 1)]
        [Display(Name = "Word", Description = "The actual word or phrase at hand.")]
        [DataType(DataType.Text)]
        [Required]
        public override string Name { get; set; }

        /// <summary>
        /// The type of word this is in general
        /// </summary>
        [Display(Name = "Type", Description = "The type of word this is.")]
        [UIHint("EnumDropDownList")]
        [Required]
        public LexicalType WordType { get; set; }

        /// <summary>
        /// Chronological tense of word
        /// </summary>
        [Display(Name = "Tense", Description = "Chronological tense (past, present, future)")]
        [UIHint("EnumDropDownList")]
        [Required]
        public LexicalTense Tense { get; set; }

        /// <summary>
        /// Does this indicate some sort of relational positioning
        /// </summary>
        [Display(Name = "Positional", Description = "Does this indicate some sort of relational positioning")]
        [UIHint("EnumDropDownList")]
        [Required]
        public LexicalPosition Positional { get; set; }

        /// <summary>
        /// Personage of the word
        /// </summary>
        [Display(Name = "Perspective", Description = "Narrative personage for the word.")]
        [UIHint("EnumDropDownList")]
        [Required]
        public NarrativePerspective Perspective { get; set; }

        /// <summary>
        /// Tags that describe the purpose/meaning of the words
        /// </summary>
        [Display(Name = "Semantic Tags", Description = "Tags that describe the purpose/meaning of the word. (like Food or Positional)")]
        [UIHint("TagContainer")]
        public HashSet<string> Semantics { get; set; }

        /// <summary>
        /// Is this a feminine or masculine word
        /// </summary>
        [Display(Name = "Feminine Form", Description = "Is this a feminine or masculine word? (only applies to gendered languages)")]
        [UIHint("Boolean")]
        public bool Feminine { get; set; }

        /// <summary>
        /// Is this an determinant form or not (usually true)
        /// </summary>
        [Display(Name = "Determinant", Description = "Is this an determinant form or not? (usually true)")]
        [UIHint("Boolean")]
        public bool Determinant { get; set; }

        /// <summary>
        /// Is this a plural form
        /// </summary>
        [Display(Name = "Plural", Description = "Is this a plural form?")]
        [UIHint("Boolean")]
        public bool Plural { get; set; }

        /// <summary>
        /// Is this a possessive form
        /// </summary>
        [Display(Name = "Possessive", Description = "Is this a possessive form?")]
        [UIHint("Boolean")]
        public bool Possessive { get; set; }

        /// <summary>
        /// Strength rating of word in relation to synonyms
        /// </summary>
        [Range(0, 100, ErrorMessage = "The {0} must be between {2} and {1}.")]
        [Display(Name = "Severity", Description = "Strength rating of word in relation to synonyms.")]
        [DataType(DataType.Text)]
        public int Severity { get; set; }

        /// <summary>
        /// Synonym rating for elegance
        /// </summary>
        [Range(0, 100, ErrorMessage = "The {0} must be between {2} and {1}.")]
        [Display(Name = "Elegance", Description = "Crudeness rating of word in relation to synonyms.")]
        [DataType(DataType.Text)]
        public int Elegance { get; set; }

        /// <summary>
        /// Finesse synonym rating; execution of form
        /// </summary>
        [Range(0, 100, ErrorMessage = "The {0} must be between {2} and {1}.")]
        [Display(Name = "Quality", Description = "Finesse synonym rating; quality of execution of form or function.")]
        [DataType(DataType.Text)]
        public int Quality { get; set; }

        [JsonProperty("Language")]
        private ConfigDataCacheKey _language { get; set; }

        /// <summary>
        /// The language this is derived from
        /// </summary>
        [ScriptIgnore]
        [JsonIgnore]
        [Display(Name = "Language", Description = "The language this is in.")]
        [UIHint("LanguageList")]
        [LanguageDataBinder]
        [Required]
        public ILanguage Language
        {
            get
            {
                if (_language == null)
                {
                    return null;
                }

                return ConfigDataCache.Get<ILanguage>(_language);
            }
            set
            {
                if (value == null)
                {
                    _language = null;
                    return;
                }

                _language = new ConfigDataCacheKey(value);
            }
        }

        [JsonProperty("Synonyms")]
        private HashSet<ConfigDataCacheKey> _synonyms { get; set; }

        /// <summary>
        /// Things this is the same as mostly
        /// </summary>
        [ScriptIgnore]
        [JsonIgnore]
        [Display(Name = "Synonyms", Description = "The synonyms (similar) of this word/phrase.")]
        [UIHint("CollectionSynonymList")]
        [DictataCollectionDataBinder]
        public HashSet<IDictata> Synonyms
        {
            get
            {
                if (_synonyms == null)
                {
                    _synonyms = new HashSet<ConfigDataCacheKey>();
                }

                return new HashSet<IDictata>(_synonyms.Select(k => ConfigDataCache.Get<IDictata>(k)));
            }
            set
            {
                if (value == null)
                {
                    _synonyms = new HashSet<ConfigDataCacheKey>();
                    return;
                }

                _synonyms = new HashSet<ConfigDataCacheKey>(value.Select(k => new ConfigDataCacheKey(k)));
            }
        }

        [JsonProperty("Antonyms")]
        private HashSet<ConfigDataCacheKey> _antonyms { get; set; }

        /// <summary>
        /// Things this is specifically opposite of mostly
        /// </summary>
        [ScriptIgnore]
        [JsonIgnore]
        [Display(Name = "Antonyms", Description = "The antonyms (opposite) of this word/phrase.")]
        [UIHint("CollectionDictataList")]
        [DictataCollectionDataBinder]
        public HashSet<IDictata> Antonyms
        {
            get
            {
                if (_antonyms == null)
                {
                    _antonyms = new HashSet<ConfigDataCacheKey>();
                }

                return new HashSet<IDictata>(_antonyms.Select(k => ConfigDataCache.Get<IDictata>(k)));
            }
            set
            {
                if (value == null)
                {
                    _antonyms = new HashSet<ConfigDataCacheKey>();
                    return;
                }

                _antonyms = new HashSet<ConfigDataCacheKey>(value.Select(k => new ConfigDataCacheKey(k)));
            }
        }

        [JsonConstructor]
        public Dictata()
        {
            Antonyms = new HashSet<IDictata>();
            Synonyms = new HashSet<IDictata>();
            Semantics = new HashSet<string>();
            Name = string.Empty;

            IGlobalConfig globalConfig = ConfigDataCache.Get<IGlobalConfig>(new ConfigDataCacheKey(typeof(IGlobalConfig), "LiveSettings", ConfigDataType.GameWorld));

            if (globalConfig?.BaseLanguage == null)
            {
                Language = ConfigDataCache.GetAll<ILanguage>().FirstOrDefault();
            }
            else
            {
                Language = globalConfig.BaseLanguage;
            }
        }


        /// <summary>
        /// Make a dictata from a lexica
        /// </summary>
        /// <param name="lexica">the incoming lexica phrase</param>
        public Dictata(ILexica lexica)
        {
            Antonyms = new HashSet<IDictata>();
            Synonyms = new HashSet<IDictata>();
            Semantics = new HashSet<string>();

            Name = lexica.Phrase;
            WordType = lexica.Type;
            Language = lexica.Context?.Language;

            if (Language == null)
            {
                IGlobalConfig globalConfig = ConfigDataCache.Get<IGlobalConfig>(new ConfigDataCacheKey(typeof(IGlobalConfig), "LiveSettings", ConfigDataType.GameWorld));

                if (globalConfig?.BaseLanguage == null)
                {
                    Language = ConfigDataCache.GetAll<ILanguage>().FirstOrDefault();
                }
                else
                {
                    Language = globalConfig.BaseLanguage;
                }
            }
        }

        /// <summary>
        /// Add language translations for this
        /// </summary>
        public void FillLanguages()
        {
            IGlobalConfig globalConfig = ConfigDataCache.Get<IGlobalConfig>(new ConfigDataCacheKey(typeof(IGlobalConfig), "LiveSettings", ConfigDataType.GameWorld));

            //Don't do this if: we have no config, translation is turned off or lacking in the azure key, the language is not a human-ui language
            //it isn't an approved language, the word is a proper noun or the language isnt the base language at all
            if (globalConfig == null || !globalConfig.TranslationActive || string.IsNullOrWhiteSpace(globalConfig.AzureTranslationKey)
                || !Language.UIOnly || !Language.SuitableForUse || WordType == LexicalType.ProperNoun || Language != globalConfig.BaseLanguage)
            {
                return;
            }

            var otherLanguages = ConfigDataCache.GetAll<ILanguage>().Where(lang => lang != Language && lang.SuitableForUse && lang.UIOnly);

            foreach (var language in otherLanguages)
            {
                var context = new LexicalContext(null)
                {
                    Language = language,
                    Perspective = Perspective,
                    Tense = Tense,
                    Position = Positional,
                    Determinant = Determinant,
                    Plural = Plural,
                    Possessive = Possessive,
                    Elegance = Elegance,
                    Quality = Quality,
                    Semantics = Semantics,
                    Severity = Severity,
                    GenderForm = new Gender() { Feminine = true }
                };

                var translatedWord = Thesaurus.GetSynonym(this, context);

                //no linguistic synonym
                if (translatedWord == this)
                {
                    var newWord = Thesaurus.GetTranslatedWord(globalConfig.AzureTranslationKey, Name, Language, language);

                    if (!string.IsNullOrWhiteSpace(newWord))
                    {
                        var newDictata = new Dictata()
                        {
                            Language = language,
                            Name = newWord,
                            Elegance = Elegance,
                            Severity = Severity,
                            Quality = Quality,
                            Determinant = Determinant,
                            Plural = Plural,
                            Perspective = Perspective,
                            Feminine = Feminine,
                            Positional = Positional,
                            Possessive = Possessive,
                            Semantics = Semantics,
                            Antonyms = Antonyms,
                            Synonyms = Synonyms,
                            Tense = Tense,
                            WordType = WordType
                        };

                        newDictata.Synonyms = new HashSet<IDictata>(Synonyms) { this };
                        newDictata.SystemSave();
                        newDictata.PersistToCache();

                        Synonyms = new HashSet<IDictata>(Synonyms) { newDictata };
                    }
                }
            }

            SystemSave();
            PersistToCache();
        }

        /// <summary>
        /// Create a lexica from this
        /// </summary>
        /// <returns></returns>
        public ILexica GetLexica(GrammaticalType role, LexicalType type, LexicalContext context)
        {
            return new Lexica(type, role, Name, context);
        }

        /// <summary>
        /// Get the significant details of what needs approval
        /// </summary>
        /// <returns>A list of strings</returns>
        public override IDictionary<string, string> SignificantDetails()
        {
            IDictionary<string, string> returnList = base.SignificantDetails();

            returnList.Add("WordType", WordType.ToString());

            return returnList;
        }

        #region Equality Functions
        /// <summary>
        /// -99 = null input
        /// -1 = wrong type
        /// 0 = same type, wrong id
        /// 1 = same reference (same id, same type)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object other)
        {
            try
            {
                return CompareTo(other as IDictata);
            }
            catch (Exception ex)
            {
                LoggingUtility.LogError(ex);
            }

            return -99;
        }

        /// <summary>
        /// -99 = null input
        /// -1 = wrong type
        /// 0 = same type, wrong id
        /// 1 = same reference (same id, same type)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(IDictata other)
        {
            if (other != null)
            {
                try
                {
                    if (other.GetType() != GetType())
                    {
                        return -1;
                    }

                    if (other.Name.Equals(Name, StringComparison.InvariantCultureIgnoreCase) && other.WordType == WordType)
                    {
                        return 1;
                    }

                    return 0;
                }
                catch (Exception ex)
                {
                    LoggingUtility.LogError(ex);
                }
            }

            return -99;
        }

        /// <summary>
        /// Compares this object to another one to see if they are the same object
        /// </summary>
        /// <param name="other">the object to compare to</param>
        /// <returns>true if the same object</returns>
        public bool Equals(IDictata other)
        {
            if (other != default(IDictata))
            {
                try
                {
                    return other.Name.Equals(Name, StringComparison.InvariantCultureIgnoreCase) && other.WordType == WordType;
                }
                catch (Exception ex)
                {
                    LoggingUtility.LogError(ex);
                }
            }

            return false;
        }

        /// <summary>
        /// Compares an object to another one to see if they are the same object
        /// </summary>
        /// <param name="x">the object to compare to</param>
        /// <param name="y">the object to compare to</param>
        /// <returns>true if the same object</returns>
        public bool Equals(IDictata x, IDictata y)
        {
            return x.Equals(y);
        }

        /// <summary>
        /// Get the hash code for comparison purposes
        /// </summary>
        /// <param name="obj">the thing to get the hashcode for</param>
        /// <returns>the hash code</returns>
        public int GetHashCode(IDictata obj)
        {
            return obj.GetType().GetHashCode() + obj.WordType.GetHashCode() + obj.Name.GetHashCode();
        }

        /// <summary>
        /// Get the hash code for comparison purposes
        /// </summary>
        /// <returns>the hash code</returns>
        public override int GetHashCode()
        {
            return GetType().GetHashCode() + WordType.GetHashCode() + Name.GetHashCode();
        }

        public override object Clone()
        {
            return new Dictata
            {
                Antonyms = Antonyms,
                Elegance = Elegance,
                Language = Language,
                Name = Name,
                Severity = Severity,
                Synonyms = Synonyms,
                Tense = Tense,
                WordType = WordType,
                Quality = Quality
            };
        }
        #endregion

        #region Data persistence functions
        /// <summary>
        /// Remove this object from the db permenantly
        /// </summary>
        /// <returns>success status</returns>
        public override bool Remove(IAccount remover, StaffRank rank)
        {
            var synonyms = ConfigDataCache.GetAll<IDictata>().Where(dict => dict.Synonyms.Any(syn => syn.Equals(this)));
            var antonyms = ConfigDataCache.GetAll<IDictata>().Where(dict => dict.Antonyms.Any(ant => ant.Equals(this)));
            var removalState = base.Remove(remover, rank);

            if(removalState)
            {
                foreach(var word in synonyms)
                {
                    var syns = new HashSet<IDictata>(word.Synonyms);
                    syns.RemoveWhere(syn => syn.Equals(this));
                    word.Synonyms = syns;
                }

                foreach (var word in antonyms)
                {
                    var ants = new HashSet<IDictata>(word.Antonyms);
                    ants.RemoveWhere(syn => syn.Equals(this));
                    word.Antonyms = ants;
                }
            }

            return removalState;
        }

        /// <summary>
        /// Update the field data for this object to the db
        /// </summary>
        /// <returns>success status</returns>
        public override bool Save(IAccount editor, StaffRank rank)
        {
            var removalState = base.Remove(editor, rank);

            if (removalState)
            {
                return base.Save(editor, rank);
            }

            return false;
        }

        /// <summary>
        /// Update the field data for this object to the db
        /// </summary>
        /// <returns>success status</returns>
        public override bool SystemSave()
        {
            var removalState = base.SystemRemove();

            if (removalState)
            {
                return base.SystemSave();
            }

            return false;
        }

        /// <summary>
        /// Remove this object from the db permenantly
        /// </summary>
        /// <returns>success status</returns>
        public override bool SystemRemove()
        {
            var synonyms = ConfigDataCache.GetAll<IDictata>().Where(dict => dict.Synonyms.Any(syn => syn.Equals(this)));
            var antonyms = ConfigDataCache.GetAll<IDictata>().Where(dict => dict.Antonyms.Any(ant => ant.Equals(this)));
            var removalState = base.SystemRemove();

            if (removalState)
            {
                foreach (var word in synonyms)
                {
                    var syns = new HashSet<IDictata>(word.Synonyms);
                    syns.RemoveWhere(syn => syn.Equals(this));
                    word.Synonyms = syns;
                }

                foreach (var word in antonyms)
                {
                    var ants = new HashSet<IDictata>(word.Antonyms);
                    ants.RemoveWhere(syn => syn.Equals(this));
                    word.Antonyms = ants;
                }
            }

            return removalState;
        }
        #endregion
    }
}
