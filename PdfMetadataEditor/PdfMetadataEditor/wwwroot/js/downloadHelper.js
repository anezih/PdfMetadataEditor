window.downloadFile = async (fileName, blobUrl) => {
    const anchorElement = document.createElement('a');
    anchorElement.href = blobUrl;
    anchorElement.download = fileName ?? '';
    anchorElement.click();
    anchorElement.remove();
}