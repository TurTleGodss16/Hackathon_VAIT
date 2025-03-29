window.downloadFile = (fileName, contentType, byteArray) => {
    const blob = new Blob([new Uint8Array(byteArray)], { type: contentType });
    const link = document.createElement("a");
    link.href = window.URL.createObjectURL(blob);
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};
