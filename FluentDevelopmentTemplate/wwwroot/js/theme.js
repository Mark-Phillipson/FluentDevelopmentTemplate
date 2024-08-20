function setTheme(theme) {
    document.documentElement.setAttribute('data-bs-theme', theme);
    localStorage.setItem('theme', theme);
}

function getTheme() {
    return localStorage.getItem('theme') || 'dark';
}

document.addEventListener("DOMContentLoaded", function () {
    const theme = getTheme();
    setTheme(theme);
});

