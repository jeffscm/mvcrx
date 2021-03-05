﻿/*
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
using System.IO;
using Newtonsoft.Json;

public class MVCCValues
{
    public string data;
    public string currentKeySetup;
    public List<BuildTargetGroup> supportedPlatforms = new List<BuildTargetGroup>();

    public void SetupNewProject(string newProjectName)
    {
        data = newProjectName;
		WriteValue(this);
	}

    public void CheckProjectActive()
    {
   
    }
    public void SetKey(string key)
	{
		currentKeySetup = key;
		WriteValue(this);
	}

    public string GetKey()
	{
		return currentKeySetup;
	}

	public static void WriteValue(MVCCValues instance)
	{
		var destPath = Application.dataPath + "/MVCRX/mvc.txt";
		File.WriteAllText(destPath, JsonConvert.SerializeObject(instance));
	}
}
