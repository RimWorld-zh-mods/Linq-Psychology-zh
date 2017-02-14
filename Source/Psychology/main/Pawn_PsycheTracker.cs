﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using HugsLib.Source.Detour;
using UnityEngine;

namespace Psychology
{
    public class Pawn_PsycheTracker : IExposable
    {
        public Pawn_PsycheTracker(PsychologyPawn pawn)
        {
            this.pawn = pawn;
        }

        public void Initialize()
        {
            Rand.PushSeed();
            Rand.Seed = this.pawn.HashOffset();
            this.upbringing = Rand.RangeInclusive(1, PersonalityCategories);
            Rand.PopSeed();
            this.nodes = new List<PersonalityNode>();
            DefDatabase<PersonalityNodeDef>.AllDefsListForReading.ForEach((PersonalityNodeDef def) => nodes.Add(PersonalityNodeMaker.MakeNode(def, this.pawn)));
        }

        public void ExposeData()
        {
            Scribe_Values.LookValue(ref this.upbringing, "upbringing", 0, false);
            Scribe_Collections.LookList(ref this.nodes, "nodes", LookMode.Deep, new object[] { this.pawn });
        }
        
        public float GetPersonalityRating(PersonalityNodeDef def)
        {
            return nodes.Find((PersonalityNode n) => n.def == def).AdjustedRating;
        }

        public PersonalityNode GetPersonalityNodeOfDef(PersonalityNodeDef def)
        {
            return nodes.Find((PersonalityNode n) => n.def == def);
        }

        public List<PersonalityNode> PersonalityNodes
        {
            get
            {
                return nodes;
            }
            set
            {
                nodes = value;
            }
        }

        public int upbringing;
        private PsychologyPawn pawn;
        private List<PersonalityNode> nodes;
        private const int PersonalityCategories = 16;
    }
}
