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
using UnityEditor;

namespace MVCC.Editor
{
    public class MVCCRootSetup : EditorWindow
    {
        private static string _currentProject = string.Empty;

        private static MVCCSetup _mvccSetup = new MVCCSetup();
        private static StateSetup   _stateSetup = new StateSetup();
        private static ViewSetup    _viewSetup  = new ViewSetup();
        private static ControllerSetup _controllerSetup = new ControllerSetup();

        [MenuItem("Window/MVCC RX")]
        static void Init()
        {
            _currentProject = EditorUtil.GetCurrentMVCC();
            MVCCRootSetup window = (MVCCRootSetup)EditorWindow.GetWindow(typeof(MVCCRootSetup));
            window.titleContent = new GUIContent("MVCC RX");
            window.Show();
        }

        void OnEnable()
        {
            _currentProject = EditorUtil.GetCurrentMVCC();
        }

        private int _tab = 0;
        private int _lastTab = -1;
        private int _counter = 0;
        private int _defaultBlocker = 10;
        private void OnGUI()
        {
            if (EditorApplication.isUpdating || EditorApplication.isCompiling)
            {
                _lastTab = -1;
                EditorGUILayout.LabelField("Wait for it...");
                return;
            }

            _counter++;
            if (_counter > 250)
            {
                var mvccData = EditorUtil.GetCurrentData();
                _currentProject = mvccData.data;
                mvccData.CheckProjectActive();
                _lastTab = -1;
                _counter = 0;
            }

            if (string.IsNullOrEmpty(_currentProject))
            {
                _currentProject = EditorUtil.GetCurrentMVCC();
                
                if (_lastTab != _tab && !string.IsNullOrEmpty(_currentProject)) _mvccSetup.Setup(_currentProject);
                if (_mvccSetup.Display(this))
                {
                    _currentProject = EditorUtil.GetCurrentMVCC();
                }
                _lastTab = _tab;
                return;
            }

            if (_defaultBlocker > 0)
            {
                EditorGUILayout.LabelField("Wait for it...");
                _defaultBlocker--;
                return;
            }

            _tab = GUILayout.Toolbar(_tab, new string[] { "Setup", "Namespaces", "Event", "View", "Controller" });

            switch (_tab)
            {
                case 0:
                    if (_lastTab != _tab) _mvccSetup.Setup(_currentProject);
                    _mvccSetup.Display(this);
                    break;
                case 2:
                    if (_lastTab != _tab) _stateSetup.Setup(_currentProject);
                    _stateSetup.Display(this);
                    break;
                case 3:
                    if (_lastTab != _tab) _viewSetup.Setup(_currentProject);
                    _viewSetup.Display(this);
                    break;
                case 4:
                    if (_lastTab != _tab) _controllerSetup.Setup(_currentProject);
                    _controllerSetup.Display(this);
                    break;
            }

            _lastTab = _tab;
        }
    }
}
