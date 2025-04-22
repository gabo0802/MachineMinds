mergeInto(LibraryManager.library, {
  GetCollection: function(collectionPath, objectName, callback, fallback) {
    // Convert C# string parameters to JS strings
    var collection = UTF8ToString(collectionPath);
    var obj = UTF8ToString(objectName);
    var successCallback = UTF8ToString(callback);
    var errorCallback = UTF8ToString(fallback);

    try {
      firebase.firestore().collection(collection).get().then(function(querySnapshot) {
        var data = [];
        querySnapshot.forEach(function(doc) {
          data.push(doc.data());
        });
        unityInstance.SendMessage(obj, successCallback, JSON.stringify(data));
      }).catch(function(error) {
        unityInstance.SendMessage(obj, errorCallback, error.message);
      });
    } catch (error) {
      unityInstance.SendMessage(obj, errorCallback, "Firebase not initialized: " + error.message);
    }
  },

  AddDocument: function(collectionPath, value, objectName, callback, fallback) {
    var collection = UTF8ToString(collectionPath);
    var data = UTF8ToString(value);
    var obj = UTF8ToString(objectName);
    var successCallback = UTF8ToString(callback);
    var errorCallback = UTF8ToString(fallback);

    try {
      firebase.firestore().collection(collection).add(JSON.parse(data))
        .then(function(docRef) {
          unityInstance.SendMessage(obj, successCallback, docRef.id);
        })
        .catch(function(error) {
          unityInstance.SendMessage(obj, errorCallback, error.message);
        });
    } catch (error) {
      unityInstance.SendMessage(obj, errorCallback, "Firebase not initialized: " + error.message);
    }
  }
});