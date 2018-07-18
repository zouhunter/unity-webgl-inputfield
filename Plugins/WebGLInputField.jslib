var WebGLInputField = {
  SetupOverlayDialogHtml:function(defaultValue,x,y,w,h){
      try {
          defaultValue = Pointer_stringify(defaultValue);
      } catch (e) {
		  alert(e);
		  return;
      }
	  
	  var x0 = x;
	  var y0 = y;
	  var ucanvas = document.getElementById("gameContainer");
	  window.onresize= function(){
		   var offsetx = (document.documentElement.clientWidth - ucanvas.clientWidth) * 0.5;
		   x = x0 + offsetx;
		   var offsety = (document.documentElement.clientHeight - ucanvas.clientHeight) * 0.5- 42;
	       y = y0  + offsety;
	       $('#nativeInputDialogInput').css({left: x + 'px', top: y + 'px', width: w, height: h});
      };
      
	  x = x0+ (document.documentElement.clientWidth -ucanvas.clientWidth) * 0.5;
	  y = y0+ (document.documentElement.clientHeight - ucanvas.clientHeight) * 0.5- 42;
 
      if(!document.getElementById("nativeInputDialog")) {
          // setup html
          var html = '<div id="nativeInputDialog" style="background:transparent; width:100%; height:100%; margin: 0; padding: 0; position: absolute; left: 0; top:0; z-index:888;">' +
              '<input id="nativeInputDialogInput" type="text" style="border: none; background: none; color: white; outline: none; display: block; position: relative; font-size: 0px; ">' +
              '</div>';
          $(document.body).append(html);
          $('#nativeInputDialogInput').keypress(function (event) {
              if (event.keyCode == 13) {
                  $('#nativeInputDialog').hide();
              }
          });
          $('#nativeInputDialogInput').click(function () {
              return false;
          });
 
          $('#nativeInputDialog').click(function () {
              $('#nativeInputDialog').hide();
          });
      }
 
      $('#nativeInputDialogInput').val(defaultValue);
      $('#nativeInputDialogInput').css({left: x + 'px', top: y + 'px', width: w, height: h});
 
      $('#nativeInputDialog').show();
      $('#nativeInputDialogInput').focus();
  },
  
  IsOverlayDialogHtmlActive:function(){
      return $('#nativeInputDialog').is(':visible');
  },
  IsOverlayDialogHtmlCanceled:function(){
      return ($('#nativeInputDialog').is(':visible'));
  },
  GetOverlayHtmlInputFieldValue:function(){
    var returnStr = $('#nativeInputDialogInput').val();
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },
   GetCursortPosition:function  () {
    var dialog = document.getElementById("nativeInputDialogInput");
    var index = dialog.selectionStart;
    return index;
},
 SetCursortPosition:function  (index) {
    var elem = document.getElementById("nativeInputDialogInput");
    var val = elem.value
    var len = val.length
 
    // 超过文本长度直接返回
    if (len < index) return
    setTimeout(function() {
        elem.focus()
        if (elem.setSelectionRange) { // 标准浏览器
            elem.setSelectionRange(index, index)   
        } else { // IE9-
            var range = elem.createTextRange()
            range.moveStart("character", -len)
            range.moveEnd("character", -len)
            range.moveStart("character", index)
            range.moveEnd("character", 0)
            range.select()
        }
    }, 10)
}
	 
};
mergeInto(LibraryManager.library , WebGLInputField );