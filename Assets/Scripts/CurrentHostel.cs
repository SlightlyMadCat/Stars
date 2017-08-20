using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentHostel : MonoBehaviour {
    public int thisHostelCapacity;
    public double price;               //TODO : ПЕРЕВЕСТИ В BIGINTEGER
    public string name;
    public Sprite icon;

    private void Start()                //при влючении этого здания, передает его вместимость в центр клонов и сам хостел
    {
        int oldCapacity = GetComponentInParent<StarHostel>().hostelCapacity;

        GetComponentInParent<StarHostel>().hostelCapacity = thisHostelCapacity;
        GetComponentInParent<StarHostel>().UpdHostelScaleCapacity();
        //GameObject.FindGameObjectWithTag("CloneCenter").GetComponent<CloneCenter>().UpdAvailableSlots(thisHostelCapacity - oldCapacity);
        CloneCenter.SharedInstance.CheckAvailPlaces();
    }
}
