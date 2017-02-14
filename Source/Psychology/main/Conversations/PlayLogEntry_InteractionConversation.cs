﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.Grammar;
using System.Reflection;

namespace Psychology
{
    public class PlayLogEntry_InteractionConversation : PlayLogEntry_Interaction
    {
        public PlayLogEntry_InteractionConversation()
        {
        }
        public PlayLogEntry_InteractionConversation(InteractionDef intDef, Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks) : base(intDef, initiator, recipient, extraSentencePacks)
        {
            FieldInfo RuleStrings = typeof(RulePack).GetField("rulesStrings", BindingFlags.Instance | BindingFlags.NonPublic);
            this.rulesInit = (List<string>)RuleStrings.GetValue(intDef.logRulesInitiator);
            this.rulesRecip = (List<string>)RuleStrings.GetValue(intDef.logRulesRecipient);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            for (int i = 0; i < rulesInit.Capacity; i++)
            {
                if(i+1 > this.rulesInit.Count)
                {
                    this.rulesInit.Add("logentry->Ended a conversation with [other_nameShortIndef].");
                }
                string ruleText = this.rulesInit[i];
                Scribe_Values.LookValue(ref ruleText, "rulesInit" + i, "logentry->Ended a conversation with [other_nameShortIndef].");
                this.rulesInit[i] = ruleText;
            }
            for (int i = 0; i < rulesRecip.Capacity; i++)
            {
                if (i+1 > this.rulesRecip.Count)
                {
                    this.rulesRecip.Add("logentry->Ended a conversation with [other_nameShortIndef].");
                }
                string ruleText = this.rulesRecip[i];
                Scribe_Values.LookValue(ref ruleText, "rulesRecip" + i, "logentry->Ended a conversation with [other_nameShortIndef].");
                this.rulesRecip[i] = ruleText;
            }
            FieldInfo IntDef = typeof(PlayLogEntry_Interaction).GetField("intDef", BindingFlags.Instance | BindingFlags.NonPublic);
            InteractionDef newIntDef = new InteractionDef();
            newIntDef.defName = "EndConversation";
            FieldInfo Symbol = typeof(InteractionDef).GetField("symbol", BindingFlags.Instance | BindingFlags.NonPublic);
            Symbol.SetValue(newIntDef, Symbol.GetValue(InteractionDefOf.DeepTalk));
            newIntDef.label = "ended conversation";
            FieldInfo RuleStrings = typeof(RulePack).GetField("rulesStrings", BindingFlags.Instance | BindingFlags.NonPublic);
            RulePack initPack = new RulePack();
            RuleStrings.SetValue(initPack, this.rulesInit);
            newIntDef.logRulesInitiator = initPack;
            RulePack recipPack = new RulePack();
            RuleStrings.SetValue(recipPack, this.rulesRecip);
            newIntDef.logRulesRecipient = recipPack;
            IntDef.SetValue(this, newIntDef);
        }

        protected List<string> rulesInit = new List<string>(1);
        protected List<string> rulesRecip = new List<string>(1);
    }
}
