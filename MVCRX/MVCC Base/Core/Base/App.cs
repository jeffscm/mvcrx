/*
Jefferson Scomacao
https://www.jscomacao.com

GitHub - Source Code
Project: MVCC 3.0 (MVCRX)

Unity3D MVC Framework for Unity2019 and higher

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
using System.Reflection;
using UnityEngine;

namespace MVCC
{

    public enum NAVANIM { NONE, FADE, MOVELEFT, MOVERIGHT, MOVEBOTTOM, MOVEUP, SCALE, NO_ANIM,
        MOVEXABS,
        MOVEYABS
    }

    public enum NOTIFY_TYPES
    {
        EVENT, SYSTEM, UI, JS, NAV, AR, VR, PROXY
    }

    public class App
    {

        public static System.Action<string> OnNotificationLog;
        public static System.Action<UIViewControllerBase> OnNavLog;
        public static System.Action<UIView, int> OnUIViewSwitchLog;

        public AppModel model;
        public static Obsever<AppMonoController> monoObsever = new Obsever<AppMonoController>();
        public static Obsever<UIView> uiviewObsever = new Obsever<UIView>();
        public static Obsever<UI3DView> ui3dviewObsever = new Obsever<UI3DView>();

        static List<AppController> controller_list = new List<AppController>();
        static List<AppMonoController> monocontroller_list = new List<AppMonoController>();

        static Dictionary<Type, List<object>> imonocontroller_list = new Dictionary<Type, List<object>>();

        static Dictionary<CONTROLLER_TYPE, UIViewControllerBase> _currentNav = new Dictionary<CONTROLLER_TYPE, UIViewControllerBase>();


        public static AppServices AppServices = new AppServices();


        public App()
        {
            model = new AppModel();

            monoObsever.registerCall = (newcontroller) =>
            {
                if (newcontroller.priority == 0)
                {
                    monocontroller_list.Insert(0, newcontroller);                    
                }
                else
                {
                    monocontroller_list.Add(newcontroller);
                }
                AddInterfaces(newcontroller);
            };

            monoObsever.Publish();

            uiviewObsever.registerCall = (uiview) =>
            {
                var t = uiview.GetType();
                if (!uiviewList.ContainsKey(t))
                {
                    uiviewList.Add(t, uiview);
                }
            };
            uiviewObsever.Publish();

            ui3dviewObsever.registerCall = (uiview) =>
            {
                var t = uiview.GetType();
                if (!uiviewList.ContainsKey(t))
                {
                    ui3dviewList.Add(t, uiview);
                }
            };
            ui3dviewObsever.Publish();

            MVCCLog.Log("App Class Started");
        }

        // testing---------
        private void AddInterfaces(object newmono)
        {
            var interfaces = newmono.GetType().GetInterfaces().Where(t => t != null && t.Name != typeof(INotify).Name);
            foreach (var i in interfaces)
            {
                if (!imonocontroller_list.ContainsKey(i))
                {
                    imonocontroller_list.Add(i, new List<object>());
                }
                imonocontroller_list[i].Add(newmono);
            }
        }

        public void NotifyWithParam<T1>(Action<T1> notifyParams) where T1 : INotify
        {
            var test = typeof(T1);
            var list = imonocontroller_list[test];
            foreach (var obj in list)
            {
                if (obj is T1)
                {
                    notifyParams((T1)obj);
                }
            }            
        }
        //---------------------------
        public void NotifyTest<T1>(Action<T1> notifyParams) where T1 : INotify
        {

            var test = typeof(T1);
            var list = imonocontroller_list[test];
            foreach (var obj in list)
            {
                if (obj is T1)
                {
                    Debug.Log($"[Test] {obj}");
                    notifyParams((T1)obj);
                }
            }

        }

        public void RegisterCSController(AppController newAppController)
        {
            controller_list.Add(newAppController); //TODO
        }

        public void RemoveCSController(AppController newAppController)
        {
            //TODO
        }

        public void Notify<T1>(INotifyParam notifyParams)
        {
            var test = typeof(T1);
            var methods = test.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var obj in controller_list)
            {
                if (obj is T1)
                {
                    foreach (var m in methods)
                    {
                        m.Invoke(obj, new object[] { notifyParams });
                    }
                }
            }

            foreach (var obj in monocontroller_list)
            {
                if (obj is T1)
                {
                    foreach (var m in methods)
                    {
                        m.Invoke(obj, new object[] { notifyParams });
                    }
                }
            }
        }

        public void Notify<T1>()
        {
            var test = typeof(T1);
            var methods = test.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var obj in controller_list)
            {
                if (obj is T1)
                {
                    foreach (var m in methods)
                    {
                        m.Invoke(obj, null);
                    }
                }
            }

            foreach (var obj in monocontroller_list)
            {
                if (obj is T1)
                {
                    foreach (var m in methods)
                    {
                        m.Invoke(obj, null);
                    }
                }
            }
        }

        public void Notify(Type type, INotifyParam notifyParams)
        {

            var test = type;

            var methods = test.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var obj in controller_list)
            {
                var ii = obj.GetType().GetInterfaces();

                foreach (var i in ii)
                {
                    if (i == type)
                    {
                        foreach (var m in methods)
                        {
                            m.Invoke(obj, new object[] { notifyParams });
                        }
                    }
                }
            }

            foreach (var obj in monocontroller_list)
            {                
                var ii = obj.GetType().GetInterfaces();

                foreach (var i in ii)
                {
                    if (i == type)
                    {
                        foreach (var m in methods)
                        {
                            m.Invoke(obj, new object[] { notifyParams });
                        }
                    }
                }
            }
        }

        public void Notify(Type type)
        {

            var test = type;

            var methods = test.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var obj in controller_list)
            {
                var ii = obj.GetType().GetInterfaces();

                foreach (var i in ii)
                {
                    if (i == type)
                    {
                        foreach (var m in methods)
                        {
                            m.Invoke(obj, null);
                        }
                    }
                }
            }

            foreach (var obj in monocontroller_list)
            {
                var ii = obj.GetType().GetInterfaces();

                foreach (var i in ii)
                {
                    if (i == type)
                    {
                        foreach (var m in methods)
                        {
                            m.Invoke(obj, null);
                        }
                    }
                }
            }

        }

        public void SwitchNavController<T>(CONTROLLER_TYPE controllerType) where T : UIViewControllerBase
        {
            UIViewControllerBase reportExit = null;
            UIViewControllerBase reportEntry = null;

            if (controllerType == CONTROLLER_TYPE.ALL)
            {
                foreach (var item in _currentNav)
                {
                    if (item.Value != null)
                    {
                        item.Value.OnNavigationExit();
                    }
                }
            }
            else if (_currentNav.ContainsKey(controllerType) && _currentNav[controllerType] != null)
            {
                reportExit = _currentNav[controllerType];
            }

            var temp = GetNav<T>(controllerType);
            if (temp != null)
            {
                OnNavLog?.Invoke(temp);
                reportEntry = temp;
                var doReportEntry = true;
                if (_currentNav.ContainsKey(temp.controllerType) && _currentNav[temp.controllerType] != null)
                {
                    doReportEntry = _currentNav[temp.controllerType].controllerId != reportEntry.controllerId;
                }

                _currentNav[controllerType] = null;
                _currentNav[temp.controllerType] = temp;

                if (doReportEntry)
                {
                    if (reportExit != null) reportExit.OnNavigationExit();
                    if (reportEntry != null) reportEntry.OnNavigationEnter();
                }
                else
                {
                    Debug.Log($"NavController Already Active {temp.name}");
                }
            }
            else
            {
                if (reportExit != null) reportExit.OnNavigationExit();

                _currentNav[controllerType] = null;
            }

        }

        public bool IsCurrentView<T>(long controllerId) where T : UIView
        {
            foreach (var view in uiviewList)
            {
                if (view.Value != null && (view.Value.controllerId == controllerId || controllerId == -1) && view.Value.IsCurrent)
                {
                    return view.Key == typeof(T);
                }
            }
            return false;
        }

        public UIViewControllerBase GetCurrentViewController(CONTROLLER_TYPE controllerType)
        {
            if (!_currentNav.ContainsKey(controllerType)) return null;
            return _currentNav[controllerType];
        }

        public void ActivateNavController<T>() where T : UIViewControllerBase
        {
            var temp = GetNav<T>(CONTROLLER_TYPE.ALL);
            if (temp != null)
            {
                if (!temp.enableSwitch)
                {
                    temp.OnNavigationActivate();
                    MVCCLog.Log($"NavController Activated: {temp.GetType()}");
                }
                OnNavLog?.Invoke(temp);
            }
        }

        public void DeactivateNavController<T>() where T : UIViewControllerBase
        {
            var temp = GetNav<T>(CONTROLLER_TYPE.ALL);
            if (temp != null)
            {
                if (!temp.enableSwitch)
                {
                    temp.OnNavigationDeativate();
                }
                OnNavLog?.Invoke(temp);
            }
        }

        private T GetNav<T>(CONTROLLER_TYPE controllerType) where T : AppMonoController
        {
            var temp2 = monocontroller_list;
            for (int i = 0; i < temp2.Count; i++)
            {
                if (temp2[i] == null) continue;
                if ((controllerType == CONTROLLER_TYPE.ALL || temp2[i].controllerType == controllerType) && temp2[i].GetType() == typeof(T))
                {
                    return temp2[i] as T;
                }
            }
            return default(T);
        }

        private List<AppMonoController> GetNavs(CONTROLLER_TYPE controllerType)
        {
            var result = new List<AppMonoController>();

            var temp2 = monocontroller_list;
            for (int i = 0; i < temp2.Count; i++)
            {
                if (temp2[i] == null) continue;
                if (temp2[i].controllerType == controllerType)
                {
                    result.Add(temp2[i]);
                }
            }
            return result;
        }

        private T GetController<T>(CONTROLLER_TYPE controllerType) where T : AppController
        {
            var temp2 = monocontroller_list;
            for (int i = 0; i < temp2.Count; i++)
            {
                if (temp2[i] == null) continue;
                if ((controllerType == CONTROLLER_TYPE.ALL || temp2[i].controllerType == controllerType) && temp2[i].GetType() == typeof(T))
                {
                    return temp2[i] as T;
                }
            }
            return default(T);
        }

        public static Dictionary<System.Type, UIView> uiviewList = new Dictionary<System.Type, UIView>();
        public static Dictionary<System.Type, UI3DView> ui3dviewList = new Dictionary<System.Type, UI3DView>();

        public void RegiterView<T>(T ui) where T : UIView
        {
            var temp = ui.GetType();
            if (!uiviewList.ContainsKey(temp))
            {
                uiviewList.Add(temp, ui);
            }
        }

        public void RegisterController(AppController newcontroller)
        {
            controller_list.Add(newcontroller);
        }

        public void HideViews<T>(T thisView, long controllerdId) where T : UIView
        {
            foreach (var ui in uiviewList)
            {
                if (ui.Value.GetType() != typeof(T) && ui.Value.controllerId == controllerdId)
                {
                    ui.Value.Dismiss();
                }
            }
        }
    }



    public interface IUIView
    {
        bool IsCurrent { get; set; }
        void Present();
        void Dismiss();
        void Hide();
        void SetModel();
    }


    public class Obsever<T>
    {

        List<T> subscriptions = new List<T>();

        public System.Action<T> registerCall;

        public void Publish()
        {
            foreach (var s in subscriptions)
            {
                registerCall?.Invoke(s);
            }
        }

        public void Subscribe(T newSub)
        {
            subscriptions.Add(newSub);
            registerCall?.Invoke(newSub);
        }
    }
}