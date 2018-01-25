using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;



namespace Events.Groups {
namespace Resetable {
namespace Methods {
public interface IResetInstance : Tools.IEventMethodBase{ void ResetInstance(); }

}public static class Invoke {
static List<Methods.IResetInstance> _users_IResetInstance  = new List<Methods.IResetInstance>();
internal static void RegisterUser(Methods.IResetInstance user){
if(user == null) return;
if(!_users_IResetInstance.Contains(user)) _users_IResetInstance.Add(user);
}
internal static void UnRegisterUser(Methods.IResetInstance user){
if(user == null) return;
if(_users_IResetInstance.Contains(user)) _users_IResetInstance.Remove(user);
}
public static void ResetInstance(){
_users_IResetInstance.ForEach(x=> x.ResetInstance());   
}

}public interface IAll_Group_Events:Methods.IResetInstance{ }

}
}


namespace Events {
public partial class Tools {
static partial void RegesterUserImplementation(object user)  {
if(!(user is Tools.IEventMethodBase))return; if(user is Groups.Resetable.Methods.IResetInstance)
	Groups.Resetable.Invoke.RegisterUser(user as Groups.Resetable.Methods.IResetInstance);

}static partial void UnRegesterUserImplementation(object user)  {
if(!(user is Tools.IEventMethodBase))return; if(user is Groups.Resetable.Methods.IResetInstance)
	Groups.Resetable.Invoke.UnRegisterUser(user as Groups.Resetable.Methods.IResetInstance);

}
}
}