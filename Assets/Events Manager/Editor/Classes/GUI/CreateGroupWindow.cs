using UnityEngine;
using System.Collections;
using UnityEditor;
using System;


public class CreateGroupWindow : ScriptableWizard
{


    const int WindowWidth = 400;
    const int WindowHeigth = 150;
    public string GroupName;


    void OnWizardOtherButton()
    {
        if (!NameValidator.isNameValid(GroupName))
            errorString = "Group name is invalid!";
        else
        {

            try
            {
                EventsManager.Instance.GroupsContainer.AddGroup(GroupName);
                Close();
            }
            catch (Exception e)
            {
                errorString =e.Message;
            }

        }

    }

    void OnWizardCreate()
    {
    }


    void OnEnable()
    {
        createButtonName = "Close";
        otherButtonName = "Create";
        minSize = maxSize = new Vector2(WindowWidth, WindowHeigth);
    }

    void OnDisable()
    {
        //FocusWindowIfItsOpen(typeof(EventPluginManagerWindow2));
        //FocusWindowIfItsOpen(typeof(EventPluginManagerWindow));
        FocusWindowIfItsOpen(typeof(EventsManagerMainWindow));

    }



}