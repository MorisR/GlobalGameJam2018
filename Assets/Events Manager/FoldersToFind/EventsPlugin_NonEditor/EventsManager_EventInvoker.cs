using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;



namespace Events.Groups {
namespace g1 {
namespace Methods {
public interface Ie1 : Tools.IEventMethodBase{ void e1(); }
public interface Ie2 : Tools.IEventMethodBase{ void e2(); }

}public static class Invoke {
static List<Methods.Ie1> _users_Ie1  = new List<Methods.Ie1>();
static List<Methods.Ie2> _users_Ie2  = new List<Methods.Ie2>();
internal static void RegisterUser(Methods.Ie1 user){
if(user == null) return;
if(!_users_Ie1.Contains(user)) _users_Ie1.Add(user);
}
internal static void UnRegisterUser(Methods.Ie1 user){
if(user == null) return;
if(_users_Ie1.Contains(user)) _users_Ie1.Remove(user);
}
public static void e1(){
_users_Ie1.ForEach(x=> x.e1());   
}
internal static void RegisterUser(Methods.Ie2 user){
if(user == null) return;
if(!_users_Ie2.Contains(user)) _users_Ie2.Add(user);
}
internal static void UnRegisterUser(Methods.Ie2 user){
if(user == null) return;
if(_users_Ie2.Contains(user)) _users_Ie2.Remove(user);
}
public static void e2(){
_users_Ie2.ForEach(x=> x.e2());   
}

}public interface IAll_Group_Events:Methods.Ie1,Methods.Ie2{ }

}namespace rrr {
namespace Methods {

}public static class Invoke {

}public interface IAll_Group_Events{ }

}
}


namespace Events {
public partial class Tools {
static partial void RegesterUserImplementation(object user)  {
if(!(user is Tools.IEventMethodBase))return; if(user is Groups.g1.Methods.Ie1)
	Groups.g1.Invoke.RegisterUser(user as Groups.g1.Methods.Ie1);
if(user is Groups.g1.Methods.Ie2)
	Groups.g1.Invoke.RegisterUser(user as Groups.g1.Methods.Ie2);

}static partial void UnRegesterUserImplementation(object user)  {
if(!(user is Tools.IEventMethodBase))return; if(user is Groups.g1.Methods.Ie1)
	Groups.g1.Invoke.UnRegisterUser(user as Groups.g1.Methods.Ie1);
if(user is Groups.g1.Methods.Ie2)
	Groups.g1.Invoke.UnRegisterUser(user as Groups.g1.Methods.Ie2);

}
}
}