using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Toggle leftHandedToggle;
    public Toggle SVRLow;
    public Toggle SVRMedium;
    public Toggle SVRHigh;

    private int lowSectorsPerFrame = 50;
    private int mediumSectorsPerFrame = 100;
    private int highSectorsPerFrame = 200;

    void Awake()
    {
        int isLeftHanded = PlayerPrefs.GetInt("IsLeftHanded", 0);
        int sectorsPerFrame = PlayerPrefs.GetInt("sectorsPerFrame", 100);
        
        leftHandedToggle.isOn = (isLeftHanded == 1);

        SVRLow.isOn = (sectorsPerFrame == lowSectorsPerFrame);
        SVRMedium.isOn = (sectorsPerFrame == mediumSectorsPerFrame);
        SVRHigh.isOn = (sectorsPerFrame == highSectorsPerFrame);

        SVRLow.onValueChanged.AddListener(delegate {
            ShadowVerificationRate(SVRLow, SVRLow.isOn, lowSectorsPerFrame);
         });
        SVRMedium.onValueChanged.AddListener(delegate {
            ShadowVerificationRate(SVRMedium, SVRMedium.isOn, mediumSectorsPerFrame);
        });
        SVRHigh.onValueChanged.AddListener(delegate {
            ShadowVerificationRate(SVRHigh, SVRHigh.isOn, highSectorsPerFrame);
        });
    }

    public void IsLeftHanded()
    {
        int isLeftHanded = leftHandedToggle.isOn ? 1 : 0;
        PlayerPrefs.SetInt("IsLeftHanded", isLeftHanded);
        PlayerPrefs.Save();
    }

    private void ShadowVerificationRate(Toggle toggle, bool on, int sectorsPerFrame)
    {
        if (on)
        {
            SVRLow.isOn = (sectorsPerFrame == lowSectorsPerFrame);
            SVRMedium.isOn = (sectorsPerFrame == mediumSectorsPerFrame);
            SVRHigh.isOn = (sectorsPerFrame == highSectorsPerFrame);

            PlayerPrefs.SetInt("sectorsPerFrame", sectorsPerFrame);
        }
        else if (!SVRLow.isOn && !SVRMedium.isOn && !SVRHigh.isOn)
            toggle.isOn = true;
    }
}
