export async function beforeStart(wasmOptions, extensions) {
    try {
        console.log("Restoring dll headers");
        //This is to support custom Blazor.start with a custom loadBootResource 
        var existingLoadBootResouce = wasmOptions.loadBootResource;

        wasmOptions.loadBootResource = function (type, name, defaultUri, integrity) {
            var existingLoaderResponse = null;
            if (existingLoadBootResouce) {
                existingLoaderResponse = existingLoadBootResouce(type, name, defaultUri, integrity);
            }
            if (type != "assembly") {
                if (existingLoaderResponse) {
                    return existingLoaderResponse;
                }
                else {
                    return defaultUri;
                }
            }
                

            console.log("Restoring dll header: " + name);
            var fetchPromise = null;
            if (existingLoaderResponse) {
                if (typeof existingLoaderResponse == "string") {
                    fetchPromise = fetch(existingLoaderResponse, {
                        cache: 'no-cache',
                        integrity: integrity,
                    });
                }
                else {
                    fetchPromise = existingLoaderResponse;
                }
            }
            else {
                fetchPromise = fetch(defaultUri, {
                    cache: 'no-cache',
                    integrity: integrity,
                });
            }
            
            var resp = fetchPromise.then(response => {
                return response.arrayBuffer().then(buffer => {
                    return { buffer: buffer, headers: response.headers };
                });
            }).then(responseResult => {
                var data = new Uint8Array(responseResult.buffer);
                data[0] = 77; //This restores header from BZ to MZ
                var resp = new Response(data, { "status": 200, headers: responseResult.headers });
                return resp;
            });

           
            return resp;

        }
    } catch (error) {
        console.log(error);
    }
}

export async function afterStarted(blazor) {
}
