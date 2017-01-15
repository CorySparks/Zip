using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

//To Do: lid anime
//			 leaderboard
//			 settings and stuff page
//			 Adding tokens to amounts
//			 add ads to get turns
//			 maybe a better looking blue box
// 			 make the amounts change to better amounts after they won and make all the lids anime happen after it showes what they won

public class ChangeBlueBox : MonoBehaviour {
	public Text Amount1;
	public Text Amount2;
	public Text Amount3;
	public Text Amount4;
	public Text Amount5;
	public Text Amount6;
	public Text Amount7;
	public Text Amount8;
	public Text Amount9;
	public Text Amount10;
	public Text Amount11;
	public Text Amount12;
	public Text Amount13;
	public Text Amount14;
	public Text Amount15;
	public Text Amount16;

	public GameObject Lid1;
	public GameObject Lid2;
	public GameObject Lid3;
	public GameObject Lid4;
	public GameObject Lid5;
	public GameObject Lid6;
	public GameObject Lid7;
	public GameObject Lid8;
	public GameObject Lid9;
	public GameObject Lid10;
	public GameObject Lid11;
	public GameObject Lid12;
	public GameObject Lid13;
	public GameObject Lid14;
	public GameObject Lid15;
	public GameObject Lid16;

	public GameObject LidPos1;
	public GameObject LidPos2;
	public GameObject LidPos3;
	public GameObject LidPos4;
	public GameObject LidPos5;
	public GameObject LidPos6;
	public GameObject LidPos7;
	public GameObject LidPos8;
	public GameObject LidPos9;
	public GameObject LidPos10;
	public GameObject LidPos11;
	public GameObject LidPos12;
	public GameObject LidPos13;
	public GameObject LidPos14;
	public GameObject LidPos15;
	public GameObject LidPos16;

	public GameObject Box1;
	public GameObject Box2;
	public GameObject Box3;
	public GameObject Box4;
	public GameObject Box5;
	public GameObject Box6;
	public GameObject Box7;
	public GameObject Box8;
	public GameObject Box9;
	public GameObject Box10;
	public GameObject Box11;
	public GameObject Box12;
	public GameObject Box13;
	public GameObject Box14;
	public GameObject Box15;
	public GameObject Box16;

	public Button StartBtn;
	public Button StartBtnPos;
	public Text StartBtnText;
	public GameObject BlueBox;
	public Text MoneyText;
	public Text WonText;
	public Canvas WonCanvas;
	public Text TimesText;
	public GameObject SettingPanel;
	public Canvas MainCanvas;

	private Vector2 fingerDown;
	private Vector2 fingerUp;
	public bool detectSwipeOnlyAfterRelease = false;
	public float SWIPE_THRESHOLD = 20f;

	public bool WonPanel = false;
	public int muliplyer = 0;
	public bool DoMulti = false;

	private IncomingData JsonData;
	private bool Stopper = true;
	string AmountURL = "http://theglasshousestudios.com:3000/users/createRandAmount";

	public void Awake(){

	}

	public void StartBlueBox(){
		StartBtnPos.enabled = false;
		GameObject[] LidArray = new GameObject[16]{Lid1, Lid2, Lid3, Lid4, Lid5, Lid6, Lid7, Lid8, Lid9, Lid10, Lid11, Lid12, Lid13, Lid14, Lid15, Lid16};
		GameObject[] LidPosArray = new GameObject[16]{LidPos1, LidPos2, LidPos3, LidPos4, LidPos5, LidPos6, LidPos7, LidPos8, LidPos9, LidPos10, LidPos11, LidPos12, LidPos13, LidPos14, LidPos15, LidPos16};
		for(int i = 0;i < LidArray.Length;i++){
			Transform theLidPos = LidPosArray[i].transform;
			LidArray[i].transform.position = new Vector3(theLidPos.position.x, theLidPos.position.y, 0.0f);
		}
		StartCoroutine(Timer());
		StartCoroutine(ChangeAmount());
		StartCoroutine(MoveBlueBox());
		StartCoroutine(WaitFor5Sec());
	}

	public void CloseWonPlanel(){
		WonCanvas.enabled = false;
		StartBtnPos.enabled = true;
		Transform thePos = StartBtnPos.transform;
		StartBtn.transform.position = new Vector3(thePos.position.x, thePos.position.y, 0.0f);
		Stopper = true;
	}

  // Update is called once per frame
  void Update(){
      foreach (Touch touch in Input.touches){
          if (touch.phase == TouchPhase.Began){
              fingerUp = touch.position;
              fingerDown = touch.position;
          }

          //Detects Swipe while finger is still moving
          if (touch.phase == TouchPhase.Moved){
              if (!detectSwipeOnlyAfterRelease){
                  fingerDown = touch.position;
                  checkSwipe();
              }
          }

          //Detects swipe after finger is released
          if (touch.phase == TouchPhase.Ended){
              fingerDown = touch.position;
              checkSwipe();
          }
      }
  }

  void checkSwipe(){
      //Check if Vertical swipe
      if (verticalMove() > SWIPE_THRESHOLD && verticalMove() > horizontalValMove()){
          //Debug.Log("Vertical");
          if (fingerDown.y - fingerUp.y > 0){//Up Swipe
              OnSwipeUp();
          }
          else if (fingerDown.y - fingerUp.y < 0){//Down swipe
              OnSwipeDown();
          }
          fingerUp = fingerDown;
      }

      //Check if Horizontal swipe
      else if (horizontalValMove() > SWIPE_THRESHOLD && horizontalValMove() > verticalMove()){
          //Debug.Log("Horizontal");
          if (fingerDown.x - fingerUp.x > 0){//Left swipe
						OnSwipeLeft();
          }
          else if (fingerDown.x - fingerUp.x < 0){//Right swipe
							OnSwipeRight();
          }
          fingerUp = fingerDown;
      }

      //No Movement at-all
      else{
          //Debug.Log("No Swipe!");
      }
  }

  float verticalMove(){
      return Mathf.Abs(fingerDown.y - fingerUp.y);
  }

  float horizontalValMove(){
      return Mathf.Abs(fingerDown.x - fingerUp.x);
  }

  //////////////////////////////////CALLBACK FUNCTIONS/////////////////////////////
  void OnSwipeUp(){

  }

  void OnSwipeDown(){

  }

  void OnSwipeLeft(){
		MainCanvas.enabled = false;
		SettingPanel.transform.position = new Vector3(0.0f, 0.0f, -1.0f);
  }

  void OnSwipeRight(){
		MainCanvas.enabled = true;
		SettingPanel.transform.position = new Vector3(500.0f, 500.0f, 15.0f);
  }

	IEnumerator ChangeAmount(){
    Text[] AmountArray = new Text[16]{Amount1, Amount2, Amount3, Amount4, Amount5, Amount6, Amount7, Amount8, Amount9, Amount10, Amount11, Amount12, Amount13, Amount14, Amount15, Amount16};

		WWW www = new WWW(AmountURL);
		yield return www;

		IncomingData data = IncomingData.CreateFromJSON(www.text);
		JsonData = data;
		for(int i = 0; i < data.moves.Length; i++){
  		AmountArray[i].text = data.moves[i];
    }
		WonText.text = AmountArray[data.index].text;
	}

	IEnumerator WaitFor5Sec(){
		GameObject[] LidArray = new GameObject[16]{Lid1, Lid2, Lid3, Lid4, Lid5, Lid6, Lid7, Lid8, Lid9, Lid10, Lid11, Lid12, Lid13, Lid14, Lid15, Lid16};
		GameObject[] LidPosArray = new GameObject[16]{LidPos1, LidPos2, LidPos3, LidPos4, LidPos5, LidPos6, LidPos7, LidPos8, LidPos9, LidPos10, LidPos11, LidPos12, LidPos13, LidPos14, LidPos15, LidPos16};

		StartBtn.interactable = false;
		for(int i = 5; i >= 0; i--){
			if(i != 0){
				StartBtnText.text = i.ToString();
				yield return new WaitForSeconds(1);
			}else{
				StartBtnText.text = "Start";
				Transform thePos = StartBtnPos.transform;
				StartBtn.transform.position = new Vector3(thePos.position.x, thePos.position.y, -10.0f);
				Transform theLidPos = LidPosArray[JsonData.index].transform;
				LidArray[JsonData.index].transform.position = new Vector3(theLidPos.position.x, theLidPos.position.y, 6800.0f);
				//Will change this when lid anime is done to pop up the WonCanvas
				WonCanvas.enabled = true;
				if(JsonData.won == 3){
					TimesText.text = JsonData.won + "x";
					muliplyer = 3;
					DoMulti = true;
				}else if(JsonData.won == 2){
					TimesText.text = JsonData.won + "x";
					muliplyer = 2;
					DoMulti = true;
				}else{
					string newMoney = string.Empty;
					int val;

					for(int x = 0; x < MoneyText.text.Length; x++){
					    if(Char.IsDigit(MoneyText.text[x])){
									newMoney += MoneyText.text[x];
							}
					}
					if(newMoney.Length > 0){
					    val = int.Parse(newMoney);
					}else{
							val = 0;
					}
					if(DoMulti){
						double MultiMoneyAmount = val + (JsonData.won * muliplyer);
						MoneyText.text = "$" + String.Format("{0:0.00}", (Double.Parse(MultiMoneyAmount.ToString()) / 100));
						DoMulti = false;
						TimesText.text = "0x";
					}else{
						double NewMoneyAmount = val + JsonData.won;
						MoneyText.text = "$" + String.Format("{0:0.00}", (Double.Parse(NewMoneyAmount.ToString()) / 100));
						DoMulti = false;
						TimesText.text = "0x";
					}
				}
			}
		}
	}

	IEnumerator Timer(){
		for(int i = 5;i >= 0;i--){
			if(i != 0){
				yield return new WaitForSeconds(1);
				Stopper = true;
			}else{
				Stopper = false;
			}
		}
	}

	IEnumerator MoveBlueBox(){
		GameObject[] BoxArray = new GameObject[16]{Box1, Box2, Box3, Box4, Box5, Box6, Box7, Box8, Box9, Box10, Box11, Box12, Box13, Box14, Box15, Box16};

		while(Stopper){
				System.Random random = new System.Random();
				int randomNumber = random.Next(0, 16);
				Transform theBox = BoxArray[randomNumber].transform;
				BlueBox.transform.position = new Vector3(theBox.position.x, theBox.position.y, 0.0f);
				yield return new WaitForSeconds(0.1f);
		}
		if(!Stopper){
				Transform theBox = BoxArray[JsonData.index].transform;
				BlueBox.transform.position = new Vector3(theBox.position.x, theBox.position.y, 0.0f);
		}
	}
}

[System.Serializable]
public class IncomingData{
		public String[] moves;
		public int index;
		public int won;

    public static IncomingData CreateFromJSON(string json){
        return JsonUtility.FromJson<IncomingData>(json);
    }
};
