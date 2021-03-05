/*
Jefferson Scomacao
https://www.jscomacao.com

GitHub - Source Code
Project: MVCC 3.0 (MVCRX)

Unity3D MVC Framework for Unity2019 and higher

Based on Previous work available here:
- https://github.com/jeffscm/mvcrx
- https://github.com/jeffscm/mvcc2
- https://github.com/jeffscm/mvccunity



Copyright (c) 2015 Jefferson Raulino Scomação

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

namespace MVCC
{
    public class PressHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public bool useFade = false;

        public bool Instant = false;

        public SOUNDTYPE playSound = SOUNDTYPE.NONE;

        public AppScriptableElement notify;

        public AppScriptableElement notifyOnUp;

        public CanvasGroup cg;

        public MonoBehaviour data;

        public ScriptableObject extraParams;

        public bool reportOnUp = false;

        public bool detectDrag = false;

        Vector2 dragPos = Vector2.zero;

        public bool pushToTalk = false;

        public bool userFirstResponder = false;

        bool hasClickActive = false;

        UIResponder _responder;

        [Space(10)]
        [Header("Is this a selection button?")]
        public bool useColor;
        public Color defaultColor;
        public Color selectedColor;
        public Image imageSelection;

        void OnEnable()
        {
            _responder = GetComponentInParent<UIResponder>();
            if (_responder == null)
            {
                _responder = GetComponentInParent<UIResponder>();
            }
        }

        void Execute(AppScriptableElement eventNotify)
        {

            if (PressManager.instance.CanClick)
            {
                if (useColor)
                {
                    imageSelection.color = selectedColor;
                }

                if (_responder != null && userFirstResponder)
                {
                    if (_responder.reponderNotify != null)
                    {
                        _responder.reponderNotify.Execute(data ?? this);
                    }
                }
                else
                {
                    notify.Execute(data ?? this);
                }
            }
            PressManager.instance.PointerExit();
        }

        public void OnPointerDown(PointerEventData eventData)
        {

            if (useFade) MVCCStart.animate.FadeOut(cg, false, new AnimateSettings()
            {
                time = 0.1f,
                fade = 0.5f
            });

            if (pushToTalk)
            {
                Execute(notify);
                return;
            }

            dragPos = eventData.position;
            if (reportOnUp)
            {
                return;
            }

            if (Instant)
            {
                if (playSound != SOUNDTYPE.NONE)
                {
                    MVCCStart.soundComponent.Play((int)playSound);
                }
                Execute(notify);
            }
            else
            {
                hasClickActive = true;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (useFade) MVCCStart.animate.FadeIn(cg, new AnimateSettings()
            {
                time = 0.1f,
                fade = 1f
            });


            if (pushToTalk)
            {
                Execute(notifyOnUp);
                return;
            }

            if (hasClickActive || reportOnUp)
            {

                if (detectDrag)
                {

                    float f = Vector2.Distance(eventData.position, dragPos);
                    //Debug.Log(f);
                    if (f > 10)
                    {
                        return;
                    }

                }
                Execute(notify);
                if (playSound != SOUNDTYPE.NONE)
                {
                    MVCCStart.soundComponent.Play((int)playSound);
                }
            }
        }

        public void ResetDefaultColor()
        {
            if (useColor)
            {
                imageSelection.color = defaultColor;
            }
        }
    }
}