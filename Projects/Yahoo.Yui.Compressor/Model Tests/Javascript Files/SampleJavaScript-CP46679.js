controls.SearchCombo = Ext.extend(Ext.form.ComboBox, {
    forceSelection: true,
    loadingText: 'Searching...',
    minChars: 3,
    mode: 'remote',
    msgTarget: 'side',
    queryDelay: 300,
    queryParam: 'q',
    selectOnFocus: true,
    typeAhead: false
}); 