using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Extentions;

[Serializable]
public class EventMethodsGroup:ISerializable, IAsReadOnly<EventMethodsGroup>,IEnumerable<ReadOnlyEventMethodInfo>, IXmlSerializable
{
    //fields----------------------------------------------------------------------------
    private string _groupName;
    private List<EventMethodInfo> _methods = new List<EventMethodInfo>();

    //constructors----------------------------------------------------------------------
    public EventMethodsGroup(string groupName, IEnumerable<EventMethodInfo> methods = null)
    {
        if (string.IsNullOrEmpty(groupName))
            throw new ArgumentNullException();

        if (!NameValidator.isNameValid(groupName))
            throw new ArgumentException("argument name is invalid!");

        _groupName = groupName;

        if(methods != null)
            _methods = methods.ToList().Select(x => x.Clone() as EventMethodInfo).ToList();

    }
    public EventMethodsGroup(SerializationInfo info, StreamingContext context)
    {
        _groupName = info.GetString("GroupName");
        _methods = (List<EventMethodInfo>)info.GetValue("Methods", typeof(List<EventMethodInfo>));
    }
    public EventMethodsGroup(EventMethodsGroup other)
    {
        _groupName = other.GroupName;
        _methods = other._methods.Select(x => x.Clone() as EventMethodInfo).ToList();
    }
    public EventMethodsGroup(XmlReader reader)
    {
        ReadXml(reader);
    }

    //properties------------------------------------------------------------------------
    public string GroupName
    {
        get { return _groupName; }
    }
    public ReadOnlyCollection<EventMethodInfo> Methods
    {
        get { return _methods.AsReadOnly(); }
    }

    //inherited/interface methods-------------------------------------------------------

    //ISerializable
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("GroupName", _groupName);
        info.AddValue("Methods", _methods);
    }
  
    //IXmlSerializable
    public XmlSchema GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
        reader.MoveToContent();
        _groupName = reader.GetAttribute("GroupName");

        int count = int.Parse(reader.GetAttribute("MethodsCount"));


        while (count > 0 && reader.Read())
            if (reader.Name == "Method" && reader.NodeType == XmlNodeType.Element)
            {
                _methods.Add(new EventMethodInfo(reader));
                if (--count <= 0) return;
            }

    }




    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement("MethodsGroup");
        writer.WriteAttributeString("GroupName", _groupName);
        writer.WriteAttributeString("MethodsCount", _methods.Count.ToString());

        _methods.ForEach(x=>x.WriteXml(writer));
        writer.WriteEndElement();

    }

    //ICloneable
    public object Clone()
    {
        return new EventMethodsGroup(this);
    }

    public List<EventMethodInfo> CloneMethods()
    {
        return _methods.Select(x=>x.Clone() as EventMethodInfo).ToList();
    }
    public EventMethodsGroup AsReadOnly
    {
        get { return new ReadOnlyEventMethodsGroup(this); }
    }
    public virtual bool IsReadOnly
    {
        get { return false; }
    }

    //methods---------------------------------------------------------------------------
    public void AddMethod(EventMethodInfo eventMethod)
    {
        if (IsReadOnly)
            throw new MemberAccessException("attribute is read-only (non-Modifiable)!");

        if (eventMethod == null)
            throw new ArgumentException("cannot add a null method");

        if(_methods.Contains(eventMethod))
            throw new ArgumentException("method with the same signature already exits");


        _methods.Add(eventMethod.Clone() as EventMethodInfo);

    }
    public bool RemoveMethod(EventMethodInfo eventMethod)
    {
        if (IsReadOnly)
            throw new MemberAccessException("attribute is read-only (non-Modifiable)!");

        return _methods.Remove(eventMethod);
    }

    public void RemoveMethod(string methodName)
    {
        if (IsReadOnly)
            throw new MemberAccessException("attribute is read-only (non-Modifiable)!");

        var temp = _methods.FindIndex(x => x.Name.Equals( methodName));
        if (temp == -1)
            throw new ArgumentException("method was not found");

        _methods.RemoveAt(temp);
    }
    public void RemoveMethods(IEnumerable<EventMethodInfo> methods)
    {
        if (IsReadOnly)
            throw new MemberAccessException("attribute is read-only (non-Modifiable)!");

        foreach (var method in methods)       
            RemoveMethod(method);
        
    }
    public void RemoveMethods(Predicate<EventMethodInfo> filter)
    {
        if (IsReadOnly)
            throw new MemberAccessException("attribute is read-only (non-Modifiable)!");

        _methods.RemoveAll(filter);
    }

    public void ReplaceMethod(EventMethodInfo oldMethod, EventMethodInfo newMethod)
    {
        if (IsReadOnly)
            throw new MemberAccessException("attribute is read-only (non-Modifiable)!");

        if(oldMethod == null|| newMethod == null)
            throw new ArgumentException("cannot accept null arguments");


        var index = _methods.FindIndex(oldMethod.Equals);
        if(index == -1)
            throw new ArgumentException("oldMethod was not found");


        if (index != -1)
            _methods[index] = newMethod.Clone() as EventMethodInfo; 
           
    }

    public void ClearMethods()
    {
        if (IsReadOnly)
            throw new MemberAccessException("attribute is read-only (non-Modifiable)!");

        _methods.Clear();
    }

    public void RenameGroup(string newName)
    {
        if (IsReadOnly)
            throw new MemberAccessException("attribute is read-only (non-Modifiable)!");

        if (string.IsNullOrEmpty(newName)|| !NameValidator.isNameValid(newName))
            throw new ArgumentException("name is invalid");

        if (NameValidator.isNameValid(newName))
            _groupName = newName;
    }
    public bool ContainsMethod(EventMethodInfo other)
    {
        return _methods.Exists(x=>x.Equals(other));
    }
    public bool ContainsMethodName(string otherMethodName)
    {
        return _methods.FindIndex(x=>x.Name.Equals(otherMethodName)) != -1;
    }

    public int IndexOfMethod(Predicate<EventMethodInfo> match)
    {
        return _methods.FindIndex(match);

    }

    public void SwapMethods(int firstInedx, int secondIndex)
    {
        if (IsReadOnly)
            throw new MemberAccessException("attribute is read-only (non-Modifiable)!");

        _methods.SwapCells(firstInedx, secondIndex);
    }

    public EventMethodInfo this[int index]
    {
        get
        {
            if(IsReadOnly)
                return _methods[index].AsReadOnly;
            return _methods[index];
        }
    }

    public int MethodsCount { get { return _methods.Count; } }
    //----------------------------------------------------------------------------------
    public IEnumerator<ReadOnlyEventMethodInfo> GetEnumerator()
    {
        for (int i = 0; i < _methods.Count; i++)
        {
            yield return _methods[i].AsReadOnly as ReadOnlyEventMethodInfo;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }


}

[Serializable]
public class ReadOnlyEventMethodsGroup : EventMethodsGroup {
    private bool _isReadOnly;

    public ReadOnlyEventMethodsGroup(string groupName, IEnumerable<EventMethodInfo> methods = null) : base(groupName, methods)
    {
    }

    public ReadOnlyEventMethodsGroup(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public ReadOnlyEventMethodsGroup(EventMethodsGroup other) : base(other)
    {
    }

    public ReadOnlyEventMethodsGroup(XmlReader reader):base( reader)
    {
        
    }

    public override bool IsReadOnly
    {
        get { return true; }
    }
}