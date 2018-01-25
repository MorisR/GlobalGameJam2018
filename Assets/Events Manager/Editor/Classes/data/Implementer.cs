using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public abstract class Implementer
{
    protected string fullPath;

    protected Implementer(string path, string className, string classType)
    {
        fullPath = string.Format("{0}/{1}{2}", path, className, classType);
    }

    public abstract void Implement(EventMethodsGroupsCollection groups);




    protected static void WriteToStream(StreamWriter sw, params string[] str)
    {
        foreach (var item in str)
            sw.Write(item);

    }
    protected static void WrapIn(StreamWriter sw, string warper, Action methodToWarp)
    {
        sw.Write(warper);
        sw.Write(" {\n");
        methodToWarp();
        sw.Write("\n}");
    }
    protected static void WriteDelegate(StreamWriter sw, string delegateName, string[] argNamesAndTypes)
    {
        //delegate with the method signature
        //sr.WriteLine("public delegate void delegate_{0}_{1} ({2});", @group.GroupName, @method.Name,string.Join(",", args));
        //WriteToStream(sr, "public delegate void delegate_", @group.GroupName, "_", @method.Name, "(",string.Join(",", args), ");\n");
        if (argNamesAndTypes == null || argNamesAndTypes.Length == 0)
            WriteToStream(sw, "public delegate void ", delegateName, "();\n");
        else
            WriteToStream(sw, "public delegate void ", delegateName, "(", string.Join(",", argNamesAndTypes), ");\n");
    }

}



public class DelegatesImplementation : Implementer
{

    public DelegatesImplementation(string path, string className, string classType) : base(path, className, classType) { }



    public override void Implement(EventMethodsGroupsCollection groups)
    {
        using (StreamWriter sw = new StreamWriter(fullPath))
        {

            sw.AutoFlush = true;

            Implement_Namespaces(sw);
            sw.Write("\n\n\n");
            Implement_body(sw, groups);
            sw.Write("\n\n\n");
            Implement_Registrations(sw, groups);

            sw.Flush();

        }
    }


    void Implement_Namespaces(StreamWriter sw)
    {
        sw.WriteLine("using System;");
        sw.WriteLine("using UnityEngine;");
        sw.WriteLine("using System.Collections.Generic;");
        sw.WriteLine("using System.Linq;");

    }
    void Implement_body(StreamWriter sw, EventMethodsGroupsCollection groups)
    {

        WrapIn(sw, "namespace Events.Groups", () =>
        {

            foreach (var group in groups)
                WrapIn(sw, "namespace " + group.GroupName, () =>
                {

                    StringBuilder interfacesNames = new StringBuilder();
                    foreach (var method in group)
                    {
                        //----------------------------------------------------------------------------------
                        var args = method.Args.Select(x => string.Format("{0} {1}", NameValidator.GetGenericTypeFullName(x.ArgType), x.ArgName)).ToArray();
                        var argsAsMethodArgs = args.Length == 0 ? "" : string.Join(",", args);
                        var argsWithoutNames = method.Args.Select(x => NameValidator.GetGenericTypeName(x.ArgType)).ToArray();
                        var argTypes_AsName = argsWithoutNames.Length == 0 ? "" : string.Join("_", argsWithoutNames);
                        argTypes_AsName = Regex.Replace(argTypes_AsName, "[^A-Za-z0-9 _]", "");
                        //----------------------------------------------------------------------------------
                        WrapIn(sw, "namespace Methods", () =>
                        {

                            WrapIn(sw,
                                string.Format("public static class {0}{1}", @method.Name,
                                    (argsWithoutNames.Length > 0 ? string.Format("_{0}", argTypes_AsName) : "")), // methods arg types
                                () =>
                                {

                                    string delegateType = string.Format("delegate_{0}_{1}", @group.GroupName, @method.Name);
                                    WriteDelegate(sw, delegateName: delegateType, argNamesAndTypes: args);


                                    //write inteface
                                    if (argsWithoutNames.Length > 0)
                                    {
                                        WrapIn(sw,
                                            string.Format("public interface I{0}_{1} : Tools.IEventMethodBase", @method.Name, argTypes_AsName),
                                            () => { WriteToStream(sw, method.MethodFullSigneture); });

                                        interfacesNames.Append(string.Format("Methods.{0}_{1}.I{0}_{1},", @method.Name, argTypes_AsName));
                                    }
                                    else
                                    {
                                        WrapIn(sw, string.Format("public interface I{0} : Tools.IEventMethodBase", @method.Name), () =>
                                        {
                                            WriteToStream(sw, method.MethodFullSigneture);
                                        });

                                        interfacesNames.Append(string.Format("Methods.{0}.I{0},", @method.Name));

                                    }


                                    //delegate instance
                                    WriteToStream(sw, "private static ", delegateType, " _listener;\n");

                                    //register user method
                                    WrapIn(sw,
                                        string.Format("public static void RegisterListener({0} user)", delegateType),
                                        () =>
                                        {
                                            WriteToStream(sw, "if(user == null) return;");
                                            WriteToStream(sw, "if(_listener == null) _listener = user;");
                                            WriteToStream(sw, "else if(!_listener.GetInvocationList().Contains(user)) _listener += user;");
                                        });

                                    //Unregister user method
                                    WrapIn(sw,
                                        string.Format("public static void UnRegisterListener({0} user)", delegateType),
                                        () =>
                                        {
                                            WriteToStream(sw, "if(user == null) return;");
                                            WriteToStream(sw, "if(_listener == null) return;");
                                            WriteToStream(sw, "if(_listener.GetInvocationList().Contains(user)) _listener -= user;");
                                        });

                                    //invoke
                                    WrapIn(sw, string.Format("public static void Invoke({0})", argsAsMethodArgs),
                                     () =>
                                     {
                                         WriteToStream(sw, "if(_listener != null) _listener(", string.Join(",", method.Args.Select(x => x.ArgName).ToArray()), ");");
                                     });



                                });


                        });
                        //----------------------------------------------------------------------------------
                    }


                    //interface IAll_Group_Events     
                    sw.Write("public interface IAll_Group_Events");
                    if (!string.IsNullOrEmpty(interfacesNames.ToString()))
                    {
                        interfacesNames = interfacesNames.Remove(interfacesNames.Length - 1, 1); //remove the "," at the end of the string builder
                        WriteToStream(sw, ":", interfacesNames.ToString());
                    }
                    sw.Write("{ }\n");


                });//end namespace {GroupName} warp


        }); //end namespace "Events.Groups" warp

    }
    void Implement_Registrations(StreamWriter sw, EventMethodsGroupsCollection groups)
    {
        WrapIn(sw, "namespace Events", () =>
        {

            WrapIn(sw, "public partial class Tools", () =>
            {

                //mehtod 1 
                WrapIn(sw, "static partial void RegesterUserImplementation(object user) ", () =>
                {
                    WriteToStream(sw, "if(!(user is Tools.IEventMethodBase))return; ");

                    foreach (var @group in groups)
                        foreach (var method in group)
                        {
                            var argsWithoutNames = method.Args.Select(x => NameValidator.GetGenericTypeName(x.ArgType)).ToArray();
                            var argTypes_AsName = argsWithoutNames.Length == 0 ? "" : "_" + string.Join("_", argsWithoutNames);
                            argTypes_AsName = Regex.Replace(argTypes_AsName, "[^A-Za-z0-9 _]", "");

                            WriteToStream(sw, "if(user is Groups.", group.GroupName, ".Methods.", method.Name, argTypes_AsName, ".I", method.Name, argTypes_AsName, ")\n\t");
                            WriteToStream(sw, "Groups.", group.GroupName, ".Methods.", method.Name, argTypes_AsName,
                                ".RegisterListener((user as Groups.", group.GroupName, ".Methods.", method.Name,
                                argTypes_AsName, ".I", method.Name, argTypes_AsName, ").", method.Name, ");\n");
                        }

                });

                //mehtod 2
                WrapIn(sw, "static partial void UnRegesterUserImplementation(object user) ", () =>
                {
                    WriteToStream(sw, "if(!(user is Tools.IEventMethodBase))return; ");

                    foreach (var @group in groups)
                        foreach (var method in group)
                        {
                            var argsWithoutNames = method.Args.Select(x => NameValidator.GetGenericTypeName(x.ArgType)).ToArray();
                            var argTypes_AsName = argsWithoutNames.Length == 0 ? "" : "_" + string.Join("_", argsWithoutNames);
                            argTypes_AsName = Regex.Replace(argTypes_AsName, "[^A-Za-z0-9 _]", "");

                            WriteToStream(sw, "if(user is Groups.", group.GroupName, ".Methods.", method.Name, argTypes_AsName, ".I", method.Name, argTypes_AsName, ")\n\t");
                            WriteToStream(sw, "Groups.", group.GroupName, ".Methods.", method.Name, argTypes_AsName,
                                ".UnRegisterListener((user as Groups.", group.GroupName, ".Methods.", method.Name,
                                argTypes_AsName, ".I", method.Name, argTypes_AsName, ").", method.Name, ");\n");
                        }

                });


            });

        });
    }




}

public class ListsImplemntation : Implementer
{
    public ListsImplemntation(string path, string className, string classType) : base(path, className, classType)
    {
    }

    public override void Implement(EventMethodsGroupsCollection groups)
    {
        using (StreamWriter sw = new StreamWriter(fullPath))
        {

            sw.AutoFlush = true;

            Implement_Namespaces(sw);
            sw.Write("\n\n\n");
            Implement_body(sw, groups);
            sw.Write("\n\n\n");
            Implement_Registrations(sw, groups);

            sw.Flush();

        }
    }

    void Implement_Namespaces(StreamWriter sw)
    {
        sw.WriteLine("using System;");
        sw.WriteLine("using UnityEngine;");
        sw.WriteLine("using System.Collections.Generic;");
        sw.WriteLine("using System.Linq;");

    }
    void Implement_body(StreamWriter sw, EventMethodsGroupsCollection groups)
    {

        WrapIn(sw, "namespace Events.Groups", () =>
        {

            foreach (var group in groups)
                WrapIn(sw, "namespace " + group.GroupName, () =>
                {

                    StringBuilder interfacesNames = new StringBuilder();

                    StringBuilder interfaceDeclereration = new StringBuilder();

                    StringBuilder invokerListDecloration = new StringBuilder();

                    StringBuilder invokerMethodsDecloration = new StringBuilder();



                    foreach (var method in group)
                    {
                        //----------------------------------------------------------------------------------
                        var args =
                            method.Args.Select(
                                x =>
                                    string.Format("{0} {1}", NameValidator.GetGenericTypeFullName(x.ArgType),
                                        x.ArgName)).ToArray();
                        var argsAsMethodArgs = args.Length == 0 ? "" : string.Join(",", args);
                        var argsWithoutNames =
                            method.Args.Select(x => NameValidator.GetGenericTypeName(x.ArgType)).ToArray();
                        var argTypes_AsName = argsWithoutNames.Length == 0 ? "" : string.Join("_", argsWithoutNames);
                        argTypes_AsName = Regex.Replace(argTypes_AsName, "[^A-Za-z0-9 _]", "");
                        //----------------------------------------------------------------------------------
                        var interfaceName = (argsWithoutNames.Length > 0)
                            ? string.Format("I{0}_{1}", @method.Name, argTypes_AsName)
                            : string.Format("I{0}", @method.Name);



                        interfaceDeclereration.AppendLine(string.Format("public interface {0} : Tools.IEventMethodBase{{ {1} }}", interfaceName, method.MethodFullSigneture));


                        invokerListDecloration.AppendLine(string.Format("static List<Methods.{0}> _users_{0}  = new List<Methods.{0}>();", interfaceName));


                        invokerMethodsDecloration.AppendLine(string.Format("internal static void RegisterUser(Methods.{0} user){{", interfaceName));
                        invokerMethodsDecloration.AppendLine("if(user == null) return;");
                        invokerMethodsDecloration.AppendLine(string.Format("if(!_users_{0}.Contains(user)) _users_{0}.Add(user);\n}}", interfaceName));



                        invokerMethodsDecloration.AppendLine(string.Format("internal static void UnRegisterUser(Methods.{0} user){{", interfaceName));
                        invokerMethodsDecloration.AppendLine("if(user == null) return;");
                        invokerMethodsDecloration.AppendLine(string.Format("if(_users_{0}.Contains(user)) _users_{0}.Remove(user);\n}}", interfaceName));


                        invokerMethodsDecloration.AppendLine(string.Format("public static void {0}({1}){{", method.Name, argsAsMethodArgs));
                        invokerMethodsDecloration.AppendLine(string.Format("_users_{0}.ForEach(x=> x.{1}({2}));   \n}}", interfaceName, method.Name, string.Join(",", method.Args.Select(x => x.ArgName).ToArray())));




                        // is used for the IAll_Group_Events interface
                        interfacesNames.Append(string.Format("Methods.{0},", interfaceName));

                        //----------------------------------------------------------------------------------
                    }


                    WrapIn(sw, "namespace Methods", () =>
                    {
                        WriteToStream(sw, interfaceDeclereration.ToString());
                    });

                    //static
                    WrapIn(sw, "public static class Invoke", () =>
                    {
                        WriteToStream(sw, invokerListDecloration.ToString());
                        WriteToStream(sw, invokerMethodsDecloration.ToString());

                    });

                    //interface IAll_Group_Events     
                    sw.Write("public interface IAll_Group_Events");
                    if (!string.IsNullOrEmpty(interfacesNames.ToString()))
                    {
                        interfacesNames = interfacesNames.Remove(interfacesNames.Length - 1, 1); //remove the "," at the end of the string builder
                        WriteToStream(sw, ":", interfacesNames.ToString());
                    }
                    sw.Write("{ }\n");


                });//end namespace {GroupName} warp


        }); //end namespace "Events.Groups" warp

    }
    void Implement_Registrations(StreamWriter sw, EventMethodsGroupsCollection groups)
    {
        WrapIn(sw, "namespace Events", () =>
        {

            WrapIn(sw, "public partial class Tools", () =>
            {

                //mehtod 1 
                WrapIn(sw, "static partial void RegesterUserImplementation(object user) ", () =>
                {
                    WriteToStream(sw, "if(!(user is Tools.IEventMethodBase))return; ");

                    foreach (var @group in groups)
                        foreach (var method in group)
                        {
                            var argsWithoutNames = method.Args.Select(x => NameValidator.GetGenericTypeName(x.ArgType)).ToArray();
                            var argTypes_AsName = argsWithoutNames.Length == 0 ? "" : "_" + string.Join("_", argsWithoutNames);
                            argTypes_AsName = Regex.Replace(argTypes_AsName, "[^A-Za-z0-9 _]", "");

                            WriteToStream(sw, "if(user is Groups.", group.GroupName, ".Methods.I", method.Name, argTypes_AsName, ")\n\t");
                            WriteToStream(sw, "Groups.", group.GroupName, ".Invoke.RegisterUser(user as Groups.", group.GroupName, ".Methods.I", method.Name, argTypes_AsName, ");\n");
                        }

                });

                //mehtod 2
                WrapIn(sw, "static partial void UnRegesterUserImplementation(object user) ", () =>
                {
                    WriteToStream(sw, "if(!(user is Tools.IEventMethodBase))return; ");

                    foreach (var @group in groups)
                        foreach (var method in group)
                        {
                            var argsWithoutNames = method.Args.Select(x => NameValidator.GetGenericTypeName(x.ArgType)).ToArray();
                            var argTypes_AsName = argsWithoutNames.Length == 0 ? "" : "_" + string.Join("_", argsWithoutNames);
                            argTypes_AsName = Regex.Replace(argTypes_AsName, "[^A-Za-z0-9 _]", "");

                            WriteToStream(sw, "if(user is Groups.", group.GroupName, ".Methods.I", method.Name, argTypes_AsName, ")\n\t");
                            WriteToStream(sw, "Groups.", group.GroupName, ".Invoke.UnRegisterUser(user as Groups.", group.GroupName, ".Methods.I", method.Name, argTypes_AsName, ");\n");
                        }

                });


            });

        });
    }


}

public class ListsImplemntation2 : Implementer
{
    public ListsImplemntation2(string path, string className, string classType) : base(path, className, classType)
    {
    }

    public override void Implement(EventMethodsGroupsCollection groups)
    {
        using (StreamWriter sw = new StreamWriter(fullPath))
        {

            sw.AutoFlush = true;

            Implement_Namespaces(sw);
            sw.Write("\n\n\n");
            Implement_body(sw, groups);
            sw.Write("\n\n\n");
            Implement_Registrations(sw, groups);

            sw.Flush();

        }
    }

    void Implement_Namespaces(StreamWriter sw)
    {
        sw.WriteLine("using System;");
        sw.WriteLine("using UnityEngine;");
        sw.WriteLine("using System.Collections.Generic;");
        sw.WriteLine("using System.Linq;");

    }
    void Implement_body(StreamWriter sw, EventMethodsGroupsCollection groups)
    {

        WrapIn(sw, "namespace Events.Groups", () =>
        {

            foreach (var group in groups)
                WrapIn(sw, "namespace " + group.GroupName, () =>
                {

                    StringBuilder interfacesNames = new StringBuilder();


                    WrapIn(sw, "namespace Methods", () =>
                    {

                        foreach (var method in group)
                        {
                            //----------------------------------------------------------------------------------
                            var args =
                                method.Args.Select(
                                    x =>
                                        string.Format("{0} {1}", NameValidator.GetGenericTypeFullName(x.ArgType),
                                            x.ArgName)).ToArray();
                            var argsAsMethodArgs = args.Length == 0 ? "" : string.Join(",", args);
                            var argsWithoutNames =
                                method.Args.Select(x => NameValidator.GetGenericTypeName(x.ArgType)).ToArray();
                            var argTypes_AsName = argsWithoutNames.Length == 0 ? "" : string.Join("_", argsWithoutNames);
                            argTypes_AsName = Regex.Replace(argTypes_AsName, "[^A-Za-z0-9 _]", "");
                            //----------------------------------------------------------------------------------
                            var interfaceName = (argsWithoutNames.Length > 0)
                                ? string.Format("I{0}_{1}", @method.Name, argTypes_AsName)
                                : string.Format("I{0}", @method.Name);



                            WrapIn(sw,
                                string.Format("public static class {0}{1}", @method.Name,
                                    (argsWithoutNames.Length > 0 ? string.Format("_{0}", argTypes_AsName) : "")),
                                // methods arg types
                                () =>
                                {

                                    WriteToStream(sw, "static List<", interfaceName, "> _users = new List<",
                                        interfaceName, ">();");


                                    //write inteface
                                    WrapIn(sw,
                                        string.Format("public interface {0} : Tools.IEventMethodBase", interfaceName),
                                        () => { WriteToStream(sw, method.MethodFullSigneture); });

                                    interfacesNames.Append(string.Format("Methods.{0}.{1},", interfaceName.Substring(1),
                                        interfaceName));




                                    //register user method
                                    WrapIn(sw,
                                        string.Format("internal static void RegisterListener({0} user)", interfaceName),
                                        () =>
                                        {
                                            WriteToStream(sw, "if(user == null) return;");
                                            WriteToStream(sw, "if(!_users.Contains(user)) _users.Add(user);");
                                        });

                                    //Unregister user method
                                    WrapIn(sw,
                                        string.Format("internal static void UnRegisterListener({0} user)", interfaceName),
                                        () =>
                                        {
                                            WriteToStream(sw, "if(user == null) return;");
                                            WriteToStream(sw, "if(_users.Contains(user)) _users.Remove(user);");
                                        });

                                    //invoke
                                    WrapIn(sw, string.Format("public static void Invoke({0})", argsAsMethodArgs),
                                        () =>
                                        {
                                            WriteToStream(sw, "_users.ForEach(x=> x.", method.Name, "(",
                                                string.Join(",", method.Args.Select(x => x.ArgName).ToArray()), "));");
                                        });



                                });


                            //----------------------------------------------------------------------------------
                        }
                    });

                    //interface IAll_Group_Events     
                    sw.Write("public interface IAll_Group_Events");
                    if (!string.IsNullOrEmpty(interfacesNames.ToString()))
                    {
                        interfacesNames = interfacesNames.Remove(interfacesNames.Length - 1, 1); //remove the "," at the end of the string builder
                        WriteToStream(sw, ":", interfacesNames.ToString());
                    }
                    sw.Write("{ }\n");


                });//end namespace {GroupName} warp


        }); //end namespace "Events.Groups" warp

    }
    void Implement_Registrations(StreamWriter sw, EventMethodsGroupsCollection groups)
    {
        WrapIn(sw, "namespace Events", () =>
        {

            WrapIn(sw, "public partial class Tools", () =>
            {

                //mehtod 1 
                WrapIn(sw, "static partial void RegesterUserImplementation(object user) ", () =>
                {
                    WriteToStream(sw, "if(!(user is Tools.IEventMethodBase))return; ");

                    foreach (var @group in groups)
                        foreach (var method in group)
                        {
                            var argsWithoutNames = method.Args.Select(x => NameValidator.GetGenericTypeName(x.ArgType)).ToArray();
                            var argTypes_AsName = argsWithoutNames.Length == 0 ? "" : "_" + string.Join("_", argsWithoutNames);
                            argTypes_AsName = Regex.Replace(argTypes_AsName, "[^A-Za-z0-9 _]", "");

                            WriteToStream(sw, "if(user is Groups.", group.GroupName, ".Methods.", method.Name, argTypes_AsName, ".I", method.Name, argTypes_AsName, ")\n\t");
                            WriteToStream(sw, "Groups.", group.GroupName, ".Methods.", method.Name, argTypes_AsName,
                                ".RegisterListener(user as Groups.", group.GroupName, ".Methods.", method.Name,
                                argTypes_AsName, ".I", method.Name, argTypes_AsName, ");\n");
                        }

                });

                //mehtod 2
                WrapIn(sw, "static partial void UnRegesterUserImplementation(object user) ", () =>
                {
                    WriteToStream(sw, "if(!(user is Tools.IEventMethodBase))return; ");

                    foreach (var @group in groups)
                        foreach (var method in group)
                        {
                            var argsWithoutNames = method.Args.Select(x => NameValidator.GetGenericTypeName(x.ArgType)).ToArray();
                            var argTypes_AsName = argsWithoutNames.Length == 0 ? "" : "_" + string.Join("_", argsWithoutNames);
                            argTypes_AsName = Regex.Replace(argTypes_AsName, "[^A-Za-z0-9 _]", "");

                            WriteToStream(sw, "if(user is Groups.", group.GroupName, ".Methods.", method.Name, argTypes_AsName, ".I", method.Name, argTypes_AsName, ")\n\t");
                            WriteToStream(sw, "Groups.", group.GroupName, ".Methods.", method.Name, argTypes_AsName,
                                ".UnRegisterListener(user as Groups.", group.GroupName, ".Methods.", method.Name,
                                argTypes_AsName, ".I", method.Name, argTypes_AsName, ");\n");
                        }

                });


            });

        });
    }


}



//example - template for the invoker class
/*

namespace Events.Groups
{

    namespace Group1
    {

        namespace Methods
        {
            public static class MethodName
            {
               public delegate void delegate_group1_method_1();
                public interface IMethod_1
                {

                }

                static delegate_group1_method_1 _delegate;

                public static void AddListener(delegate_group1_method_1 other)
                {
                    if(other == null)return;
                    if (_delegate == null) _delegate = other;
                    else if(!_delegate.GetInvocationList().Contains(other)) _delegate += other;
                }
                public static void RemoveListener(IMethod_1 listener) { }
                public static void InvokeUsers()
                {
                    _delegate();
                }

            }
        }
        public interface IAll_Group_Events : Methods.MethodName.IMethod_1
        {

        }
    }



}


namespace Events
{
    using Groups;
    public static class Tools
    {
        public static void RegesterUser(object user)
        {
           // if(user is Groups.g1.Methods.e1.Ie1)
            //    Groups.g1.Methods.e1.RegisterListener((user as Groups.g1.Methods.e1.Ie1).e1);
        }
        public static void UnregesterUser(object user)
        {
            
        }
    }
}*/
