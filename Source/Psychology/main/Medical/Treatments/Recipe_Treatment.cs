﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using System.Diagnostics;
using UnityEngine;

namespace Psychology
{
    public class Recipe_Treatment : Recipe_Surgery
    {
        public Recipe_Treatment(TraitDef treatedTrait, HediffDef treatedHediff, string treatedTraitName, float treatedDifficulty, TaleDef treatedTale, int treatedDegree = 0) : base()
        {
            traitDef = treatedTrait;
            traitDegree = treatedDegree;
            hediffDef = treatedHediff;
            traitName = treatedTraitName;
            difficultyFactor = treatedDifficulty;
            taleDef = treatedTale;
        }

        private bool CheckTreatmentFail(Pawn surgeon, Pawn patient)
        {
            float num = 1f;
            float num2 = surgeon.GetStatValue(StatDefOf.SurgerySuccessChance, true);
            num *= Mathf.Min(num2*2,1f);
            float num3 = surgeon.GetStatValue(StatDefOf.SocialImpact, true);
            num *= num3;
            float num4 = patient.needs.comfort.CurLevel;
            num *= num4;
            num *= difficultyFactor;
            if(Rand.Value > num)
            {
                return true;
            }
            return false;
        }

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients)
        {
            if(!CheckTreatmentFail(billDoer, pawn))
            {
                TaleRecorder.RecordTale(taleDef, new object[]
                {
                    billDoer,
                    pawn
                });
                if (PawnUtility.ShouldSendNotificationAbout(pawn) || PawnUtility.ShouldSendNotificationAbout(billDoer))
                {
                    Messages.Message("TreatedTrait".Translate(new object[] { pawn.LabelShort, traitName }), pawn, MessageSound.Benefit);
                }
                Hediff recover = HediffMaker.MakeHediff(hediffDef, pawn, pawn.health.hediffSet.GetBrain());
                recover.Tended(1f);
                pawn.health.AddHediff(recover);
                return;
            }
            ThoughtDef failure = ThoughtDefOfPsychology.TreatmentFailed;
            failure.description = failure.description.Translate(new string[] { traitName });
            pawn.needs.mood.thoughts.memories.TryGainMemoryThought(failure);
        }

        [DebuggerHidden]
        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {
            if(pawn.RaceProps.Humanlike && pawn.story.traits.HasTrait(traitDef) && pawn.story.traits.GetTrait(traitDef).Degree == traitDegree && !pawn.health.hediffSet.HasHediff(hediffDef))
            {
                List<BodyPartRecord> brain = new List<BodyPartRecord>();
                brain.Add(pawn.health.hediffSet.GetBrain());
                return brain;
            }
            return new List<BodyPartRecord>();
        }
        
        protected string traitName;
        protected int traitDegree;
        protected float difficultyFactor;
        protected TaleDef taleDef;
        protected HediffDef hediffDef;
        protected TraitDef traitDef;
    }
}
