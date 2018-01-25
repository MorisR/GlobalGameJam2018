using System;
using UnityEditor;
using UnityEngine;

public class RenameGroupWindow : ScriptableWizard
{

    const int WindowWidth = 400;
    const int WindowHeigth = 150;
    [SerializeField] [ReadOnly] string currentGroupName;
    [SerializeField] string newGroupName;
    bool isReady;

    void OnWizardOtherButton()
    {
        if (!isReady) Close();
        try
        {
            EventsManager.Instance.GroupsContainer.RenameGroup(currentGroupName, newGroupName);
            Close();
        }
        catch (Exception e)
        {
            errorString = e.Message;

        }
    }

    void OnWizardCreate()
    {
    }


    void OnEnable()
    {
        createButtonName = "Close";
        otherButtonName = "Rename";
        minSize = maxSize = new Vector2(WindowWidth, WindowHeigth);
    }

    void OnDisable()
    {
        FocusWindowIfItsOpen(typeof(EventsManagerMainWindow));

    }

    public void SetGroup(string groupName)
    {
        isReady = !string.IsNullOrEmpty(groupName);
        currentGroupName = groupName;
    }

}