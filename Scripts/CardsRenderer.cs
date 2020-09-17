﻿using System.Collections;
using System.Collections.Generic;
using PlayerAndEditorGUI;
using QuizCannersUtilities;
using UnityEngine;
using static QuizCannersUtilities.QcUtils;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace DeckRenderer
{

    [ExecuteAlways]
    public class CardsRenderer : MonoBehaviour, IPEGI
    {
        public static CardsRenderer instance;

        public ScreenShootTaker screenGrabber = new ScreenShootTaker();
        
        public List<CardsRepository> decks = new List<CardsRepository>();

        public Canvas canvas;

        public CardDesignBase cardDesignInstance;
        
        void OnPostRender()
        {
            screenGrabber.OnPostRender();
        }

        void OnEnable()
        {
            instance = this;
        }

        private bool _coroInProgress;

        public bool IsRendering => _coroInProgress;

        public void RenderAllTheCards(CardsRepository deck)
        {
            StartCoroutine(RenderAllTheCardsCoro(deck));
        }

        public void ShowCard(CardsRepository deck, CardPrototypeBase card = null)
        {
            if (cardDesignInstance)
                cardDesignInstance.gameObject.DestroyWhatever();

            if (!deck.cardDesignPrefab)
            {
                Debug.LogError("No design prefab in " + deck.name, deck);
                return;
            }

            cardDesignInstance = Instantiate(deck.cardDesignPrefab, canvas.transform);

            if (card != null)
                cardDesignInstance.ActivePrototype = card;
        }

        private IEnumerator RenderAllTheCardsCoro(CardsRepository deck)
        {
            ShowCard(deck);

            if (!cardDesignInstance)
                yield break;

            _coroInProgress = true;

            screenGrabber.folderName = "Deck Renders/{0}".F(deck.name);

            foreach (var card in deck.cards)
            {
                cardDesignInstance.ActivePrototype = card;
                screenGrabber.screenShotName = card.NameForPEGI;
                screenGrabber.RenderToTextureManually();
                yield return null;
            }

            _coroInProgress = false;

            yield break;
        }

        #region Inspector

        private int _inspectedStuff = -1;

        private int _inspectedDeck = -1;

        public bool Inspect()
        {
            var changed = pegi.toggleDefaultInspector(this);

            pegi.EditorView.Lock_UnlockClick(this); 

            if (!canvas)
                "Canvas".edit(ref canvas);

            if (icon.Refresh.Click())
            {
                _inspectedStuff = -1;
                _inspectedDeck = -1;
            }

            if (_coroInProgress)
                "Rendering...".write();

            pegi.nl();

            if (cardDesignInstance && "Clear prefab".Click().nl())
                cardDesignInstance.gameObject.DestroyWhatever();

            "Screen Grabber".enter_Inspect(screenGrabber, ref _inspectedStuff, 0).nl(ref changed); 

            "Decks".enter_List_UObj(ref decks,ref _inspectedDeck, ref _inspectedStuff, 1).nl(ref changed);

            return changed;
        }
        #endregion
    }


#if UNITY_EDITOR

    [CustomEditor(typeof(CardsRenderer))]
    public class CardsRendererDrawer : PEGI_Inspector_Mono<CardsRenderer> { }

#endif

}