using System;
using System.CodeDom.Compiler;
using System.Text;

public class NameValidator
{
    private static CodeDomProvider provider;
    public static CodeDomProvider Provider
    {
        get
        {
            if(provider == null)
                provider = CodeDomProvider.CreateProvider("c#");
            return provider;
        }
    }

    public static bool isNameValid(string name)
    {
        if (string.IsNullOrEmpty(name))
            return false;
        return Provider.IsValidIdentifier(name??" ");
    }

    public static string GetGenericTypeFullName(Type type)
    {
        if (!type.IsGenericType) return type.FullName;


        StringBuilder returnValue = new StringBuilder();
        returnValue.Append(type.GetGenericTypeDefinition().FullName);
        bool remove = false;
        for (int i = 0; i < returnValue.Length; i++)
        {
            if (returnValue[i] == '`')
            {
                returnValue.Remove(i, returnValue.Length - i);
                break;
            }
        }
        returnValue.Append("<");

        var args = type.GetGenericArguments();
        for (int i = 0; i < args.Length; i++)
        {
            if (i != 0) returnValue.Append(",");

            returnValue.Append(GetGenericTypeName(args[i]));
        }
        returnValue.Append(">");

        returnValue.Replace("+", ".");
        return returnValue.ToString();
    }
    public static string GetGenericTypeName(Type type)
    {
        if (!type.IsGenericType) return type.Name;


        StringBuilder returnValue = new StringBuilder();
        returnValue.Append(type.GetGenericTypeDefinition().Name);
        bool remove = false;
        for (int i = 0; i < returnValue.Length; i++)
        {
            if (returnValue[i] == '`')
            {
                returnValue.Remove(i, returnValue.Length - i);
                break;
            }
        }
        returnValue.Append("<");

        var args = type.GetGenericArguments();
        for (int i = 0; i < args.Length; i++)
        {
            if (i != 0) returnValue.Append(",");

            returnValue.Append(GetGenericTypeName(args[i]));
        }
        returnValue.Append(">");

        returnValue.Replace("+", ".");
        return returnValue.ToString();
    }

}