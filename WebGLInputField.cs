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
    private string lastText;

    protected override void Awake()
    {
        base.Awake();
        onValueChanged.AddListener(OpenHtmlTextEditor);
    }


#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void SetupOverlayDialogHtml(string text, int x, int y, int width, int height);
    [DllImport("__Internal")]
    private static extern bool IsOverlayDialogHtmlActive();
    [DllImport("__Internal")]
    private static extern string GetOverlayHtmlInputFieldValue();
    [DllImport("__Internal")]
    private static extern int GetCursortPosition();
    [DllImport("__Internal")]
    private static extern void HideDialog();
    [DllImport("__Internal")]
    private static extern void SetCursortPosition(int index);

    private bool inHtml;
    private float timer;
    protected override void OnEnable()
    {
        inHtml = false;
        lastText = this.text;
    }

    private void OpenHtmlTextEditor(string arg0)
    {
        if(lastText != arg0)
        {
            if (!inHtml)
            {
                inHtml = true;
                ShowAndDelyHide(arg0);
            }
        }
       
        lastText = arg0;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        if (Time.time - timer < 0.5f)
        {
            caretPosition = GetCursortPosition();
            SwitchInputFieldState();
        }
        else
        {
            timer = Time.time;
            if (!inHtml)
            {
                inHtml = true;
                if(caretPosition == 0)
                 {
                     caretPosition = lastText.Length;
                 }
                ShowAndDelyHide(lastText);
            }
        }
    }

    private void SwitchInputFieldState()
    {
        if (!inHtml)
        {
            inHtml = true;
            ShowAndDelyHide(lastText);
        }
        else
        {
            inHtml = false;
            HideDialog();
            StopCoroutine(OverlayHtmlCoroutine());
        }
    }

    private void ShowAndDelyHide(string text)
    {
        var rect = GetScreenRect();
        WebGLInput.captureAllKeyboardInput = false;
        SetupOverlayDialogHtml(text, (int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
        StartCoroutine(this.OverlayHtmlCoroutine());
    }

    private IEnumerator OverlayHtmlCoroutine()
    {
        WebGLInput.captureAllKeyboardInput = true;
        yield return new WaitForSeconds(0.2f);
        SetCursortPosition(caretPosition);
        WebGLInput.captureAllKeyboardInput = false;
        //EventSystem.current.SetSelectedGameObject(null);
        while (IsOverlayDialogHtmlActive() && isFocused)
		{
			yield return null;
            var textFromHtml = GetOverlayHtmlInputFieldValue();
            if(textFromHtml != this.text) {
                this.text = textFromHtml;
                ForceLabelUpdate();
                yield return null;
            }

            if(!WebGLInput.captureAllKeyboardInput)
            {
                caretPosition = GetCursortPosition();
                ForceLabelUpdate();
                yield return null;
            }
        }
        HideDialog();
        WebGLInput.captureAllKeyboardInput = true;
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
        return new Rect(conner[0].x, Screen.height - conner[0].y, conner[3].x - conner[0].x, conner[1].y - conner[0].y);
    }

}