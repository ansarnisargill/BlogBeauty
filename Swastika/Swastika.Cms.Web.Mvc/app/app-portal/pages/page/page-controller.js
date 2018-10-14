﻿'use strict';
app.controller('PageController', ['$scope', '$rootScope', 'ngAppSettings', '$routeParams', '$timeout', '$location', 'AuthService', 'PageServices',
    function ($scope, $rootScope, ngAppSettings, $routeParams, $timeout, $location, authService, pageServices) {
        $scope.request = angular.copy(ngAppSettings.request);
        $scope.activedPage = null;

        $scope.relatedPages = [];

        $rootScope.isBusy = false;

        $scope.data = {
            pageIndex: 0,
            pageSize: 1,
            totalItems: 0
        };

        $scope.errors = [];

        $scope.range = function (max) {
            var input = [];
            for (var i = 1; i <= max; i += 1) input.push(i);
            return input;
        };

        $scope.getPage = async function (id) {
            $rootScope.isBusy = true;
            var resp = await pageServices.getPage(id, 'be');
            if (resp && resp.isSucceed) {
                $scope.activedPage = resp.data;
                $rootScope.initEditor();
                $rootScope.isBusy = false;
                $scope.$apply();
            }
            else {
                if (resp) { $rootScope.showErrors(resp.errors); }
                $rootScope.isBusy = false;
                $scope.$apply();
            }
        };

        $scope.initPage = function () {
            if (!$rootScope.isBusy) {
                $rootScope.isBusy = true;
                pageServices.initPage('be').then(function (response) {
                    if (response.isSucceed) {
                        $scope.activedPage = response.data;
                        $rootScope.initEditor();
                    }
                    $rootScope.isBusy = false;
                    $rootScope.isBusy = false;
                    $scope.$apply();
                }).error(function (a, b, c) {
                    errors.push(a, b, c);
                    $rootScope.isBusy = false;
                });
            }
        };

        $scope.loadPage = async function () {
            $rootScope.isBusy = true;
            var id = $routeParams.id;
            $rootScope.isBusy = true;
            var response = await pageServices.getPage(id, 'be');
            if (response.isSucceed) {
                $scope.activedPage = response.data;
                $rootScope.initEditor();
                $rootScope.isBusy = false;
                $scope.$apply();
            }
            else {
                $rootScope.showErrors(response.errors);
                $rootScope.isBusy = false;
                $scope.$apply();
            }
        };

        $scope.loadPageDatas = async function () {
            $rootScope.isBusy = true;
            var id = $routeParams.id;
            var response = await pageServices.getPage(id, 'fe');
            if (response.isSucceed) {
                $scope.activedPage = response.data;
                $rootScope.initEditor();
                $rootScope.isBusy = false;
                $scope.$apply();
            }
            else {
                $rootScope.showErrors(response.errors);
                $rootScope.isBusy = false;
                $scope.$apply();
            }
        };

        $scope.loadPages = async function (pageIndex) {
            if (pageIndex !== undefined) {
                $scope.request.pageIndex = pageIndex;
            }
            if ($scope.request.fromDate !== null) {
                var d = new Date($scope.request.fromDate);
                $scope.request.fromDate = d.toISOString();
            }
            if ($scope.request.toDate !== null) {
                var d = new Date($scope.request.toDate);
                $scope.request.toDate = d.toISOString();
            }
            var resp = await pageServices.getPages($scope.request);
            if (resp && resp.isSucceed) {

                ($scope.data = resp.data);
                $.each($scope.data.items, function (i, page) {

                    $.each($scope.activedPages, function (i, e) {
                        if (e.pageId === page.id) {
                            page.isHidden = true;
                        }
                    });
                });
                $rootScope.isBusy = false;
                $scope.$apply();
            }
            else {
                if (resp) { $rootScope.showErrors(resp.errors); }
                $rootScope.isBusy = false;
                $scope.$apply();
            }
        };

        $scope.removePage = function (id) {
            $rootScope.showConfirm($scope, 'removePageConfirmed', [id], null, 'Remove Page', 'Are you sure');
        };

        $scope.removePageConfirmed = async function (id) {
            $rootScope.isBusy = true;
            var result = await pageServices.removePage(id);
            if (result.isSucceed) {
                $scope.loadPages();
            }
            else {
                $rootScope.showMessage('failed');
                $rootScope.isBusy = false;
                $scope.$apply();
            }
        };


        $scope.savePage = async function (page) {
            page.content = $('.editor-content').val();
            $rootScope.isBusy = true;
            var resp = await pageServices.savePage(page);
            if (resp && resp.isSucceed) {
                $scope.activedPage = resp.data;
                $rootScope.showMessage('success', 'success');
                $rootScope.isBusy = false;
                $location.path('/backend/page/list');
                $scope.$apply();
            }
            else {
                if (resp) { $rootScope.showErrors(resp.errors); }
                $rootScope.isBusy = false;
                $scope.$apply();
            }
        };

    }]);
