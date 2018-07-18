using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;

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

    public override void OnPointerDown(PointerEventData eventData)
    {
         base.OnPointerDown(eventData);
        var rect = GetScreenRect();
        Debug.Log(rect);
        SetupOverlayDialogHtml(this.text, (int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
        lastText = text;
        this.text = "";
        placeHold.text = "";
        StartCoroutine(this.OverlayHtmlCoroutine());
    }

    private IEnumerator OverlayHtmlCoroutine()
    {
        caretPosition = text.Length;
        yield return new WaitForEndOfFrame();
        WebGLInput.captureAllKeyboardInput = false;
		while (IsOverlayDialogHtmlActive())
		{
			yield return null;
            var textFromHtml = GetOverlayHtmlInputFieldValue();
            if(textFromHtml != this.text)
            {
                this.text = textFromHtml;
                caretPosition = textFromHtml.Length;
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
}
#else
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        var rect = GetScreenRect();
        Debug.Log(rect);
        Debug.Log(Input.mousePosition);
    }

#endif
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            this.DeactivateInputField();
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
    private Rect GetScreenRect()
    {
        var rectTransform = textComponent.transform as RectTransform;
        rectTransform.GetWorldCorners(conner);
        Debug.Log(Screen.width);
        return new Rect(conner[0].x,Screen.height - conner[0].y, conner[3].x - conner[0].x, conner[1].y - conner[0].y);
    }

}