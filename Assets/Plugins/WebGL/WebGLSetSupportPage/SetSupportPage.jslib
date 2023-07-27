var SetSupportPagePlugin = {
  SetSupportPage: function (url) {
      SUPPORT_PAGE_URL = Pointer_stringify(url);
      return;
    },
};

mergeInto(LibraryManager.library, SetSupportPagePlugin);
