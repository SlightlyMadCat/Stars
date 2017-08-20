using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;
using UnityEngine.UI;

public class MathData : MonoBehaviour
{
    public Text debug;
    public TextAsset math;

    public LevelManager levelManager;
    public DronSpawner dronSpawner;
    public TouchRecognize touchRecognize;
    public CloneCenter cloneCenter;
    public Economics economics;
    public CarSpawner carSpawner;
    public RecordingStudio recStudio;
    public PostBox post;
    public BusStation busStation;

    void Awake()
    {
        GameObject[] hostelsPrefs = GameObject.FindGameObjectsWithTag("Hostel");
        XmlTextReader reader = new XmlTextReader(new StringReader(math.text));

        while (reader.Read())
        {
            if (reader.IsStartElement("HostelPrice"))
            {
                for (int j = 0; j < hostelsPrefs.Length; j++)
                {
                    for (int i = 1; i < reader.AttributeCount; i++)
                    {
                        string attrName = "hostel_" + i.ToString();
                        hostelsPrefs[j].GetComponent<StarHostel>().hostelBuildings[i - 1].GetComponent<CurrentHostel>().price = double.Parse(reader.GetAttribute(attrName));
                    }
                }
            } else if (reader.IsStartElement("HostelCapacity"))
            {
                for (int j = 0; j < hostelsPrefs.Length; j++)
                {
                    for (int i = 1; i < reader.AttributeCount; i++)
                    {
                        string attrName = "hostel_" + i.ToString();
                        hostelsPrefs[j].GetComponent<StarHostel>().hostelBuildings[i - 1].GetComponent<CurrentHostel>().thisHostelCapacity = int.Parse(reader.GetAttribute(attrName));
                    }
                }
            } else if (reader.IsStartElement("LevelFV"))
            {
                for (int i = 1; i < reader.AttributeCount; i++)
                {
                    string attrName = "level_" + i.ToString();
                    levelManager.cloneValues[i - 1] = double.Parse(reader.GetAttribute(attrName));
                }
            } else if (reader.IsStartElement("LevelComplete"))
            {
                for (int i = 1; i < reader.AttributeCount; i++)
                {
                    string attrName = "level_" + i.ToString();
                    levelManager.valueToBuy[i - 1] = double.Parse(reader.GetAttribute(attrName));
                }
            } else if (reader.IsStartElement("Dron"))
            {
                dronSpawner.spawnCd = float.Parse(reader.GetAttribute("spawnCd"));
                dronSpawner.dron.GetComponent<Dron>().minPercent = float.Parse(reader.GetAttribute("minPrize"));
                dronSpawner.dron.GetComponent<Dron>().maxPercent = float.Parse(reader.GetAttribute("maxPrize"));
            } else if (reader.IsStartElement("People"))
            {
                touchRecognize.addClonePerSec = float.Parse(reader.GetAttribute("attractRate"));
            } else if (reader.IsStartElement("CloneCenter"))
            {
                cloneCenter.timePanelPerSecond = float.Parse(reader.GetAttribute("hatcheryPanelRefill"));
                cloneCenter.minusTimePerSecond = float.Parse(reader.GetAttribute("hatcheryPerPerson"));
                cloneCenter.cloneLayingRate = float.Parse(reader.GetAttribute("cloneMakingRate"));
            } else if (reader.IsStartElement("RecordingStudio"))
            {
                recStudio.siloPrice = float.Parse(reader.GetAttribute("slotPrice"));
            } else if (reader.IsStartElement("Money"))
            {
                economics.layingRate = float.Parse(reader.GetAttribute("moneyMakingRate"));
                economics.minADtimer = float.Parse(reader.GetAttribute("watchAdmin"));
                economics.maxADtimer = float.Parse(reader.GetAttribute("watchADmax"));
                economics.minPercentAd = float.Parse(reader.GetAttribute("minPercentAd"));
                economics.maxPercentAd = float.Parse(reader.GetAttribute("maxPercentAd"));

                carSpawner.minCd = float.Parse(reader.GetAttribute("postGiftmin"));
                carSpawner.maxCd = float.Parse(reader.GetAttribute("postGiftmax"));

                post.minPercent = float.Parse(reader.GetAttribute("minPercentGift"));
                post.maxPercent = float.Parse(reader.GetAttribute("maxPercentGift"));
            } else if (reader.IsStartElement("VehiclePrice"))
            {
                for (int i = 1; i < reader.AttributeCount; i++)
                {
                    string attrName = "vehicle_" + i.ToString();
                    busStation.carPrefabs[i-1].GetComponent<Car>().price = double.Parse(reader.GetAttribute(attrName));
                }
            } else if (reader.IsStartElement("VehicleCapacity"))
            {
                for (int i = 1; i < reader.AttributeCount; i++)
                {
                    string attrName = "vehicle_" + i.ToString();
                    busStation.carPrefabs[i - 1].GetComponent<Car>().capacity = double.Parse(reader.GetAttribute(attrName));
                }
            }
        }
        reader.Close();

       // debug.text = "done";
        /*XmlDocument doc = new XmlDocument();
        debug.text = doc + "create "+ Application.persistentDataPath + "/Math.xml";
        doc.Load(Application.persistentDataPath + "/Math.xml");
        debug.text = doc+"doc";

        XmlElement data = doc.GetElementById("HOSTELPRICE");
        debug.text = data+"data";
        //XmlElement hostCapacity = doc.GetElementById("HOSTELCAPACITY");

        GameObject[] hostelsPrefs = GameObject.FindGameObjectsWithTag("Hostel");

        //сначала цены
        for (int j = 0; j < hostelsPrefs.Length; j++)
        {
            for (int i = 1; i < data.Attributes.Count; i++)
            {
                string attrName = "hostel_" + i.ToString();
                hostelsPrefs[j].GetComponent<StarHostel>().hostelBuildings[i-1].GetComponent<CurrentHostel>().price = double.Parse(data.GetAttribute(attrName));
            }
        }
        //потом вместимость

        data = doc.GetElementById("HOSTELCAPACITY");
        debug.text = "" + double.Parse(data.GetAttribute("hostel_1"));

        for (int j = 0; j < hostelsPrefs.Length; j++)
        {
            for (int i = 1; i < data.Attributes.Count; i++)
            {
                string attrName = "hostel_" + i.ToString();
                hostelsPrefs[j].GetComponent<StarHostel>().hostelBuildings[i-1].GetComponent<CurrentHostel>().thisHostelCapacity = int.Parse(data.GetAttribute(attrName));
            }
        }
        //потом цены fv

        data = doc.GetElementById("LEVELFV");
        for (int i = 1; i < data.Attributes.Count; i++)
        {
            string attrName = "level_" + i.ToString();
            levelManager.cloneValues[i-1] = double.Parse(data.GetAttribute(attrName));
        }
        //потом цена на след уровень

        data = doc.GetElementById("LEVELCOMPLETE");
        for (int i = 1; i < data.Attributes.Count; i++)
        {
            string attrName = "level_" + i.ToString();
            levelManager.valueToBuy[i-1] = double.Parse(data.GetAttribute(attrName));
        }
        //потом dron spawn cd

        data = doc.GetElementById("DRON");
        dronSpawner.spawnCd = float.Parse(data.GetAttribute("spawnCd"));
        dronSpawner.dron.GetComponent<Dron>().minPercent = float.Parse(data.GetAttribute("minPrize"));
        dronSpawner.dron.GetComponent<Dron>().maxPercent = float.Parse(data.GetAttribute("maxPrize"));
        // потом attractRate

        data = doc.GetElementById("PEOPLE");
        touchRecognize.addClonePerSec = float.Parse(data.GetAttribute("attractRate"));
        //потом cloneCenter K

        data = doc.GetElementById("CLONECENTER");
        cloneCenter.timePanelPerSecond = float.Parse(data.GetAttribute("hatcheryPanelRefill"));
        cloneCenter.minusTimePerSecond = float.Parse(data.GetAttribute("hatcheryPerPerson"));
        cloneCenter.cloneLayingRate = float.Parse(data.GetAttribute("cloneMakingRate"));
        //потом money

        data = doc.GetElementById("MONEY");
        economics.layingRate = float.Parse(data.GetAttribute("moneyMakingRate"));
        economics.minADtimer = float.Parse(data.GetAttribute("watchAdmin"));
        economics.maxADtimer = float.Parse(data.GetAttribute("watchADmax"));
        economics.minPercentAd = float.Parse(data.GetAttribute("minPercentAd"));
        economics.maxPercentAd = float.Parse(data.GetAttribute("maxPercentAd"));

        carSpawner.minCd = float.Parse(data.GetAttribute("postGiftmin"));
        carSpawner.maxCd = float.Parse(data.GetAttribute("postGiftmax"));*/
    }
}

