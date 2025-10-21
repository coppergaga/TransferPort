using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using KMod;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RsLib
{
    public static class RsUtil
    {
        public static string GetModPath(Assembly modDLL)
        {
            if (modDLL == null)
                throw new ArgumentNullException(nameof(modDLL));
            string str = null;
            try
            {
                str = Directory.GetParent(modDLL.Location)?.FullName;
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }

            if (str == null)
                str = Path.Combine(Manager.GetDirectory(), modDLL.GetName()?.Name ?? "");
            return str;
        }

        public static void AddBuildingToTech(string techID, string buildingID)
        {
            var tech = Db.Get().Techs.Get(techID);
            var flag = tech != null;
            if (flag)
                tech.unlockedItemIDs.Add(buildingID);
            else
                Debug.LogWarning("AddBuildingToTech() Failed to find tech ID: " + techID);
        }

        public static void AddPlanScreenAndTech(HashedString category, string techID, string buildingID)
        {
            ModUtil.AddBuildingToPlanScreen(category, buildingID);
            AddBuildingToTech(techID, buildingID);
        }

        public static void AddPlanScreenAndTech(HashedString category, string techID, string buildingID,
            string subcategoryID)
        {
            ModUtil.AddBuildingToPlanScreen(category, buildingID, subcategoryID);
            AddBuildingToTech(techID, buildingID);
        }


        public static void ContrastSet<T>(ISet<T> source, ISet<T> target, Action<T> onAdd = null, Action<T> onRemove = null)
        {
            ISet<T> oldTarget = new HashSet<T>(target);
           foreach (T x1 in source)
           {
               if (!target.Contains(x1))
               {
                   target.Add(x1);
                   onAdd?.Invoke(x1);
               }
               else
               {
                   oldTarget.Remove(x1);
               }
           }
           foreach (T x1 in oldTarget)
           {
               target.Remove(x1);
               onRemove?.Invoke(x1);
           }
        }


        /// <summary>
        /// 散点就近有序连接排序
        /// </summary>
        public static void NearestSort(Vector2[] points)
        {
            int len = points.Length;
            if (len < 2)
            {
                return;
            }
            //先获取最近零点
            int sp = NearestPoint(Vector2.zero, points, 0);
            //交换
            Vector2 temp = points[sp];
            points[sp] = points[0];
            points[0] = temp;
            
            for (int i = 1; i < len; i++)
            {
                //查找最近的点
                int point = NearestPoint(points[i - 1], points, i);
                //交换
                temp = points[i];
                points[i] = points[point];
                points[point] = temp;
            }
        }
        
        public static void NearestSort(IList<GameObject> points)
        {
            int len = points.Count;
            if (len < 2)
            {
                return;
            }
            //先获取最近零点
            int sp = NearestGo(null, points, 0);
            //交换
            GameObject temp = points[sp];
            points[sp] = points[0];
            points[0] = temp;
            
            for (int i = 1; i < len; i++)
            {
                //查找最近的点
                int point = NearestGo(points[i - 1], points, i);
                //交换
                temp = points[i];
                points[i] = points[point];
                points[point] = temp;
            }
        }
        
        public static int NearestGo(GameObject target, IList<GameObject> points, int startIndex)
        {
            float shortest = float.MaxValue;
            int shortestIdx = startIndex;
            for (var i = startIndex; i < points.Count; i++)
            {
                float magnitude = Vector2.SqrMagnitude((target == null ? Vector3.zero : target.transform.position) - points[i].transform.position);
                if (magnitude < shortest)
                {
                    shortest = magnitude;
                    shortestIdx = i;
                }
            }

            return shortestIdx;
        }
        
        public static void NearestSort(GameObject[] points)
        {
            int len = points.Length;
            if (len < 2)
            {
                return;
            }
            //先获取最近零点
            int sp = NearestGo(null, points, 0);
            //交换
            GameObject temp = points[sp];
            points[sp] = points[0];
            points[0] = temp;
            
            for (int i = 1; i < len; i++)
            {
                //查找最近的点
                int point = NearestGo(points[i - 1], points, i);
                //交换
                temp = points[i];
                points[i] = points[point];
                points[point] = temp;
            }
        }
        
        public static int NearestGo(GameObject target, GameObject[] points, int startIndex)
        {
            float shortest = float.MaxValue;
            int shortestIdx = startIndex;
            for (var i = startIndex; i < points.Length; i++)
            {
                float magnitude = Vector2.SqrMagnitude((target == null ? Vector3.zero : target.transform.position) - points[i].transform.position);
                if (magnitude < shortest)
                {
                    shortest = magnitude;
                    shortestIdx = i;
                }
            }

            return shortestIdx;
        }

        public static int NearestPoint(Vector2 target, Vector2[] points, int startIndex)
        {
            float shortest = float.MaxValue;
            int shortestIdx = startIndex;
            for (var i = startIndex; i < points.Length; i++)
            {
                float magnitude = Vector2.SqrMagnitude(target - points[i]);
                if (magnitude < shortest)
                {
                    shortest = magnitude;
                    shortestIdx = i;
                }
            }

            return shortestIdx;
        }

        public static void SetParent(this GameObject target, GameObject parent, bool worldPositionStays = true)
        {
            if (target == null)
            {
                Debug.LogWarning("target is null");
                return;
            }
            target.transform.SetParent(parent != null ? parent.transform : null, worldPositionStays);
        }

        /// <summary>
        /// 不重复设置active
        /// </summary>
        /// <param name="go"></param>
        /// <param name="active"></param>
        public static void SetActiveNR(this GameObject go, bool active)
        {
            if (go.activeSelf == active)
            {
                return;
            }
            go.SetActive(active);
        }
        
        // public static bool IsNullOrDestroyed(Component com)
        // {
        //     if (com == null)
        //         return true;
        //
        //     if ((object) (com as Component) != null && com as Component != null)
        //     {
        //         return IsNullOrDestroyed((GameObject) com.gameObject);
        //     }
        //     
        //     return true;
        // }
        
        public static bool IsNullOrDestroyed(Object obj)
        {
            if (obj == null)
                return true;
            return (object) (obj as UnityEngine.Object) != null && obj as UnityEngine.Object == (UnityEngine.Object) null;
        }
    }
}