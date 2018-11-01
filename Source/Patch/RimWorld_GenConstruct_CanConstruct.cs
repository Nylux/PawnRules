﻿using Harmony;
using PawnRules.Data;
using RimWorld;
using Verse;
using Verse.AI;

namespace PawnRules.Patch
{
    [HarmonyPatch(typeof(GenConstruct), "CanConstruct")]
    internal static class RimWorld_GenConstruct_CanConstruct
    {
        private static void Postfix(ref bool __result, Thing t, Pawn p, bool checkConstructionSkill = true, bool forced = false)
        {
            if (!Registry.IsActive || (__result == false)) { return; }

            var rules = p.GetRules();
            if ((rules == null) || rules.AllowArtisan || !checkConstructionSkill) { return; }

            if (!(t.def.entityDefToBuild is ThingDef thingDef) || !thingDef.HasComp(typeof(CompQuality))) { return; }

            if (forced && !JobFailReason.HaveReason && !rules.AllowArtisan) { JobFailReason.Is(Lang.Get("Rules.NotArtisanReason"), Lang.Get("Rules.NotArtisanJob", t.LabelCap)); }

            __result = false;
        }
    }
}
