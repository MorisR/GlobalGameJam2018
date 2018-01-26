using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Extentions;

[Serializable]
public class EventMethodInfo : ISerializable,ICloneable,IAsReadOnly<EventMethodInfo>,IEnumerable<ReadOnlyParameterInfo>, IXmlSerializable
{


    //fields----------------------------------------------------------------------------
    private string _name;
    private readonly List<ParameterInfo> _args = new List<ParameterInfo>();
    //  private ReadOnlyEventMethodInfo _current;


    //constructors----------------------------------------------------------------------
    public EventMethodInfo(string name, IEnumerable<ParameterInfo> args)
        : this(name, args.ToArray())
    {
    }
    public EventMethodInfo(string name, params ParameterInfo[] args)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException();

        if (!NameValidator.isNameValid(name))
            throw new ArgumentException("argument name is invalid!");

        _name = name;

        if(args != null)
            _args = args.ToList().Select(x => x.Clone() as ParameterInfo).ToList();


    }
    public EventMethodInfo(EventMethodInfo other)
    {
        if(other == null)
            throw new ArgumentNullException();

        _name = other.Name;
        _args = other.Args.Select(x => x.Clone() as ParameterInfo).ToList();
    }
    public EventMethodInfo(SerializationInfo info, StreamingContext context)
    {
        _name = info.GetString("EventName");
        _args = (List<ParameterInfo>)info.GetValue("EventArgs", typeof(List<ParameterInfo>));
    }
    public EventMethodInfo(XmlReader reader)
    {
        ReadXml(reader);
    }

    ~EventMethodInfo()
    {
        _args.Clear();
        _name = null;
    }

    //properties------------------------------------------------------------------------
    public string Name
    {
        get { return _name; }
    }
    public ReadOnlyCollection<ParameterInfo> Args
    {
        get { return _args.Select(x => x.AsReadOnly).ToList().AsReadOnly(); }
    }
    public int ArgsCount
    {
        get
        {
            return _args.Count;
        }
    }

    //IAsReadOnly<EventMethodInfo>
    public  EventMethodInfo AsReadOnly
    {
        get { return new ReadOnlyEventMethodInfo(this); }
    }
    public virtual bool IsReadOnly
    {
        get { return false; }
    }


    //inherited/interface methods-------------------------------------------------------

    //ISerializable
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("EventName", Name);
        info.AddValue("EventArgs", _args);
    }
  
    //IXmlSerializable
    public XmlSchema GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {

        reader.MoveToContent();
        _name = reader.GetAttribute("MethodName");

        int count = int.Parse(reader.GetAttribute("ArgCount"));

        while (count >0 && reader.Read())
        
            if (reader.NodeType == XmlNodeType.Element && reader.Name == "Parameter")
            {
                _args.Add(new ParameterInfo(reader));
                if(--count <= 0) return;
            }
        



    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement("Method");
        writer.WriteAttributeString("MethodName", _name);
        writer.WriteAttributeString("ArgCount", _args.Count.ToString());

        for (int i = 0; i < _args.Count; i++)
        {
            _args[i].WriteXml(writer);
        }


        writer.WriteEndElement();

    }


    //ICloneable
    public object Clone()
    {
        return new EventMethodInfo(this);
    }
    public List<ParameterInfo> CloneParameters()
    {
        return _args.Select(x=>x.Clone() as ParameterInfo).ToList();
    }

    //object
    protected bool Equals(EventMethodInfo other)
    {
        if (_args.Count != other.Args.Count || !string.Equals(_name, other._name))
            return false;

        for (int i = 0; i < _args.Count; i++)
            if (!_args[i].Equals(other.Args[i]))
                return false;

        return true;
    }
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (!(obj is EventMethodInfo))
            return false;
        return Equals((EventMethodInfo)obj);
    }
    public override int GetHashCode()
    {
        unchecked
        {
            return ((_name != null ? _name.GetHashCode() : 0) * 397) ^ (_args != null ? _args.GetHashCode() : 0);
        }
    }
    public override string ToString()
    {
        var argsName = _args.Select(x => string.Format("{0} {1}", NameValidator.GetGenericTypeName(x.ArgType), x.ArgName)).ToArray();
        return string.Format("{0}({1})", _name, string.Join(" , ", argsName));
    }


    //IEnumerable<ReadOnlyParameterInfo>
    public IEnumerator<ReadOnlyParameterInfo> GetEnumerator()
    {
        for (int i = 0; i < _args.Count; i++)
        {
            yield return _args[i].AsReadOnly as ReadOnlyParameterInfo;
        }
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }


    //methods---------------------------------------------------------------------------
    public ParameterInfo this[int index]
    {
        get
        {
            if (IsReadOnly)
                return _args[index].AsReadOnly;
            
            return _args[index].Clone() as ParameterInfo;
        }
    }
    public virtual void RenameMethod(string newName)
    {
        if (IsReadOnly)
            throw new MemberAccessException("attribute is read-only (non-Modifiable)!");

        if (string.IsNullOrEmpty(newName))
            return;

        if (NameValidator.isNameValid(newName))        
            _name = newName;
        

    }
    public virtual void SwapArgs(int firstInedx, int secondIndex)
    {
        if (IsReadOnly)
            throw new MemberAccessException("attribute is read-only (non-Modifiable)!");

        _args.SwapCells(firstInedx,secondIndex);
    }
    public virtual void RemoveArg(ParameterInfo arg)
    {
        if (IsReadOnly)
            throw new MemberAccessException("attribute is read-only (non-Modifiable)!");

        if (arg == null)
            return;


        var index = _args.FindIndex(arg.Equals);
        if(index!= -1)
            _args.RemoveAt(index);
    }
    public virtual void AddArg(ParameterInfo arg)
    {
        if (IsReadOnly)
            throw new MemberAccessException("attribute is read-only (non-Modifiable)!");

        if (arg == null)
            return;

            _args.Add(arg);
    }

    public virtual string MethodFullSigneture
    {
        get
        {
            var argsName = _args.Select(x => string.Format("{0} {1}", NameValidator.GetGenericTypeFullName(x.ArgType), x.ArgName)).ToArray();
            return string.Format("void {0}({1});", _name, string.Join(" , ", argsName));
        }
    }
    //----------------------------------------------------------------------------------


}


[Serializable]
public class ReadOnlyEventMethodInfo: EventMethodInfo
{
    public ReadOnlyEventMethodInfo(string name, IEnumerable<ParameterInfo> args) : base(name, args)
    {
    }

    public ReadOnlyEventMethodInfo(string name, params ParameterInfo[] args) : base(name, args)
    {
    }

    public ReadOnlyEventMethodInfo(EventMethodInfo other) : base(other)
    {
    }

    public ReadOnlyEventMethodInfo(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public override bool IsReadOnly
    {
        get { return true; }
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
}

/*
public class EventMethodInfoProxy : ISerializable
{
    private EventMethodsGroup _containingGroup;
    private EventMethodInfo _method;

    public EventMethodInfoProxy(EventMethodsGroup containingGroup, EventMethodInfo method)
    {
        if (containingGroup == null || method == null)
            throw new ArgumentException("arguments cannot be null!");

        if (!containingGroup.ContainsMethod(method))
            throw new ArgumentException("the method must be contained in the group in order!");

        _containingGroup = containingGroup;
        _method = method;
    }

    public string Name
    {
        get { return _method.Name; }
    }
    public ReadOnlyCollection<ParameterInfo> Args
    {
        get
        {
            return _method.Args;
        }
    }


    //inherited/interface methods-------------------------------------------------------
    protected bool Equals(EventMethodInfoProxy other)
    {
        return Equals(_containingGroup, other._containingGroup) && Equals(_method, other._method);
    }
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((EventMethodInfoProxy)obj);
    }
    public override int GetHashCode()
    {
        unchecked
        {
            return ((_containingGroup != null ? _containingGroup.GetHashCode() : 0) * 397) ^ (_method != null ? _method.GetHashCode() : 0);
        }
    }
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        _method.GetObjectData(info, context);
    }


    //methods---------------------------------------------------------------------------
    public ParameterInfo this[int index]
    {
        get { return _method[index]; }
    }

    public void RenameMethod(string newName)
    {

        if (_method.Name.Equals(newName))
            return;

        var newMethod = _method.Clone() as EventMethodInfo;
        newMethod.RenameMethod(newName);

        if (!NameValidator.isNameValid(newName))
            throw new ArgumentException("method name is Invalid!");


        if (_containingGroup.ContainsMethod(newMethod) && !_method.Name.Equals(newName))
            throw new InvalidOperationException("method exists in group!");


        _containingGroup.ReplaceMethod(_method, newMethod);
    }
    public void SwapArgs(int firstInedx, int secondIndex)
    {
        var newMethod = _method.Clone() as EventMethodInfo;
        newMethod.SwapArgs(firstInedx, secondIndex);

        if (_containingGroup.ContainsMethod(newMethod) && !_method.Equals(newMethod))
            throw new InvalidOperationException("method exists in group!");

        else if (!_method.Equals(newMethod))
            _containingGroup.ReplaceMethod(_method, newMethod);

    }
    public void RemoveArg(ParameterInfo arg)
    {
        var newMethod = _method.Clone() as EventMethodInfo;
        newMethod.RemoveArg(arg);

        if (_containingGroup.ContainsMethod(newMethod) && !_method.Equals(newMethod))
            throw new InvalidOperationException("method exists in group!");

        else if (!_method.Equals(newMethod))
            _containingGroup.ReplaceMethod(_method, newMethod);

    }
    public void AddArg(ParameterInfo arg)
    {
        var newMethod = _method.Clone() as EventMethodInfo;
        newMethod.AddArg(arg);

        if (_containingGroup.ContainsMethod(newMethod) && !_method.Equals(newMethod))
            throw new InvalidOperationException("method exists in group!");

        else if (!_method.Equals(newMethod))
            _containingGroup.ReplaceMethod(_method, newMethod);
    }

    //----------------------------------------------------------------------------------


}
*/
