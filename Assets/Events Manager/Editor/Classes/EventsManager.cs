using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Extentions;


[ExecuteInEditMode]
public class EventsManager
{

    //statics---------------------------------------------------------------------------
    private static EventsManager instance;

    public static EventsManager Instance
    {
        get { return instance ?? (instance = new EventsManager()); }
    }

    //fields----------------------------------------------------------------------------
    ISaveLoad saveLoad;
    Implementer implementer;
    EventMethodsGroupsCollection groups;
    string fileName;

    //constructors----------------------------------------------------------------------
    private EventsManager()
    {
        InitializeSaveLaod();

        InitializeImplementer();


    }
    void InitializeSaveLaod()
    {
        //find the IsaveLoad default Path 
        string path = Application.dataPath;
        var directories = Directory.GetDirectories(Application.dataPath, "others_eventsPlugin", SearchOption.AllDirectories);
        if (directories.Length > 0)
            path = directories[0];

        //initialize the ISaveLoad
        SaveLoad = new IXmlSerializableSaveLoad(path, "EventsManager", ".xml");
        Load();
        groups.AddListener_OnModify(Save);
    }
    void InitializeImplementer()
    {
        //find the Implementer default Path 
        string path = Application.dataPath;

        var directories = Directory.GetDirectories(Application.dataPath, "EventsPlugin_NonEditor",
            SearchOption.AllDirectories);
        if (directories.Length > 0)
            path = directories[0];

        //initialize the Implementer

        implementer = new ListsImplemntation(path, "EventsManager_EventInvoker", ".cs");
        UpdateOrCreateInvokerClass();
    }





    private EventsManager(ISaveLoad saveLoad, Implementer implementer)
    {
        if(saveLoad == null || implementer == null)
            throw  new ArgumentNullException();
     
        SaveLoad = saveLoad;
        Load();
        groups.AddListener_OnModify(Save);

        this.implementer = implementer;
        UpdateOrCreateInvokerClass();

    }

    //properties------------------------------------------------------------------------

    public ISaveLoad SaveLoad
    {
        get { return saveLoad; }
        set { saveLoad = value; }
    }


    public EventMethodsGroupsCollection GroupsContainer
    {
        get { return groups; }
    }

    public bool IsInvokerClassUpdated { get; set; }

    //methods---------------------------------------------------------------------------

    public void Save()
    {
        SaveLoad.Save(groups);
        IsInvokerClassUpdated = false;
    }

    public void Load()
    {
        try
        {
            saveLoad.Load(ref groups);
        }
        catch (Exception)
        {
            groups = new EventMethodsGroupsCollection();
        }
    }

    public void UpdateOrCreateInvokerClass()
    {
        IsInvokerClassUpdated = true;

        implementer.Implement(groups);

        AssetDatabase.Refresh();

    }

    //----------------------------------------------------------------------------------
}


