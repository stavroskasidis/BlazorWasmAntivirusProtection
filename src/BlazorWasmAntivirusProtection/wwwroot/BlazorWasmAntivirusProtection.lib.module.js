export async function beforeStart(wasmOptions, extensions) {
    if (!extensions || !extensions.avpsettings) {
        return;
    }

    try {
        const integrity = extensions.avpsettings['avp-settings.json'];
        const settingsResponse = await fetch('avp-settings.json', { integrity: integrity, cache: 'no-cache' });
        const settings = await settingsResponse.json();

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
                if (settings.obfuscationMode == 1) {//Changed Headers
                    console.log("Restoring binary header: " + name);
                    data[0] = 77; //This restores header from BZ to MZ
                }
                else if (settings.obfuscationMode == 2) { //Xored dll
                    console.log("Restoring binary file Xor: " + name);
                    var key = settings.xorKey;
                    for (let i = 0; i < data.length; i++)
                        data[i] = data[i] ^ key.charCodeAt(i % key.length); //This reverses the Xor'ing of the dll
                }
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
