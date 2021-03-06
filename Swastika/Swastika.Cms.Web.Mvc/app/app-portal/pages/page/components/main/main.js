﻿
app.component('pageMain', {
    templateUrl: '/app-portal/pages/page/components/main/main.html',
    controller: ['$rootScope', '$scope', 'ngAppSettings', function ($rootScope, $scope) {
        var ctrl = this;
        ctrl.settings = $rootScope.settings;
        ctrl.setPageType = function (type) {
            ctrl.page.type = $index;
        }
        ctrl.generateSeo = function () {
            if (ctrl.page) {

                if (ctrl.page.seoName == null || ctrl.page.seoName == '') {
                    ctrl.page.seoName = $rootScope.generateKeyword(ctrl.page.title, '-');
                }
                if (ctrl.page.urlAlias.alias == null || ctrl.page.urlAlias.alias == '') {
                    ctrl.page.urlAlias.alias = $rootScope.generateKeyword(ctrl.page.title, '-');
                }
            }
        }
    }],
    bindings: {
        page: '=',
        onDelete: '&',
        onUpdate: '&'
    }
});