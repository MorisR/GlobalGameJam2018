using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public sealed class EventManagerBase : MonoBehaviour {


    List<MonoBehaviour> components;

    private void Awake()
    {
        var comp = GetComponents<MonoBehaviour>();
        components = comp.Where(x => x is Events.Tools.IEventMethodBase).ToList(); ;
    }

    private void OnEnable()
    {
        components.ForEach(com => Events.Tools.RegesterUser(com));
    }

    private void OnDisable()
    {
        components.ForEach(com => Events.Tools.UnRegesterUser(com));
    }
}
