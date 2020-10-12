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

using UnityEngine;
using System;
using System.Collections.Generic;
using MVCC;

public class HttpHelper : IHttpConnector
{

    public IRequest Request(string url, MVCC.HTTPMethods method, Dictionary<string, string> headers = null, Action<string, int> onFinish = null)
    {

        //var request = new HTTPRequest(new Uri(url), BestHTTP.HTTPMethods.Get, (req, resp) =>
        //    {
        //        if (req.State != HTTPRequestStates.Aborted)
        //        {
        //            if (resp != null && resp.IsSuccess)
        //            {
        //                onFinish?.Invoke(resp.DataAsText, resp.StatusCode);
        //            }
        //            else if (resp != null)
        //            {
        //                onFinish?.Invoke(null, resp.StatusCode);
        //            }
        //            else
        //            {
        //                onFinish?.Invoke(null, 0);
        //            }
        //        }
        //    }
        //);

        //if (headers != null)
        //{
        //    foreach (var d in headers)
        //    {
        //        request.AddField(d.Key, d.Value);
        //    }
        //}
        //request.Send();

        return null;
    }

    public IRequest Request(string url, MVCC.HTTPMethods method, Dictionary<string, string> headers = null, Action<Texture2D, int> onFinish = null)
    {

        //var request = new HTTPRequest(new Uri(url), BestHTTP.HTTPMethods.Get, (req, resp) =>
        //{
        //    if (req.State != HTTPRequestStates.Aborted)
        //    {
        //        if (resp != null && resp.IsSuccess)
        //        {
        //            onFinish?.Invoke(resp.DataAsTexture2D, resp.StatusCode);
        //        }
        //        else if (resp != null)
        //        {
        //            onFinish?.Invoke(null, resp.StatusCode);
        //        }
        //        else
        //        {
        //            onFinish?.Invoke(null, 0);
        //        }
        //    }
        //}
        //);

        //if (headers != null)
        //{
        //    foreach (var d in headers)
        //    {
        //        request.AddField(d.Key, d.Value);
        //    }
        //}
        //request.Send();

        return null;
    }

    public void CancelDownload(IRequest request)
    {
        //if (request != null)
        //{
        //    request.Abort();
        //}
    }

}

