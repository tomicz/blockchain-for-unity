mergeInto(LibraryManager.library, {
    
    // Establish connection between MetaMask and Unity WebGL build.
    Connect: async function(chainId, gameObjectName, callback, fallback){

        const parsedGameObjectName = UTF8ToString(gameObjectName);
        const parsedCallback = UTF8ToString(callback);
        const parsedFallback = UTF8ToString(fallback);
        const parsedChainId = UTF8ToString(chainId);
        
        try{
            const accounts = await window.ethereum.request({method: "eth_requestAccounts"});

            if(window.ethereum.request({method: "eth_chainId"}) != chainId){
                try{
                    await window.ethereum.request({method: "wallet_switchEthereumChain", params: [{chainId: parsedChainId}]});
                }
                catch(e){
                    if(e.code == 4902){
                        await window.ethereum.request({method: "wallet_addEthereumChain", params: [parsedChainId]}); 
                    } else{
                        SendMessage(parsedGameObjectName, parsedFallback, e.message);     
                        return null;
                    }
                }
            }

            SendMessage(parsedGameObjectName, parsedCallback, accounts[0]);
        }
        catch(e){
            SendMessage(parsedGameObjectName, parsedFallback, e.message)
        }
    },
    
    CallRpc: async function(message, gameObject, callback, error){
        const request = UTF8ToString(message); 
        const parsedGameObject = UTF8ToString(gameObject);
        const parsedCallback = UTF8ToString(callback); 
        const parsedError = UTF8ToString(error);

        try{
            const response = await window.ethereum.request(request);
            let rpc = {
                jsonrpc: "2.0",
                result: response,
                id: request.Id,
                error: null 
            }

            var json = JSON.stringify(rpc);
            SendMessage(gameObject, parsedCallback, json);
        }
        catch(e){
            let rpc = {
                jsonrpc: "2.0",
                id: request.id,
                error: {
                    message: e.message
                }
            }
            var json = JSON.stringify(rpc);
            SendMessage(gameObject, parsedError, json);
        }
    },
    
    // Check if MetaMask plugin is available in your browser.
    IsMetaMask: function(){
        return window.ethereum.isMetaMask;
    }

});
