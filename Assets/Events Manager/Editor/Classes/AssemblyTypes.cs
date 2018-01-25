using System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;


    [InitializeOnLoad]
    public static class AssemblyTypes
    {
        static List<Type> allAssemblyTypes;
        static List<Type> nonGenericTypes;
        static List<Type> genericTypes;

        static List<string> namespaces;

        static AssemblyTypes()
        {
            Namespaces.Add("UnityEngine");
            Namespaces.Add("System");
            Namespaces.Add("UnityEngine");
            Initialize();
        }

        public static List<Type> AllAssemblyTypes
        {
            get
            {
                return allAssemblyTypes;
            }
            private set { }
        }
        public static List<Type> NonGenericTypes
        {
            get { return nonGenericTypes; }
            set { nonGenericTypes = value; }
        }
        public static List<Type> GenericTypes
        {
            get { return genericTypes; }
            set { genericTypes = value; }
        }


        public static List<string> Namespaces
        {
            get
            {
                if (namespaces == null) namespaces = new List<string>();
                return namespaces;
            }
            set { }
        }



        static void Initialize()
        {
            var tempClassesAndStructs = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(t => t.GetTypes())
                .Where(
                    t =>
                        (t.IsClass || t.IsValueType || t.IsInterface)
                        && (t.IsVisible)
                        && (Namespaces.Contains(t.Namespace)
                            || string.IsNullOrEmpty(t.Namespace)
                            ||Namespaces.Any(ns => string.IsNullOrEmpty(t.Namespace) ? false : t.Namespace.Contains(ns)))

                ).ToList();

            /*
            var subClassesAndStructs =
                tempClassesAndStructs.SelectMany(
                    x => x.Assembly.GetTypes().Where(z => z.IsAssignableFrom(x) && z.IsVisible && z.IsPublic)).ToList();
            */

            var subClassesAndStructs =
                tempClassesAndStructs.SelectMany(
                    x => x.Assembly.GetTypes().Where(z =>(!x.IsInterface)? (z.IsSubclassOf(x)) : z.IsAssignableFrom(x)
                    && z.IsVisible && z.IsPublic)).ToList();


            allAssemblyTypes = new List<Type>();
            allAssemblyTypes.AddRange(tempClassesAndStructs);
            allAssemblyTypes.AddRange(subClassesAndStructs);
            allAssemblyTypes = allAssemblyTypes.Distinct().ToList();
            allAssemblyTypes.Sort(
                (type, type1) => String.Compare(type.FullName, type1.FullName, StringComparison.Ordinal));


            NonGenericTypes = allAssemblyTypes.Where(x => !x.IsGenericType).ToList();
            GenericTypes = allAssemblyTypes.Where(x => x.IsGenericType).ToList();



        }

        public static void LogNames(string fileName)
        {
            using (StreamWriter sr = new StreamWriter(fileName))
            {
                var temp = AllAssemblyTypes.Select(x => x.FullName.Replace('+', '.')).Distinct().ToList();

                for (int i = 0; i < temp.Count; i++)
                    sr.WriteLine(temp[i]);
                sr.Flush();
            }
            Debug.Log(string.Format("All \"classes,interfaces,structs\"'s names are saved in {0}", fileName));

        }
    }


