using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

//To Do: put in so when network error pops up to tell then
//			 Put in phone code verification
//			 maybe a better loading anime

public class Register : MonoBehaviour {
	public InputField User;
	public InputField Pass;
  public InputField ConfirmPass;
	public InputField Email;
  public Text ErrText;
  public Text Load;
  public Button RegBtn;

    bool AllowLoading = true;
	string CreateUserURL = "http://theglasshousestudios.com:3000/users/createUser";

    public void Start(){
        message = "Loading...";
    }

    public void SendUser(){
        RegBtn.interactable = false;
        LoaderAnimate();
        StartCoroutine(SendUserData());
    }

    public void LoaderAnimate(){
        AllowLoading = true;
        StartCoroutine(TypeText());
    }

	IEnumerator SendUserData(){
        if(CheckUser()){
    		WWWForm form = new WWWForm();
    		form.AddField("User", User.text);
    		form.AddField("Pass", Sha256(Pass.text));
        form.AddField("ConfirmPass", Sha256(ConfirmPass.text));
    		form.AddField("Email", Email.text);

    		WWW www = new WWW(CreateUserURL, form);
        yield return www;

            if(!string.IsNullOrEmpty(www.error)) {
                ErrText.text = "Error: " + www.error;
            }else{
                if(www.text == "1"){
                    ErrText.text = "<color=green>Registration was successful. Taking you to the login screen.</color>";
										AllowLoading = false;
										yield return new WaitForSeconds(3);
										LoadStage();

                }else if(www.text == "User Taken"){
                    AllowLoading = false;
                    ErrText.text = "Sorry, your username is already in use. Please try another!";
                    RegBtn.interactable = true;

                }else if(www.text == "2"){
                    AllowLoading = false;
                    ErrText.text = "It seems as if your passwords do not match. Please try again.";
                    RegBtn.interactable = true;

                }else if(www.text == "3"){
                    AllowLoading = false;
                    ErrText.text = "Your password must exceed the minimum of 8 characters but less than 24 characters.";
                    RegBtn.interactable = true;

                }else if(www.text == "4"){
                    AllowLoading = false;
                    ErrText.text = "Your username must exceed the minimum of two characters but less than 16 characters.";
                    RegBtn.interactable = true;

                }else if(www.text == "5"){
                    AllowLoading = false;
                    ErrText.text = "You have to complete all fields.";
                    RegBtn.interactable = true;

                }else{
                    AllowLoading = false;
                    ErrText.text = "We are very sorry but we are experiencing trouble right now, please try again in a few moments.";
                    RegBtn.interactable = true;

                }
            }
        }else{
            AllowLoading = false;
            RegBtn.interactable = true;
        }
	}

    public bool CheckUser(){
        if(User.text != "" && Pass.text != "" && Email.text != ""){
            if(Pass.text.Length >= 8){
                if(Pass.text == ConfirmPass.text){
                    return true;
                }else{
                    ErrText.text = "It seems as if your passwords do not match. Please try again.";
                    return false;
                }
            }else{
                ErrText.text = "Your password must be greater than 8 characters.";
                return false;
            }
        }else{
            ErrText.text = "You must complete all fields.";
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
        foreach (char letter in message.ToCharArray()) {
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

    public void LoadStage(){
        SceneManager.LoadScene("Login");
    }
}
