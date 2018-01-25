using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;



namespace Events.Groups {
namespace AstroidEvents {
namespace Methods {
public interface IReset : Tools.IEventMethodBase{ void Reset(); }

}public static class Invoke {
static List<Methods.IReset> _users_IReset  = new List<Methods.IReset>();
internal static void RegisterUser(Methods.IReset user){
if(user == null) return;
if(!_users_IReset.Contains(user)) _users_IReset.Add(user);
}
internal static void UnRegisterUser(Methods.IReset user){
if(user == null) return;
if(_users_IReset.Contains(user)) _users_IReset.Remove(user);
}
public static void Reset(){
_users_IReset.ForEach(x=> x.Reset());   
}

}public interface IAll_Group_Events:Methods.IReset{ }

}
}


namespace Events {
public partial class Tools {
static partial void RegesterUserImplementation(object user)  {
if(!(user is Tools.IEventMethodBase))return; if(user is Groups.AstroidEvents.Methods.IReset)
	Groups.AstroidEvents.Invoke.RegisterUser(user as Groups.AstroidEvents.Methods.IReset);

}static partial void UnRegesterUserImplementation(object user)  {
if(!(user is Tools.IEventMethodBase))return; if(user is Groups.AstroidEvents.Methods.IReset)
	Groups.AstroidEvents.Invoke.UnRegisterUser(user as Groups.AstroidEvents.Methods.IReset);

}
}
}