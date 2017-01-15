using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

//To Do: Maybe a better loading anime
//			 Make session to know they are that account

public class Login : MonoBehaviour {

	  public InputField UserL;
	  public InputField PassL;
	  public Text ErrText;
    public Text Load;
		public Button RegBtn;
    public Button LoginBtn;

		bool AllowLoading = true;
		string LoginURL = "http://theglasshousestudios.com:3000/users/loginUser";
		string TokenURL = "http://theglasshousestudios.com:3000/users/TokenCheck";

		public void Awake(){
			StartCoroutine(CheckTokenRequest());
		}

    public void Start(){
				if(!CheckForInternetConnection()){
					LoginBtn.interactable = false;
					RegBtn.interactable = false;
					ErrText.text = "It seem that you are not connected to the internet, please check your connection.";
				}else{
					LoginBtn.interactable = true;
					RegBtn.interactable = true;
					ErrText.text = "";
				}
        message = "Loading...";
    }

		public void Update(){

		}

    public void SendUser(){
        LoginBtn.interactable = false;
        LoaderAnimate();
        StartCoroutine(SendUserData());
    }

    public void LoaderAnimate(){
        AllowLoading = true;
        StartCoroutine(TypeText());
    }

	  IEnumerator SendUserData(){
        if(CheckLog()){
    		WWWForm form = new WWWForm();
    		form.AddField("username", UserL.text);
    		form.AddField("password", Sha256(PassL.text));

    		WWW www = new WWW(LoginURL, form);
        yield return www;

				IncomingTokenData data = IncomingTokenData.CreateFromJSON(www.text);

            if(!string.IsNullOrEmpty(www.error)) {
                ErrText.text = "Error: " + www.error;
            }else{
                if(data.pass == "1"){
										PlayerPrefs.SetString("Token", data.token);
										PlayerPrefs.SetInt("Expire", data.expire);
                    ErrText.text = "";
                    SceneManager.LoadScene("Main");
                }else{
                    ErrText.text = "Sorry, the Username or Password you have entered is not recognized.";
                    LoginBtn.interactable = true;
                    AllowLoading = false;
                }
            }
        }else{
            AllowLoading = false;
            LoginBtn.interactable = true;
        }
	  }

    public bool CheckLog(){
        if(UserL.text != "" && PassL.text != ""){
            return true;
        }else{
            ErrText.text = "Please enter your Username and Password.";
            return false;
        }
    }

    public static string Sha256(string text){
        byte[] bytes = Encoding.Unicode.GetBytes(text);
        SHA256Managed hashstring = new SHA256Managed();
        byte[] hash = hashstring.ComputeHash(bytes);
        string hashString = string.Empty;

        foreach (byte x in hash){
            hashString += String.Format("{0:x2}", x);
        }
        return hashString;
    }

    public float letterPause = 0.2f;

    string message;

    IEnumerator TypeText () {
        foreach(char letter in message.ToCharArray()) {
            if(!AllowLoading){
                Load.text = "";
                break;
            }
            Load.text += letter;
            yield return new WaitForSeconds(letterPause);
        }
        Load.text = "";
        if(AllowLoading){
            StartCoroutine(TypeText());
        }
    }

		IEnumerator CheckTokenRequest(){
			WWWForm form = new WWWForm();
			form.AddField("Token", PlayerPrefs.GetString("Token"));
			form.AddField("Expire", PlayerPrefs.GetInt("Expire"));

			WWW www = new WWW(TokenURL, form);
			yield return www;

			Debug.Log(www.text);
			if(www.text == "1"){
				SceneManager.LoadScene("Main");
			}else{
				Debug.Log("Token Not Created");
			}
		}

  	public void LoadStage(){
    	SceneManager.LoadScene("Register");
	  }

		public static bool CheckForInternetConnection(){
		    try{
		        using (var client = new System.Net.WebClient()){
		            using (var stream = client.OpenRead("http://www.google.com")){
		                return true;
		            }
		        }
		    }
		    catch{
		        return false;
		    }
		}
}

[System.Serializable]
public class IncomingTokenData{
		public string pass;
		public string token;
		public int expire;

    public static IncomingTokenData CreateFromJSON(string json){
        return JsonUtility.FromJson<IncomingTokenData>(json);
    }
};
