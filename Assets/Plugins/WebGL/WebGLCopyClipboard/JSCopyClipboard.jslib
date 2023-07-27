var CopyClipboardPlugin = {
    JSCopyClipboard: function(copyText) {
        var clipboardBuffer = document.createElement('textarea');
        clipboardBuffer.style.cssText = 'position:fixed; top:-10px; left:-10px; height:0; width:0; opacity:0;';
        document.body.appendChild(clipboardBuffer);

        var copyButton = document.createElement('button');
        copyButton.innerHTML = 'Copy "' + copyText + '" to the system clipboard';
        copyButton.onclick = function() {
          clipboardBuffer.focus();
          clipboardBuffer.value = copyText;
          clipboardBuffer.setSelectionRange(0, clipboardBuffer.value.length);
          var succeeded;
          try { succeeded = document.execCommand('copy'); } catch (e) {}
          alert(succeeded ? 'Copied successfully' : 'Error: current browser does not fully support execCommand functionality or this event handler was not initiated by the user');
        }
        document.body.appendChild(copyButton);
    }
}
mergeInto(LibraryManager.library, CopyClipboardPlugin);
