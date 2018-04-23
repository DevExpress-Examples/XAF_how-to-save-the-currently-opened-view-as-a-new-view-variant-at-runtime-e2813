var theme;
var cookie;
var LPcellSize;
function Init(themeName, cookieName){
    theme = themeName;
    cookieName = cookieName;
}
function OnMouseEnter() {
    var separatorButton = document.getElementById('separatorButton');
    separatorButton.className += (' dxsplVSeparatorButtonHover_' + theme);
}
function OnMouseLeave() {
    var separatorButton = document.getElementById('separatorButton');
    separatorButton.className = 'dxsplVSeparatorButton_' + theme;
}
function UpdateSeparatorImage(){
    var LPcell = document.getElementById('LPcell');
    var separatorImage = document.getElementById('separatorImage');
    separatorImage.className = (LPcell.style.display == 'none' ? 'dxWeb_splVCollapseForwardButton_' : 'dxWeb_splVCollapseBackwardButton_');
    separatorImage.className += theme;
}
function OnClick() {
    var LPcell = document.getElementById('LPcell');
    LPcell.style.display = (!LPcell.style.display || LPcell.style.display == 'table-cell') ? 'none' : 'table-cell';
    if (!__aspxIE) {
        LPcell.style.width = LPcellSize + 'px';
    }
    UpdateSeparatorImage();
    _aspxSetCookie(cookie, LPcell.style.display);
    AdjustSize();
}
