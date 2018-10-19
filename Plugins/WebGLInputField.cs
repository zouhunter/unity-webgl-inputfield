using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;
using System;

public class WebGLInputField : InputField
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]//显示对话框
    private static extern void ShowInputFieldDialog(string text);
    [DllImport("__Internal")]//隐藏对话框
    private static extern void HideInputFieldDialog();
    [DllImport("__Internal")]//对话框是否显示中
    private static extern bool IsInputFieldDialogActive();
    [DllImport("__Internal")]//获取对话框的数据
    private static extern string GetInputFieldValue();
    [DllImport("__Internal")]//获取光标选中坐标（起点点）
    private static extern int GetInputFieldCursortPosition();
    [DllImport("__Internal")]//获取光标选中坐标（终点）
    private static extern int GetInputFieldCursortFocusPosition();
    [DllImport("__Internal")]//设置光标选择
    private static extern void SetInputFieldCursortPosition(int selectionStart, int selectionEnd);
    private bool captureAllKeyboardInput
    {
        get
        {
            return WebGLInput.captureAllKeyboardInput;
        }
        set
        {
            WebGLInput.captureAllKeyboardInput = value;
        }
    }
    private float timer;
    private Coroutine overlayhtml;
    private Coroutine setposCoroutine;

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        captureAllKeyboardInput = false;

        ShowInputFieldDialog(text);

        if (IsInputFieldDialogActive() && overlayhtml != null)
        {
            //更新光标
            if(setposCoroutine != null)
            {
                SetSelection();
            }
            else
            {
                setposCoroutine = StartCoroutine(DelySetPostion());
            }
        }
        else
        {
            //打开html端的输入框
            overlayhtml = StartCoroutine(this.OverlayHtmlCoroutine());
        }
    }

    private IEnumerator DelySetPostion()
    {
        captureAllKeyboardInput = true;
        yield return null;
        SetSelection();
        captureAllKeyboardInput = false;
        setposCoroutine = null;
        System.GC.Collect();
    }

    private IEnumerator OverlayHtmlCoroutine()
    {
        yield return DelySetPostion();
        //设置选中对象为
        while (IsInputFieldDialogActive() && isFocused)
        {
            yield return null;
            var textFromHtml = GetInputFieldValue();
            if (textFromHtml != this.text)
            {
                this.text = textFromHtml;
                ForceLabelUpdate();
                yield return null;
            }

            if (!captureAllKeyboardInput && setposCoroutine == null && !Input.GetMouseButton(0))
            {
                UpdateCaretPositions();
                yield return null;
            }
        }
        HideInputFieldDialog();
        EventSystem.current.SetSelectedGameObject(null);
        captureAllKeyboardInput = true;
        overlayhtml = null;
        System.GC.Collect();
    }

    /// <summary>
    /// 设置选中区域
    /// </summary>
    private void SetSelection()
    {
        var selectionStart = selectionAnchorPosition < selectionFocusPosition ? selectionAnchorPosition : selectionFocusPosition;
        var selectionEnd = selectionAnchorPosition > selectionFocusPosition ? selectionAnchorPosition : selectionFocusPosition;
        SetInputFieldCursortPosition(selectionStart, selectionEnd);
    }

    /// <summary>
    /// 从html更新caretPosition
    /// </summary>
    private void UpdateCaretPositions()
    {
        var cpos = GetInputFieldCursortPosition();
        var fpos = GetInputFieldCursortFocusPosition();
        var changed = false;
        if (cpos != caretPosition)
        {
            caretPosition = cpos;
            changed = true;
        }
        if (fpos != selectionFocusPosition)
        {
            selectionFocusPosition = fpos;
            changed = true;
        }

        if (changed)
        {
            ForceLabelUpdate();
        }
    }

#endif
    //注意Time.timeScale = 0 会无法更新信息
    //private void Update()
    //{
    //    if(Input.GetMouseButtonDown(1) && isFocused)
    //    {
    //        Debug.Log("caretPosition:" + caretPosition);//光标坐标
    //        Debug.Log("currentSelectionState:" + currentSelectionState);//选中状态
    //        Debug.Log("selectionAnchorPosition:" + selectionAnchorPosition);//选择起点
    //        Debug.Log("selectionFocusPosition:" + selectionFocusPosition);//选择结束点
    //    }
    //}
}