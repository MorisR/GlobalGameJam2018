using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;




public class CreateEditEventWindow : ScriptableWizard
{

    //----------------------------------------------

    #region variables

    //-----------------------------------
    const int WindowMinWidth = 550;
    const int WindowMinHeigth = 500;
    //-----------------------------------
    const float ArgsListWidth_number_presentage = .2f;
    const float ArgsListWidth_type_presentage = .40f;
    const float ArgsListWidth_name_presentage = .2f;
    //-----------------------------------
    //create event
    string eventName;
    List<ParameterInfo> args;
    //edit event
    string ContainingGroup;
    EventMethodInfo eventmethod;
    //-----------------------------------
    string[] groupNames;
    int selectedgroupNamePopupValue = 0;
    //-----------------------------------
    Vector2 ArgsScrollerPos;
    //-----------------------------------
    bool AddArgsButtonClicked;
    //-----------------------------------
    string[] argTypePopupValues = {"Non-Generic", "Generic"};
    //int selectedArgTypePopup;
    //-----------------------------------
    // List<string> ArgTypeNames, ArgNames;
    // List<string[]> ArgTypeSuggestions;
    // List<int> selectedArgTypeSuggestions;

    string argName;
    SortedDictionary<string, Suggestions> argSuggestionses;

    //-----------------------------------

    #endregion

    //----------------------------------------------

    #region props


    public List<ParameterInfo> Args
    {
        get
        {
            if (args == null) args = new List<ParameterInfo>();
            return args;
        }
        set { args = value; }
    }

    bool IsInEditMode
    {
        get { return  eventmethod != null; }
    }

    string[] GroupNames
    {
        get
        {
            if (groupNames == null)
                groupNames = EventsManager.Instance.GroupsContainer.GroupNames.ToArray();
            return groupNames;
        }
    }


    #endregion

    //----------------------------------------------
    private class Suggestions
    {
        public ArrayType arrType = ArrayType.None;
        public ParameterInfo.PassingType PassType = ParameterInfo.PassingType.Normal; // not used

        static readonly string[] emptyList = {""};
        string[] suggestionsArray;
        int selectedItem;
        IEnumerable<Type> _sugTypes;
        Type sugType;
        public string argTypeFilter = "";

        public int genericTypeArgsCount;
        public int selectedArgTypePopup;
        public Vector2 genericArgsScrollerPos;
        public bool isGenericOpen;

        public Suggestions()
        {
            Reset(true);
        }

        public Suggestions(IEnumerable<Type> sugTypes)
        {
            Reset(true);
            SetTypes(sugTypes);
        }

        public Suggestions(Type type)
        {
            Reset(true);
            SetTypes(type);
        }


        public void SetTypes(IEnumerable<Type> sugTypes)
        {
            sugType = null;
            _sugTypes = null;
            SuggestionsArray = emptyList;

            if (sugTypes != null)
            {
                _sugTypes = sugTypes;
                SuggestionsArray = sugTypes.Select(x => x.FullName).ToArray();
            }

        }

        public void SetTypes(Type type)
        {
            sugType = null;
            _sugTypes = null;
            SuggestionsArray = emptyList;

            if (type != null)
            {
                sugType = type;
                SuggestionsArray = new[] {type.FullName};
            }

        }

        public Type GetSelectedType()
        {
            if (SuggestionsArray == emptyList) return null;
            if (SuggestionsArray.Length == 0) return sugType;
            if (SelectedItem == -1) return null;
            if (sugType != null) return sugType;
            return _sugTypes.ElementAt(SelectedItem);
        }

        public int SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
            }
        }

        public string[] SuggestionsArray
        {
            get
            {
                if (suggestionsArray == null) suggestionsArray = emptyList;
                return suggestionsArray;
            }
            set
            {
                suggestionsArray = value;

                SelectedItem = -1;
/*                if (suggestionsArray != null)
                    if (suggestionsArray.Length == 1)
                    {
                        SelectedItem = 0;
                    }*/
                
            }
        }

        public void Reset(bool fullReset)
        {
            if (fullReset)
            {
                genericArgsScrollerPos = Vector2.zero;
                selectedArgTypePopup = 0;
                isGenericOpen = false;
                SuggestionsArray = emptyList;
                genericTypeArgsCount = -1;
                sugType = null;
                _sugTypes = null;
                arrType = ArrayType.None;
                //PassType = PassingType.Normal;
                argTypeFilter = "";
            }
            SelectedItem = -1;




        }
    }



//----------------------------------------------

    void OnEnable()
    {
        minSize = new Vector2(WindowMinWidth, WindowMinHeigth);


        AddArgsButtonClicked = false;



        argName = "";

        argSuggestionses = new SortedDictionary<string, Suggestions>();
        argSuggestionses.Add("base", new Suggestions());

        errorString = null;
    }

    void OnDisable()
    {
        if (ContainingGroup != null)
        {
            FocusWindowIfItsOpen(typeof(EventsManagerMainWindow));
        }
    }

    void OnGUI()
    {

        //----------------------------------------
        if (ContainingGroup == null) Close();
        //----------------------------------------
        DrawEventGroupField();
        //----------------------------------------
        DrawEventNameField();
        //----------------------------------------
        EditorGUILayout.Space();
        //----------------------------------------
        DrawAddArgsHeader();
        //----------------------------------------
        if (AddArgsButtonClicked && ContainingGroup != null) DrawAddArgsBox(true, "base");
        //----------------------------------------
        EditorGUILayout.Space();
        //----------------------------------------
        DrawEvetArgsHeader();
        //----------------------------------------
        DrawEvetArgsList();
        //----------------------------------------
        DrawBottomButtons();
        //----------------------------------------

    }

    //----------------------------------------------
    void DrawEventGroupField()
    {
        if (IsInEditMode)
        {
            selectedgroupNamePopupValue = EditorGUILayout.Popup("Group", selectedgroupNamePopupValue, GroupNames);

        }
        else
        {
            EditorGUILayout.LabelField("Group", ContainingGroup);
        }

    }

    void DrawEventNameField()
    {
        //draw event name text field
        GUILayout.BeginHorizontal();
        eventName = EditorGUILayout.TextField("Event Name", eventName);
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        //check if event name is valid
        string errorMessageTemp;
        EventNameIsValid(out errorMessageTemp);
       // errorString = errorMessageTemp;

        //show error if event name is invalid
        if (!string.IsNullOrEmpty(errorMessageTemp))
        {
            var tempColor = GUI.color;
            GUI.color = Color.red;
            GUILayout.Label(errorMessageTemp);
            GUI.color = tempColor;
        }

        GUILayout.EndHorizontal();
    }
    bool EventNameIsValid(out string errorMessage)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            errorMessage = "";
            return false;
        }
        if (!NameValidator.isNameValid(eventName))
        {
            errorMessage = "Event name is invalid!";
            return false;
        }




        errorMessage = "";
        return true;
    }

    void DrawAddArgsHeader()
    {

        GUI.enabled = !AddArgsButtonClicked;

        GUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Args:");
        if (GUILayout.Button("Add"))
        {
            AddArgsButtonClicked = true;
        }

        GUILayout.EndHorizontal();

        GUI.enabled = true;
    }

    void DrawAddArgsBox(bool ShowIsGenericSuggestion, string Key)
    {
        Suggestions sug = argSuggestionses[Key];
        Suggestions argSuggestionsesVal = argSuggestionses[Key];

        //-----------------------------------------------------------------------------------------------
        EditorGUILayout.BeginVertical(GUI.skin.FindStyle("Box"));
        //-----------------------------------------------------------------------------------------------
        if (ShowIsGenericSuggestion)
        {
            GUILayout.BeginHorizontal();
            var tmpTypePopup = EditorGUILayout.Popup(argSuggestionsesVal.selectedArgTypePopup, argTypePopupValues);
            if (tmpTypePopup != argSuggestionsesVal.selectedArgTypePopup)
            {
                resetAddArgFields(Key);
                sug.argTypeFilter = "";
                argSuggestionsesVal.selectedArgTypePopup = tmpTypePopup;

            }
            GUILayout.EndHorizontal();
        }
        else
        {
            argSuggestionsesVal.selectedArgTypePopup = 0;
        }
        //-----------------------------------------------------------------------------------------------
        if (Key == "base")
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Arg Name", GUILayout.Width(70));
            argName = GUILayout.TextField(argName);
            GUILayout.EndHorizontal();

            //write an error message if the parameter is invalid
            GUILayout.BeginHorizontal();
            if (!string.IsNullOrEmpty(argName) && !NameValidator.isNameValid(argName))
            {
                var tempc = GUI.color;
                GUI.color = Color.red;
                EditorGUILayout.LabelField("", "Arg name is InValid!");
                GUI.color = tempc;
            }
            else if (Args.Exists(x => x.ArgName.Equals(argName)))
            {
                var tempc = GUI.color;
                GUI.color = Color.red;
                EditorGUILayout.LabelField("", "Arg name has already been used!");
                GUI.color = tempc;
            }
            GUILayout.EndHorizontal();

        }


        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Types popup Filter", GUILayout.Width(110));
        var tempArgTypeNames = GUILayout.TextField(sug.argTypeFilter);

        if (sug.argTypeFilter != tempArgTypeNames ||
            (sug.argTypeFilter == tempArgTypeNames && string.IsNullOrEmpty(tempArgTypeNames)))
        {
            if (sug.argTypeFilter != tempArgTypeNames
                || (sug.argTypeFilter == tempArgTypeNames
                    && string.IsNullOrEmpty(tempArgTypeNames)
                    &&
                    argSuggestionsesVal.SuggestionsArray.Length !=
                    ((argSuggestionsesVal.selectedArgTypePopup == 0)
                        ? AssemblyTypes.NonGenericTypes
                        : AssemblyTypes.GenericTypes).Count))
            {

                FilterTypeSuggestrions(
                    (argSuggestionsesVal.selectedArgTypePopup == 0)
                        ? AssemblyTypes.NonGenericTypes
                        : AssemblyTypes.GenericTypes,
                    tempArgTypeNames,
                    argSuggestionsesVal);

            }


            if (sug.argTypeFilter != tempArgTypeNames)
                if (argSuggestionsesVal.genericTypeArgsCount > 0)
                    SetGenericOprions(ref argSuggestionsesVal.genericTypeArgsCount,
                        argSuggestionsesVal.GetSelectedType(), AssemblyTypes.NonGenericTypes, Key);

            sug.argTypeFilter = tempArgTypeNames;

        }
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Arg type", GUILayout.Width(70));
        var tempSelectedArgTypeSuggestions = EditorGUILayout.Popup(argSuggestionsesVal.SelectedItem,
            argSuggestionsesVal.SuggestionsArray);
        if (argSuggestionsesVal.SelectedItem != tempSelectedArgTypeSuggestions)
        {
            argSuggestionsesVal.SelectedItem = tempSelectedArgTypeSuggestions;

            SetGenericOprions(ref argSuggestionsesVal.genericTypeArgsCount, argSuggestionsesVal.GetSelectedType(),
                AssemblyTypes.NonGenericTypes, Key);

        }
        //  else if (argSuggestionsesVal.SelectedItem == 1 && ) { }
        GUILayout.EndHorizontal();

        /*
            if (Key == "base")
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Passing Type", GUILayout.Width(90));
                argSuggestionses["base"].PassType = (PassingType)EditorGUILayout.EnumPopup(argSuggestionses["base"].PassType);
                GUILayout.EndHorizontal();
            }
            */

        if (argSuggestionsesVal.selectedArgTypePopup == 0)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Array type", GUILayout.Width(80));
            argSuggestionses[Key].arrType = (ArrayType) EditorGUILayout.EnumPopup(argSuggestionses[Key].arrType, GUILayout.Width(position.width/2 - 100));

            GUI.enabled = false;
            EditorGUILayout.LabelField("passing type", GUILayout.Width(80));
            argSuggestionses[Key].PassType = (ParameterInfo.PassingType) EditorGUILayout.EnumPopup(argSuggestionses[Key].PassType, GUILayout.Width(position.width/2 - 100));
            GUI.enabled = true;

            GUILayout.EndHorizontal();
        }

        //-----------------------------------------------------------------------------------------------
        if (argSuggestionsesVal.selectedArgTypePopup == 1)
        {

            if (argSuggestionsesVal.genericTypeArgsCount >= 1)
                argSuggestionsesVal.genericArgsScrollerPos =
                    GUILayout.BeginScrollView(argSuggestionsesVal.genericArgsScrollerPos,
                        GUILayout.MinHeight(Key == "base" ? position.height/2.2f : 150), GUILayout.ExpandHeight(true));

            for (int i = 1; i <= argSuggestionsesVal.genericTypeArgsCount; i++)
            {
                var sugName = string.Format("{0}_sub{1}", Key, i - 1);
                var sugVal = argSuggestionses[sugName];

                sugVal.isGenericOpen = EditorGUILayout.Foldout(sugVal.isGenericOpen, string.Format("arg {0}", i));
                if (sugVal.isGenericOpen)
                {
                    DrawAddArgsBox(true, sugName);
                }
            }

            if (argSuggestionsesVal.genericTypeArgsCount >= 1)
                GUILayout.EndScrollView();

        }
        //-----------------------------------------------------------------------------------------------
        if (Key == "base")
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("");

            //---------------------------------------
            if (string.IsNullOrEmpty(argName) || !NameValidator.isNameValid(argName) ||
                Args.Exists(x => x.ArgName.Equals(argName)) || argSuggestionses["base"].SelectedItem < 0)
                GUI.enabled = false;
            else if (argSuggestionses["base"].selectedArgTypePopup == 1)
            {
                GUI.enabled = IsGenericTypeLegal("base");
            }

            //---------------------------------------

            if (GUILayout.Button("OK", GUILayout.Width(80)))
            {
                if (argSuggestionses["base"].selectedArgTypePopup == 0) AddNonGenericArg();
                else AddGenericArg();
            }
            GUI.enabled = !string.IsNullOrEmpty(ContainingGroup);


            if (GUILayout.Button("Cancel", GUILayout.Width(60)))
            {
                AddArgsButtonClicked = false;
                resetAddArgFields("base");
            }
            GUILayout.EndHorizontal();
        }
        //-----------------------------------------------------------------------------------------------
        EditorGUILayout.EndVertical();
        //-----------------------------------------------------------------------------------------------

        argSuggestionses[Key] = argSuggestionsesVal;

    } //****

    void DrawEvetArgsHeader()
    {
        var tempc2 = GUI.backgroundColor;
        GUI.backgroundColor =new Color(tempc2.r/2, tempc2.g/2, tempc2.b/2);
        GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
        EditorGUILayout.LabelField("Pos",
            GUILayout.Width(ArgsListWidth_number_presentage*position.width +
                            GUI.skin.verticalScrollbarUpButton.fixedWidth +
                            GUI.skin.verticalScrollbarDownButton.fixedWidth));
        EditorGUILayout.LabelField("Type", GUILayout.Width(ArgsListWidth_type_presentage*position.width));
        EditorGUILayout.LabelField("Name", GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();
        GUI.backgroundColor = tempc2;
    }

    void DrawEvetArgsList()
    {

        ArgsScrollerPos = GUILayout.BeginScrollView(ArgsScrollerPos, false, true);

        if (!string.IsNullOrEmpty(ContainingGroup))
            for (int i = 0; i < Args.Count; i++)
            {
                GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));


                //move arg up button
                if (i == 0) GUI.enabled = false;
                if (GUILayout.Button("", GUI.skin.verticalScrollbarUpButton))
                {
                    var temparg = Args[i - 1];
                    Args[i - 1] = Args[i];
                    Args[i] = temparg;
                }
                if (i == 0) GUI.enabled = true;

                //move arg down button
                if (i == Args.Count - 1) GUI.enabled = false;
                if (GUILayout.Button("", GUI.skin.verticalScrollbarDownButton))
                {
                    var temparg = Args[i + 1];
                    Args[i + 1] = Args[i];
                    Args[i] = temparg;
                }
                if (i == Args.Count - 1) GUI.enabled = true;


                //write arg number
                EditorGUILayout.LabelField(i.ToString(), GUILayout.Width(ArgsListWidth_number_presentage*position.width));


                //write the arg type
                var argTypeString = NameValidator.GetGenericTypeName(Args[i].ArgType);
                EditorGUILayout.LabelField(
                    new GUIContent(argTypeString, argTypeString),
                    GUILayout.Width(ArgsListWidth_type_presentage*position.width));

                //write the arg name
                EditorGUILayout.LabelField(new GUIContent(Args[i].ArgName, Args[i].ArgName),
                    GUILayout.Width(ArgsListWidth_name_presentage*position.width));

                //remove arg button
                if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
                {
                    Args.RemoveAt(i);
                }


                GUILayout.EndHorizontal();
            }

        EditorGUILayout.EndScrollView();

    }

    void DrawBottomButtons()
    {
        GUILayout.BeginHorizontal();


        if (GUILayout.Button(IsInEditMode ? "Edit" : "Create"))
        {
            try
            {
                if (IsInEditMode)
                {
                    var methodToAdd = new EventMethodInfo(eventName, args);
                    if(EventsManager.Instance.GroupsContainer[GroupNames[selectedgroupNamePopupValue]].ContainsMethod(methodToAdd))
                        throw new ArgumentException("another event method with the same signature already exists.");
                    EventsManager.Instance.GroupsContainer.RemoveEvent(ContainingGroup, eventmethod);
                    EventsManager.Instance.GroupsContainer.AddEvent(GroupNames[selectedgroupNamePopupValue], methodToAdd);
                }
                else
                    EventsManager.Instance.GroupsContainer.AddEvent(ContainingGroup, new EventMethodInfo(eventName, args));

                Close();
            }
            catch (Exception e)
            {
                errorString = e.Message;
            }

        }


        if (GUILayout.Button("Close"))
        {
            Close();
        }

        GUILayout.EndHorizontal();

        if (!string.IsNullOrEmpty(errorString))
        {
            GUILayout.BeginHorizontal();
            var tempColor = GUI.color;
            GUI.color = Color.red;
            GUILayout.Label(errorString);
            GUI.color = tempColor;
            GUILayout.EndHorizontal();
        }
    } 

    //----------------------------------------------
    public void Initialize(string containingGroupName, EventMethodInfo eventMethod)
    {

        if (EventsManager.Instance.GroupsContainer.ContainsGroup(containingGroupName))
        {
            ContainingGroup = containingGroupName;
            selectedgroupNamePopupValue = GroupNames.ToList().IndexOf(containingGroupName);


            if (EventsManager.Instance.GroupsContainer[containingGroupName].ContainsMethod(eventMethod))
            {
                this.eventmethod = eventMethod;
                eventName = eventMethod.Name;
                Args = eventMethod.Args.ToList();
            }

        }
        else { Close();}

    }

    //----------------------------------------------

    bool IsGenericTypeLegal(string key)
    {
        var sug = argSuggestionses[key];

        if (sug.genericTypeArgsCount <= 0)
            return sug.SelectedItem >= 0;

        bool returnVal = true;
        for (int i = 0; i < sug.genericTypeArgsCount; i++)
        {
            var sugName = string.Format("{0}_sub{1}", key, i);
            returnVal = returnVal && IsGenericTypeLegal(sugName);
            if (returnVal == false) return false;
        }

        return returnVal;
    }

    //----------------------------------------------

    void AddGenericArg()
    {


        //Args.Add(new PluginEventArg(GetGenericFullType("base"), argName /*, argSuggestionses["base"].PassType*/));
        Args.Add(new ParameterInfo(GetGenericFullType("base"), argName, ParameterInfo.PassingType.Normal));
        AddArgsButtonClicked = false;
        resetAddArgFields("base");


    }

    void AddNonGenericArg()
    {

        Type t = argSuggestionses["base"].GetSelectedType();

        switch (argSuggestionses["base"].arrType)
        {
            case ArrayType._1D:
                t = t.MakeArrayType();
                break;

            case ArrayType._2D:
                t = t.MakeArrayType(2);
                break;

            case ArrayType._3D:
                t = t.MakeArrayType(3);
                break;

            case ArrayType._2D_Jagged:
                t = t.MakeArrayType().MakeArrayType();
                break;

        }

        Args.Add(new ParameterInfo(t, argName, ParameterInfo.PassingType.Normal));
        //Args.Add(new PluginEventArg(t, argName /*, argSuggestionses["base"].PassType*/));
        AddArgsButtonClicked = false;
        resetAddArgFields("base");
    }

    void resetAddArgFields(string key)
    {
        var tempArgSuggestionses = argSuggestionses.Where(X => X.Key.Contains(key)).Select(x => x.Key);
        foreach (var sug in tempArgSuggestionses)
            argSuggestionses[sug].Reset(key == "base");



        if (key == "base")
            argName = "";
    }

    //----------------------------------------------
    Type GetGenericFullType(string key)
    {

        var genericType = argSuggestionses[key].GetSelectedType();
        var temp = genericType.MakeGenericType(GetGenericArgTypes(key).ToArray());
        switch (argSuggestionses[key].arrType)
        {
            case ArrayType._1D:
                temp = temp.MakeArrayType();
                break;

            case ArrayType._2D:
                temp = temp.MakeArrayType(2);
                break;

            case ArrayType._3D:
                temp = temp.MakeArrayType(3);
                break;

            case ArrayType._2D_Jagged:
                temp = temp.MakeArrayType().MakeArrayType();
                break;

        }


        return temp;
    }

    List<Type> GetGenericArgTypes(string key)
    {
        var sug = argSuggestionses[key];
        if (sug.genericTypeArgsCount <= 0)
        {
            Type t = sug.GetSelectedType();
            switch (sug.arrType)
            {
                case ArrayType._1D:
                    t = t.MakeArrayType();
                    break;

                case ArrayType._2D:
                    t = t.MakeArrayType(2);
                    break;

                case ArrayType._3D:
                    t = t.MakeArrayType(3);
                    break;

                case ArrayType._2D_Jagged:
                    t = t.MakeArrayType().MakeArrayType();
                    break;

            }
            return new List<Type>() {t};
        }

        List<Type> templst = new List<Type>();
        for (int i = 0; i < sug.genericTypeArgsCount; i++)
        {
            var sugName = string.Format("{0}_sub{1}", key, i);
            if (argSuggestionses[sugName].genericTypeArgsCount > 0)
                templst.Add(GetGenericFullType(sugName));

            else templst.AddRange(GetGenericArgTypes(sugName));
        }

        return templst;
    }

    //----------------------------------------------
    void FilterTypeSuggestrions(IEnumerable<Type> types, string nameFilter, Suggestions sug)
    {
        nameFilter = nameFilter.ToLower();
        if (sug == null) sug = new Suggestions();

        switch (nameFilter)
        {
            case "float":
                if (types.Contains(typeof (float)))
                    sug.SetTypes(typeof (float));
                break;
            case "double":
                if (types.Contains(typeof (double)))
                    sug.SetTypes(typeof (double));
                break;
            case "int":
                if (types.Contains(typeof (int)))
                    sug.SetTypes(typeof (int));
                break;
            case "uint":
                if (types.Contains(typeof (uint)))
                    sug.SetTypes(typeof (uint));
                break;
            case "byte":
                if (types.Contains(typeof (byte)))
                    sug.SetTypes(typeof (byte));
                break;
            case "long":
                if (types.Contains(typeof (long)))
                    sug.SetTypes(typeof (long));
                break;
            case "ulong":
                if (types.Contains(typeof (ulong)))
                    sug.SetTypes(typeof (ulong));
                break;
            case "string":
                if (types.Contains(typeof (string)))
                    sug.SetTypes(typeof (string));
                break;
            case "char":
                if (types.Contains(typeof (char)))
                    sug.SetTypes(typeof (char));
                break;
            case "bool":
                if (types.Contains(typeof (bool)))
                    sug.SetTypes(typeof (bool));
                break;
            case "":
                sug.SetTypes(types);
                break;

            default:
                if (nameFilter[0] == '~') nameFilter = nameFilter.Substring(1);
                var temp = types.Where(x => x.FullName.ToLower().Contains(nameFilter));
                if (nameFilter == "float")
                {
                    nameFilter = "single";
                    temp.Concat(types.Where(x => x.FullName.ToLower().Contains(nameFilter)));
                }

                sug.SetTypes(temp);
                break;
        }
    }

    void SetGenericOprions(ref int argscCount, Type selectedGenericType, IEnumerable<Type> filteredAssembly, string Key)
    {
        if (selectedGenericType == null) return;
        if (!selectedGenericType.IsGenericType) return;


        var args = selectedGenericType.GetGenericArguments();

        argscCount = args.Length;
        for (int i = 0; i < args.Length; i++)
        {
            //------------------------------------------------------------------------------------------------------------------------
            string SubName = string.Format("{0}_sub{1}", Key, i);
            if (!argSuggestionses.ContainsKey(SubName)) argSuggestionses.Add(SubName, new Suggestions());


            //------------------------------------------------------------------------------------------------------------------------
            var argsParams = args[i].GetGenericParameterConstraints();
            var argsParamsBaseClass = argsParams.Where(x => x.IsClass || x.IsValueType).ToList();
            var argsParamsInterfaces = argsParams.Where(x => x.IsInterface).ToList();
            //------------------------------------------------------------------------------------------------------------------------

            var filtered = filteredAssembly;
            //8888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888

            if (argsParamsBaseClass.Count() != 0)
                filtered = filtered.Where(x => x.IsSubclassOf(argsParams[0]) || x == argsParams[0]).ToList();

            if (argsParamsInterfaces.Count() != 0)
                filtered =
                    filtered.Where(x => argsParamsInterfaces.All(intf => x.GetInterfaces().Contains(intf))).ToList();

            GenericParameterAttributes gpa = args[i].GenericParameterAttributes;

            if ((gpa & GenericParameterAttributes.ReferenceTypeConstraint) != 0)
                filtered = filtered.Where(x => x.IsClass).ToList();

            if ((gpa & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0)
                filtered = filtered.Where(x => x.IsValueType).ToList();

            if ((gpa & GenericParameterAttributes.DefaultConstructorConstraint) != 0)
                filtered =
                    filtered.Where(
                        x =>
                            x.GetConstructors()
                                .All(c => c.GetParameters().Length == 0 || c.GetParameters().All(p => p.IsOptional)))
                        .ToList();
            //------------------------------------------------------------------------------------------------------------------------
            argSuggestionses[SubName].SetTypes(filtered);
            //------------------------------------------------------------------------------------------------------------------------
        }
    }

    //----------------------------------------------
}


public enum ArrayType
{
    None,
    _1D,
    _2D,
    _3D,
    _2D_Jagged,

}
public static class ArrayTypeMethods
{
    public static string ArrayTypeAsString(this ArrayType arrType)
    {
        switch (arrType)
        {
            case ArrayType.None:
                return string.Empty;
            case ArrayType._1D:
                return "[]";
            case ArrayType._2D:
                return "[,]";
            case ArrayType._3D:
                return "[,,]";
            case ArrayType._2D_Jagged:
                return "[][]";
            default:
                throw new ArgumentOutOfRangeException("arrType", arrType, null);
        }
    }
}