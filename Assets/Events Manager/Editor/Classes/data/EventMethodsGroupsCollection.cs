using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Extentions;

[Serializable]
public class EventMethodsGroupsCollection: IEnumerable<ReadOnlyEventMethodsGroup>,ISerializable,IXmlSerializable
{
    public enum Reorder { MoveUp,MoveDown}
  
    //fields----------------------------------------------------------------------------
    List<EventMethodsGroup> groups;
    Action _onModifyListener;

    //constructors----------------------------------------------------------------------
    public EventMethodsGroupsCollection()
    {
        groups = new List<EventMethodsGroup>();
    }
    public EventMethodsGroupsCollection(SerializationInfo info, StreamingContext context)
    {
        groups = (List<EventMethodsGroup>) info.GetValue("Groups", typeof (List<EventMethodsGroup>));
    }

    ~EventMethodsGroupsCollection()
    {
        groups.Clear();
        groups = null;
    }


    //properties------------------------------------------------------------------------
    public List<string> GroupNames { get { return groups.Select(x=>x.GroupName).ToList(); } }

    //methods---------------------------------------------------------------------------
    public bool ContainsGroup(string groupName)
    {
        return groups.Exists(x => x.GroupName.Equals(groupName));
    }
    public bool IsEventMethodExists(string groupName, EventMethodInfo method)
    {
        if (!ContainsGroup(groupName))
            return false;
        return groups.Where(group => group.GroupName.Equals(groupName)).Any(group => group.ContainsMethod(method)); //PluginEvents.UsedEvents.Exists(x=>x.Name== name);
    }
    public bool IsNameValid(string naame)
    {
        return NameValidator.isNameValid(naame);
    }
    //------------------------------------
    public void AddGroup(string groupName)
    {
        if (!IsNameValid(groupName))
            throw new ArgumentException("group name is not valid");
        if (ContainsGroup(groupName))
            throw new ArgumentException("group name has already been used");

        groups.Add(new EventMethodsGroup(groupName));

        if(_onModifyListener != null)
            _onModifyListener();

    }
    public void RemoveGroup(string groupName)
    {
        if (!IsNameValid(groupName)|| !ContainsGroup(groupName))
            throw new ArgumentException("group was not found");


        var temp = groups.Find(x => x.GroupName.Equals(groupName));
        if (temp == null)
            throw new ArgumentException("group was not found");


        temp.ClearMethods();
        groups.Remove(temp);


        if (_onModifyListener != null)
            _onModifyListener();

    }
    public void RenameGroup(string groupName, string newGroupName)
    {
        var temp = groups.Find(x => x.GroupName.Equals(groupName));
        if (temp == null)
            throw new ArgumentException("group was not found");

        if (!IsNameValid(newGroupName))
            throw new ArgumentException("new group name is invalid!");


        if (ContainsGroup(newGroupName))
            throw new ArgumentException("new group name has already been used!");

        temp.RenameGroup(newGroupName);

        if (_onModifyListener != null)
            _onModifyListener();

    }
    //------------------------------------
    public void AddEvent(string groupName, EventMethodInfo method)
    {
        var temp = groups.Find(x => x.GroupName.Equals(groupName));
        if (temp == null)
            throw new ArgumentException("group was not found!");

        temp.AddMethod(method);


        if (_onModifyListener != null)
            _onModifyListener();

    }
    public void RemoveEvent(string groupName, EventMethodInfo method)
    {
        var group = groups.Find(x => x.GroupName.Equals(groupName));
        if (group == null)
            throw new ArgumentException("group was not found!");


        group.RemoveMethod(method);


        if (_onModifyListener != null)
            _onModifyListener();
    }
    public void RemoveEvents(string groupName, Predicate<EventMethodInfo> methodFilter)
    {
        var group = groups.Find(x => x.GroupName.Equals(groupName));
        if (group == null)
            throw new ArgumentException("group was not found!");


        group.RemoveMethods(methodFilter);


        if (_onModifyListener != null)
            _onModifyListener();
    }
    public EventMethodModifier GetEventModifier(string groupName, string eventName)
    {
        var group = groups.Find(x => x.GroupName.Equals(groupName));
        if (group == null)
            throw new ArgumentException("group was not found!");

        if(!group.ContainsMethodName(eventName))
            throw new ArgumentException("method was not found!");

        var method = group[group.IndexOfMethod(x => x.Name.Equals(eventName))];

        var returnVal = new EventMethodModifier(group, method);
        returnVal.AddListener_OnModify(_onModifyListener);
        return returnVal;
    }
    public void ChangeEventGroup(string groupName, string otherGroupName, EventMethodInfo method)
    {
        if (!ContainsGroup(groupName))
            throw new ArgumentException("group was not found!");

        if (!ContainsGroup(otherGroupName))
            throw new ArgumentException("other group was not found!");

        if (!IsEventMethodExists(groupName, method))
            throw new ArgumentException("method was not found in the group");

        if (IsEventMethodExists(otherGroupName, method))
            throw new ArgumentException("method with the same signature already exists in the other group");

        RemoveEvent(groupName, method);
        AddEvent(otherGroupName, method);

        if (_onModifyListener != null)
            _onModifyListener();
    }
    //------------------------------------
    public void SwapGroupsOrder(string groupName, Reorder direction)
    {
        var index = groups.FindIndex(x => x.GroupName.Equals(groupName));
        if (index == -1)
            throw new ArgumentException("group was not found!");

        switch (direction)
        {
            case Reorder.MoveUp:
                if(index == 0 )return;
                var temp = groups[index - 1];
                groups[index - 1] = groups[index];
                groups[index] = temp;
                break;

            case Reorder.MoveDown:
                if (index == groups.Count-1) return;
                var temp2 = groups[index + 1];
                groups[index + 1] = groups[index];
                groups[index] = temp2;
                break;

            default:
                throw new ArgumentOutOfRangeException("direction", direction, null);
        }
    }
    public void SwapGroupMethod(string groupName, EventMethodInfo method, Reorder direction)
    {
        var group = groups.Find(x => x.GroupName.Equals(groupName));
        if (group == null)
            throw new ArgumentException("group was not found!");

        if(!group.ContainsMethod(method))
            throw new ArgumentException("method was not found in the group!");

        var indexOfMethod = group.IndexOfMethod(x => x.Equals(method));

        var nextIndex = indexOfMethod;
        if (direction == Reorder.MoveUp)
            nextIndex--;
        else if (direction == Reorder.MoveDown)
            nextIndex++;

        if(group.Methods.IndexInRange(nextIndex))       
            group.SwapMethods(indexOfMethod, nextIndex);

    }

    public void SwapGroupMethod(string groupName, int firstIndex, int secondIndex)
    {
        var group = groups.Find(x => x.GroupName.Equals(groupName));
        if (group == null)
            throw new ArgumentException("group was not found!");

        if (group.MethodsCount <= firstIndex || firstIndex < 0)
            throw new ArgumentException("index out of range!");

        if (group.MethodsCount <= secondIndex || secondIndex < 0)
            throw new ArgumentException("index out of range!");

        if (firstIndex == secondIndex)
            return;

        group.SwapMethods(firstIndex, secondIndex);

    }

    public void SwapGroups(string group1, string group2)
    {
        var index1 = groups.FindIndex(x => x.GroupName.Equals(group1));
        var index2 = groups.FindIndex(x => x.GroupName.Equals(group2));
        if (index1 == -1 || index2 == -1)
            throw new ArgumentException("invalid input arguments!");

        var temp = groups[index1];
        groups[index1] = groups[index2];
        groups[index2] = temp;


    }
    //------------------------------------
    public ReadOnlyEventMethodsGroup this[string groupName]
    {
        get
        {
            var temp = groups.Find(x => x.GroupName.Equals(groupName));
            if (temp == null)
                throw new ArgumentException("group was not found!");

            return temp.AsReadOnly as ReadOnlyEventMethodsGroup;
        }
    }
    //------------------------------------
    public void AddListener_OnModify(Action effect)
    {
        if (effect == null)
            return;

        if (_onModifyListener == null)
            _onModifyListener = effect;
        else
            _onModifyListener += effect;
    }
    public void RemoveListener_OnModify(Action effect)
    {
        if (effect == null || _onModifyListener == null)
            return;


        try
        {
            _onModifyListener -= effect;
        }
        catch (Exception)
        {
            return;
        }
    }
    public void clearListeners_OnModify()
    {
        _onModifyListener = null;
    }
    
  
    //inherited/interface methods-------------------------------------------------------


    public IEnumerator<ReadOnlyEventMethodsGroup> GetEnumerator()
    {
        for (int i = 0; i < groups.Count; i++)
        {
            yield return groups[i].AsReadOnly as ReadOnlyEventMethodsGroup;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }


    //ISerializable
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("Groups", groups);

    }
    
    //IXmlSerializable
    public XmlSchema GetSchema()
    {
        return null;
    }
    public void ReadXml(XmlReader reader)
    {
        reader.MoveToContent();

        int count = int.Parse(reader.GetAttribute("GroupsCount"));


        while (count > 0 && reader.Read())
            if (reader.Name == "MethodsGroup" && reader.NodeType == XmlNodeType.Element)
            {
                groups.Add(new EventMethodsGroup(reader));
                if (--count <= 0) return;

            }


    }
    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement("GroupsContainer");
        writer.WriteAttributeString("GroupsCount", groups.Count.ToString());

        groups.ForEach(x => x.WriteXml(writer));
        writer.WriteEndElement();
    }
    //----------------------------------------------------------------------------------
}