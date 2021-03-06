﻿using NetMud.Commands.Attributes;
using NetMud.Communication.Messaging;
using NetMud.DataStructure.Administrative;
using NetMud.DataStructure.Architectural;
using System.Collections.Generic;

namespace NetMud.Commands.Social
{
    [CommandKeyword("laid", false, new string[] { "fucked", "fapfapfap" }, false, true)]
    [CommandPermission(StaffRank.Player)]
    [CommandRange(CommandRangeType.Touch, 0)]
    public class Laid : CommandPartial
    {
        /// <summary>
        /// All Commands require a generic constructor
        /// </summary>
        public Laid()
        {
            //Generic constructor for all IHelpfuls is needed
        }

        public override void Execute()
        {
            Message messagingObject = new Message(new LexicalParagraph("You get laid, fucked, fapfapfap."));

            messagingObject.ExecuteMessaging(Actor, null, null, null, null);
        }

        public override IEnumerable<string> RenderSyntaxHelp()
        {
            List<string> sb = new List<string>
            {
                "Valid Syntax: laid"
            };

            return sb;
        }

        /// <summary>
        /// The custom body of help text
        /// </summary>
        public override MarkdownString HelpText
        {
            get
            {
                return string.Format("Get laid, yo.");
            }
            set { }
        }
    }
}
