﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class NotificationSystem : MonoBehaviour {
	private static int MaxSentences = 5;
	private static float ScreenRatioX;
	private static float ScreenRatioY;
	private static Transform AnnounceNotification;
	private static Transform MissileNotification;
	private static NotificationSystem NotiSystem;
	private static List<RectTransform> Sentences = new List<RectTransform> ();
	private static List<RectTransform> MissileSentences = new List<RectTransform> ();
	// Use this for initialization
	void Awake () {
		ScreenRatioY = (float)Screen.height / 332;
		ScreenRatioX = (float)Screen.width / 588;
		AnnounceNotification = GameObject.Find("Notification").transform;
		MissileNotification = GameObject.Find("MissileNotification").transform;
		NotiSystem = gameObject.GetComponent<NotificationSystem>();
	}

	public enum NotificationType{
		Announce,
		Missile
	}

	private IEnumerator test(){
		int i = 0;
		while (true) {
			i++;
			StartCoroutine (UpdateMissileNotification());
			yield return new WaitForSeconds (1f);
			yield return null;
		}
	}

	public static IEnumerator UpdateNotification(string text){
		GameObject TextBox = NewTextBox (AnnounceNotification);
		yield return NotiSystem.StartCoroutine (MoveUpSentences(true,NotificationType.Announce));
		yield return NotiSystem.StartCoroutine (TextBoxStartUp(TextBox,NotificationType.Announce));
		yield return NotiSystem.StartCoroutine(NotiSystem.FadeInText(TextBox.GetComponent<Text> (),text));
		yield return null;
	}

	public static IEnumerator UpdateMissileNotification(){
		GameObject TextBox = NewTextBox (MissileNotification);
		yield return NotiSystem.StartCoroutine (MoveUpSentences(false,NotificationType.Missile));
		yield return NotiSystem.StartCoroutine (TextBoxStartUp(TextBox,NotificationType.Missile));
		EntrySentence ("Hit!", TextBox.GetComponent<Text> ());
		yield return NotiSystem.StartCoroutine(NotiSystem.FadeOutText(TextBox.GetComponent<Text> (),1f,1f));
		yield return null;
	}

	private static GameObject NewTextBox(Transform Notification){
		GameObject TextBox = new GameObject ("Text");
		TextBox.transform.parent = Notification.transform;
		TextBox.AddComponent<Text> ();
		return TextBox;
	}

	private IEnumerator FadeInText(Text TextBox,string text){
		TextBox.text = text;
		while(TextBox.color.a < 1){
			TextBox.color = new Color (TextBox.color.r,TextBox.color.g,TextBox.color.b,TextBox.color.a + (Time.deltaTime/0.1f));
				yield return null;
		}
	}
	private IEnumerator FadeOutText(Text TextBox,float duration,float delay){
		yield return new WaitForSeconds (duration);
		while (TextBox.color.a > 0) {
			TextBox.color = new Color (TextBox.color.r,TextBox.color.g,TextBox.color.b,TextBox.color.a - (Time.deltaTime * delay));
			yield return null;
		}
		RectTransform TextTransform = TextBox.GetComponent<RectTransform> ();
		if(Sentences.Contains(TextTransform)){
			Sentences.Remove(TextTransform);
		} else if(MissileSentences.Contains(TextTransform)){
			MissileSentences.Remove(TextTransform);
		}
		Destroy (TextBox.gameObject);
		yield return null;
	}

	private static IEnumerator MoveUpSentences(bool isEnableTrash,NotificationType type){//List<RectTransform> TextList){
        var UpRange = VRMode.isVRMode ? 5 : 25;
		List<RectTransform> TextList = new List<RectTransform>();
		if (type == NotificationType.Announce) {
			TextList = Sentences;
		} else {
			TextList = MissileSentences;
		}
		if (isEnableTrash) {
			MoveTrashSentence ();
			if (TextList.Count != 0) {
				TextList [TextList.Count - 1].GetComponent<Text> ().color = Color.white;
			}
		}
		
		for (float time = 0f; time < 0.1f; time += Time.deltaTime) {
			foreach (RectTransform text in TextList) {
				text.Translate (0, (ScreenRatioY * (isEnableTrash ? UpRange*0.4f : UpRange)) * (Time.deltaTime / 0.1f), 0,Space.Self);
			}
			yield return null;
		}
		yield return null;
	}

	private static void MoveTrashSentence(){
		if(Sentences.Count > MaxSentences){
			Destroy(Sentences[0].gameObject);
			Sentences.RemoveAt(0);
		}
	}


	private static void MoveTrashSentence(bool isAll){
		if (isAll) {
			Sentences.Clear ();
			return;
		} else {
			if(Sentences.Count > MaxSentences){
				Destroy(Sentences[0].gameObject);
				Sentences.RemoveAt(0);
			}
		}
	}




	private static IEnumerator TextBoxStartUp(GameObject TextBox,NotificationType type){
		bool isAnnounce = type == NotificationType.Announce ? true : false;

		var TextRect = TextBox.GetComponent<RectTransform> ();
		var TextComponent = TextBox.GetComponent<Text> ();
		TextRect.pivot = isAnnounce ? new Vector2 (1,1) : new Vector2 (0.5f,0.5f);
		TextRect.transform.localPosition = new Vector3 (0,0,0);
        TextRect.transform.rotation = new Quaternion(0,0,0,0);
		TextRect.sizeDelta = isAnnounce ? new Vector2 (1400,130) : new Vector2 (ScreenRatioX*300,ScreenRatioY * 28);
		TextRect.localScale = isAnnounce ? new Vector3 (0.7f, 0.7f, 1) : new Vector3 (1, 1, 1);
		//TextRect.sizeDelta = isAnnounce ? new Vector2 (ScreenRatioX*300,ScreenRatioY * 20) : new Vector2 (ScreenRatioX*300,ScreenRatioY * 28);
		TextRect.anchorMax = isAnnounce ? new Vector2 (1,0) : new Vector2 (0.5f,0);
		TextRect.anchorMin = TextRect.anchorMax;
		TextComponent.alignment = isAnnounce ? TextAnchor.MiddleRight : TextAnchor.MiddleCenter;
		TextComponent.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
		TextComponent.fontSize = isAnnounce ? 43 : (int)(ScreenRatioY * 250);
//		TextComponent.fontSize = isAnnounce ? (int)(ScreenRatioY * 100) : (int)(ScreenRatioY * 250);
		TextComponent.color = isAnnounce ? new Color (1,0.92f,0.16f,0) : new Color (1,0f,0f,0.8f) ;
		if (isAnnounce) {
			Sentences.Add (TextBox.GetComponent<RectTransform> ());
		} else {
			MissileSentences.Add (TextBox.GetComponent<RectTransform> ());
		}
		yield return null;
	}

	private static void EntrySentence(string text,Text NewSentence){
		NewSentence.text = text;
	}


	void OnDestroy(){
		StopAllCoroutines ();
		MoveTrashSentence (true);
	}
}
