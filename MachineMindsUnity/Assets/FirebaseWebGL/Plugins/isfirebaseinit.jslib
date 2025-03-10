mergeInto(LibraryManager.library, {
  IsFirebaseInitialized: function () {
    // Check if the function exists in window context
    if (typeof window.IsFirebaseInitialized === "function") {
      return window.IsFirebaseInitialized();
    }
    // Fallback if function doesn't exist
    return false;
  },
});
