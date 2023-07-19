mergeInto(LibraryManager.library, {
  QueueBitsGameSaved: function (data) {
    window.dispatchReactUnityEvent(
      "QueueBitsGameSaved",
      Pointer_stringify(data)
    );
    console.log("123");
  },
});