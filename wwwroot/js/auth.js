window.authHelper = {
    setUser: function (userJson) {
        localStorage.setItem("user", userJson);
        window.dispatchEvent(new Event("userChanged"));
    },

    getUser: function () {
        return localStorage.getItem("user");
    },

    clearUser: function () {
        localStorage.removeItem("user");
        window.dispatchEvent(new Event("userChanged"));
    },

    registerUserChangedCallback: function (dotNetObjRef, methodName) {
        window.addEventListener("userChanged", () => {
            dotNetObjRef.invokeMethodAsync(methodName);
        });
    }
};
