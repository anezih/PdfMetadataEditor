import * as mupdf from "./mupdf.js"

export function initialize(data) {
    var doc = mupdf.Document.openDocument(data, "application/pdf")
    return doc;
}