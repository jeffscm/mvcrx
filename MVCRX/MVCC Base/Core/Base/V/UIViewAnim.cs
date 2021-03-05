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
using System;
namespace MVCC
{

    public class UIViewAnim : AppElement
    {

        public UIView uiView;

        public AnimateSettings animateIn;
        public AnimateSettings animateOut;
        public CanvasGroup cg;
        public bool deactivateOnOut = false;

        public Action onAnimateIn, onAnimateOut;

        private void Awake()
        {
            if (uiView != null)
            {
                uiView.OnDismiss += () =>
                {
                    DoAnimateOut();
                };

                uiView.OnPresent += () =>
                {
                    DoAnimateIn();
                };
            }
        }

        public void DoAnimateIn()
        {
            onAnimateIn?.Invoke();
            switch (animateIn.animateType)
            {
                case NAVANIM.FADE:
                    MVCCStart.animate.FadeIn(cg, animateIn, null);
                    break;
                case NAVANIM.MOVEYABS:
                case NAVANIM.MOVEBOTTOM:
                case NAVANIM.MOVEUP:
                    MVCCStart.animate.MoveYIn(cg, animateIn, null);
                    break;
                case NAVANIM.MOVEXABS:
                case NAVANIM.MOVELEFT:
                case NAVANIM.MOVERIGHT:
                    MVCCStart.animate.MoveXIn(cg, animateIn, null);
                    break;
                case NAVANIM.SCALE:
                    MVCCStart.animate.ScaleIn(cg, animateIn, null);
                    break;
            }
        }

        public void DoAnimateOut()
        {
            onAnimateOut?.Invoke();
            switch (animateOut.animateType)
            {
                case NAVANIM.FADE:
                    MVCCStart.animate.FadeOut(cg, deactivateOnOut, animateOut, null);
                    break;
                case NAVANIM.MOVEXABS:
                case NAVANIM.MOVELEFT:
                case NAVANIM.MOVERIGHT:
                    MVCCStart.animate.MoveXOut(cg, deactivateOnOut, animateOut, null);
                    break;
                case NAVANIM.MOVEYABS:
                case NAVANIM.MOVEBOTTOM:
                    MVCCStart.animate.MoveYOut(cg, deactivateOnOut, animateOut, true, null);
                    break;
                case NAVANIM.MOVEUP:
                    MVCCStart.animate.MoveYOut(cg, deactivateOnOut, animateOut, false, null);
                    break;
                case NAVANIM.SCALE:
                    MVCCStart.animate.ScaleOut(cg, deactivateOnOut, animateOut, null);
                    break;
            }
        }

#if UNITY_EDITOR
        public void SetDefaultOut()
        {

            var rectTrans = this.transform as RectTransform;
            var rectSize = 0f;
            var f = 0f;
            var v = rectTrans.anchoredPosition;

            switch (animateOut.animateType)
            {
                case NAVANIM.FADE:
                    cg.alpha = animateOut.fade;
                    break;
                case NAVANIM.MOVEXABS:
                    v.x = animateOut.animateDistance;
                    break;
                case NAVANIM.MOVEYABS:
                    v.y = animateOut.animateDistance;
                    break;
                case NAVANIM.MOVELEFT:
                    rectSize = -rectTrans.rect.width;
                    f = (animateOut.animateDistance == 0f) ? rectSize : animateOut.animateDistance;
                    v.x = f;
                    break;
                case NAVANIM.MOVERIGHT:
                    rectSize = rectTrans.rect.width;
                    f = (animateOut.animateDistance == 0f) ? rectSize : animateOut.animateDistance;
                    v.x = f;
                    break;
                case NAVANIM.MOVEBOTTOM:
                    rectSize = -rectTrans.rect.height;
                    f = (animateOut.animateDistance == 0f) ? rectSize : animateOut.animateDistance;
                    v.y = f;
                    break;
                case NAVANIM.MOVEUP:
                    rectSize = rectTrans.rect.height;
                    f = (animateOut.animateDistance == 0f) ? rectSize : animateOut.animateDistance;
                    v.y = f;

                    break;
                case NAVANIM.SCALE:
                    this.transform.localScale = animateOut.scale * Vector3.one;
                    if (animateOut.useFade) cg.alpha = animateOut.fade;
                    break;
            }
            rectTrans.anchoredPosition = v;
            cg.blocksRaycasts = animateOut.cgActive;
        }
#endif
    }
}