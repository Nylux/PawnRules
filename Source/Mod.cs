﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Harmony;
using PawnRules.Data;
using PawnRules.Interface;
using PawnRules.Patch;
using RimWorld;
using UnityEngine;
using Verse;

namespace PawnRules
{
    internal class Mod : Verse.Mod
    {
        public const string Id = "PawnRules";
        public const string Name = "Pawn Rules";
        public const string Version = "1.1.3";

        public static readonly DirectoryInfo ConfigDirectory = new DirectoryInfo(Path.Combine(GenFilePaths.ConfigFolderPath, Id));

        public static Mod Instance { get; private set; }
        public static bool FirstTimeUser { get; private set; }

        public Mod(ModContentPack contentPack) : base(contentPack)
        {
            Instance = this;
            Log("Loaded");

            FirstTimeUser = !ConfigDirectory.Exists;
            ConfigDirectory.Create();

            if (!FirstTimeUser) { TryRegisterHugsLibUpdateFeature(); }
        }

        private static void TryRegisterHugsLibUpdateFeature()
        {
            var hugsLib = (from assembly in AppDomain.CurrentDomain.GetAssemblies() from type in assembly.GetTypes() where type.Name == "HugsLibController" select type).FirstOrDefault();
            if (hugsLib == null) { return; }

            var updateFeatures = Traverse.Create(hugsLib)?.Field("instance")?.Property("UpdateFeatures")?.GetValue();
            if (updateFeatures == null) { return; }

            AccessTools.Method(updateFeatures.GetType(), "InspectActiveMod")?.Invoke(updateFeatures, new object[] { Id, Assembly.GetExecutingAssembly().GetName().Version });
        }

        public static void Log(string message) => Verse.Log.Message(PrefixMessage(message));
        public static void Warning(string message) => Verse.Log.Warning(PrefixMessage(message));
        public static void Error(string message) => Verse.Log.Error(PrefixMessage(message));
        public static void Message(string message) => Messages.Message(message, MessageTypeDefOf.TaskCompletion, false);

        public static string PrefixMessage(string message) => $"[{Name} v{Version}] {message}";
        public override string SettingsCategory() => Name;
        public override void DoSettingsWindowContents(Rect inRect)
        {
            var rect = inRect.GetHGrid(1f, -1f, 400f, -1f)[1];

            var listing = new Listing_Standard();
            listing.Begin(rect);

            if (Registry.IsActive)
            {
                if (listing.ButtonText(Lang.Get("Button.RemoveMod"), Lang.Get("Button.RemoveModDesc"))) { Dialog_Alert.Open(Lang.Get("Button.RemoveModConfirm"), Dialog_Alert.Buttons.YesNo, Registry.DeactivateMod); }
            }
            else { listing.Label(Lang.Get("Settings.NoGame")); }

            listing.End();
        }

        internal class Exception : System.Exception
        {
            public Exception(string message) : base($"[{Name} v{Version} : EXCEPTION] {message}")
            { }
        }
    }
}
