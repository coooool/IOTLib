using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMOD;
using System.Text;
using System.Text.RegularExpressions;
using IOTLib.Configure.ValueFactory;
using static UnityEngine.Networking.UnityWebRequest;
using System.Xml.Linq;
using UnityEngine.Assertions;
using System.Data.Common;
using UnityEngine.Networking;

namespace IOTLib.Configure
{
    public class ApiMap
    {
        private static Stack<ApiRequestData> urlPathStack;
        private static StringBuilder urlPathBuild;
        private static List<ApiRequestData> requestGroups;

        internal static void InitData()
        {
            var groups = ConfigureSystem.GetObject<ApiRequestData[]>("$.apimap");

            requestGroups = new List<ApiRequestData>(5);
            urlPathStack = new Stack<ApiRequestData>(4);
            urlPathBuild = new StringBuilder(256);

            requestGroups.AddRange(groups);
        }

        /// <summary>
        /// ����һ�������еı���
        /// </summary>
        /// <param name="url"></param>
        /// <param name="result">�����Ľ����ÿ����������һ��</param>
        public static void ParseUrlParameter(ref string url, 
            KeyValuePairs? parameter, 
            System.Action<string, string>? result)
        {
            Assert.IsFalse(string.IsNullOrEmpty(url), "���������Ӳ���Ϊ��");

            var strReplace = Regex.Matches(url, "\\{[^{}]+\\}");
            if (strReplace.Count > 0)
            {
                foreach (Match mat in strReplace)
                {
                    var text = mat.Groups[0].Value;

                    if (ValueProviderFactory.GetValueFromProvider(text, parameter, out var data))
                    {
                        result?.Invoke(text, data);

                        if (mat.Groups[0].Index > 0)
                        {
                            url = url.Replace(text, UnityWebRequest.EscapeURL(data));
                        }
                        else
                        {
                            url = url.Replace(text, data);
                        }
                    }
                    else
                    {
                        result?.Invoke(text, string.Empty);

                        // ֱ���滻����{}����Ϊ��
                        url = url.Replace(text, "");

                        if (result == null)
                            Debug.LogError($"����ƥ��ֵʧ��:{text}");
                    }
                }
            }
        }

        /// <summary>
        /// ���ݽӿ����Ʒ�������������
        /// </summary>
        /// <returns></returns>
        public static ApiRequestData GetApiData(string name)
        {
            return GetApiData(name, null);
        }

        public static ApiRequestData GetApiData(string name, KeyValuePairs? parameter)
        {
            bool result = false;

            if (IteraGetUrl(name, requestGroups, ref result))
            {
                ApiRequestData apitmp = ApiRequestData.CreateDefault(name);

                while (urlPathStack.Count > 0)
                {
                    var node = urlPathStack.Pop();
                    apitmp.InheritAttribute(node);

                    urlPathBuild.Append(node.Url);
                }

                var path = urlPathBuild.ToString();

                ParseUrlParameter(ref path, parameter, null);

                // ��BodyҲ���
                if (!string.IsNullOrEmpty(apitmp.Body))
                {
                    var tmpBody = apitmp.Body;
                    ParseUrlParameter(ref tmpBody, parameter, null);
                    apitmp.Body = tmpBody;
                }

                apitmp.Url = path;

                urlPathStack.Clear();
                urlPathBuild.Clear();

                return apitmp;
            }

            throw new System.InvalidOperationException($"��ȡAPI����ʧ��[{name}]");
        }

        private static bool IteraGetUrl(string name, ICollection<ApiRequestData> group, ref bool findSuccess)
        {
            foreach (var a in group)
            {
                if (a.Name == name)
                {
                    findSuccess = true;
                }
                else if (a.Childs != null && a.Childs.Length > 0)
                {
                    IteraGetUrl(name, a.Childs, ref findSuccess);
                }

                if (findSuccess)
                {
                    urlPathStack.Push(a);
                    break;
                }
            }

            return findSuccess;
        }

        /// <summary>
        /// ����������������
        /// </summary>
        /// <param name="action"></param>
        /// <param name="callback"></param>
        public static void IteratorAllApi(System.Action<ApiRequestData> callback)
        {
             IteraUrl(requestGroups, callback);
        }

        private static void IteraUrl(ICollection<ApiRequestData> group, System.Action<ApiRequestData> callback)
        {
            foreach (var a in group)
            {
                callback.Invoke(a);

                if (a.Childs != null && a.Childs.Length > 0)
                {
                    IteraUrl(a.Childs, callback);
                }
            }
        }
    }
}