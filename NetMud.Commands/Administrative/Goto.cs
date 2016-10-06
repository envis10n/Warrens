﻿using NetMud.DataStructure.Behaviors.Rendering;
using NutMud.Commands.Attributes;
using System.Collections.Generic;

using NetMud.Utility;
using NetMud.Data.Game;
using NetMud.Commands.Attributes;
using NetMud.Communication.Messaging;
using System;
using NetMud.DataStructure.SupportingClasses;

namespace NutMud.Commands.Administrative
{
    /// <summary>
    /// Invokes the current container's RenderToLook
    /// </summary>
    [CommandKeyword("goto", false)]
    [CommandPermission(StaffRank.Guest)]
    [CommandParameter(CommandUsage.Subject, typeof(Room), new CacheReferenceType[] { CacheReferenceType.Entity }, true)]
    [CommandRange(CommandRangeType.Global, 0)]
    public class Goto : CommandPartial
    {
        /// <summary>
        /// All Commands require a generic constructor
        /// </summary>
        public Goto()
        {
            //Generic constructor for all IHelpfuls is needed
        }

        /// <summary>
        /// Executes this command
        /// </summary>
        public override void Execute()
        {
            var moveTo = (ILocation)Subject;
            var sb = new List<string>();

            sb.Add("You teleport.");

            var messagingObject = new MessageCluster(sb, new string[] { }, new string[] { }, new string[] { "$A$ disappears in a puff of smoke." }, new string[] { "$A$ appears out of nowhere." });

            messagingObject.ExecuteMessaging(Actor, null, null, OriginLocation, null);

            moveTo.MoveInto<Player>((Player)Actor);
        }

        /// <summary>
        /// Renders syntactical help for the command, invokes automatically when syntax is bungled
        /// </summary>
        /// <returns>string</returns>
        public override IEnumerable<string> RenderSyntaxHelp()
        {
            var sb = new List<string>();

            sb.Add("Valid Syntax: goto &lt;room name&gt;");

            return sb;
        }

        /// <summary>
        /// The custom body of help text
        /// </summary>
        public override string HelpText
        {
            get
            {
                return string.Format("Goto allows staff members to directly teleport to a room irrespective of its capacity limitations.");
            }
            set { throw new NotImplementedException(); }
        }
    }
}
