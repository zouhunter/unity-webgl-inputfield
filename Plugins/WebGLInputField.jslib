var WebGLInputField = {
	//打开输入框
  ShowInputFieldDialog:function(defaultValue){
  
      try {
          defaultValue = Pointer_stringify(defaultValue);
      } catch (e) {
		  alert(e);
		  return;
      }

      if(!document.getElementById("nativeInputDialog")) {
	      var element = document.createElement('div');
          // setup html
          var html = '<div id="nativeInputDialog" style="background:transparent; width:0%; height:0%; margin: 0; padding: 0; position: absolute; z-index:888;">' +
              '<input id="nativeInputDialogInput" type="text" style="border: none; background: none; width:0; height:0;left: 0; top:0;color: white; outline: none; display: block; position: relative; font-size: 10px; ">' +
              '</div>';
		  element.innerHTML = html;
		  document.body.appendChild(element);
		  
		  var m_nativeInputDialogInput = document.getElementById("nativeInputDialogInput");
		  m_nativeInputDialogInput.onkeypress  = function (event) {
			  //点击回车键，隐藏输入框
              if (event.keyCode == 13) {
				  document.getElementById("nativeInputDialog").style.display="none";
              }
          };
		  
		  document.onmousemove=function(event){
		     event = event||window.event;
			 document.getElementById("nativeInputDialog").style.left = event.clientX + 'px';
			 document.getElementById("nativeInputDialog").style.top = event.clientY + 20 + 'px';
		  }
      }
	  var m_nativeInputDialog = document.getElementById("nativeInputDialogInput");
	  m_nativeInputDialog.value = defaultValue;
	  document.getElementById("nativeInputDialog").style.display="";
      document.getElementById("nativeInputDialogInput").focus();
	  
  },
  //隐藏输入框
  HideInputFieldDialog :function(){
	 document.getElementById("nativeInputDialog").style.display="none";
  },
  IsInputFieldDialogActive:function(){
     var nativeDialog = document.getElementById("nativeInputDialog" );
     if(!nativeDialog ){
        return false;
     }
     return ( nativeDialog.style.display != 'none' );
  },
  GetInputFieldValue:function(){
    var elem = document.getElementById("nativeInputDialogInput");
    var returnStr = elem.value;
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },
   GetInputFieldCursortPosition:function  () {
    var dialog = document.getElementById("nativeInputDialogInput");
    var index = dialog.selectionStart;
    return index;
  },
  GetInputFieldCursortFocusPosition:function  () {
    var dialog = document.getElementById("nativeInputDialogInput");
    var index = dialog.selectionEnd;
    return index;
  },
  SetInputFieldCursortPosition:function  (selectionStart,selectionEnd) {
    var elem = document.getElementById("nativeInputDialogInput");
    var val = elem.value
    var len = val.length
 
    // 超过文本长度直接返回
    if (len < selectionStart || len < selectionEnd) return;
	
    setTimeout(function() {
        elem.focus()
        if (elem.setSelectionRange) { // 标准浏览器
			elem.setSelectionRange(selectionStart, selectionEnd); 
        } 
    }, 10)
}
	 
};
mergeInto(LibraryManager.library , WebGLInputField );