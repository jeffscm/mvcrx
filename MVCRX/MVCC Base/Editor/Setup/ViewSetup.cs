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
using System;
using System.Linq;
using System.IO;
using System.Reflection;

namespace MVCC.Editor
{
    public class ViewSetup
    {

        static string currentProject = string.Empty;
        private string _newViewName = string.Empty;
        private string _newViewNavName = string.Empty;
		private string _extraNamespace = string.Empty;
		
		private bool _addViewToObject = true;
        private bool _addControllerToObject = true;

        private bool _doAddViewToObject;
		private bool _addExtraNamespace = false;
		private bool _runSetup = false;


        private string[] _namespacesOptions;
        private int _selectedNamespace;

		static string outputFolder;
        static string pathSource;

        private string _addViewType = string.Empty;

        public void Setup(string newProject) 
        {
			currentProject = EditorUtil.GetCurrentMVCC();
            outputFolder = Application.dataPath + "/" + currentProject + "/Scripts/";
            pathSource = "Packages/com.mvcrx.main/MVCRX/MVCC Base/Editor/Setup/Source/";

            var asm = EditorUtil.GetProjectAssembly();

            _namespacesOptions = asm.GetTypes()
                         .Select(t => t.Namespace).Where(s => s != null && (s.StartsWith(currentProject + ".View", StringComparison.Ordinal) || s.EndsWith(currentProject, StringComparison.Ordinal)))
                         .Distinct().ToArray();

            var temp = PlayerPrefs.GetString("MVCC_ADD_VIEW", "");
            if (temp != "")
            {
                _doAddViewToObject = true;
                _addViewType = temp;
                PlayerPrefs.DeleteKey("MVCC_ADD_VIEW");
            }
        }

        public void Display(EditorWindow context)
        {

			if (EditorApplication.isUpdating || EditorApplication.isCompiling)
            {
                EditorGUILayout.LabelField("Wait for it...");
				_runSetup = true;

				return;
            }

            if (string.IsNullOrEmpty(currentProject))
            {
                EditorGUILayout.LabelField("You dont have a MVCC project setup yet.");
                return;
            }

            if (_runSetup)
			{
				_runSetup = false;
				Setup(currentProject);
			}

            if (_doAddViewToObject)
            {
                _doAddViewToObject = false; 
                AssetDatabase.Refresh();

                var asm = EditorUtil.GetProjectAssembly();
                var newType = asm.GetType(_addViewType);

                var temp = Selection.activeGameObject;

                var hasViewController = temp.GetComponent<UIViewController>();
                if (hasViewController != null)
                {
                    var newChild = new GameObject(newType.Name);
                    newChild.transform.parent = Selection.activeGameObject.transform;
                    newChild.transform.localPosition = Vector3.zero;
                    newChild.transform.localScale = Vector3.one;
                    newChild.layer = Selection.activeGameObject.layer;
                    temp = newChild;
                    temp.AddComponent<RectTransform>();
                }
                
                temp.AddComponent(newType);
                temp.name = newType.Name;
                _newViewName = string.Empty;
                return;
            }
			GUILayout.Space(20);
			
            EditorGUILayout.LabelField("View Options:");
            GUILayout.Space(10);
            _addViewToObject = GUILayout.Toggle(_addViewToObject, "Add to selected gameObject");
            _newViewName = EditorGUILayout.TextField("New View Name", _newViewName);
            _selectedNamespace = EditorGUILayout.Popup("Namespace", _selectedNamespace, _namespacesOptions);
            if (GUILayout.Button("Generate New View Script") && _selectedNamespace != -1)
            {
                string content = File.ReadAllText(pathSource + "View.txt");
                content = content.Replace("%NAMESPACE%", _namespacesOptions[_selectedNamespace]);
                content = content.Replace("%VIEW%", _newViewName);
                EditorUtil.WriteData(outputFolder + "Views/", _newViewName + ".cs", content);
                if (_addViewToObject && Selection.activeGameObject != null)
                {
                    //_doAddViewToObject = true;
                    PlayerPrefs.SetString("MVCC_ADD_VIEW", _namespacesOptions[_selectedNamespace] + "." + _newViewName);
                }
                AssetDatabase.Refresh();
            }
        }
    }
}