using Unity.VisualScripting;
using UnityEngine;

public class TopLeftPanel : MonoBehaviour
{
    public GameObject HelpPanel;
    public GameObject SynergyPanel;

    public void OpenPanel(int index)
    {
        if (index == 0)
        {
            HelpPanel.SetActive(!HelpPanel.activeSelf);
            SynergyPanel.SetActive(false);
        }
        else if (index == 1)
        {
            HelpPanel.SetActive(false);
            SynergyPanel.SetActive(!SynergyPanel.activeSelf);
        }
    }

    public void ClosePanel()
    {
        HelpPanel.SetActive(false);
        SynergyPanel.SetActive(false);
    }
}
