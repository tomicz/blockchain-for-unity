mergeInto(LibraryManager.library, {
  // Establish connection between MetaMask and Unity WebGL build.
  Connect: async function (chainId, gameObjectName, callback, fallback) {
    const parsedGameObjectName = UTF8ToString(gameObjectName);
    const parsedCallback = UTF8ToString(callback);
    const parsedFallback = UTF8ToString(fallback);
    const parsedChainId = UTF8ToString(chainId);

    try {
      const accounts = await window.ethereum.request({
        method: "eth_requestAccounts",
      });

      if (window.ethereum.request({ method: "eth_chainId" }) != chainId) {
        try {
          await window.ethereum.request({
            method: "wallet_switchEthereumChain",
            params: [{ chainId: parsedChainId }],
          });
        } catch (e) {
          if (e.code == 4902) {
            await window.ethereum.request({
              method: "wallet_addEthereumChain",
              params: [parsedChainId],
            });
          } else {
            SendMessage(parsedGameObjectName, parsedFallback, e.message);
            return null;
          }
        }
      }

      SendMessage(parsedGameObjectName, parsedCallback, accounts[0]);
    } catch (e) {
      SendMessage(parsedGameObjectName, parsedFallback, e.message);
    }
  },

  CallRpc: async function (message, gameObject, callback, error) {
    const request = UTF8ToString(message);
    const parsedGameObject = UTF8ToString(gameObject);
    const parsedCallback = UTF8ToString(callback);
    const parsedError = UTF8ToString(error);

    try {
      // Parse the JSON string into an object
      const parsedRequest = JSON.parse(request);
      console.log("Parsed RPC Request:", parsedRequest);

      // Extract method and params for ethereum.request (not the full JSON-RPC format)
      const ethereumRequest = {
        method: parsedRequest.method,
        params: parsedRequest.params || [],
      };
      console.log("Ethereum Request:", ethereumRequest);

      const response = await window.ethereum.request(ethereumRequest);

      let rpc = {
        jsonrpc: "2.0",
        result: response,
        id: parsedRequest.id,
        error: null,
      };

      var json = JSON.stringify(rpc);
      console.log("RPC Response:", json);
      SendMessage(parsedGameObject, parsedCallback, json);
    } catch (e) {
      console.error("RPC Error:", e);
      let rpc = {
        jsonrpc: "2.0",
        id: request ? JSON.parse(request).id : null,
        error: {
          message: e.message,
          code: e.code || -1,
        },
      };
      var json = JSON.stringify(rpc);
      SendMessage(parsedGameObject, parsedError, json);
    }
  },

  // Check if MetaMask plugin is available in your browser.
  IsMetaMask: function () {
    return window.ethereum.isMetaMask;
  },
});
