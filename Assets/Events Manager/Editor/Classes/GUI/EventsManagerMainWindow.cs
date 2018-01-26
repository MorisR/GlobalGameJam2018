using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


class EventsManagerMainWindow : EditorWindow
{

    //--------------------------------------------------

    #region Statics

    // variables
    static EventsManagerMainWindow window;
    static readonly Vector2 minWindowSize = new Vector2(240, 300);
    static readonly Vector2 maxWindowSize = new Vector2(minWindowSize.x*3, minWindowSize.y*2);

    //props
    public static EventsManagerMainWindow Window
    {
        get
        {
            if (window == null)
            {
                window = EditorWindow.GetWindow<EventsManagerMainWindow>(true, "Event Plug-in Manager", true);

            }
            return window;
        }
    }
    public static bool IsWindowVisable
    {
        get { return window != null; }
    }



    //methods
    [MenuItem("Plugins/Events Manager new")]
    static void EventsManagerWindow()
    {
        Window.Focus();
    }
  


    [MenuItem("Plugins/Update Invoker Class")]
    static void UpdateInvokerClass()
    {
        EventsManager.Instance.UpdateOrCreateInvokerClass();
    }


   

    static float ButtonWidth = 60;


    #endregion

    //--------------------------------------------------

    #region variables

    string selectedGroupName;
    Vector2 scroller = Vector2.zero;
    ReorderableList groupsList;
    ReorderableList eventsList;
    GUIStyle titleStyle;

    public EventMethodsGroup SelectedGroup
    {
        get
        {
            if (!EventsManager.Instance.GroupsContainer.ContainsGroup(selectedGroupName))
                return null;
            return EventsManager.Instance.GroupsContainer[selectedGroupName];
        }
        set
        {
            if (value == null)
                selectedGroupName = null;
        }
    }

    #endregion

    //--------------------------------------------------

    #region Methods


    void OnEnable()
    {
        if (window == null) window = this;
        window.minSize = minWindowSize;
        window.maxSize = maxWindowSize;

        SelectedGroup = null;
        initializeOrderableLists();


        titleStyle = new GUIStyle();
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontSize = 16;
        // titleStyle.stretchHeight = true;
        // titleStyle.stretchWidth = true;
        titleStyle.normal.textColor = new Color(224/255f, 185 / 255f, 121 / 255f);
        titleStyle.richText = true; 

        DirectoryInfo di = new DirectoryInfo(Application.dataPath);
        var files = di.GetFiles("flame.jpg", SearchOption.AllDirectories);

        if (files.Length > 0)
        {
            WWW w = new WWW(@"file:///" + files[0].FullName.Replace("\\", "/"));
            var texture = w.texture;
            texture.wrapMode = TextureWrapMode.Repeat;
            // TextureScale.Bilinear(texture, (int) (position.width/20), (int)position.height*20);
            titleStyle.normal.background = texture; // new Texture2D(1024, 1024, TextureFormat.DXT1, false);

        }
    }

    void OnDisable()
    {
        var CreatGroupWin = GetWindow(typeof (CreateGroupWindow));
        CreatGroupWin.Close();

        var RenameGroupWin = GetWindow(typeof (RenameGroupWindow));
        RenameGroupWin.Close();

        var CreateEditEventWin = GetWindow(typeof (CreateEditEventWindow));
        CreateEditEventWin.Close();
        /*
            if (!EventPluginManager.IsInvokerClassReady)
                EventPluginManager.UpdateOrCreateInvokerClass();*/
    }

    void OnFocus()
    {
        EditorWindow.FocusWindowIfItsOpen(typeof (CreateGroupWindow));
        EditorWindow.FocusWindowIfItsOpen(typeof (RenameGroupWindow));
        EditorWindow.FocusWindowIfItsOpen(typeof (CreateEditEventWindow));

        if (focusedWindow != this)
        {

                if(!EventsManager.Instance.GroupsContainer.Equals(groupsList.list))
                    NotifyGroupDatasetChanged();
               
            

            if (SelectedGroup != null)
                NotifyEventsDatasetChanged();
        }
    }




    void OnGUI()
    { 
        //--------------------------------------------------------------------------------------
        //label
        GUILayout.Label(new GUIContent("<b><size=25>Event Manager</size></b>"), titleStyle, GUILayout.Height(40), GUILayout.ExpandWidth(true));
        GUILayout.Space(5);
        //--------------------------------------------------------------------------------------
        //scroller
        scroller = EditorGUILayout.BeginScrollView(scroller);

        //group list
        groupsList.DoLayoutList();


        //event list
        if (SelectedGroup != null)
            eventsList.DoLayoutList();

        //end scroller
        EditorGUILayout.EndScrollView();
        //--------------------------------------------------------------------------------------
        //button
            if (!EventsManager.Instance.IsInvokerClassUpdated)
                if (GUILayout.Button("Update Invoker Class", GUILayout.Height(50)))
                    EventsManager.Instance.UpdateOrCreateInvokerClass();
        //--------------------------------------------------------------------------------------


    }



    void initializeOrderableLists()
    {
        groupsList = new ReorderableList(EventsManager.Instance.GroupsContainer.GroupNames, typeof (string));

        groupsList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Groups");
        groupsList.drawElementCallback = (rect, index, active, focused) =>
        {
            Rect r = rect;
            r.width = rect.width - ButtonWidth;
            EditorGUI.LabelField(r, groupsList.list[index] as string);

             r.x += r.width;
                r.width = ButtonWidth;
                r.height = rect.height * .75f;
                if (GUI.Button(r, "Rename"))
                {
                    var win = GetWindow<RenameGroupWindow>(true, "Rename Group", true);
                    win.SetGroup((groupsList.list[index] as string));
                }
        };
        groupsList.onSelectCallback = list =>
        {
            selectedGroupName = list.list[list.index] as string;
            CreateEventOrderableList();
        };

        groupsList.onAddCallback = list =>
        {
            GetWindow<CreateGroupWindow>(true, "Create Group", true);
        };
        groupsList.onReorderCallback = list => UpdateGroupListOrder(list.list);


        groupsList.onRemoveCallback = list =>
        {
            SelectedGroup = null;
            EventsManager.Instance.GroupsContainer.RemoveGroup(list.list[list.index] as string);
            list.index = -1;
            NotifyGroupDatasetChanged();
            groupsList.ReleaseKeyboardFocus();
        };




    }

    void CreateEventOrderableList()
    {
        if (SelectedGroup == null)
            return;

        var methods = SelectedGroup.CloneMethods();
        eventsList = new ReorderableList(methods, typeof (EventMethodInfo));

        eventsList.elementHeight = 30;
        eventsList.drawHeaderCallback =
            (rect) => EditorGUI.LabelField(rect, string.Format("Events - {0}", SelectedGroup.GroupName));
        eventsList.drawElementCallback = (rect, index, active, focused) =>
        {
            var method = (eventsList.list[index] as EventMethodInfo);
            Rect r = rect;
            r.width = rect.width - ButtonWidth;
            EditorGUI.LabelField(r, new GUIContent(method.ToString(), method.ToString()));

            r.x += r.width;
            r.width = ButtonWidth;
            r.height = rect.height*.75f;
            if (GUI.Button(r, "Edit"))
            {
                var win = GetWindow<CreateEditEventWindow>(true, "Edit Event", true);
                win.Initialize(SelectedGroup.GroupName, method);
            }

        };

        eventsList.onReorderCallback = evList => UpdateEventsListOrder(evList.list);
        eventsList.onRemoveCallback = evList =>
        {
            EventsManager.Instance.GroupsContainer.RemoveEvent(SelectedGroup.GroupName,
                evList.list[evList.index] as EventMethodInfo);
            NotifyEventsDatasetChanged();

        };
        eventsList.onAddCallback =
            evList =>
            {
                var win = GetWindow<CreateEditEventWindow>(true, "Create Event", true);
                win.Initialize(SelectedGroup.GroupName, null);
            };

    }


    #endregion


    void NotifyGroupDatasetChanged()
    {
        groupsList.list = EventsManager.Instance.GroupsContainer.GroupNames;
        groupsList.ReleaseKeyboardFocus();

    }

    void NotifyEventsDatasetChanged()
    {
        eventsList.ReleaseKeyboardFocus();
        CreateEventOrderableList();

    }

    void UpdateGroupListOrder(IList newOrder)
    {
        var currentOrder = EventsManager.Instance.GroupsContainer.GroupNames;

        for (int i = 0; i < newOrder.Count; i++)
        {

            if (!currentOrder.Equals(newOrder[i] as string))
            {
                EventsManager.Instance.GroupsContainer.SwapGroups(currentOrder[i], newOrder[i] as string);
                currentOrder = EventsManager.Instance.GroupsContainer.GroupNames;
            }

        }

        NotifyGroupDatasetChanged();
        groupsList.ReleaseKeyboardFocus();

    }
    void UpdateEventsListOrder(IList newOrder)
    {

        for (int i = 0; i < newOrder.Count; i++)
        {
            var currentOrder = SelectedGroup.Methods;

            if (!currentOrder[i].Equals(newOrder[i]))
            {
                var index = currentOrder.IndexOf(newOrder[i] as EventMethodInfo);
                EventsManager.Instance.GroupsContainer.SwapGroupMethod(selectedGroupName, i,index);
            }

        }

        NotifyEventsDatasetChanged();
    }

    //--------------------------------------------------



}





