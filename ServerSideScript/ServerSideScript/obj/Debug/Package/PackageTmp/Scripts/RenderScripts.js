function listAllFiles(strFolderPath) {
    alert('start');
    objFso = new ActiveXObject("Scripting.FileSystemObject");
    alert('objFs');
    objFolder = objFso.GetFolder(strFolderPath);
    alert(strFolderPath);
    alert('objFolder');
    objFiles = new Enumerator(objFolder.files);
    strContent = "<table>";

    for (i = 0; !objFiles.atEnd(); objFiles.moveNext(), i++) {
        strContent += "<tr class = 'tr" +  ((i % 2) ? "True" : "False") + "' ><td>" ;
        strContent += objFiles.item();
        strContent += "</td></tr>";
    }

    strContent += "</table>";
    return strContent;
}