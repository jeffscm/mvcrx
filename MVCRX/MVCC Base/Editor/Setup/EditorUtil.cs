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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Linq;
using Newtonsoft.Json;
using System.Reflection;

namespace MVCC.Editor
{
    public static class EditorUtil
    {
        public static MVCCValues mvccValues;

        public static void ReplaceNameSpace(string pathSource, string pathDest, string newNamespace)
        {
            string animContent = File.ReadAllText(pathSource);
            animContent = animContent.Replace("%NAMESPACE%", newNamespace);
            File.WriteAllText(pathDest, animContent);
        }

        public static bool CreateDir(string path)
        {
            if (!Directory.Exists(path))
            {
				try
				{
					Directory.CreateDirectory(path);
					return true;
				}
                catch
				{
                    MVCCLog.LogError($"[MVCC] Failed to Create folder: {path}");
				}				
            }
			return false;
		}

        public static string GetCurrentMVCC()
        {
            if (mvccValues == null)
            {
				LoadDefaultValues();
				if (mvccValues == null)
                {
					CopyNewResource();
				}
            }
            return mvccValues.data;
        }

        private static void LoadDefaultValues()
		{
			var destPath = Application.dataPath + "/MVCRX/mvc.txt";
			if (File.Exists(destPath))
			{
				var content = File.ReadAllText(destPath);
				mvccValues = JsonConvert.DeserializeObject<MVCCValues>(content);
			}
		}

		private static void CopyNewResource()
        {
            var destPath = Application.dataPath + "/MVCRX";
			CreateDir(destPath);

			destPath = destPath + "/mvc.txt";

            mvccValues = new MVCCValues();

			if (!File.Exists(destPath))
			{
				File.WriteAllText(destPath, JsonConvert.SerializeObject(mvccValues));
			}
			
		}

        public static MVCCValues GetCurrentData()
        {
            if (mvccValues == null)
            {
				LoadDefaultValues();
				if (mvccValues == null)
                {
                    CopyNewResource();
                }
            }
            return mvccValues;
        }

        public static void WriteData(string path, string fileName, string content)
        {
            CreateDir(path);
            File.WriteAllText(path + fileName, content);
        }

        public static List<Type> GetAllEntities()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                 .Where(x => x.GetInterface("INotify") != null && x.IsInterface)
                 .ToList();
        }

        public static List<Type> GetAllParamEntities()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                 .Where(x => x.GetInterface("INotifyParam") != null && !x.IsInterface)
                 .ToList();
        }

        public static string PascalToCamelCase(string value)
		{
			var temp = value;
			var a = value[0].ToString().ToLower();
			temp = a[0] + temp.Substring(1, temp.Length-1);
			return temp;
		}
        public static string CamelToPascalCase(string value)
        {
            var temp = value;
            var a = value[0].ToString().ToUpper();
            temp = a[0] + temp.Substring(1, temp.Length - 1);
            return temp;
        }

        public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }

        public static Assembly GetAssemblyByName(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies().
                   SingleOrDefault(assembly => assembly.GetName().Name == name);
        }

        public static Assembly GetProjectAssembly()
        {
            return GetAssemblyByName("Assembly - CSharp");
        }

    }
}
