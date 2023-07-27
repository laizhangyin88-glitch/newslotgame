var GetPageInfoPlugin = {
  /* definition of contextId exists in BiEvent.js @ WebGLTemplate */
  GetPageContextId: function() {
    var size = lengthBytesUTF8(contextId) + 1;
    var buffer = _malloc(size);
    stringToUTF8(contextId, buffer, size);
    return buffer;
  },

  /* definition of SERVER_BASE_URL exists in config.js @ WebGLTemplate */
  GetServerBaseUrlConfig: function() {
    var size = lengthBytesUTF8(SERVER_BASE_URL) + 1;
    var buffer = _malloc(size);
    stringToUTF8(SERVER_BASE_URL, buffer, size);
    return buffer;
  },

  /* definition of CHATTING_URL exists in config.js @ WebGLTemplate */
  GetChattingUrlConfig: function() {
    var size = lengthBytesUTF8(CHATTING_URL) + 1;
    var buffer = _malloc(size);
    stringToUTF8(CHATTING_URL, buffer, size);
    return buffer;
  },

  /* definition of APP_DOWNLOAD_URL exists in config.js @ WebGLTemplate */
  GetAppDonwloadUrlConfig: function() {
    var size = lengthBytesUTF8(APP_DOWNLOAD_URL) + 1;
    var buffer = _malloc(size);
    stringToUTF8(APP_DOWNLOAD_URL, buffer, size);
    return buffer;
  }
};

mergeInto(LibraryManager.library, GetPageInfoPlugin);
