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
#if UNITY_IOS
using UnityEngine.iOS;
#endif
namespace MVCC
{

    public class NavAnimate : MonoBehaviour, INavAnimate
    {

        public AnimateSettings animateIn;
        public AnimateSettings animateOut;
        public List<MediaQueryItem> mediaQuery = new List<MediaQueryItem>();

        public CanvasGroup cg;

        public bool deactivateOnOut = false;
        public bool startOffScreen = false;

        Vector2 min, max;
        bool _hasMinMax = false;
        void Awake()
        {
            SetInitialMinMax();
            ScreenRotation.OnRotationChange += ProcessMediaQuery;
        }

        public virtual void AnimateIn(Action onComplete = null)
        {

            switch (animateIn.animateType)
            {
                case NAVANIM.FADE:
                    MVCCStart.animate.FadeIn(cg, animateIn, onComplete);
                    break;
                case NAVANIM.MOVEYABS:
                case NAVANIM.MOVEBOTTOM:
                case NAVANIM.MOVEUP:
                    MVCCStart.animate.MoveYIn(cg, animateIn, onComplete);
                    break;
                case NAVANIM.MOVELEFT:
                case NAVANIM.MOVEXABS:
                case NAVANIM.MOVERIGHT:
                    MVCCStart.animate.MoveXIn(cg, animateIn, onComplete);
                    break;
                case NAVANIM.NO_ANIM:
                    this.gameObject.SetActive(true);
                    LeanTween.cancel(this.gameObject);
                    if (animateIn.delay > 0f)
                    {
                        LeanTween.value(0f, 1f, animateIn.delay).setOnComplete(() =>
                        {
                            onComplete?.Invoke();
                        });
                    }
                    else
                    {
                        onComplete?.Invoke();
                    }

                    break;
            }
        }

        public virtual void AnimateOut(Action onComplete = null)
        {
            switch (animateOut.animateType)
            {
                case NAVANIM.FADE:
                    MVCCStart.animate.FadeOut(cg, deactivateOnOut, animateOut, onComplete);
                    break;
                case NAVANIM.MOVEXABS:
                case NAVANIM.MOVELEFT:
                case NAVANIM.MOVERIGHT:
                    MVCCStart.animate.MoveXOut(cg, deactivateOnOut, animateOut, onComplete);
                    break;
                case NAVANIM.MOVEYABS:
                case NAVANIM.MOVEBOTTOM:
                    MVCCStart.animate.MoveYOut(cg, deactivateOnOut, animateOut, true, onComplete);
                    break;
                case NAVANIM.MOVEUP:
                    MVCCStart.animate.MoveYOut(cg, deactivateOnOut, animateOut, false, onComplete);
                    break;
                case NAVANIM.NO_ANIM:

                    LeanTween.cancel(this.gameObject);
                    if (animateOut.delay > 0f)
                    {
                        LeanTween.value(0f, 1f, animateOut.delay).setOnComplete(() =>
                        {
                            onComplete?.Invoke();
                            if (deactivateOnOut)
                            {
                                this.gameObject.SetActive(false);
                            }
                        });
                    }
                    else
                    {
                        onComplete?.Invoke();
                        if (deactivateOnOut)
                        {
                            this.gameObject.SetActive(false);
                        }
                    }
                    break;
            }
        }

        public virtual void AnimateOutInstant()
        {
            if (!_hasMinMax) return;

            MVCCStart.animate.MoveXOutInstant(cg, animateIn.animateType == NAVANIM.MOVERIGHT);
        }

        public virtual void Setup(Action onComplete = null)
        {
            SetInitialMinMax();
            ProcessMediaQuery(ScreenRotation.CurrentRotation);

            if (startOffScreen)
            {
                var rectTrans = transform as RectTransform;
                var v = rectTrans.anchoredPosition;
                float rectSize = 0;
                float f = 0;

                switch (animateIn.animateType)
                {
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
                    case NAVANIM.FADE:

                        break;
                    case NAVANIM.SCALE:

                        break;
                    case NAVANIM.NO_ANIM:

                        break;
                }

                rectTrans.anchoredPosition = v;
                if (deactivateOnOut)
                {
                    this.gameObject.SetActive(false);
                }
            }

            onComplete?.Invoke();
        }

        void ProcessMediaQuery(DeviceOrientation newOrientation)
        {
            var rectTrans = transform as RectTransform;

            foreach (var mq in mediaQuery)
            {
                if (mq.orientation == DeviceOrientation.Unknown || newOrientation == mq.orientation)
                {
                    if (mq.useOffset && (string.IsNullOrEmpty(mq.searchGeneration)
                        #if UNITY_IOS
                        || Device.generation.ToString().ToLower().IndexOf(mq.searchGeneration.ToLower()) > -1
                        #endif
                        ))
                    {
                        rectTrans.offsetMin = new Vector2(min.x + mq.offset.x, min.y + mq.offset.y);
                        rectTrans.offsetMax = new Vector2(max.x - mq.offset.x, max.y - mq.offset.y);
                    }
                }
            }
        }

        void SetInitialMinMax()
        {
            if (_hasMinMax) return;

            _hasMinMax = true;
            var rectTrans = transform as RectTransform;
            min = rectTrans.offsetMin;
            max = rectTrans.offsetMax;

        }
    }


    [Serializable]
    public class AnimateSettings
    {
        public NAVANIM animateType;
        public float animateDistance;
        public LeanTweenType tweenType = LeanTweenType.easeInOutQuad;
        public float time = 0.35f;
        public float delay = 0f;

        public bool useFade = false;
        public float fade = 0f;

        //public bool useScale = false;
        public float scale = 0f;
        public bool cgActive = true;

        //todo
        //public bool useScreenSize = false;

    }

    [Serializable]
    public class MediaQueryItem
    {
        public DeviceOrientation orientation = DeviceOrientation.Unknown;
        public float screenRatio = -1;
        public string searchGeneration = string.Empty;
        public Vector2 offset;
        public bool useOffset = false;
    }
}