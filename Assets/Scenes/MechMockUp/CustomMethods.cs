using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DGW
{
    public class CustomMethods
    {
        #region 独自メソッド
        /// <summary> 
        /// <para>第１引数が真の時のみ第２引数の処理を実行する </para>
        /// When 1st Argument is True, Do 2nd Arguments Process
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="action"></param>
        public void When(bool condition , Action action)
        {
            if(condition) { action(); }
        }
        /* ------------------------------------------------------------------ */
        #endregion
    }
    public static class Extentions
    {
        /// <summary>指定されたトランスフォームの子オブジェクトにする</summary>
        /// <param name="obj"></param>
        /// <param name="parent"></param>
        public static void ToChildObject(this GameObject obj, Transform parent)
        {
            obj.transform.parent = parent;
        }
        /// <summary>オブジェクトの親子関係を切る</summary>
        /// <param name="obj"></param>
        public static void ToParenObject(this GameObject obj)
        {
            obj.transform.parent = null;
        }
        public static List<Transform> GetChildObjects(this GameObject parent)
        {
            List<Transform> list = new();
            var cnt = parent.transform.childCount;
            for(int i = 0; i < cnt; i++)
            {
                var child = parent.transform.GetChild(i);
                list.Add(child);
            }
            return list;
        }
    }
}
