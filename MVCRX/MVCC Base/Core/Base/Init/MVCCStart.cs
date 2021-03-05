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
using MVCC.Component;

namespace MVCC
{
    public class MVCCStart
    {
        public static Action OnStart;

        static App _app;
        public static App app { get { return _app; } }
        static bool _isReady = false;
        public static bool IsReady { get { return _isReady; } }
        static IAnimate _anim;
        public static IAnimate animate { get { return _anim; } }

        static IAnimate _anim3d;
        public static IAnimate animate3d { get { return _anim3d; } }

        static ISoundComponent _soundComponent;
        public static ISoundComponent soundComponent { get { return _soundComponent; } }

        static IHttpConnector _httpHelper;
        public static IHttpConnector httpHelper { get { return _httpHelper; } }

        public static void RegisterApp(App newApp)
        {
            _app = newApp;
            _isReady = true;
        }

        public static void RegisterAnim(IAnimate anim)
        {
            _anim = anim;
        }

        public static void RegisterAnim3d(IAnimate anim)
        {
            _anim3d = anim;
        }

        public static void RegisterSoundComponent(ISoundComponent sound)
        {
            _soundComponent = sound;
        }

        public static void RegisterHttpHelper(IHttpConnector helper)
        {
            _httpHelper = helper;
        }
    }
}