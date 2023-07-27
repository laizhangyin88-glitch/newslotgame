function addBookmark() {
  var bookmarkURL = "https://apps.facebook.com/clubvegas";
  var bookmarkTitle = document.title;

  if (window.sidebar && window.sidebar.addPanel) {
      // Firefox <=22
      window.sidebar.addPanel(bookmarkTitle, bookmarkURL, '');
  } else if ((window.sidebar && /Firefox/i.test(navigator.userAgent)) || (window.opera && window.print)) {
      // Firefox 23+ and Opera <=14
      var bookmarkButton = document.getElementById('bookmark-link');
      bookmarkButton.href = bookmarkURL;
      bookmarkButton.title = bookmarkTitle;
      bookmarkButton.rel = 'sidebar';
  } else if (window.external && ('AddFavorite' in window.external)) {
      // IE Favorites
      window.external.AddFavorite(bookmarkURL, bookmarkTitle);
  } else {
      // Other browsers (mainly WebKit & Blink - Safari, Chrome, Opera 15+)
      alert('Press ' + (/Mac/i.test(navigator.userAgent) ? 'Cmd' : 'Ctrl') + '+D to bookmark this page.');
  }
}
