import scribe from '../scribejs/scribe.js'

export function getScribe() {
    scribe.opt.displayMode = "invis";
    return scribe;
}

function ToArrayOfArrayBuffer(data) {
    let buffer;

    if (data instanceof ArrayBuffer)
    {
        buffer = data;
    }
    else if (ArrayBuffer.isView(data))
    {
        buffer = data.buffer;
    }
    else
    {
        throw new TypeError("Expected byte[] like object.");
    }

    return [buffer];
}

export function ImportPdf(pdfData) {
    let pdf = ToArrayOfArrayBuffer(pdfData);

    const SortedInputFiles = {
        pdfFiles: pdf,
    };
    scribe.importFiles(SortedInputFiles);
}