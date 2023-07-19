mergeInto(LibraryManager.library, {
  TwinTanglementLoad: function (callback) {
    window.dispatchReactUnityEvent(
      "TwinTanglementLoad",
      Pointer_stringify(callback),
    );
    console.log("TwinTanglement load");
  },
});

mergeInto(LibraryManager.library, {
  TwinTanglementSave: function (data) {
    window.dispatchReactUnityEvent(
      "TwinTanglementSave",
      Pointer_stringify(data),
    );
    console.log("TwinTanglement save")
  },
});
