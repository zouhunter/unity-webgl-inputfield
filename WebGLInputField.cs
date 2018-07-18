using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;
using System;

public class WebGLInputField : InputField
{
    private Vector3[] conner = new Vector3[4];
    private string placeholdText;
    private string lastText;
    private Text placeHold
    {
        get
        {
            return placeholder.GetComponent<Text>();
        }
    }
    protected override void Awake()
    {
        base.Awake();
        placeholdText = placeHold.text;
        onValueChanged.AddListener(OpenHtmlTextEditor);
    }




#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void SetupOverlayDialogHtml(string text, int x, int y, int width, int height);
    [DllImport("__Internal")]
    private static extern bool IsOverlayDialogHtmlActive();
    [DllImport("__Internal")]
    private static extern bool IsOverlayDialogHtmlCanceled();
    [DllImport("__Internal")]
    private static extern string GetOverlayHtmlInputFieldValue();
    [DllImport("__Internal")]
    private static extern int GetCursortPosition();
    [DllImport("__Internal")]
    private static extern void SetCursortPosition(int index);

    private bool inHtml;
    protected override void OnEnable()
    {
        inHtml = false;
    }

    private void OpenHtmlTextEditor(string arg0)
    {
        if (!inHtml)
        {
            inHtml = true;
            var rect = GetScreenRect();
            SetupOverlayDialogHtml(this.text, (int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
            lastText = text;
            StartCoroutine(this.OverlayHtmlCoroutine());
        }
    }

    private IEnumerator OverlayHtmlCoroutine()
    {
        WebGLInput.captureAllKeyboardInput = true;
        SetCursortPosition(caretPosition);
        yield return new WaitForFixedUpdate();
        WebGLInput.captureAllKeyboardInput = false;
        this.text = "";
        placeHold.text = "";

        while (IsOverlayDialogHtmlActive())
		{
			yield return null;
            var textFromHtml = GetOverlayHtmlInputFieldValue();
            if(textFromHtml != this.text) {
                this.text = textFromHtml;
            }
            if(!WebGLInput.captureAllKeyboardInput)
            {
                caretPosition = GetCursortPosition();
            }
        }

        WebGLInput.captureAllKeyboardInput = true;

        if (!IsOverlayDialogHtmlCanceled())
		{
			this.text = GetOverlayHtmlInputFieldValue();
           
        }
        else
        {
            this.text = lastText;
        }
        placeHold.text = placeholdText;
        inHtml = false;
}
#else
    private void OpenHtmlTextEditor(string arg0)
    {
        var rect = GetScreenRect();
        Debug.Log(rect);
        Debug.Log(Input.mousePosition);
    }
#endif

    private Rect GetScreenRect()
    {
        var rectTransform = textComponent.transform as RectTransform;
        rectTransform.GetWorldCorners(conner);
        Debug.Log(Screen.width);
        return new Rect(conner[0].x,Screen.height - conner[0].y, conner[3].x - conner[0].x, conner[1].y - conner[0].y);
    }

}