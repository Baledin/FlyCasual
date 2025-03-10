using Mirror;
using TMPro;
using UnityEngine;

public class NetworkConnectionAttemptHandler : MonoBehaviour
{
    public TMP_Text IpInput;
    public GameObject BottomPanel;
    private bool IsTracking;

    public void StartAttempt()
    {
        BottomPanel.SetActive(false);
        IpInput.enabled = false;
        IsTracking = true;
    }

    public void StopAttempt()
    {
        IsTracking = false;
    }

    void Update()
    {
        if (IsTracking)
        {
            if (!NetworkManager.singleton.isNetworkActive)
            {
                AbortAttempt();
            }
        }
    }

    public void AbortAttempt()
    {
        NetworkManager.singleton.StopClient();
        Messages.ShowError("Connection attempt is failed");
        IsTracking = false;
        BottomPanel.SetActive(true);
        IpInput.enabled = true;
    }
}
