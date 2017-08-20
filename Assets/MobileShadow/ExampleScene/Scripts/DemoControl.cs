using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoControl : MonoBehaviour
{
	public MobileShadow MobileShadow; 
	public Dropdown QualityDropdown;
	public Slider OpacitySlider;
	public Toggle SoftShadowToggle;
	public Slider ShadowBlur;
    public Dropdown TimeOfDay;
    public GameObject LightsGroup;
	public Toggle Fog;

    public Color ambient;

	void Awake ()
	{
	    switch ((int)MobileShadow.TextureSize)
	    {
            case 256:
                QualityDropdown.value = 0;
                break;
            case 512:
                QualityDropdown.value = 1;
                break;
            case 1024:
                QualityDropdown.value = 2;
                break;
            case 2048:
                QualityDropdown.value = 3;
                break;
        }
		
		//OpacitySlider.value = MobileShadow.ShadowOpacity;
		//SoftShadowToggle.isOn = MobileShadow.SoftShadow;
		//ShadowBlur.value = MobileShadow.BlurSize;
		
		//QualityDropdown.onValueChanged.AddListener(ChangeQuality);
		//OpacitySlider.onValueChanged.AddListener(ChangeOpacity);
		//SoftShadowToggle.onValueChanged.AddListener(TogleSoftShadow);
		//ShadowBlur.onValueChanged.AddListener(ChangeBlur);
        //TimeOfDay.onValueChanged.AddListener(ChangeTimeOfDay);
		//Fog.onValueChanged.AddListener(EnableFog);
	    //ChangeTimeOfDay(0);

        if (SystemInfo.systemMemorySize < 1600)
	    {
	        ChangeQuality(0);
            TogleSoftShadow(false);
            EnableFog(false);
	    }
    }

	public void ChangeTimeOfDay(int value)
    {
        switch (value)
        {
            case 0://day
                RenderSettings.ambientLight = new Color(0.381f, 0.519f, 0.617f);
	            RenderSettings.fogColor = RenderSettings.ambientLight;
                MobileShadow.SunLight.color = new Color32(244, 220, 154, 255);
                MobileShadow.SunLight.transform.rotation = Quaternion.Euler(-240, -308, 0);
                LightsGroup.SetActive(false);
                break;
            case 1://sunset
                //RenderSettings.ambientLight = new Color(0.477f, 0.326f, 0.304f);
                RenderSettings.ambientLight = ambient;
	            RenderSettings.fogColor = RenderSettings.ambientLight;
                //MobileShadow.SunLight.color = new Color32(217, 160, 154, 255);
                MobileShadow.SunLight.color = Color.white;
                MobileShadow.SunLight.transform.rotation = Quaternion.Euler(24, 376, 0);
                LightsGroup.SetActive(false);
                break;
            case 2://night
                RenderSettings.ambientLight = new Color(0.168f, 0.207f, 0.478f);
	            RenderSettings.fogColor = RenderSettings.ambientLight;
                //MobileShadow.SunLight.color = new Color32(45, 98, 186, 255);
                MobileShadow.SunLight.color = Color.white;
                //MobileShadow.SunLight.transform.rotation = Quaternion.Euler(50, 345, 0);
                MobileShadow.SunLight.transform.rotation = Quaternion.Euler(-552, 345, 0);
                LightsGroup.SetActive(true);
                break;
        }
    }

    public void ChangeBlur(float value)
	{
		MobileShadow.BlurSize = value;
	}

	public void TogleSoftShadow(bool value)
	{
		MobileShadow.SoftShadow = value;
	}

	public void ChangeOpacity(float value)
	{
		MobileShadow.ShadowOpacity = value;
	}
	
	public void EnableFog(bool value)
	{
		RenderSettings.fog = value;
		RenderSettings.fogMode = FogMode.Linear;
	}

	public void ChangeQuality(int value)
	{
		int quality = 256;
		switch (value)
		{
			case 0:
				quality = 256;
				MobileShadow.BlurSize = 0.8f;
				break;
			case 1:
				quality = 512;
				MobileShadow.BlurSize = 1.2f;
				break;
			case 2:
				quality = 1024;
				MobileShadow.BlurSize = 1.5f;
				break;
			case 3:
				quality = 2048;
				MobileShadow.BlurSize = 2f;
				break;
		}

        QualityDropdown.value = value;
		MobileShadow.TextureSize = (MobileShadow.Dimension) quality;
	}
}
