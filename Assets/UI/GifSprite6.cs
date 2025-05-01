using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GifSprite6 : MonoBehaviour
{
    [SerializeField] private float mImageDelay;
    private float mCurrentDelay;
    private int mCurrentImageID;
    private SpriteRenderer mySR;
    private Image myImage;

    //public Sprite[] WiperSprite;
    //[SerializeField] private string WiperResourcePath = "Wiper/Comp ";
    //[SerializeField] private int WiperSpriteCount = 101;

    //public Sprite[] WaterSprite;
    //[SerializeField] private string WaterResourcePath = "Washer/Comp ";
    //[SerializeField] private int WaterSpriteCount = 111;

    //public Sprite[] TireSprite;
    //[SerializeField] private string TireResourcePath = "Tire/Comp ";
    //[SerializeField] private int TireSpriteCount = 51;

    //public Sprite[] SpraySprite;
    //[SerializeField] private string SprayResourcePath = "Spray/Comp ";
    //[SerializeField] private int SpraySpriteCount = 71;

    //public Sprite[] OilSprite;
    //[SerializeField] private string OilResourcePath = "Oil/Comp ";
    //[SerializeField] private int OilSpriteCount = 81;

    //public Sprite[] BatterySprite;
    //[SerializeField] private string BatteryResourcePath = "Battery/Comp ";
    //[SerializeField] private int BatterySpriteCount = 75;

    public Sprite[] FilterSprite;
    [SerializeField] private string FilterResourcePath = "Filter/Comp ";
    [SerializeField] private int FilterSpriteCount = 88;

    // Start is called before the first frame update
    void Awake()
    {
        this.mySR = GetComponent<SpriteRenderer>();

        if(mySR == null)
        {
            this.myImage = GetComponent<Image>();
        }

        //WiperSprite = new Sprite[WiperSpriteCount];
        //for (int i = 0; i < WiperSprite.Length; i++)
        //{
        //    WiperSprite[i] = Resources.Load<Sprite>(WiperResourcePath + (i + 1000));
        //}

        //WaterSprite = new Sprite[WaterSpriteCount];
        //for (int i = 0; i < WaterSprite.Length; i++)
        //{
        //    WaterSprite[i] = Resources.Load<Sprite>(WaterResourcePath + (i + 1051));
        //}

        //TireSprite = new Sprite[TireSpriteCount];
        //for (int i = 0;i < TireSprite.Length; i++)
        //{
        //    TireSprite[i] = Resources.Load<Sprite>(TireResourcePath + (i + 100));
        //}

        //SpraySprite = new Sprite[SpraySpriteCount];
        //for (int i = 0; i < TireSprite.Length; i++)
        //{
        //    SpraySprite[i] = Resources.Load<Sprite>(SprayResourcePath + (i + 100));
        //}

        //OilSprite = new Sprite[OilSpriteCount];
        //for (int i = 0; i < TireSprite.Length; i++)
        //{
        //    OilSprite[i] = Resources.Load<Sprite>(OilResourcePath + (i + 1030));
        //}

        //BatterySprite = new Sprite[BatterySpriteCount];
        //for (int i = 0; i < BatterySprite.Length; i++)
        //{
        //    BatterySprite[i] = Resources.Load<Sprite>(BatteryResourcePath + (i + 100));
        //}

        FilterSprite = new Sprite[FilterSpriteCount];
        for (int i = 0; i < FilterSprite.Length; i++)
        {
            FilterSprite[i] = Resources.Load<Sprite>(FilterResourcePath + (i + 100));
        }
    }
    private void OnEnable()
    {
        mCurrentImageID = 0;
        mCurrentDelay = mImageDelay;

        //if (mySR != null)
        //{
        //    mySR.sprite = WiperSprite[mCurrentImageID];
        //}
        //else
        //{
        //    myImage.sprite = WiperSprite[mCurrentImageID];
        //}

        //if (mySR != null)
        //{
        //    mySR.sprite = WaterSprite[mCurrentImageID];
        //}
        //else
        //{
        //    myImage.sprite = WaterSprite[mCurrentImageID];
        //}

        //if (mySR != null)
        //{
        //    mySR.sprite = TireSprite[mCurrentImageID];
        //}
        //else
        //{
        //    myImage.sprite = TireSprite[mCurrentImageID];
        //}

        //if (mySR != null)
        //{
        //    mySR.sprite = SpraySprite[mCurrentImageID];
        //}
        //else
        //{
        //    myImage.sprite = SpraySprite[mCurrentImageID];
        //}

        //if (mySR != null)
        //{
        //    mySR.sprite = OilSprite[mCurrentImageID];
        //}
        //else
        //{
        //    myImage.sprite = OilSprite[mCurrentImageID];
        //}

        //if (mySR != null)
        //{
        //    mySR.sprite = BatterySprite[mCurrentImageID];
        //}
        //else
        //{
        //    myImage.sprite = BatterySprite[mCurrentImageID];
        //}

        if (mySR != null)
        {
            mySR.sprite = FilterSprite[mCurrentImageID];
        }
        else
        {
            myImage.sprite = FilterSprite[mCurrentImageID];
        }
    }

    // Update is called once per frame
    void Update()
    {
        mCurrentDelay -= Time.deltaTime;

        //if (mCurrentDelay < 0)
        //{
        //    ++mCurrentImageID;

        //    if (mCurrentImageID == WiperSprite.Length)
        //    {
        //        mCurrentImageID = 0;
        //    }
        //    if (mySR != null)
        //    {
        //        mySR.sprite = WiperSprite[mCurrentImageID];
        //    }
        //    else
        //    {
        //        myImage.sprite = WiperSprite[mCurrentImageID];
        //    }
        //    mCurrentDelay = mImageDelay;
        //}

        //if (mCurrentDelay < 0)
        //{
        //    ++mCurrentImageID;

        //    if (mCurrentImageID == WaterSprite.Length)
        //    {
        //        mCurrentImageID = 0;
        //    }
        //    if (mySR != null)
        //    {
        //        mySR.sprite = WaterSprite[mCurrentImageID];
        //    }
        //    else
        //    {
        //        myImage.sprite = WaterSprite[mCurrentImageID];
        //    }
        //    mCurrentDelay = mImageDelay;
        //}

        //if (mCurrentDelay < 0)
        //{
        //    ++mCurrentImageID;

        //    if (mCurrentImageID == TireSprite.Length)
        //    {
        //        mCurrentImageID = 0;
        //    }
        //    if (mySR != null)
        //    {
        //        mySR.sprite = TireSprite[mCurrentImageID];
        //    }
        //    else
        //    {
        //        myImage.sprite = TireSprite[mCurrentImageID];
        //    }
        //    mCurrentDelay = mImageDelay;
        //}

        //if (mCurrentDelay < 0)
        //{
        //    ++mCurrentImageID;

        //    if (mCurrentImageID == SpraySprite.Length)
        //    {
        //        mCurrentImageID = 0;
        //    }
        //    if (mySR != null)
        //    {
        //        mySR.sprite = SpraySprite[mCurrentImageID];
        //    }
        //    else
        //    {
        //        myImage.sprite = SpraySprite[mCurrentImageID];
        //    }
        //    mCurrentDelay = mImageDelay;
        //}

        //if (mCurrentDelay < 0)
        //{
        //    ++mCurrentImageID;

        //    if (mCurrentImageID == OilSprite.Length)
        //    {
        //        mCurrentImageID = 0;
        //    }
        //    if (mySR != null)
        //    {
        //        mySR.sprite = OilSprite[mCurrentImageID];
        //    }
        //    else
        //    {
        //        myImage.sprite = OilSprite[mCurrentImageID];
        //    }
        //    mCurrentDelay = mImageDelay;
        //}

        //if (mCurrentDelay < 0)
        //{
        //    ++mCurrentImageID;

        //    if (mCurrentImageID == BatterySprite.Length)
        //    {
        //        mCurrentImageID = 0;
        //    }
        //    if (mySR != null)
        //    {
        //        mySR.sprite = BatterySprite[mCurrentImageID];
        //    }
        //    else
        //    {
        //        myImage.sprite = BatterySprite[mCurrentImageID];
        //    }
        //    mCurrentDelay = mImageDelay;
        //}

        if (mCurrentDelay < 0)
        {
            ++mCurrentImageID;

            if (mCurrentImageID == FilterSprite.Length)
            {
                mCurrentImageID = 0;
            }
            if (mySR != null)
            {
                mySR.sprite = FilterSprite[mCurrentImageID];
            }
            else
            {
                myImage.sprite = FilterSprite[mCurrentImageID];
            }
            mCurrentDelay = mImageDelay;
        }
    }
}
