window.clickLink = (url) => {
    // Check if the URL is a valid URL
    if (isValidUrl(url)) {
        window.open(url, '_blank');
    } else {
        console.error('Invalid URL:', url);
    }
}

const isValidUrl = (url) => {
    try {
        new URL(url);
        return true;
    } catch (e) {
        return false;
    }
};