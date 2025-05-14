window.changeAntThemeToDark = async () => {
    document
        .querySelector('link[rel="stylesheet"][href*="ant-design-blazor"]')
        .setAttribute("href", "_content/AntDesign/css/ant-design-blazor.dark.css")
}

window.changeAntThemeToLight = async () => {
    document
        .querySelector('link[rel="stylesheet"][href*="ant-design-blazor"]')
        .setAttribute("href", "_content/AntDesign/css/ant-design-blazor.css")
}

window.isDark = async () => {
    return window.matchMedia("(prefers-color-scheme: dark)").matches;
}