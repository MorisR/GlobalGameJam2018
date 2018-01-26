using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NUnit.Framework;
using UnityEngine;


[Serializable][XmlRoot("ParameterInfo")]
public class ParameterInfo : ISerializable, ICloneable,IAsReadOnly<ParameterInfo>, IXmlSerializable
{

    //enums-----------------------------------------------------------------------------
    public enum PassingType
    {
        Normal,
        Out,
        Ref
    }

    //statics---------------------------------------------------------------------------
    static void SerializeType(SerializationInfo info, StreamingContext context, Type type, string name)
    {

        info.AddValue(string.Format("{0}_isGenericType", name), type.IsGenericType);

        if (!type.IsGenericType)
        {
            info.AddValue(name, type.FullName);
            return;
        }


        info.AddValue(name, type.GetGenericTypeDefinition().AssemblyQualifiedName);
        info.AddValue(string.Format("{0}_Count", name), type.GetGenericArguments().Length);
        var args = type.GetGenericArguments();

        for (int i = 0; i < args.Length; i++)
        {
            string argName = string.Format("{0}_arg{1}", name, i);
            SerializeType(info, context, args[i], argName);
        }

    }
    static void SerializeType(XmlWriter writer, Type type, string name)
    {

        writer.WriteAttributeString(string.Format("{0}_isGenericType", name), type.IsGenericType.ToString());

        if (!type.IsGenericType)
        {
            writer.WriteAttributeString(name, type.FullName);
            return;
        }


        writer.WriteAttributeString(name, type.GetGenericTypeDefinition().AssemblyQualifiedName);
        writer.WriteAttributeString(string.Format("{0}_Count", name), type.GetGenericArguments().Length.ToString());
        var args = type.GetGenericArguments();

        for (int i = 0; i < args.Length; i++)
        {
            string argName = string.Format("{0}_arg{1}", name, i);
            SerializeType(writer, args[i], argName);
        }

    }


    static Type DeserializeType(SerializationInfo info, StreamingContext context, string name)
    {

        var temp = info.GetString(name);
        //var currentTypeFullName = Type.GetType(temp) ;
        var currentTypeFullName = GetType(temp) ;
        var isGeneric = info.GetBoolean(string.Format("{0}_isGenericType", name));

        if (!isGeneric)
            return currentTypeFullName;


        var generictypeArgsCount = info.GetInt32(string.Format("{0}_Count", name));
        var args = new List<Type>();

        for (int i = 0; i < generictypeArgsCount; i++)
        {
            string argName = string.Format("{0}_arg{1}", name, i);
            args.Add(DeserializeType(info, context, argName));
        }

        return currentTypeFullName.MakeGenericType(args.ToArray());

    }
    static Type DeserializeType(XmlReader reader, string name)
    {
        
        var isGeneric = bool.Parse( reader.GetAttribute(string.Format("{0}_isGenericType", name)) );

        var temp = reader.GetAttribute(name);
        //var currentTypeFullName = Type.GetType(temp) ;
        var currentTypeFullName = GetType(temp);

        if (!isGeneric)
            return currentTypeFullName;


        var generictypeArgsCount = int.Parse(reader.GetAttribute(string.Format("{0}_Count", name)));
        var args = new List<Type>();

        for (int i = 0; i < generictypeArgsCount; i++)
        {
            string argName = string.Format("{0}_arg{1}", name, i);
            args.Add(DeserializeType(reader, argName));
        }

        return currentTypeFullName.MakeGenericType(args.ToArray());

    }

    public static Type GetType(string TypeName)
    {

        // Try Type.GetType() first. This will work with types defined
        // by the Mono runtime, etc.
        var type = Type.GetType(TypeName);

        // If it worked, then we're done here
        if (type != null)
            return type;

        // Get the name of the assembly (Assumption is that we are using
        // fully-qualified type names)
        string assemblyName = "";
        if (TypeName.Contains("."))
            assemblyName = TypeName.Substring(0, TypeName.IndexOf('.'));


        // Attempt to load the indicated Assembly
        Assembly assembly = null;
        if (!string.IsNullOrEmpty(assemblyName))
            assembly = Assembly.LoadWithPartialName(assemblyName);

        if (assembly == null)
        {
            var temp = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.GetType(TypeName) != null).ToList();
            assembly = temp.Count > 0 ? temp[0] : null;
        }

        if (assembly == null)
            throw new NullReferenceException("assembly was not found, therefor one of the argument types was not found.");

        // Ask that assembly to return the proper Type
        return assembly.GetType(TypeName);

    }
   
    //fields----------------------------------------------------------------------------
    private Type _argType;
    private string _argName;
    private PassingType _passingType;

    //constructors----------------------------------------------------------------------

    public ParameterInfo(Type argType, string argName, PassingType passingType)
    {

        if (argType == null || string.IsNullOrEmpty(argName))
            throw new ArgumentNullException();

        if (!NameValidator.isNameValid(argName))
            throw new ArgumentException("argument name is invalid!");

        _argType = argType;
        _argName = argName;
        _passingType = passingType;
    }

    public ParameterInfo(ParameterInfo other)
    {
        if (other == null)
            throw new ArgumentNullException();

        _argType = other.ArgType;
        _argName = other.ArgName;
        _passingType = other.ArgPassingType;
    }

    public ParameterInfo(SerializationInfo info, StreamingContext context)
    {
        _argName = info.GetString("ArgName");
        _passingType = (PassingType) info.GetValue("PassingType", typeof (PassingType));
        _argType = DeserializeType(info, context, "Type");
    }

    public ParameterInfo(XmlReader reader)
    {
        ReadXml(reader);
    }

    //properties------------------------------------------------------------------------
    public Type ArgType
    {
        get { return _argType; }
    }

    public string ArgName
    {
        get { return _argName; }
    }

    public PassingType ArgPassingType
    {
        get { return _passingType; }
    }
   
    //IAsReadOnly
    public virtual bool IsReadOnly { get { return false; } }
    public ParameterInfo AsReadOnly { get { return new ReadOnlyParameterInfo(this); } }

    //inherited/interface methods-------------------------------------------------------

    //ISerializable
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("ArgName", ArgName);
        info.AddValue("PassingType", ArgPassingType);

        SerializeType(info, context, ArgType, "Type");

    }

    //IXmlSerializable
    public XmlSchema GetSchema()
    {
        return null;
    }
    public void ReadXml(XmlReader reader)
    {
         reader.MoveToContent();
        _argName = reader.GetAttribute("ArgName");
        _passingType = (PassingType)Enum.Parse(typeof(PassingType), reader.GetAttribute("PassingType"));
        _argType = DeserializeType(reader, "Type");


    }
    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement("Parameter");
        writer.WriteAttributeString("ArgName", ArgName);
        writer.WriteAttributeString("PassingType", ArgPassingType.ToString());
        SerializeType(writer, ArgType, "Type");
        writer.WriteEndElement();

    }

    //ICloneable
    public object Clone()
    {
        return new ParameterInfo(this);
    }
 
    //Object
    protected bool Equals(ParameterInfo other)
    {
        return _argType == other._argType && string.Equals(_argName, other._argName) &&
               _passingType == other._passingType;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if(!(obj is ParameterInfo))
            return false;
        return Equals((ParameterInfo) obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = (_argType != null ? _argType.GetHashCode() : 0);
            hashCode = (hashCode*397) ^ (_argName != null ? _argName.GetHashCode() : 0);
            hashCode = (hashCode*397) ^ (int) _passingType;
            return hashCode;
        }
    }

    public override string ToString()
    {
        return string.Format("param: {0} {1}", _argType.Name, ArgName);
    }

    //methods---------------------------------------------------------------------------

    public bool ChangeType(Type type)
    {
        if(IsReadOnly)
            throw new MemberAccessException("attribute is read-only (non-Modifiable)!");

        if(type == null)
            return false;

        _argType = type;
        return true;

    }

    public bool ChangeArgName(string name)
    {
        if (IsReadOnly)
            throw new MemberAccessException("attribute is read-only (non-Modifiable)!");

        if (name == null)
            return false;

        _argName = name;
        return true;
    }

    public void ChangePassongType(PassingType passingType)
    {
        if (IsReadOnly)
            throw new MemberAccessException("attribute is read-only (non-Modifiable)!");

        _passingType =passingType;
    }

    //----------------------------------------------------------------------------------


}

[Serializable]
public class ReadOnlyParameterInfo : ParameterInfo
{
    private bool _isReadOnly;

    public ReadOnlyParameterInfo(Type argType, string argName, PassingType passingType) : base(argType, argName, passingType)
    {
    }

    public ReadOnlyParameterInfo(ParameterInfo other) : base(other)
    {
    }

    public ReadOnlyParameterInfo(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public override bool IsReadOnly
    {
        get { return true; }
    }
}
