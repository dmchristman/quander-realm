mergeInto(LibraryManager.library, {
  GameLoaded: function (callback) {
    window.dispatchReactUnityEvent(
      "GameLoaded",
      Pointer_stringify(callback)
    );
  },
});

mergeInto(LibraryManager.library, {
  SendData: function (dataToSend) {
    window.dispatchReactUnityEvent(
      "SendData",
      Pointer_stringify(dataToSend)
    );
  },
});

