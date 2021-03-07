
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MVCC
{
    public class SafeArea : MonoBehaviour
    {
        public RectTransform SafeAreaRect;

        private Rect lastSafeArea = Rect.zero;

        [SerializeField]
        private Canvas canvas;

        private void Start()
        {
            lastSafeArea = Screen.safeArea;
            ApplySafeArea();
#if !UNITY_EDITOR
            ScreenRotation.OnRotationChange += (newOrientation) =>
            {
                if (lastSafeArea != Screen.safeArea)
                {
                    lastSafeArea = Screen.safeArea;
                    ApplySafeArea();
                }
            };
#endif
        }

        private void ApplySafeArea()
        {
            if (SafeAreaRect == null)
            {
                return;
            }

            Rect safeArea = Screen.safeArea;

            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= canvas.pixelRect.width;
            anchorMin.y /= canvas.pixelRect.height;
            anchorMax.x /= canvas.pixelRect.width;
            anchorMax.y /= canvas.pixelRect.height;

            SafeAreaRect.anchorMin = anchorMin;
            SafeAreaRect.anchorMax = anchorMax;
        }
    }
}