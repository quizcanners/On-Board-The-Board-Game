﻿using System;
using System.Collections;
using System.Collections.Generic;
using PlayerAndEditorGUI;
using QuizCannersUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DeckRenderer
{
    public class CardDesignBase : MonoBehaviour, IPEGI
    {
        public TextMeshProUGUI Type;
        public TextMeshProUGUI Name;
        public TextMeshProUGUI Description;
        public TextMeshProUGUI Lore;

        protected CardsRenderer DeckRederer => CardsRenderer.instance;
        
        [NonSerialized] protected CardPrototypeBase activePrototype;

        public CardPrototypeBase ActivePrototype
        {
            get
            {
                return activePrototype;
            }
            set
            {
                activePrototype = value;
                Fill();
            }
        }

        protected void TrySet(TextMeshProUGUI element, string text)
        {
            if (element)
                element.text = text;
        }

        protected void TrySet(Image element, Sprite sprite)
        {
            if (element)
                element.sprite = sprite;
        }

        protected void TrySetEnable(Component cmp, bool value)
        {
            if (cmp)
                cmp.gameObject.SetActive(value);
        }

        public virtual void Fill()
        {
            if (activePrototype != null)
            {
                TrySet(Name, activePrototype.NameForPEGI);
                TrySet(Description, activePrototype.description);
                TrySet(Lore, activePrototype.lore);
            }
            else
            {
                TrySet(Name, "No Prototype");
                TrySet(Description, "No Prototype");
                TrySet(Lore, "No Prototype");
            }
        }

        #region Inspector

        public bool Inspect()
        {
            var changed = false;

            if (activePrototype != null)
            {
                activePrototype.Nested_Inspect().nl(ref changed);
            }

            if (changed)
                Fill();

            return changed;
        }

        #endregion
    }

    public abstract class CardDesignGeneric<T> : CardDesignBase where T: CardPrototypeBase
    {

        public abstract void Fill(T prot);

        public override void Fill()
        {
            base.Fill();

            var prot = ActivePrototype as T;

            if (prot != null)
                Fill(prot);
            else 
                Debug.LogError("Couldn't convert {0} to {1}".F(prot, typeof(T).ToString()));
        }
    }

}
