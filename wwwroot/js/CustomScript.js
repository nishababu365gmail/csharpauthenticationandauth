function confirmDelete(uniqueid, isDeleteClicked) {
    
    var deletespan = 'deletespan_' + uniqueid;
    var confirmdeletespan = 'confirmdeletespan_' + uniqueid;
    if (isDeleteClicked) {
        $('#' + deletespan).hide();
        $('#' + confirmdeletespan).show();
    }
    else {
        $('#' + deletespan).show();
        $('#' + confirmdeletespan).hide();
    }
}