var app = angular.module('PetQuest', ['ngRoute', 'LocalStorageModule']);
console.log("test");
app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptorService');
});

app.config(function ($routeProvider) {
    console.log("ran");
    $routeProvider.when("/home", {
        controller: "homeController",
        templateUrl: "Scripts/angular-views/home.html"
    });
    $routeProvider.when("/pets", {
        controller: "petsController",
        templateUrl: "Scripts/angular-views/pets.html"
    });
    $routeProvider.when("/pet/:petid", {
        controller: "petProfileController",
        templateUrl: "Scripts/angular-views/pet_profile.html"
    });

    $routeProvider.when("/report_found", {
        controller: "foundPetController",
        templateUrl: "Scripts/angular-views/report_found.html"
    });

    $routeProvider.when("/profile", {
        controller: "profileController",
        templateUrl: "Scripts/angular-views/profile.html"
    });

    $routeProvider.when("/pet_profile", {
        controller: "profileController",
        templateUrl: "Scripts/angular-views/profile.html"
    });

    $routeProvider.otherwise({ redirectTo: "/home" });
    console.log('route given');
});

app.controller('homeController', ['$scope'], function ($scope) {

});

app.controller('petProfileController', ['$scope', '$routeParams', 'petsService', function ($scope, $routeParams, petsService) {
    var petID = $routeParams.petid;
    $scope.editPet = 0;
    console.log(petID);

    petsService.getPet(petID).then(function (results) {
        $scope.pet = results.data;
        $scope.petUpdate.Location = results.data.Location;
        $scope.petUpdate.Name = results.data.Name;
        $scope.petUpdate.Image = results.data.Image;
        $scope.petUpdate.Species = results.data.Species;
        $scope.petUpdate.Breed = results.data.Breed;

        petsService.getFoundInfo(results.data.FoundID).then(function (res){
            $scope.foundInfo = res.data;
            
        });
        
    });

    $scope.petUpdate = {
        ID: petID,
        Name: $scope.pet.Name,
        Image: $scope.pet.Image,
        Breed: $scope.pet.Breed,
        Species: $scope.pet.Species,
        Location: $scope.pet.Location
    };
    
    
    console.log($scope.pet);
    console.log($scope.pet.Location);
 

 

    $scope.edit = function (id) {
        console.log("test");
        $scope.editPet = id;
    };

    

    $scope.updatePet = function () {
        console.log($scope.petUpdate);
        petsService.updatePet(petID, $scope.petUpdate).then(function (results) {
            console.log(results);
            location.reload();
        });
    };

    $scope.deletePet = function() {
        petsService.deletePet(petID).then(function (results) {
            console.log(results);
            location.reload();
        })};

}]);

app.controller('foundPetController', ['$scope', 'petsService', function ($scope, petsService) {

    $scope.pets = [];
    $scope.order = 'ID';
    $scope.sortType = 'PublishedDate';
    $scope.sortReverse = false;
    $scope.searchPets = '';

    $scope.petSelected = 0;

    petsService.getPets().then(function (results) {

        $scope.pets = results.data;

    }, function (error) {
        alert(error.data.message);
    });

    $scope.petData = {
        LocationFound: ""
    };

    $scope.updateSelection = function (id)
    {
        $scope.petSelected = id;
        console.log($scope.petSelected);
    }

    $scope.petfound = function () {

        console.log('func hit');
        console.log($scope.petSelected);
        $scope.petData.PetID = $scope.petSelected;
        petsService.foundPet($scope.petData).then(function (response) {
            
            $scope.savedSuccessfully = true;
            $scope.message = "Pet has been found successfully.";
            alert("Pet saved.");

        },
         function (response) {
             var errors = [];
             for (var key in response.data.modelState) {
                 for (var i = 0; i < response.data.modelState[key].length; i++) {
                     errors.push(response.data.modelState[key][i]);
                 }
             }
             $scope.message = "Failed to find pet due to:" + errors.join(' ');
         });
    };

}]);
app.controller('petController', ['$scope', '$location', 'authService', function ($scope, $location, authService) {

    console.log("hit");

}]);

app.controller('loginController', ['$scope', '$location', 'authService', function ($scope, $location, authService) {
    console.log("logincontroller");
    $scope.loginData = {
        username: "",
        password: "",
        grant_type: "password"
    };

    $scope.message = "";

    $scope.login = function () {

        authService.login($scope.loginData).then(function (response) {

            $location.path('/');

        },
         function (err) {
             $scope.message = err.error_description;
         });
    };

}]);
app.controller('registerController', ['$scope', '$location', '$timeout', 'authService', function ($scope, $location, $timeout, authService) {
    console.log('controller hit');
    $scope.savedSuccessfully = false;
    $scope.message = "";

    $scope.registration = {
        Name: "",
        Email: "",
        Password: "",
        ConfirmPassword: ""
    };

    $scope.signUp = function () {
        console.log('func hit');
        authService.saveRegistration($scope.registration).then(function (response) {

            $scope.savedSuccessfully = true;
            $scope.message = "User has been registered successfully.";
            alert("User saved.");

        },
         function (response) {
             var errors = [];
             for (var key in response.data.modelState) {
                 for (var i = 0; i < response.data.modelState[key].length; i++) {
                     errors.push(response.data.modelState[key][i]);
                 }
             }
             $scope.message = "Failed to register user due to:" + errors.join(' ');
         });
    };



}]);

app.factory('authService', ['$http', '$q', 'localStorageService', function ($http, $q, localStorageService) {

    var serviceBase = '/';
    var authServiceFactory = {};

    var _authentication = {
        isAuth: false,
        isAdmin: false,
        userName: ""
    };

    var _saveRegistration = function (registration) {

        _logOut();

        return $http.post(serviceBase + 'api/account/register', registration).then(function (response) {
            return response;
        });

    };

    var _login = function (loginData) {

        var data = "grant_type=password&username=" + loginData.username + "&password=" + loginData.password;

        var deferred = $q.defer();

        $http.post(serviceBase + 'token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).success(function (response) {
            console.log("storage set");
            localStorageService.set('authorizationData', { token: response.access_token, userName: loginData.username, roles: response.roles, ID: response.ID });

            _authentication.isAuth = true;
            console.log("auth set");
            _authentication.userName = loginData.username;
            _authentication.ID = response.ID;
            //if(response.roles)
            var arr = JSON.parse(response.roles);
            console.log(arr[1]);

            if (arr == "Administrator")
                _authentication.isAdmin = 1;
            else
                _authentication.isAdmin = 0;

            deferred.resolve(response);

        }).error(function (err, status) {
            _logOut();
            deferred.reject(err);
        });

        return deferred.promise;

    };

    var _logOut = function () {

        localStorageService.remove('authorizationData');

        _authentication.isAuth = false;
        _authentication.isAdmin = 0;
        _authentication.userName = "";
        _authentication.ID = 0;

    };

    var _fillAuthData = function () {

        var authData = localStorageService.get('authorizationData');
        if (authData) {
            _authentication.isAuth = true;
            _authentication.userName = authData.userName;
            if (JSON.parse(authData.roles)[1] == "Administrator")
                _authentication.isAdmin = 1;
            else
                _authentication.isAdmin = 0;
            _authentication.ID = authData.ID;
        }

    }

    authServiceFactory.saveRegistration = _saveRegistration;
    authServiceFactory.login = _login;
    authServiceFactory.logOut = _logOut;
    authServiceFactory.fillAuthData = _fillAuthData;
    authServiceFactory.authentication = _authentication;

    return authServiceFactory;
}]);

app.run(['authService', function (authService) {
    authService.fillAuthData();
}]);

app.controller('indexController', ['$scope', '$location', 'authService', function ($scope, $location, authService) {

    $scope.logOut = function () {
        authService.logOut();
        $location.path('/home');
    }

    $scope.authentication = authService.authentication;

}]);

app.factory('petsService', ['$http', function ($http) {

    var petsServiceFactory = {};

    var _getPets = function () {

        return $http.get('/api/Pets').then(function (results) {
            return results;
        });
    };

    var _getPet = function (petid) {

        return $http.get('/api/Pets/' + petid ).then(function (results) {
            return results;
        });
    };

    var _getLostPets = function () {

        return $http.get('/api/Pets/Lost').then(function (results) {
            return results;
        });
    };

    var _getFoundInfo = function (foundid) {
        return $http.get('/api/FoundPets/' + foundid).then(function (response) {
            return response;
        });
    }

    var _savePet = function (petdata) {
        return $http.post(serviceBase + 'api/Pets', petdata).then(function (response) {
            return response;
        });
    }

    var _foundPet = function (petdata) {
        return $http.post('/api/FoundPets', petdata).then(function (response) {
            return response;
        });
    }

    var _deletePet = function (petid) {
        return $http.delete('/api/Pets/' + petid).then(function (response) {
            return response;
        });
    }

    var _updatePet = function (petid, petData) {
        return $http.put('/api/Pets/' + petid, petData).then(function (response) {
            return response;
        });
    }

    petsServiceFactory.getPets = _getPets;
    petsServiceFactory.getPet = _getPet;
    petsServiceFactory.savePet = _savePet;
    petsServiceFactory.foundPet = _foundPet;
    petsServiceFactory.deletePet = _deletePet;
    petsServiceFactory.updatePet = _updatePet;
    petsServiceFactory.getFoundInfo = _getFoundInfo;

    return petsServiceFactory;

}]);

app.factory('authInterceptorService', ['$q', '$location', 'localStorageService', function ($q, $location, localStorageService) {

    var authInterceptorServiceFactory = {};

    var _request = function (config) {

        config.headers = config.headers || {};

        var authData = localStorageService.get('authorizationData');
        if (authData) {
            config.headers.Authorization = 'Bearer ' + authData.token;
        }

        return config;
    }

    var _responseError = function (rejection) {
        if (rejection.status === 401) {
            $location.path('/login');
        }
        return $q.reject(rejection);
    }

    authInterceptorServiceFactory.request = _request;
    authInterceptorServiceFactory.responseError = _responseError;

    return authInterceptorServiceFactory;
}]);


app.controller('petsController', ['$scope', '$location', 'petsService', function ($scope, $location, petsService) {

    $scope.pets = [];
    $scope.order = 'ID';
    $scope.sortType = 'PublishedDate';
    $scope.sortReverse = false; 
    $scope.searchPets = '';

    $scope.deletePet = function (petID) {
        console.log(petID);
        petsService.deletePet(petID).then(function (results) {
            console.log(results);
            location.reload();
        })
    };

    petsService.getPets().then(function (results) {

        $scope.pets = results.data;

    }, function (error) {
        alert(error.data.message);
    });

    $scope.petData = {
        Name: "",
        Location: "",
        Image: "",
        Species: "",
        Breed: ""
    };

    $scope.sortLost = function () {
        
    }

    $scope.petlost = function () {

        console.log('func hit');
        var file = $scope.petData.Image;
        console.log(file);
        petsService.savePet($scope.petData).then(function (response) {

            $scope.savedSuccessfully = true;
            $scope.message = "Pet has been registered successfully.";
            alert("Pet saved.");
            location.reload();

        },
         function (response) {
             var errors = [];
             for (var key in response.data.modelState) {
                 for (var i = 0; i < response.data.modelState[key].length; i++) {
                     errors.push(response.data.modelState[key][i]);
                 }
             }
             $scope.message = "Failed to find pet due to:" + errors.join(' ');
         });
    };

}]);

var serviceBase = '/';
app.constant('ngAuthSettings', {
    apiServiceBaseUri: "/",
    clientId: 'PetQuest'
});

// This directive allows the image to be read as base64 code, so it can be stored in a database.
app.directive("fileread", [function () {
    return {
        scope: {
            fileread: "="
        },
        link: function (scope, element, attributes) {
            element.bind("change", function (changeEvent) {
                var reader = new FileReader();
                reader.onload = function (loadEvent) {
                    scope.$apply(function () {
                        scope.fileread = loadEvent.target.result;
                    });
                }
                reader.readAsDataURL(changeEvent.target.files[0]);
            });
        }
    }
}]);