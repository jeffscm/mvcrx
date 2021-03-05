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
using System.Linq;
using UnityEngine;

namespace MVCC
{

    public class UIViewControllerBase : AppMonoController
    {
        public bool enableSwitch = true;

        public sealed override bool OnNavigationSwitch(int controllerId)
        {
            if (enableSwitch)
            {
                if (this.controllerId == controllerId)
                {
                    OnNavigationEnter();
                }
                else
                {
                    OnNavigationExit();
                }
            }

            return true;
        }

        public virtual void OnNavigationEnter()
        {

        }

        public virtual void OnNavigationExit()
        {

        }

        public virtual void OnNavigationActivate()
        {

        }

        public virtual void OnNavigationDeativate()
        {


        }

        public T GetView<T>() where T : UIView
        {
            var temp = typeof(T);

            if (App.uiviewList.ContainsKey(temp))
            {
                if (App.uiviewList[temp].controllerId == controllerId)
                {
                    return App.uiviewList[temp] as T;
                }
            }
            MVCCLog.LogError($"Invalid View {typeof(T)}");
            return default;
        }

        public UIView GetView(string viewName)
        {
            foreach (var view in App.uiviewList)
            {
                if (view.Value.controllerId == controllerId)
                {
                    if (view.Value.name == viewName)
                    {
                        return view.Value;
                    }
                }
            }
            return default;
        }

        public Type GetCurrentView()
        {
            foreach(var view in App.uiviewList)
            {
                if (view.Value.controllerId == controllerId)
                {
                    if (view.Value.IsCurrent)
                    {
                        return view.Value.GetType();
                    }
                }
            }

            return null;
        }

        public void GetView<T>(Action<T> onSuccess, Action onFailed) where T : UIView
        {
            var temp = typeof(T);

            if (App.uiviewList.ContainsKey(temp))
            {
                if (App.uiviewList[temp].controllerId == controllerId)
                {
                    onSuccess?.Invoke(App.uiviewList[temp] as T);
                    return;
                }
            }
            onFailed?.Invoke();
            MVCCLog.LogError($"Invalid View {typeof(T)}");
        }

        public bool IsCurrentView<T>() where T : UIView
        {
            foreach (var view in App.uiviewList)
            {
                if (view.Value != null && (view.Value.controllerId == controllerId || controllerId == -1) && view.Value.IsCurrent)
                {
                    return view.Key == typeof(T);
                }
            }
            return false;
        }

        public List<UIView> GetViews()
        {
            var response = new List<UIView>();
            foreach (var view in App.uiviewList)
            {
                if (view.Value != null && view.Value.controllerId == controllerId)
                {
                    response.Add(view.Value);
                }
            }
            return response;
        }

        public void HideViews<T>(T thisView) where T : UIView
        {
            app.HideViews(thisView, controllerId);
        }

        public void HideViews()
        {
            foreach (var ui in App.uiviewList)
            {
                if (ui.Value.controllerId == controllerId)
                {
                    ui.Value.Dismiss();
                }
            }
        }

    }

}