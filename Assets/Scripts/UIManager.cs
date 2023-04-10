using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;

public class UIManager : MonoBehaviour
{
    [FormerlySerializedAs("productionParent")] [SerializeField] private GameObject informationParent;
    public BarracksController barracksController;
    public static UIManager Instance { get; set; }

    [SerializeField] private Image buildImage;
    [SerializeField] private TextMeshProUGUI buildName;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    public void OpenInformationParent(string buildNameText,Sprite buildSprite,bool openProduction)
    {
        buildImage.sprite = buildSprite;
        buildName.text = buildNameText;
        if (openProduction)
        {
            informationParent.SetActive(true);
        }
        else
        {
            informationParent.SetActive(false);
        }
    }

    public void CreateProduct(int value)
    {
        barracksController.CreateProduct(value);
    }
    
   

    // Update is called once per frame
    void Update()
    {
        
    }
}
