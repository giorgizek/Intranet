$.fn.select2.defaults.set('theme', 'bootstrap');
$.fn.select2.defaults.set('width', '100%');
$.fn.cascadeDropdown.defaults.optionHtml = '&nbsp;';
//$.fn.select2.defaults.set('allowClear', true);


$(function () {
    var route = $.getRoute();
    switch (route.controller) {
    case 'AdminEmployees':
        $('#DepartmentId').select2().cascadeDropdown();
        $('#JobTitleId').select2();
        break;

    case 'Employees':
        $('#DepartmentId').select2().cascadeDropdown().autoFilter();
        $('#JobTitleId').select2().autoFilter();
        $('#BranchId').select2().autoFilter();
        break;


    case 'AdminProducts':
        $('#CategoryId').select2().cascadeDropdown();
        $('#SubCategoryId').select2();
        break;

    }
});