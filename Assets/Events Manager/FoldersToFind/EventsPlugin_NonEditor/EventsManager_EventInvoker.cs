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

}namespace Astroid {
namespace Methods {
public interface IFlyAwayFromPlayer_Vector3_Single : Tools.IEventMethodBase{ void FlyAwayFromPlayer(UnityEngine.Vector3 player_position , System.Single speed); }

}public static class Invoke {
static List<Methods.IFlyAwayFromPlayer_Vector3_Single> _users_IFlyAwayFromPlayer_Vector3_Single  = new List<Methods.IFlyAwayFromPlayer_Vector3_Single>();
internal static void RegisterUser(Methods.IFlyAwayFromPlayer_Vector3_Single user){
if(user == null) return;
if(!_users_IFlyAwayFromPlayer_Vector3_Single.Contains(user)) _users_IFlyAwayFromPlayer_Vector3_Single.Add(user);
}
internal static void UnRegisterUser(Methods.IFlyAwayFromPlayer_Vector3_Single user){
if(user == null) return;
if(_users_IFlyAwayFromPlayer_Vector3_Single.Contains(user)) _users_IFlyAwayFromPlayer_Vector3_Single.Remove(user);
}
public static void FlyAwayFromPlayer(UnityEngine.Vector3 player_position,System.Single speed){
_users_IFlyAwayFromPlayer_Vector3_Single.ForEach(x=> x.FlyAwayFromPlayer(player_position,speed));   
}

}public interface IAll_Group_Events:Methods.IFlyAwayFromPlayer_Vector3_Single{ }

}
}


namespace Events {
public partial class Tools {
static partial void RegesterUserImplementation(object user)  {
if(!(user is Tools.IEventMethodBase))return; if(user is Groups.Resetable.Methods.IResetInstance)
	Groups.Resetable.Invoke.RegisterUser(user as Groups.Resetable.Methods.IResetInstance);
if(user is Groups.Astroid.Methods.IFlyAwayFromPlayer_Vector3_Single)
	Groups.Astroid.Invoke.RegisterUser(user as Groups.Astroid.Methods.IFlyAwayFromPlayer_Vector3_Single);

}static partial void UnRegesterUserImplementation(object user)  {
if(!(user is Tools.IEventMethodBase))return; if(user is Groups.Resetable.Methods.IResetInstance)
	Groups.Resetable.Invoke.UnRegisterUser(user as Groups.Resetable.Methods.IResetInstance);
if(user is Groups.Astroid.Methods.IFlyAwayFromPlayer_Vector3_Single)
	Groups.Astroid.Invoke.UnRegisterUser(user as Groups.Astroid.Methods.IFlyAwayFromPlayer_Vector3_Single);

}
}
}