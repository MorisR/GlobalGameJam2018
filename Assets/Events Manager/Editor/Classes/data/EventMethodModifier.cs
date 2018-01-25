using System;
using Extentions;

public class EventMethodModifier 
{
    private EventMethodsGroup _containingGroup;
    private EventMethodInfo _method;
    Action _onModifyListener;

    public EventMethodModifier(EventMethodsGroup containingGroup, EventMethodInfo method)
    {
        if (containingGroup == null || method == null)
            throw new ArgumentException("arguments cannot be null!");

        if (!containingGroup.ContainsMethod(method))
            throw new ArgumentException("the method must be contained in the group in order!");

        _containingGroup = containingGroup;
        _method = method;
    }

    ~EventMethodModifier()
    {
       clearListeners_OnModify();
        _method = null;
        _containingGroup = null;
    }


    //inherited/interface methods-------------------------------------------------------

    public void RenameMethod(string newName)
    {

        if (_method.Name.Equals(newName))
            return;

        if (!NameValidator.isNameValid(newName))
            throw new ArgumentException("method name is Invalid!");


        var newMethod = _method.Clone() as EventMethodInfo;
        newMethod.RenameMethod(newName);


        if (_containingGroup.ContainsMethod(newMethod))
            throw new InvalidOperationException("method exists in group!");


        _containingGroup.ReplaceMethod(_method, newMethod);
    }
    public void SwapArgs(int firstInedx, int secondIndex)
    {
        if(_method.ArgsCount <2)
            return;

        var args = _method.Args;
        if (!args.IndexInRange(firstInedx) || !args.IndexInRange(secondIndex))
            return;

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
        if (effect == null)
            return;

        if (_onModifyListener == null)
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

    //----------------------------------------------------------------------------------
}