﻿using System;
using PawnRules.Data;
using PawnRules.Patch;
using UnityEngine;
using Verse;

namespace PawnRules.Interface
{
    internal class Dialog_Alert : Window
    {
        public override Vector2 InitialSize { get; }
        private readonly string _message;
        private readonly Buttons _buttons;
        private readonly Action _onAccept;
        private readonly Action _onCancel;
        private bool _isAccepted;

        public Dialog_Alert(string message, Buttons buttons = Buttons.Ok, Action onAccept = null, Action onCancel = null)
        {
            doCloseButton = false;
            closeOnAccept = true;
            closeOnClickedOutside = false;
            absorbInputAroundWindow = true;

            _message = message;
            _buttons = buttons;
            _onAccept = onAccept;
            _onCancel = onCancel;

            var wrap = Text.WordWrap;
            Text.WordWrap = true;
            InitialSize = new Vector2(400f, 72f + Text.CalcHeight(_message, 364f));
            Text.WordWrap = wrap;
        }

        public override void DoWindowContents(Rect rect)
        {
            var listing = new Listing_Standard();
            var vGrid = rect.GetVGrid(4f, 0f, 30f);

            listing.Begin(vGrid[0]);
            listing.Label(_message);
            listing.End();

            var hGrid = vGrid[1].GetHGrid(4f, 100f, 0f);

            listing.Begin(_buttons == Buttons.Ok ? vGrid[1] : hGrid[0]);

            if (listing.ButtonText(_buttons == Buttons.YesNo ? Lang.Get("Button.Yes") : Lang.Get("Button.OK")))
            {
                _isAccepted = true;
                _onAccept?.Invoke();
                Close();
            }

            listing.End();

            if (_buttons == Buttons.Ok) { return; }

            listing.Begin(hGrid[1]);
            if (listing.ButtonText(_buttons == Buttons.YesNo ? Lang.Get("Button.No") : Lang.Get("Button.Cancel"))) { Close(); }
            listing.End();
        }

        public override void Close(bool doCloseSound = true)
        {
            if (!_isAccepted) { _onCancel?.Invoke(); }
            base.Close(doCloseSound);
        }

        public enum Buttons
        {
            Ok,
            OkCancel,
            YesNo
        }
    }
}
