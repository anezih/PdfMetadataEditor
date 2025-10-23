import scribe from '../scribejs/scribe.js'

var _objRef;

function ToArrayOfArrayBuffer(data) {
    let buffer;

    if (data instanceof ArrayBuffer) {
        buffer = data;
    }
    else if (ArrayBuffer.isView(data)) {
        buffer = data.buffer;
    }
    else {
        throw new TypeError("Expected byte[] like object.");
    }
    return [buffer];
}

function ProgressHandler(msg) {
    if (_objRef != null) {
        _objRef.invokeMethodAsync("OcrProgressHandler", msg.n, msg.type, msg.info.engineName);
    }
}

export function getScribe(workerN, dotnetObjRef) {
    _objRef = dotnetObjRef;
    scribe.opt.displayMode = "invis";
    scribe.opt.workerN = workerN
    scribe.opt.progressHandler = ProgressHandler;
    return scribe;
}

export function ImportPdf(pdfData) {
    let pdf = ToArrayOfArrayBuffer(pdfData);

    const SortedInputFiles = {
        pdfFiles: pdf,
    };
    scribe.importFiles(SortedInputFiles);
}