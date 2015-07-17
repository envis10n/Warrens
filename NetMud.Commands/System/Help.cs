﻿using NetMud.DataStructure.Base.System;
using NetMud.DataStructure.Behaviors.Rendering;
using NutMud.Commands.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NutMud.Commands.System
{
    //Really help can be invoked on anything that is helpful, even itself
    [CommandKeyword("Help")]
    [CommandPermission(StaffRank.Player)]
    [CommandParameter(CommandUsage.Subject, typeof(IHelpful), new CacheReferenceType[] { CacheReferenceType.Help, CacheReferenceType.Code }, false )] 
    public class Help : ICommand, IHelpful
    {
        public object Subject { get; set; }
        public object Target { get; set; }
        public object Supporting { get; set; }
        public ILocation OriginLocation { get; set; }
        public IEnumerable<ILocation> Surroundings { get; set; }

        public Help()
        {
            //Generic constructor for all IHelpfuls is needed
        }

        public IEnumerable<string> Execute()
        {
            var topic = (IHelpful)Subject;
            var sb = GetHelpHeader(topic);

            sb = sb.Concat(topic.RenderHelpBody()).ToList();

            //If it's a command render the syntax help at the bottom
            if (topic.GetType().GetInterfaces().Contains(typeof(ICommand)))
            {
               var subject = (ICommand)topic;
               sb.Add(String.Empty);
               sb = sb.Concat(subject.RenderSyntaxHelp()).ToList();
            }

            return sb;
        }

        public IEnumerable<string> RenderSyntaxHelp()
        {
            var sb = new List<string>();

            sb.Add(String.Format("Valid Syntax: help &lt;topic&gt;"));

            return sb;
        }

        /// <summary>
        /// Renders the help text for the help command itself
        /// </summary>
        /// <returns>string</returns>
        public IEnumerable<string> RenderHelpBody()
        {
            var sb = new List<string>();

            sb.Add(String.Format("Help provides useful information and syntax for the various commands you can use in the world."));

            return sb;
        }

        private IList<string> GetHelpHeader(IHelpful subject)
        {
            var sb = new List<string>();
            var subjectName = subject.GetType().Name;
            var typeName = "Help";

            if(subject.GetType().GetInterfaces().Contains(typeof(IReference)))
            {
                var refSubject = (IReference)subject;

                subjectName = refSubject.Name;
                typeName = "Reference";
            }
            else if(subject.GetType().GetInterfaces().Contains(typeof(ICommand)))
            {
                typeName = "Commands";
            }

            sb.Add(string.Format("{0} - <span style=\"color: orange\">{1}</span>", typeName, subjectName));
            sb.Add(String.Empty.PadLeft(typeName.Length + 3 + subjectName.Length, '-'));

            return sb;
        }
    }
}