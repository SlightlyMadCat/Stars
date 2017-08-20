using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Runtime.InteropServices;

public class ShareScreenshot : MonoBehaviour
{
    public static ShareScreenshot SharedInstance;
    public UIManager uiManager;

    private void Awake()
    {
        SharedInstance = this;
    }

    /*private bool isProcessing = false;
    public float startX;
    public float startY;
    public int valueX;
    public int valueY;



    public void shareScreenshot()
    {

        //if (!isProcessing && GameObject.Find("Ask Friend").GetComponent<Image>().enabled)
            StartCoroutine(captureScreenshot());
    }

    public IEnumerator captureScreenshot()
    {
        isProcessing = true;
        yield return new WaitForEndOfFrame();

        Texture2D screenTexture = new Texture2D(Screen.width * valueX, Screen.height * valueY, TextureFormat.RGB24, true);

        // put buffer into texture
        screenTexture.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height),0,0);
        //create a Rect object as per your needs.
        //screenTexture.ReadPixels(new Rect
                                 //(Screen.width * startX, (Screen.height * startY), Screen.width * valueX / 10000, Screen.height * valueY / 10000), 0, 0);

        // apply
        screenTexture.Apply();

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- PHOTO

        //byte[] dataToSave = Resources.Load<TextAsset>("everton").bytes;
        byte[] dataToSave = screenTexture.EncodeToPNG();

        string destination = Path.Combine(Application.persistentDataPath, System.DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".png");

        File.WriteAllBytes(destination, dataToSave);


        if (!Application.isEditor)
        {
            // block to open the file and share it ------------START
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + destination);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), "Huetest\n" +
                                                 "Go and fuck yourself at " + "\nhttps://play.google.com/store/apps/details?id=com.DarkGlasses.PeterRider&hl=en");
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), "WTF?");
            intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

            // option one WITHOUT chooser:
            currentActivity.Call("startActivity", intentObject);

            // block to open the file and share it ------------END

        }
        isProcessing = false;

    }*/

    public string ScreenshotName = "screenshot.png";

    public void ShareScreenshotWithText(string text)
    {
        string screenShotPath = Application.persistentDataPath + "/" + ScreenshotName;

        //Application.CaptureScreenshot(ScreenshotName);

        //string path = System.IO.Path.Combine(Application.streamingAssetsPath, "sharing_picture.png"); Share(text, path, "");

        Share(text, screenShotPath, "");
    }

    public void Share(string shareText, string imagePath, string url, string subject = "")
    {
#if UNITY_ANDROID
        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
        AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
        AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + imagePath);
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
        intentObject.Call<AndroidJavaObject>("setType", "image/png");

        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareText + "\n" +
                                                /* "Go and ride like Peter " + "\nhttps://play.google.com/store/apps/details?id=com.DarkGlasses.PeterRider&hl=en*/ "Try star city!");

        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

        AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, subject);
        currentActivity.Call("startActivity", jChooser);
#elif UNITY_IOS
         CallSocialShareAdvanced(shareText, subject, url, imagePath);
#else
         Debug.Log("No sharing set up for this platform.");
#endif
        HideSnapScreen();
    }

#if UNITY_IOS
     public struct ConfigStruct
     {
         public string title;
         public string message;
     }
 
     [DllImport ("__Internal")] private static extern void showAlertMessage(ref ConfigStruct conf);
     
     public struct SocialSharingStruct
     {
         public string text;
         public string url;
         public string image;
         public string subject;
     }
     
     [DllImport ("__Internal")] private static extern void showSocialSharing(ref SocialSharingStruct conf);
     
     public static void CallSocialShare(string title, string message)
     {
         ConfigStruct conf = new ConfigStruct();
         conf.title  = title;
         conf.message = message;
         showAlertMessage(ref conf);
     }
 
     public static void CallSocialShareAdvanced(string defaultTxt, string subject, string url, string img)
     {
         SocialSharingStruct conf = new SocialSharingStruct();
         conf.text = defaultTxt; 
         conf.url = url;
         conf.image = img;
         conf.subject = subject;
         
         showSocialSharing(ref conf);
     }
#endif

    public void HideSnapScreen()
    {
        uiManager.ShowSnapScreen(2);
    }
}
