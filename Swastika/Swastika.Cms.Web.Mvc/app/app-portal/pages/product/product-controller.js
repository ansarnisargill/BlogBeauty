﻿'use strict';
app.controller('ProductController', ['$scope', '$rootScope', 'ngAppSettings', '$routeParams', '$timeout', '$location', 'AuthService', 'ProductServices', 'ngAppSettings',
    function ($scope, $rootScope, ngAppSettings, $routeParams, $timeout, $location, authService, productServices) {
        $scope.request = angular.copy(ngAppSettings.request);

        $scope.activedProduct = null;
        $scope.relatedProducts = [];
        $rootScope.isBusy = false;
        $scope.data = {
            pageIndex: 0,
            pageSize: 1,
            totalItems: 0,
            items: []
        };
        $scope.errors = [];

        $scope.range = function (max) {
            var input = [];
            for (var i = 1; i <= max; i += 1) input.push(i);
            return input;
        }

        $scope.loadProduct = async function () {
            $rootScope.isBusy = true;
            var id = $routeParams.id;
            var response = await productServices.getProduct(id, 'be');
            if (response.isSucceed) {
                $scope.activedProduct = response.data;
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
        $scope.loadProducts = async function (pageIndex) {
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
            $rootScope.isBusy = true;
            var resp = await productServices.getProducts($scope.request);
            if (resp && resp.isSucceed) {

                ($scope.data = resp.data);
                //$("html, body").animate({ "scrollTop": "0px" }, 500);
                $.each($scope.data.items, function (i, product) {

                    $.each($scope.activedProducts, function (i, e) {
                        if (e.productId === product.id) {
                            product.isHidden = true;
                        }
                    })
                })
                $rootScope.isBusy = false;
                $scope.$apply();
            }
            else {
                if (resp) { $rootScope.showErrors(resp.errors); }
                $rootScope.isBusy = false;
                $scope.$apply();
            }
        };

        $scope.removeProduct = function (id) {
            $rootScope.showConfirm($scope, 'removeProductConfirmed', [id], null, 'Remove Product', 'Are you sure');
        };

        $scope.removeProductConfirmed = async function (id) {
            $rootScope.isBusy = true;
            var result = await productServices.removeProduct(id);
            if (result.isSucceed) {
                $scope.loadProducts();
            }
            else {
                $rootScope.showMessage('failed');
                $rootScope.isBusy = false;
                $scope.$apply();
            }
        };

        $scope.saveProduct = async function (product) {
            product.content = $('.editor-content.content').val();
            product.excerpt = $('.editor-content.excerpt').val();
            $rootScope.isBusy = true;
            var resp = await productServices.saveProduct(product);
            if (resp && resp.isSucceed) {
                $scope.activedProduct = resp.data;
                $rootScope.showMessage('Thành công', 'success');
                $rootScope.isBusy = false;
                $scope.$apply();
                //$location.path('/backend/product/details/' + resp.data.id);
            }
            else {
                if (resp) { $rootScope.showErrors(resp.errors); }
                $rootScope.isBusy = false;
                $scope.$apply();
            }
        };

        $scope.preview = function (item) {
            $rootScope.preview('product', item, item.title, 'modal-lg');
        };
    }]);
