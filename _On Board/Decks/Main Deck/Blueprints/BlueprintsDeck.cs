﻿using QuizCannersUtilities;
using PlayerAndEditorGUI;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DeckRenderer.OnBoard
{
    [CreateAssetMenu(fileName = "Blueprints Deck", menuName = "DeckRenderer/OnBoard/Deck/Blueprints")]
    public class BlueprintsDeck : DeckGeneric<BlueprintPrototype> {  }

#if UNITY_EDITOR
    [CustomEditor(typeof(BlueprintsDeck))]
    public class BlueprintsDeckDrawer : PEGI_Inspector_SO<BlueprintsDeck> { }
#endif


    [Serializable]
    public class BlueprintPrototype : CardPrototypeBase
    {
        public int resourcesNeeded = 1;
        public DiceRole diceType;


        #region Decoding
        public override void Decode(string key, CfgData token)
        {
            switch (key)
            {
                case "Operators Count": resourcesNeeded = token.ToInt(); break;
                case "Operated by": diceType = OnBoardUtils.DiceRoleFrom(token.ToString()); break;
                default: base.Decode(key, token); break;
            }
        }

        #endregion
        
        public override bool Inspect()
        {
            var changed = base.Inspect();

            "Description".editBig(ref description);

            return changed;
        }

    }

}