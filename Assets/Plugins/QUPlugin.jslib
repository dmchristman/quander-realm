mergeInto(LibraryManager.library, {
  QupcakesGameLoaded: function (callback) {
    window.dispatchReactUnityEvent(
      "QupcakesGameLoaded",
      Pointer_stringify(callback)
    );
  },
});

mergeInto(LibraryManager.library, {
  QupcakesGameSaved: function (data) {
    window.dispatchReactUnityEvent(
      "QupcakesGameSaved",
      Pointer_stringify(data)
    );
    console.log("123");
  },
});
