using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Security;
using UnityEditor;
using UnityEngine;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;

public interface ISaveLoad
{
    ISaveLoad Save<T>(T data) where T : class;
    ISaveLoad Load<T>(ref T date) where T : class;
    ISaveLoad Load(ref EventMethodsGroupsCollection date);
}


public abstract class BaseSaveLload : ISaveLoad
{
    //fields----------------------------------------------------------------------------
    private string _path;
    private string _fileName;
    private string _fileType;

    //constructors----------------------------------------------------------------------
    public BaseSaveLload(string path,string fileName, string fileType)
    {
        FileType = fileType;
        Path = path;
        FileName = fileName;
    }

    //properties------------------------------------------------------------------------
    public string Path
    {
        get { return _path; }
        set { _path = value; }
    }
    public string FileName
    {
        get { return _fileName; }
        set { _fileName = value; }
    }
    public string FileType
    {
        get { return _fileType; }
        set { _fileType = value; }
    }
    public string FullPath { get { return string.Format("{0}/{1}{2}", Path, FileName??"", FileType??""); } }

    //inherited/interface methods-------------------------------------------------------
    public abstract ISaveLoad Save<T>(T data1) where T : class;

    public abstract ISaveLoad Load<T>(ref T data) where T : class;
    public abstract ISaveLoad Load(ref EventMethodsGroupsCollection date);

    protected static void CheckIfPathIsValid(string path)
    {
        try
        {
            if (string.IsNullOrEmpty( System.IO.Path.GetDirectoryName(path)) || string.IsNullOrEmpty(System.IO.Path.GetFileName(path)))
                return;
                
        }
        catch (Exception e)
        {
            throw new ArgumentException("file path/name is invalid ( may contain forbidden letters!!!!");
        }


    }
    //----------------------------------------------------------------------------------

}

public class SerializationSaveLoad : BaseSaveLload
{
    //fields----------------------------------------------------------------------------

    readonly IFormatter _formatter = new BinaryFormatter();

    //constructors----------------------------------------------------------------------
    public SerializationSaveLoad(string path, string fileName, string fileType)
        :base(path,fileName,fileType)
    {

    }

    //properties------------------------------------------------------------------------

    //inherited/interface methods-------------------------------------------------------
    public override ISaveLoad Save<T>(T data)
    {
        if (data == null)
            return this;

        CheckIfPathIsValid(FullPath);


        using (Stream stream = new FileStream(FullPath, FileMode.OpenOrCreate, FileAccess.Write))
        {

            try
            {
                _formatter.Serialize(stream, data);
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("error", e.Message, "ok");
                Debug.LogWarning(e.Message);
                throw e;
            }
            /* catch (ArgumentNullException e)
             {
                 EditorUtility.DisplayDialog("error", e.Message, "ok");
                 Debug.LogError(e.Message);
             }
             catch (SerializationException e)
             {
                 EditorUtility.DisplayDialog("error", e.Message, "ok");
                 Debug.LogError(e.Message);
             }
             catch (SecurityException e)
             {
                 EditorUtility.DisplayDialog("error", e.Message, "ok");
                 Debug.LogError(e.Message);
             }*/

            stream.Flush();
        }
        AssetDatabase.Refresh();
        return this;
    }

    public override ISaveLoad Load<T>(ref T data)
    {

        CheckIfPathIsValid(FullPath);


        try
        {
            using (Stream stream = new FileStream(FullPath, FileMode.Open, FileAccess.Read))
            {
                data = (T) _formatter.Deserialize(stream);
                return this;

            }
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("error", e.Message, "ok");
            Debug.LogWarning(e.Message);
            throw e;
        }
        /*
                catch (FileNotFoundException e)
                {
                    EditorUtility.DisplayDialog("error", e.Message, "ok");
                    Debug.LogWarning(e.Message);
                }
                catch (ArgumentNullException e)
                {
                    EditorUtility.DisplayDialog("error", e.Message, "ok");
                    Debug.LogWarning(e.Message);
                }
                catch (SerializationException e)
                {
                    EditorUtility.DisplayDialog("error", e.Message, "ok");
                    Debug.LogWarning(e.Message);
                }
                catch (SecurityException e)
                {
                    EditorUtility.DisplayDialog("error", e.Message, "ok");
                    Debug.LogWarning(e.Message);
                }
        */


        data = default(T);
        return this;

    }

    public override ISaveLoad Load(ref EventMethodsGroupsCollection date)
    {
        return Load<EventMethodsGroupsCollection> (ref date);
    }

    //----------------------------------------------------------------------------------

}
public class IXmlSerializableSaveLoad : BaseSaveLload
{
    //fields----------------------------------------------------------------------------


    //constructors----------------------------------------------------------------------
    public IXmlSerializableSaveLoad(string path, string fileName, string fileType)
        : base(path, fileName, fileType)
    {

    }

    //properties------------------------------------------------------------------------

    //inherited/interface methods-------------------------------------------------------
    public override ISaveLoad Save<T>(T data)
    {
        if (data == null || !(data is IXmlSerializable))
            throw new ArgumentException();

        CheckIfPathIsValid(FullPath);


        using (Stream stream = new FileStream(FullPath, FileMode.OpenOrCreate, FileAccess.Write))
        {

            try
            {
                using (XmlWriter writer = XmlWriter.Create(stream))
                {
                    (data as IXmlSerializable).WriteXml(writer);
                    writer.Flush();
                }
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("error", e.Message, "ok");
                Debug.LogWarning(e.Message);
                throw e;
            }
            

            stream.Flush();
        }
        AssetDatabase.Refresh();
        return this;
    }

    //buggy non functional - if the data value is null, there is no way to identify the type of 'T'
    public override ISaveLoad Load<T>(ref T data)
    { 
        if (!(data is IXmlSerializable))
            throw new ArgumentException();

        CheckIfPathIsValid(FullPath);



        using (Stream stream = new FileStream(FullPath, FileMode.OpenOrCreate, FileAccess.Read))
        {

            try
            {
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    if (data != null)
                        (data as IXmlSerializable).ReadXml(reader);
                    else
                    {
                      var ctors=  typeof(T).GetConstructors(BindingFlags.Public|BindingFlags.Instance).Where(x=>x.GetParameters().Length == 1).Where(x=>x.GetParameters()[0].ParameterType.IsSubclassOf(typeof(XmlReader))).ToList();
                        if (ctors.Count > 0)
                            ctors[0].Invoke(data, new object[] {reader});
                        else
                        {
                            var defaultCtor= typeof(T).GetType().GetConstructors(BindingFlags.Public | BindingFlags.Instance).Where(x => x.GetParameters().Length == 0).ToList();
                            if (defaultCtor.Count > 0) defaultCtor[0].Invoke(data, null);
                            (data as IXmlSerializable).ReadXml(reader);
                        }
                    }             
                }
              //  XmlSerializer serializer = new XmlSerializer(typeof(T));
              // data =  (T) serializer.Deserialize(stream);
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("error", e.Message, "ok");
                Debug.LogWarning(e.Message);
                throw e;
            }


            stream.Flush();
        }
        AssetDatabase.Refresh();
        return this;
    }

    public override ISaveLoad Load(ref EventMethodsGroupsCollection date)
    {
        if(date == null) date = new EventMethodsGroupsCollection();
        return Load<EventMethodsGroupsCollection>(ref date);
    }


    //----------------------------------------------------------------------------------

}